using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LongbowMan : UnitDefault
{
    [Header("집중")]
    public float incAttackPer;
    public float incCriticalPer;
    public float incAttackSpeedAmount;
    public float duration;

    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float IncCriticalPer { get => incCriticalPer + StaticManager.skillPoint[unitIdx] / 2 * 5f; }
    float IncAttackSpeedAmount { get => incAttackSpeedAmount + StaticManager.skillPoint[unitIdx] / 4 * 0.1f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5 * 0.5f; }

    bool isConcentration;
    WaitForSeconds wait;
    Coroutine co;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStat(attackPowerStat, IncAttackPer, true, true, duration);
        SetStat(criticalPerStat, IncCriticalPer, false, true, duration);
        SetStat(attackSpeedStat, IncAttackSpeedAmount, false, true, duration);

        SetStateBar(State.incAttack, duration);
        SetStateBar(State.incCriticalPer, duration);
        SetStateBar(State.incAttackSpeed, duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(ConcentrationTime());
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower, target, target.transform.position);
    }

    IEnumerator ConcentrationTime()
    {
        isConcentration = true;
        yield return wait;
        isConcentration = false;

        co = null;
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        return base.OnFire(target, targetPos, isConcentration, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 본인의 공격력을 <color=green>{IncAttackPer}%</color>, 치명타 확률을 <color=green>{IncCriticalPer}%p</color>, 공격 속도를 <color=green>{IncAttackSpeedAmount}</color>만큼 증가시킵니다.";

        ENSkillDesc = $"Increases own attack power by <color=green>{IncAttackPer}%</color>, critical hit rate by <color=green>{IncCriticalPer}%p</color>, and attack speed by <color=green>{IncAttackSpeedAmount}</color> for <color=yellow>{Duration} seconds</color>.";

        FRSkillDesc = $"Augmente la puissance d'attaque de <color=green>{IncAttackPer}%</color>, le taux de coup critique de <color=green>{IncCriticalPer}%p</color> et la vitesse d'attaque de <color=green>{IncAttackSpeedAmount}</color> pendant <color=yellow>{Duration} secondes</color>.";

        ITSkillDesc = $"Aumenta la potenza di attacco di <color=green>{IncAttackPer}%</color>, la probabilità di colpo critico di <color=green>{IncCriticalPer}%p</color> e la velocità di attacco di <color=green>{IncAttackSpeedAmount}</color> per <color=yellow>{Duration} secondi</color>.";

        DESkillDesc = $"Erhöht die eigene Angriffskraft um <color=green>{IncAttackPer}%</color>, die Kritische Trefferquote um <color=green>{IncCriticalPer}%p</color> und die Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedAmount}</color> für <color=yellow>{Duration} Sekunden</color>.";

        ESSkillDesc = $"Aumenta el poder de ataque en <color=green>{IncAttackPer}%</color>, la probabilidad de golpe crítico en <color=green>{IncCriticalPer}%p</color> y la velocidad de ataque en <color=green>{IncAttackSpeedAmount}</color> durante <color=yellow>{Duration} segundos</color>.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、自身の攻撃力が<color=green>{IncAttackPer}%</color>、クリティカル確率が<color=green>{IncCriticalPer}%p</color>、攻撃速度が<color=green>{IncAttackSpeedAmount}</color>だけ増加します。";

        PT_BRSkillDesc = $"Aumenta o poder de ataque em <color=green>{IncAttackPer}%</color>, a probabilidade de golpe crítico em <color=green>{IncCriticalPer}%p</color> e a velocidade de ataque em <color=green>{IncAttackSpeedAmount}</color> por <color=yellow>{Duration} segundos</color>.";

        RUSkillDesc = $"Увеличивает собственную силу атаки на <color=green>{IncAttackPer}%</color>, вероятность критического удара на <color=green>{IncCriticalPer}%p</color> и скорость атаки на <color=green>{IncAttackSpeedAmount}</color> на <color=yellow>{Duration} секунд</color>.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内，自身的攻击力提高<color=green>{IncAttackPer}%</color>，暴击率提高<color=green>{IncCriticalPer}%p</color>，攻击速度提高<color=green>{IncAttackSpeedAmount}</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "치명타 확률 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "공격 속도 증가량 <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Critical chance increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Attack speed increase <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation d'attaque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Augmentation de la probabilité de critique <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento attacco <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Aumento probabilità critico <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Aumento velocità attacco <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffserhöhung <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Erhöhung Kritische Trefferchance <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Angriffsgeschwindigkeitserhöhung <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Aumento de probabilidad crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Aumento de velocidad de ataque <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "クリティカル確率増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "攻撃速度増加 <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Aumento de chance crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение атаки <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Увеличение критического шанса <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Увеличение скорости атаки <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力增加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "暴击率增加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "攻击速度增加 <color=green>+0.1</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Critical chance increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Attack speed increase <color=green>+0.1</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}