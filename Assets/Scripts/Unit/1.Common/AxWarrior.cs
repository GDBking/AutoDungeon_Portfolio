using UnityEngine;
using UnityEngine.Localization.Settings;

public class AxWarrior : UnitDefault
{
    [Header("파쇄")]
    public float decDefenseAmount;
    public float duration;

    float DecDefenseAmount { get => decDefenseAmount + StaticManager.skillPoint[unitIdx] * 7f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 2 * 0.5f; }

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

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();

        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, SkillTarget.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        unitComp.SetStat(unitComp.defenseStat, DecDefenseAmount, false, false, Duration);
        unitComp.SetStateBar(State.decDefense, Duration);

        CreateAttackBox(AttackPower, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 방어력을 <color=yellow>{Duration}초</color> 동안 <color=red>{DecDefenseAmount}</color>만큼 감소시키고, 기본 공격 피해를 입힙니다.";

        ENSkillDesc = $"Reduces the defense of the attacking target by <color=red>{DecDefenseAmount}</color> for <color=yellow>{Duration} seconds</color> and deals basic attack damage.";

        FRSkillDesc = $"Réduit la défense de la cible attaquante de <color=red>{DecDefenseAmount}</color> pendant <color=yellow>{Duration} secondes</color> et inflige des dégâts d'attaque de base.";

        ITSkillDesc = $"Riduce la difesa del bersaglio attaccante di <color=red>{DecDefenseAmount}</color> per <color=yellow>{Duration} secondi</color> e infligge danni da attacco base.";

        DESkillDesc = $"Reduziert die Verteidigung des angreifenden Ziels um <color=red>{DecDefenseAmount}</color> für <color=yellow>{Duration} Sekunden</color> und verursacht Grundangriffsschaden.";

        ESSkillDesc = $"Reduce la defensa del objetivo atacante en <color=red>{DecDefenseAmount}</color> durante <color=yellow>{Duration} segundos</color> e inflige daño de ataque básico.";

        JASkillDesc = $"攻撃中の対象の防御力を<color=red>{DecDefenseAmount}</color>だけ<color=yellow>{Duration}秒</color>間減少させ、基本攻撃ダメージを与えます。";

        PT_BRSkillDesc = $"Reduz a defesa do alvo atacante em <color=red>{DecDefenseAmount}</color> por <color=yellow>{Duration} segundos</color> e causa dano de ataque básico.";

        RUSkillDesc = $"Снижает защиту атакующей цели на <color=red>{DecDefenseAmount}</color> на <color=yellow>{Duration} секунд</color> и наносит урон от базовой атаки.";

        ZH_HANSSkillDesc = $"将攻击目标的防御力降低<color=red>{DecDefenseAmount}</color>，持续<color=yellow>{Duration}秒</color>，并造成基础攻击伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "방어력 감소량 <color=red>+7</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Defense reduction <color=red>+7</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Réduction de défense <color=red>+7</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Riduzione difesa <color=red>+7</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Rüstungsreduktion <color=red>+7</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Reducción de defensa <color=red>+7</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "防御力減少 <color=red>+7</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Redução de defesa <color=red>+7</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Снижение брони <color=red>+7</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "防御力降低 <color=red>+7</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Defense reduction <color=red>+7</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}