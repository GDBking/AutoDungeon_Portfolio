using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutBurst : UnitDefault
{
    [Header("분출")]
    public float attackCoeff;

    readonly List<string> stateBarList = new();
    readonly HashSet<string> targetStates = new() {
        "incCriticalPer", "incAttackRange", "incAttack", "incAttackSpeed",
        "penetrationShoot", "incDefense", "incLifeSteal", "comeback", "berserk",
        "incSpeed"
    };

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {

    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;

        UnitDefault unitComp = AttackTarget.GetComponent<UnitDefault>();
        stateBarList.Clear();
        foreach (Transform state in unitComp.uiStateBar) {
            stateBarList.Add(state.name);
        }

        if (stateBarList.Any(state => targetStates.Contains(state))) {
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

        if (isKitingUnit && !isStunAnim && !isDeath && !isBondage && !isTaunt && Random.Range(1, 101) <= 40) {
            isKitingAnim = true;
            StartCoroutine(OnKitingAnim()); // 카이팅 실행
        }
        isAttackAnim = false;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 버프 효과(상태바에 표시되는 효과)가 있을 시 기본 공격이 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"If the attacking target has a buff effect (an effect displayed on the status bar), the basic attack deals damage equal to <color=red>{attackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Si la cible attaquée a un effet de buff (un effet affiché sur la barre d'état), l'attaque de base inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Se il bersaglio attaccante ha un effetto di potenziamento (un effetto visualizzato sulla barra di stato), l'attacco base infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Wenn das angreifende Ziel einen Buff-Effekt (einen auf der Statusleiste angezeigten Effekt) hat, verursacht der Grundangriff Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Si el objetivo atacante tiene un efecto de mejora (un efecto que se muestra en la barra de estado), el ataque básico inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"攻撃中の対象にバフ効果（ステータスバーに表示される効果）がある場合、通常攻撃が攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Si el objetivo atacante tiene un efecto de mejora (un efecto que se muestra en la barra de estado), el ataque básico inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.";

        RUSkillDesc = $"Если у атакующей цели есть эффект баффа (эффект, отображаемый на панели состояния), базовая атака наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"如果正在攻击的目标具有增益效果（显示在状态栏上的效果），则基础攻击会造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}
