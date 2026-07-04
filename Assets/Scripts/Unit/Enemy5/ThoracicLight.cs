using UnityEngine;

public class ThoracicLight : UnitDefault
{
    [Header("흉광")]
    public float attackCoeff;
    public float incHealthPer;
    public float size;

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
        float damage = CreateAttackBox(AttackPower * attackCoeff, null, pos, AttackStyle.range);
        attackSize = 0.5f;

        Healing(damage / 100f * incHealthPer);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상 반경 <color=#AEEAFF>{size}X{size}</color>범위에 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 입힌 피해량의 <color=green>{incHealthPer}%</color>만큼 체력을 회복합니다.";
        
        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power in a <color=#AEEAFF>{size}X{size}</color> radius around the attacking target, and heals for <color=green>{incHealthPer}%</color> of the damage dealt.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque dans un rayon de <color=#AEEAFF>{size}X{size}</color> autour de la cible attaquée, et se soigne de <color=green>{incHealthPer}%</color> des dégâts infligés.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco in un raggio di <color=#AEEAFF>{size}X{size}</color> intorno al bersaglio attaccante e si cura per il <color=green>{incHealthPer}%</color> dei danni inflitti.";

        DESkillDesc = $"Fügt im Umkreis von <color=#AEEAFF>{size}X{size}</color> um das angreifende Ziel Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft zu und heilt sich um <color=green>{incHealthPer}%</color> des zugefügten Schadens.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque en un radio de <color=#AEEAFF>{size}X{size}</color> alrededor del objetivo atacante, y se cura por <color=green>{incHealthPer}%</color> del daño infligido.";

        JASkillDesc = $"攻撃中の対象の周囲<color=#AEEAFF>{size}X{size}</color>範囲に攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、与えたダメージの<color=green>{incHealthPer}%</color>だけ体力を回復します。";

        PT_BRSkillDesc = $"Causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque em um raio de <color=#AEEAFF>{size}X{size}</color> ao redor do alvo atacante, e cura <color=green>{incHealthPer}%</color> do dano causado.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки в радиусе <color=#AEEAFF>{size}X{size}</color> вокруг атакующей цели и восстанавливает здоровье на <color=green>{incHealthPer}%</color> от нанесенного урона.";

        ZH_HANSSkillDesc = $"对被攻击目标周围<color=#AEEAFF>{size}X{size}</color>范围内造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并根据所造成伤害的<color=green>{incHealthPer}%</color>恢复生命值。";
    }
}
