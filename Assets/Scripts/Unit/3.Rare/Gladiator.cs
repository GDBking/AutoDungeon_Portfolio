using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Gladiator : UnitDefault
{
    [Header("반격")]
    public float attackCoeff;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.2f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] * 0.1f; }

    float saveDamage;
    WaitForSeconds wait;

    protected override void Start()
    {
        base.Start();

        wait = new(Duration);
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
          return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower + saveDamage * AttackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        float damage = base.Hit(attack, criticalPer, hitUnit, isPenetration);
        StartCoroutine(SaveDamage(damage));
        return damage;
    }

    IEnumerator SaveDamage(float damage)
    {
        saveDamage += damage;
        yield return wait;
        saveDamage -= damage;
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        float damage = base.CreateAttackBox(attackPower, target, targetPos, attackStyle, isSkill, isPenetration);

        if (isSkill)
            Healing(damage);

        return damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 기본 공격과 더불어 스킬 사용 <color=yellow>{Duration}초</color> 전까지 받은 모든 피해량의 <color=red>{AttackCoeff}배</color> 피해를 추가로 입히고, 입힌 피해량만큼 체력을 회복합니다.";
        
        ENSkillDesc = $"In addition to basic attacks on the target being attacked, deals additional damage equal to <color=red>{AttackCoeff} times</color> the total damage received until <color=yellow>{Duration} seconds</color> after using the skill, and heals for the amount of damage dealt.";

        FRSkillDesc = $"En plus des attaques de base sur la cible attaquée, inflige des dégâts supplémentaires égaux à <color=red>{AttackCoeff} fois</color> les dégâts totaux reçus jusqu'à <color=yellow>{Duration} secondes</color> après l'utilisation de la compétence, et se soigne de la quantité de dégâts infligés.";

        ITSkillDesc = $"Oltre agli attacchi base sul bersaglio attaccato, infligge danni aggiuntivi pari a <color=red>{AttackCoeff} volte</color> il totale dei danni ricevuti fino a <color=yellow>{Duration} secondi</color> dopo l'uso dell'abilità, e si cura per la quantità di danni inflitti.";

        DESkillDesc = $"Zusätzlich zu den Grundangriffen auf das angegriffene Ziel verursacht es zusätzlichen Schaden in Höhe von <color=red>{AttackCoeff} mal</color> des gesamten erlittenen Schadens bis zu <color=yellow>{Duration} Sekunden</color> nach der Verwendung der Fähigkeit und heilt sich um die Menge des verursachten Schadens.";

        ESSkillDesc = $"Además de los ataques básicos al objetivo atacado, inflige daño adicional igual a <color=red>{AttackCoeff} veces</color> el daño total recibido hasta <color=yellow>{Duration} segundos</color> después de usar la habilidad, y se cura por la cantidad de daño infligido.";

        JASkillDesc = $"攻撃中の対象に基本攻撃とともに、スキル使用後<color=yellow>{Duration}秒</color>までに受けた総ダメージの<color=red>{AttackCoeff}倍</color>の追加ダメージを与え、与えたダメージ分だけ体力を回復します。";

        PT_BRSkillDesc = $"Además de los ataques básicos al objetivo atacado, inflige daño adicional igual a <color=red>{AttackCoeff} veces</color> el daño total recibido hasta <color=yellow>{Duration} segundos</color> después de usar la habilidad, y se cura por la cantidad de daño infligido.";

        RUSkillDesc = $"Помимо базовых атак по атакуемой цели, наносит дополнительный урон, равный <color=red>{AttackCoeff} раза</color> от общего полученного урона до <color=yellow>{Duration} секунд</color> после использования навыка, и исцеляет на количество нанесенного урона.";

        ZH_HANSSkillDesc = $"除了对被攻击目标的基本攻击外，还会在使用技能后<color=yellow>{Duration}秒</color>内对所受总伤害造成相当于<color=red>{AttackCoeff}倍</color>的额外伤害，并根据造成的伤害量进行治疗。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.2</color>\n" +
                   "지속 시간 <color=yellow>+0.1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.2</color>\n" +
                   "Duration <color=yellow>+0.1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.2</color>\n" +
                   "Durée <color=yellow>+0.1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.2</color>\n" +
                   "Durata <color=yellow>+0.1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.2</color>\n" +
                   "Dauer <color=yellow>+0.1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Duración <color=yellow>+0.1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.2</color>\n" +
                   "持続時間 <color=yellow>+0.1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Duração <color=yellow>+0.1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.2</color>\n" +
                   "Длительность <color=yellow>+0.1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.2</color>\n" +
                   "持续时间 <color=yellow>+0.1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.2</color>\n" +
               "Duration <color=yellow>+0.1</color>";
    }
}