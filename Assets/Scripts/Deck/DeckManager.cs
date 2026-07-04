using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    public List<string> initDeckList;
    public List<GameObject> cardAnimList;
    public List<GameObject> cardFrontList;
    public GameObject deckObj;
    public TextMeshProUGUI deckCountText;
    public List<GameObject> cardSlotList;
    public List<TextMeshProUGUI> allDeckRelicsCountTxt;

    int[] allDeckRelicsCount;
    public GameObject useEventCardPanel;

    public AudioClip cardDrawClip;
    public AudioClip deckClickClip;

    private void Awake()
    {
        if (StaticManager.allDeckList == null) {
            StaticManager.allDeckList = new List<string>();
            for (int i = 0; i < 4; i++) {
                StaticManager.allDeckList.Add(initDeckList[Random.Range(0, 12)]);
            }
            for (int i = 0; i < 3; i++) {
                StaticManager.allDeckList.Add(initDeckList[Random.Range(12, 24)]);
            }
            for (int i = 0; i < 2; i++) {
                StaticManager.allDeckList.Add(initDeckList[Random.Range(24, 36)]);
            }
            StaticManager.allDeckList.Add(initDeckList[Random.Range(36, 48)]);
        }
        DeckSort();
    }

    public void DeckSort()
    {
        StaticManager.allDeckList.Sort((a, b) => b.CompareTo(a));
        StaticManager.remainingDeckList = new(StaticManager.allDeckList);

        deckCountText.text = StaticManager.remainingDeckList.Count.ToString();

        allDeckRelicsCount = new int[11];
        foreach (string ID in StaticManager.allDeckList) {
            if (ID[..4] == "unit" && Resources.Load<GameObject>($"Units/{ID}").TryGetComponent<UnitDefault>(out UnitDefault unitComp)) {
                allDeckRelicsCount[(int)StaticManager.title[unitComp.unitIdx][0]]++;
                allDeckRelicsCount[(int)StaticManager.title[unitComp.unitIdx][1]]++;
                allDeckRelicsCount[(int)StaticManager.title[unitComp.unitIdx][2]]++;
                if ((int)unitComp.rank == 0 || (int)unitComp.rank == 3)
                    allDeckRelicsCount[9]++;
                else
                    allDeckRelicsCount[10]++;
            }
            else
                break;
        }

        for (int i = 0; i < allDeckRelicsCountTxt.Count; i++) {
            allDeckRelicsCountTxt[i].text = allDeckRelicsCount[i].ToString();
        }

        // 남은 덱 패널이 켜져 있을 경우 최신화
        if (RemainingDeck.instance.gameObject.activeSelf)
            RemainingDeck.instance.SetRemainingDeck();
    }

    public void DeckClick()
    {
        // 해당 레벨에서 처음으로 덱을 클릭 했을 시
        if (!StaticManager.isDeckInit) {
            DrawAllCards();
            StaticManager.isDeckInit = true;
        }
        // 그렇지 않을 경우 남은 덱 보여주기 or 창 닫기
        else {
            AudioManager.instance.PlaySfx(deckClickClip);

            GameObject remainingDeckPanel = RemainingDeck.instance.gameObject;
            remainingDeckPanel.SetActive(!remainingDeckPanel.activeSelf);
        }
    }

    public void DrawAllCards()
    {
        StartCoroutine(DrawAllCardsRoutine());
    }

    IEnumerator DrawAllCardsRoutine()
    {
        for (int i = 0; i < cardSlotList.Count; i++) {
            if (StaticManager.remainingDeckList.Count == 0) {
                // 남은 덱 패널이 켜져 있을 경우 최신화
                if (RemainingDeck.instance.gameObject.activeSelf)
                    RemainingDeck.instance.SetRemainingDeck();

                yield break;
            }

            int randCardNum = Random.Range(0, StaticManager.remainingDeckList.Count);
            string randCardID = StaticManager.remainingDeckList[randCardNum];

            // 드로우된 카드 제거 및 카운트 최신화
            StaticManager.remainingDeckList.RemoveAt(randCardNum);
            deckCountText.text = StaticManager.remainingDeckList.Count.ToString();

            // CardDraw가 끝날 때까지 대기
            yield return StartCoroutine(CardDrawRoutine(randCardID, cardSlotList[i].transform));
        }
    }

    public void CardDraw(Transform emptyCardSlot)
    {
        if (StaticManager.remainingDeckList.Count == 0){
            // 남은 덱 패널이 켜져 있을 경우 최신화
            if (RemainingDeck.instance.gameObject.activeSelf)
                RemainingDeck.instance.SetRemainingDeck();

            return;
        }

        int randCardNum = Random.Range(0, StaticManager.remainingDeckList.Count);
        string randCardID = StaticManager.remainingDeckList[randCardNum];

        // 드로우된 카드 제거 및 카운트 최신화
        StaticManager.remainingDeckList.RemoveAt(randCardNum);
        deckCountText.text = StaticManager.remainingDeckList.Count.ToString();

        StartCoroutine(CardDrawRoutine(randCardID, emptyCardSlot));
    }

    public void MasterKeyCardDraw(Transform emptyCardSlot, string masterKeyID)
    {
        // 드로우된 카드 제거 및 카운트 최신화
        StaticManager.remainingDeckList.Remove(masterKeyID);
        deckCountText.text = StaticManager.remainingDeckList.Count.ToString();

        StartCoroutine(CardDrawRoutine(masterKeyID, emptyCardSlot));
    }

    IEnumerator CardDrawRoutine(string cardID, Transform emptyCardSlot)
    {
        AudioManager.instance.PlaySfx(cardDrawClip);

        // 남은 덱 패널이 켜져 있을 경우 최신화
        if (RemainingDeck.instance.gameObject.activeSelf)
            RemainingDeck.instance.SetRemainingDeck();

        cardID = GetCardTypeAndResourcesPath(cardID, out GameObject cardAnim, out _);
        GameObject cardAnimObj = Instantiate(cardAnim, deckObj.transform.position, Quaternion.identity, emptyCardSlot);

        // 빈 슬롯까지 덱에서 카드가 자연스럽게 이동
        Vector2 startPos = deckObj.transform.position;
        Vector2 targetPos = emptyCardSlot.position;

        float duration = cardAnimObj.GetComponent<AnimationToImage>().GetAnimationLength();
        float elapsedTime = 0f;
        float t = 0f;
        while (t < 1f && cardAnimObj != null) {
            elapsedTime += Time.deltaTime;

            t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t); // Ease In/Out 적용

            cardAnimObj.transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 카드가 완전히 뒤집어지기 전에 전투를 시작하면서 사라질 수 있음
        if (cardAnimObj == null)
            yield break;

        // 카드 이동이 끝났으면 해당 카드 타입의 자식으로 실제 카드의 콘텐츠 생성
        GameObject contents = Instantiate(Resources.Load<GameObject>(cardID), cardAnimObj.transform);
        contents.GetComponent<Drag>().enabled = true;

        if (cardID[..4] == "Unit") {
            // 유물 효과 적용
            RelicsManager.instance.UnitEquipment(contents.GetComponent<UnitDefault>());
        }
    }

    public void FieldDeckReset()
    {
        for (int i = 0; i < cardSlotList.Count; i++) {
            if (cardSlotList[i].transform.childCount != 0) {
                Destroy(cardSlotList[i].transform.GetChild(0).gameObject);
            }
        }

        StaticManager.remainingDeckList = new(StaticManager.allDeckList);
        deckCountText.text = StaticManager.remainingDeckList.Count.ToString();

        // 남은 덱 패널이 켜져 있을 경우 최신화
        if (RemainingDeck.instance.gameObject.activeSelf)
            RemainingDeck.instance.SetRemainingDeck();
    }

    public string GetCardTypeAndResourcesPath(string cardID, out GameObject cardAnim, out GameObject cardFront)
    {
        // 카드 타입 확인
        string cardTypeID = cardID[..4];

        // 카드 타입에 맞는 카드 반환
        switch (cardTypeID) {
            case "unit":
                cardID = "Units/" + cardID;
                cardAnim = cardAnimList[0];
                cardFront = cardFrontList[0];
                break;
            case "item":
                cardID = "Items/" + cardID;
                cardAnim = cardAnimList[1];
                cardFront = cardFrontList[1];
                break;
            case "even":
                cardID = "Events/" + cardID;
                cardAnim = cardAnimList[2];
                cardFront = cardFrontList[2];
                break;
            default:
                cardAnim = null;
                cardFront = null;
                break;
        }

        return cardID;
    }
}