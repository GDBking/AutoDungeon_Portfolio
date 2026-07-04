using System.Collections;
using UnityEngine;

public class Confusion : UnitDefault
{
    [Header("교란")]
    public float size;

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower, SkillTarget, SkillTarget.transform.position, isPenetration: true);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        if (hitEnemies.Length <= 1) {
            SkillTarget = Target;
            StartCoroutine(OnSkillAnim());
            yield break;
        }

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;
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
        skillDesc = $"본인 반경 <color=#AEEAFF>{size}X{size}</color>범위 내에 적 유닛이 <color=yellow>1명</color>만 존재 시 기본 공격이 방어력을 관통합니다.";
        
        ENSkillDesc = $"When there is only <color=yellow>1</color> enemy unit within a radius of <color=#AEEAFF>{size}X{size}</color> around you, your basic attacks penetrate defense.";

        FRSkillDesc = $"Lorsqu'il n'y a qu'<color=yellow>1</color> unité ennemie dans un rayon de <color=#AEEAFF>{size}X{size}</color> autour de vous, vos attaques de base traversent la défense.";

        ITSkillDesc = $"Quando c'è solo <color=yellow>1</color> unità nemica in un raggio di <color=#AEEAFF>{size}X{size}</color> intorno a te, i tuoi attacchi base penetrano la difesa.";

        DESkillDesc = $"Wenn sich nur <color=yellow>1</color> feindliche Einheit in einem Radius von <color=#AEEAFF>{size}X{size}</color> um dich herum befindet, durchdringen deine Grundangriffe die Verteidigung.";

        ESSkillDesc = $"Cuando solo hay <color=yellow>1</color> unidad enemiga dentro de un radio de <color=#AEEAFF>{size}X{size}</color> a tu alrededor, tus ataques básicos penetran la defensa.";

        JASkillDesc = $"自身の半径<color=#AEEAFF>{size}X{size}</color>範囲内に敵ユニットが<color=yellow>1体</color>のみ存在する場合、通常攻撃が防御力を貫通します。";

        PT_BRSkillDesc = $"Quando há apenas <color=yellow>1</color> unidade inimiga dentro de um raio de <color=#AEEAFF>{size}X{size}</color> ao seu redor, seus ataques básicos penetram a defesa.";

        RUSkillDesc = $"Когда в радиусе <color=#AEEAFF>{size}X{size}</color> вокруг вас находится только <color=yellow>1</color> вражеский юнит, ваши базовые атаки игнорируют защиту.";

        ZH_HANSSkillDesc = $"当你周围半径为<color=#AEEAFF>{size}X{size}</color>的范围内只有<color=yellow>1</color>个敌方单位时，你的基础攻击会忽视防御。";
    }
}