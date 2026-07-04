using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;
using Steamworks;

public class AnalyticsInitializer : MonoBehaviour
{
    public static AnalyticsInitializer instance;
    private bool _isInitialized = false;

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }

    public void FieldUnits(string UnitName)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Field_Unit")
        {
            { "Field_Unit", UnitName }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void DefeatGame()
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Defeat_Game")
        {
            { "Cur_Stage", StaticManager.curStage },
            { "Cur_Level", GameManager.instance.currentLevel },
            { "Map_Type", (int)StaticManager.rewardType },
            { "Cur_Gold", StaticManager.gold },
            { "Cur_Cost", StaticManager.curCost },
            { "Cur_Stone", StaticManager.upStone },
            { "Cur_Deck_Cnt", StaticManager.allDeckList.Count },
            { "Play_Time_Min", StaticManager.playTime / 60 },
            { "Cur_HP", StaticManager.curHP },
            { "Cur_HP_Price", StaticManager.HPPrice },
            { "Total_Info", SteamUser.GetSteamID().ToString() }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void EndGame(string MVPName)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("End_Game")
        {
            { "Cur_Stage", StaticManager.curStage },
            { "Cur_Level", GameManager.instance.currentLevel },
            { "Map_Type", (int)StaticManager.rewardType },
            { "Cur_Gold", StaticManager.gold },
            { "Cur_Cost", StaticManager.curCost },
            { "Cur_Stone", StaticManager.upStone },
            { "Cur_Deck_Cnt", StaticManager.allDeckList.Count },
            { "Play_Time_Min", StaticManager.playTime / 60 },
            { "Cur_HP", StaticManager.curHP },
            { "Cur_HP_Price", StaticManager.HPPrice },
            { "MVP_Unit_Name", MVPName },
            { "Total_Achieve_Point", SaveController.instance.saveDataAchi.totalAchiPoint },
            { "Cur_Achieve_Point", SaveController.instance.saveDataAchi.achiPoint },
            { "Total_Info", SteamUser.GetSteamID().ToString() }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void BossClear()
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Boss_Clear")
        {
            { "Cur_Stage", StaticManager.curStage },
            { "Cur_Gold", StaticManager.gold },
            { "Cur_Cost", StaticManager.curCost },
            { "Cur_Stone", StaticManager.upStone },
            { "Cur_Deck_Cnt", StaticManager.allDeckList.Count },
            { "Play_Time_Min", StaticManager.playTime / 60 },
            { "Cur_HP", StaticManager.curHP },
            { "Cur_HP_Price", StaticManager.HPPrice },
            { "Total_Achieve_Point", SaveController.instance.saveDataAchi.totalAchiPoint },
            { "Cur_Achieve_Point", SaveController.instance.saveDataAchi.achiPoint },
            { "Total_Info", SteamUser.GetSteamID().ToString() }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void UseItem(string ID)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Use_Item")
        {
            { "Item_ID", ID }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void Upgrade(string name, int level)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Upgrade")
        {
            { "Unit_Name", name },
            { "Unit_Level", level },
            { "Unit_Name_Level", $"{name} {level}" }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}
