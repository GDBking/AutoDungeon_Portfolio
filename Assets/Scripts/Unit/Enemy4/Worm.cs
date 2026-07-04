using UnityEngine;

public class Worm : UnitDefault
{
    [Header("벌레")]
    public float attackCoeff;
    public float size;

    protected override void Awake()
    {
        base.Awake();

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
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, pos, AttackStyle.range);
        attackSize = 0.5f;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 위치 <color=#AEEAFF>{size}X{size}</color>범위의 땅에 벌레를 소환해 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Summons worms on the ground in a <color=#AEEAFF>{size}X{size}</color> area at the location of the attacking target, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies.";
    
        FRSkillDesc = $"Invoque des vers sur le sol dans une zone de <color=#AEEAFF>{size}X{size}</color> à l'emplacement de la cible attaquante, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis.";

        ITSkillDesc = $"Evoca vermi sul terreno in un'area di <color=#AEEAFF>{size}X{size}</color> nella posizione del bersaglio attaccante, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici.";

        DESkillDesc = $"Beschwört Würmer auf dem Boden in einem <color=#AEEAFF>{size}X{size}</color>-Bereich am Standort des angreifenden Ziels und fügt Feinden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft zu.";

        ESSkillDesc = $"Invoca gusanos en el suelo en un área de <color=#AEEAFF>{size}X{size}</color> en la ubicación del objetivo atacado, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos.";

        JASkillDesc = $"攻撃中の対象の位置にある地面に<color=#AEEAFF>{size}X{size}</color>範囲の虫を召喚し、敵に攻撃力<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Invoca vermes no chão em uma área de <color=#AEEAFF>{size}X{size}</color> na localização do alvo atacado, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos.";

        RUSkillDesc = $"Призывает червей на землю в области <color=#AEEAFF>{size}X{size}</color> в месте нахождения атакуемой цели, нанося врагам урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"在被攻击目标位置的地面上召唤一个<color=#AEEAFF>{size}X{size}</color>范围的虫子，对敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}
