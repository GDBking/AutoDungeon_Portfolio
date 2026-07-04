using UnityEngine;

public class Fetter : UnitDefault
{
    [Header("족쇄")]
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

        foreach (UnitDefault enemy in friends) {
            CreateAttackBox(-1f, enemy.gameObject, enemy.transform.position);
            enemy.Bondage(duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"모든 적 유닛들을 <color=yellow>{duration}초</color> 동안 속박합니다.";
        
        ENSkillDesc = $"Bonds all enemy units for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Lie toutes les unités ennemies pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Leghi tutte le unità nemiche per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Bindet alle feindlichen Einheiten für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Ata a todas las unidades enemigas durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"すべての敵ユニットを<color=yellow>{duration}秒</color>間束縛します。";

        PT_BRSkillDesc = $"Vincula a todas as unidades inimigas por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Связывает всех вражеских юнитов на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"将所有敌方单位束缚<color=yellow>{duration}秒</color>。";
    }
}