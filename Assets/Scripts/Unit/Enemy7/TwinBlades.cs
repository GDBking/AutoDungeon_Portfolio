using System.Collections;
using UnityEngine;

public class TwinBlades : UnitDefault
{
    [Header("쌍날")]
    public float attackCoeff;

    int count;
    readonly WaitForSeconds wait1 = new(1f);
    readonly WaitForSeconds wait01 = new(0.1f);

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        for (int i = 1; i <= 2; i++) {
            count = i;
            StartCoroutine(OnFire(null, Vector2.zero, true, true));
        }
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position, isPenetration: true);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = transform.position; // 시작 위치
        startPos += new Vector2(curFlipX ? -0.5f : 0.5f, count == 1 ? 0.5f : -0.5f);

        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, Quaternion.identity);
        bulletList.Add(bullet);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(isSkill ? skillBulletAnimClip : bulletAnimClip, attackSize);

        if (!curFlipX) {
            Vector2 scale = bullet.transform.localScale;
            scale.x *= -1f;
            bullet.transform.localScale = scale;
        }

        yield return wait1;

        while (Target == null)
            yield return wait01;

        targetPos = startPos + ((Vector2)Target.transform.position - startPos).normalized * 20f;

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        bullet.transform.localRotation = rotation;
        if (bullet.transform.localScale.x < 0) {
            Vector2 scale = bullet.transform.localScale;
            scale.x *= -1f;
            bullet.transform.localScale = scale;
        }

        bullet.tag = "Penetration Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        bulletComp.unitDefault = this;
        bulletComp.target = target;
        bulletComp.enemyTag = enemyTag;

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

        if (bullet != null) {
            // 마지막 위치를 타겟의 위치로 설정 (정확한 위치 보정)
            bullet.transform.position = targetPos;

            // 발사체 제거
            Destroy(bullet);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"유닛과 방어력을 관통하는 칼날 두 개를 소환해 공격 중인 방향으로 날립니다.\n" +
                    $"피격된 적들은 공격력 <color=red>{attackCoeff}배</color>의 방어력 관통 피해를 입습니다.";
        
        ENSkillDesc = $"Summons two blades that pierce through units and defenses, flying in the direction of the attack.\n" +
                      $"Hit enemies take defense-piercing damage equal to <color=red>{attackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Invoque deux lames qui traversent les unités et les défenses, volant dans la direction de l'attaque.\n" +
                        $"Les ennemis touchés subissent des dégâts perforants égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Evoca due lame che attraversano unità e difese, volando nella direzione dell'attacco.\n" +
                          $"I nemici colpiti subiscono danni che penetrano la difesa pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Beschwört zwei Klingen, die Einheiten und Verteidigungen durchdringen und in Angriffsrichtung fliegen.\n" +
                      $"Getroffene Feinde erleiden verteidigungsdurchdringenden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Invoca dos hojas que atraviesan unidades y defensas, volando en la dirección del ataque.\n" +
                        $"Los enemigos alcanzados reciben daño que penetra la defensa igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"ユニットと防御を貫通する刃を2つ召喚し、攻撃方向に飛ばします。\n" +
                      $"命中した敵は攻撃力の<color=red>{attackCoeff}倍</color>の防御貫通ダメージを受けます。";

        PT_BRSkillDesc = $"Invoca duas lâminas que perfuram unidades e defesas, voando na direção do ataque.\n" +
                         $"Os inimigos atingidos recebem dano que penetra a defesa igual a <color=red>{attackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"Призывает два клинка, которые пронизывают юнитов и оборону, летя в направлении атаки.\n" +
                        $"Попавшие враги получают урон, игнорирующий защиту, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"召唤两把穿透单位和防御的刀刃，朝攻击方向飞去。\n" +
                            $"被击中的敌人会受到相当于攻击力<color=red>{attackCoeff}倍</color>的无视防御伤害。";
    }
}