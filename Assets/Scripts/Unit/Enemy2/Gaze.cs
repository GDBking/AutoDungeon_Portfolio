using System.Collections.Generic;
using UnityEngine;

public class Gaze : UnitDefault
{
    [Header("시선")]
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0 || Health == MaxHealth)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        List<UnitDefault> gazeList = FindEnemiesWithLowerHPPercent();
        if (gazeList == null)
            return;

        SoundPlay(skillSoundClip);
        foreach (UnitDefault unitComp in gazeList) {
            GameObject effect = Instantiate(attackObj, unitComp.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            unitComp.OnStunAnim(duration);
        }
    }

    List<UnitDefault> FindEnemiesWithLowerHPPercent()
    {
        List<UnitDefault> lowerHpEnemies = new();

        foreach (UnitDefault unitComp in friends) {
            if ((unitComp.Health / unitComp.MaxHealth) > (Health / MaxHealth))
                lowerHpEnemies.Add(unitComp);
        }

        if (lowerHpEnemies.Count == 0)
            return null;
        else
            return lowerHpEnemies;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인보다 체력비가 높은 적 유닛들을 <color=yellow>{duration}초</color> 동안 기절시킵니다.";

        ENSkillDesc = $"Stuns enemy units whose HP percentage is higher than this unit for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Étourdit les unités ennemies dont le pourcentage de PV est supérieur à celui de cette unité pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Stordisce le unità nemiche la cui percentuale di HP è superiore a quella di questa unità per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Betäubt feindliche Einheiten, deren HP-Prozentsatz höher ist als der dieser Einheit, für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Aturde a las unidades enemigas cuyo porcentaje de HP es mayor que el de esta unidad durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"このユニットよりHP割合が高い敵ユニットを<color=yellow>{duration}秒</color>間?絶させます。";

        PT_BRSkillDesc = $"Aturde las unidades enemigas cuyo porcentaje de vida es mayor que el de esta unidad durante <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Оглушает вражеские юниты, у которых процент здоровья выше, чем у этой единицы, на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"眩晕HP百分比高于此单位的敌方单位<color=yellow>{duration}秒</color>。";
    }
}
