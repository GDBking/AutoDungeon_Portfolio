using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public List<Transform> saleSlotList;
    public GameObject shopCheckBtn;
    public Button buyBtn;
    public TextMeshProUGUI buyText;
    public Button exitBtn;

    public Image HPSlotImg;
    public List<Sprite> HPSlotSprite;
    public Button HPSlotBtn;
    public GameObject HPPrefab;
    public TextMeshProUGUI HPPriceTxt;
    public GameObject HPBuyCheckPanel;
    public Button HPBuyBtn;
    public TextMeshProUGUI HPBuyTxt;
    public List<Transform> playerHPUIList;

    public AudioClip getItemClip;
    public AudioClip getHPClip;

    Transform clickItem;
    string clickItemID;
    int clickItemPrice;

    int buyCnt;

    void SetSaleSlotList()
    {
        foreach (Transform slot in saleSlotList) {
            string cardID = CardReward.instance.GetRewardID();

            string CardPath = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out _, out GameObject cardFront);

            GameObject cardFrontObj = Instantiate(cardFront, slot.position, Quaternion.identity, slot);
            GameObject contents = Instantiate(Resources.Load<GameObject>(CardPath), cardFrontObj.transform);

            TextMeshProUGUI priceText = slot.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (contents.TryGetComponent<UnitDefault>(out UnitDefault unitComp)) {
                priceText.SetText(unitComp.unitMoney.ToString());
            }
            else if (contents.TryGetComponent<Item>(out Item itemComp)) {
                priceText.SetText(itemComp.itemMoney.ToString());
            }
            else if (contents.TryGetComponent<EventItem>(out EventItem eventItemComp)) {
                priceText.SetText(eventItemComp.money.ToString());
            }

            Button btn = contents.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                BtnClick(btn.transform);
            });
        }

        HPPriceTxt.SetText($"{StaticManager.HPPrice}G");
    }

    public void BtnClick(Transform item)
    {
        clickItem = item.parent.transform;
        if (item.TryGetComponent<UnitDefault>(out UnitDefault unitComp)) {
            clickItemID = unitComp.unitID;
            clickItemPrice = unitComp.unitMoney;

            string name = GetLocalizedUnitName(unitComp);
            buyText.SetText(GetPurchaseTemplate(clickItemPrice, name));
        }
        else if (item.TryGetComponent<Item>(out Item itemComp)) {
            clickItemID = itemComp.data.itemID;
            clickItemPrice = itemComp.itemMoney;

            string name = GetLocalizedItemName(itemComp);
            buyText.SetText(GetPurchaseTemplate(clickItemPrice, name));
        }
        else if (item.TryGetComponent<EventItem>(out EventItem eventItemComp)) {
            clickItemID = eventItemComp.ID;
            clickItemPrice = eventItemComp.money;

            string name = GetLocalizedEventName(eventItemComp);
            buyText.SetText(GetPurchaseTemplate(clickItemPrice, name));
        }

        shopCheckBtn.SetActive(true);
        buyBtn.interactable = StaticManager.gold >= clickItemPrice;
    }

    public void BuyBtnClick()
    {
        StaticManager.shopDOTWeenCnt++;
        exitBtn.interactable = false;

        shopCheckBtn.SetActive(false);
        GameManager.instance.SetGold(-clickItemPrice);

        clickItem.GetChild(0).GetComponent<Button>().enabled = false;
        
        string captureID = clickItemID;
        Transform captureItem = clickItem;
        captureItem.SetParent(DeckManager.instance.deckObj.transform);

        captureItem.DOMove(DeckManager.instance.deckObj.transform.position, DeckManager.instance.cardAnimList[0].GetComponent<AnimationToImage>().GetAnimationLength())
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                AudioManager.instance.PlaySfx(getItemClip);

                OnMoveEnd(captureID);
                Destroy(captureItem.gameObject);
            });

        if (++buyCnt == 9) {
            SteamAchievement.Unlock("싹쓸이");
        }
    }

    void OnMoveEnd(string captureID)
    {
        StaticManager.allDeckList.Add(captureID);
        DeckManager.instance.DeckSort();

        if (--StaticManager.shopDOTWeenCnt == 0)
            exitBtn.interactable = true;
    }

    public void HPBtnClick()
    {
        if (StaticManager.curHP == GameManager.instance.maxHP) {
            HPBuyTxt.SetText(GetLocalizedMaxHPMessage());
            HPBuyBtn.interactable = false;
        }
        else {
            HPBuyTxt.SetText(GetPurchaseHPTemplate(StaticManager.HPPrice));
            HPBuyBtn.interactable = StaticManager.HPPrice <= StaticManager.gold;
        }
        HPBuyCheckPanel.SetActive(true);
    }

    public void HPBuyBtnClick()
    {
        if (StaticManager.HPPrice == 2500) {
            SteamAchievement.Unlock("불사의 의지");
        }

        StaticManager.shopDOTWeenCnt++;
        exitBtn.interactable = false;

        HPBuyCheckPanel.SetActive(false);
        GameManager.instance.SetGold(-StaticManager.HPPrice);

        HPSlotBtn.enabled = false;
        HPSlotImg.sprite = HPSlotSprite[1];

        GameObject HPObj = Instantiate(HPPrefab, HPSlotImg.transform);
        StaticManager.HPPrice += 500;

        HPObj.transform.SetParent(playerHPUIList[^1]);
        HPObj.transform.DOMove(playerHPUIList[StaticManager.curHP].position, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                AudioManager.instance.PlaySfx(getHPClip);

                Destroy(HPObj);
                GameManager.instance.SetPlayerHP(++StaticManager.curHP);

                if (--StaticManager.shopDOTWeenCnt == 0)
                    exitBtn.interactable = true;
            });
    }

    void SlotRemove()
    {
        foreach (Transform slot in saleSlotList) {
            if (slot.childCount == 4) {
                Destroy(slot.GetChild(3).gameObject);
            }
        }
    }

    public void ExitBtnClick()
    {
        gameObject.SetActive(false);
        StaticManager.isShop = false;
        SlotRemove();

        HPSlotImg.sprite = HPSlotSprite[0];
        HPSlotBtn.enabled = true;

        buyCnt = 0;

        GameManager.instance.UpdateStageUI(++GameManager.instance.currentLevel);
        GameManager.instance.map.SetActive(true);
    }

    private void OnEnable()
    {
        StaticManager.isShop = true;
        SetSaleSlotList();
    }

    private string GetPurchaseTemplate(int price, string highlightedName)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;

        switch (idx)
        {
            case 0:
                return $"{highlightedName}을(를) <color=#FBF236>{price}</color>G로 구매하시겠습니까?";
            case 1:
                return $"Do you want to purchase {highlightedName} for <color=#FBF236>{price}</color>G?";
            case 2:
                return $"Voulez-vous acheter {highlightedName} pour <color=#FBF236>{price}</color>G ?";
            case 3:
                return $"Vuoi acquistare {highlightedName} per <color=#FBF236>{price}</color>G?";
            case 4:
                return $"Möchten Sie {highlightedName} für <color=#FBF236>{price}</color>G kaufen?";
            case 5:
                return $"¿Desea comprar {highlightedName} por <color=#FBF236>{price}</color>G?";
            case 6:
                return $"{highlightedName}を<color=#FBF236>{price}</color>Gで購入しますか？";
            case 7:
                return $"Deseja comprar {highlightedName} por <color=#FBF236>{price}</color>G?";
            case 8:
                return $"Вы хотите приобрести {highlightedName} за <color=#FBF236>{price}</color>G?";
            case 9:
                return $"是否以 <color=#FBF236>{price}</color>G 购买 {highlightedName}？";
            default:
                return $"{highlightedName}을(를) <color=#FBF236>{price}</color>G로 구매하시겠습니까?";
        }
    }

    private string GetPurchaseHPTemplate(int price)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;

        switch (idx)
        {
            case 0:
                return $"플레이어의 생명을 <color=#FBF236>{price}</color>G로 구매하시겠습니까?";
            case 1:
                return $"Do you want to purchase one player HP for <color=#FBF236>{price}</color>G?";
            case 2:
                return $"Souhaitez-vous acheter une vie pour <color=#FBF236>{price}</color>G ?";
            case 3:
                return $"Vuoi acquistare una vita per <color=#FBF236>{price}</color>G?";
            case 4:
                return $"Möchten Sie ein Leben für <color=#FBF236>{price}</color>G kaufen?";
            case 5:
                return $"¿Desea comprar una vida por <color=#FBF236>{price}</color>G?";
            case 6:
                return $"<color=#FBF236>{price}</color>Gでプレイヤーの体力を1つ購入しますか？";
            case 7:
                return $"Deseja comprar uma vida por <color=#FBF236>{price}</color>G?";
            case 8:
                return $"Хотите ли вы приобрести одну жизнь за <color=#FBF236>{price}</color>G?";
            case 9:
                return $"是否以 <color=#FBF236>{price}</color>G 购买一条生命？";
            default:
                return $"Do you want to purchase one player HP for <color=#FBF236>{price}</color>G?";
        }
    }

    private string GetLocalizedMaxHPMessage()
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;

        switch (idx)
        {
            case 0: return "플레이어의 생명이 이미 최대입니다.";
            case 1: return "Player's HP is already at maximum.";
            case 2: return "La vie du joueur est déjà au maximum.";
            case 3: return "La vita del giocatore è già al massimo.";
            case 4: return "Die Lebensanzahl des Spielers ist bereits maximal.";
            case 5: return "La vida del jugador ya está al máximo.";
            case 6: return "プレイヤーの体力は既に最大です。";
            case 7: return "A vida do jogador já está no máximo.";
            case 8: return "Жизни игрока уже максимально.";
            case 9: return "玩家的生命值已达到最大。";
            default: return "Player's HP is already at maximum.";
        }
    }

    private string GetLocalizedUnitName(UnitDefault unitComp)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        string name = unitComp.unitName;

        switch (idx)
        {
            case 0: name = !string.IsNullOrEmpty(unitComp.unitName) ? unitComp.unitName : unitComp.ENUnitName; break;
            case 1: name = !string.IsNullOrEmpty(unitComp.ENUnitName) ? unitComp.ENUnitName : unitComp.unitName; break;
            case 2: name = !string.IsNullOrEmpty(unitComp.FRUnitName) ? unitComp.FRUnitName : unitComp.unitName; break;
            case 3: name = !string.IsNullOrEmpty(unitComp.ITUnitName) ? unitComp.ITUnitName : unitComp.unitName; break;
            case 4: name = !string.IsNullOrEmpty(unitComp.DEUnitName) ? unitComp.DEUnitName : unitComp.unitName; break;
            case 5: name = !string.IsNullOrEmpty(unitComp.ESUnitName) ? unitComp.ESUnitName : unitComp.unitName; break;
            case 6: name = !string.IsNullOrEmpty(unitComp.JAUnitName) ? unitComp.JAUnitName : unitComp.unitName; break;
            case 7: name = !string.IsNullOrEmpty(unitComp.PT_BRUnitName) ? unitComp.PT_BRUnitName : unitComp.unitName; break;
            case 8: name = !string.IsNullOrEmpty(unitComp.RUUnitName) ? unitComp.RUUnitName : unitComp.unitName; break;
            case 9: name = !string.IsNullOrEmpty(unitComp.ZH_HANSUnitName) ? unitComp.ZH_HANSUnitName : unitComp.unitName; break;
            default: name = unitComp.ENUnitName; break;
        }

        return $"<b>‘{name}’</b>";
    }

    private string GetLocalizedItemName(Item itemComp)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        var d = itemComp.data;
        string name = d.itemName;

        switch (idx)
        {
            case 0: name = !string.IsNullOrEmpty(d.itemName) ? d.itemName : d.ENItemName; break;
            case 1: name = !string.IsNullOrEmpty(d.ENItemName) ? d.ENItemName : d.itemName; break;
            case 2: name = !string.IsNullOrEmpty(d.FRItemName) ? d.FRItemName : d.itemName; break;
            case 3: name = !string.IsNullOrEmpty(d.ITItemName) ? d.ITItemName : d.itemName; break;
            case 4: name = !string.IsNullOrEmpty(d.DEItemName) ? d.DEItemName : d.itemName; break;
            case 5: name = !string.IsNullOrEmpty(d.ESItemName) ? d.ESItemName : d.itemName; break;
            case 6: name = !string.IsNullOrEmpty(d.JAItemName) ? d.JAItemName : d.itemName; break;
            case 7: name = !string.IsNullOrEmpty(d.PT_BRItemName) ? d.PT_BRItemName : d.itemName; break;
            case 8: name = !string.IsNullOrEmpty(d.RUItemName) ? d.RUItemName : d.itemName; break;
            case 9: name = !string.IsNullOrEmpty(d.ZH_HANSItemName) ? d.ZH_HANSItemName : d.itemName; break;
            default: name = d.ENItemName; break;
        }

        return $"<b>‘{name}’</b>";
    }

    private string GetLocalizedEventName(EventItem eventItemComp)
    {
        int idx = Localization.instance != null ? Localization.instance.index : 0;
        string name = eventItemComp.itemName;

        switch (idx)
        {
            case 0: name = !string.IsNullOrEmpty(eventItemComp.itemName) ? eventItemComp.itemName : eventItemComp.ENItemName; break;
            case 1: name = !string.IsNullOrEmpty(eventItemComp.ENItemName) ? eventItemComp.ENItemName : eventItemComp.itemName; break;
            case 2: name = !string.IsNullOrEmpty(eventItemComp.FRItemName) ? eventItemComp.FRItemName : eventItemComp.itemName; break;
            case 3: name = !string.IsNullOrEmpty(eventItemComp.ITItemName) ? eventItemComp.ITItemName : eventItemComp.itemName; break;
            case 4: name = !string.IsNullOrEmpty(eventItemComp.DEItemName) ? eventItemComp.DEItemName : eventItemComp.itemName; break;
            case 5: name = !string.IsNullOrEmpty(eventItemComp.ESItemName) ? eventItemComp.ESItemName : eventItemComp.itemName; break;
            case 6: name = !string.IsNullOrEmpty(eventItemComp.JAItemName) ? eventItemComp.JAItemName : eventItemComp.itemName; break;
            case 7: name = !string.IsNullOrEmpty(eventItemComp.PT_BRItemName) ? eventItemComp.PT_BRItemName : eventItemComp.itemName; break;
            case 8: name = !string.IsNullOrEmpty(eventItemComp.RUItemName) ? eventItemComp.RUItemName : eventItemComp.itemName; break;
            case 9: name = !string.IsNullOrEmpty(eventItemComp.ZH_HANSItemName) ? eventItemComp.ZH_HANSItemName : eventItemComp.itemName; break;
            default: name = eventItemComp.ENItemName; break;
        }

        return $"<b>'‘{name}’</b>";
    }
}