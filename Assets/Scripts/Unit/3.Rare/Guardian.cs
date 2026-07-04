using UnityEngine;
using UnityEngine.Localization.Settings;

public class Guardian : UnitDefault
{
    [Header("수호")]
    public float size;
    public float duration;

    float Size { get => size + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.1f; }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        attackSize = Size;
        CreateAttackBox(-1f, null, transform.position);
        attackSize = 0.5f;
        Collider2D[] hitFriends = Physics2D.OverlapCircleAll(transform.position, Size / 2f, LayerMask.GetMask(tag));

        SoundPlay(skillSoundClip);
        foreach (Collider2D friendly in hitFriends)
            friendly.GetComponent<UnitDefault>().Invincible(Duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{Size}X{Size}</color>범위의 아군들을 수호하여 <color=yellow>{Duration}초</color> 동안 무적 상태로 만듭니다.";
        
        ENSkillDesc = $"Protects allies within a <color=#AEEAFF>{Size}X{Size}</color> area around yourself, making them invincible for <color=yellow>{Duration} seconds</color>.";

        FRSkillDesc = $"Protège les alliés dans une zone de <color=#AEEAFF>{Size}X{Size}</color> autour de vous, les rendant invincibles pendant <color=yellow>{Duration} secondes</color>.";

        ITSkillDesc = $"Protegge gli alleati in un'area di <color=#AEEAFF>{Size}X{Size}</color> intorno a te, rendendoli invincibili per <color=yellow>{Duration} secondi</color>.";

        DESkillDesc = $"Schützt Verbündete in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich um dich herum und macht sie für <color=yellow>{Duration} Sekunden</color> unverwundbar.";

        ESSkillDesc = $"Protege a los aliados dentro de un área de <color=#AEEAFF>{Size}X{Size}</color> alrededor de ti, haciéndolos invencibles durante <color=yellow>{Duration} segundos</color>.";

        JASkillDesc = $"自身の周囲<color=#AEEAFF>{Size}X{Size}</color>範囲の味方を守り、<color=yellow>{Duration}秒</color>間無敵状態にします。";

        PT_BRSkillDesc = $"Protege os aliados dentro de uma área de <color=#AEEAFF>{Size}X{Size}</color> ao seu redor, tornando-os invencíveis por <color=yellow>{Duration} segundos</color>.";

        RUSkillDesc = $"Защищает союзников в области <color=#AEEAFF>{Size}X{Size}</color> вокруг себя, делая их неуязвимыми на <color=yellow>{Duration} секунд</color>.";

        ZH_HANSSkillDesc = $"保护自身周围<color=#AEEAFF>{Size}X{Size}</color>范围内的盟友，使他们在<color=yellow>{Duration}秒</color>内无敌。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.1</color>\n" +
                   "지속 시간 <color=yellow>+0.1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Range <color=#AEEAFF>+0.1</color>\n" +
                   "Duration <color=yellow>+0.1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Portée <color=#AEEAFF>+0.1</color>\n" +
                   "Durée <color=yellow>+0.1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.1</color>\n" +
                   "Durata <color=yellow>+0.1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.1</color>\n" +
                   "Dauer <color=yellow>+0.1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.1</color>\n" +
                   "Duración <color=yellow>+0.1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.1</color>\n" +
                   "持続時間 <color=yellow>+0.1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.1</color>\n" +
                   "Duração <color=yellow>+0.1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.1</color>\n" +
                   "Длительность <color=yellow>+0.1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.1</color>\n" +
                   "持续时间 <color=yellow>+0.1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Range <color=#AEEAFF>+0.1</color>\n" +
               "Duration <color=yellow>+0.1</color>";
    }
}