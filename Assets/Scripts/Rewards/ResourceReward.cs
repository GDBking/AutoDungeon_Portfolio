using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ResourceInfo
{
    public int costMin;
    public int costMax;
    public int moneyMin;
    public int moneyMax;
    public int stoneMin;
    public int stoneMax;
}

[System.Serializable]
public class ResourceInfoList
{
    public List<ResourceInfo> resourceInfo;
}

public class ResourceReward : MonoBehaviour
{
    public List<ResourceInfoList> resourceInfoList;
    public List<GameObject> resourcePrefabList;
    public List<Transform> resourceUIList;
    public Transform rewardGrid;
    public GameObject getRewardsBtn;
    public GameObject retryBtn;
    public GameObject nextMapBtn;

    readonly List<Transform> rewardsObjList = new();
    readonly int[] resourceAmount = new int[3];

    public void GetRewardsBtnClick()
    {
        getRewardsBtn.SetActive(false);

        for (int i = 0; i < rewardsObjList.Count; i++) {
            int idx = i;
            rewardsObjList[i].SetParent(resourceUIList[^1]);

            rewardsObjList[i].DOMove(resourceUIList[i].position, 1f)
                .SetEase(Ease.InOutSine);

            rewardsObjList[i].DOScale(rewardsObjList[i].localScale / 4f, 1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (idx == rewardsObjList.Count - 1)
                        SetResources();
                });
        }
    }

    void SetResources()
    {
        foreach (Transform reward in rewardsObjList) {
            Destroy(reward.gameObject);
        }

        GameManager.instance.SetCost(resourceAmount[0]);
        if (StaticManager.isWin) {
            GameManager.instance.SetGold(resourceAmount[1]);
            GameManager.instance.SetUpgradeStone(resourceAmount[2]);
        }

        if (!StaticManager.isWin)
            retryBtn.SetActive(true);
        else
            nextMapBtn.SetActive(true);
    }

    public void RetryBtnClick()
    {
        gameObject.SetActive(false);
        GameManager.instance.resultPanel.SetActive(false);

        // 초기 덱을 다시 뽑을 수 있도록 수정
        StaticManager.isDeckInit = false;
        // 기본 유닛 생성 버튼 활성화
        GameManager.instance.defaultUnitSpawnBtn.interactable = true;
        // 적 유닛 다시 생성
        EnemySpawner.instance.LoadEnemy();
        // 점수판 초기화
        GameManager.instance.ClearScore();
    }

    public void NextMapBtnClick()
    {
        gameObject.SetActive(false);
        GameManager.instance.resultPanel.SetActive(false);

        GameManager.instance.UpdateStageUI(++GameManager.instance.currentLevel);
        GameManager.instance.map.SetActive(true);

        if (GameManager.instance.currentLevel == 1)
            SaveController.instance.OnSaveData();
    }

    private void OnEnable()
    {
        ResourceInfo resource = resourceInfoList[StaticManager.curStage - 1].resourceInfo[(int)(StaticManager.isWin ? StaticManager.rewardType : RewardType.eliteMonster)];
        resourceAmount[0] = Random.Range(resource.costMin, resource.costMax + 1);
        resourceAmount[1] = Random.Range(resource.moneyMin, resource.moneyMax + 1);
        resourceAmount[2] = Random.Range(resource.stoneMin, resource.stoneMax + 1);

        for (int i = 0; i < resourcePrefabList.Count; i++) {
            GameObject contents = Instantiate(resourcePrefabList[i], rewardGrid);
            contents.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(resourceAmount[i].ToString());

            rewardsObjList.Add(contents.transform);

            if (!StaticManager.isWin)
                break;
        }
    }

    private void OnDisable()
    {
        getRewardsBtn.SetActive(true);
        retryBtn.SetActive(false);
        nextMapBtn.SetActive(false);

        rewardsObjList.Clear();
    }
}