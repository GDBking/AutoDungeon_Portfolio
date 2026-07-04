using System.Collections;
using UnityEngine;

public class Craving : UnitDefault
{
    [Header("갈망")]
    public float attackCoeff;
    public float cravingPer;

    protected override void UseSkill()
    {

    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;

        UnitDefault unitComp = AttackTarget.GetComponent<UnitDefault>();
        if (unitComp.Health / unitComp.MaxHealth <= cravingPer / 100f) {
            SkillTarget = AttackTarget;
            StartCoroutine(OnSkillAnim());
            yield break;
        }

        isAttackAnim = true;
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Attack");
        yield return null;
        // 공격 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);

        if (!isStunAnim && !isDeath && !isBondage && !isTaunt && Random.Range(1, 101) <= 40) {
            isMovingAnim = true;
            StartCoroutine(OnMovingAnim()); // 무빙 애니메이션 실행
        }
        isAttackAnim = false;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 체력이 <color=green>{cravingPer}%</color> 이하이면 기본 공격이 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"If the current target's HP percentage is at or below <color=green>{cravingPer}%</color>, basic attacks deal damage equal to <color=red>{attackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Si le pourcentage de HP de la cible actuelle est égal ou inférieur à <color=green>{cravingPer}%</color>, les attaques de base infligent des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Se la percentuale di HP del bersaglio attuale è pari o inferiore a <color=green>{cravingPer}%</color>, gli attacchi base infliggono danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Wenn der HP-Prozentsatz des aktuellen Ziels bei oder unter <color=green>{cravingPer}%</color> liegt, verursachen Grundangriffe Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Si el porcentaje de HP del objetivo actual está en o por debajo de <color=green>{cravingPer}%</color>, los ataques básicos infligen daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"現在の対象のHP割合が<color=green>{cravingPer}%</color>以下の場合、通常攻撃は攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Si el porcentaje de HP del objetivo actual está en o por debajo de <color=green>{cravingPer}%</color>, los ataques básicos infligen daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        RUSkillDesc = $"Если процент HP текущей цели находится на уровне или ниже <color=green>{cravingPer}%</color>, базовые атаки наносят урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"如果当前目标的生命值百分比低于或等于 <color=green>{cravingPer}%</color>，普通攻击造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}