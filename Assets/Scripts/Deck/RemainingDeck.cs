using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoldingCardInfo
{
    public int idx;
    public int duplicate;
    public int allDuplicate;
}

public class RemainingDeck : MonoBehaviour
{
    public static RemainingDeck instance; // GameManager에서 할당
    public Transform emptyCardSlot;

    public List<string> allCardList;
    public TextMeshProUGUI remainingText;
    public Transform remainingDeckBg;
    public int cardsPerPage;
    public Button nextBtn;
    public Button previousBtn;
    public TextMeshProUGUI pageInfoText;
    [HideInInspector] public List<string> useEventCardList = new();
    [HideInInspector] public int useDestroyCardGold;

    public GameObject upgradeSellCheckUI;
    public TextMeshProUGUI upgradeSellTxt;
    public Button upgradeBtn;
    UnitDefault clickUnitComp;
    Item clickItemComp;
    EventItem clickEventItemComp;
    string upgradeSellID;
    string[] clickDuplicateCount;
    int upgradePrice;
    string clickType;
    string clickName;

    public GameObject masterKeyPanel;
    public TextMeshProUGUI masterKeyTxt;
    public Button masterKeyBtn;
    public Transform tempItemP;

    public Slider slider;
    public TMP_InputField inputField;

    int curPage;
    readonly List<HoldingCardInfo> holdingCardInfo = new();

    [Header("아이템 업그레이드 연출")]
    public AudioClip upgradeSound;
    public AnimationClip upgradeAnim;

    // 모든 덱 오브젝트 풀링(게임 매니저에서 호출)
    public void Init()
    {
        // 모든 카드 내림차순으로 정렬
        allCardList.Sort((a, b) => b.CompareTo(a));

        // 모든 카드를 생성 후 비활성화(오브젝트 풀링)
        foreach (string cardID in allCardList) {
            string pathID = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out _, out GameObject cardFront);
            GameObject cardFrontObj = Instantiate(cardFront, remainingDeckBg);

            GameObject contents = Instantiate(Resources.Load<GameObject>(pathID), cardFrontObj.transform);

            GameObject duplicateTxt = Instantiate(remainingText.gameObject, cardFrontObj.transform);

            Button btn = contents.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                upgradeSellID = cardID;
                clickDuplicateCount = duplicateTxt.GetComponent<TextMeshProUGUI>().text.Split('/');

                if (contents.TryGetComponent<UnitDefault>(out clickUnitComp)) {
                    upgradePrice = clickUnitComp.unitMoney;
                    int sellPrice = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
                    clickType = "unit";
                    clickName = clickUnitComp.GetLocalizedUnitName();

                    if (!StaticManager.isShop)
                        return;

                    /*upgradeSellTxt.SetText($"{clickName}\n" +
                                           $"업그레이드 : {(clickUnitComp.Level < StaticManager.maxLevel[clickUnitComp.unitIdx] ? $"강화석 {clickUnitComp.Level + 1}개" : "MaxLevel")}\n" +
                                           $"판매 : {upgradePrice / 2 * slider.value}G");*/

                    upgradeSellTxt.SetText(
                        GetUpgradeSellText_10langs_ForUnit(clickName,
                        clickUnitComp.Level < StaticManager.maxLevel[clickUnitComp.unitIdx],
                        clickUnitComp.Level + 1,
                        sellPrice)
                    );

                    slider.maxValue = int.Parse(clickDuplicateCount[1]);
                    slider.value = 1;

                    upgradeBtn.interactable = (StaticManager.upStone >= clickUnitComp.Level + 1) && (clickUnitComp.Level < StaticManager.maxLevel[clickUnitComp.unitIdx]);
                    upgradeSellCheckUI.SetActive(true);
                }
                else if (contents.TryGetComponent<Item>(out clickItemComp)) {
                    upgradePrice = clickItemComp.itemMoney;
                    int sellPrice = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
                    clickType = "item";
                    clickName = clickItemComp.data.GetLocalizedName();

                    if (!StaticManager.isShop)
                        return;

                    /*upgradeSellTxt.SetText($"{clickName}\n" +
                                           $"업그레이드 : {(clickItemComp.data.itemLevel < 2 ? $"강화석 {(clickItemComp.data.itemLevel + 1) * 2}개" : "MaxLevel")}\n" +
                                           $"판매 : {upgradePrice / 2 * slider.value}G");*/

                    upgradeSellTxt.SetText(
                        GetUpgradeSellText_10langs_ForItem(clickName,
                        (clickItemComp.data.itemLevel == 0 && SaveController.instance.saveDataAchi.isCheck[1]) || (clickItemComp.data.itemLevel <= 1 && SaveController.instance.saveDataAchi.isCheck[2]),
                        (clickItemComp.data.itemLevel + 1) * 2,
                        sellPrice)
                    );

                    slider.maxValue = int.Parse(clickDuplicateCount[1]);
                    slider.value = 1;

                    upgradeBtn.interactable = StaticManager.upStone >= (clickItemComp.data.itemLevel + 1) * 2 && ((clickItemComp.data.itemLevel == 0 && SaveController.instance.saveDataAchi.isCheck[1]) || (clickItemComp.data.itemLevel <= 1 && SaveController.instance.saveDataAchi.isCheck[2]));
                    upgradeSellCheckUI.SetActive(true);
                }
                else if (contents.TryGetComponent<EventItem>(out clickEventItemComp)){
                    upgradePrice = clickEventItemComp.money;
                    int sellPrice = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
                    if (clickEventItemComp.type == EventItem.EventItemType.gold)
                        upgradePrice *= int.Parse(clickDuplicateCount[1]);
                    clickType = "eventItem";
                    clickName = clickEventItemComp.GetLocalizedName();

                    if (!StaticManager.isShop)
                        return;

                    /*upgradeSellTxt.SetText($"{clickName}\n" +
                                           $"업그레이드 : 불가\n" +
                                           $"판매 : {upgradePrice / 2 * slider.value}G");*/

                    upgradeSellTxt.SetText(
                        GetUpgradeSellText_10langs_ForNone(clickName, sellPrice)
                    );

                    slider.maxValue = int.Parse(clickDuplicateCount[1]);
                    slider.value = 1;

                    upgradeBtn.interactable = false;
                    upgradeSellCheckUI.SetActive(true);
                }
            });
            btn.onClick.AddListener(() =>
            {
                if (!StaticManager.isMasterKey)
                    return;

                /*if (clickDuplicateCount[0] == "0") {
                    masterKeyTxt.SetText($"{clickName} 조각은\n남아있지 않습니다.");
                    masterKeyBtn.interactable = false;
                }
                else {
                    masterKeyTxt.SetText($"{clickName} 조각을\n가져오시겠습니까?");
                    masterKeyBtn.interactable = true;
                }*/

                if (clickDuplicateCount[0] == "0")
                {
                    masterKeyTxt.SetText(GetMasterKeyText_10langs(clickName, false));
                    masterKeyBtn.interactable = false;
                }
                else
                {
                    masterKeyTxt.SetText(GetMasterKeyText_10langs(clickName, true));
                    masterKeyBtn.interactable = true;
                }

                masterKeyPanel.SetActive(true);
            });
            btn.onClick.AddListener(() =>
            {
                if (!StaticManager.isPray || clickType != "unit")
                    return;

                Pray.instance.CardClick(clickUnitComp);
            });
            btn.onClick.AddListener(() =>
            {
                if (!StaticManager.isDuplicate)
                    return;

                Duplicate.instance.CardClick(clickName, cardID, upgradePrice, clickUnitComp);
            });
            btn.onClick.AddListener(() =>
            {
                if (!StaticManager.isManaReduce || clickType != "unit")
                    return;

                ManaReduce.instance.CardClick(clickUnitComp);
            });

            cardFrontObj.SetActive(false);
        }
    }

    public void SetRemainingDeck()
    {
        // 활성화되어있는 카드를 모두 비활성화
        foreach (HoldingCardInfo card in holdingCardInfo) {
            remainingDeckBg.GetChild(card.idx).gameObject.SetActive(false);
        }

        // 남은 카드 목록 초기화
        holdingCardInfo.Clear();
        List<string> remainingDeckList = StaticManager.remainingDeckList;
        List<string> allDeckList = StaticManager.allDeckList;

        int i = 0, j = 0, k = 0;
        // 소유한 모든 카드와 풀링된 모든 카드를 비교
        for (; i < allDeckList.Count; i++) {
            int allDuplicateCount = 1;
            int duplicateCount = 0;
            for (; j < allCardList.Count; j++) {
                // 남은 카드 중에 같은 카드를 발견하면 중복 개수 체크
                if (k < remainingDeckList.Count && remainingDeckList[k] == allCardList[j]) {
                    duplicateCount = 1;
                    while (k != remainingDeckList.Count - 1 && remainingDeckList[k] == remainingDeckList[k + 1]) {
                        duplicateCount++;
                        k++;
                    }
                    k++;
                }
                // 소유한 카드와 같은 카드를 발견하면 중복 개수를 체크
                if (allDeckList[i] == allCardList[j]) {
                    while (i != allDeckList.Count - 1 && allDeckList[i] == allDeckList[i + 1]) {
                        allDuplicateCount++;
                        i++;
                    }

                    // 같은 카드의 풀링 인덱스와 중복 개수를 저장
                    HoldingCardInfo cardInfo = new()
                    {
                        idx = j,
                        duplicate = duplicateCount,
                        allDuplicate = allDuplicateCount
                    };
                    holdingCardInfo.Add(cardInfo);
                    j++;
                    break;
                }
            }
        }

        // 카드가 줄었을 때 현재 페이지에 남은 카드가 없으면 페이지를 1감소
        if (curPage * cardsPerPage > holdingCardInfo.Count - 1) {
            curPage--;
        }
        
        // 현재 페이지에 맞는 카드를 활성화 및 텍스트 최신화
        for (i = curPage * cardsPerPage; i < holdingCardInfo.Count && i < (curPage * cardsPerPage) + cardsPerPage && allDeckList.Count > 0; i++) {
            Transform activeCard = remainingDeckBg.GetChild(holdingCardInfo[i].idx);
            activeCard.GetChild(1).GetComponent<TextMeshProUGUI>().SetText($"{holdingCardInfo[i].duplicate}/{holdingCardInfo[i].allDuplicate}");

            activeCard.gameObject.SetActive(true);

            if (activeCard.GetChild(0).TryGetComponent<UnitDefault>(out UnitDefault unitComp)) {
                unitComp.levelEffect.SetInteger("Level", unitComp.Level);
            }
        }

        // 페이지 버튼 활성화 여부 결정
        nextBtn.gameObject.SetActive((curPage + 1) * cardsPerPage < holdingCardInfo.Count);
        previousBtn.gameObject.SetActive(curPage > 0);

        // 현재 페이지 정보 최신화
        pageInfoText.text = (curPage + 1).ToString() + "/" + ((holdingCardInfo.Count - 1) / cardsPerPage + 1).ToString();
    }

    public void NextBtnClick()
    {
        curPage++;
        SetRemainingDeck();
    }

    public void PreviousBtnClick()
    {
        curPage--;
        SetRemainingDeck();
    }

    public void UpgradeBtnClick()
    {
        upgradeSellCheckUI.SetActive(false);

        if (clickType == "unit") {
            GameManager.instance.SetUpgradeStone(-(clickUnitComp.Level + 1));
            DeckManager.instance.deckObj.GetComponent<Button>().interactable = false;
            UpgradeManager.instance.SpecificUpgrade(clickUnitComp);
        }
        else if (clickType == "item") {
            AudioManager.instance.PlaySfx(upgradeSound);

            GameObject effect = Instantiate(GameManager.instance.UIEffectObj, clickItemComp.transform.position, Quaternion.identity);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(upgradeAnim);
            effect.transform.localScale *= 0.65f;
            effect.transform.localPosition = new Vector2(effect.transform.position.x, 0.95f);

            GameManager.instance.upStatPanel.SetActive(false);

            GameManager.instance.SetUpgradeStone(-(clickItemComp.data.itemLevel + 1) * 2);

            StaticManager.potionLevel[(int)clickItemComp.data.itemType]++;
            GameManager.instance.SetPotionStateSpr();

            for (int i = 0; i < StaticManager.allDeckList.Count; i++) {
                if (StaticManager.allDeckList[i] == upgradeSellID) {
                    StaticManager.allDeckList[i] = upgradeSellID + "1";
                }
            }
            DeckManager.instance.DeckSort();

            for (int i = 0; i < CardReward.instance.rewardList.Count; i++) {
                if (CardReward.instance.rewardList[i].ID == upgradeSellID) {
                    CardReward.instance.rewardList[i].ID = upgradeSellID + "1";
                    break;
                }
            }

            foreach (Transform shopSlot in Shop.instance.saleSlotList) {
                if (shopSlot.childCount >= 4 && shopSlot.GetChild(3).GetChild(0).TryGetComponent<Item>(out Item itemComp)) {
                    if (itemComp.data.itemID == upgradeSellID) {
                        Destroy(itemComp.gameObject);
                        GameObject contents = Instantiate(Resources.Load<GameObject>($"Items/{upgradeSellID}1"), shopSlot.GetChild(3));

                        Button btn = contents.AddComponent<Button>();
                        btn.onClick.AddListener(() =>
                        {
                            Shop.instance.BtnClick(btn.transform);
                        });
                    }
                }
            }

            tempItem.SetItemInfo();

            AnalyticsInitializer.instance.Upgrade(clickItemComp.data.itemName, clickItemComp.data.itemLevel + 1);
        }
    }

    ItemPointerEnter tempItem;
    public void UpgradeBtnEnter()
    {
        if (clickType != "item" || upgradeSellID[^2..] == "11")
            return;

        if (!(clickItemComp.data.itemLevel == 0 && SaveController.instance.saveDataAchi.isCheck[1]) && !(clickItemComp.data.itemLevel <= 1 && SaveController.instance.saveDataAchi.isCheck[2]))
            return;

        if (tempItem != null)
            Destroy(tempItem.gameObject);

        tempItem = Instantiate(Resources.Load<GameObject>($"Items/{upgradeSellID}1"), tempItemP).GetComponent<ItemPointerEnter>();

        tempItem.SetItemInfo(true);
        GameManager.instance.upStatPanel.SetActive(true);
    }

    public void SellBtnClick()
    {
        upgradeSellCheckUI.SetActive(false);

        for (int i = 0; i < slider.value; i++) {
            StaticManager.allDeckList.Remove(upgradeSellID);
        }
        DeckManager.instance.DeckSort();

        GameManager.instance.SetGold(upgradePrice / 2 * (int)slider.value);

        if (upgradeSellID == "event_gold") {
            for (int i = 0; i < slider.value; i++)
                AnalyticsInitializer.instance.UseItem(upgradeSellID);

            if (SteamUserStats.GetAchievement("보석 감정사", out bool 보석_감정사) && !보석_감정사) {
                SteamUserStats.GetStat("보석 감정사1", out int goldCnt);
                SteamUserStats.SetStat("보석 감정사1", goldCnt + slider.value);
                SteamUserStats.StoreStats();

                if (goldCnt + slider.value >= 50) {
                    SaveController.instance.saveDataAchi.totalAchiPoint++;
                    SaveController.instance.saveDataAchi.achiPoint++;
                    SaveController.instance.OnSaveDataAchi();
                }
            }
        }
    }

    public void ChangedSlider()
    {
        inputField.SetTextWithoutNotify(slider.value.ToString());

      /*  if (clickType == "unit") { 
            upgradeSellTxt.SetText($"{clickName}\n" +
                                   $"업그레이드 : {(clickUnitComp.Level < StaticManager.maxLevel[clickUnitComp.unitIdx] ? $"강화석 {clickUnitComp.Level + 1}개" : "MaxLevel")}\n" +
                                   $"판매 : {upgradePrice / 2 * slider.value}G");
        }
        else if (clickType == "item") {
            upgradeSellTxt.SetText($"{clickName}\n" +
                                   $"업그레이드 : {(clickItemComp.data.itemLevel < 2 ? $"강화석 {(clickItemComp.data.itemLevel + 1) * 2}개" : "MaxLevel")}\n" +
                                   $"판매 : {upgradePrice / 2 * slider.value}G");
        }
        else if (clickType == "eventItem") {
            upgradeSellTxt.SetText($"{clickName}\n" +
                                   $"업그레이드 : 불가\n" +
                                   $"판매 : {upgradePrice / 2 * slider.value}G");
        }*/

        if (clickType == "unit")
        {
            int sellP = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
            upgradeSellTxt.SetText(GetUpgradeSellText_10langs_ForUnit(clickName,
                clickUnitComp.Level < StaticManager.maxLevel[clickUnitComp.unitIdx],
                clickUnitComp.Level + 1,
                sellP));
        }
        else if (clickType == "item")
        {
            int sellP = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
            upgradeSellTxt.SetText(GetUpgradeSellText_10langs_ForItem(clickName,
                (clickItemComp.data.itemLevel == 0 && SaveController.instance.saveDataAchi.isCheck[1]) || (clickItemComp.data.itemLevel <= 1 && SaveController.instance.saveDataAchi.isCheck[2]),
                (clickItemComp.data.itemLevel + 1) * 2,
                sellP));
        }
        else if (clickType == "eventItem")
        {
            int sellP = Mathf.RoundToInt(upgradePrice / 2 * slider.value);
            upgradeSellTxt.SetText(GetUpgradeSellText_10langs_ForNone(clickName, sellP));
        }
    }

    public void SetInputField()
    {
        // 입력값 파싱, 예외 처리
        if (!int.TryParse(inputField.text, out int value))
            value = 0;

        // 오버/언더 플로우 제한
        value = Mathf.Clamp(value, 0, (int)slider.maxValue);

        // 인풋필드와 슬라이더 동기화
        inputField.SetTextWithoutNotify(value.ToString());
        slider.value = value;
    }

    public void MaterKeyBtnClick()
    {
        masterKeyPanel.SetActive(false);
        StaticManager.isMasterKey = false;
        DeckManager.instance.MasterKeyCardDraw(emptyCardSlot, upgradeSellID);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        curPage = 0;
        SetRemainingDeck();

        if (!GameManager.instance.isEnd)
            Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        if (!GameManager.instance.isEnd)
            Time.timeScale = GameManager.instance.timeScale;
    }

    private string GetUpgradeSellText_10langs_ForUnit(string highlightedName, bool canUpgrade, int stonesNeeded, int sellPrice)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        string upgradePart = canUpgrade ? GetStonesText(idx, stonesNeeded) : GetMaxLevelText(idx);
        switch (idx)
        {
            case 0:
                return $"{highlightedName}\n업그레이드 : {upgradePart}\n판매 : {sellPrice}G";
            case 1:
                return $"{highlightedName}\nUpgrade: {upgradePart}\nSell: {sellPrice}G";
            case 2:
                return $"{highlightedName}\nAmélioration : {upgradePart}\nVente : {sellPrice}G";
            case 3:
                return $"{highlightedName}\nPotenziare : {upgradePart}\nVendita : {sellPrice}G";
            case 4:
                return $"{highlightedName}\nAufrüstung : {upgradePart}\nVerkauf : {sellPrice}G";
            case 5:
                return $"{highlightedName}\nMejora : {upgradePart}\nVenta : {sellPrice}G";
            case 6: 
                return $"{highlightedName}\nアップグレード : {upgradePart}\n販売 : {sellPrice}G";
            case 7:
                return $"{highlightedName}\nMelhoria : {upgradePart}\nVenda : {sellPrice}G";
            case 8:
                return $"{highlightedName}\nУлучшение : {upgradePart}\nПродажа : {sellPrice}G";
            case 9:
                return $"{highlightedName}\n升级 : {upgradePart}\n出售 : {sellPrice}G";
            default:
                return $"{highlightedName}\nUpgrade: {upgradePart}\nSell: {sellPrice}G";
        }
    }

    private string GetUpgradeSellText_10langs_ForItem(string highlightedName, bool canUpgrade, int stonesNeeded, int sellPrice)
    {
        return GetUpgradeSellText_10langs_ForUnit(highlightedName, canUpgrade, stonesNeeded, sellPrice);
    }

    private string GetUpgradeSellText_10langs_ForNone(string highlightedName, int sellPrice)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        string upgradePart = GetNotAvailableText(idx);
        switch (idx)
        {
            case 0: return $"{highlightedName}\n업그레이드 : {upgradePart}\n판매 : {sellPrice}G";
            case 1: return $"{highlightedName}\nUpgrade: {upgradePart}\nSell: {sellPrice}G";
            case 2: return $"{highlightedName}\nAmélioration : {upgradePart}\nVente : {sellPrice}G";
            case 3: return $"{highlightedName}\nPotenziare : {upgradePart}\nVendita : {sellPrice}G";
            case 4: return $"{highlightedName}\nAufrüstung : {upgradePart}\nVerkauf : {sellPrice}G";
            case 5: return $"{highlightedName}\nMejora : {upgradePart}\nVenta : {sellPrice}G";
            case 6: return $"{highlightedName}\nアップグレード : {upgradePart}\n販売 : {sellPrice}G";
            case 7: return $"{highlightedName}\nMelhoria : {upgradePart}\nVenda : {sellPrice}G";
            case 8: return $"{highlightedName}\nУлучшение : {upgradePart}\nПродажа : {sellPrice}G";
            case 9: return $"{highlightedName}\n升级 : {upgradePart}\n出售 : {sellPrice}G";
            default: return $"{highlightedName}\nUpgrade: {upgradePart}\nSell: {sellPrice}G";
        }
    }

    private string GetMasterKeyText_10langs(string highlightedName, bool available)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        if (!available)
        {
            switch (idx)
            {
                case 0: return $"{highlightedName} 조각은\n남아있지 않습니다.";
                case 1: return $"{highlightedName} piece is\nnot available.";
                case 2: return $"{highlightedName} fragment n'est\nplus disponible.";
                case 3: return $"{highlightedName} frammento\nnon è disponibile.";
                case 4: return $"{highlightedName} Fragment ist\nnicht vorhanden.";
                case 5: return $"{highlightedName} fragmento\nno está disponible.";
                case 6: return $"{highlightedName}の断片は\n残っていません。";
                case 7: return $"{highlightedName} fragmento\nnão está disponível.";
                case 8: return $"{highlightedName} фрагмент\nотсутствует.";
                case 9: return $"{highlightedName} 片段\n已不存在。";
                default: return $"{highlightedName} piece is\nnot available.";
            }
        }
        else
        {
            switch (idx)
            {
                case 0: return $"{highlightedName} 조각을\n가져오시겠습니까?";
                case 1: return $"Do you want to retrieve the\n{highlightedName} piece?";
                case 2: return $"Voulez-vous récupérer\nle fragment {highlightedName} ?";
                case 3: return $"Vuoi recuperare\nil frammento {highlightedName}?";
                case 4: return $"Möchten Sie das Fragment\n{highlightedName} holen?";
                case 5: return $"¿Desea recuperar\nel fragmento {highlightedName}?";
                case 6: return $"{highlightedName} の断片を\n回収しますか？";
                case 7: return $"Deseja recuperar\no fragmento {highlightedName}?";
                case 8: return $"Вы хотите забрать\nфрагмент {highlightedName}?";
                case 9: return $"要取回\n{highlightedName} 片段吗？";
                default: return $"Do you want to retrieve the\n{highlightedName} piece?";
            }
        }
    }

    private string GetStonesText(int langIdx, int count)
    {
        switch (langIdx)
        {
            case 0: return $"강화석 {count}개";
            case 1: return $"Upgrade stones {count}";
            case 2: return $"Pierres d'amélioration {count}";
            case 3: return $"Pietre di potenziamento {count}";
            case 4: return $"Aufwertungssteine {count}";
            case 5: return $"Piedras de mejora {count}";
            case 6: return $"強化石 {count}個";
            case 7: return $"Pedras de melhoria {count}";
            case 8: return $"Камни улучшения {count}";
            case 9: return $"强化石 {count}个";
            default: return $"Upgrade stones {count}개";
        }
    }

    // MaxLevel 텍스트
    private string GetMaxLevelText(int langIdx)
    {
        switch (langIdx)
        {
            case 0: return "Max Level";
            case 1: return "Max Level";
            case 2: return "Niveau max";
            case 3: return "Livello massimo";
            case 4: return "Maximales Level";
            case 5: return "Nivel máximo";
            case 6: return "最大レベル";
            case 7: return "Nível máximo";
            case 8: return "Максимальный уровень";
            case 9: return "最高等级";
            default: return "Max Level";
        }
    }

    // 업그레이드 불가 텍스트
    private string GetNotAvailableText(int langIdx)
    {
        switch (langIdx)
        {
            case 0: return "불가";
            case 1: return "Not available";
            case 2: return "Non disponible";
            case 3: return "Non disponibile";
            case 4: return "Nicht verfügbar";
            case 5: return "No disponible";
            case 6: return "不可";
            case 7: return "Não disponível";
            case 8: return "Недоступно";
            case 9: return "不可";
            default: return "Not available";
        }
    }
}