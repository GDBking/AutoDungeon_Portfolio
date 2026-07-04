using System.Collections;
using UnityEngine;

public class Stare : UnitDefault
{
    [Header("주시")]
    public float attackCoeff;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindFarthestEnemy(friends);
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);

        if (target.GetComponent<UnitDefault>().isDeath)
            return;

        StartCoroutine(StareRoutine(target));
    }

    readonly WaitForFixedUpdate wait = new();
    IEnumerator StareRoutine(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        while (!unitComp.isDeath && Vector2.Distance(transform.position, target.transform.position) > 1f) {
            target.transform.position = Vector2.MoveTowards(target.transform.position, transform.position, Time.fixedDeltaTime * 5f);
            yield return wait;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 기준 거리가 가장 먼 적에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 본인 앞으로 끌어옵니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power to the enemy farthest from this unit and pulls them toward this unit.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque au ennemi le plus éloigné de cette unité et le attire vers cette unité.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al nemico più lontano da questa unità e lo trascina verso questa unità.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft beim am weitesten von dieser Einheit entfernten Gegner und zieht ihn zu dieser Einheit.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al enemigo más lejano de esta unidad y lo atrae hacia esta unidad.";

        JASkillDesc = $"このユニットから最も遠い敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、その敵をこのユニットの前に引き寄せます。";

        PT_BRSkillDesc = $"Causa daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al enemigo más lejano de esta unidad y lo atrae hacia esta unidad.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки самому дальнему от этой единицы врагу, и притягивает его к этой единице.";

        ZH_HANSSkillDesc = $"对距离该单位最远的敌人造成等同于攻击力<color=red>{attackCoeff}倍</color>的伤害，并将其拉向该单位。";
    }
}
