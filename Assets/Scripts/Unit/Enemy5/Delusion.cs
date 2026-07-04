using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delusion : UnitDefault
{
    [Header("미혹")]
    public float attackCoeff;
    public int count;
    public float subAttackCoeff;
    public int duration;

    readonly List<(float dist, UnitDefault enemy)> unitDist = new();
    bool isDelusion;
    int bulletCnt;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target) || isDelusion)
            return;

        isDelusion = true;
        bulletCnt = 0;
        unitDist.Clear();
        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null) {
            isDelusion = false;
            return;
        }

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        if (target == SkillTarget) {
            UnitDefault unitComp = target.GetComponent<UnitDefault>();
            skillTargetPos = target.transform.position;

            unitComp.Enthrallment(duration);
            CreateAttackBox(AttackPower * attackCoeff, target, skillTargetPos);

            foreach (UnitDefault enemy in friends) {
                if (enemy == unitComp) continue;

                float dist = Vector2.Distance(skillTargetPos, enemy.transform.position);
                unitDist.Add((dist, enemy));
            }

            // 거리 기준 오름차순 정렬
            unitDist.Sort((a, b) => a.dist.CompareTo(b.dist));

            for (int i = 0; i < count && i < unitDist.Count; i++) {
                StartCoroutine(OnFire(unitDist[i].enemy.gameObject, unitDist[i].enemy.transform.position));
            }
        }
        else {
            CreateAttackBox(AttackPower * subAttackCoeff, target, target.transform.position);

            if (++bulletCnt == count)
                isDelusion = false;
        }
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos;
        if (!isSkill || unitDist.Count == 0)
            startPos = transform.position;
        else
            startPos = skillTargetPos;

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation);
        bulletList.Add(bullet);
        if (isSkill)
            bullet.tag = "Skill Bullet";
        if (isPenetration)
            bullet.tag = "Penetration Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(isSkill ? skillBulletAnimClip : bulletAnimClip, attackSize);
        bulletComp.unitDefault = this;
        bulletComp.target = target;
        if (isPenetration)
            bulletComp.enemyTag = enemyTag;

        if (!isSkill)
            SoundPlay(attackSoundClip);
        else
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

            if (isSkill) {
                if (unitDist.Count == 0)
                    isDelusion = false;
                else if (++bulletCnt == count)
                    isDelusion = false;
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{duration}초</color> 동안 매혹시킵니다.\n" +
                    $"적중 시 대상으로부터 가장 가까운 <color=yellow>{count}명</color>의 적에게 공격력 <color=red>{subAttackCoeff}배</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power to the attacking target and enthralls them for <color=yellow>{duration} seconds</color>.\n" +
                      $"Upon hit, deals damage equal to <color=red>{subAttackCoeff} times</color> the attack power to the <color=yellow>{count} closest</color> enemies from the target.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque à la cible attaquée et l'enchante pendant <color=yellow>{duration} secondes</color>.\n" +
                        $"À l'impact, inflige des dégâts égaux à <color=red>{subAttackCoeff} fois</color> la puissance d'attaque aux <color=yellow>{count} plus proches</color> ennemis de la cible.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al bersaglio attaccante e lo incanta per <color=yellow>{duration} secondi</color>.\n" +
                      $"Al momento dell'impatto, infligge danni pari a <color=red>{subAttackCoeff} volte</color> la potenza d'attacco ai <color=yellow>{count} nemici più vicini</color> al bersaglio.";

        DESkillDesc = $"Fügt dem angreifenden Ziel Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft zu und bezaubert es für <color=yellow>{duration} Sekunden</color>.\n" +
                        $"Bei Treffer fügt es den <color=yellow>{count} nächstgelegenen</color> Feinden des Ziels Schaden in Höhe von <color=red>{subAttackCoeff} mal</color> der Angriffskraft zu.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al objetivo atacado y lo cautiva durante <color=yellow>{duration} segundos</color>.\n" +
                        $"Al impactar, inflige daño igual a <color=red>{subAttackCoeff} veces</color> el poder de ataque a los <color=yellow>{count} enemigos más cercanos</color> al objetivo.";

        JASkillDesc = $"攻撃中の対象に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{duration}秒</color>間魅了します。\n" +
                       $"命中時に対象から最も近い<color=yellow>{count}体</color>の敵に攻撃力の<color=red>{subAttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque ao alvo atacado e o encanta por <color=yellow>{duration} segundos</color>.\n" +
                         $"Ao atingir, causa dano igual a <color=red>{subAttackCoeff} vezes</color> o poder de ataque aos <color=yellow>{count} inimigos mais próximos</color> do alvo.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> силе атаки атакуемой цели и очаровывает её на <color=yellow>{duration} секунд</color>.\n" +
                        $"При попадании наносит урон, равный <color=red>{subAttackCoeff} раза</color> силе атаки <color=yellow>{count} ближайшим</color> врагам от цели.";

        ZH_HANSSkillDesc = $"对被攻击目标造成等同于攻击力<color=red>{attackCoeff}倍</color>的伤害，并使其迷惑<color=yellow>{duration}秒</color>。\n" +
                          $"命中时，对目标<color=yellow>最近的{count}个</color>敌人造成等同于攻击力<color=red>{subAttackCoeff}倍</color>的伤害。";
    }
}