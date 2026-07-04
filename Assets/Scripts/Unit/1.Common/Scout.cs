using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Scout : UnitDefault
{
    [Header("넓은 시야")]
    public float attackCoeff;
    public float incCriticalPer;
    public float incAttackRangeAmount;
    public float duration;

    float IncCriticalPer { get => incCriticalPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5 * 0.5f; }

    bool isWideView;
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

        wait = new(Duration);
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange + incAttackRangeAmount, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStat(criticalPerStat, IncCriticalPer, false, true, Duration);
        SetStat(attackRangeStat, incAttackRangeAmount, false, true, Duration);

        SetStateBar(State.incCriticalPer, Duration);
        SetStateBar(State.incAttackRange, Duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(WideViewTime());
    }

    IEnumerator WideViewTime()
    {
        isWideView = true;
        criticalCoeff = attackCoeff;
        yield return wait;
        criticalCoeff = 1.75f;
        isWideView = false;

        co = null;
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower, target, target.transform.position);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        return base.OnFire(target, targetPos, isWideView, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 본인의 치명타 확률이 <color=green>{IncCriticalPer}%p</color>, 사정거리가 <color=green>{incAttackRangeAmount}</color>만큼 증가하며, 치명타 적중 시 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"For <color=yellow>{Duration} seconds</color>, increases own critical hit rate by <color=green>{IncCriticalPer}%p</color> and attack range by <color=green>{incAttackRangeAmount}</color>, and deals damage equal to <color=red>{attackCoeff} times</color> the attack power on critical hits.";

        FRSkillDesc = $"Pendant <color=yellow>{Duration} secondes</color>, augmente la probabilité de coup critique de <color=green>{IncCriticalPer}%p</color> et la portée d'attaque de <color=green>{incAttackRangeAmount}</color>, et inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque lors des coups critiques.";

        ITSkillDesc = $"Per <color=yellow>{Duration} secondi</color>, aumenta la probabilità di colpo critico del <color=green>{IncCriticalPer}%p</color> e la portata di attacco di <color=green>{incAttackRangeAmount}</color>, e infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco sui colpi critici.";

        DESkillDesc = $"Erhöht für <color=yellow>{Duration} Sekunden</color> die kritische Trefferchance um <color=green>{IncCriticalPer}%p</color> und die Angriffsreichweite um <color=green>{incAttackRangeAmount}</color> und verursacht bei kritischen Treffern Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Durante <color=yellow>{Duration} segundos</color>, aumenta la probabilidad de golpe crítico en <color=green>{IncCriticalPer}%p</color> y el alcance de ataque en <color=green>{incAttackRangeAmount}</color>, e inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque en golpes críticos.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、自身のクリティカル確率が<color=green>{IncCriticalPer}%p</color>、射程が<color=green>{incAttackRangeAmount}</color>増加し、クリティカルヒット時に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Por <color=yellow>{Duration} segundos</color>, aumenta a taxa de acerto crítico própria em <color=green>{IncCriticalPer}%p</color> e o alcance de ataque em <color=green>{incAttackRangeAmount}</color>, e causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque em acertos críticos.";

        RUSkillDesc = $"В течение <color=yellow>{Duration} секунд</color> увеличивает собственный шанс критического удара на <color=green>{IncCriticalPer}%p</color> и дальность атаки на <color=green>{incAttackRangeAmount}</color>, а также наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки при критических ударах.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内，自身的暴击率提高<color=green>{IncCriticalPer}%p</color>，攻击范围增加<color=green>{incAttackRangeAmount}</color>，暴击时造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "치명타 계수 <color=red>+0.2</color>\n" +
                   "치명타 확률 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Critical multiplier <color=red>+0.2</color>\n" +
                   "Critical chance increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur critique <color=red>+0.2</color>\n" +
                   "Augmentation de la chance critique <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore critico <color=red>+0.2</color>\n" +
                   "Aumento probabilità critico <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Krit-Multiplikator <color=red>+0.2</color>\n" +
                   "Krit-Chance Erhöhung <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador crítico <color=red>+0.2</color>\n" +
                   "Incremento de probabilidad crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "クリティカル係数 <color=red>+0.2</color>\n" +
                   "クリティカル確率増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador crítico <color=red>+0.2</color>\n" +
                   "Aumento de chance crítica <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Критический множитель <color=red>+0.2</color>\n" +
                   "Увеличение шанса крита <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "暴击系数 <color=red>+0.2</color>\n" +
                   "暴击率提升 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Critical multiplier <color=red>+0.2</color>\n" +
               "Critical chance increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}