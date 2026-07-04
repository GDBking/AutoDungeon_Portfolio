using System.Collections;
using UnityEngine;

public class Fury : UnitDefault
{
    [Header("격앙")]
    public float incAttackAmount;
    public float incAttackSpeedAmount;
    public float duration;

    bool isFury;
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
        SetStat(attackPowerStat, incAttackAmount, false, true, duration);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true, duration);

        SetStateBar(State.incAttack, duration);
        SetStateBar(State.incAttackSpeed, duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(FuryTime());
    }

    IEnumerator FuryTime()
    {
        isFury = true;
        yield return wait;
        isFury = false;

        co = null;
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        if (!isFury)
            SoundPlay(attackSoundClip);
        else
            SoundPlay(skillSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, isFury);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 본인의 공격력이 <color=green>{incAttackAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>만큼 증가합니다.";

        ENSkillDesc = $"Increases this unit's attack by <color=green>{incAttackAmount}</color> and attack speed by <color=green>{incAttackSpeedAmount}</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Augmente l'attaque de cette unité de <color=green>{incAttackAmount}</color> et la vitesse d'attaque de <color=green>{incAttackSpeedAmount}</color> pour <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Aumenta l'attacco di questa unità di <color=green>{incAttackAmount}</color> e la velocità d'attacco di <color=green>{incAttackSpeedAmount}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Erhöht den Angriff dieser Einheit um <color=green>{incAttackAmount}</color> und die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Aumenta el ataque de esta unidad en <color=green>{incAttackAmount}</color> y la velocidad de ataque en <color=green>{incAttackSpeedAmount}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"<color=yellow>{duration}秒</color>の間、自身の攻撃力が<color=green>{incAttackAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>だけ上昇します。";

        PT_BRSkillDesc = $"Aumenta o ataque desta unidade em <color=green>{incAttackAmount}</color> e a velocidade de ataque em <color=green>{incAttackSpeedAmount}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Увеличивает атаку этой единицы на <color=green>{incAttackAmount}</color> и скорость атаки на <color=green>{incAttackSpeedAmount}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"在<color=yellow>{duration}秒</color>内，提升自身的攻击力<color=green>{incAttackAmount}</color>和攻击速度<color=green>{incAttackSpeedAmount}</color>。";
    }
}
