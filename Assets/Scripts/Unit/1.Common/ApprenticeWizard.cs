using UnityEngine;
using UnityEngine.Localization.Settings;

public class ApprenticeWizard : UnitDefault
{
    [Header("파이어볼")]
    public float attackCoeff;
    public float burnDamage;
    public int count;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.2f; }
    float BurnDamage { get => burnDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 5; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
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

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        target.GetComponent<UnitDefault>().Burn(BurnDamage, count, this, dealMetricsIdx);
        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 파이어볼을 던져 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고 <color=yellow>{Count}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{BurnDamage}</color>";

        ENSkillDesc = $"Throws a fireball at the attacking target, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power and inflicting burns for <color=yellow>{Count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{BurnDamage}</color>";

        FRSkillDesc = $"Lance une boule de feu sur la cible attaquée, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque et lui infligeant des brûlures pendant <color=yellow>{Count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{BurnDamage}</color>";

        ITSkillDesc = $"Lancia una palla di fuoco al bersaglio attaccante, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco e infliggendo ustioni per <color=yellow>{Count} secondi</color>.\n" +
                      $"Danno da ustione al secondo: <color=red>{BurnDamage}</color>";

        DESkillDesc = $"Wirft einen Feuerball auf das angreifende Ziel und verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft und fügt für <color=yellow>{Count} Sekunden</color> Verbrennungen zu.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{BurnDamage}</color>";

        ESSkillDesc = $"Lanza una bola de fuego al objetivo atacante, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque e infligiendo quemaduras durante <color=yellow>{Count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{BurnDamage}</color>";

        JASkillDesc = $"攻撃中の対象にファイアボールを投げ、攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、<color=yellow>{Count}秒</color>間火傷を負わせます。\n" +
                      $"1秒あたりの火傷ダメージ: <color=red>{BurnDamage}</color>";

        PT_BRSkillDesc = $"Lança uma bola de fogo no alvo atacante, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque e causando queimaduras por <color=yellow>{Count} segundos</color>.\n" +
                         $"Dano de queimadura por segundo: <color=red>{BurnDamage}</color>";

        RUSkillDesc = $"Бросает огненный шар в атакующую цель, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки, и нанося ожоги на <color=yellow>{Count} секунд</color>.\n" +
                      $"Урон от ожога в секунду: <color=red>{BurnDamage}</color>";

        ZH_HANSSkillDesc = $"向正在攻击的目标投掷火球，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，并在<color=yellow>{Count}秒</color>内造成灼烧效果。\n" +
                           $"每秒灼烧伤害: <color=red>{BurnDamage}</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.2</color>\n" +
                   "초당 화상 데미지 <color=red>+5</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "화상 지속시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.2</color>\n" +
                   "Burn DPS <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Burn duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.2</color>\n" +
                   "DPS de brûlure <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée de brûlure <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.2</color>\n" +
                   "DPS da bruciatura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata della bruciatura <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.2</color>\n" +
                   "Brand-DPS <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Branddauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "DPS por quemadura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración de la quemadura <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.2</color>\n" +
                   "焼けダメージDPS <color=red>+5</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "焼け持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "DPS de queimadura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração da queimadura <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.2</color>\n" +
                   "DPS от ожога <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность ожога <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.2</color>\n" +
                   "灼烧DPS <color=red>+5</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "灼烧持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.2</color>\n" +
               "Burn DPS <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Burn duration <color=yellow>+1</color>";
    }
}