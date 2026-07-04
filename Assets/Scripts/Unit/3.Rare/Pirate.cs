using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Pirate : UnitDefault
{
    [Header("칼날 폭풍")]
    public float attackCoeff;
    public int count;
    public float interval;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.15f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 3; }

    WaitForSeconds wait;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        wait = new(interval);
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        StartCoroutine(BladeStorm());
    }

    IEnumerator BladeStorm()
    {
        for (int i = 0; i < Count; i++) {
            RandomEnemyUnit(enemies);
            if (SkillTarget == null || isDeath)
                yield break;

            StartCoroutine(OnFire(SkillTarget, skillTargetPos));
            yield return wait;
        }
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"무작위 적 유닛들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히는 칼날을 빠른 속도로 <color=yellow>{Count}회</color> 던집니다.";
        
        ENSkillDesc = $"Throws blades at random enemy units <color=yellow>{Count} times</color> at a rapid pace, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Lance des lames sur des unités ennemies aléatoires <color=yellow>{Count} fois</color> à un rythme rapide, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Lancia lame su unità nemiche casuali <color=yellow>{Count} volte</color> a un ritmo rapido, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Wirft Klingen auf zufällige feindliche Einheiten <color=yellow>{Count} mal</color> in schnellem Tempo und verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Lanza cuchillas a unidades enemigas aleatorias <color=yellow>{Count} veces</color> a un ritmo rápido, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"ランダムな敵ユニットに攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与える刃を素早い速度で<color=yellow>{Count}回</color>投げつけます。";

        PT_BRSkillDesc = $"Lança lâminas em unidades inimigas aleatórias <color=yellow>{Count} vezes</color> em um ritmo rápido, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"Бросает лезвия в случайные вражеские юниты <color=yellow>{Count} раз</color> с высокой скоростью, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"以快速的速度向随机敌方单位投掷<color=yellow>{Count}次</color>刀刃，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "칼날 수 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Number of blades <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Nombre de lames <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Numero di lame <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Anzahl der Klingen <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Número de cuchillas <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "刃の数 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Número de lâminas <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Количество лезвий <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "刀刃数量 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.15</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Number of blades <color=yellow>+1</color>";
    }
}