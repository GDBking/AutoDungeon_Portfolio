using UnityEngine;
using UnityEngine.Localization.Settings;

public class ManaResearcher : UnitDefault
{
    [Header("마나 차단")]
    public int decManaAmount;
    public float duration;
    int DecManaAmount { get => decManaAmount + StaticManager.skillPoint[unitIdx] / 3; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.25f; }

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
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        unitComp.Bondage(Duration, skillAnimClip);
        unitComp.SetMana(-DecManaAmount);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상을 <color=yellow>{Duration}초</color> 동안 속박하고, 마나를 <color=red>{DecManaAmount}</color>만큼 감소시킵니다.";

        ENSkillDesc = $"Binds the attacking target for <color=yellow>{Duration} seconds</color> and reduces mana by <color=red>{DecManaAmount}</color>.";

        FRSkillDesc = $"Lie le cible attaquante pendant <color=yellow>{Duration} secondes</color> et réduit le mana de <color=red>{DecManaAmount}</color>.";

        ITSkillDesc = $"Vincola il bersaglio attaccante per <color=yellow>{Duration} secondi</color> e riduce il mana di <color=red>{DecManaAmount}</color>.";

        DESkillDesc = $"Bindet das angreifende Ziel für <color=yellow>{Duration} Sekunden</color> und reduziert Mana um <color=red>{DecManaAmount}</color>.";

        ESSkillDesc = $"Ata al objetivo atacante durante <color=yellow>{Duration} segundos</color> y reduce el maná en <color=red>{DecManaAmount}</color>.";

        JASkillDesc = $"攻撃中の対象を<color=yellow>{Duration}秒</color>間束縛し、マナを<color=red>{DecManaAmount}</color>だけ減少させます。";

        PT_BRSkillDesc = $"Prende o alvo atacante por <color=yellow>{Duration} segundos</color> e reduz o mana em <color=red>{DecManaAmount}</color>.";

        RUSkillDesc = $"Связывает атакующую цель на <color=yellow>{Duration} секунд</color> и уменьшает ману на <color=red>{DecManaAmount}</color>.";

        ZH_HANSSkillDesc = $"将攻击目标束缚<color=yellow>{Duration}秒</color>，并减少<color=red>{DecManaAmount}</color>点法力值。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "속박 지속 시간 <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "마나 차단량 <color=red>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Bind duration <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Mana block <color=red>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Durée de lien <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Blocage de mana <color=red>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Durata vincolo <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Blocco mana <color=red>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Fesselungsdauer <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Mana-Block <color=red>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Duración de atadura <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Bloqueo de mana <color=red>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "拘束持続時間 <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "マナブロック <color=red>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Duração de vínculo <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Bloqueio de mana <color=red>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Длительность связывания <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Блок маны <color=red>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "束缚持续时间 <color=yellow>+0.25</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "法力阻挡 <color=red>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Bind duration <color=yellow>+0.25</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Mana block <color=red>+1</color>";
    }
}