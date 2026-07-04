using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StageData
{
    public List<StageEnemyData> monsterDatas;
    public List<StageEnemyData> eliteDatas;
    public StageEnemyData bossDatas;
}

public class MapLoad : MonoBehaviour
{
    public static MapLoad instance;

    public GameObject mapCan;
    public Image fieldBGImg;
    public List<Sprite> monsterFieldList;
    public List<Sprite> eliteFieldList;
    public List<Sprite> bossFieldList;

    public List<StageData> stageDatas; // 스테이지 번호별 적 정보들

    public GameObject shop;

    public List<GameObject> eventPanel;
    public TextMeshProUGUI eventMapDescTxt;

    public AudioClip monsterBgm;
    public AudioClip eliteBgm;
    public AudioClip eventBgm;
    public AudioClip shopBgm;
    public AudioClip bossBgm;

    private void Awake()
    {
        instance = this;
    }

    public void MonsterRoom()
    {

        AudioManager.instance.PlayBgm(monsterBgm);

        StaticManager.rewardType = RewardType.monster;
        StaticManager.isDeckInit = false;
        GameManager.instance.defaultUnitSpawnBtn.interactable = true;
        GameManager.instance.ClearScore();
        RelicsManager.instance.SetRelicsAvtive();

        StageEnemyData data = stageDatas[StaticManager.curStage - 1].monsterDatas[GameManager.instance.currentLevel < 7 ? 0 : 1];
        EnemySpawner.instance.SpawnEnemies(data);

        fieldBGImg.sprite = monsterFieldList[StaticManager.curStage - 1];
        
        mapCan.SetActive(false);
    }

    public void MonsterRoomElite()
    {
        AudioManager.instance.PlayBgm(eliteBgm);

        StaticManager.rewardType = RewardType.eliteMonster;
        StaticManager.isDeckInit = false;
        GameManager.instance.defaultUnitSpawnBtn.interactable = true;
        GameManager.instance.ClearScore();
        RelicsManager.instance.SetRelicsAvtive();

        StageEnemyData data = stageDatas[StaticManager.curStage - 1].eliteDatas[GameManager.instance.currentLevel < 7 ? 0 : 1];
        EnemySpawner.instance.SpawnEnemies(data);

        fieldBGImg.sprite = eliteFieldList[StaticManager.curStage - 1];

        mapCan.SetActive(false);

        if (SteamUserStats.GetAchievement("두려움 없는 선택", out bool 두려움_없는_선택) && !두려움_없는_선택) {
            SteamUserStats.GetStat("두려움 없는 선택1", out int eliteCnt);
            SteamUserStats.SetStat("두려움 없는 선택1", eliteCnt + 1);
            SteamUserStats.StoreStats();

            if (eliteCnt + 1 == 100) {
                SaveController.instance.saveDataAchi.totalAchiPoint++;
                SaveController.instance.saveDataAchi.achiPoint++;
                SaveController.instance.OnSaveDataAchi();
            }
        }

        StaticManager.visitedElite++;
    }

    public void EventRoom()
    {
        AudioManager.instance.PlayBgm(eventBgm);

        GameManager.instance.timeLimitPanel.SetActive(false);

        int randNum = Random.Range(0, eventPanel.Count);

        EventCardDrop.instance.exitBtn.gameObject.SetActive(true);
        EventCardDrop.instance.EventBoxSpawn();
        EventCardDrop.instance.activePanel = eventPanel[randNum];

        DealMetrics.instance.ResetDealMetrics();

        eventPanel[randNum].SetActive(true);

        mapCan.SetActive(false);

        if (SteamUserStats.GetAchievement("낯설지 않은 우연", out bool 낯설지_않은_우연) && !낯설지_않은_우연) {
            SteamUserStats.GetStat("낯설지 않은 우연1", out int eventCnt);
            SteamUserStats.SetStat("낯설지 않은 우연1", eventCnt + 1);
            SteamUserStats.StoreStats();

            if (eventCnt + 1 == 100) {
                SaveController.instance.saveDataAchi.totalAchiPoint++;
                SaveController.instance.saveDataAchi.achiPoint++;
                SaveController.instance.OnSaveDataAchi();
            }
        }

        StaticManager.visitedEvent++;
    }

    public void ShopRoom()
    {
        AudioManager.instance.PlayBgm(shopBgm);

        GameManager.instance.timeLimitPanel.SetActive(false);

        shop.SetActive(true);

        mapCan.SetActive(false);
    }

    public void BossRoom()
    {
        AudioManager.instance.PlayBgm(bossBgm);

        StaticManager.rewardType = RewardType.bossMonster;
        StaticManager.isDeckInit = false;
        GameManager.instance.defaultUnitSpawnBtn.interactable = true;
        GameManager.instance.ClearScore();
        RelicsManager.instance.SetRelicsAvtive();

        StageEnemyData data = stageDatas[StaticManager.curStage - 1].bossDatas;
        EnemySpawner.instance.SpawnEnemies(data);

        fieldBGImg.sprite = bossFieldList[StaticManager.curStage - 1];

        mapCan.SetActive(false);
    }
}