using UnityEngine;
using UnityEngine.Localization.Settings;

public class Ninja : UnitDefault
{
    [Header("연막탄")]
    public float incDefenseAmount;
    public float incAttackSpeedPer;
    public float size;
    public int count;
    public GameObject smokeShellPrefab;

    public float IncDefenseAmount { get => incDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    public float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 5f; }
    public float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.2f; }
    public int Count { get => count + StaticManager.skillPoint[unitIdx] / 4; }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        GameObject effect = Instantiate(smokeShellPrefab, transform.position, Quaternion.identity, GameManager.instance.skillEffect);

        SmokeShell smokeComp = effect.GetComponent<SmokeShell>();
        smokeComp.unitComp = this;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{Size}X{Size}</color>범위에 <color=yellow>{Count}초</color> 동안 유지되는 연막탄을 터뜨려 연막 안에 있는 아군 유닛들은 방어력이 <color=green>{IncDefenseAmount}</color>, 공격 속도가 <color=green>{IncAttackSpeedPer}%</color> 상승합니다.";
        
        ENSkillDesc = $"Detonates a smoke shell that lasts for <color=yellow>{Count} seconds</color> in a <color=#AEEAFF>{Size}X{Size}</color> area around yourself, increasing the defense by <color=green>{IncDefenseAmount}</color> and attack speed by <color=green>{IncAttackSpeedPer}%</color> of allied units within the smoke.";

        FRSkillDesc = $"Détonne une grenade fumigène qui dure <color=yellow>{Count} secondes</color> dans une zone de <color=#AEEAFF>{Size}X{Size}</color> autour de vous, augmentant la défense de <color=green>{IncDefenseAmount}</color> et la vitesse d'attaque de <color=green>{IncAttackSpeedPer}%</color> des unités alliées à l'intérieur de la fumée.";

        ITSkillDesc = $"Fa detonare un fumogeno che dura <color=yellow>{Count} secondi</color> in un'area di <color=#AEEAFF>{Size}X{Size}</color> intorno a te, aumentando la difesa di <color=green>{IncDefenseAmount}</color> e la velocità di attacco di <color=green>{IncAttackSpeedPer}%</color> delle unità alleate all'interno del fumo.";

        DESkillDesc = $"Löst eine Rauchgranate aus, die <color=yellow>{Count} Sekunden</color> lang in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich um dich herum anhält und die Verteidigung um <color=green>{IncDefenseAmount}</color> und die Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedPer}%</color> der verbündeten Einheiten innerhalb des Rauchs erhöht.";

        ESSkillDesc = $"Detona una granada de humo que dura <color=yellow>{Count} segundos</color> en un área de <color=#AEEAFF>{Size}X{Size}</color> alrededor de ti, aumentando la defensa en <color=green>{IncDefenseAmount}</color> y la velocidad de ataque en <color=green>{IncAttackSpeedPer}%</color> de las unidades aliadas dentro del humo.";

        JASkillDesc = $"自身の周囲<color=#AEEAFF>{Size}X{Size}</color>範囲に<color=yellow>{Count}秒</color>間持続する煙幕弾を爆発させ、煙幕内にいる味方ユニットの防御力を<color=green>{IncDefenseAmount}</color>、攻撃速度を<color=green>{IncAttackSpeedPer}%</color>上昇させます。";

        PT_BRSkillDesc = $"Detona uma granada de fumaça que dura <color=yellow>{Count} segundos</color> em uma área de <color=#AEEAFF>{Size}X{Size}</color> ao seu redor, aumentando a defesa em <color=green>{IncDefenseAmount}</color> e a velocidade de ataque em <color=green>{IncAttackSpeedPer}%</color> das unidades aliadas dentro da fumaça.";

        RUSkillDesc = $"Взрывает дымовую гранату, которая длится <color=yellow>{Count} секунд</color> в области <color=#AEEAFF>{Size}X{Size}</color> вокруг вас, увеличивая защиту на <color=green>{IncDefenseAmount}</color> и скорость атаки на <color=green>{IncAttackSpeedPer}%</color> союзных единиц внутри дыма.";

        ZH_HANSSkillDesc = $"在自身周围的<color=#AEEAFF>{Size}X{Size}</color>范围内引爆持续<color=yellow>{Count}秒</color>的烟雾弹，增加烟雾内友军单位的防御力<color=green>{IncDefenseAmount}</color>和攻击速度<color=green>{IncAttackSpeedPer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "방어력 증가량 <color=green>+5</color>\n" +
                   "공격 속도 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Defense increase <color=green>+5</color>\n" +
                   "Attack speed increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de défense <color=green>+5</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento difesa <color=green>+5</color>\n" +
                   "Aumento velocità attacco <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Erhöhung der Verteidigung <color=green>+5</color>\n" +
                   "Erhöhung der Angriffsgeschwindigkeit <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de defensa <color=green>+5</color>\n" +
                   "Aumento de velocidad de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "防御力増加 <color=green>+5</color>\n" +
                   "攻撃速度増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de defesa <color=green>+5</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение защиты <color=green>+5</color>\n" +
                   "Увеличение скорости атаки <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "防御力增加 <color=green>+5</color>\n" +
                   "攻击速度增加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Defense increase <color=green>+5</color>\n" +
               "Attack speed increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.2</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Duration <color=yellow>+1</color>";
    }
}