using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string steamID;

    public int mapSeed;

    public int curHP;
    public int gold;
    public int curCost;
    public int upStone;
    public int curStage = 1;
    public int curLevel;

    public List<string> allDeckList;

    public UnitUpgradeStat[] stat;
    public int[] maxLevel;
    public int[] skillPoint;
    public Title[] title = new Title[StaticManager.title.SelectMany(inner => inner).Count()];
    public string[] titleString;

    public int HPPrice;

    public List<RelicsProb> relicsProbs;
    public List<RelicsProb> enemyRelicsProbs;

    public List<RewardProbability> rewardList;

    public int donationProb;

    public int relicsSealCnt;

    public int[] potionLevel;

    public int[] unitCnt;
    public FloatArray[] totalScore;
    public int playTime;

    public int visitedElite;
    public int visitedEvent;
}

[Serializable]
public class SaveDataAchi
{
    public int totalAchiPoint;
    public int achiPoint;
    public bool[] isCheck = new bool[10];
}

public class SaveController : MonoBehaviour
{
    public static SaveController instance;

    [HideInInspector] public SaveData saveData;
    [HideInInspector] public SaveDataAchi saveDataAchi;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GetSaveData();
    }

    public void GetSaveData()
    {
        saveData = SaveManager.Load();
        saveDataAchi = SaveManager.LoadAchi();
    }

    // 보스 레벨 클리어 후 저장
    public void OnSaveData()
    {
        saveData.steamID = SteamUser.GetSteamID().ToString();

        saveData.mapSeed = StaticManager.mapSeed;

        saveData.curHP = StaticManager.curHP;
        saveData.gold = StaticManager.gold;
        saveData.curCost = StaticManager.curCost;
        saveData.upStone = StaticManager.upStone;
        saveData.curStage = StaticManager.curStage;
        saveData.curLevel = GameManager.instance.currentLevel;

        saveData.allDeckList = StaticManager.allDeckList;

        saveData.stat = StaticManager.stat;
        saveData.maxLevel = StaticManager.maxLevel;
        saveData.skillPoint = StaticManager.skillPoint;
        for (int i = 0; i < StaticManager.title.Length; i++)
            for (int j = 0; j < StaticManager.title[i].Length; j++)
                saveData.title[i * StaticManager.title[i].Length + j] = StaticManager.title[i][j];
        saveData.titleString = StaticManager.titleString;

        saveData.HPPrice = StaticManager.HPPrice;

        saveData.relicsProbs = GameManager.instance.relicsProbs;
        saveData.enemyRelicsProbs = GameManager.instance.enemyRelicsProbs;

        saveData.rewardList = CardReward.instance.rewardList;

        saveData.donationProb = StaticManager.donationProb;

        saveData.relicsSealCnt = StaticManager.relicsSealCnt;
        
        saveData.potionLevel = StaticManager.potionLevel;

        saveData.totalScore = StaticManager.totalScore;
        saveData.playTime = StaticManager.playTime;

        saveData.visitedElite = StaticManager.visitedElite;
        saveData.visitedEvent = StaticManager.visitedEvent;

        SaveManager.Save(saveData);
    }

    // 이어하기 버튼 클릭 시 불러오기
    public void ApplyLoadedData()
    {
        StaticManager.mapSeed = saveData.mapSeed;

        StaticManager.curHP = saveData.curHP;
        StaticManager.gold = saveData.gold;
        StaticManager.curCost = saveData.curCost;
        StaticManager.upStone = saveData.upStone;
        StaticManager.curStage = saveData.curStage;

        StaticManager.allDeckList = saveData.allDeckList;

        StaticManager.stat = saveData.stat;
        StaticManager.maxLevel = saveData.maxLevel;
        StaticManager.skillPoint = saveData.skillPoint;
        for (int i = 0; i < StaticManager.title.Length; i++)
            for (int j = 0; j < StaticManager.title[i].Length; j++)
                StaticManager.title[i][j] = saveData.title[i * StaticManager.title[i].Length + j];
        StaticManager.titleString = saveData.titleString;

        StaticManager.HPPrice = saveData.HPPrice;

        StaticManager.relicsProbs = saveData.relicsProbs;
        StaticManager.enemyRelicsProbs = saveData.enemyRelicsProbs;

        StaticManager.rewardList = saveData.rewardList;

        StaticManager.donationProb = saveData.donationProb;

        StaticManager.relicsSealCnt = saveData.relicsSealCnt;

        StaticManager.potionLevel = saveData.potionLevel;

        StaticManager.totalScore = saveData.totalScore;
        StaticManager.playTime = saveData.playTime;

        StaticManager.visitedElite = saveData.visitedElite;
        StaticManager.visitedEvent = saveData.visitedEvent;
    }

    public void OnSaveDataAchi()
    {
        SaveManager.SaveAchi(saveDataAchi);
    }
}
