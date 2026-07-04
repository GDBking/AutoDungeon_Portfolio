using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Berserker : UnitDefault
{
    [Header("광폭")]
    public float incDefensePer;
    public float incAttackSpeedPer;
    public float incLifeStealPer;
    public float incAttackPer;

    float IncDefensePer { get => incDefensePer + StaticManager.skillPoint[unitIdx] * 3f; }
    float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 2f; }
    float IncLifeStealPer { get => incLifeStealPer + StaticManager.skillPoint[unitIdx] * 2f; }
    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 3f; }

    bool isBerserk;

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());

        SetStat(defenseStat, IncDefensePer, true, true);
        SetStat(attackSpeedStat, IncAttackSpeedPer, true, true);
        SetStat(lifeStealPerStat, IncLifeStealPer, false, true);
        SetStat(attackPowerStat, IncAttackPer, true, true);

        SetStateBar(State.berserk, -2f);
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        if (!isBerserk)
            SoundPlay(attackSoundClip);
        else
            SoundPlay(skillSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, isBerserk);
    }

    protected override IEnumerator ChargingMana()
    {
        WaitForSeconds wait = new(1f);

        while (true) {
            yield return wait;

            while (isSilence)
                yield return new WaitForSeconds(silenceRemainingTime);

            updateMpBarAction(MaxMana, ++CurrentMana); // 마나바 최신화
            UpdateStatInfo();

            if (CurrentMana == MaxMana) {
                isBerserk = true;
                isSkillAvaliable = true;
                yield break;
            }
        }
    }

    public override void SetMana(int amount)
    {
        if (isBerserk)
            return;

        base.SetMana(amount);
    }

    protected override void SkillUseAfterState()
    {
        isSkillAvaliable = false;
        CurrentMana = 0;
        updateMpBarAction(MaxMana, CurrentMana);
        UpdateStatInfo();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"마나가 모두 차면 본인의 방어력이 <color=green>{IncDefensePer}%</color>, 공격 속도가 <color=green>{IncAttackSpeedPer}%</color>, 피해 흡혈이 <color=green>{IncLifeStealPer}%p</color>, 공격력이 <color=green>{IncAttackPer}%</color> 증가합니다.\n" +
                    $"(스폰당 1회 사용 가능)";
        
        ENSkillDesc = $"When mana is full, increases own defense by <color=green>{IncDefensePer}%</color>, attack speed by <color=green>{IncAttackSpeedPer}%</color>, life steal by <color=green>{IncLifeStealPer}%p</color>, and attack power by <color=green>{IncAttackPer}%</color>.\n" +
                      $"(Can be used once per spawn)";

        FRSkillDesc = $"Lorsque la mana est pleine, augmente la défense de <color=green>{IncDefensePer}%</color>, la vitesse d'attaque de <color=green>{IncAttackSpeedPer}%</color>, le vol de vie de <color=green>{IncLifeStealPer}%p</color> et la puissance d'attaque de <color=green>{IncAttackPer}%</color>." +
                      $"\n(Peut être utilisé une fois par apparition)";

        ITSkillDesc = $"Quando la mana è piena, aumenta la difesa di <color=green>{IncDefensePer}%</color>, la velocità di attacco di <color=green>{IncAttackSpeedPer}%</color>, il furto di vita di <color=green>{IncLifeStealPer}%p</color> e la potenza d'attacco di <color=green>{IncAttackPer}%</color>." +
                      $"\n(Può essere usato una volta per spawn)";

        DESkillDesc = $"Wenn das Mana voll ist, erhöht sich die Verteidigung um <color=green>{IncDefensePer}%</color>, die Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedPer}%</color>, die Lebensraub um <color=green>{IncLifeStealPer}%p</color> und die Angriffskraft um <color=green>{IncAttackPer}%</color>." +
                      $"\n(Kann einmal pro Spawn verwendet werden)";

        ESSkillDesc = $"Cuando el maná está lleno, aumenta la defensa en <color=green>{IncDefensePer}%</color>, la velocidad de ataque en <color=green>{IncAttackSpeedPer}%</color>, el robo de vida en <color=green>{IncLifeStealPer}%p</color> y el poder de ataque en <color=green>{IncAttackPer}%</color>." +
                      $"\n(Se puede usar una vez por aparición)";

        JASkillDesc = $"マナが満タンになると、自身の防御力が<color=green>{IncDefensePer}%</color>、攻撃速度が<color=green>{IncAttackSpeedPer}%</color>、ライフスティールが<color=green>{IncLifeStealPer}%p</color>、攻撃力が<color=green>{IncAttackPer}%</color>だけ増加します。\n" +
                      $"(スポーンごとに1回使用可能)";

        PT_BRSkillDesc = $"Quando sua mana está cheia, aumenta a defesa em <color=green>{IncDefensePer}%</color>, a velocidade de ataque em <color=green>{IncAttackSpeedPer}%</color>, o roubo de vida em <color=green>{IncLifeStealPer}%p</color> e o poder de ataque em <color=green>{IncAttackPer}%</color>." +
                         $"\n(Pode ser usado uma vez por spawn)";

        RUSkillDesc = $"Когда ваша мана полна, увеличивает защиту на <color=green>{IncDefensePer}%</color>, скорость атаки на <color=green>{IncAttackSpeedPer}%</color>, кражу жизни на <color=green>{IncLifeStealPer}%p</color> и силу атаки на <color=green>{IncAttackPer}%</color>." +
                      $"\n(Можно использовать один раз за спавн)";

        ZH_HANSSkillDesc = $"当你的法力值充满时，防御力提高<color=green>{IncDefensePer}%</color>，攻击速度提高<color=green>{IncAttackSpeedPer}%</color>，生命偷取提高<color=green>{IncLifeStealPer}%p</color>，攻击力提高<color=green>{IncAttackPer}%</color>。\n" +
                           $"(每次生成可使用一次)";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "방어력 증가량 <color=green>+3%p</color>\n" +
                   "공격 속도 증가량 <color=green>+2%p</color>\n" +
                   "피해 흡혈 증가량 <color=green>+2%p</color>\n" +
                   "공격력 증가량 <color=green>+3%p</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Defense increase <color=green>+3%p</color>\n" +
                   "Attack speed increase <color=green>+2%p</color>\n" +
                   "Lifesteal increase <color=green>+2%p</color>\n" +
                   "Attack increase <color=green>+3%p</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de la défense <color=green>+3%p</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+2%p</color>\n" +
                   "Vol de vie <color=green>+2%p</color>\n" +
                   "Augmentation de l'attaque <color=green>+3%p</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento difesa <color=green>+3%p</color>\n" +
                   "Aumento velocità attacco <color=green>+2%p</color>\n" +
                   "Vampirismo <color=green>+2%p</color>\n" +
                   "Aumento attacco <color=green>+3%p</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Verteidigungssteigerung <color=green>+3%p</color>\n" +
                   "Angriffsgeschwindigkeit + <color=green>+2%p</color>\n" +
                   "Lebensraub <color=green>+2%p</color>\n" +
                   "Angriffserhöhung <color=green>+3%p</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de defensa <color=green>+3%p</color>\n" +
                   "Aumento de velocidad de ataque <color=green>+2%p</color>\n" +
                   "Robo de vida <color=green>+2%p</color>\n" +
                   "Aumento de ataque <color=green>+3%p</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "防御力増加量 <color=green>+3%p</color>\n" +
                   "攻撃速度増加量 <color=green>+2%p</color>\n" +
                   "ダメージ吸収増加量 <color=green>+2%p</color>\n" +
                   "攻撃力増加量 <color=green>+3%p</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de defesa <color=green>+3%p</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+2%p</color>\n" +
                   "Roubo de vida <color=green>+2%p</color>\n" +
                   "Aumento de ataque <color=green>+3%p</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение защиты <color=green>+3%p</color>\n" +
                   "Увеличение скорости атаки <color=green>+2%p</color>\n" +
                   "Вампиризм <color=green>+2%p</color>\n" +
                   "Увеличение атаки <color=green>+3%p</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "防御增加量 <color=green>+3%p</color>\n" +
                   "攻击速度增加量 <color=green>+2%p</color>\n" +
                   "吸血增加量 <color=green>+2%p</color>\n" +
                   "攻击力增加量 <color=green>+3%p</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Defense increase <color=green>+3%p</color>\n" +
               "Attack speed increase <color=green>+2%p</color>\n" +
               "Lifesteal increase <color=green>+2%p</color>\n" +
               "Attack increase <color=green>+3%p</color>";
    }
}