using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Tracker : UnitDefault
{
    [Header("추적")]
    public float incHealthAmount;
    public float incAttackAmount;
    public float incDefenseAmount;
    public float duration;

    float IncHealthAmount { get => incHealthAmount + StaticManager.skillPoint[unitIdx] * 10f; }
    float IncAttackAmount { get => incAttackAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float IncDefenseAmount { get => incDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }

    bool isRush;
    bool isTracker;
    WaitForSeconds wait;
    Coroutine co;

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
    }

    protected override void FixedUpdate()
    {
        if (isRush) {
            OnMove();
            IsFlipX();
            sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);

            if (Target == null || CheckForTargetRange(0.5f, Target) || isDeath) {
                isRush = false;

                SetStat(speedStat, 3f, false, false);
                rigid2D.mass = 1f;

                SetStat(attackPowerStat, IncAttackAmount, false, true, duration);
                SetStat(defenseStat, IncDefenseAmount, false, true, duration);

                SetStateBar(State.incAttack, duration);
                SetStateBar(State.incDefense, duration);

                if (co != null) {
                    StopCoroutine(co);
                    co = null;
                }
                co = StartCoroutine(TrackerTime());
            }
        }
        else
            base.FixedUpdate();
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Healing(IncHealthAmount);

        RandomLongEnemy();
        if (SkillTarget == null)
            return;

        isRush = true;
        Target = SkillTarget;

        SetStat(speedStat, 3f, false, true);
        rigid2D.mass = 3f;
    }

    void RandomLongEnemy()
    {
        List<UnitDefault> longEnemies = new();
        foreach (UnitDefault enemy in enemies) {
            if (enemy.isKitingUnit)
                longEnemies.Add(enemy);
        }

        if (longEnemies.Count == 0) {
            SkillTarget = null;
        }
        else {
            SkillTarget = longEnemies[Random.Range(0, longEnemies.Count)].gameObject;
        }
    }

    IEnumerator TrackerTime()
    {
        isTracker = true;
        yield return wait;
        isTracker = false;

        co = null;
    }

    protected override void OnMove()
    {
        if ((!isRush && CheckForTargetRange(AttackRange, Target)) || isStunAnim || isSkillAnim || isDeath || isKitingAnim || isMovingAnim || isBondage || isAttackAnim)
            return;

        // 타겟이 없으면 Idle 상태로 전환
        if (Target == null) {
            animator.speed = 1f;
            animator.SetBool("1_Move", false);
            return;
        }

        // 타겟 방향으로 이동속도의 속도로 이동
        animator.speed = Speed;
        animator.SetBool("1_Move", true);
        moveSoundCoroutine ??= StartCoroutine(MoveSound());
        transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, Speed * Time.fixedDeltaTime);
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        SoundPlay(attackSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, isTracker);
    }

    protected override IEnumerator OnMovingAnim()
    {
        if (isTracker) {
            isMovingAnim = false;
            yield break;
        }

        yield return base.OnMovingAnim();
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
                isSkillAvaliable = true;
            }
        }
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
        skillDesc = $"마나가 모두 차면 체력을 <color=green>{IncHealthAmount}</color>만큼 회복하고 랜덤한 원거리 적 유닛에게 돌진하며, 돌진이 끝나면 <color=yellow>{duration}초</color> 동안 공격력이 <color=green>{IncAttackAmount}</color>, 방어력이 <color=green>{IncDefenseAmount}</color>만큼 증가합니다.";

        ENSkillDesc = $"When mana is full, restores <color=green>{IncHealthAmount}</color> health and rushes to a random ranged enemy unit. After the rush, increases attack power by <color=green>{IncAttackAmount}</color> and defense by <color=green>{IncDefenseAmount}</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Lorsque la mana est pleine, restaure <color=green>{IncHealthAmount}</color> points de vie et se précipite vers une unité ennemie à distance aléatoire. Après la ruée, augmente la puissance d'attaque de <color=green>{IncAttackAmount}</color> et la défense de <color=green>{IncDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Quando la mana è piena, ripristina <color=green>{IncHealthAmount}</color> punti salute e si precipita verso un'unità nemica a distanza casuale. Dopo la carica, aumenta la potenza d'attacco di <color=green>{IncAttackAmount}</color> e la difesa di <color=green>{IncDefenseAmount}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Wenn die Mana voll ist, stellt sie <color=green>{IncHealthAmount}</color> Gesundheit wieder her und stürzt sich auf eine zufällige Fernkampf-Feindeinheit. Nach dem Ansturm werden Angriffskraft um <color=green>{IncAttackAmount}</color> und Verteidigung um <color=green>{IncDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color> erhöht.";

        ESSkillDesc = $"Cuando el maná está lleno, restaura <color=green>{IncHealthAmount}</color> de salud y se lanza hacia una unidad enemiga a distancia aleatoria. Después de la carrera, aumenta el poder de ataque en <color=green>{IncAttackAmount}</color> y la defensa en <color=green>{IncDefenseAmount}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"マナが満タンになると、体力を<color=green>{IncHealthAmount}</color>回復し、ランダムな遠距離の敵ユニットに突進します。突進が終了すると、<color=yellow>{duration}秒</color>間、攻撃力が<color=green>{IncAttackAmount}</color>、防御力が<color=green>{IncDefenseAmount}</color>増加します。";

        PT_BRSkillDesc = $"Quando o mana está cheio, restaura <color=green>{IncHealthAmount}</color> de saúde e avança para uma unidade inimiga de longo alcance aleatória. Após a investida, aumenta o poder de ataque em <color=green>{IncAttackAmount}</color> e a defesa em <color=green>{IncDefenseAmount}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Когда мана полна, восстанавливает <color=green>{IncHealthAmount}</color> здоровья и бросается к случайной дальнобойной вражеской единице. После рывка увеличивает силу атаки на <color=green>{IncAttackAmount}</color> и защиту на <color=green>{IncDefenseAmount}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"当法力值充满时，恢复<color=green>{IncHealthAmount}</color>点生命值并冲向一个随机的远程敌方单位。冲锋结束后，攻击力提高<color=green>{IncAttackAmount}</color>，防御力提高<color=green>{IncDefenseAmount}</color>，持续<color=yellow>{duration}秒</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "체력 증가량 <color=green>+10</color>\n" +
                   "공격력 증가량 <color=green>+5</color>\n" +
                   "방어력 증가량 <color=green>+5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Health increase <color=green>+10</color>\n" +
                   "Attack increase <color=green>+5</color>\n" +
                   "Defense increase <color=green>+5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de santé <color=green>+10</color>\n" +
                   "Augmentation d'attaque <color=green>+5</color>\n" +
                   "Augmentation de défense <color=green>+5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento salute <color=green>+10</color>\n" +
                   "Aumento attacco <color=green>+5</color>\n" +
                   "Aumento difesa <color=green>+5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Gesundheitszuwachs <color=green>+10</color>\n" +
                   "Angriffszuwachs <color=green>+5</color>\n" +
                   "Verteidigungszuwachs <color=green>+5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de salud <color=green>+10</color>\n" +
                   "Aumento de ataque <color=green>+5</color>\n" +
                   "Aumento de defensa <color=green>+5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "体力増加 <color=green>+10</color>\n" +
                   "攻撃力増加 <color=green>+5</color>\n" +
                   "防御力増加 <color=green>+5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de vida <color=green>+10</color>\n" +
                   "Aumento de ataque <color=green>+5</color>\n" +
                   "Aumento de defesa <color=green>+5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение здоровья <color=green>+10</color>\n" +
                   "Увеличение атаки <color=green>+5</color>\n" +
                   "Увеличение защиты <color=green>+5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "生命值增加 <color=green>+10</color>\n" +
                   "攻击力增加 <color=green>+5</color>\n" +
                   "防御力增加 <color=green>+5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Health increase <color=green>+10</color>\n" +
               "Attack increase <color=green>+5</color>\n" +
               "Defense increase <color=green>+5</color>";
    }
}