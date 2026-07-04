using UnityEngine;

public class Seduction : UnitDefault
{
    [Header("유혹")]
    public float attackCoeff;
    public float duration;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

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

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        unitComp.Enthrallment(duration);
        unitComp.SetStateBar(State.enthrallment, duration);
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 적 유닛에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{duration}초</color> 동안 매혹시킵니다.";
        
        ENSkillDesc = $"Deals <color=red>{attackCoeff}x</color> attack damage to the attacking enemy unit and enthralls it for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Inflige des dégâts d'attaque de <color=red>{attackCoeff} fois</color> à l'unité ennemie attaquante et l'enchante pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Infligge danni d'attacco di <color=red>{attackCoeff}x</color> all'unità nemica attaccante e la incanta per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Fügt der angreifenden feindlichen Einheit Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft zu und bezaubert sie für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Inflige <color=red>{attackCoeff}x</color> de daño de ataque a la unidad enemiga atacante y la cautiva durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"攻撃中の敵ユニットに攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{duration}秒</color>間、魅了します。";

        PT_BRSkillDesc = $"Causa <color=red>{attackCoeff}x</color> de dano de ataque na unidade inimiga atacante e a encanta por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Наносит атакующей вражеской единице урон, равный <color=red>{attackCoeff}x</color> от атаки, и очаровывает её на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"对攻击的敌方单位造成<color=red>{attackCoeff}倍</color>的攻击伤害，并使其在<color=yellow>{duration}秒</color>内着迷。";
    }
}
