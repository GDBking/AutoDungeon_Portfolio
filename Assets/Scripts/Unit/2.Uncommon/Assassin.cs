using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Assassin : UnitDefault
{
    [Header("기습")]
    public float attackCoeff;
    public float incLifeStealPer;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.15f; }
    float IncLifeStealPer { get => incLifeStealPer + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }

    bool isAssassin;
    WaitForSeconds wait;
    bool isBuff;

    protected override void Start()
    {
        base.Start();

        wait = new(Duration);
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        CreateAttackBox(-1f, null, transform.position);
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        StartCoroutine(BuffTime());

        SoundPlay(skillSoundClip);

        SetStat(lifeStealPerStat, IncLifeStealPer, false, true, Duration);
        SetStateBar(State.incLifeSteal, Duration);

        FindFarthestEnemy(enemies);
        if (SkillTarget == null)
            return;

        Vector2 targetPos = SkillTarget.transform.position;
        targetPos += transform.position.x < targetPos.x ? Vector2.right * 0.5f : Vector2.left * 0.5f;
        transform.position = targetPos;

        CreateAttackBox(-1f, null, transform.position);
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
                isAssassin = true;
                isSkillAvaliable = true;
                yield break;
            }
        }
    }

    public override void SetMana(int amount)
    {
        if (isAssassin)
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

    IEnumerator BuffTime()
    {
        isBuff = true;
        yield return wait;
        isBuff = false;
    }

    protected override IEnumerator OnMovingAnim()
    {
        if (isBuff) {
            isMovingAnim = false;
            yield break;
        }

        yield return base.OnMovingAnim();
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        if (target != null && isBuff && target.TryGetComponent<UnitDefault>(out UnitDefault unitComp) && unitComp.isKitingUnit)
            return base.CreateAttackBox(attackPower * AttackCoeff, target, targetPos, attackStyle, isSkill, isPenetration);
        else
            return base.CreateAttackBox(attackPower, target, targetPos, attackStyle, isSkill, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"마나가 모두 차면 본인 기준 가장 멀리 있는 적의 뒤로 순간이동하며, <color=yellow>{Duration}초</color> 동안 피해 흡혈이 <color=green>{IncLifeStealPer}%p</color> 증가하고, 공격 대상이 원거리 유닛일 경우 기본 공격이 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다." +
                    $"(스폰당 1회 사용 가능)";

        ENSkillDesc = $"When mana is full, teleports behind the farthest enemy from yourself, increasing life steal by <color=green>{IncLifeStealPer}%p</color> for <color=yellow>{Duration} seconds</color>, and dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power when the attack target is a ranged unit." +
                      $"(Can be used once per spawn)";

        FRSkillDesc = $"Lorsque la mana est pleine, se téléporte derrière l'ennemi le plus éloigné de vous, augmentant le vol de vie de <color=green>{IncLifeStealPer}%p</color> pendant <color=yellow>{Duration} secondes</color> et infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque lorsque la cible de l'attaque est une unité à distance." +
                      $"(Peut être utilisé une fois par apparition)";

        ITSkillDesc = $"Quando la mana è piena, si teletrasporta dietro il nemico più lontano da te, aumentando il furto di vita di <color=green>{IncLifeStealPer}%p</color> per <color=yellow>{Duration} secondi</color> e infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco quando l'obiettivo dell'attacco è un'unità a distanza." +
                      $"(Può essere usato una volta per spawn)";

        DESkillDesc = $"Wenn das Mana voll ist, teleportiert es sich hinter den am weitesten entfernten Feind von dir, erhöht die Lebensraub um <color=green>{IncLifeStealPer}%p</color> für <color=yellow>{Duration} Sekunden</color> und verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft, wenn das Angriffsziel eine Fernkampfeinheit ist." +
                      $"(Kann einmal pro Spawn verwendet werden)";

        ESSkillDesc = $"Cuando el maná está lleno, se teletransporta detrás del enemigo más lejano de ti, aumentando el robo de vida en <color=green>{IncLifeStealPer}%p</color> durante <color=yellow>{Duration} segundos</color> e infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque cuando el objetivo del ataque es una unidad a distancia." +
                      $"(Se puede usar una vez por aparición)";

        JASkillDesc = $"マナが満タンになると、自分から最も遠い敵の後ろに瞬間移動し、<color=yellow>{Duration}秒</color>間、被ダメージ吸収が<color=green>{IncLifeStealPer}%p</color>増加し、攻撃対象が遠距離ユニットの場合、通常攻撃が攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。" +
                      $"(スポーンごとに1回使用可能)";

        PT_BRSkillDesc = $"Quando a mana está cheia, teleporta-se atrás do inimigo mais distante de você, aumentando o roubo de vida em <color=green>{IncLifeStealPer}%p</color> por <color=yellow>{Duration} segundos</color> e causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque quando o alvo do ataque é uma unidade de longo alcance." +
                         $"(Pode ser usado uma vez por spawn)";

        RUSkillDesc = $"Когда мана полна, телепортируется за самого дальнего от вас врага, увеличивая кражу жизни на <color=green>{IncLifeStealPer}%p</color> на <color=yellow>{Duration} секунд</color> и нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки, когда целью атаки является юнит дальнего боя." +
                      $"(Можно использовать один раз за спавн)";

        ZH_HANSSkillDesc = $"当法力值充满时，传送到离你最远的敌人身后，在<color=yellow>{Duration}秒</color>内将生命偷取提高<color=green>{IncLifeStealPer}%p</color>，当攻击目标是远程单位时，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。" +
                           $"（每次生成可使用一次）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.15</color>\n" +
                   "피해 흡혈 증가량 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.15</color>\n" +
                   "Life steal increase <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.15</color>\n" +
                   "Augmentation de vol de vie <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.15</color>\n" +
                   "Aumento furto di vita <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.15</color>\n" +
                   "Lebensraub Erhöhung <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "Aumento de robo de vida <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.15</color>\n" +
                   "吸血増加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "Aumento de roubo de vida <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.15</color>\n" +
                   "Увеличение вампиризма <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.15</color>\n" +
                   "吸血增加 <color=green>+5%p</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.15</color>\n" +
               "Life steal increase <color=green>+5%p</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}