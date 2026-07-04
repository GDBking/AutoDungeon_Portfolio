using System.Collections.Generic;
using UnityEngine;

public class RandomPositionManager : MonoBehaviour
{
    public static RandomPositionManager instance;

    public List<RectTransform> playerFieldList;
    public List<RectTransform> enemyFieldList;
    public List<RectTransform> battleFieldList;

    private void Awake()
    {
        instance = this;
    }

    public Vector2 GetRandomPlayerFieldPos()
    {
        RectTransform rectComp = playerFieldList[StaticManager.curStage - 1];

        return GetRandomFieldPos(rectComp);
    }

    public Vector2 GetRandomEnemyFieldPos()
    {
        RectTransform rectComp = enemyFieldList[StaticManager.curStage - 1];

        return GetRandomFieldPos(rectComp);
    }

    public Vector2 GetRandomBattleFieldPos()
    {
        RectTransform rectTransform = battleFieldList[StaticManager.curStage - 1];

        float posX = Random.Range(-rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) / 2f + rectTransform.transform.localPosition.x;
        float posY = Random.Range(-rectTransform.sizeDelta.y, rectTransform.sizeDelta.y) / 2f + rectTransform.transform.localPosition.y;

        return new Vector2(posX, posY) / 100f;
    }

    public Vector2 GetRandomFieldPos(RectTransform rectComp)
    {
        float posX = Random.Range(rectComp.rect.xMin, rectComp.rect.xMax);
        float posY = Random.Range(rectComp.rect.yMin, rectComp.rect.yMax);

        return new Vector2(posX, posY);
    }
}
