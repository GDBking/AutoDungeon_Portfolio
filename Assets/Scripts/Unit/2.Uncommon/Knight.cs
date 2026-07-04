using UnityEngine;
using UnityEngine.Localization.Settings;

public class Knight : UnitDefault
{
    [Header("가로베기")]
    public float attackCoeff;
    public float size;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.15f; }
    float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.1f; }

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
        CreateAttackBox(AttackPower * AttackCoeff, null, pos, AttackStyle.range);
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
            scale = new Vector2(1f, Size);
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
        skillDesc = $"공격 방향 <color=#AEEAFF>1X{Size}</color>범위 안의 적들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Deals damage equal to <color=red>{AttackCoeff} times</color> the attack power to enemies in a <color=#AEEAFF>1X{Size}</color> area in the attack direction.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque aux ennemis dans une zone de <color=#AEEAFF>1X{Size}</color> dans la direction de l'attaque.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco ai nemici in un'area di <color=#AEEAFF>1X{Size}</color> nella direzione dell'attacco.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft an Feinden in einem <color=#AEEAFF>1X{Size}</color>-Bereich in Angriffsrichtung.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque a los enemigos en un área de <color=#AEEAFF>1X{Size}</color> en la dirección del ataque.";

        JASkillDesc = $"攻撃方向の<color=#AEEAFF>1X{Size}</color>範囲内の敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Inflige dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque aos inimigos em uma área de <color=#AEEAFF>1X{Size}</color> na direção do ataque.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{AttackCoeff} раза</color> силы атаки врагам в области <color=#AEEAFF>1X{Size}</color> в направлении атаки.";

        ZH_HANSSkillDesc = $"对攻击方向<color=#AEEAFF>1X{Size}</color>范围内的敌人造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.15</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.1</color>";
    }
}