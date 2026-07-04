using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyList
{
    public List<GameObject> list;
}

[CreateAssetMenu(menuName = "Scriptable Object/StageEnemyData")]
public class StageEnemyData : ScriptableObject
{
    public int cost;
    public List<EnemyList> enemyList;
}