using UnityEngine;

public class StomachAcid : UnitDefault
{
    [Header("위액")]
    public float attackCoeff;
    public float decAttackPer;
    public float poisonDamage;
    public float size;
    public int count;

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
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        StartCoroutine(OnFire(SkillTarget, pos));
    }

    void OnSkillFire(GameObject target)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(target.transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        foreach (Collider2D hit in hitEnemies) {
            UnitDefault unitComp = hit.GetComponent<UnitDefault>();
            unitComp.SetStat(unitComp.attackPowerStat, decAttackPer, true, false, count);
            unitComp.SetStateBar(State.decAttack, count);
            unitComp.Poison(poisonDamage, count, this);
        }

        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, target.transform.position, AttackStyle.range);
        attackSize = 0.5f;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 위액을 뿜어 반경 <color=#AEEAFF>{size}X{size}</color>범위의 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, " +
                    $"<color=yellow>{count}초</color> 동안 공격력을 <color=red>{decAttackPer}%</color> 감소시키며 독 효과를 입힙니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";
        
        ESSkillDesc = $"Sprays stomach acid at the attacking target, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies within a radius of <color=#AEEAFF>{size}X{size}</color>, " +
                      $"reducing their attack by <color=red>{decAttackPer}%</color> for <color=yellow>{count} seconds</color> and inflicting poison.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"Projette de l'acide gastrique sur la cible attaquée, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis dans un rayon de <color=#AEEAFF>{size}X{size}</color>, " +
                      $"réduisant leur attaque de <color=red>{decAttackPer}%</color> pendant <color=yellow>{count} secondes</color> et infligeant du poison.\n" +
                      $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Spruzza acido gastrico sul bersaglio attaccante, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici entro un raggio di <color=#AEEAFF>{size}X{size}</color>, " +
                      $"riducendo il loro attacco di <color=red>{decAttackPer}%</color> per <color=yellow>{count} secondi</color> e infliggendo veleno.\n" +
                      $"Danno da veleno al secondo: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Sprüht Magensäure auf das angreifende Ziel und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft an Feinden in einem Radius von <color=#AEEAFF>{size}X{size}</color>, " +
                      $"reduziert deren Angriff um <color=red>{decAttackPer}%</color> für <color=yellow>{count} Sekunden</color> und fügt Gift zu.\n" +
                      $"Gift-Schaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Rocía ácido estomacal sobre el objetivo atacante, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos dentro de un radio de <color=#AEEAFF>{size}X{size}</color>, " +
                      $"reduciendo su ataque en <color=red>{decAttackPer}%</color> durante <color=yellow>{count} segundos</color> e infligiendo veneno.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"攻撃中の対象に胃液を噴射し、半径<color=#AEEAFF>{size}X{size}</color>内の敵に攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、" +
                      $"<color=yellow>{count}秒</color>間、攻撃力を<color=red>{decAttackPer}%</color>減少させ、中毒を与えます。\n" +
                      $"秒間毒ダメージ: <color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Borrifa ácido estomacal no alvo atacante, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos dentro de um raio de <color=#AEEAFF>{size}X{size}</color>, " +
                         $"reduzindo seu ataque em <color=red>{decAttackPer}%</color> por <color=yellow>{count} segundos</color> e infligindo veneno.\n" +
                         $"Dano por veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"Брызгает желудочной кислотой в атакуемую цель, нанося урон, равный <color=red>{attackCoeff} раза</color> силе атаки врагам в радиусе <color=#AEEAFF>{size}X{size}</color>, " +
                      $"уменьшая их атаку на <color=red>{decAttackPer}%</color> на <color=yellow>{count} секунд</color> и нанося яд.\n" +
                      $"Урон от яда в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"向攻击目标喷洒胃酸，对半径<color=#AEEAFF>{size}X{size}</color>内的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，" +
                           $"在<color=yellow>{count}秒</color>内将其攻击力降低<color=red>{decAttackPer}%</color>并施加中毒效果。\n" +
                           $"每秒中毒伤害: <color=red>{poisonDamage}</color>";
    }
}
