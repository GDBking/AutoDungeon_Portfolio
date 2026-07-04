using UnityEngine;

public class Fortress : UnitDefault
{
    [Header("보루")]
    public float incDefenseAmount;
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, gameObject, transform.position);

        SetStat(defenseStat, incDefenseAmount, false, true, duration);
        SetStateBar(State.incDefense, duration);

        foreach (UnitDefault enemy in friends) {
            enemy.Taunt(gameObject, duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 본인의 방어력을 <color=green>{incDefenseAmount}</color>만큼 상승시키고, 모든 적을 도발합니다.";
        
        ENSkillDesc = $"Increases own defense by <color=green>{incDefenseAmount}</color> for <color=yellow>{duration} seconds</color> and taunts all enemies.";

        FRSkillDesc = $"Augmente sa défense de <color=green>{incDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color> et provoque tous les ennemis.";

        ITSkillDesc = $"Aumenta la propria difesa di <color=green>{incDefenseAmount}</color> per <color=yellow>{duration} secondi</color> e provoca tutti i nemici.";

        DESkillDesc = $"Erhöht die eigene Verteidigung um <color=green>{incDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color> und provoziert alle Gegner.";

        ESSkillDesc = $"Aumenta la defensa propia en <color=green>{incDefenseAmount}</color> durante <color=yellow>{duration} segundos</color> y provoca a todos los enemigos.";

        JASkillDesc = $"自身の防御力が<color=green>{incDefenseAmount}</color>だけ<color=yellow>{duration}秒</color>間増加し、すべての敵を挑発します。";

        PT_BRSkillDesc = $"Aumenta a defesa própria em <color=green>{incDefenseAmount}</color> por <color=yellow>{duration} segundos</color> e provoca todos os inimigos.";

        RUSkillDesc = $"Увеличивает собственную защиту на <color=green>{incDefenseAmount}</color> на <color=yellow>{duration} секунд</color> и провоцирует всех врагов.";

        ZH_HANSSkillDesc = $"在<color=yellow>{duration}秒</color>内，自己的防御力提高<color=green>{incDefenseAmount}</color>，并嘲讽所有敌人。";
    }
}
