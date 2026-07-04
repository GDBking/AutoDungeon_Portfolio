using UnityEngine;

public class Claw : UnitDefault
{
    [Header("갈퀴")]
    public float attackCoeff;
    public float size;
    public float incHealthAmount;
    public float healSize;
    public AnimationClip healClip;

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
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
        CreateAttackBox(AttackPower * attackCoeff, null, pos, AttackStyle.range);

        GameObject effect = Instantiate(attackObj, transform.position, Quaternion.identity);
        effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(healClip);
        effect.transform.localScale *= healSize;

        Collider2D[] hitFriends = Physics2D.OverlapCircleAll(transform.position, healSize / 2f, LayerMask.GetMask(tag));
        foreach (Collider2D friendly in hitFriends) {
            friendly.GetComponent<UnitDefault>().Healing(incHealthAmount);
        }
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;
        float angle = 0f;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

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
        if (!isSkill)
            scale *= attackSize; // 스프라이트 크기 조정
        else
            scale = new Vector2(1f, size);
        effect.transform.localScale = scale;

        // 박스 캐스트 수행 (회전 적용)
        Collider2D[] hitEnemies;
        if (!isSkill)
            hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize, LayerMask.GetMask(enemyTag));
        else
            hitEnemies = Physics2D.OverlapBoxAll(targetPos, effect.transform.localScale, angle, LayerMask.GetMask(enemyTag));

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
        skillDesc = $"공격 중인 유닛 반경 <color=#AEEAFF>1x{size}</color>범위를 할퀴어 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 본인 반경 <color=#AEEAFF>{healSize}x{healSize}</color>범위의 아군 유닛들의 체력을 <color=green>{incHealthAmount}</color>만큼 회복시킵니다.";

        ENSkillDesc = $"Claws a <color=#AEEAFF>1x{size}</color> area around the attacking unit, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power, and heals allied units in a <color=#AEEAFF>{healSize}x{healSize}</color> area around itself for <color=green>{incHealthAmount}</color>.";

        FRSkillDesc = $"Griffe une zone de <color=#AEEAFF>1x{size}</color> autour de l'unité attaquante, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque, et soigne les unités alliées dans une zone de <color=#AEEAFF>{healSize}x{healSize}</color> autour de soi de <color=green>{incHealthAmount}</color>.";

        ITSkillDesc = $"Artiglia un'area di <color=#AEEAFF>1x{size}</color> intorno all'unità attaccante, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco, e cura le unità alleate in un'area di <color=#AEEAFF>{healSize}x{healSize}</color> intorno a sé per <color=green>{incHealthAmount}</color>.";

        DESkillDesc = $"Kratzt einen Bereich von <color=#AEEAFF>1x{size}</color> um die angreifende Einheit und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft und heilt verbündete Einheiten in einem Bereich von <color=#AEEAFF>{healSize}x{healSize}</color> um sich herum um <color=green>{incHealthAmount}</color>.";

        ESSkillDesc = $"Araña un área de <color=#AEEAFF>1x{size}</color> alrededor de la unidad atacante, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque, y cura a las unidades aliadas en un área de <color=#AEEAFF>{healSize}x{healSize}</color> a su alrededor por <color=green>{incHealthAmount}</color>.";

        JASkillDesc = $"攻撃中のユニットの周囲<color=#AEEAFF>1x{size}</color>の範囲を引っかき、攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、自身の周囲<color=#AEEAFF>{healSize}x{healSize}</color>の範囲内の味方ユニットの体力を<color=green>{incHealthAmount}</color>回復します。";

        PT_BRSkillDesc = $"Arranha uma área de <color=#AEEAFF>1x{size}</color> ao redor da unidade atacante, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque, e cura unidades aliadas em uma área de <color=#AEEAFF>{healSize}x{healSize}</color> ao seu redor por <color=green>{incHealthAmount}</color>.";

        RUSkillDesc = $"Царапает область размером <color=#AEEAFF>1x{size}</color> вокруг атакующей единицы, нанося урон, равный <color=red>{attackCoeff} раза</color> силе атаки, и исцеляет союзные единицы в области размером <color=#AEEAFF>{healSize}x{healSize}</color> вокруг себя на <color=green>{incHealthAmount}</color>.";

        ZH_HANSSkillDesc = $"抓挠攻击单位周围<color=#AEEAFF>1x{size}</color>的区域，造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并治疗自身周围<color=#AEEAFF>{healSize}x{healSize}</color>区域内的友军单位<color=green>{incHealthAmount}</color>。";
    }
}