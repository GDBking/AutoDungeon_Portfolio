using UnityEngine;

public class Smash : UnitDefault
{
    [Header("분쇄")]
    public float incSpeedAmount;
    public float incAttackSpeedAmount;
    public float incDefenseAmount;

    bool isSmash;

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        isSmash = true;
        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, transform.position);

        SetStat(speedStat, incSpeedAmount, false, true);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true);
        SetStat(defenseStat, incDefenseAmount, false, true);

        SetStateBar(State.smash, -2f);
    }

    public override void OnStunAnim(float stunTime)
    {
        if (isSmash)
            return;

        OnSkillAttack();
        base.OnStunAnim(stunTime);
    }

    public override void Bondage(float time, AnimationClip animClip = null)
    {
        if (isSmash)
            return;

        OnSkillAttack();
        base.Bondage(time, animClip);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"군중 제어 효과(기절, 속박)에 피격 시 이동 속도가 <color=green>{incSpeedAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>, 방어력이 <color=green>{incDefenseAmount}</color>만큼 증가하고, 이후 군중 제어 효과에 면역이 생깁니다.";

        ENSkillDesc = $"When hit by a crowd control effect (stun, bondage), increases movement speed by <color=green>{incSpeedAmount}</color>, attack speed by <color=green>{incAttackSpeedAmount}</color>, and defense by <color=green>{incDefenseAmount}</color>, then becomes immune to crowd control effects.";

        FRSkillDesc = $"Lorsqu'il est touché par un effet de contrôle de foule (assommage, entrave), la vitesse de déplacement augmente de <color=green>{incSpeedAmount}</color>, la vitesse d'attaque de <color=green>{incAttackSpeedAmount}</color> et la défense de <color=green>{incDefenseAmount}</color>, puis il devient immunisé aux effets de contrôle de foule.";

        ITSkillDesc = $"Quando viene colpito da un effetto di controllo della folla (stordimento, legame), aumenta la velocità di movimento di <color=green>{incSpeedAmount}</color>, la velocità di attacco di <color=green>{incAttackSpeedAmount}</color> e la difesa di <color=green>{incDefenseAmount}</color>, quindi diventa immune agli effetti di controllo della folla.";

        DESkillDesc = $"Wenn es von einem Massenkontrolleffekt (Betäubung, Fesselung) getroffen wird, erhöht sich die Bewegungsgeschwindigkeit um <color=green>{incSpeedAmount}</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color> und die Verteidigung um <color=green>{incDefenseAmount}</color>, und es wird anschließend gegen Massenkontrolleffekte immun.";

        ESSkillDesc = $"Cuando es golpeado por un efecto de control de masas (aturdimiento, atadura), aumenta la velocidad de movimiento en <color=green>{incSpeedAmount}</color>, la velocidad de ataque en <color=green>{incAttackSpeedAmount}</color> y la defensa en <color=green>{incDefenseAmount}</color>, y luego se vuelve inmune a los efectos de control de masas.";

        JASkillDesc = $"群集制御効果（眩暈、拘束）を受けると、移動速度が<color=green>{incSpeedAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>、防御が<color=green>{incDefenseAmount}</color>だけ上昇し、その後群集制御効果に対して免疫を得ます。";

        PT_BRSkillDesc = $"Quando atingido por um efeito de controle de multidão (atordoamento, restrição), aumenta a velocidade de movimento em <color=green>{incSpeedAmount}</color>, a velocidade de ataque em <color=green>{incAttackSpeedAmount}</color> e a defesa em <color=green>{incDefenseAmount}</color>, e então torna-se imune a efeitos de controle de multidão.";

        RUSkillDesc = $"При получении эффекта контроля толпы (оглушение, связывание) увеличивает скорость движения на <color=green>{incSpeedAmount}</color>, скорость атаки на <color=green>{incAttackSpeedAmount}</color> и защиту на <color=green>{incDefenseAmount}</color>, затем становится невосприимчивым к эффектам контроля толпы.";

        ZH_HANSSkillDesc = $"被群体控制效果（眩晕、束缚）击中时，移动速度增加<color=green>{incSpeedAmount}</color>，攻击速度增加<color=green>{incAttackSpeedAmount}</color>，防御增加<color=green>{incDefenseAmount}</color>，随后获得对群体控制效果的免疫。";
    }
}
