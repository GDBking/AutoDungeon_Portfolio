using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;
    private bool _isInitialized = false;

    private void Awake()
    {
        /*if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }*/
    }

    /*async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }*/

    // 캐릭터 선택
    public void SeletCharacter(string Character)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("CharacterSelection")
        {
            { "CharSelect", Character }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    // 클리어 수
    public void StageClear(int Clear)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Clear")
        {
            {"Clear", Clear }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    // 몇 스테이지에 졌는 지
    public void LoserStage(int Stage)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Clear_Stage")
        {
            {"Clear_Stage", Stage }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    // 클리어 했을 때 남은 돈
    public void ClearMoney(int Money)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Clear_Money")
        {
            {"Clear_Money", Money }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    // 클리어 했을 때 캐릭터
    public void ClearCharacter(string Character)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Clear_Character")
        {
            {"Clear_Characters", Character }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    /*  // 사용자를 스팀 아이디로 바꾸기
      public void SteamName(string SteamName)
      {
          if (!_isInitialized)
          {
              return;
          }
          CustomEvent myEvent = new("SteamName")
          {
              {"SteamName", SteamName }
          };
          AnalyticsService.Instance.RecordEvent(myEvent);
      }*/

    // 유닛을 뭘 많이 사용하는 지
    public void FieldUnits(string UnitName)
    {
        if (!_isInitialized)
            return;
      
        CustomEvent myEvent = new("Field_Unit")
        {
            {"Field_Unit", UnitName }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    // 시너지 뭘 많이 사용하는 지
    public void Synergys(string Synergy)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new("Synergy")
        {
            {"Synergy", Synergy }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}