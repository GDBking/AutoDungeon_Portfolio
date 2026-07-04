using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SharpShooter : UnitDefault
{
    [Header("관통샷")]
    public float duration;

    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.25f; }

    bool isPenetration;
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
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStateBar(State.penetrationShoot, Duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(PenetrationShoot());
    }

    IEnumerator PenetrationShoot()
    {
        isPenetration = true;
        yield return wait;
        isPenetration = false;

        co = null;
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower, target, target.transform.position, isPenetration: true);
    }


    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        if (this.isPenetration)
            StartCoroutine(base.OnFire(target, targetPos, true, true)); 
        else
            StartCoroutine(base.OnFire(target, targetPos, false));

        yield return null;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 기본 공격이 유닛과 방어력을 관통하는 관통샷으로 변경됩니다.";
        
        ENSkillDesc = $"For <color=yellow>{Duration} seconds</color>, basic attacks are changed to penetration shots that pierce through units and defenses.";

        FRSkillDesc = $"Pendant <color=yellow>{Duration} secondes</color>, les attaques de base sont transformées en tirs de pénétration qui traversent les unités et les défenses.";

        ITSkillDesc = $"Per <color=yellow>{Duration} secondi</color>, gli attacchi base vengono trasformati in colpi di penetrazione che trapassano unità e difese.";

        DESkillDesc = $"Für <color=yellow>{Duration} Sekunden</color> werden Grundangriffe in Durchschlagschüsse geändert, die Einheiten und Verteidigungen durchdringen.";

        ESSkillDesc = $"Durante <color=yellow>{Duration} segundos</color>, los ataques básicos se cambian a disparos de penetración que atraviesan unidades y defensas.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、基本攻撃がユニットと防御力を貫通する貫通ショットに変更されます。";

        PT_BRSkillDesc = $"Por <color=yellow>{Duration} segundos</color>, os ataques básicos são alterados para tiros de penetração que perfuram unidades e defesas.";

        RUSkillDesc = $"В течение <color=yellow>{Duration} секунд</color> базовые атаки превращаются в проникающие выстрелы, которые проходят сквозь юнитов и защиты.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内，基础攻击变为穿透单位和防御的穿透射击。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.25</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Duration <color=yellow>+0.25</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Durée <color=yellow>+0.25</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Durata <color=yellow>+0.25</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Dauer <color=yellow>+0.25</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Duración <color=yellow>+0.25</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.25</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Duração <color=yellow>+0.25</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Длительность <color=yellow>+0.25</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "持续时间 <color=yellow>+0.25</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Duration <color=yellow>+0.25</color>";
    }
}