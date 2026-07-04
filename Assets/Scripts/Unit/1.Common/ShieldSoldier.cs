using UnityEngine;
using UnityEngine.Localization.Settings;

public class ShieldSoldier : UnitDefault
{
    [Header("강타")]
    public float duration;

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

        CreateAttackBox(AttackPower, SkillTarget, SkillTarget.transform.position, isSkill: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상을 <color=yellow>{Duration}초</color> 동안 기절시키고, 기본 공격 피해를 입힙니다.";

        ENSkillDesc = $"Stuns the attacking target for <color=yellow>{Duration} seconds</color> and deals basic attack damage.";

        FRSkillDesc = $"Étourdit la cible attaquante pendant <color=yellow>{Duration} secondes</color> et inflige des dégâts d'attaque de base.";

        ITSkillDesc = $"Stordisce il bersaglio attaccante per <color=yellow>{Duration} secondi</color> e infligge danni da attacco base.";

        DESkillDesc = $"Betäubt das angreifende Ziel für <color=yellow>{Duration} Sekunden</color> und verursacht Grundangriffsschaden.";

        ESSkillDesc = $"Aturde al objetivo atacante durante <color=yellow>{Duration} segundos</color> e inflige daño de ataque básico.";

        JASkillDesc = $"攻撃中の対象を<color=yellow>{Duration}秒</color>間気絶させ、基本攻撃ダメージを与えます。";

        PT_BRSkillDesc = $"Aturde o alvo atacante por <color=yellow>{Duration} segundos</color> e causa dano de ataque básico.";

        RUSkillDesc = $"Оглушает атакующую цель на <color=yellow>{Duration} секунд</color> и наносит урон от базовой атаки.";

        ZH_HANSSkillDesc = $"使攻击目标眩晕<color=yellow>{Duration}秒</color>，并造成基础攻击伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "기절 지속 시간 <color=yellow>+0.2</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Stun duration <color=yellow>+0.2</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Durée d'étourdissement <color=yellow>+0.2</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Durata stordimento <color=yellow>+0.2</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Betäubungsdauer <color=yellow>+0.2</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Duración aturdimiento <color=yellow>+0.2</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "スタン持続時間 <color=yellow>+0.2</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Duração do atordoamento <color=yellow>+0.2</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Длительность оглушения <color=yellow>+0.2</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "眩晕持续时间 <color=yellow>+0.2</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Stun duration <color=yellow>+0.2</color>";
    }
}