using UnityEngine;

public class Revenge : UnitDefault
{
    [Header("복수")]
    public float attackCoeff;

    int friendlyCount;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void Start()
    {
        base.Start();

        friendlyCount = enemies.Count;
    }

    private void Update()
    {
        if (enemies.Count != friendlyCount) {
            if (enemies.Count < friendlyCount) {
                UseSkill();
            }
            friendlyCount = enemies.Count;
        }
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
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"아군 유닛이 사망 시 공격 중인 대상에게 공격력 <color=red>{attackCoeff}배</color>의 강력한 한 발을 발사해 복수합니다.";

        ENSkillDesc = $"When an allied unit dies, fires a powerful shot at the current target dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to exact revenge.";

        FRSkillDesc = $"Lorsqu'une unité alliée meurt, elle tire un coup puissant sur la cible actuelle qui inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque pour se venger.";

        ITSkillDesc = $"Quando un'unità alleata muore, spara un colpo potente al bersaglio attuale che infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco per vendicarsi.";

        DESkillDesc = $"Wenn eine verbündete Einheit stirbt, feuert es einen mächtigen Schuss auf das aktuelle Ziel, der Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft verursacht, um Rache zu nehmen.";

        ESSkillDesc = $"Cuando una unidad aliada muere, dispara un poderoso disparo al objetivo actual que inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque para vengarse.";

        JASkillDesc = $"味方ユニットが死亡すると、現在の対象に攻撃力の<color=red>{attackCoeff}倍</color>の強力な一発を放ち、復讐します。";

        PT_BRSkillDesc = $"Quando uma unidade aliada morre, dispara um tiro poderoso no alvo atual causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque para se vingar.";

        RUSkillDesc = $"Когда союзная единица умирает, выпускает мощный выстрел по текущей цели, нанося урон, равный <color=red>{attackCoeff} раза</color> силы атаки, чтобы отомстить.";

        ZH_HANSSkillDesc = $"当友方单位死亡时，向当前目标射出一发强力子弹，其造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害以报仇。";
    }
}
