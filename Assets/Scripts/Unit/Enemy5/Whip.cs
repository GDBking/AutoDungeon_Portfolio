using UnityEngine;

public class Whip : UnitDefault
{
    [Header("채찍")]
    public float attackCoeff;
    public float decDefenseAmount;

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

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();

        unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false);
        unitComp.SetStateBar(State.decDefense, -2f);

        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 방어력을 <color=red>{decDefenseAmount}</color>만큼 감소시키고 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.\n" +
                    $"(방어력 감소 중첩 가능)";
        
        ENSkillDesc = $"Reduces the defense of the current target by <color=red>{decDefenseAmount}</color> and deals damage equal to <color=red>{attackCoeff} times</color> the attack power.\n" +
                      $"(Defense reduction can stack)";

        FRSkillDesc = $"Réduit la défense de la cible actuelle de <color=red>{decDefenseAmount}</color> et inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque.\n" +
                        $"(La réduction de la défense peut s'empiler)";

        ITSkillDesc = $"Riduce la difesa del bersaglio attuale di <color=red>{decDefenseAmount}</color> e infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco.\n" +
                      $"(La riduzione della difesa può accumularsi)";

        DESkillDesc = $"Reduziert die Verteidigung des aktuellen Ziels um <color=red>{decDefenseAmount}</color> und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.\n" +
                        $"(Verteidigungsreduktion kann stapeln)";

        ESSkillDesc = $"Reduce la defensa del objetivo actual en <color=red>{decDefenseAmount}</color> e inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque.\n" +
                      $"(La reducción de defensa puede acumularse)";

        JASkillDesc = $"攻撃中の対象の防御力を<color=red>{decDefenseAmount}</color>だけ減少させ、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。\n" +
                        $"（防御力減少は重複可能）";

        PT_BRSkillDesc = $"Reduz a defesa do alvo atual em <color=red>{decDefenseAmount}</color> e causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque.\n" +
                        $"(A redução de defesa pode se acumular)";

        RUSkillDesc = $"Уменьшает защиту текущей цели на <color=red>{decDefenseAmount}</color> и наносит урон, равный <color=red>{attackCoeff} разам</color> силы атаки.\n" +
                        $"(Снижение защиты может накапливаться)";

        ZH_HANSSkillDesc = $"降低当前目标的防御力<color=red>{decDefenseAmount}</color>，并造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。\n" +
                          $"（防御力降低可叠加）";
    }
}