using UnityEngine;

public class Precipitation : UnitDefault
{
    [Header("침전")]
    public float attackCoeff;
    public float decDefenseAmount;
    public float duration;
    public float size;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(size / 2f, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, transform.position, AttackStyle.range);
        attackSize = 0.5f;
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
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

        // 스킬 이펙트가 유닛의 뒤에 보이도록 레이어 재설정
        if (isSkill) {
            effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
        }

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1 : 1;
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
                for (int i = hitEnemies.Length - 1; i >= 0; i--) { 
                    UnitDefault unitComp = hitEnemies[i].GetComponent<UnitDefault>();
                    unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, duration);
                    unitComp.SetStateBar(State.decDefense, duration);
                    damage += unitComp.Hit(attackPower, CriticalPer, this, isPenetration);
                }
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
        skillDesc = $"본인 반경 <color=#AEEAFF>{size}X{size}</color>범위에 있는 적들의 방어력을 <color=yellow>{duration}초</color> 동안 <color=red>{decDefenseAmount}</color>만큼 감소시키고, 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Reduces the defense of enemies within a <color=#AEEAFF>{size}X{size}</color> area around this unit by <color=red>{decDefenseAmount}</color> for <color=yellow>{duration} seconds</color> and deals damage equal to <color=red>{attackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Reduit la défense des ennemis dans une zone de <color=#AEEAFF>{size}X{size}</color> autour de cette unité de <color=red>{decDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color> et inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Riduce la difesa dei nemici in un'area di <color=#AEEAFF>{size}X{size}</color> intorno a questa unità di <color=red>{decDefenseAmount}</color> per <color=yellow>{duration} secondi</color> e infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Reduziert die Verteidigung von Gegnern in einem <color=#AEEAFF>{size}X{size}</color>-Bereich um diese Einheit um <color=red>{decDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color> und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Reduce la defensa de los enemigos dentro de un área de <color=#AEEAFF>{size}X{size}</color> alrededor de esta unidad en <color=red>{decDefenseAmount}</color> durante <color=yellow>{duration} segundos</color> y causa daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"このユニットの周囲<color=#AEEAFF>{size}X{size}</color>範囲にいる敵の防御力を<color=yellow>{duration}秒</color>間<color=red>{decDefenseAmount}</color>だけ減少させ、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Reduz a defesa dos inimigos dentro de uma área de <color=#AEEAFF>{size}X{size}</color> ao redor desta unidade em <color=red>{decDefenseAmount}</color> por <color=yellow>{duration} segundos</color> e causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"Уменьшает защиту врагов в области <color=#AEEAFF>{size}X{size}</color> вокруг этой единицы на <color=red>{decDefenseAmount}</color> на <color=yellow>{duration} секунд</color> и наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"在此单位周围<color=#AEEAFF>{size}X{size}</color>范围内，降低敌人的防御力<color=red>{decDefenseAmount}</color>，持续<color=yellow>{duration}秒</color>，并造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}
