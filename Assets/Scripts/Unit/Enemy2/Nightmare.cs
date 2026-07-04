using UnityEngine;

public class Nightmare : UnitDefault
{
    [Header("악몽")]
    public float attackCoeff;
    public float decDefenseAmount;
    public float poisonDamage;
    public int count;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        StartCoroutine(OnFire(SkillTarget, pos, true, true));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);

        if (unitComp.isDeath)
            return;

        unitComp.Poison(poisonDamage, count, this);
        unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, count);
        unitComp.SetStateBar(State.decDefense, count);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect;
        if (!isSkill)
            effect = Instantiate(attackObj, targetPos, rotation);
        else
            effect = Instantiate(attackObj, target.transform);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1f : 1f;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize / 2f, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            if (attackStyle == AttackStyle.single) {
                foreach (Collider2D enemy in hitEnemies) {
                    if (enemy.gameObject == target) {
                        damage = target.GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
                        break;
                    }
                }
            }
            else {
                for (int i = hitEnemies.Length - 1; i >= 0; i--)
                    damage += hitEnemies[i].GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
            }

            // 피흡 계산
            float lifeSteal = damage * LifeStealPer / 100f;
            Healing(lifeSteal);

            DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
        }
        return damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 방향으로 유닛을 관통하는 악령을 날립니다.\n" +
                    $"악령에 닿은 적들은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입고, <color=yellow>{count}초</color> 동안 방어력이 <color=red>{decDefenseAmount}</color>만큼 감소하며 독 효과를 입습니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";

        ENSkillDesc = $"Launches a wraith that penetrates units in the attacking direction.\n" +
                      $"Enemies hit by the wraith take damage equal to <color=red>{attackCoeff} times</color> the attack power, have their defense reduced by <color=red>{decDefenseAmount}</color> for <color=yellow>{count} seconds</color>, and are poisoned.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"Lance un spectre qui pénètre les unités dans la direction de l'attaque.\n" +
                      $"Les ennemis touchés par le spectre subissent des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque, leur défense est réduite de <color=red>{decDefenseAmount}</color> pendant <color=yellow>{count} secondes</color> et ils sont empoisonnés.\n" +
                      $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Lancia uno spettro che penetra le unità nella direzione di attacco.\n" +
                      $"I nemici colpiti dallo spettro subiscono danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco, la loro difesa è ridotta di <color=red>{decDefenseAmount}</color> per <color=yellow>{count} secondi</color> e sono avvelenati.\n" +
                      $"Danno da veleno al secondo: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Startet einen Wraith, der Einheiten in Angriffsrichtung durchdringt.\n" +
                      $"Getroffene Gegner erleiden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft, ihre Verteidigung wird für <color=yellow>{count} Sekunden</color> um <color=red>{decDefenseAmount}</color> reduziert und sie werden vergiftet.\n" +
                      $"Giftschaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Lanza un espectro que penetra las unidades en la dirección de ataque.\n" +
                      $"Los enemigos alcanzados por el espectro reciben daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque, su defensa se reduce en <color=red>{decDefenseAmount}</color> durante <color=yellow>{count} segundos</color> y quedan envenenados.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"攻撃方向にユニットを貫通する幽霊を放ちます。\n" +
                      $"幽霊に当たった敵は攻撃力の<color=red>{attackCoeff}倍</color>のダメージを受け、<color=yellow>{count}秒</color>間防御力が<color=red>{decDefenseAmount}</color>減少し、毒状態になります。\n" +
                      $"毒ダメージ（秒）：<color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Lança um espectro que penetra unidades na direção do ataque.\n" +
                          $"Os inimigos atingidos pelo espectro recebem dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque, têm sua defesa reduzida em <color=red>{decDefenseAmount}</color> por <color=yellow>{count} segundos</color> e são envenenados.\n" +
                          $"Dano de veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"Выпускает призрака, который проходит через юниты в направлении атаки.\n" +
                      $"Враги, поражённые призраком, получают урон, равный <color=red>{attackCoeff} раза</color> силы атаки, их защита уменьшается на <color=red>{decDefenseAmount}</color> на <color=yellow>{count} секунд</color> и они отравлены.\n" +
                      $"Ядовитый урон в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"朝攻击方向射穿单位的幽灵。\n" +
                           $"被幽灵击中的敌人承受相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，防御在<color=yellow>{count}秒</color>内减少<color=red>{decDefenseAmount}</color>，并受到中毒效果。\n" +
                           $"每秒中毒伤害：<color=red>{poisonDamage}</color>";
    }
}
