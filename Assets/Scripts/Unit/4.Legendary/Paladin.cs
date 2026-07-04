using UnityEngine;
using UnityEngine.Localization.Settings;

public class Paladin : UnitDefault
{
    [Header("성스러운 방패")]
    public float incAttackPer;
    public float duration;

    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 3f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.1f; }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        foreach (UnitDefault friend in friends) {
            GameObject effect = Instantiate(attackObj, friend.transform.position, Quaternion.identity, friend.transform);
            effect.transform.localScale *= 0.5f;
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            friend.Invincible(Duration);
            friend.SetStat(friend.attackPowerStat, IncAttackPer, true, true, Duration);
            friend.SetStateBar(State.invincible, Duration);
            friend.SetStateBar(State.incAttack, Duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 모든 아군 유닛들은 무적이 되고, 공격력이 <color=green>{IncAttackPer}%</color> 증가합니다.";
        
        ENSkillDesc = $"All allied units become invincible for <color=yellow>{Duration} seconds</color>, and their attack power increases by <color=green>{IncAttackPer}%</color>.";

        FRSkillDesc = $"Toutes les unités alliées deviennent invincibles pendant <color=yellow>{Duration} secondes</color>, et leur puissance d'attaque augmente de <color=green>{IncAttackPer}%</color>.";

        ITSkillDesc = $"Tutte le unità alleate diventano invincibili per <color=yellow>{Duration} secondi</color> e la loro potenza di attacco aumenta del <color=green>{IncAttackPer}%</color>.";

        DESkillDesc = $"Alle verbündeten Einheiten werden für <color=yellow>{Duration} Sekunden</color> unverwundbar und ihre Angriffskraft erhöht sich um <color=green>{IncAttackPer}%</color>.";

        ESSkillDesc = $"Todas las unidades aliadas se vuelven invencibles durante <color=yellow>{Duration} segundos</color>, y su poder de ataque aumenta en <color=green>{IncAttackPer}%</color>.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、すべての味方ユニットが無敵になり、攻撃力が<color=green>{IncAttackPer}%</color>増加します。";

        PT_BRSkillDesc = $"Todas as unidades aliadas se tornam invencíveis por <color=yellow>{Duration} segundos</color>, e seu poder de ataque aumenta em <color=green>{IncAttackPer}%</color>.";

        RUSkillDesc = $"Все союзные юниты становятся неуязвимыми на <color=yellow>{Duration} секунд</color>, и их сила атаки увеличивается на <color=green>{IncAttackPer}%</color>.";

        ZH_HANSSkillDesc = $"所有友军单位在<color=yellow>{Duration}秒</color>内变得无敌，攻击力提高<color=green>{IncAttackPer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+3%p</color>\n" +
                   "지속 시간 <color=yellow>+0.1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack power increase <color=green>+3%p</color>\n" +
                   "Duration <color=yellow>+0.1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de l'attaque <color=green>+3%p</color>\n" +
                   "Durée <color=yellow>+0.1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento potenza attacco <color=green>+3%p</color>\n" +
                   "Durata <color=yellow>+0.1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffskraft-Erhöhung <color=green>+3%p</color>\n" +
                   "Dauer <color=yellow>+0.1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de poder de ataque <color=green>+3%p</color>\n" +
                   "Duración <color=yellow>+0.1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+3%p</color>\n" +
                   "持続時間 <color=yellow>+0.1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de ataque <color=green>+3%p</color>\n" +
                   "Duração <color=yellow>+0.1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение атаки <color=green>+3%p</color>\n" +
                   "Длительность <color=yellow>+0.1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力增加 <color=green>+3%p</color>\n" +
                   "持续时间 <color=yellow>+0.1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack power increase <color=green>+3%p</color>\n" +
               "Duration <color=yellow>+0.1</color>";
    }
}