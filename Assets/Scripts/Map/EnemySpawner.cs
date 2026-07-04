using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    public GameObject prefab;
    public Vector2 pos;
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    readonly List<EnemyInfo> enemyInfos = new();

    private void Awake()
    {
        instance = this;
    }

    int cost;
    bool isMaxCost;
    bool isBossSpawn;
    readonly List<GameObject> temp = new();
    public void SpawnEnemies(StageEnemyData data)
    {
        enemyInfos.Clear();
        cost = data.cost;
        isMaxCost = false;
        isBossSpawn = false;

        while (cost > 0) {
            if (!isMaxCost) {
                temp.Clear();
                for (int i = 0; i < cost && i < data.enemyList.Count && i < 4; i++) {
                    foreach (GameObject enemyPrefab in data.enemyList[i].list) {
                        temp.Add(enemyPrefab);
                    }
                }
            }

            GameObject prefab;
            if (!isBossSpawn && data.enemyList.Count == 5) {
                prefab = data.enemyList[4].list[0];
                isBossSpawn = true;
            }
            else {
                prefab = temp[Random.Range(0, temp.Count)];
            }

            GameObject enemy = Instantiate(prefab, RandomPositionManager.instance.enemyFieldList[StaticManager.curStage - 1]);
            Vector2 pos = RandomPositionManager.instance.GetRandomEnemyFieldPos();
            enemy.GetComponent<RectTransform>().anchoredPosition = pos;

            EnemyInfo enemyInfo = new()
            {
                prefab = prefab,
                pos = enemy.transform.position
            };
            enemyInfos.Add(enemyInfo);

            cost -= (int)enemy.GetComponent<UnitDefault>().rank + 1;

            isMaxCost = cost >= 4 || cost >= data.enemyList.Count;
        }
    }

    public void LoadEnemy()
    {
        foreach(EnemyInfo enemyInfo in enemyInfos)
        {
            GameObject go = Instantiate(enemyInfo.prefab, RandomPositionManager.instance.enemyFieldList[StaticManager.curStage - 1]);
            go.transform.position = enemyInfo.pos;
        }
    }
}