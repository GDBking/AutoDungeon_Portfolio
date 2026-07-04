using UnityEngine;
using UnityEngine.Localization.Settings;

public class ShieldKnight : UnitDefault
{
    [Header("방패 강타")]
    public float attackCoeff;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.2f; }

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
        SkillTarget.GetComponent<UnitDefault>().OnStunAnim(Duration);
        CreateAttackBox(AttackPower * AttackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, <color=yellow>{Duration}초</color> 동안 기절시킵니다.";
        
        ENSkillDesc = $"Deals <color=red>{AttackCoeff}x</color> attack damage to the target being attacked and stuns them for <color=yellow>{Duration} seconds</color>.";

        FRSkillDesc = $"Inflige des dégâts d'attaque de <color=red>{AttackCoeff}x</color> à la cible attaquée et l'étourdit pendant <color=yellow>{Duration} secondes</color>.";

        ITSkillDesc = $"Infligge danni d'attacco pari a <color=red>{AttackCoeff}x</color> al bersaglio attaccato e lo stordisce per <color=yellow>{Duration} secondi</color>.";

        DESkillDesc = $"Verursacht <color=red>{AttackCoeff}x</color> Angriffsschaden am angegriffenen Ziel und betäubt es für <color=yellow>{Duration} Sekunden</color>.";

        ESSkillDesc = $"Inflige <color=red>{AttackCoeff}x</color> de daño de ataque al objetivo atacado y lo aturde durante <color=yellow>{Duration} segundos</color>.";

        JASkillDesc = $"攻撃中の対象に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、<color=yellow>{Duration}秒</color>間気絶させます。";

        PT_BRSkillDesc = $"Causa <color=red>{AttackCoeff}x</color> de dano de ataque ao alvo atacado e o atordoa por <color=yellow>{Duration} segundos</color>.";

        RUSkillDesc = $"Наносит <color=red>{AttackCoeff}x</color> урона от атаки атакуемой цели и оглушает её на <color=yellow>{Duration} секунд</color>.";

        ZH_HANSSkillDesc = $"对被攻击目标造成<color=red>{AttackCoeff}倍</color>的攻击伤害，并使其眩晕<color=yellow>{Duration}秒</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "기절 지속 시간 <color=yellow>+0.2</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "Stun duration <color=yellow>+0.2</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Durée d'étourdissement <color=yellow>+0.2</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Durata stordimento <color=yellow>+0.2</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Betäubungsdauer <color=yellow>+0.2</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Duración aturdimiento <color=yellow>+0.2</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "気絶持続時間 <color=yellow>+0.2</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Duração atordoamento <color=yellow>+0.2</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "Длительность оглушения <color=yellow>+0.2</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "眩晕持续时间 <color=yellow>+0.2</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "Stun duration <color=yellow>+0.2</color>";
    }
}