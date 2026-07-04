using System.Collections;
using UnityEngine;

public class GoblinLancer : UnitDefault
{
    [Header("투혼의 돌격")]
    public float incAttackSpeedPer;
    public float duration;

    bool isSpearFrenzy;
    WaitForSeconds wait;

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStat(attackSpeedStat, incAttackSpeedPer, true, true, duration);

        SetStateBar(State.incAttackSpeed, duration);
        SetStateBar(State.penetrationShoot, duration);

        StartCoroutine(SpearFrenzyTime());
    }

    IEnumerator SpearFrenzyTime()
    {
        isSpearFrenzy = true;
        yield return wait;
        isSpearFrenzy = false;
    }

    public override void OnShortAttack()
    {
        if (!isSpearFrenzy)
            base.OnShortAttack();
        else {
            if (AttackTarget == null)
                return;

            SoundPlay(skillSoundClip);

            CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, isPenetration: true);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인의 공격 속도가 <color=yellow>{duration}초</color> 동안 <color=green>{incAttackSpeedPer}%</color> 증가하며, 공격이 방어력 관통 피해를 입힙니다.";

        ENSkillDesc = $"Increases own attack speed by <color=green>{incAttackSpeedPer}%</color> for <color=yellow>{duration} seconds</color>, and attacks deal defense-penetrating damage.";

        FRSkillDesc = $"Augmente sa vitesse d'attaque de <color=green>{incAttackSpeedPer}%</color> pendant <color=yellow>{duration} secondes</color>, et ses attaques infligent des dégâts perforants la défense.";

        ITSkillDesc = $"Aumenta la propria velocità d'attacco di <color=green>{incAttackSpeedPer}%</color> per <color=yellow>{duration} secondi</color> e gli attacchi infliggono danni che penetrano la difesa.";

        DESkillDesc = $"Erhöht die eigene Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> für <color=yellow>{duration} Sekunden</color> und Angriffe verursachen verteidigungsdurchdringenden Schaden.";

        ESSkillDesc = $"Aumenta su velocidad de ataque en <color=green>{incAttackSpeedPer}%</color> durante <color=yellow>{duration} segundos</color>, y sus ataques infligen daño que penetra la defensa.";

        JASkillDesc = $"自身の攻撃速度が<color=yellow>{duration}秒</color>間<color=green>{incAttackSpeedPer}%</color>増加し、攻撃は防御を無視してダメージを与えます。";

        PT_BRSkillDesc = $"Aumenta sua velocidade de ataque em <color=green>{incAttackSpeedPer}%</color> por <color=yellow>{duration} segundos</color>, e seus ataques causam dano que penetra a defesa.";

        RUSkillDesc = $"Увеличивает собственную скорость атаки на <color=green>{incAttackSpeedPer}%</color> на <color=yellow>{duration} секунд</color>, а атаки наносят урон, игнорирующий защиту.";

        ZH_HANSSkillDesc = $"自身的攻击速度在<color=yellow>{duration}秒</color>内提高<color=green>{incAttackSpeedPer}%</color>，攻击造成忽视防御的伤害。";
    }
}
