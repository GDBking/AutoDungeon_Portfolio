using UnityEngine;

public class Obsession : UnitDefault
{
    UnitDefault hitEnemy;

    protected override void FixedUpdate()
    {
        OnMove();
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);

        if (isDeath || isKitingAnim || isMovingAnim)
            return;

        if (hitEnemy == null) {
            FindNearestEnemy(friends);
        }
        else {
            Target = hitEnemy.gameObject;
        }
        IsFlipX();

        StartCoroutine(OnAttackAnim());
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        if (hitEnemy == null) {
            hitEnemy = hitUnit;
        }

        return base.Hit(attack, criticalPer, hitUnit, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인을 가장 먼저 공격한 적을 끝까지 쫓아갑니다.";

        ENSkillDesc = $"Pursues the enemy that first attacked it relentlessly until it is defeated or lost.";

        FRSkillDesc = $"Poursuit sans relâche l'ennemi qui l'a attaqué en premier jusqu'à ce qu'il soit vaincu ou perdu de vue.";

        ITSkillDesc = $"Insegue instancabilmente il nemico che l'ha attaccato per primo fino a quando non viene sconfitto o perso di vista.";

        DESkillDesc = $"Verfolgt den Feind, der es zuerst angegriffen hat, unermüdlich, bis dieser besiegt ist oder außer Sicht gerät.";

        ESSkillDesc = $"Persigue implacablemente al enemigo que lo atacó primero hasta que sea derrotado o se pierda de vista.";

        JASkillDesc = $"最初に自分を攻撃した敵を、倒されるか視界から失われるまで執拗に追いかけます。";

        PT_BRSkillDesc = $"Persegue implacavelmente o inimigo que o atacou primeiro até que seja derrotado ou perdido de vista.";

        RUSkillDesc = $"Неустанно преследует врага, который атаковал его первым, пока тот не будет повержен или не исчезнет из поля зрения.";

        ZH_HANSSkillDesc = $"不懈追击最先攻击自己的敌人，直到其被击败或失去视线。";
    }
}
