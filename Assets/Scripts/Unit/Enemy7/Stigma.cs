using System.Collections.Generic;
using UnityEngine;

public class Stigma : UnitDefault
{
    [Header("낙인")]
    public float decAttackAmount;

    protected override void Awake()
    {
        base.Awake();

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
        RandomEnemyUnit(friends);
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, SkillTarget.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        unitComp.SetStat(unitComp.attackPowerStat, decAttackAmount, false, false);
        unitComp.SetStateBar(State.decAttack, -2f);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"적 유닛 중 랜덤한 한 명의 공격력을 <color=red>{decAttackAmount}</color>만큼 감소시킵니다.\n" +
                    $"(중첩 가능)";
        
        ENSkillDesc = $"Reduces the attack power of a random enemy unit by <color=red>{decAttackAmount}</color>.\n" +
                      $"(Can stack)";

        FRSkillDesc = $"Réduit la puissance d'attaque d'une unité ennemie aléatoire de <color=red>{decAttackAmount}</color>.\n" +
                        $"(Peut s'empiler)";

        ITSkillDesc = $"Riduce la potenza d'attacco di un'unità nemica casuale di <color=red>{decAttackAmount}</color>.\n" +
                          $"(Può accumularsi)";

        DESkillDesc = $"Reduziert die Angriffskraft einer zufälligen feindlichen Einheit um <color=red>{decAttackAmount}</color>.\n" +
                          $"(Kann gestapelt werden)";

        ESSkillDesc = $"Reduce el poder de ataque de una unidad enemiga aleatoria en <color=red>{decAttackAmount}</color>.\n" +
                          $"(Puede acumularse)";

        JASkillDesc = $"敵ユニットの中からランダムに1体の攻撃力を<color=red>{decAttackAmount}</color>だけ減少させます。\n" +
                            $"（重複可能）";

        PT_BRSkillDesc = $"Reduz o poder de ataque de uma unidade inimiga aleatória em <color=red>{decAttackAmount}</color>.\n" +
                          $"(Pode ser acumulado)";

        RUSkillDesc = $"Уменьшает силу атаки случайной вражеской единицы на <color=red>{decAttackAmount}</color>.\n" +
                            $"(Может накапливаться)";

        ZH_HANSSkillDesc = $"将随机敌方单位的攻击力降低<color=red>{decAttackAmount}</color>。\n" +
                            $"（可叠加）";
    }
}