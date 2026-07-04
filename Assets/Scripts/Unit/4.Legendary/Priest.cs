using UnityEngine;
using UnityEngine.Localization.Settings;

public class Priest : UnitDefault
{
    [Header("신성한 빛")]
    public float incHealthPer;
    public int incManaAmount;

    float IncHealthPer { get => incHealthPer + StaticManager.skillPoint[unitIdx] * 4f; }
    int IncManaAmount { get => incManaAmount + StaticManager.skillPoint[unitIdx] / 3; }

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
        isHealer = true;
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        foreach (UnitDefault unitComp in friends) {
            GameObject effect = Instantiate(attackObj, unitComp.transform.position, Quaternion.identity);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            unitComp.Healing(unitComp.MaxHealth / 100f * IncHealthPer, unitIdx);
            if (unitComp is not Druid && unitComp is not Priest) {
                unitComp.SetMana(incManaAmount);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"아군 전체의 체력을 {IncHealthPer}%, 마나를 {IncManaAmount}만큼 회복시켜 줍니다.\n" +
                    $"(드루이드, 성직자는 마나 회복 제한)";
        
        ENSkillDesc = $"Restores {IncHealthPer}% health and {IncManaAmount} mana to all allies.\n" +
                      $"(Druids and Priests have mana recovery limit)";

        FRSkillDesc = $"Restaure {IncHealthPer}% de santé et {IncManaAmount} de mana à tous les alliés.\n" +
                      $"(Les druides et les prêtres ont une limite de récupération de mana)";

        ITSkillDesc = $"Ripristina il {IncHealthPer}% di salute e {IncManaAmount} di mana a tutti gli alleati.\n" +
                      $"(I druidi e i sacerdoti hanno un limite di recupero del mana)";

        DESkillDesc = $"Stellt {IncHealthPer}% Gesundheit und {IncManaAmount} Mana für alle Verbündeten wieder her.\n" +
                      $"(Druiden und Priester haben ein Mana-Wiederherstellungslimit)";

        ESSkillDesc = $"Restaura el {IncHealthPer}% de salud y {IncManaAmount} de maná a todos los aliados.\n" +
                      $"(Los druidas y sacerdotes tienen un límite de recuperación de maná)";

        JASkillDesc = $"味方全体の体力を{IncHealthPer}%、マナを{IncManaAmount}回復させます。\n" +
                      $"（ドルイド、プリーストはマナ回復制限）";

        PT_BRSkillDesc = $"Restaura {IncHealthPer}% de saúde e {IncManaAmount} de mana para todos os aliados.\n" +
                         $"(Druidas e sacerdotes têm limite de recuperação de mana)";

        RUSkillDesc = $"Восстанавливает {IncHealthPer}% здоровья и {IncManaAmount} маны всем союзникам.\n" +
                      $"(У друидов и жрецов есть ограничение на восстановление маны)";

        ZH_HANSSkillDesc = $"为所有盟友恢复{IncHealthPer}%的生命值和{IncManaAmount}的法力值。\n" +
                           $"（德鲁伊和牧师有法力恢复限制）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "체력 회복량 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "마나 회복량 <color=green>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Health recovery <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Mana recovery <color=green>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Récupération de PV <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Récupération de mana <color=green>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Recupero salute <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Recupero mana <color=green>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Lebensregeneration <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Manaregeneration <color=green>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Recuperación de vida <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Recuperación de maná <color=green>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "体力回復量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "マナ回復量 <color=green>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Recuperação de vida <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Recuperação de mana <color=green>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Восстановление здоровья <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Восстановление маны <color=green>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "生命恢复量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "法力恢复量 <color=green>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Health recovery <color=green>+4%p</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Mana recovery <color=green>+1</color>";
    }
}