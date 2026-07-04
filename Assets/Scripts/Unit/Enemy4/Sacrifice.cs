using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : UnitDefault
{
    [Header("제물")]
    public float decAttackPer;
    public float decAttackSpeedPer;
    public float duration;

    List<UnitDefault> enemyUnits;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void Start()
    {
        base.Start();

        enemyUnits = new(friends);
    }

    private void Update()
    {
        if (enemyUnits.Count != friends.Count) {
            if (friends.Count < enemyUnits.Count) {
                UseSkill();
            }
            enemyUnits = new(friends);
        }
    }

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
            CreateAttackBox(-1f, null, enemy.transform.position);

            enemy.Silence(duration);
            enemy.SetStat(enemy.attackPowerStat, decAttackPer, true, false, duration);
            enemy.SetStat(enemy.attackSpeedStat, decAttackSpeedPer, true, false, duration);

            enemy.SetStateBar(State.silence, duration);
            enemy.SetStateBar(State.decAttack, duration);
            enemy.SetStateBar(State.decAttackSpeed, duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"적 유닛 사망 시 모든 적 유닛에게 <color=yellow>{duration}초</color> 동안 침묵을 걸고 공격력을 <color=red>{decAttackPer}%</color>, 공격 속도를 <color=red>{decAttackSpeedPer}%</color> 감소시킵니다.";
        
        ENSkillDesc = $"When an enemy unit dies, silences all enemy units for <color=yellow>{duration} seconds</color> and decreases their attack power by <color=red>{decAttackPer}%</color> and attack speed by <color=red>{decAttackSpeedPer}%</color>.";

        FRSkillDesc = $"Wenn eine feindliche Einheit stirbt, verstummen alle feindlichen Einheiten für <color=yellow>{duration} Sekunden</color> und ihre Angriffskraft um <color=red>{decAttackPer}%</color> und Angriffsgeschwindigkeit um <color=red>{decAttackSpeedPer}%</color> verringert.";

        ITSkillDesc = $"Quando un'unità nemica muore, silenzia tutte le unità nemiche per <color=yellow>{duration} secondi</color> e diminuisce la loro potenza d'attacco del <color=red>{decAttackPer}%</color> e la velocità d'attacco del <color=red>{decAttackSpeedPer}%</color>.";

        DESkillDesc = $"Wenn eine feindliche Einheit stirbt, verstummen alle feindlichen Einheiten für <color=yellow>{duration} Sekunden</color> und ihre Angriffskraft um <color=red>{decAttackPer}%</color> und Angriffsgeschwindigkeit um <color=red>{decAttackSpeedPer}%</color> verringert.";

        ESSkillDesc = $"Cuando una unidad enemiga muere, silencia a todas las unidades enemigas durante <color=yellow>{duration} segundos</color> y disminuye su poder de ataque en <color=red>{decAttackPer}%</color> y su velocidad de ataque en <color=red>{decAttackSpeedPer}%</color>.";

        JASkillDesc = $"敵ユニットが死亡すると、すべての敵ユニットに<color=yellow>{duration}秒</color>間沈黙を付与し、攻撃力を<color=red>{decAttackPer}%</color>、攻撃速度を<color=red>{decAttackSpeedPer}%</color>減少させます。";

        PT_BRSkillDesc = $"Quando uma unidade inimiga morre, silencia todas as unidades inimigas por <color=yellow>{duration} segundos</color> e diminui seu poder de ataque em <color=red>{decAttackPer}%</color> e sua velocidade de ataque em <color=red>{decAttackSpeedPer}%</color>.";

        RUSkillDesc = $"Когда вражеский юнит умирает, накладывает молчание на все вражеские юниты на <color=yellow>{duration} секунд</color> и уменьшает их силу атаки на <color=red>{decAttackPer}%</color> и скорость атаки на <color=red>{decAttackSpeedPer}%</color>.";

        ZH_HANSSkillDesc = $"敌方单位死亡时，使所有敌方单位沉默<color=yellow>{duration}秒</color>，并使其攻击力降低<color=red>{decAttackPer}%</color>，攻击速度降低<color=red>{decAttackSpeedPer}%</color>。";
    }
}
