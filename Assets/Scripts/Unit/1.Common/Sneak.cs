using UnityEngine;
using UnityEngine.Localization.Settings;

public class Sneak : UnitDefault
{
    [Header("강탈")]
    public float incAttackPer;

    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] / 2 * 0.5f; }

    protected override void Start()
    {
        base.Start();

        SetStat(attackPowerStat, StaticManager.gold / 100f * IncAttackPer, true, true);
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"전투 시작 시 보유한 G량에 비례하여 본인의 공격력이 상승합니다.\n" +
                    $"상승 비율 (<color=#FFE600>100G</color> : 공격력 <color=green>+{IncAttackPer}%</color>)\n" +
                    $"전투 시작 시 증가량: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";
        
        ENSkillDesc = $"At the start of combat, increases own attack power in proportion to the amount of G held.\n" +
                      $"Increase rate (<color=#FFE600>100G</color> : Attack Power <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Increase amount at the start of combat: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        FRSkillDesc = $"Au début du combat, augmente la puissance d'attaque en fonction de la quantité de G détenue.\n" +
                      $"Taux d'augmentation (<color=#FFE600>100G</color> : Puissance d'attaque <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Montant de l'augmentation au début du combat : <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        ITSkillDesc = $"All'inizio del combattimento, aumenta la potenza di attacco in proporzione alla quantità di G posseduta.\n" +
                      $"Tasso di aumento (<color=#FFE600>100G</color> : Potenza di attacco <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Aumento all'inizio del combattimento: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        DESkillDesc = $"Zu Beginn des Kampfes erhöht sich die eigene Angriffskraft proportional zur gehaltenen G-Menge.\n" +
                      $"Erhöhungsrate (<color=#FFE600>100G</color> : Angriffskraft <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Erhöhungsbetrag zu Beginn des Kampfes: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        ESSkillDesc = $"Al comienzo del combate, aumenta el poder de ataque en proporción a la cantidad de G que se posee.\n" +
                      $"Tasa de aumento (<color=#FFE600>100G</color> : Poder de ataque <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Cantidad de aumento al comienzo del combate: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        JASkillDesc = $"戦闘開始時に所持しているG量に比例して、自身の攻撃力が上昇します。\n" +
                      $"上昇率 (<color=#FFE600>100G</color> : 攻撃力 <color=green>+{IncAttackPer}%</color>)\n" +
                      $"戦闘開始時の増加量: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        PT_BRSkillDesc = $"No início do combate, aumenta o poder de ataque em proporção à quantidade de G possuída.\n" +
                         $"Taxa de aumento (<color=#FFE600>100G</color> : Poder de Ataque <color=green>+{IncAttackPer}%</color>)\n" +
                         $"Quantidade de aumento no início do combate: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        RUSkillDesc = $"В начале боя увеличивает собственную силу атаки пропорционально количеству имеющихся G.\n" +
                      $"Коэффициент увеличения (<color=#FFE600>100G</color> : Сила атаки <color=green>+{IncAttackPer}%</color>)\n" +
                      $"Количество увеличения в начале боя: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";

        ZH_HANSSkillDesc = $"战斗开始时，根据所持有的G量提升自身的攻击力。\n" +
                           $"提升比例 (<color=#FFE600>100G</color> : 攻击力 <color=green>+{IncAttackPer}%</color>)\n" +
                           $"战斗开始时的提升量: <color=green>+{StaticManager.gold / 100f * IncAttackPer}%</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>2P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+0.5%p</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Attack increase <color=green>+0.5%p</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Augmentation d'attaque <color=green>+0.5%p</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Aumento attacco <color=green>+0.5%p</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Angriffserhöhung <color=green>+0.5%p</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Incremento de ataque <color=green>+0.5%p</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+0.5%p</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Aumento de ataque <color=green>+0.5%p</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Увеличение атаки <color=green>+0.5%p</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每2P效果</color>\n" +
                   "攻击力提升 <color=green>+0.5%p</color>";

        return "<color=#FFBF00>Effect per 2P</color>\n" +
               "Attack increase <color=green>+0.5%p</color>";
    }
}