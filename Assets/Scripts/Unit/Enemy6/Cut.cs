using UnityEngine;

public class Cut : UnitDefault
{
    [Header("삭감")]
    public float cutHealthPer;

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
        CreateAttackBox(SkillTarget.GetComponent<UnitDefault>().MaxHealth / 100f * cutHealthPer, SkillTarget, SkillTarget.transform.position, isPenetration: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 대상 최대 체력의 <color=red>{cutHealthPer}%</color> 만큼의 방어력 관통 피해를 입힙니다.";
        
        ENSkillDesc = $"Deals defense-piercing damage equal to <color=red>{cutHealthPer}%</color> of the target's maximum health to the current target.";

        FRSkillDesc = $"Inflige des dégâts perforants égaux à <color=red>{cutHealthPer}%</color> de la santé maximale de la cible actuelle.";

        ITSkillDesc = $"Infligge danni perforanti pari a <color=red>{cutHealthPer}%</color> della salute massima del bersaglio attuale.";

        DESkillDesc = $"Verursacht durchdringenden Schaden in Höhe von <color=red>{cutHealthPer}%</color> der maximalen Gesundheit des aktuellen Ziels.";

        ESSkillDesc = $"Inflige daño penetrante igual al <color=red>{cutHealthPer}%</color> de la salud máxima del objetivo actual.";

        JASkillDesc = $"攻撃中の対象に、対象の最大体力の<color=red>{cutHealthPer}%</color>に相当する防御貫通ダメージを与えます。";

        PT_BRSkillDesc = $"Causa dano penetrante igual a <color=red>{cutHealthPer}%</color> da saúde máxima do alvo atual.";

        RUSkillDesc = $"Наносит проникающий урон, равный <color=red>{cutHealthPer}%</color> от максимального здоровья текущей цели.";

        ZH_HANSSkillDesc = $"对当前目标造成相当于其最大生命值的<color=red>{cutHealthPer}%</color>的防御穿透伤害。";
    }
}
