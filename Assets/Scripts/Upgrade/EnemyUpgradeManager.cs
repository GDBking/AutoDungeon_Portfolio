using System.Collections.Generic;
using UnityEngine;

public class EnemyUpgradeManager : MonoBehaviour
{
    public static EnemyUpgradeManager instance;

    public List<UpgradeOptionData> upgradeDatas;
    public List<UnitDefault> enemyUnitList;

    private void Awake()
    {
        instance = this;
    }

    public void EnemyUpgrade()
    {
        for (int i = 63; i < StaticManager.stat.Length; i++) {
            for (int j = 0; j < 3 * enemyUnitList[i - 48].Level; j++) {
                UpgradeOptionData upData = upgradeDatas[Random.Range(0, upgradeDatas.Count)];

                switch (upData.upgradeType) {
                    case UpgradeOptionData.UpgradeType.healthPer:
                        StaticManager.stat[i].maxHealth += enemyUnitList[i - 48].MaxHealth * (upData.value / 100f);
                        break;
                    case UpgradeOptionData.UpgradeType.attackPer:
                        StaticManager.stat[i].attackPower += enemyUnitList[i - 48].AttackPower * (upData.value / 100f);
                        break;
                    case UpgradeOptionData.UpgradeType.defensePer:
                        StaticManager.stat[i].defense += upData.value;
                        break;
                    case UpgradeOptionData.UpgradeType.attackSpeedPer:
                        StaticManager.stat[i].attackSpeed += enemyUnitList[i - 48].AttackSpeed * (upData.value / 100f);
                        break;
                    case UpgradeOptionData.UpgradeType.attackRange:
                        StaticManager.stat[i].attackRange += upData.value;
                        break;
                    case UpgradeOptionData.UpgradeType.criticalPer:
                        StaticManager.stat[i].criticalPer += upData.value;
                        break;
                    case UpgradeOptionData.UpgradeType.lifeStealPer:
                        StaticManager.stat[i].lifeStealPer += upData.value;
                        break;
                }
            }
        }
    }
}
