using System.Collections;
using UnityEngine;

public class Leap : UnitDefault
{
    public float duration;

    bool isLeap;
    UnitDefault targetUnit;

    protected override void Start()
    {
        base.Start();

        UseSkill();
    }

    private void Update()
    {
        if (!isSkillAnim && !isStunAnim && !isSilence && !isMovingAnim && !isAttackAnim && !isEnthrallment && (targetUnit == null || targetUnit.isDeath))
            UseSkill();
    }

    protected override void FixedUpdate()
    {
        if (isLeap) {
            OnMove();
            IsFlipX();
            sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);
            
            if (Target == null || CheckForTargetRange(AttackRange, Target) || isDeath) {
                isLeap = false;
                SetStat(speedStat, 3f, false, false);
                rigid2D.mass = 1f;

                Invincible(duration);
            }
        }
        else
            base.FixedUpdate();
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0 || isLeap)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindFarthestEnemy(friends);
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, transform.position);

        targetUnit = SkillTarget.GetComponent<UnitDefault>();
        Target = SkillTarget;
        isLeap = true;
        SetStat(speedStat, 3f, false, true);
        rigid2D.mass = 3f;
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = Target.transform.position;
        targetUnit = AttackTarget.GetComponent<UnitDefault>();
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

    protected override IEnumerator OnSkillAnim()
    {
        isSkillAnim = true;
        SkillUseAfterState(); // 마나, isSkillAvailable 등 초기화
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Skill");
        yield return null;
        // 스킬 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);
        isSkillAnim = false;
        if (!isStunAnim && !isDeath && !isBondage && isKiting && Random.Range(1, 101) <= 40) {
            isKitingAnim = true;
            StartCoroutine(OnKitingAnim()); // 카이팅 실행
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"스폰 시 본인으로부터 가장 멀리 있는 적에게 돌진하며 돌진 후<color=yellow>{duration}초</color> 동안 무적 상태가 됩니다.\n" +
                    $"공격 중인 대상이 사망 시 다시 가장 멀리 있는 적에게 돌진하며 위 과정을 반복합니다.";

        ENSkillDesc = $"Dashes toward the enemy farthest from itself upon spawning and becomes invincible for <color=yellow>{duration} seconds</color> after the dash.\n" +
                        $"If the current target dies, it dashes again toward the farthest enemy, repeating this process.";

        FRSkillDesc = $"Se précipite vers l’ennemi le plus éloigné lors de son apparition et devient invincible pendant <color=yellow>{duration} secondes</color> après la charge.\n" +
                       $"Si la cible actuelle meurt, il se précipite de nouveau vers l’ennemi le plus éloigné, répétant ce processus.";

        ITSkillDesc = $"Si scaglia verso il nemico più lontano da sé alla comparsa e diventa invincibile per <color=yellow>{duration} secondi</color> dopo la carica.\n" +
                       $"Se il bersaglio attuale muore, si scaglia di nuovo verso il nemico più lontano, ripetendo il processo.";

        DESkillDesc = $"Stürmt beim Erscheinen zum am weitesten entfernten Feind und wird nach dem Ansturm für <color=yellow>{duration} Sekunden</color> unverwundbar.\n" +
                       $"Stirbt das aktuelle Ziel, stürmt er erneut zum am weitesten entfernten Feind und wiederholt diesen Vorgang.";

        ESSkillDesc = $"Embiste hacia el enemigo más alejado al aparecer y se vuelve invencible durante <color=yellow>{duration} segundos</color> después de la embestida.\n" +
                       $"Si el objetivo actual muere, embiste de nuevo hacia el enemigo más alejado, repitiendo el proceso.";

        JASkillDesc = $"生成時に自分から最も遠い敵へ突進し、突進後<color=yellow>{duration}秒</color>の間、無敵状態になります。\n" +
                       $"現在の対象が死亡した場合、再び最も遠い敵へ突進し、この過程を繰り返します。";

        PT_BRSkillDesc = $"Avança em direção ao inimigo mais distante ao aparecer e fica invencível por <color=yellow>{duration} segundos</color> após a investida.\n" +
                         $"Se o alvo atual morrer, avança novamente em direção ao inimigo mais distante, repetindo o processo.";

        RUSkillDesc = $"При появлении совершает рывок к самому дальнему от себя врагу и становится неуязвимым на <color=yellow>{duration} секунд</color> после рывка.\n" +
                       $"Если текущая цель умирает, он снова совершает рывок к самому дальнему врагу, повторяя этот процесс.";

        ZH_HANSSkillDesc = $"生成时冲向距离自己最远的敌人，并在冲锋后<color=yellow>{duration}秒</color>内进入无敌状态。\n" +
                           $"若当前目标死亡，则再次冲向最远的敌人，并重复该过程。";
    }
}