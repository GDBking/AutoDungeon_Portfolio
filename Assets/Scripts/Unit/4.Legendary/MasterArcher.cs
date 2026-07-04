using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MasterArcher : UnitDefault
{
    [Header("전화위복")]
    public float incAttackPer;
    public float incAttackSpeedPer;
    public float incAttackRangePer;

    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 3f; }

    float elapsedTime;
    bool isUpgrade;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        elapsedTime += Time.fixedDeltaTime;
        if (elapsedTime >= 1f) {
            if (enemies.Count > friends.Count) {
                isUpgrade = true;
                OnSkillAttack();
            }
            else
                isUpgrade = false;

            elapsedTime = 0f;
        }
    }

    protected override void UseSkill()
    {
        
    }
    
    protected override void OnSkillAttack()
    {
        SetStat(attackPowerStat, IncAttackPer, true, true, 1f);
        SetStat(attackSpeedStat, IncAttackSpeedPer, true, true, 1f);
        SetStat(attackRangeStat, incAttackRangePer, true, true, 1f);
        SetStateBar(State.comeback, 1f);
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower, target, target.transform.position);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        return base.OnFire(target, targetPos, isUpgrade, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"아군 유닛의 수가 적군 유닛의 수 보다 더 적을 경우 본인의 공격력이 <color=green>{IncAttackPer}%</color>, 공격 속도가 <color=green>{IncAttackSpeedPer}%</color>, 사정거리가 <color=green>{incAttackRangePer}%</color> 증가합니다.";
        
        ENSkillDesc = $"When the number of allied units is less than that of enemy units, increases own attack power by <color=green>{IncAttackPer}%</color>, attack speed by <color=green>{IncAttackSpeedPer}%</color>, and attack range by <color=green>{incAttackRangePer}%</color>.";

        FRSkillDesc = $"Lorsque le nombre d'unités alliées est inférieur à celui des unités ennemies, augmente la puissance d'attaque de <color=green>{IncAttackPer}%</color>, la vitesse d'attaque de <color=green>{IncAttackSpeedPer}%</color> et la portée d'attaque de <color=green>{incAttackRangePer}%</color>.";

        ITSkillDesc = $"Quando il numero di unità alleate è inferiore a quello delle unità nemiche, aumenta la potenza d'attacco di <color=green>{IncAttackPer}%</color>, la velocità d'attacco di <color=green>{IncAttackSpeedPer}%</color> e la portata d'attacco di <color=green>{incAttackRangePer}%</color>.";

        DESkillDesc = $"Wenn die Anzahl der verbündeten Einheiten geringer ist als die der feindlichen Einheiten, erhöht sich die eigene Angriffskraft um <color=green>{IncAttackPer}%</color>, die Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedPer}%</color> und die Angriffsreichweite um <color=green>{incAttackRangePer}%</color>.";

        ESSkillDesc = $"Cuando el número de unidades aliadas es menor que el de unidades enemigas, aumenta el poder de ataque en <color=green>{IncAttackPer}%</color>, la velocidad de ataque en <color=green>{IncAttackSpeedPer}%</color> y el alcance de ataque en <color=green>{incAttackRangePer}%</color>.";

        JASkillDesc = $"味方ユニットの数が敵ユニットの数よりも少ない場合、自身の攻撃力が<color=green>{IncAttackPer}%</color>、攻撃速度が<color=green>{IncAttackSpeedPer}%</color>、射程が<color=green>{incAttackRangePer}%</color>増加します。";

        PT_BRSkillDesc = $"Quando o número de unidades aliadas é menor que o de unidades inimigas, aumenta o poder de ataque em <color=green>{IncAttackPer}%</color>, a velocidade de ataque em <color=green>{IncAttackSpeedPer}%</color> e o alcance de ataque em <color=green>{incAttackRangePer}%</color>.";

        RUSkillDesc = $"Когда количество союзных единиц меньше, чем количество вражеских единиц, увеличивает собственную силу атаки на <color=green>{IncAttackPer}%</color>, скорость атаки на <color=green>{IncAttackSpeedPer}%</color> и дальность атаки на <color=green>{incAttackRangePer}%</color>.";

        ZH_HANSSkillDesc = $"当友军单位数量少于敌军单位数量时，自身的攻击力提高<color=green>{IncAttackPer}%</color>，攻击速度提高<color=green>{IncAttackSpeedPer}%</color>，攻击范围提高<color=green>{incAttackRangePer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+5%p</color>\n" +
                   "공격 속도 증가량 <color=green>+3%p</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack power increase <color=green>+5%p</color>\n" +
                   "Attack speed increase <color=green>+3%p</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de l'attaque <color=green>+5%p</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+3%p</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento potenza attacco <color=green>+5%p</color>\n" +
                   "Aumento velocità attacco <color=green>+3%p</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffskraft-Erhöhung <color=green>+5%p</color>\n" +
                   "Angriffsgeschwindigkeit-Erhöhung <color=green>+3%p</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de poder de ataque <color=green>+5%p</color>\n" +
                   "Aumento de velocidad de ataque <color=green>+3%p</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+5%p</color>\n" +
                   "攻撃速度増加 <color=green>+3%p</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de ataque <color=green>+5%p</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+3%p</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение атаки <color=green>+5%p</color>\n" +
                   "Увеличение скорости атаки <color=green>+3%p</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力增加 <color=green>+5%p</color>\n" +
                   "攻击速度增加 <color=green>+3%p</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack power increase <color=green>+5%p</color>\n" +
               "Attack speed increase <color=green>+3%p</color>";
    }
}