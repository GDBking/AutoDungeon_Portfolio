using UnityEngine;

public class Flooring : UnitDefault
{
    [Header("장판")]
    public float attackCoeff;
    public float decDefensePer;
    public float size;
    public int count;
    public FlooringObj flooringObj;

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

        FlooringObj obj = Instantiate(flooringObj, pos, Quaternion.identity, GameManager.instance.skillEffect);
        obj.flooringComp = this;

        CreateAttackBox(AttackPower * attackCoeff, null, pos, AttackStyle.range);
    }

    public void OnFlooring(UnitDefault enemy)
    {
        enemy.SetStat(enemy.defenseStat, decDefensePer, true, false, 1f);
        enemy.SetStateBar(State.decDefense, 1f);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"대상 지역에 <color=#AEEAFF>{size}X{size}</color> 크기의 바닥을 생성하여 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 해당 바닥에 있는 적들의 방어력을 <color=red>{decDefensePer}%</color>만큼 <color=yellow>{count}초</color> 동안 감소시킵니다.";

        ENSkillDesc = $"Creates a <color=#AEEAFF>{size}X{size}</color> area on the ground that deals damage equal to <color=red>{attackCoeff} times</color> the attack power, reducing the defense of enemies within that area by <color=red>{decDefensePer}%</color> for <color=yellow>{count} seconds</color>.";

        FRSkillDesc = $"Crée une zone de <color=#AEEAFF>{size}X{size}</color> sur le sol qui inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque, réduisant la défense des ennemis dans cette zone de <color=red>{decDefensePer}%</color> pendant <color=yellow>{count} secondes</color>.";

        ITSkillDesc = $"Crea un'area di <color=#AEEAFF>{size}X{size}</color> sul terreno che infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco, riducendo la difesa dei nemici in quell'area del <color=red>{decDefensePer}%</color> per <color=yellow>{count} secondi</color>.";

        DESkillDesc = $"Erstellt einen Bereich von <color=#AEEAFF>{size}X{size}</color> auf dem Boden, der Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft verursacht und die Verteidigung von Feinden in diesem Bereich um <color=red>{decDefensePer}%</color> für <color=yellow>{count} Sekunden</color> reduziert.";

        ESSkillDesc = $"Crea un área de <color=#AEEAFF>{size}X{size}</color> en el suelo que inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque, reduciendo la defensa de los enemigos dentro de esa área en <color=red>{decDefensePer}%</color> durante <color=yellow>{count} segundos</color>.";

        JASkillDesc = $"対象の地面に<color=#AEEAFF>{size}X{size}</color>の範囲を作り、攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、その範囲内にいる敵の防御力を<color=red>{decDefensePer}%</color>、<color=yellow>{count}秒</color>間減少させます。";

        PT_BRSkillDesc = $"Cria uma área de <color=#AEEAFF>{size}X{size}</color> no chão que causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque, reduzindo a defesa dos inimigos dentro dessa área em <color=red>{decDefensePer}%</color> por <color=yellow>{count} segundos</color>.";

        RUSkillDesc = $"Создает область размером <color=#AEEAFF>{size}X{size}</color> на земле, которая наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки, снижая защиту врагов в этой области на <color=red>{decDefensePer}%</color> в течение <color=yellow>{count} секунд</color>.";

        ZH_HANSSkillDesc = $"在地面上创建一个<color=#AEEAFF>{size}X{size}</color>的区域，造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并在该区域内将敌人的防御力降低<color=red>{decDefensePer}%</color>，持续<color=yellow>{count}秒</color>。";
    }
}
