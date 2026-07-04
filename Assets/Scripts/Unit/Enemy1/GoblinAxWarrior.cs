using UnityEngine;

public class GoblinAxWarrior : UnitDefault
{
    [Header("파쇄")]
    public float decDefenseAmount;
    public float duration;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();

        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, SkillTarget.transform.position);

        unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, duration);
        unitComp.SetStateBar(State.decDefense, duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 방어력을 <color=yellow>{duration}초</color> 동안 <color=red>{decDefenseAmount}</color>만큼 감소시킵니다.";

        ENSkillDesc = $"Decreases the defense of the attacking target by <color=red>{decDefenseAmount}</color> for <color=yellow>{duration} seconds</color>.";
        
        FRSkillDesc = $"Diminue la défense de la cible attaquante de <color=red>{decDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Diminuisce la difesa del bersaglio attaccante di <color=red>{decDefenseAmount}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Verringert die Verteidigung des angreifenden Ziels um <color=red>{decDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Disminuye la defensa del objetivo atacante en <color=red>{decDefenseAmount}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"攻撃中の対象の防御力を<color=red>{decDefenseAmount}</color>だけ<color=yellow>{duration}秒</color>間減少させます。";

        PT_BRSkillDesc = $"Diminui a defesa do alvo atacante em <color=red>{decDefenseAmount}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Уменьшает защиту атакующей цели на <color=red>{decDefenseAmount}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"将正在攻击的目标的防御力降低<color=red>{decDefenseAmount}</color>，持续<color=yellow>{duration}秒</color>。";
    }
}