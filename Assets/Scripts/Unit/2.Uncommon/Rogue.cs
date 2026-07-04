using UnityEngine;
using UnityEngine.Localization.Settings;

public class Rogue : UnitDefault
{
    [Header("베놈 커터")]
    public float poisonDamage;
    public int count;
    public float incCriticalPer;
    public float duration;

    float PoisonDamage { get => poisonDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 5; }
    float IncCriticalPer { get => incCriticalPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        SetStat(criticalPerStat, IncCriticalPer, false, true, Duration);
        SetStateBar(State.incCriticalPer, Duration);

        if (SkillTarget == null)
            return;

        SkillTarget.GetComponent<UnitDefault>().Poison(PoisonDamage, count, this, dealMetricsIdx);
        CreateAttackBox(AttackPower, SkillTarget, SkillTarget.transform.position, isSkill: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 <color=yellow>{Count}초</color> 동안 독 효과를 입힙니다.\n" +
                    $"또한 <color=yellow>{Duration}초</color> 동안 본인의 치명타 확률이 <color=green>{IncCriticalPer}%p</color> 증가하며, 기본 공격 피해를 입힙니다.\n" +
                    $"초당 독 데미지: <color=red>{PoisonDamage}</color>";

        ENSkillDesc = $"Inflicts a poison effect on the target being attacked for <color=yellow>{Count} seconds</color>.\n" +
                      $"Also increases own critical hit rate by <color=green>{IncCriticalPer}%p</color> for <color=yellow>{Duration} seconds</color>, dealing basic attack damage.\n" +
                      $"Poison damage per second: <color=red>{PoisonDamage}</color>";

        FRSkillDesc = $"Inflige un effet de poison à la cible attaquée pendant <color=yellow>{Count} secondes</color>.\n" +
                      $"Augmente également la probabilité de coup critique de <color=green>{IncCriticalPer}%p</color> pendant <color=yellow>{Duration} secondes</color>, infligeant des dégâts d'attaque de base.\n" +
                      $"Dégâts de poison par seconde : <color=red>{PoisonDamage}</color>";

        ITSkillDesc = $"Infligge un effetto veleno al bersaglio attaccato per <color=yellow>{Count} secondi</color>.\n" +
                      $"Aumenta inoltre la probabilità di colpo critico del <color=green>{IncCriticalPer}%p</color> per <color=yellow>{Duration} secondi</color>, infliggendo danni da attacco base.\n" +
                      $"Danno da veleno al secondo: <color=red>{PoisonDamage}</color>";

        DESkillDesc = $"Fügt dem angegriffenen Ziel für <color=yellow>{Count} Sekunden</color> einen Gift-Effekt zu.\n" +
                      $"Erhöht außerdem die kritische Trefferchance um <color=green>{IncCriticalPer}%p</color> für <color=yellow>{Duration} Sekunden</color> und verursacht normalen Angriffsschaden.\n" +
                      $"Giftschaden pro Sekunde: <color=red>{PoisonDamage}</color>";

        ESSkillDesc = $"Inflige un efecto de veneno al objetivo atacado durante <color=yellow>{Count} segundos</color>.\n" +
                      $"También aumenta la probabilidad de golpe crítico en <color=green>{IncCriticalPer}%p</color> durante <color=yellow>{Duration} segundos</color>, infligiendo daño de ataque básico.\n" +
                      $"Daño por veneno por segundo: <color=red>{PoisonDamage}</color>";

        JASkillDesc = $"攻撃中の対象に<color=yellow>{Count}秒</color>間、毒効果を与えます。\n" +
                      $"また、<color=yellow>{Duration}秒</color>間、自身のクリティカル確率が<color=green>{IncCriticalPer}%p</color>増加し、通常攻撃ダメージを与えます。\n" +
                      $"秒間毒ダメージ: <color=red>{PoisonDamage}</color>";

        PT_BRSkillDesc = $"Inflige um efeito de veneno no alvo atacado por <color=yellow>{Count} segundos</color>.\n" +
                         $"Também aumenta a taxa de acerto crítico própria em <color=green>{IncCriticalPer}%p</color> por <color=yellow>{Duration} segundos</color>, causando dano de ataque básico.\n" +
                         $"Dano por veneno por segundo: <color=red>{PoisonDamage}</color>";

        RUSkillDesc = $"Накладывает эффект яда на атакуемую цель на <color=yellow>{Count} секунд</color>.\n" +
                      $"Также увеличивает шанс критического удара на <color=green>{IncCriticalPer}%p</color> на <color=yellow>{Duration} секунд</color>, нанося урон от базовой атаки.\n" +
                      $"Урон от яда в секунду: <color=red>{PoisonDamage}</color>";

        ZH_HANSSkillDesc = $"对正在攻击的目标施加<color=yellow>{Count}秒</color>的毒效果。\n" +
                           $"同时，自身的暴击率在<color=yellow>{Duration}秒</color>内提高<color=green>{IncCriticalPer}%p</color>，造成基础攻击伤害。\n" +
                           $"每秒毒素伤害: <color=red>{PoisonDamage}</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "초당 독 데미지 <color=red>+5</color>\n" +
                   "치명타 확률 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "치명타 확률 증가 지속 시간 <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "독 지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Poison damage per second <color=red>+5</color>\n" +
                   "Critical chance increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Critical chance duration <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Poison duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Dégâts de poison par seconde <color=red>+5</color>\n" +
                   "Augmentation probabilité critique <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée de l'augmentation critique <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée du poison <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Danno veleno al secondo <color=red>+5</color>\n" +
                   "Aumento probabilità critico <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata aumento critico <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata veleno <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Gift-Schaden pro Sekunde <color=red>+5</color>\n" +
                   "Erhöhung Kritische Trefferchance <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer der Kritischen Trefferchance <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Giftdauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Daño de veneno por segundo <color=red>+5</color>\n" +
                   "Aumento probabilidad crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración aumento crítico <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración veneno <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "秒間毒ダメージ <color=red>+5</color>\n" +
                   "クリティカル確率増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "クリティカル確率増加持続時間 <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "毒持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Dano de veneno por segundo <color=red>+5</color>\n" +
                   "Aumento chance crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração do aumento crítico <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração do veneno <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Урон ядом в секунду <color=red>+5</color>\n" +
                   "Увеличение критического шанса <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность увеличения крит. шанса <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность яда <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "每秒毒伤害 <color=red>+5</color>\n" +
                   "暴击率增加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "暴击率增加持续时间 <color=yellow>+0.5</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "毒持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Poison damage per second <color=red>+5</color>\n" +
               "Critical chance increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Critical chance duration <color=yellow>+0.5</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Poison duration <color=yellow>+1</color>";
    }
}