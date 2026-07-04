using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Archer : UnitDefault
{
    [Header("연사")]
    public float attackCoeff;
    public float incAttackSpeedPer;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5 * 0.5f; }

    bool isRapidFire;
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
        SetStat(attackSpeedStat, IncAttackSpeedPer, true, true, Duration);
        SetStateBar(State.incAttackSpeed, Duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(RapidFireTime());
    }

    IEnumerator RapidFireTime()
    {
        isRapidFire = true;
        yield return wait;
        isRapidFire = false;

        co = null;
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        return base.OnFire(target, targetPos, isRapidFire, isPenetration);
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        if (unitComp.Health / unitComp.MaxHealth >= 0.5f) {
            CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
        }
        else {
            CreateAttackBox(AttackPower, target, target.transform.position);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 본인의 공격 속도가 <color=green>{IncAttackSpeedPer}%</color> 증가하며, 공격 중인 대상의 남은 체력이 50% 이상일 경우 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"For <color=yellow>{Duration} seconds</color>, increases own attack speed by <color=green>{IncAttackSpeedPer}%</color>, and deals damage equal to <color=red>{attackCoeff} times</color> the attack power if the target being attacked has more than 50% health remaining.";

        FRSkillDesc = $"Pendant <color=yellow>{Duration} secondes</color>, augmente la vitesse d'attaque de <color=green>{IncAttackSpeedPer}%</color>, et inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque si la cible attaquée a plus de 50% de santé restante.";

        ITSkillDesc = $"Per <color=yellow>{Duration} secondi</color>, aumenta la velocità di attacco del <color=green>{IncAttackSpeedPer}%</color> e infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco se il bersaglio attaccato ha più del 50% di salute rimanente.";

        DESkillDesc = $"Erhöht für <color=yellow>{Duration} Sekunden</color> die eigene Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedPer}%</color> und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft, wenn das angegriffene Ziel mehr als 50% verbleibende Gesundheit hat.";

        ESSkillDesc = $"Durante <color=yellow>{Duration} segundos</color>, aumenta la velocidad de ataque propia en <color=green>{IncAttackSpeedPer}%</color> e inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque si el objetivo atacado tiene más del 50% de salud restante.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、自身の攻撃速度が<color=green>{IncAttackSpeedPer}%</color>増加し、攻撃中の対象の残り体力が50%以上の場合、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Por <color=yellow>{Duration} segundos</color>, aumenta a velocidade de ataque própria em <color=green>{IncAttackSpeedPer}%</color> e causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque se o alvo atacado tiver mais de 50% de saúde restante.";

        RUSkillDesc = $"В течение <color=yellow>{Duration} секунд</color> увеличивает собственную скорость атаки на <color=green>{IncAttackSpeedPer}%</color> и наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки, если у атакуемой цели осталось более 50% здоровья.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内，自身的攻击速度提高<color=green>{IncAttackSpeedPer}%</color>，如果被攻击目标的剩余生命值超过50%，则造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "공격 속도 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "Attack speed increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Aumento velocità attacco <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Angriffsgeschwindigkeit +<color=green>5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Incremento de velocidad de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "攻撃速度増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "Увеличение скорости атаки <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "攻击速度提升 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "Attack speed increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}