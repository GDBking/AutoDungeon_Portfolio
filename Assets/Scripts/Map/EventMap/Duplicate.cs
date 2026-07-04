using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Duplicate : MonoBehaviour
{
    public static Duplicate instance;

    public TextMeshProUGUI cntTxt;
    public RectTransform slot1;
    public RectTransform slot2;
    public GameObject sliderPanel;
    public Slider slider;
    public TextMeshProUGUI sliderTxt;
    public Button duplicateBtn;

    public GameObject duplicateCheckUI;
    public TextMeshProUGUI duplicateTxt;

    public AudioClip sliderClip;
    public AudioClip successClip;
    public AudioClip failureClip;
    public AudioClip getCardClip;

    int cnt;
    string cardID;
    int cardPrice;
    string captureID;
    int capturePrice;

    int useMoney;
    int prob;

    GameObject slot1Card;
    GameObject slot2Card;

    string selectedName;

    private void Awake()
    {
        instance = this;
    }

    public void CardClick(string name, string ID, int price, UnitDefault unitComp)
    {
        if (cnt == 0)
            return;

        cardID = ID;
        cardPrice = price;

        string Highlight = $"<b>‘{unitComp.GetLocalizedUnitName()}’</b>";

        // 로컬 텍스트 가져오기
        var texts = GetDuplicateLocaleTexts();

        duplicateTxt.SetText(
            string.Format(texts.duplicateConfirm, Highlight)
        );
        duplicateCheckUI.SetActive(true);
    }

    public void OKBtnClick()
    {
        duplicateCheckUI.SetActive(false);

        captureID = cardID;
        capturePrice = cardPrice * 2;

        if (slot1.childCount != 0) {
            Destroy(slot1.GetChild(0).gameObject);
        }

        RemainingDeck.instance.gameObject.SetActive(false);

        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(captureID, out _, out GameObject cardFront);
        slot1Card = Instantiate(cardFront, slot1);

        Instantiate(Resources.Load<GameObject>(cardPath), slot1Card.transform);

        slider.SetValueWithoutNotify(1);
        ChangeSliderValue();
        slider.interactable = true;
        duplicateBtn.interactable = true;
        sliderPanel.SetActive(true);
    }

    public void ChangeSliderValue()
    {
        AudioManager.instance.PlaySfx(sliderClip);

        useMoney = capturePrice / 100 * (int)slider.value * 5;
        prob = (int)slider.value * 5;

        var texts = GetDuplicateLocaleTexts();
        sliderTxt.SetText(string.Format(texts.priceProbTemplate, useMoney, prob));
    }

    public void DuplicateBtnClick()
    {
        if (StaticManager.gold < useMoney)
            return;

        GameManager.instance.SetGold(-useMoney);

        slider.interactable = false;
        duplicateBtn.interactable = false;

        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(captureID, out _, out GameObject cardFront);
        slot2Card = Instantiate(cardFront, slot1.position, Quaternion.identity, slot2);

        Instantiate(Resources.Load<GameObject>(cardPath), slot2Card.transform);

        RectTransform rect = slot2Card.GetComponent<RectTransform>();

        // 스파크 이펙트 추가
        GameObject spark2 = Instantiate(Resources.Load<GameObject>("Effect/Spark 2"), slot1.position, Quaternion.identity, rect);

        StaticManager.eventDOTWeenCnt++;
        EventCardDrop.instance.exitBtn.interactable = false;
        rect.DOAnchorPosX(0f, 3f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                Destroy(spark2);

                if (Random.Range(1, 101) <= prob)
                {
                    AudioManager.instance.PlaySfx(successClip);

                    SuccessSlotSparkEffect();
                    GetCard();

                    if (prob == 5) {
                        SteamAchievement.Unlock("한 줌의 기적");
                    }
                }
                else
                {
                    AudioManager.instance.PlaySfx(failureClip);

                    FailSlotSparkEffect();

                    if (--StaticManager.eventDOTWeenCnt == 0)
                        EventCardDrop.instance.exitBtn.interactable = true;

                    SetCntTxt(--cnt);
                    Destroy(slot1Card);
                    Destroy(slot2Card);

                    if (prob == 90) {
                        SteamAchievement.Unlock("믿음의 붕괴");
                    }
                }
            });
    }
    void SuccessSlotSparkEffect()
    {
        GameObject spark1 = Instantiate(Resources.Load<GameObject>("Effect/Circle Spark"), slot1.position, Quaternion.identity);
        Destroy(spark1,0.5f);
    }

    void FailSlotSparkEffect()
    {
        GameObject spark1 = Instantiate(Resources.Load<GameObject>("Effect/Spark 1"), slot1.position, Quaternion.identity);
        Destroy(spark1,0.5f);
    }

    void GetCard()
    {
        slot1Card.transform.SetParent(DeckManager.instance.deckObj.transform);
        slot2Card.transform.SetParent(DeckManager.instance.deckObj.transform);

        slot1Card.transform.DOMove(DeckManager.instance.deckObj.transform.position, DeckManager.instance.cardAnimList[0].GetComponent<AnimationToImage>().GetAnimationLength())
            .SetEase(Ease.InOutSine);

        slot2Card.transform.DOMove(DeckManager.instance.deckObj.transform.position, DeckManager.instance.cardAnimList[0].GetComponent<AnimationToImage>().GetAnimationLength())
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                AudioManager.instance.PlaySfx(getCardClip);

                StaticManager.allDeckList.Add(captureID);
                DeckManager.instance.DeckSort();

                if (--StaticManager.eventDOTWeenCnt == 0)
                    EventCardDrop.instance.exitBtn.interactable = true;

                SetCntTxt(--cnt);
                Destroy(slot1Card);
                Destroy(slot2Card);
            });
    }

    void SetCntTxt(int cnt)
    {
        var texts = GetDuplicateLocaleTexts();
        cntTxt.SetText(string.Format(texts.remainingTemplate, cnt));
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetDuplicateFragmentLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        StaticManager.isDuplicate = true;
        SetCntTxt(cnt = 2);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        if (slot1.childCount != 0) {
            Destroy(slot1.GetChild(0).gameObject);
        }

        sliderPanel.SetActive(false);
        StaticManager.isDuplicate = false;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        // 현재 활성화된 UI만 갱신
        if (!gameObject.activeInHierarchy) return;

        // cnt 텍스트 갱신
        SetCntTxt(cnt);

        // 슬라이더 패널이 보여지고 있으면 슬라이더 텍스트 갱신
        if (sliderPanel.activeInHierarchy)
            ChangeSliderValue();

        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetDuplicateFragmentLocaleText());

        // 확인창이 열려 있으면 문구 갱신 (selectedName이 비어있지 않으면)
        if (duplicateCheckUI != null && duplicateCheckUI.activeInHierarchy && !string.IsNullOrEmpty(selectedName))
        {
            var texts = GetDuplicateLocaleTexts();
            duplicateTxt.SetText(string.Format(texts.duplicateConfirm, selectedName));
        }
    }

    private struct DuplicateLocaleTexts
    {
        public string duplicateConfirm;   // "{0} 조각을 복제하길 원하십니까?"
        public string priceProbTemplate;  // "가격 : {0}G\n확률 : {1}%"
        public string remainingTemplate;  // "남은 횟수 : {0}"
    }

    private DuplicateLocaleTexts GetDuplicateLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        if (code.StartsWith("ko"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "{0} 조각을 복제하길 원하십니까?",
                priceProbTemplate = "가격 : {0}G\n확률 : {1}%",
                remainingTemplate = "남은 횟수 : {0}"
            };
        }
        if (code.StartsWith("en"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Do you want to duplicate the {0} piece?",
                priceProbTemplate = "Price: {0}G\nChance: {1}%",
                remainingTemplate = "Remaining tries: {0}"
            };
        }
        if (code.StartsWith("fr"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Voulez-vous dupliquer le fragment {0} ?",
                priceProbTemplate = "Prix : {0}G\nProbabilité : {1}%",
                remainingTemplate = "Restant : {0}"
            };
        }
        if (code.StartsWith("it"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Vuoi duplicare il frammento {0}?",
                priceProbTemplate = "Prezzo: {0}G\nProbabilità: {1}%",
                remainingTemplate = "Tentativi rimanenti: {0}"
            };
        }
        if (code.StartsWith("de"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Möchtest du das Fragment {0} duplizieren?",
                priceProbTemplate = "Preis: {0}G\nChance: {1}%",
                remainingTemplate = "Verbleibende Versuche: {0}"
            };
        }
        if (code.StartsWith("es"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "¿Deseas duplicar la pieza {0}?",
                priceProbTemplate = "Precio: {0}G\nProbabilidad: {1}%",
                remainingTemplate = "Intentos restantes: {0}"
            };
        }
        if (code.StartsWith("ja"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "{0} の破片を複製しますか？",
                priceProbTemplate = "価格 : {0}G\n確率 : {1}%",
                remainingTemplate = "残り回数 : {0}"
            };
        }
        if (code.StartsWith("pt"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Deseja duplicar o fragmento {0}?",
                priceProbTemplate = "Preço: {0}G\nChance: {1}%",
                remainingTemplate = "Tentativas restantes: {0}"
            };
        }
        if (code.StartsWith("ru"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "Вы хотите дублировать фрагмент {0}?",
                priceProbTemplate = "Цена: {0}G\nШанс: {1}%",
                remainingTemplate = "Осталось попыток: {0}"
            };
        }
        if (code.StartsWith("zh"))
        {
            return new DuplicateLocaleTexts
            {
                duplicateConfirm = "是否要复制 {0} 碎片？",
                priceProbTemplate = "价格：{0}G\n概率：{1}%",
                remainingTemplate = "剩余次数：{0}"
            };
        }

        // fallback (영어)
        return new DuplicateLocaleTexts
        {
            duplicateConfirm = "Do you want to duplicate the {0} piece?",
            priceProbTemplate = "Price: {0}G\nChance: {1}%",
            remainingTemplate = "Remaining tries: {0}"
        };
    }

    private string GetDuplicateFragmentLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "보유 중인 조각 중 원하는 조각을 복제할 수 있습니다.\n복제 성공 확률은 지불한 G량에 따라 달라집니다.";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "You can duplicate a fragment of your choice from those you own.\nThe success chance depends on the amount of G you pay.";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Vous pouvez dupliquer le fragment de votre choix parmi ceux que vous possédez.\nLes chances de réussite dépendent de la quantité de G payée.";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Puoi duplicare un frammento a tua scelta tra quelli che possiedi.\nLa probabilità di successo dipende dalla quantità di G pagata.";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Sie können ein Fragment Ihrer Wahl aus Ihren vorhandenen Fragmenten duplizieren.\nDie Erfolgschance hängt von der gezahlten G-Menge ab.";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "Puedes duplicar el fragmento que desees de los que posees.\nLa probabilidad de éxito depende de la cantidad de G pagada.";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "所持している断片の中から、好きな断片を複製できます。\n複製の成功確率は支払ったGの量によって変化します。";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Você pode duplicar um fragmento de sua escolha entre os que possui.\nA chance de sucesso depende da quantidade de G paga.";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Вы можете скопировать выбранный фрагмент из тех, которыми владеете.\nВероятность успеха зависит от количества уплаченного G.";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "你可以从已拥有的碎片中选择一个进行复制。\n复制成功率取决于你支付的G数量。";

        // fallback: English
        return "You can duplicate a fragment of your choice from those you own.\nThe success chance depends on the amount of G you pay.";
    }
}