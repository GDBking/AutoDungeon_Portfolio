using UnityEngine;

public class FirstStrike : UnitDefault
{
    [Header("선격")]
    public float attackCoeff;
    public float size;

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

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * attackCoeff, null, pos, AttackStyle.range, true, true);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;
        Vector2 thisPos = Vector2.zero;
        Vector2 dir = Vector2.zero;
        float angle = 0f;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill)) {
            thisPos = transform.position;

            // 타겟 방향 계산
            dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, isSkill ? thisPos : targetPos, rotation);

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
            scale = new Vector2(size, 1f);
        effect.transform.localScale = scale;

        // 박스 캐스트 수행 (회전 적용)
        Collider2D[] hitEnemies;
        if (!isSkill)
            hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize, LayerMask.GetMask(enemyTag));
        else
            hitEnemies = Physics2D.OverlapBoxAll(thisPos + dir * effect.transform.localScale.x / 2f, effect.transform.localScale, angle, LayerMask.GetMask(enemyTag));

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
        skillDesc = $"공격 방향 <color=#AEEAFF>{size}X1</color>범위의 적들에게 공격력 <color=red>{attackCoeff}배</color>의 방어력 관통 피해를 입힙니다.";
        
        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power as defense-piercing damage to enemies in a <color=#AEEAFF>{size}X1</color> area in the attack direction.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque en tant que dégâts perforants à la défense aux ennemis dans une zone de <color=#AEEAFF>{size}X1</color> dans la direction de l'attaque.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco come danni che penetrano la difesa ai nemici in un'area di <color=#AEEAFF>{size}X1</color> nella direzione dell'attacco.";

        DESkillDesc = $"Fügt Feinden in einem <color=#AEEAFF>{size}X1</color>-Bereich in Angriffsrichtung Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft als verteidigungsdurchdringenden Schaden zu.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque como daño que penetra la defensa a los enemigos en un área de <color=#AEEAFF>{size}X1</color> en la dirección del ataque.";

        JASkillDesc = $"攻撃方向の<color=#AEEAFF>{size}X1</color>範囲の敵に攻撃力の<color=red>{attackCoeff}倍</color>の防御力貫通ダメージを与えます。";

        PT_BRSkillDesc = $"Causa daño igual a <color=red>{attackCoeff} vezes</color> o poder de ataque como dano que penetra a defesa aos inimigos em uma área de <color=#AEEAFF>{size}X1</color> na direção do ataque.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раз</color> силе атаки в виде урона, проникающего через защиту, врагам в области <color=#AEEAFF>{size}X1</color> в направлении атаки.";

        ZH_HANSSkillDesc = $"对攻击方向<color=#AEEAFF>{size}X1</color>范围内的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的防御穿透伤害。";
    }
}