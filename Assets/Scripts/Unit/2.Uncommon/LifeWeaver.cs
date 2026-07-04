using UnityEngine;
using UnityEngine.Localization.Settings;

public class LifeWeaver : UnitDefault
{
    [Header("치유")]
    public float incHealthAmount;

    float IncHealthAmount { get => incHealthAmount + StaticManager.skillPoint[unitIdx] * 30f; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
        isHealer = true;
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindLesserHpGaugePerFriendly();
        if (FriendlyTarget == null)
            return;

        if (FriendlyTarget == gameObject){
            SoundPlay(skillSoundClip);
            OnSkillFire(gameObject);
        }
        else
            StartCoroutine(OnFire(FriendlyTarget, FriendlyTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        GameObject effect = Instantiate(attackObj, target.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        target.GetComponent<UnitDefault>().Healing(IncHealthAmount, unitIdx);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인을 포함한 가장 체력비가 낮은 아군 유닛의 체력을 <color=green>{IncHealthAmount}</color>만큼 회복시킵니다.";
        
        ENSkillDesc = $"Heals the ally unit with the lowest health percentage, including yourself, by <color=green>{IncHealthAmount}</color>.";

        FRSkillDesc = $"Soigne l'unité alliée ayant le pourcentage de santé le plus bas, y compris vous-même, de <color=green>{IncHealthAmount}</color>.";

        ITSkillDesc = $"Cura l'unità alleata con la percentuale di salute più bassa, incluso te stesso, di <color=green>{IncHealthAmount}</color>.";

        DESkillDesc = $"Heilt die verbündete Einheit mit dem niedrigsten Gesundheitsprozentsatz, einschließlich dir selbst, um <color=green>{IncHealthAmount}</color>.";

        ESSkillDesc = $"Cura a la unidad aliada con el porcentaje de salud más bajo, incluido tú mismo, en <color=green>{IncHealthAmount}</color>.";

        JASkillDesc = $"自分を含む最も体力比が低い味方ユニットの体力を<color=green>{IncHealthAmount}</color>回復します。";

        PT_BRSkillDesc = $"Cura a unidade aliada com a menor porcentagem de saúde, incluindo você mesmo, em <color=green>{IncHealthAmount}</color>.";

        RUSkillDesc = $"Исцеляет союзный юнит с самым низким процентом здоровья, включая вас, на <color=green>{IncHealthAmount}</color>.";

        ZH_HANSSkillDesc = $"治疗包括你自己在内的生命值百分比最低的友军单位，恢复<color=green>{IncHealthAmount}</color>生命值。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "치유량 <color=green>+30</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Healing <color=green>+30</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Soins <color=green>+30</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Cura <color=green>+30</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Heilung <color=green>+30</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Curación <color=green>+30</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "回復量 <color=green>+30</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Cura <color=green>+30</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Исцеление <color=green>+30</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "治疗量 <color=green>+30</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Healing <color=green>+30</color>";
    }
}