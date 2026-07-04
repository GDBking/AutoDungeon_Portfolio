using DG.Tweening;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("플레이어 자원")]
    public int maxHP = 3;
    [HideInInspector] public int maxCost;
    int eventCost;
    public Text costText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI upgradeStoneText;
    public int currentLevel = 1;
    public List<AudioClip> resourceClipList;

    [Header("맵")]
    public GameObject map;
    // 현재 스테이지에 따라 아이콘 크기 버튼 활성화
    [HideInInspector] public List<GameObject> toButton = new();
    public GameObject mapNode;

    [Header("생명 UI")]
    public List<Image> HPImage;
    public Sprite fullHPImg;
    public Sprite emptyHPImg;

    [Header("전투 시작 버튼")]
    public Button SBTN;

    [Header("기본 유닛")]
    public Button defaultUnitSpawnBtn;
    public GameObject defaultUnitPrefab;
    public AudioClip defaultUnitSpawnClip;
    public AudioClip unitEquipClip;

    [Header("설정 창")]
    public GameObject settingPanel;
    public Setting setting;

    [Header("인스턴스 초기화 목록")]
    public RemainingDeck remainingDeck;
    public DeckManager deckManager;
    public CardReward cardReward;
    public GameObject relicsReward;
    public GameObject resourceReward;
    public Shop shop;

    private void Awake()
    {
        instance = this;

        maxCost = StaticManager.curCost;

        DeckManager.instance = deckManager;
        RemainingDeck.instance = remainingDeck;
        CardReward.instance = cardReward;
        Shop.instance = shop;
        RemainingDeck.instance.Init();

        if (StaticManager.relicsProbs != null) {
            relicsProbs = StaticManager.relicsProbs;
            enemyRelicsProbs = StaticManager.enemyRelicsProbs;
        }

        UnitDefault.attackObj = effectObj;
        UnitDefault.bulletObj = bulletObj;

        SetCost(0);
        SetGold(0);
        SetUpgradeStone(0);
        SetPlayerHP(StaticManager.curHP);

        SetPotionStateSpr();


        trashCan.SetActive(SaveController.instance.saveDataAchi.isCheck[3]);
        HPSlot.SetActive(SaveController.instance.saveDataAchi.isCheck[4]);
    }

    private void Start()
    {
        SetField();
    }

    [Header("체크 패널")]
    public Transform checkPanel;
    public GameObject saveCheckPanel;

    float playTime;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            UnitDefault.isReclicsActive = !UnitDefault.isReclicsActive;
            foreach (string key in StaticManager.unitSpecificComp.Keys) {
                foreach (UnitDefault comp in StaticManager.unitSpecificComp[key]) {
                    comp.relicsState.gameObject.SetActive(UnitDefault.isReclicsActive);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            if (remainingDeck.gameObject.activeSelf) {
                foreach (Transform panel in checkPanel) {
                    if (panel.gameObject.activeSelf) {
                        panel.gameObject.SetActive(false);
                        return;
                    }
                }

                remainingDeck.gameObject.SetActive(false);
            }
            else {
                settingPanel.SetActive(!settingPanel.activeSelf);
                setting.SaveBtnClick();
            }
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Z)) && defaultUnitSpawnBtn.interactable) {
            defaultUnitSpawnBtn.onClick.Invoke();
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X)) && SBTN.interactable) {
            SBTN.onClick.Invoke();
        }

        playTime += Time.deltaTime;
        if (playTime >= 1f) {
            StaticManager.playTime++;
            playTime--;
        }
    }

    public bool SetGold(int amount)
    {
        if (StaticManager.gold + amount < 0) {
            AudioManager.instance.PlaySfx(resourceClipList[3]);
            return false;
        }
        else
            return SetGoldTween(amount);
    }

    Tween goldTween;
    bool SetGoldTween(int amount)
    {
        AudioManager.instance.PlaySfx(resourceClipList[0]);

        if (amount < 0 && SteamUserStats.GetAchievement("지갑을 열다", out bool 지갑을_열다) && !지갑을_열다) {
            SteamUserStats.GetStat("지갑을 열다1", out int gold);
            SteamUserStats.SetStat("지갑을 열다1", gold - amount);
            SteamUserStats.StoreStats();

            if (gold - amount >= 5000) {
                SteamAchievement.Unlock("지갑을 열다");
            }
        }

        int startValue = StaticManager.gold;
        int endValue = StaticManager.gold + amount;

        // 기존 트윈이 있다면 종료
        if (goldTween != null && goldTween.IsActive()) goldTween.Kill();

        goldTween = DOTween.To(() => startValue, x =>
        {
            startValue = x;
            moneyText.text = x.ToString("N0");
        }, endValue, 0.5f).SetEase(Ease.OutQuad);

        StaticManager.gold = endValue;

        if (StaticManager.gold == 0) {
            SteamAchievement.Unlock("올인");
        }

        return true;
    }

    public bool SetCost(int amount)
    {
        if (StaticManager.curCost + eventCost + amount < 0) {
            AudioManager.instance.PlaySfx(resourceClipList[3]);
            return false;
        }
        else
            return SetCostTween(amount);
    }

    Tween costTween;
    bool SetCostTween(int amount)
    {
        AudioManager.instance.PlaySfx(resourceClipList[1]);

        if (eventCost > 0 && amount < 0) {
            eventCost += amount;
            // 이벤트 코스트 만으로 소모 가능할 때
            if (eventCost >= 0) {
                costText.text = $"{StaticManager.curCost}+{eventCost}/{maxCost}";
                return true;
            }
            // 이벤트 코스트 보다 값이 클 때
            else {
                amount = eventCost;
                eventCost = 0;
            }
        }

        int startValue = StaticManager.curCost;
        int endValue = StaticManager.curCost + amount;
        if (startValue < endValue) {
            maxCost = endValue;
        }

        // 기존 트윈이 있다면 종료
        if (costTween != null && costTween.IsActive()) costTween.Kill();

        costTween = DOTween.To(() => startValue, x =>
        {
            startValue = x;
            costText.text = $"{x}/{maxCost}";
        }, endValue, 0.5f).SetEase(Ease.OutQuad);

        StaticManager.curCost = endValue;

        return true;
    }

    public void UseEventCost(int amount)
    {
        eventCost += amount;
        costText.text = $"{StaticManager.curCost}+{eventCost}/{maxCost}";
    }

    public bool SetUpgradeStone(int amount)
    {
        if (StaticManager.upStone + amount < 0)
            return false;
        else
            return SetUpgradeStoneTween(amount);
    }

    Tween upgradeStoneTween;
    bool SetUpgradeStoneTween(int amount)
    {
        AudioManager.instance.PlaySfx(resourceClipList[2]);

        int startValue = StaticManager.upStone;
        int endValue = StaticManager.upStone + amount;

        // 기존 트윈이 있다면 종료
        if (upgradeStoneTween != null && upgradeStoneTween.IsActive()) upgradeStoneTween.Kill();

        upgradeStoneTween = DOTween.To(() => startValue, x =>
        {
            startValue = x;
            upgradeStoneText.SetText(x.ToString());
        }, endValue, 0.5f).SetEase(Ease.OutQuad);

        StaticManager.upStone = endValue;

        return true;
    }

    [HideInInspector] public Transform unitField;
    [HideInInspector] public Transform enemyField;
    public void SetField()
    {
        unitField = RandomPositionManager.instance.playerFieldList[StaticManager.curStage - 1];
        enemyField = RandomPositionManager.instance.enemyFieldList[StaticManager.curStage - 1];

        UnitDefault.unitField = unitField;
        UnitDefault.enemyField = enemyField;
    }

    [System.NonSerialized] public bool isEnd = true;
    Coroutine timeCo;
    // 게임 시작 버튼에 있음
    public void GameStart()
    {
        // 필드 덱 초기화
        DeckManager.instance.StopAllCoroutines();
        DeckManager.instance.FieldDeckReset();
        StaticManager.isDeckInit = true;
        StaticManager.isMasterKey = false;

        defaultUnitSpawnBtn.interactable = false;
        SBTN.interactable = false;

        UnitDefault.friends.Clear();
        UnitDefault.enemies.Clear();

        DealMetrics.instance.ResetDealMetrics();

        UnitDefault[] unitComps = unitField.GetComponentsInChildren<UnitDefault>();
        foreach (UnitDefault comp in unitComps) {
            if (comp.unitIdx != 48)
                StaticManager.totalScore[comp.unitIdx].cnt += (int)comp.rank + 1;
            comp.enabled = true;

            AnalyticsInitializer.instance.FieldUnits(comp.unitName);
        }

        UnitDefault[] enemyComps = enemyField.GetComponentsInChildren<UnitDefault>();
        foreach (UnitDefault comp in enemyComps) {
            comp.enabled = true;
        }

        isEnd = false;

        RandomPositionManager.instance.playerFieldList[StaticManager.curStage - 1].GetComponent<Image>().enabled = false;

        timeCo = StartCoroutine(TimeLimit());

        speedBtnObj.SetActive(true);
        Time.timeScale = timeScale;
    }

    [Header("제한 시간")]
    public GameObject timeLimitPanel;
    public TextMeshProUGUI timeLimitTxt;
    float duration;
    int sec;

    public TextMeshProUGUI playerScoreTxt;
    public TextMeshProUGUI enemyScoreTxt;
    [HideInInspector] public int playerScore;
    [HideInInspector] public int enemyScore;

    public GameObject countdownBg;
    public TextMeshProUGUI countdownTxt;
    public CanvasGroup countdownCanGroup;

    IEnumerator TimeLimit()
    {
        while (sec > 0) {
            duration -= Time.deltaTime;

            if ((int)duration != sec) {
                sec = (int)duration;
                timeLimitTxt.SetText(sec.ToString());

                if (sec <= 3) {
                    countdownTxt.SetText(sec.ToString());
                    countdownBg.SetActive(true);

                    StartCoroutine(FadeOutCountdown());
                }
            }
            yield return null;
        }
        countdownBg.SetActive(false);

        while (playerScore == enemyScore) {
            yield return null;
        }

        End(playerScore > enemyScore);
    }

    IEnumerator FadeOutCountdown()
    {
        countdownCanGroup.alpha = 1f;

        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime;
            countdownCanGroup.alpha = 1f - t;
            yield return null;
        }

        countdownCanGroup.alpha = 0f;
    }

    public void SetScore(int amount, bool isPlayerScore)
    {
        int score;
        TextMeshProUGUI text;
        if (isPlayerScore) {
            score = playerScore += amount;
            text = playerScoreTxt;
        }
        else {
            score = enemyScore += amount;
            text = enemyScoreTxt;
        }
        text.SetText(score.ToString());
    }

    public void ClearScore()
    {
        int timeLimit = 0;
        switch (StaticManager.rewardType) {
            case RewardType.monster:
                timeLimit = 40;
                break;
            case RewardType.eliteMonster:
                timeLimit = 50;
                break;
            case RewardType.bossMonster:
                timeLimit = 80;
                break;
        }

        timeLimitTxt.SetText(timeLimit.ToString());
        duration = timeLimit + 1f;
        sec = timeLimit;

        playerScoreTxt.SetText("0");
        enemyScoreTxt.SetText("0");
        playerScore = 0;
        enemyScore = 0;

        timeLimitPanel.SetActive(true);
    }

    [Header("전투 종료 사운드 / UI")]
    public AudioClip winClip;
    public AudioClip loseClip;
    public GameObject resultPanel;
    public Image resultImage;
    public Sprite[] resultSprites;

    [Header("결과창")]
    public GameObject totalPanel;
    // 끝나면
    public void End(bool isWin)
    {
        StopCoroutine(timeCo);

        foreach (Transform remainingEffect in skillEffect)
            Destroy(remainingEffect.gameObject);

        foreach (Transform enemy in enemyField.transform) {
            Destroy(enemy.gameObject);
        }

        foreach (Transform unit in unitField.transform) {
            Destroy(unit.gameObject);
        }

        foreach (Transform dealMetrix in DealMetrics.instance.transform) {
            dealMetrix.GetComponent<Button>().interactable = false;
        }

        isEnd = true;

        if (!isWin)
        {
            resultImage.sprite = resultSprites[0];

            AudioManager.instance.PlaySfx(loseClip);

            StaticManager.isWin = false;

            SetPlayerHP(--StaticManager.curHP);
            StaticManager.curCost = maxCost;
            costText.text = StaticManager.curCost.ToString() + "/" + maxCost.ToString();
            StaticManager.allDeckList.AddRange(RemainingDeck.instance.useEventCardList);
            SetGold(RemainingDeck.instance.useDestroyCardGold);
            DeckManager.instance.DeckSort();

            RemainingDeck.instance.useEventCardList.Clear();
            RemainingDeck.instance.useDestroyCardGold = 0;

            cardReward.gameObject.SetActive(true);
            resourceReward.SetActive(true);

            if (StaticManager.curHP >= 0)
                AnalyticsInitializer.instance.DefeatGame();
        }
        else
        {
            RelicsManager.instance.ResetRelics();

            resultImage.sprite = resultSprites[1];

            AudioManager.instance.PlaySfx(winClip);

            StaticManager.isWin = true;

            maxCost = StaticManager.curCost;
            costText.text = $"{StaticManager.curCost}/{maxCost}";

            cardReward.gameObject.SetActive(true);
            RewardType rewardType = StaticManager.rewardType;
            if (rewardType != RewardType.monster)
                relicsReward.SetActive(true);
            resourceReward.SetActive(true);

            if (currentLevel == MapGrid.instance.maxLevel)
            {
                AnalyticsInitializer.instance.BossClear();

                switch (StaticManager.curStage) {
                    case 1:
                        SteamAchievement.Unlock("탐욕을 넘어서");
                        break;
                    case 2:
                        SteamAchievement.Unlock("질투를 잠재우다");
                        break;
                    case 3:
                        SteamAchievement.Unlock("분노를 제압하다");
                        break;
                    case 4:
                        SteamAchievement.Unlock("삼켜진 식탐");
                        break;
                    case 5:
                        SteamAchievement.Unlock("색욕을 거두다");
                        break;
                    case 6:
                        SteamAchievement.Unlock("나태의 끝");
                        break;
                    case 7:
                        SteamAchievement.Unlock("교만을 꺾다");
                        break;
                }

                if (StaticManager.curStage == 7) {
                    // 승리 결과 창
                    totalPanel.SetActive(true);

                    if (StaticManager.curHP == maxHP)
                        SteamAchievement.Unlock("완전무결");

                    SaveController.instance.OnSaveData();
                    SaveManager.DeleteAllSaves();
                    SaveController.instance.GetSaveData();
                }
                else {
                    currentLevel = 0;
                    StaticManager.curStage++;

                    StaticManager.mapSeed = Random.Range(int.MinValue, int.MaxValue);
                    MapGrid.instance.NextStage();
                    SetField();
                }
            }

            if (enemyScore == 0)
                SteamAchievement.Unlock("무사 귀환");
            if (playerScore - enemyScore == 1)
                SteamAchievement.Unlock("한 끗 차이");
            if (playerScore == enemyScore * 2)
                SteamAchievement.Unlock("압도적 승리");
        }
        RemainingDeck.instance.useEventCardList.Clear();
        RemainingDeck.instance.useDestroyCardGold = 0;
        StaticManager.itemStat.Clear();

        RandomPositionManager.instance.playerFieldList[StaticManager.curStage - 1].GetComponent<Image>().enabled = true;

        speedBtnObj.SetActive(false);
        Time.timeScale = 1f;

        StaticManager.legendaryUnitCnt = 0;

        resultPanel.SetActive(true);
    }

    public void SetPlayerHP(int currentHP)
    {
        if (currentHP > maxHP) {
            StaticManager.curHP = maxHP;
            return;
        }
        else if (currentHP < 0) {
            // 패배 결과 창
            totalPanel.SetActive(true);

            SteamAchievement.Unlock("첫 좌절");

            SaveController.instance.OnSaveData();
            SaveManager.DeleteAllSaves();
            SaveController.instance.GetSaveData();

            return;
        }

        for (int i = 0; i < currentHP; i++) {
            HPImage[i].sprite = fullHPImg;
        }
        for (int i = currentHP; i < maxHP; i++) {
            HPImage[i].sprite = emptyHPImg;
        }
    }

    public void DefaultUnitSpawn()
    {
        if (!SetCost(-defaultUnitPrefab.GetComponent<DefaultUnit>().cost))
            return;

        Vector2 spawnPos = RandomPositionManager.instance.GetRandomPlayerFieldPos();
        GameObject defaultUnit = Instantiate(defaultUnitPrefab, unitField.transform);
        defaultUnit.transform.localPosition = spawnPos;

        AudioManager.instance.PlaySfx(defaultUnitSpawnClip);

        GameObject effect = Instantiate(effectObj, defaultUnit.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(defaultUnit.GetComponent<DefaultUnit>().spawnAnimClip);

        SBTN.interactable = true;
    }

    public void UpdateStageUI(int currentLevel)
    {
        if (currentLevel == 1)
            return;

        foreach (var obj in MapGrid.instance.mapList[currentLevel - 2].list) {
            obj.GetComponent<RectTransform>().transform.localScale = new Vector2(1f, 1f);
            obj.GetComponent<Button>().interactable = false;
        }

        foreach (var obj in toButton) {
            obj.GetComponent<RectTransform>().transform.localScale = new Vector2(1.5f, 1.5f);
            obj.GetComponent<Button>().interactable = true;
        }
    }

    //------------------------------------------------------------------------------------------
    [Header("데미지UI")]
    public Transform damageCan;
    public Text damageTextPrefab;
    // 데미지 텍스트
    readonly float displayDuration = 0.6f; // 텍스트가 표시되는 시간
    public void ShowDamage(float damage, Transform unit, Color color)
    {
        Vector3 posi = new(unit.position.x + UnityEngine.Random.Range(-0.4f, 0.4f), unit.position.y + 0.3f, 0);
        // 데미지 텍스트 인스턴스 생성
        Text damageText = Instantiate(damageTextPrefab, posi, Quaternion.identity, damageCan);
        damageText.text = damage.ToString("F1");
        damageText.color = color;

        // 애니메이션 시작
        StartCoroutine(AnimateDamageText(damageText));
    }
    IEnumerator AnimateDamageText(Text damageText)
    {
        float elapsedTime = 0f;
        float Rand = Random.Range(1, 2);
        float Rands = Random.Range(-0.4f, 0.4f);
        Vector3 startPosition = damageText.transform.position;

        // 자연스럽게 올라가는 애니메이션
        while (elapsedTime < displayDuration)
        {
            elapsedTime += Time.deltaTime;

            damageText.transform.position = startPosition + new Vector3(elapsedTime * Rands, elapsedTime * Rand, 0f); // 위로 이동
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1.5f - (elapsedTime / displayDuration)); // 투명도 감소
            yield return null; // 다음 프레임까지 대기
        }

        Destroy(damageText.gameObject); // 텍스트 삭제
    }

    [Header("유닛이 사용할 프리팹")]
    public GameObject effectObj;
    public GameObject UIEffectObj;
    public GameObject bulletObj;
    public GameObject stateImgPrefab;
    public List<Sprite> stateSprites;
    public List<Sprite> potionStateSprites;
    public GameObject luciferState;
    public Transform skillEffect;
    public AudioClip moveClip;

    public void SetPotionStateSpr()
    {
        stateSprites[(int)UnitDefault.State.HPPotion] = potionStateSprites[0 + StaticManager.potionLevel[0]];
        stateSprites[(int)UnitDefault.State.ATKPotion] = potionStateSprites[3 + StaticManager.potionLevel[1]];
        stateSprites[(int)UnitDefault.State.DEFPotion] = potionStateSprites[6 + StaticManager.potionLevel[2]];
        stateSprites[(int)UnitDefault.State.DPSPotion] = potionStateSprites[9 + StaticManager.potionLevel[3]];
        stateSprites[(int)UnitDefault.State.LSPotion] = potionStateSprites[12 + StaticManager.potionLevel[4]];
        stateSprites[(int)UnitDefault.State.CRTPotion] = potionStateSprites[15 + StaticManager.potionLevel[5]];
    }

    [Header("스탯 UI")]
    public GameObject unitInfo;
    public List<TextMeshProUGUI> statTxt;
    public List<TextMeshProUGUI> statAmountTxt;
    public Image curHealthImg;
    public Image curManaImg;
    public ScrollRect descScrollRect;

    public GameObject upStatPanel;
    public GameObject upUnitInfo;
    public List<TextMeshProUGUI> upStatTxt;
    public List<TextMeshProUGUI> upStatAmountTxt;
    public ScrollRect upDescScrollRect;

    public GameObject itemInfo;
    public List<TextMeshProUGUI> itemTxt;
    public GameObject upItemInfo;
    public List<TextMeshProUGUI> upItemTxt;

    public GameObject eventInfo;
    public List<TextMeshProUGUI> eventTxt;

    public GameObject relicsInfo;
    public List<TextMeshProUGUI> relicsTxt;
    [Header("유물")]
    public List<GameObject> relicsList;
    public List<RelicsProb> relicsProbs;
    public List<RelicsProb> enemyRelicsProbs;
    public List<Sprite> relicsSprList;

    [Header("배속 버튼")]
    public GameObject speedBtnObj;
    public TextMeshProUGUI speedBtnTxt;
    [HideInInspector] public float timeScale = 1f;
    public void TimeScaleBtn()
    {
        switch (Time.timeScale) {
            case 1f:
                Time.timeScale = 1.5f;
                speedBtnTxt.SetText("X1.5");
                break;
            case 1.5f:
                Time.timeScale = 2f;
                speedBtnTxt.SetText("X2");
                break;
            default:
                Time.timeScale = 1f;
                speedBtnTxt.SetText("X1");
                break;
        }

        timeScale = Time.timeScale;
    }

    public void ExitBtnClick()
    {
        SceneManager.LoadScene("1.Lobby");
    }

    [Header("Achievement")]
    public GameObject trashCan;
    public GameObject HPSlot;
}