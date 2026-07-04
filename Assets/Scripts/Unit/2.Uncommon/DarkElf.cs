using UnityEngine;
using UnityEngine.Localization.Settings;

public class DarkElf : UnitDefault
{
    [Header("약점 노출")]
    public float decDefenseAmount;

    float DecDefenseAmount { get => decDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        RandomEnemyUnit(enemies);
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        GameObject effect = Instantiate(attackObj, target.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        unitComp.SetStat(unitComp.defenseStat, DecDefenseAmount, false, false);
        unitComp.SetStateBar(State.decDefense, -2f);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"랜덤 적 유닛 한 명의 방어력을 <color=red>{DecDefenseAmount}</color>만큼 감소시킵니다.\n" +
                    $"(중첩 가능)";
        
        ENSkillDesc = $"Reduces the defense of a random enemy unit by <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Can stack)";

        FRSkillDesc = $"Réduit la défense d'une unité ennemie aléatoire de <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Peut s'empiler)";

        ITSkillDesc = $"Riduce la difesa di un'unità nemica casuale di <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Può accumularsi)";

        DESkillDesc = $"Reduziert die Verteidigung einer zufälligen feindlichen Einheit um <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Kann gestapelt werden)";

        ESSkillDesc = $"Reduce la defensa de una unidad enemiga aleatoria en <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Se puede apilar)";

        JASkillDesc = $"ランダムな敵ユニット1体の防御力を<color=red>{DecDefenseAmount}</color>だけ減少させます。\n" +
                      $"(重複可能)";
        
        PT_BRSkillDesc = $"Reduz a defesa de uma unidade inimiga aleatória em <color=red>{DecDefenseAmount}</color>.\n" +
                         $"(Pode ser acumulado)";

        RUSkillDesc = $"Снижает защиту случайного вражеского юнита на <color=red>{DecDefenseAmount}</color>.\n" +
                      $"(Можно складывать)";

        ZH_HANSSkillDesc = $"将随机敌方单位的防御力降低<color=red>{DecDefenseAmount}</color>。\n" +
                           $"(可叠加)";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "방어력 감소량 <color=red>+5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Defense reduction <color=red>+5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Réduction de défense <color=red>+5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Riduzione difesa <color=red>+5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Verteidigungsreduktion <color=red>+5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Reducción de defensa <color=red>+5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "防御力減少 <color=red>+5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Redução de defesa <color=red>+5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Снижение защиты <color=red>+5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "防御力降低 <color=red>+5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Defense reduction <color=red>+5</color>";
    }
}