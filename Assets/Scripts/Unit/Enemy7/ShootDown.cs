using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootDown : UnitDefault
{
    [Header("격살")]
    public int count;
    public float duration;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (Target == null)
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget == null ? skillTargetPos : SkillTarget.transform.position;
        Vector2 dir = (pos - (Vector2)transform.position).normalized;
        StartCoroutine(FanShot(dir, count, 60f, 20f));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(-1f, null, target.transform.position);
        target.GetComponent<UnitDefault>().OnStunAnim(duration);
    }

    public virtual IEnumerator FanShot(Vector2 direction, int bulletCount, float fanAngle, float distance)
    {
        // 시작 위치
        Vector2 startPosition = transform.position;

        // 기본 방향 각도
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 각도 간격
        float angleStep = fanAngle / (bulletCount - 1);

        // 시작 각도
        float startAngle = baseAngle - fanAngle / 2f;

        // 생성된 탄환들
        List<GameObject> bullets = new();
        List<Vector2> targets = new();

        for (int i = 0; i < bulletCount; i++) {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // 발사 방향
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            // 도착 지점
            Vector2 targetPos = startPosition + dir * distance;

            // 회전
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(bulletObj, startPosition, rotation);
            bullet.tag = "Penetration Bullet";

            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.SetAnimatorOverrideController(skillBulletAnimClip, attackSize);
            bulletComp.unitDefault = this;
            bulletComp.enemyTag = enemyTag;

            bulletList.Add(bullet);
            bullets.Add(bullet);
            targets.Add(targetPos);
        }

        // 이동
        float duration = distance / ShotSpeed;
        float elapsedTime = 0f;
        WaitForFixedUpdate wait = new();

        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;

            for (int i = 0; i < bullets.Count; i++) {
                if (bullets[i] != null)
                    bullets[i].transform.position = Vector2.Lerp(startPosition, targets[i], t);
            }

            yield return wait;
        }

        // 도착 후 파괴
        for (int i = 0; i < bullets.Count; i++) {
            if (bullets[i] != null) {
                bullets[i].transform.position = targets[i];
                Destroy(bullets[i]);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 방향으로 유닛을 관통하는 화살 <color=yellow>{count}개</color>를 부채꼴 모양으로 발사합니다.\n" +
                    $"피격된 적들은 <color=yellow>{duration}초</color> 동안 기절합니다.";
        
        ENSkillDesc = $"Fires <color=yellow>{count} arrows</color> in a fan shape that pierce through units in the attacking direction.\n" +
                       $"Hit enemies are stunned for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Tire <color=yellow>{count} flèches</color> en forme d'éventail qui traversent les unités dans la direction de l'attaque.\n" +
                       $"Les ennemis touchés sont étourdis pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Spara <color=yellow>{count} frecce</color> a ventaglio che trapassano le unità nella direzione di attacco.\n" +
                          $"I nemici colpiti sono storditi per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Feuert <color=yellow>{count} Pfeile</color> in Fächerform ab, die Einheiten in Angriffsrichtung durchdringen.\n" +
                       $"Getroffene Feinde werden für <color=yellow>{duration} Sekunden</color> betäubt.";

        ESSkillDesc = $"Dispara <color=yellow>{count} flechas</color> en forma de abanico que atraviesan las unidades en la dirección de ataque.\n" +
                          $"Los enemigos alcanzados quedan aturdidos durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"攻撃方向にユニットを貫通する矢を<color=yellow>{count}本</color>、扇形に発射します。\n" +
                        $"命中した敵は<color=yellow>{duration}秒</color>間、気絶します。";

        PT_BRSkillDesc = $"Dispara <color=yellow>{count} flechas</color> em forma de leque que perfuram unidades na direção de ataque.\n" +
                          $"Os inimigos atingidos ficam atordoados por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Выстреливает <color=yellow>{count} стрелами</color> в виде веера, которые пронизывают юнитов в направлении атаки.\n" +
                          $"Попавшие враги оглушаются на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"以扇形发射<color=yellow>{count}支</color>穿透单位的箭矢，朝攻击方向发射。\n" +
                           $"被击中的敌人将被眩晕<color=yellow>{duration}秒</color>。";
    }
}