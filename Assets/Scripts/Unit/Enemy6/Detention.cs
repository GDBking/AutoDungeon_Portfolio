using UnityEngine;

public class Detention : UnitDefault
{
    [Header("억류")]
    public float decAttackSpeedPer;
    public float decSpeedPer;
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (friends.Count == 0)
            return;

        SoundPlay(skillSoundClip);

        foreach (UnitDefault enemy in friends) {
            GameObject effect = Instantiate(attackObj, enemy.transform);
            effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            enemy.SetStat(enemy.attackSpeedStat, decAttackSpeedPer, true, false, duration);
            enemy.SetStat(enemy.speedStat, decSpeedPer, true, false, duration);
            enemy.SetStateBar(State.decAttackSpeed, duration);
            enemy.SetStateBar(State.decSpeed, duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 모든 적 유닛의 공격 속도를 <color=red>{decAttackSpeedPer}%</color>, 이동 속도를 <color=red>{decSpeedPer}%</color> 감소시킵니다.";
        
        ENSkillDesc = $"Decreases the attack speed of all enemy units by <color=red>{decAttackSpeedPer}%</color> and movement speed by <color=red>{decSpeedPer}%</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Diminue la vitesse d'attaque de toutes les unités ennemies de <color=red>{decAttackSpeedPer}%</color> et la vitesse de déplacement de <color=red>{decSpeedPer}%</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Riduce la velocità di attacco di tutte le unità nemiche del <color=red>{decAttackSpeedPer}%</color> e la velocità di movimento del <color=red>{decSpeedPer}%</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Verringert die Angriffsgeschwindigkeit aller feindlichen Einheiten um <color=red>{decAttackSpeedPer}%</color> und die Bewegungsgeschwindigkeit um <color=red>{decSpeedPer}%</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Disminuye la velocidad de ataque de todas las unidades enemigas en <color=red>{decAttackSpeedPer}%</color> y la velocidad de movimiento en <color=red>{decSpeedPer}%</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"すべての敵ユニットの攻撃速度を<color=red>{decAttackSpeedPer}%</color>、移動速度を<color=red>{decSpeedPer}%</color>、<color=yellow>{duration}秒</color>間減少させます。";

        PT_BRSkillDesc = $"Diminui a velocidade de ataque de todas as unidades inimigas em <color=red>{decAttackSpeedPer}%</color> e a velocidade de movimento em <color=red>{decSpeedPer}%</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Уменьшает скорость атаки всех вражеских юнитов на <color=red>{decAttackSpeedPer}%</color> и скорость передвижения на <color=red>{decSpeedPer}%</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"将所有敌方单位的攻击速度降低<color=red>{decAttackSpeedPer}%</color>，移动速度降低<color=red>{decSpeedPer}%</color>，持续<color=yellow>{duration}秒</color>。";
    }
}
