using TMPro;
using UnityEngine;

public class Runaway : UnitDefault
{
    [Header("폭주")]
    public float incAttackAmount;
    public float incDefenseAmount;
    public float incAttackSpeedAmount;
    public float incSpeedAmount;
    public TextMeshProUGUI stackTxt;

    int stack;

    protected override void Start()
    {
        base.Start();

        uiStateBar.GetChild(0).gameObject.SetActive(true);
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        SetStat(attackPowerStat, incAttackAmount, false, true);
        SetStat(defenseStat, incDefenseAmount, false, true);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true);
        SetStat(speedStat, incSpeedAmount, false, true);

        stackTxt.text = (++stack).ToString();
    }

    public override void OnShortAttack()
    {
        base.OnShortAttack();

        OnSkillAttack();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"기본 공격을 가할 때마다 공격력이 <color=green>{incAttackAmount}</color>, 방어력이 <color=green>{incDefenseAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>, 이동 속도가 <color=green>{incSpeedAmount}</color>씩 증가합니다.";
        
        ENSkillDesc = $"Increases attack power by <color=green>{incAttackAmount}</color>, defense by <color=green>{incDefenseAmount}</color>, attack speed by <color=green>{incAttackSpeedAmount}</color>, and movement speed by <color=green>{incSpeedAmount}</color> each time a basic attack is made.";

        FRSkillDesc = $"Augmente la puissance d'attaque de <color=green>{incAttackAmount}</color>, la défense de <color=green>{incDefenseAmount}</color>, la vitesse d'attaque de <color=green>{incAttackSpeedAmount}</color> et la vitesse de déplacement de <color=green>{incSpeedAmount}</color> à chaque fois qu'une attaque de base est effectuée.";

        ITSkillDesc = $"Aumenta la potenza di attacco di <color=green>{incAttackAmount}</color>, la difesa di <color=green>{incDefenseAmount}</color>, la velocità di attacco di <color=green>{incAttackSpeedAmount}</color> e la velocità di movimento di <color=green>{incSpeedAmount}</color> ogni volta che viene effettuato un attacco base.";

        DESkillDesc = $"Erhöht die Angriffskraft um <color=green>{incAttackAmount}</color>, die Verteidigung um <color=green>{incDefenseAmount}</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color> und die Bewegungsgeschwindigkeit um <color=green>{incSpeedAmount}</color>, jedes Mal, wenn ein Grundangriff ausgeführt wird.";

        ESSkillDesc = $"Aumenta el poder de ataque en <color=green>{incAttackAmount}</color>, la defensa en <color=green>{incDefenseAmount}</color>, la velocidad de ataque en <color=green>{incAttackSpeedAmount}</color> y la velocidad de movimiento en <color=green>{incSpeedAmount}</color> cada vez que se realiza un ataque básico.";

        JASkillDesc = $"通常攻撃を行うたびに、攻撃力が<color=green>{incAttackAmount}</color>、防御力が<color=green>{incDefenseAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>、移動速度が<color=green>{incSpeedAmount}</color>ずつ増加します。";

        PT_BRSkillDesc = $"Aumenta o poder de ataque em <color=green>{incAttackAmount}</color>, a defesa em <color=green>{incDefenseAmount}</color>, a velocidade de ataque em <color=green>{incAttackSpeedAmount}</color> e a velocidade de movimento em <color=green>{incSpeedAmount}</color> cada vez que um ataque básico é realizado.";

        RUSkillDesc = $"Увеличивает силу атаки на <color=green>{incAttackAmount}</color>, защиту на <color=green>{incDefenseAmount}</color>, скорость атаки на <color=green>{incAttackSpeedAmount}</color> и скорость передвижения на <color=green>{incSpeedAmount}</color> каждый раз, когда выполняется базовая атака.";

        ZH_HANSSkillDesc = $"每次进行基本攻击时，攻击力提高<color=green>{incAttackAmount}</color>，防御力提高<color=green>{incDefenseAmount}</color>，攻击速度提高<color=green>{incAttackSpeedAmount}</color>，移动速度提高<color=green>{incSpeedAmount}</color>。";
    }
}
