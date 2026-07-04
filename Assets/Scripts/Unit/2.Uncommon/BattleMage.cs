using UnityEngine;
using UnityEngine.Localization.Settings;

public class BattleMage : UnitDefault
{
    [Header("블라스트")]
    public float attackCoeff;
    public float size;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float Size { get => size +  StaticManager.skillPoint[unitIdx] / 2 * 0.25f; }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(Size / 2f, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        attackSize = Size;
        CreateAttackBox(AttackPower * AttackCoeff, null, transform.position, AttackStyle.range);
        attackSize = 0.5f;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{Size}X{Size}</color>범위에 폭발을 일으켜 적들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Creates an explosion in a <color=#AEEAFF>{Size}X{Size}</color> area around yourself, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power to enemies.";

        FRSkillDesc = $"Crée une explosion dans une zone de <color=#AEEAFF>{Size}X{Size}</color> autour de vous, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque aux ennemis.";

        ITSkillDesc = $"Crea un'esplosione in un'area di <color=#AEEAFF>{Size}X{Size}</color> intorno a te, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco ai nemici.";

        DESkillDesc = $"Erzeugt eine Explosion in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich um dich herum und verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft an Feinden.";

        ESSkillDesc = $"Crea una explosión en un área de <color=#AEEAFF>{Size}X{Size}</color> alrededor de ti, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque a los enemigos.";

        JASkillDesc = $"自身の周囲<color=#AEEAFF>{Size}X{Size}</color>範囲に爆発を起こし、敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Cria uma explosão em uma área de <color=#AEEAFF>{Size}X{Size}</color> ao seu redor, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque aos inimigos.";

        RUSkillDesc = $"Создает взрыв в области <color=#AEEAFF>{Size}X{Size}</color> вокруг себя, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки врагам.";

        ZH_HANSSkillDesc = $"在自身周围的<color=#AEEAFF>{Size}X{Size}</color>范围内引发爆炸，对敌人造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.25</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.25</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.25</color>";
    }
}