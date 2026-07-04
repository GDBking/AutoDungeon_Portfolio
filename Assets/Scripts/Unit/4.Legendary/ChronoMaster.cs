using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChronoMaster : UnitDefault
{
    [Header("시간 왜곡")]
    public float speedPer;
    public float attackSpeedPer;
    public float duration;

    float SpeedPer { get => Mathf.Min(speedPer + StaticManager.skillPoint[unitIdx] * 3f, 99f); }
    float AttackSpeedPer { get => Mathf.Min(attackSpeedPer + StaticManager.skillPoint[unitIdx] * 3f, 99f); }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }


    protected override void Awake()
    {
        base.Awake();

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
        if (enemies.Count == 0)
            return;

        SoundPlay(skillSoundClip);

        foreach (UnitDefault enemy in enemies) {
            GameObject effect = Instantiate(attackObj, enemy.transform.position, Quaternion.identity, enemy.transform);
            effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            enemy.SetStat(enemy.speedStat, SpeedPer, true, false, Duration);
            enemy.SetStat(enemy.attackSpeedStat, AttackSpeedPer, true, false, Duration);

            enemy.SetStateBar(State.decSpeed, Duration);
            enemy.SetStateBar(State.decAttackSpeed, Duration);
        }

        foreach (UnitDefault friend in friends) {
            GameObject effect = Instantiate(attackObj, friend.transform.position, Quaternion.identity, friend.transform);
            effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            friend.SetStat(friend.speedStat, SpeedPer, true, true, Duration);
            friend.SetStat(friend.attackSpeedStat, AttackSpeedPer, true, true, Duration);

            friend.SetStateBar(State.incSpeed, Duration);
            friend.SetStateBar(State.incAttackSpeed, Duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 모든 적 유닛들의 이동 속도를 <color=red>{SpeedPer}%</color>, 공격 속도를 <color=red>{AttackSpeedPer}%</color> 감소시키고, 모든 아군 유닛들의 이동 속도를 <color=green>{SpeedPer}%</color>, 공격 속도를 <color=green>{AttackSpeedPer}%</color> 증가시킵니다.";
        
        ENSkillDesc = $"For <color=yellow>{Duration} seconds</color>, decreases the movement speed of all enemy units by <color=red>{SpeedPer}%</color> and attack speed by <color=red>{AttackSpeedPer}%</color>, while increasing the movement speed of all allied units by <color=green>{SpeedPer}%</color> and attack speed by <color=green>{AttackSpeedPer}%</color>.";

        FRSkillDesc = $"Pendant <color=yellow>{Duration} secondes</color>, diminue la vitesse de déplacement de toutes les unités ennemies de <color=red>{SpeedPer}%</color> et la vitesse d'attaque de <color=red>{AttackSpeedPer}%</color>, tout en augmentant la vitesse de déplacement de toutes les unités alliées de <color=green>{SpeedPer}%</color> et la vitesse d'attaque de <color=green>{AttackSpeedPer}%</color>.";

        ITSkillDesc = $"Per <color=yellow>{Duration} secondi</color>, diminuisce la velocità di movimento di tutte le unità nemiche del <color=red>{SpeedPer}%</color> e la velocità di attacco del <color=red>{AttackSpeedPer}%</color>, aumentando al contempo la velocità di movimento di tutte le unità alleate del <color=green>{SpeedPer}%</color> e la velocità di attacco del <color=green>{AttackSpeedPer}%</color>.";

        DESkillDesc = $"Verringert für <color=yellow>{Duration} Sekunden</color> die Bewegungsgeschwindigkeit aller feindlichen Einheiten um <color=red>{SpeedPer}%</color> und die Angriffsgeschwindigkeit um <color=red>{AttackSpeedPer}%</color>, während die Bewegungsgeschwindigkeit aller verbündeten Einheiten um <color=green>{SpeedPer}%</color> und die Angriffsgeschwindigkeit um <color=green>{AttackSpeedPer}%</color> erhöht wird.";

        ESSkillDesc = $"Durante <color=yellow>{Duration} segundos</color>, disminuye la velocidad de movimiento de todas las unidades enemigas en <color=red>{SpeedPer}%</color> y la velocidad de ataque en <color=red>{AttackSpeedPer}%</color>, mientras que aumenta la velocidad de movimiento de todas las unidades aliadas en <color=green>{SpeedPer}%</color> y la velocidad de ataque en <color=green>{AttackSpeedPer}%</color>.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>間、すべての敵ユニットの移動速度を<color=red>{SpeedPer}%</color>、攻撃速度を<color=red>{AttackSpeedPer}%</color>減少させ、すべての味方ユニットの移動速度を<color=green>{SpeedPer}%</color>、攻撃速度を<color=green>{AttackSpeedPer}%</color>増加させます。";

        PT_BRSkillDesc = $"Por <color=yellow>{Duration} segundos</color>, disminuye la velocidad de movimiento de todas las unidades enemigas en <color=red>{SpeedPer}%</color> y la velocidad de ataque en <color=red>{AttackSpeedPer}%</color>, mientras que aumenta la velocidad de movimiento de todas las unidades aliadas en <color=green>{SpeedPer}%</color> y la velocidad de ataque en <color=green>{AttackSpeedPer}%</color>.";

        RUSkillDesc = $"В течение <color=yellow>{Duration} секунд</color> уменьшает скорость передвижения всех вражеских юнитов на <color=red>{SpeedPer}%</color> и скорость атаки на <color=red>{AttackSpeedPer}%</color>, одновременно увеличивая скорость передвижения всех союзных юнитов на <color=green>{SpeedPer}%</color> и скорость атаки на <color=green>{AttackSpeedPer}%</color>.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内，所有敌方单位的移动速度降低<color=red>{SpeedPer}%</color>，攻击速度降低<color=red>{AttackSpeedPer}%</color>，而所有友军单位的移动速度提高<color=green>{SpeedPer}%</color>，攻击速度提高<color=green>{AttackSpeedPer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "이동 속도 감소/증가량 <color=red>+3%p</color>\n" +
                   "공격 속도 감소/증가량 <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Movement speed change <color=red>+3%p</color>\n" +
                   "Attack speed change <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Variation de la vitesse de déplacement <color=red>+3%p</color>\n" +
                   "Variation de la vitesse d'attaque <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Variazione velocità movimento <color=red>+3%p</color>\n" +
                   "Variazione velocità attacco <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Bewegungsgeschwindigkeitsänderung <color=red>+3%p</color>\n" +
                   "Angriffsgeschwindigkeitsänderung <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Cambio de velocidad de movimiento <color=red>+3%p</color>\n" +
                   "Cambio de velocidad de ataque <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "移動速度減少/増加量 <color=red>+3%p</color>\n" +
                   "攻撃速度減少/増加量 <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Variação de velocidade de movimento <color=red>+3%p</color>\n" +
                   "Variação de velocidade de ataque <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Изменение скорости движения <color=red>+3%p</color>\n" +
                   "Изменение скорости атаки <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "移动速度变化 <color=red>+3%p</color>\n" +
                   "攻击速度变化 <color=red>+3%p</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Movement speed change <color=red>+3%p</color>\n" +
               "Attack speed change <color=red>+3%p</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}