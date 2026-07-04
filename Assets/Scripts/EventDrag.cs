using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EventItem;

public class EventDrag : Drag
{
    EventItem eventItemComp;
    EventItemType type;

    public Material eventMaterial;
    public Material cardMaterial;

    public float minValue;
    public float maxValue;
    public float getValue;

    GameObject cardFrontObj;

    Coroutine eventCoroutine;
    static bool isRunTime = false;

    public AudioClip eventUseClip;

    protected override void Awake()
    {
        base.Awake();

        eventItemComp = GetComponent<EventItem>();
        type = eventItemComp.type;
    }

    private void Start()
    {
        getValue = maxValue;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (type != EventItemType.steroid && type != EventItemType.gold)
            DeckManager.instance.useEventCardPanel.SetActive(true);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (type != EventItemType.steroid && type != EventItemType.gold)
            DeckManager.instance.useEventCardPanel.SetActive(false);

        GameObject dropObj = eventData.pointerEnter;

        if (dropObj == null) {
            InitPos();
            return;
        }

        if (dropObj.CompareTag("Use Event")) {
            switch (type) {
                case EventItemType.cost:
                    AudioManager.instance.PlaySfx(eventUseClip);
                    GameManager.instance.UseEventCost(Random.Range(10, 16));
                    UseEventCard();
                    break;
                case EventItemType.reroll:
                    Reroll();
                    break;
                case EventItemType.mercenary:
                    SpawnMercenary();
                    break;
                case EventItemType.sacrifice:
                    Sacrifice();
                    break;
                case EventItemType.masterKey:
                    MasterKey();
                    break;
            }

            AnalyticsInitializer.instance.UseItem(eventItemComp.ID);
        }
        // 아이템 부스터를 장비를 장착한 아군 유닛에게 사용했을 시
        else if (type == EventItemType.steroid && dropObj.TryGetComponent<UnitDefault>(out UnitDefault unitComp) && unitComp.isEquipmentUnit) {
            ItemBooster(unitComp);
            AnalyticsInitializer.instance.UseItem(eventItemComp.ID);
        }
        // 쓰레기통에 버렸을 때
        else if (dropObj.CompareTag("Trash Can") && type != EventItemType.gold) {
            if (!GameManager.instance.SetGold(-200)) {
                InitPos();
                return;
            }

            RemainingDeck.instance.useDestroyCardGold += 200;

            DeckManager.instance.CardDraw(beginParent.parent);
            Destroy(beginParent.gameObject);
            Destroy(gameObject);
        }
        else {
            InitPos();
        }
    }

    void UseEventCard()
    {
        RemainingDeck.instance.useEventCardList.Add(eventItemComp.ID);
        StaticManager.allDeckList.Remove(eventItemComp.ID);

        EventUse();
        DeckManager.instance.CardDraw(beginParent.parent);
        Destroy(beginParent.gameObject);
    }

    void Reroll()
    {
        AudioManager.instance.PlaySfx(eventUseClip);

        RemainingDeck.instance.useEventCardList.Add(eventItemComp.ID);
        StaticManager.allDeckList.Remove(eventItemComp.ID);

        List<GameObject> cardSlotList = DeckManager.instance.cardSlotList;
        for (int i = 0; i < cardSlotList.Count; i++) {
            if (cardSlotList[i].transform.childCount != 0) {
                Destroy(cardSlotList[i].transform.GetChild(0).gameObject);
            }
        }

        DeckManager.instance.DrawAllCards();

        EventUse();
    }

    void ItemBooster(UnitDefault unitComp)
    {
        AudioManager.instance.PlaySfx(eventUseClip);

        unitComp.steroid *= 2;
        unitComp.UpdateStatInfo(true);

        unitComp.SetStateBar(UnitDefault.State.steroid, -2f);

        UseEventCard();
    }

    void SpawnMercenary()
    {
        int cnt = 0;
        foreach (string unitID in StaticManager.allDeckList) {
            if (unitID[0] != 'u') {
                break;
            }
            cnt++;
        }

        if (cnt == 0) {
            InitPos();
            return;
        }

        AudioManager.instance.PlaySfx(eventUseClip);

        string randomUnitID = "Units/" + StaticManager.allDeckList[Random.Range(0, cnt)];
        
        GameObject prefab = Resources.Load<GameObject>(randomUnitID);
        GameObject unit = Instantiate(prefab, RandomPositionManager.instance.playerFieldList[StaticManager.curStage - 1]);
        
        unit.GetComponent<Drag>().enabled = true;

        UnitDefault unitComp = unit.GetComponent<UnitDefault>();
        unitComp.isEquipmentUnit = true;
        RelicsManager.instance.UnitEquipment(unitComp);

        GameManager.instance.SBTN.interactable = true;

        UseEventCard();
    }

    void Sacrifice()
    {
        AudioManager.instance.PlaySfx(eventUseClip);

        RelicsManager.instance.ResetRelics();
        RelicsManager.instance.SetRelicsAvtive();

        UnitDefault[] unitComps = UnitDefault.unitField.GetComponentsInChildren<UnitDefault>();
        foreach (UnitDefault unitComp in unitComps) {
            unitComp.ResetRelicsEffect();
            RelicsManager.instance.UnitEquipment(unitComp);
        }

        UnitDefault[] deckUnitComps = DeckManager.instance.deckObj.transform.parent.GetComponentsInChildren<UnitDefault>();
        foreach (UnitDefault unitComp in deckUnitComps) {
            unitComp.ResetRelicsEffect();
            RelicsManager.instance.UnitEquipment(unitComp);
        }

        UseEventCard();
    }

    void MasterKey()
    {
        if (StaticManager.isMasterKey) {
            InitPos();
            return;
        }

        AudioManager.instance.PlaySfx(eventUseClip);

        RemainingDeck.instance.useEventCardList.Add(eventItemComp.ID);
        StaticManager.allDeckList.Remove(eventItemComp.ID);

        StaticManager.isMasterKey = true;
        RemainingDeck.instance.emptyCardSlot = beginParent.parent;

        if (!RemainingDeck.instance.gameObject.activeSelf) {
            RemainingDeck.instance.gameObject.SetActive(true);
            RemainingDeck.instance.SetRemainingDeck();
        }

        EventUse();
        Destroy(beginParent.gameObject);
    }

    void EventUse()
    {
        if (isRunTime && eventCoroutine != null)
        {
            StopCoroutine(eventCoroutine);

            if (cardFrontObj != null)
                Destroy(cardFrontObj);
        }

        isRunTime = true;

        RectTransform rt = GetComponent<RectTransform>();

        cardFrontObj = Instantiate(DeckManager.instance.cardFrontList[2], rt.parent, false);
        transform.SetParent(cardFrontObj.transform, false);

        RectTransform eventRT = GetComponent<RectTransform>();
        eventRT.anchoredPosition = Vector2.zero;
        transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        transform.localPosition = new Vector3(0, -15, 0);

        cardFrontObj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        cardFrontObj.GetComponent<Image>().material = Instantiate(cardMaterial);
        GetComponent<Image>().material = Instantiate(eventMaterial);

        cardMaterial.SetFloat("_Value", maxValue);
        eventMaterial.SetFloat("_Value", maxValue);

        getValue = maxValue;

        eventCoroutine = StartCoroutine(EventAnimationCoroutine());
    }

    private IEnumerator EventAnimationCoroutine()
    {
        Image cardImg = cardFrontObj.GetComponent<Image>();
        Image eventImg = GetComponent<Image>();

        while (getValue > minValue)
        {
            getValue -= 0.6f * Time.deltaTime;
            getValue = Mathf.Max(getValue, minValue);

            if (cardImg != null) cardImg.material.SetFloat("_Value", getValue);
            eventImg.material.SetFloat("_Value", getValue);

            yield return null;
        }

        isRunTime = false;
        Destroy(cardFrontObj);
    }
}