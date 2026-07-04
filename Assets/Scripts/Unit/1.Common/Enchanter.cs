using UnityEngine;
using UnityEngine.Localization.Settings;

public class Enchanter : UnitDefault
{
    [Header("강화")]
    public float incAttackPer;
    public float duration;

    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 10f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 3 * 0.5f; }

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 1 || enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        RandomFriendlyUnit(friends);
        if (FriendlyTarget == null)
            return;

        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, FriendlyTarget.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        UnitDefault unitComp = FriendlyTarget.GetComponent<UnitDefault>();
        unitComp.SetStat(unitComp.attackPowerStat, IncAttackPer, true, true, Duration);
        unitComp.SetStateBar(State.incAttack, Duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인을 제외한 랜덤한 아군 유닛 한 명의 공격력을 <color=yellow>{Duration}초</color> 동안 <color=green>{IncAttackPer}%</color> 상승시킵니다.";

        ENSkillDesc = $"Increases the attack power of a random allied unit, excluding yourself, by <color=green>{IncAttackPer}%</color> for <color=yellow>{Duration} seconds</color>.";

        FRSkillDesc = $"Augmente la puissance d'attaque d'une unité alliée aléatoire, vous excluant, de <color=green>{IncAttackPer}%</color> pendant <color=yellow>{Duration} secondes</color>.";

        ITSkillDesc = $"Aumenta la potenza di attacco di un'unità alleata casuale, escludendo te stesso, del <color=green>{IncAttackPer}%</color> per <color=yellow>{Duration} secondi</color>.";

        DESkillDesc = $"Erhöht die Angriffskraft einer zufälligen verbündeten Einheit, ausgenommen dich selbst, um <color=green>{IncAttackPer}%</color> für <color=yellow>{Duration} Sekunden</color>.";

        ESSkillDesc = $"Aumenta el poder de ataque de una unidad aliada aleatoria, excluyéndote a ti mismo, en <color=green>{IncAttackPer}%</color> durante <color=yellow>{Duration} segundos</color>.";

        JASkillDesc = $"自分を除くランダムな味方ユニット1体の攻撃力を<color=green>{IncAttackPer}%</color>、<color=yellow>{Duration}秒</color>間上昇させます。";

        PT_BRSkillDesc = $"Aumenta o poder de ataque de uma unidade aliada aleatória, excluindo você mesmo, em <color=green>{IncAttackPer}%</color> por <color=yellow>{Duration} segundos</color>.";

        RUSkillDesc = $"Увеличивает силу атаки случайного союзного юнита, исключая вас, на <color=green>{IncAttackPer}%</color> на <color=yellow>{Duration} секунд</color>.";

        ZH_HANSSkillDesc = $"将除自己外的随机友军单位的攻击力提高<color=green>{IncAttackPer}%</color>，持续<color=yellow>{Duration}秒</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack increase <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation d'attaque <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento attacco <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffserhöhung <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Incremento de ataque <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de ataque <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение атаки <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力提升 <color=green>+10%p</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack increase <color=green>+10%p</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}