using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class ShellGame : MonoBehaviour
{
    public List<Transform> shellList;
    public List<Image> shellImgList;
    public List<Button> shellBtnList;
    public List<Transform> contentsPosList;
    public TextMeshProUGUI curPriceTxt;
    public Button retryButton;

    public AudioClip successClip;
    public AudioClip failureClip;
    public AudioClip getCardClip;

    int curPrice = 100;

    private void Awake()
    {
        for (int i = 0; i < shellBtnList.Count; i++)
        {
            int captureIdx = i;
            shellBtnList[captureIdx].onClick.AddListener(() =>
            {
                for (int j = 0; j < shellBtnList.Count; j++) {
                    shellBtnList[j].enabled = false;
                }

                if (!GameManager.instance.SetGold(-curPrice))
                    return;

                EventCardDrop.instance.exitBtn.interactable = false;
                StaticManager.eventDOTWeenCnt += 2;

                if (curPrice == 500) {
                    SteamAchievement.Unlock("멈추지 않는 손");
                }
                curPrice += 100;

                if (contentsPosList[captureIdx].childCount != 0) {
                    AudioManager.instance.PlaySfx(successClip);
                    SteamAchievement.Unlock("손보다 빠른 눈");
                }
                else {
                    AudioManager.instance.PlaySfx(failureClip);
                    SteamAchievement.Unlock("눈속임에 걸리다");
                }

                shellList[captureIdx].DOMoveY(5f, 1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    for (int j = 0; j < shellBtnList.Count; j++) {
                        int captureJ = j;
                        if (captureIdx != captureJ) {
                            shellImgList[captureJ].DOFade(0.7f, 1f)
                            .OnComplete(() =>
                            {
                                retryButton.interactable = true;

                                if (--StaticManager.eventDOTWeenCnt == 0)
                                    EventCardDrop.instance.exitBtn.interactable = true;
                            });
                        }
                    }
                });
            });
        }
    }

    void SetContents()
    {
        int randNum = Random.Range(0, shellList.Count);
        for (int i = 0; i < shellList.Count; i++)
        {
            shellImgList[i].color = Color.white;
            shellList[i].position = contentsPosList[i].position;
            shellBtnList[i].enabled = true;

            if (contentsPosList[i].childCount != 0) {
                Destroy(contentsPosList[i].GetChild(0).gameObject);
            }

            if (i == randNum)
                continue;

            int randNum2 = Random.Range(48, 61);
            while (CardReward.instance.rewardList[randNum2].ID[0] == 'i' && CardReward.instance.rewardList[randNum2].probability == 0) {
                randNum2 = Random.Range(48, 61);
            }
            string cardID = CardReward.instance.rewardList[randNum2].ID;
            string pathID = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out _, out GameObject cardFront);
            GameObject cardFrontObj = Instantiate(cardFront, contentsPosList[i]);
            GameObject contents = Instantiate(Resources.Load<GameObject>(pathID), cardFrontObj.transform);

            contents.AddComponent<Button>().onClick.AddListener(() =>
            {
                contents.GetComponent<Button>().enabled = false;

                EventCardDrop.instance.exitBtn.interactable = false;
                StaticManager.eventDOTWeenCnt++;

                cardFrontObj.transform.SetParent(DeckManager.instance.deckObj.transform);
                cardFrontObj.transform.DOMove(DeckManager.instance.deckObj.transform.position, 1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    AudioManager.instance.PlaySfx(getCardClip);

                    Destroy(cardFrontObj);

                    StaticManager.allDeckList.Add(cardID);
                    DeckManager.instance.DeckSort();

                    if (--StaticManager.eventDOTWeenCnt == 0)
                        EventCardDrop.instance.exitBtn.interactable = true;
                });
            });
        }
    }

    public void RetryBtnClick()
    {
        retryButton.interactable = false;
        UpdatePriceText();
        SetContents();
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetShellLocaleDesc());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        SetContents();

        // 로케일 변경 시 즉시 텍스트 갱신을 위해 이벤트 구독
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        // 초기 표시 갱신 (로케일에 맞춰)
        UpdatePriceText();
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        curPrice = 100;
        UpdatePriceText();
        retryButton.interactable = false;
    }

    // 로컬 텍스트 묶음
    private struct ShellLocaleTexts
    {
        public string priceTemplate; // {0} -> curPrice
    }

    private void UpdatePriceText()
    {
        var tpl = GetLocaleTexts();
        curPriceTxt.SetText(string.Format(tpl.priceTemplate, curPrice));
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        // 창이 활성화되어 있을 때만 갱신
        if (gameObject.activeInHierarchy)
            UpdatePriceText();

        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetShellLocaleDesc());
    }

    // 현재 Locale에 맞는 템플릿 반환 (정확매치 -> 접두사 -> zh -> en fallback)
    private ShellLocaleTexts GetLocaleTexts()
    {
        // SelectedLocale null 체크 후 Identifier.Code 사용, ToLower 제거
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        // StringComparison.OrdinalIgnoreCase로 비교
        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "현재 가격 : <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Current price: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Prix actuel : <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Prezzo attuale: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Aktueller Preis: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Precio actual: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "現在の価格： <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Preço atual: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "Текущая цена: <color=#FFE600>{0}G</color>" };
        }
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShellLocaleTexts { priceTemplate = "当前价格： <color=#FFE600>{0}G</color>" };
        }

        // fallback: 영어
        return new ShellLocaleTexts { priceTemplate = "Current price: <color=#FFE600>{0}G</color>" };
    }

    // 이벤트 설명(상단 안내문)을 로컬라이즈해서 반환하는 헬퍼 추가
    private string GetShellLocaleDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "3개 중 2개의 컵 아래에 이벤트 또는 아이템 조각이 숨어있습니다.\n운이 좋다면 원하는 조각을 얻을 수 있을 것입니다.\n이용 횟수에 따라 가격이 증가하니 신중히 선택하세요.\n(찾은 조각을 클릭하지 않고 재시도 하거나 나가게 되면 조각은 소멸됩니다.)";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "Two of the three cups hide an event or item fragment.\nIf you're lucky you may get the piece you want.\nPrice increases with attempts, so choose carefully.\n(If you retry or leave without clicking the found piece, it will be destroyed.)";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Deux des trois coupes cachent un fragment d'événement ou d'objet.\nSi vous avez de la chance, vous obtiendrez peut-être la pièce souhaitée.\nLe prix augmente avec les tentatives, choisissez donc judicieusement.\n(Si vous réessayez ou partez sans cliquer sur la pièce trouvée, elle sera détruite.)";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Due delle tre tazze nascondono un frammento evento o oggetto.\nSe sei fortunato potresti ottenere il pezzo desiderato.\nIl prezzo aumenta con i tentativi, quindi scegli con cura.\n(Se riprovi o esci senza cliccare il frammento trovato, verrà distrutto.)";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Zwei der drei Becher verbergen ein Ereignis- oder Gegenstandfragment.\nWenn Sie Glück haben, erhalten Sie möglicherweise das gewünschte Fragment.\nDer Preis steigt mit den Versuchen, wählen Sie also sorgfältig.\n(Wenn Sie erneut versuchen oder nicht auf das gefundene Fragment klicken, wird es zerstört.)";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "Dos de las tres copas esconden un fragmento de evento o objeto.\nSi tienes suerte, puedes obtener la pieza que deseas.\nEl precio aumenta con los intentos, así que elige con cuidado.\n(Si vuelves a intentarlo o te vas sin hacer clic en la pieza encontrada, se destruirá.)";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "3つのカップのうち2つはイベントまたはアイテムの断片が隠れています。\n運が良ければ望む断片を手に入れられるでしょう。\n試行回数に応じて価格が上がるので慎重に選んでください。\n（見つけた断片をクリックせずに再試行または退出すると断片は消滅します。）";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Duas das três taças escondem um fragmento de evento ou item.\nSe tiver sorte, poderá obter o fragmento desejado.\nO preço aumenta com as tentativas, por isso escolha com cuidado.\n(Se tentar novamente ou sair sem clicar no fragmento encontrado, ele será destruído.)";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Две из трех чашек скрывают фрагмент события или предмета.\nЕсли повезет, вы можете получить нужный фрагмент.\nЦена увеличивается с количеством попыток, поэтому выбирайте осторожно.\n(Если вы повторите попытку или уйдете, не кликнув найденный фрагмент, он будет уничтожен.)";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "三个杯子中有两个隐藏着事件或道具碎片。\n如果你幸运的话可能会得到想要的碎片。\n随着尝试次数增加，价格会上涨，请谨慎选择。\n（如果您在未点击找到的碎片情况下重试或离开，碎片将被销毁。）";

        // fallback: 영어
        return "Two of the three cups hide an event or item fragment.\nIf you're lucky you may get the piece you want.\nPrice increases with attempts, so choose carefully.\n(If you retry or leave without clicking the found piece, it will be destroyed.)";
    }
}