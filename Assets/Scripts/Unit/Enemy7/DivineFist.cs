using System.Collections;
using UnityEngine;

public class DivineFist : UnitDefault
{
    [Header("천권")]
    public float attackCoeff;
    public float decSpeedPer;
    public float bleedingDamage;
    public int count;
    public float size;


    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = Target.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget == null ? skillTargetPos : SkillTarget.transform.position;

        StartCoroutine(OnFire(null, pos));
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = targetPos + (Vector2.up * 5f);

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation);
        bullet.transform.localScale *= size;
        bulletList.Add(bullet);
        bullet.tag = "Skill Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(skillBulletAnimClip, attackSize);
        bulletComp.unitDefault = this;

        SoundPlay(skillSoundClip);

        WaitForFixedUpdate wait = new();
        // 발사체가 타겟으로 이동하는 로직
        while (elapsedTime < duration) {
            if (bullet == null)
                yield break;

            // 경과 시간 업데이트
            elapsedTime += Time.fixedDeltaTime;

            // 현재 시간에 비례하여 이동할 거리 계산
            float t = elapsedTime / duration;

            // 발사체의 위치 계산 (Lerp 사용)
            bullet.transform.position = Vector2.Lerp(startPos, targetPos, t);

            // 다음 프레임 대기
            yield return wait;
        }
        CameraShake.instance.Shake(1f, 2f);

        if (bullet != null) {
            // 마지막 위치를 타겟의 위치로 설정 (정확한 위치 보정)
            bullet.transform.position = targetPos;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, size / 2f, LayerMask.GetMask(enemyTag));
            foreach (Collider2D enemy in hitEnemies) {
                UnitDefault unitComp = enemy.GetComponent<UnitDefault>();
                unitComp.SetStat(unitComp.speedStat, decSpeedPer, true, false, count);
                unitComp.SetStateBar(State.decSpeed, count);
                unitComp.Bleeding(bleedingDamage, count, this);
            }

            attackSize = size;
            CreateAttackBox(AttackPower * attackCoeff, null, targetPos, AttackStyle.range);
            attackSize = 0.5f;

            // 발사체 제거
            Destroy(bullet);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"하늘에서 <color=#AEEAFF>{size}X{size}</color>크기의 주먹이 내려와 피격된 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{count}초</color> 동안 이동 속도를 <color=red>{decSpeedPer}%</color> 감소시키며 출혈을 입힙니다.\n" +
                    $"초당 출혈 데미지: <color=red>{bleedingDamage}</color>";
        
        ENSkillDesc = $"A fist of size <color=#AEEAFF>{size}X{size}</color> descends from the sky, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to struck enemies, reducing their movement speed by <color=red>{decSpeedPer}%</color> for <color=yellow>{count} seconds</color>, and inflicting bleeding.\n" +
                       $"Bleeding damage per second: <color=red>{bleedingDamage}</color>";

        FRSkillDesc = $"Un poing de taille <color=#AEEAFF>{size}X{size}</color> descend du ciel, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis touchés, réduisant leur vitesse de déplacement de <color=red>{decSpeedPer}%</color> pendant <color=yellow>{count} secondes</color> et infligeant une hémorragie.\n" +
                        $"Dégâts d'hémorragie par seconde : <color=red>{bleedingDamage}</color>";

        ITSkillDesc = $"Un pugno di dimensioni <color=#AEEAFF>{size}X{size}</color> discende dal cielo, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici colpiti, riducendo la loro velocità di movimento del <color=red>{decSpeedPer}%</color> per <color=yellow>{count} secondi</color> e infliggendo sanguinamento.\n" +
                          $"Danno da sanguinamento al secondo: <color=red>{bleedingDamage}</color>";

        DESkillDesc = $"Eine Faust der Größe <color=#AEEAFF>{size}X{size}</color> senkt sich vom Himmel herab und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft bei getroffenen Feinden, reduziert deren Bewegungsgeschwindigkeit um <color=red>{decSpeedPer}%</color> für <color=yellow>{count} Sekunden</color> und verursacht Blutung.\n" +
                       $"Blutungsschaden pro Sekunde: <color=red>{bleedingDamage}</color>";

        ESSkillDesc = $"Un puño de tamaño <color=#AEEAFF>{size}X{size}</color> desciende del cielo, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos golpeados, reduciendo su velocidad de movimiento en <color=red>{decSpeedPer}%</color> durante <color=yellow>{count} segundos</color> e infligiendo sangrado.\n" +
                          $"Daño por sangrado por segundo: <color=red>{bleedingDamage}</color>";

        JASkillDesc = $"サイズ<color=#AEEAFF>{size}X{size}</color>の拳が天から降り注ぎ、命中した敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{count}秒</color>間、移動速度を<color=red>{decSpeedPer}%</color>減少させ、出血を与えます。\n" +
                        $"毎秒の出血ダメージ: <color=red>{bleedingDamage}</color>";

        PT_BRSkillDesc = $"Um punho de tamanho <color=#AEEAFF>{size}X{size}</color> desce do céu, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos atingidos, reduzindo sua velocidade de movimento em <color=red>{decSpeedPer}%</color> por <color=yellow>{count} segundos</color> e causando sangramento.\n" +
                          $"Dano por sangramento por segundo: <color=red>{bleedingDamage}</color>";

        RUSkillDesc = $"Кулак размером <color=#AEEAFF>{size}X{size}</color> опускается с неба, нанося урон, равный <color=red>{attackCoeff} раз</color> силе атаки пораженным врагам, снижая их скорость передвижения на <color=red>{decSpeedPer}%</color> на <color=yellow>{count} секунд</color> и вызывая кровотечение.\n" +
                        $"Урон от кровотечения в секунду: <color=red>{bleedingDamage}</color>";

        ZH_HANSSkillDesc = $"一个大小为<color=#AEEAFF>{size}X{size}</color>的拳头从天而降，对被击中的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，在<color=yellow>{count}秒</color>内将其移动速度降低<color=red>{decSpeedPer}%</color>并造成出血。\n" +
                          $"每秒出血伤害：<color=red>{bleedingDamage}</color>";
    }
}