using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public float respawnTime;

    public AnimationClip friendlyRespawnEffect;
    public AnimationClip enemyRespawnEffect;

    public AudioClip friendlyRespawnSound;
    public AudioClip enemyRespawnSound;

    WaitForSeconds wait;

    private void Awake()
    {
        instance = this;

        wait = new(respawnTime);
    }

    public void RespawnUnit(string ID, int dealIdx, int curMana, float steroid, UnitUpgradeStat itemStat, RectTransform parentRect)
    {
        StartCoroutine(RespawnRoutine(ID, dealIdx, curMana, steroid, itemStat, parentRect));
    }

    IEnumerator RespawnRoutine(string ID, int dealIdx, int curMana, float steroid, UnitUpgradeStat itemStat, RectTransform parentRect)
    {
        yield return wait;

        if (GameManager.instance.isEnd)
            yield break;

        Vector2 randPos = RandomPositionManager.instance.GetRandomFieldPos(parentRect);
        string path;
        if (dealIdx != -1)
            path = "Units/";
        else 
            path = $"Enemy{StaticManager.curStage}/";

        GameObject unit = Instantiate(Resources.Load<GameObject>($"{path}{ID}"), parentRect);
        unit.GetComponent<RectTransform>().anchoredPosition = randPos;

        UnitDefault unitComp = unit.GetComponent<UnitDefault>();
        if (dealIdx != -1)
            RelicsManager.instance.UnitEquipment(unitComp);
        unitComp.dealMetricsIdx = dealIdx;
        unitComp.CurrentMana = curMana;
        unitComp.updateMpBarAction(unitComp.MaxMana, unitComp.CurrentMana);
        unitComp.steroid = steroid;
        unitComp.itemStat = itemStat;
        unitComp.enabled = true;

        if (unitComp.steroid != 0) {
            for (int i = 2; i <= unitComp.steroid; i *= 2) {
                unitComp.SetStateBar(UnitDefault.State.steroid, -2f);
            }
        }

        AudioManager.instance.PlaySfx(dealIdx != -1 ? friendlyRespawnSound : enemyRespawnSound);

        GameObject effect = Instantiate(GameManager.instance.effectObj, unit.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(dealIdx != -1 ? friendlyRespawnEffect : enemyRespawnEffect);
    }
}