using UnityEngine;
using UnityEngine.Localization.Settings;

public class Silencer : UnitDefault
{
    [Header("사일런트")]
    public float incCriticalPer;
    public float decAttackAmount;
    public float decSpeedPer;
    public float duration;

    float IncCriticalPer { get => incCriticalPer + StaticManager.skillPoint[unitIdx] * 3f; }
    float DecAttackAmount { get => decAttackAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float DecSpeedPer { get => Mathf.Min(decSpeedPer + StaticManager.skillPoint[unitIdx] * 5f, 99f); }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5 * 0.5f; }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        foreach (UnitDefault enemy in enemies) {
            GameObject effect = Instantiate(attackObj, enemy.transform.position, Quaternion.identity, enemy.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            enemy.Silence(Duration);
            enemy.SetStat(enemy.attackPowerStat, DecAttackAmount, false, false, Duration);
            enemy.SetStat(enemy.speedStat, DecSpeedPer, true, false, Duration);
            enemy.SetStateBar(State.decAttack, Duration);
            enemy.SetStateBar(State.decSpeed, Duration);
        }

        foreach (UnitDefault friend in friends) {
            friend.SetStat(criticalPerStat, IncCriticalPer, false, true, Duration);
            friend.SetStateBar(State.incCriticalPer, Duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"적 전체에게 <color=yellow>{Duration}초</color> 동안 침묵을 걸고 공격력을 <color=red>{DecAttackAmount}</color>, 이동 속도를 <color=red>{DecSpeedPer}%</color> 감소시킵니다.\n" +
                    $"또한 모든 아군 유닛의 치명타 확률을 <color=green>{IncCriticalPer}%p</color> 증가시킵니다.";
        
        ENSkillDesc = $"Silences all enemies for <color=yellow>{Duration} seconds</color>, reducing their attack power by <color=red>{DecAttackAmount}</color> and movement speed by <color=red>{DecSpeedPer}%</color>.\n" +
                        $"Also increases the critical hit rate of all allied units by <color=green>{IncCriticalPer}%p</color>.";

        FRSkillDesc = $"Silence tous les ennemis pendant <color=yellow>{Duration} secondes</color>, réduisant leur puissance d'attaque de <color=red>{DecAttackAmount}</color> et leur vitesse de déplacement de <color=red>{DecSpeedPer}%</color>.\n" +
                      $"Augmente également la probabilité de coup critique de tous les alliés de <color=green>{IncCriticalPer}%p</color>.";

        ITSkillDesc = $"Silenzia tutti i nemici per <color=yellow>{Duration} secondi</color>, riducendo la loro potenza di attacco di <color=red>{DecAttackAmount}</color> e la velocità di movimento di <color=red>{DecSpeedPer}%</color>.\n" +
                      $"Aumenta inoltre la probabilità di colpo critico di tutti gli alleati del <color=green>{IncCriticalPer}%p</color>.";

        DESkillDesc = $"Schweigt alle Feinde für <color=yellow>{Duration} Sekunden</color> und verringert deren Angriffskraft um <color=red>{DecAttackAmount}</color> und Bewegungsgeschwindigkeit um <color=red>{DecSpeedPer}%</color>.\n" +
                        $"Erhöht außerdem die kritische Trefferchance aller Verbündeten um <color=green>{IncCriticalPer}%p</color>.";

        ESSkillDesc = $"Silencia a todos los enemigos durante <color=yellow>{Duration} segundos</color>, reduciendo su poder de ataque en <color=red>{DecAttackAmount}</color> y su velocidad de movimiento en <color=red>{DecSpeedPer}%</color>.\n" +
                      $"También aumenta la probabilidad de golpe crítico de todos los aliados en <color=green>{IncCriticalPer}%p</color>.";

        JASkillDesc = $"全ての敵に<color=yellow>{Duration}秒</color>間サイレンスをかけ、攻撃力を<color=red>{DecAttackAmount}</color>、移動速度を<color=red>{DecSpeedPer}%</color>減少させます。\n" +
                      $"また、全ての味方のクリティカル確率を<color=green>{IncCriticalPer}%p</color>増加させます。";

        PT_BRSkillDesc = $"Silencia todos os inimigos por <color=yellow>{Duration} segundos</color>, reduzindo seu poder de ataque em <color=red>{DecAttackAmount}</color> e sua velocidade de movimento em <color=red>{DecSpeedPer}%</color>.\n" +
                         $"Também aumenta a taxa de acerto crítico de todos os aliados em <color=green>{IncCriticalPer}%p</color>.";

        RUSkillDesc = $"Накладывает немоту на всех врагов на <color=yellow>{Duration} секунд</color>, снижая их силу атаки на <color=red>{DecAttackAmount}</color> и скорость передвижения на <color=red>{DecSpeedPer}%</color>.\n" +
                        $"Также увеличивает шанс критического удара всех союзных юнитов на <color=green>{IncCriticalPer}%p</color>.";

        ZH_HANSSkillDesc = $"对所有敌人施加<color=yellow>{Duration}秒</color>的沉默效果，降低其攻击力<color=red>{DecAttackAmount}</color>和移动速度<color=red>{DecSpeedPer}%</color>。\n" +
                           $"同时提升所有友方单位的暴击率<color=green>{IncCriticalPer}%p</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko"))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "치명타 확률 증가량 <color=green>+3%p</color>\n" +
                   "공격력 감소량 <color=red>+5</color>\n" +
                   "이동 속도 감소량 <color=red>+5%p</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en"))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Critical chance increase <color=green>+3 percentage points</color>\n" +
                   "Attack power decrease <color=red>+5</color>\n" +
                   "Movement speed decrease <color=red>+5 percentage points</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr"))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Chance de coup critique augmentée <color=green>+3 points de pourcentage</color>\n" +
                   "Attaque réduite <color=red>+5</color>\n" +
                   "Vitesse de déplacement réduite <color=red>+5 points de pourcentage</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it"))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Probabilità di colpo critico aumentata <color=green>+3 punti percentuali</color>\n" +
                   "Attacco ridotto <color=red>+5</color>\n" +
                   "Velocità di movimento ridotta <color=red>+5 punti percentuali</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de"))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Kritische Trefferchance erhöht <color=green>+3 Prozentpunkte</color>\n" +
                   "Angriff verringert <color=red>+5</color>\n" +
                   "Bewegungsgeschwindigkeit verringert <color=red>+5 Prozentpunkte</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es"))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Probabilidad de crítico aumentada <color=green>+3 puntos porcentuales</color>\n" +
                   "Ataque reducido <color=red>+5</color>\n" +
                   "Velocidad de movimiento reducida <color=red>+5 puntos porcentuales</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja"))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "クリティカル確率増加 <color=green>+3パーセンテージポイント</color>\n" +
                   "攻撃力減少 <color=red>+5</color>\n" +
                   "移動速度減少 <color=red>+5パーセンテージポイント</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt"))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Chance de crítico aumentada <color=green>+3 pontos percentuais</color>\n" +
                   "Ataque reduzido <color=red>+5</color>\n" +
                   "Velocidade de movimento reduzida <color=red>+5 pontos percentuais</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru"))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Шанс критического удара увеличен <color=green>+3 процентных пункта</color>\n" +
                   "Атака снижена <color=red>+5</color>\n" +
                   "Скорость передвижения снижена <color=red>+5 процентных пункта</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh"))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "暴击率提升 <color=green>+3 个百分点</color>\n" +
                   "攻击力降低 <color=red>+5</color>\n" +
                   "移动速度降低 <color=red>+5 个百分点</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Critical chance increase <color=green>+3 percentage points</color>\n" +
               "Attack power decrease <color=red>+5</color>\n" +
               "Movement speed decrease <color=red>+5 percentage points</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}