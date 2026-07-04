using System.Collections;
using UnityEngine;

public class Oath : UnitDefault
{
    [Header("서약")]
    public float healthPer;
    public float incAttackPer;
    public float incDefenseAmount;

    bool isOath;
    readonly WaitForSeconds wait = new(1f);

    protected override void Start()
    {
        base.Start();

        StartCoroutine(OathTime());
    }

    IEnumerator OathTime()
    {
        while (true) {
            if (isOath) {
                SetStat(attackPowerStat, incAttackPer, true, true, 1f);
                SetStat(defenseStat, incDefenseAmount, false, true, 1f);
                SetStateBar(State.incAttack, 1f);
                SetStateBar(State.incDefense, 1f);
            }

            yield return wait;
        }
    }

    protected override void UseSkill()
    {
        if (Health / MaxHealth < healthPer / 100f)
            isOath = true;
        else
            isOath = false;
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        if (!isOath)
            SoundPlay(attackSoundClip);
        else
            SoundPlay(skillSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, isOath);
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        float damage = base.Hit(attack, criticalPer, hitUnit, isPenetration);

        UseSkill();

        return damage;
    }

    public override void Healing(float healingAmount, int idx = -1)
    {
        base.Healing(healingAmount);

        UseSkill();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인의 체력이 <color=red>{healthPer}%</color> 미만이면 공격력이 <color=green>{incAttackPer}%</color>, 방어력이 <color=green>{incDefenseAmount}</color>만큼 상승합니다."; 
        
        ENSkillDesc = $"When this unit's health is below <color=red>{healthPer}%</color>, its attack is increased by <color=green>{incAttackPer}%</color> and its defense is increased by <color=green>{incDefenseAmount}</color>.";

        FRSkillDesc = $"Wenn die Gesundheit dieser Einheit unter <color=red>{healthPer}%</color> liegt, wird ihr Angriff um <color=green>{incAttackPer}%</color> erhöht und ihre Verteidigung um <color=green>{incDefenseAmount}</color> erhöht.";

        ITSkillDesc = $"Quando la salute di questa unità è inferiore a <color=red>{healthPer}%</color>, il suo attacco aumenta di <color=green>{incAttackPer}%</color> e la sua difesa aumenta di <color=green>{incDefenseAmount}</color>.";

        DESkillDesc = $"Wenn die Gesundheit dieser Einheit unter <color=red>{healthPer}%</color> liegt, wird ihr Angriff um <color=green>{incAttackPer}%</color> erhöht und ihre Verteidigung um <color=green>{incDefenseAmount}</color> erhöht.";

        ESSkillDesc = $"Cuando la salud de esta unidad está por debajo de <color=red>{healthPer}%</color>, su ataque aumentará en <color=green>{incAttackPer}%</color> y su defensa aumentará en <color=green>{incDefenseAmount}</color>.";

        JASkillDesc = $"このユニットの体力が<color=red>{healthPer}%</color>未満になると、攻撃が<color=green>{incAttackPer}%</color>増加し、防御が<color=green>{incDefenseAmount}</color>増加します。";

        PT_BRSkillDesc = $"Quando a saúde desta unidade estiver abaixo de <color=red>{healthPer}%</color>, seu ataque aumentará em <color=green>{incAttackPer}%</color> e sua defesa aumentará em <color=green>{incDefenseAmount}</color>.";

        RUSkillDesc = $"Когда здоровье этой единицы опускается ниже <color=red>{healthPer}%</color>, её атака увеличивается на <color=green>{incAttackPer}%</color>, а защита увеличивается на <color=green>{incDefenseAmount}</color>.";

        ZH_HANSSkillDesc = $"当敌方或友方单位的生命值低于 <color=red>{healthPer}%</color> 时，其攻击力提高 <color=green>{incAttackPer}%</color>，防御力提高 <color=green>{incDefenseAmount}</color>。";
    }
}