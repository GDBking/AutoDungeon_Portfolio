using UnityEngine;
using UnityEngine.Localization.Settings;

public class Sniper : UnitDefault
{
    [Header("저격")]
    public float attackCoeff;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.25f; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindFarthestEnemy(enemies);

        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 기준 가장 멀리 있는 적에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Deals damage equal to <color=red>{AttackCoeff} times</color> the attack power to the farthest enemy from yourself.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque à l'ennemi le plus éloigné de vous.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco al nemico più lontano da te.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft am am weitesten entfernten Feind von dir.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque al enemigo más lejano de ti.";

        JASkillDesc = $"自分から最も遠い敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Inflige dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque ao inimigo mais distante de você.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{AttackCoeff} раза</color> силы атаки самому дальнему от вас врагу.";

        ZH_HANSSkillDesc = $"对距离你最远的敌人造成相当于<color=red>{AttackCoeff}倍</color>攻击力的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.25</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.25</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.25</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.25</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.25</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.25</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.25</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.25</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.25</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.25</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.25</color>";
    }
}