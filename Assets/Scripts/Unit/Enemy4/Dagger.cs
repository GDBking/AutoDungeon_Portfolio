using UnityEngine;

public class Dagger : UnitDefault
{
    [Header("단도")]
    public float attackCoeff;
    public float decHealthPer;

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

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, SkillTarget.transform.position);

        Hit(Health / 100f * decHealthPer, 0f, this, true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인의 남은 체력 중 <color=red>{decHealthPer}%</color>를 깎아 공격 중인 대상에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Sacrifices <color=red>{decHealthPer}%</color> of its current health to deal damage equal to <color=red>{attackCoeff} times</color> the attack power to the current target.";

        FRSkillDesc = $"Sacrifie <color=red>{decHealthPer}%</color> de sa santé actuelle pour infliger des dégâts équivalents à <color=red>{attackCoeff} fois</color> la puissance d'attaque à la cible actuelle.";

        ITSkillDesc = $"Sottrae <color=red>{decHealthPer}%</color> della propria salute attuale per infliggere danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al bersaglio attuale.";

        DESkillDesc = $"Opfert <color=red>{decHealthPer}%</color> der aktuellen Gesundheit, um dem aktuellen Ziel Schaden in H?he von <color=red>{attackCoeff} mal</color> der Angriffskraft zuzuf?gen.";

        ESSkillDesc = $"Sacrifica <color=red>{decHealthPer}%</color> de su salud actual para infligir daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque al objetivo actual.";

        JASkillDesc = $"自身の現在の体力の<color=red>{decHealthPer}%</color>を犠牲にして、現在の対象に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Sacrifica <color=red>{decHealthPer}%</color> da sua vida atual para causar dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque ao alvo atual.";

        RUSkillDesc = $"Жертвует <color=red>{decHealthPer}%</color> от текущего здоровья, чтобы нанести текущей цели урон, равный <color=red>{attackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"牺牲当前生命值的<color=red>{decHealthPer}%</color>，对当前目标造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}