using UnityEngine;

public class GoblinSniper : UnitDefault
{
    [Header("저격")]
    public float attackCoeff;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
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
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 기준 가장 멀리 있는 적에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power to the enemy farthest from this unit.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque à l'ennemi le plus éloigné de cette unité.";

        ITSkillDesc = $"Infligge danno pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al nemico più lontano da questa unità.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft beim am weitesten von dieser Einheit entfernten Gegner.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al enemigo más lejano de esta unidad.";

        JASkillDesc = $"このユニットから最も遠い敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Causa dano igual a <color=red>{attackCoeff} vezes</color> el poder de ataque al enemigo más lejano de esta unidad.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки, самому дальнему от этой единицы врагу.";

        ZH_HANSSkillDesc = $"对距离该单位最远的敌人造成等同于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}