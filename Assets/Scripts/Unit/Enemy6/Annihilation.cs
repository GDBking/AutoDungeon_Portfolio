using UnityEngine;

public class Annihilation : UnitDefault
{
    [Header("격멸")]
    public float attackCoeff;
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

        SoundPlay(skillSoundClip);

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        unitComp.OnStunAnim(duration);
        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{duration}초</color> 동안 기절시킵니다.";
        
        ENSkillDesc = $"Deals <color=red>{attackCoeff}x</color> damage to the attacking target and stuns them for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Inflige des dégâts de <color=red>{attackCoeff}x</color> à la cible attaquante et l'étourdit pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff}x</color> al bersaglio attaccante e lo stordisce per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Fügt dem angreifenden Ziel <color=red>{attackCoeff}x</color> Schaden zu und betäubt es für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Inflige <color=red>{attackCoeff}x</color> de daño al objetivo atacante y lo aturde durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"攻撃対象に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{duration}秒</color>間気絶させます。";

        PT_BRSkillDesc = $"Causa <color=red>{attackCoeff}x</color> de dano ao alvo atacante e o atordoa por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Наносит атакующей цели урон в размере <color=red>{attackCoeff}x</color> и оглушает её на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"对攻击目标造成<color=red>{attackCoeff}倍</color>伤害，并使其眩晕<color=yellow>{duration}秒</color>。";
    }
}