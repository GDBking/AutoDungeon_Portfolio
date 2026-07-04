using UnityEngine;

public class GoblinWarrior : UnitDefault
{
    [Header("내려치기")]
    public float attackCoeff;

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
        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 돌진하여 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Rushes the attacking target and deals damage equal to <color=red>{attackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Se précipite sur la cible attaquante et inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Si precipita sul bersaglio attaccante e infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Stürzt auf das angegriffene Ziel zu und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Embiste contra el objetivo atacado y provoca daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"攻撃中の対象に突進し、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Embiste al objetivo atacado y causa daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        RUSkillDesc = $"Рвётся к атакуемой цели и наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"向被攻击目标突进，造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}