using UnityEngine;
using UnityEngine.Localization.Settings;

public class Spearman : UnitDefault
{
    [Header("투창")]
    public float attackCoeff;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.15f; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (Target == null)
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        StartCoroutine(OnFire(SkillTarget, pos, true, true));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position, isPenetration: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격중인 방향으로 유닛을 관통하는 창을 던집니다.\n" +
                    $"창에 맞은 적들은 공격력 <color=red>{AttackCoeff}배</color>의 방어력 관통 피해를 입습니다.";
        
        ENSkillDesc = $"Throws a spear that pierces units in the attack direction.\n" +
                      $"Enemies hit by the spear take defense-piercing damage equal to <color=red>{AttackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Lance une lance qui transperce les unités dans la direction de l'attaque.\n" +
                      $"Les ennemis touchés par la lance subissent des dégâts perforants égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Lancia una lancia che trafigge le unità nella direzione dell'attacco.\n" +
                      $"I nemici colpiti dalla lancia subiscono danni perforanti pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Wirft einen Speer, der Einheiten in Angriffsrichtung durchbohrt.\n" +
                      $"Von dem Speer getroffene Feinde erleiden durchdringenden Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Lanza una lanza que atraviesa unidades en la dirección del ataque.\n" +
                      $"Los enemigos alcanzados por la lanza reciben daño penetrante igual a <color=red>{AttackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"攻撃方向にユニットを貫通する槍を投げます。\n" +
                      $"槍に当たった敵は攻撃力の<color=red>{AttackCoeff}倍</color>の防御力貫通ダメージを受けます。";

        PT_BRSkillDesc = $"Lança uma lança que perfura unidades na direção do ataque.\n" +
                         $"Inimigos atingidos pela lança recebem dano perfurante igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"Бросает копье, которое пронизывает юнитов в направлении атаки.\n" +
                      $"Враги, пораженные копьем, получают проникающий урон, равный <color=red>{AttackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"向攻击方向投掷一支贯穿单位的长矛。\n" +
                           $"被长矛击中的敌人会受到相当于攻击力<color=red>{AttackCoeff}倍</color>的无视防御伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.15</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.15</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.15</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.15</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.15</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.15</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.15</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.15</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.15</color>";
    }
}