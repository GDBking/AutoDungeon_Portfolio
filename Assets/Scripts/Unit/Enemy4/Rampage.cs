using System.Collections;
using UnityEngine;

public class Rampage : UnitDefault
{
    [Header("광란")]
    public float incAttackPer;
    public float incAttackSpeedPer;
    public float incSpeedPer;
    public float duration;
    public float stunDuration;

    bool isRampage;
    WaitForSeconds wait;
    Coroutine co;

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStat(attackPowerStat, incAttackPer, true, true, duration);
        SetStat(attackSpeedStat, incAttackSpeedPer, true, true, duration);
        SetStat(speedStat, incSpeedPer, true, true, duration);

        SetStateBar(State.incAttack, duration);
        SetStateBar(State.incAttackSpeed, duration);
        SetStateBar(State.incSpeed, duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(RampageTime());
    }

    IEnumerator RampageTime()
    {
        isRampage = true;
        yield return wait;
        OnStunAnim(stunDuration);
        isRampage = false;

        co = null;
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        if (!isRampage)
            SoundPlay(attackSoundClip);
        else
            SoundPlay(skillSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, isRampage);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;
        isAttackAnim = true;
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Attack");
        yield return null;
        // 공격 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);

        if (!isStunAnim && !isDeath && !isBondage && !isTaunt && Random.Range(1, 101) <= 40 && !isRampage) {
            isMovingAnim = true;
            StartCoroutine(OnMovingAnim()); // 무빙 애니메이션 실행
        }
        isAttackAnim = false;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 광란하여 본인의 공격력이 <color=green>{incAttackPer}%</color>, 공격 속도가 <color=green>{incAttackSpeedPer}%</color>, 이동 속도가 <color=green>{incSpeedPer}%</color> 증가합니다.\n" +
                    $"광란이 끝나면 본인은 <color=yellow>{stunDuration}초</color> 동안 기절합니다.";

        ENSkillDesc = $"Goes into a rampage for <color=yellow>{duration} seconds</color>, increasing this unit's attack by <color=green>{incAttackPer}%</color>, attack speed by <color=green>{incAttackSpeedPer}%</color>, and movement speed by <color=green>{incSpeedPer}%</color>.\n" +
                      $"When rampage ends, this unit is stunned for <color=yellow>{stunDuration} seconds</color>.";

        FRSkillDesc = $"Gerät in Raserei für <color=yellow>{duration} Sekunden</color> und erhöht den Angriff dieser Einheit um <color=green>{incAttackPer}%</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> und die Bewegungsgeschwindigkeit um <color=green>{incSpeedPer}%</color>.\n" +
                      $"Wenn die Raserei endet, ist diese Einheit für <color=yellow>{stunDuration} Sekunden</color> betäubt.";

        ITSkillDesc = $"Entra in frenesia per <color=yellow>{duration} secondi</color>, aumentando l'attacco di questa unità di <color=green>{incAttackPer}%</color>, la velocità d'attacco di <color=green>{incAttackSpeedPer}%</color> e la velocità di movimento di <color=green>{incSpeedPer}%</color>.\n" +
                      $"Al termine della frenesia, questa unità è stordita per <color=yellow>{stunDuration} secondi</color>.";

        DESkillDesc = $"Gelangt für <color=yellow>{duration} Sekunden</color> in Raserei und erhöht den Angriff dieser Einheit um <color=green>{incAttackPer}%</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> und die Bewegungsgeschwindigkeit um <color=green>{incSpeedPer}%</color>.\n" +
                      $"Wenn die Raserei endet, ist diese Einheit für <color=yellow>{stunDuration} Sekunden</color> betäubt.";

        ESSkillDesc = $"Entra en una furia durante <color=yellow>{duration} segundos</color>, aumentando el ataque de esta unidad en <color=green>{incAttackPer}%</color>, la velocidad de ataque en <color=green>{incAttackSpeedPer}%</color> y la velocidad de movimiento en <color=green>{incSpeedPer}%</color>.\n" +
                      $"Cuando termina la furia, esta unidad queda aturdida durante <color=yellow>{stunDuration} segundos</color>.";

        JASkillDesc = $"<color=yellow>{duration}秒</color>の間激怒し、このユニットの攻撃が<color=green>{incAttackPer}%</color>、攻撃速度が<color=green>{incAttackSpeedPer}%</color>、移動速度が<color=green>{incSpeedPer}%</color>増加します。\n" +
                      $"激怒が終わると、このユニットは<color=yellow>{stunDuration}秒</color>間気絶します。";

        PT_BRSkillDesc = $"Entra em fúria por <color=yellow>{duration} segundos</color>, aumentando o ataque desta unidade em <color=green>{incAttackPer}%</color>, a velocidade de ataque em <color=green>{incAttackSpeedPer}%</color> e a velocidade de movimento em <color=green>{incSpeedPer}%</color>.\n" +
                          $"Quando a fúria termina, esta unidade fica atordoada por <color=yellow>{stunDuration} segundos</color>.";

        RUSkillDesc = $"Входит в ярость на <color=yellow>{duration} секунд</color>, увеличивая атаку этой единицы на <color=green>{incAttackPer}%</color>, скорость атаки на <color=green>{incAttackSpeedPer}%</color> и скорость передвижения на <color=green>{incSpeedPer}%</color>.\n" +
                      $"Когда ярость заканчивается, эта единица оглушается на <color=yellow>{stunDuration} секунд</color>.";

        ZH_HANSSkillDesc = $"进入狂暴状态<color=yellow>{duration}秒</color>，使该单位的攻击力提高<color=green>{incAttackPer}%</color>，攻击速度提高<color=green>{incAttackSpeedPer}%</color>，移动速度提高<color=green>{incSpeedPer}%</color>。\n" +
                            $"狂暴结束后，该单位将眩晕<color=yellow>{stunDuration}秒</color>。";
    }
}