using UnityEngine;

public class Retribution : UnitDefault
{
    [Header("응보")]
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
            if (enemy.Target == gameObject) {
                CreateAttackBox(-1f, null, enemy.transform.position);
                enemy.OnStunAnim(duration);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인을 공격 중인 적 유닛들을 <color=yellow>{duration}초</color> 동안 기절시킵니다.";
        
        ENSkillDesc = $"Stuns enemy units attacking self for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Étourdit les unités ennemies attaquant soi-même pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Stordisce le unità nemiche che attaccano se stessi per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Betäubt feindliche Einheiten, die sich selbst angreifen, für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Aturde a las unidades enemigas que atacan a uno mismo durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"自分を攻撃している敵ユニットを<color=yellow>{duration}秒</color>間気絶させます。";

        PT_BRSkillDesc = $"Aturde as unidades inimigas que atacam a si mesmo por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Оглушает вражеские юниты, атакующие себя, на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"使攻击自己的敌方单位昏迷<color=yellow>{duration}秒</color>。";
    }
}
