using UnityEngine;

public class Predation : UnitDefault
{
    [Header("포식")]
    public float attackCoeff;
    public float incHealthPer;

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
        Healing(MaxHealth / 100f * incHealthPer);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 주고, 본인의 체력을 <color=green>{incHealthPer}%</color> 회복합니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power to the current target and heals this unit for <color=green>{incHealthPer}%</color> of max health.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque à la cible actuelle et soigne cette unité de <color=green>{incHealthPer}%</color> de la santé maximale.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al bersaglio attuale e cura questa unità per <color=green>{incHealthPer}%</color> della salute massima.";

        DESkillDesc = $"Verursacht dem aktuellen Ziel Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft und heilt diese Einheit um <color=green>{incHealthPer}%</color> der maximalen Gesundheit.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al objetivo actual y cura a esta unidad por <color=green>{incHealthPer}%</color> de la salud máxima.";

        JASkillDesc = $"現在の対象に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、自身の体力を最大体力の<color=green>{incHealthPer}%</color>回復します。";

        PT_BRSkillDesc = $"Causa dano igual a <color=red>{attackCoeff} vezes</color> el poder de ataque al objetivo actual y cura a esta unidad por <color=green>{incHealthPer}%</color> de la salud máxima.";

        RUSkillDesc = $"Наносит текущей цели урон, равный <color=red>{attackCoeff} раза</color> силы атаки, и лечит эту единицу на <color=green>{incHealthPer}%</color> от максимального здоровья.";

        ZH_HANSSkillDesc = $"??前目标造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，????位回复相当于最大生命<color=green>{incHealthPer}%</color>的生命。";
    }
}
