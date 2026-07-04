using UnityEngine;

public class Digestion : UnitDefault
{
    [Header("소화")]
    public float incHealthAmount;
    public float incDefenseAmount;
    public float duration;

    protected override void UseSkill()
    {
        if (Health == MaxHealth && friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, transform.position);

        Healing(incHealthAmount);
        SetStat(defenseStat, incDefenseAmount, false, true, duration);
        SetStateBar(State.incDefense, duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"체력을 <color=green>{incHealthAmount}</color>만큼 회복하며, <color=yellow>{duration}초</color> 동안 방어력이 <color=green>{incDefenseAmount}</color>만큼 증가합니다.";

        ENSkillDesc = $"Heals for <color=green>{incHealthAmount}</color> and increases defense by <color=green>{incDefenseAmount}</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Restaure <color=green>{incHealthAmount}</color> de santé et augmente la défense de <color=green>{incDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Guarisce di <color=green>{incHealthAmount}</color> e aumenta la difesa di <color=green>{incDefenseAmount}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Heilt um <color=green>{incHealthAmount}</color> und erhöht die Verteidigung um <color=green>{incDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Cura <color=green>{incHealthAmount}</color> y aumenta la defensa en <color=green>{incDefenseAmount}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"体力を<color=green>{incHealthAmount}</color>回復し、<color=yellow>{duration}秒</color>の間防御が<color=green>{incDefenseAmount}</color>増加します。";

        PT_BRSkillDesc = $"Cura <color=green>{incHealthAmount}</color> e aumenta a defesa em <color=green>{incDefenseAmount}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Исцеляет на <color=green>{incHealthAmount}</color> и увеличивает защиту на <color=green>{incDefenseAmount}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"恢复 <color=green>{incHealthAmount}</color> 点生命，并在 <color=yellow>{duration} 秒</color>内将防御提升 <color=green>{incDefenseAmount}</color>。";
    }
}