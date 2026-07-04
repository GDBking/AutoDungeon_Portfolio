using UnityEngine;
using UnityEngine.Localization.Settings;

public class Soldier : UnitDefault
{
    [Header("내려치기")]
    public float attackCoeff;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.3f; }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);

        CreateAttackBox(AttackPower * AttackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{AttackCoeff} times</color> the attack power to the attacking target.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque à la cible attaquante.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco al bersaglio attaccante.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft am angreifenden Ziel.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque al objetivo atacante.";

        JASkillDesc = $"攻撃中の対象に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Inflige dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque ao alvo atacante.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{AttackCoeff} раза</color> силы атаки атакующей цели.";

        ZH_HANSSkillDesc = $"对攻击目标造成相当于<color=red>{AttackCoeff}倍</color>攻击力的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.3</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.3</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.3</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.3</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.3</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.3</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.3</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.3</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.3</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.3</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.3</color>";
    }
}