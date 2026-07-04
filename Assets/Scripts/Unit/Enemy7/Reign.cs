using UnityEngine;

public class Reign : UnitDefault
{
    [Header("군림")]
    public float incAttackPer;
    public float incDefensePer;
    public float incAttackSpeedPer;
    public float incLifeStealPer;
    public float duration;

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
        SoundPlay(skillSoundClip);

        foreach (UnitDefault friendly in enemies) {
            GameObject effect = Instantiate(attackObj, friendly.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            friendly.SetStat(friendly.attackPowerStat, incAttackPer, true, true, duration);
            friendly.SetStat(friendly.defenseStat, incDefensePer, true, true, duration);
            friendly.SetStat(friendly.attackSpeedStat, incAttackSpeedPer, true, true, duration);
            friendly.SetStat(friendly.lifeStealPerStat, incLifeStealPer, false, true, duration);
            friendly.SetStateBar(State.reign, duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 모든 아군 유닛의 공격력을 <color=green>{incAttackPer}%</color>, 방어력을 <color=green>{incDefensePer}%</color>, 공격 속도를 <color=green>{incAttackSpeedPer}%</color>, 피해 흡혈을 <color=green>{incLifeStealPer}%p</color> 상승시킵니다.";
        
        ENSkillDesc = $"Increases all allied units' attack by <color=green>{incAttackPer}%</color>, defense by <color=green>{incDefensePer}%</color>, attack speed by <color=green>{incAttackSpeedPer}%</color>, and life steal by <color=green>{incLifeStealPer}%p</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Augmente l'attaque de toutes les unités alliées de <color=green>{incAttackPer}%</color>, la défense de <color=green>{incDefensePer}%</color>, la vitesse d'attaque de <color=green>{incAttackSpeedPer}%</color> et le vol de vie de <color=green>{incLifeStealPer}%p</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Aumenta l'attacco di tutte le unità alleate del <color=green>{incAttackPer}%</color>, la difesa del <color=green>{incDefensePer}%</color>, la velocità di attacco del <color=green>{incAttackSpeedPer}%</color> e il furto di vita del <color=green>{incLifeStealPer}%p</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Erhöht den Angriff aller verbündeten Einheiten um <color=green>{incAttackPer}%</color>, die Verteidigung um <color=green>{incDefensePer}%</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> und die Lebensraub um <color=green>{incLifeStealPer}%p</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Aumenta el ataque de todas las unidades aliadas en <color=green>{incAttackPer}%</color>, la defensa en <color=green>{incDefensePer}%</color>, la velocidad de ataque en <color=green>{incAttackSpeedPer}%</color> y el robo de vida en <color=green>{incLifeStealPer}%p</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"すべての味方ユニットの攻撃力を<color=green>{incAttackPer}%</color>、防御力を<color=green>{incDefensePer}%</color>、攻撃速度を<color=green>{incAttackSpeedPer}%</color>、ライフスティールを<color=green>{incLifeStealPer}%p</color>、<color=yellow>{duration}秒</color>間増加させます。";

        PT_BRSkillDesc = $"Aumenta o ataque de todas as unidades aliadas em <color=green>{incAttackPer}%</color>, a defesa em <color=green>{incDefensePer}%</color>, a velocidade de ataque em <color=green>{incAttackSpeedPer}%</color> e o roubo de vida em <color=green>{incLifeStealPer}%p</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Увеличивает атаку всех союзных юнитов на <color=green>{incAttackPer}%</color>, защиту на <color=green>{incDefensePer}%</color>, скорость атаки на <color=green>{incAttackSpeedPer}%</color> и кражу жизни на <color=green>{incLifeStealPer}%p</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"将所有友方单位的攻击力提高<color=green>{incAttackPer}%</color>，防御力提高<color=green>{incDefensePer}%</color>，攻击速度提高<color=green>{incAttackSpeedPer}%</color>，生命偷取提高<color=green>{incLifeStealPer}%p</color>，持续<color=yellow>{duration}秒</color>。";
    }
}