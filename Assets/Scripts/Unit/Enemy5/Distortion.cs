using System.Collections.Generic;
using UnityEngine;

public class Distortion : UnitDefault
{
    [Header("왜곡")]
    public float incHealthPer;

    List<UnitDefault> enemyUnits;

    protected override void Awake()
    {
        base.Awake();

        isHealer = true;
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
                foreach (UnitDefault enemy in enemyUnits) {
                    if (enemy.isDeath) {
                        UseSkill();
                        break;
                    }
                }
            }
            enemyUnits = new(friends);
        }
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        foreach (UnitDefault friendly in enemies) {
            CreateAttackBox(-1f, null, friendly.transform.position);
            friendly.Healing(friendly.MaxHealth / 100f * incHealthPer);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"적 유닛 사망 시 모든 아군 유닛의 체력을 <color=green>{incHealthPer}%</color> 회복시킵니다.";
        
        ENSkillDesc = $"When an enemy unit dies, heals all allied units by <color=green>{incHealthPer}%</color> of their max health.";

        FRSkillDesc = $"Lorsque une unité ennemie meurt, soigne toutes les unités alliées de <color=green>{incHealthPer}%</color> de leur santé maximale.";

        ITSkillDesc = $"Quando un'unità nemica muore, cura tutte le unità alleate del <color=green>{incHealthPer}%</color> della loro salute massima.";

        DESkillDesc = $"Wenn eine feindliche Einheit stirbt, heilt sie alle verbündeten Einheiten um <color=green>{incHealthPer}%</color> ihrer maximalen Gesundheit.";

        ESSkillDesc = $"Cuando una unidad enemiga muere, cura a todas las unidades aliadas en un <color=green>{incHealthPer}%</color> de su salud máxima.";

        JASkillDesc = $"敵ユニットが死亡すると、すべての味方ユニットの体力を<color=green>{incHealthPer}%</color>回復します。";

        PT_BRSkillDesc = $"Quando uma unidade inimiga morre, cura todas as unidades aliadas em <color=green>{incHealthPer}%</color> de sua saúde máxima.";

        RUSkillDesc = $"Когда вражеский юнит умирает, исцеляет всех союзных юнитов на <color=green>{incHealthPer}%</color> от их максимального здоровья.";

        ZH_HANSSkillDesc = $"当敌方单位死亡时，治疗所有友方单位，恢复其最大生命值的<color=green>{incHealthPer}%</color>。";
    }
}
