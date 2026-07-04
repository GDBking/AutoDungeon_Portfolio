using UnityEngine;

public class GoblinGuard : UnitDefault
{
    [Header("철벽 방어")]
    public float incDefenseAmount;
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        SetStat(defenseStat, incDefenseAmount, false, true, duration);
        SetStateBar(State.incDefense, duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 본인의 방어력이 <color=green>{incDefenseAmount}</color>만큼 증가합니다.";
        
        ENSkillDesc = $"Increases own defense by <color=green>{incDefenseAmount}</color> for <color=yellow>{duration} seconds</color>.";
        
        FRSkillDesc = $"Augmente sa défense de <color=green>{incDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color>.";
        
        ITSkillDesc = $"Aumenta la propria difesa di <color=green>{incDefenseAmount}</color> per <color=yellow>{duration} secondi</color>.";
        
        DESkillDesc = $"Erhöht die eigene Verteidigung um <color=green>{incDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color>.";
        
        ESSkillDesc = $"Aumenta la defensa propia en <color=green>{incDefenseAmount}</color> durante <color=yellow>{duration} segundos</color>.";
        
        JASkillDesc = $"自身の防御力が<color=green>{incDefenseAmount}</color>だけ<color=yellow>{duration}秒</color>間増加します。";
        
        PT_BRSkillDesc = $"Aumenta a defesa própria em <color=green>{incDefenseAmount}</color> por <color=yellow>{duration} segundos</color>.";
        
        RUSkillDesc = $"Увеличивает собственную защиту на <color=green>{incDefenseAmount}</color> на <color=yellow>{duration} секунд</color>.";
        
        ZH_HANSSkillDesc = $"在<color=yellow>{duration}秒</color>内，自己的防御力提高<color=green>{incDefenseAmount}</color>。";
    }
}
