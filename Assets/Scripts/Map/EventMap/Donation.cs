using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Donation : MonoBehaviour
{
    public Transform cardSlot;
    public TextMeshProUGUI donationTxt;
    public GameObject donationBtn;
    public GameObject donationGrid;
    public GameObject getBtn;
    public Sprite defaultSprite;

    public AudioClip rewardClip;
    public AudioClip getCardClip;

    string cardID;
    readonly List<string> rewardList = new();
    readonly List<GameObject> rewardCardList = new();

    void SetDonationCard()
    {
        cardID = StaticManager.allDeckList[Random.Range(0, StaticManager.allDeckList.Count)];
        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out _, out GameObject cardFront);

        GameObject cardFrontObj = Instantiate(cardFront, cardSlot);
        Instantiate(Resources.Load<GameObject>(cardPath), cardFrontObj.transform);

        var texts = GetDonationLocaleTexts();
        donationTxt.SetText(texts.confirm);
    }

    public void DonationBtnClick()
    {
        donationBtn.SetActive(false);
        StaticManager.eventDOTWeenCnt++;
        EventCardDrop.instance.exitBtn.interactable = false;

        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out _, out GameObject cardFront);

        GameObject cardFrontObj = Instantiate(cardFront, DeckManager.instance.deckObj.transform.position, Quaternion.identity, DeckManager.instance.deckObj.transform);
        Instantiate(Resources.Load<GameObject>(cardPath), cardFrontObj.transform);

        cardFrontObj.transform.DOMove(cardSlot.transform.position, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                Destroy(cardFrontObj);
                StaticManager.allDeckList.Remove(cardID);
                DeckManager.instance.DeckSort();
                RewardProb();
            });

        GetComponent<Animator>().enabled = true;
    }

    void RewardProb()
    {
        var texts = GetDonationLocaleTexts();
        donationTxt.SetText(texts.fortune);

        if (Random.Range(1, 101) <= StaticManager.donationProb) {
            AudioManager.instance.PlaySfx(rewardClip);

            StaticManager.donationProb = 25;
            rewardList.Clear();
            rewardCardList.Clear();
            for (int i = 0; i < 5; i++) {
                rewardList.Add(CardReward.instance.rewardList[Random.Range(54, 61)].ID);

                string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(rewardList[i], out _, out GameObject cardFront);

                GameObject cardFrontObj = Instantiate(cardFront, donationGrid.transform);
                Instantiate(Resources.Load<GameObject>(cardPath), cardFrontObj.transform);

                rewardCardList.Add(cardFrontObj);
            }

            donationGrid.SetActive(true);
            getBtn.SetActive(true);

            SteamAchievement.Unlock("선의의 보답");
        }
        else {
            StaticManager.donationProb += 25;

            if (--StaticManager.eventDOTWeenCnt == 0)
                EventCardDrop.instance.exitBtn.interactable = true;

            SteamAchievement.Unlock("보답 없는 선의");
        }
    }

    public void GetBtnClick()
    {
        getBtn.SetActive(false);

        for (int i = 0; i < rewardCardList.Count; i++) {
            int idx = i;

            rewardCardList[i].transform.SetParent(DeckManager.instance.deckObj.transform);
            rewardCardList[i].transform.DOMove(DeckManager.instance.deckObj.transform.position, 1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    AudioManager.instance.PlaySfx(getCardClip);

                    Destroy(rewardCardList[idx]);

                    if (idx == rewardCardList.Count - 1 && --StaticManager.eventDOTWeenCnt == 0) {
                        EventCardDrop.instance.exitBtn.interactable = true;
                    }
                });
        }

        StaticManager.allDeckList.AddRange(rewardList);
        DeckManager.instance.DeckSort();
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetNeedPieceEventDescLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        SetDonationCard();

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        if (donationBtn.activeSelf) {
            SteamAchievement.Unlock("외면");
        }

        if (cardSlot.childCount != 0) {
            Destroy(cardSlot.GetChild(0).gameObject);
        }

        GetComponent<Image>().sprite = defaultSprite;
        GetComponent<Animator>().enabled = false;

        donationBtn.SetActive(true);
        donationGrid.SetActive(false);
        getBtn.SetActive(false);

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    private void OnLocaleChanged(Locale newLocale)
    {
        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetNeedPieceEventDescLocaleText());

        // 창이 활성화된 상태에서만 텍스트만 갱신 (카드 재생성은 하지 않음)
        if (!gameObject.activeInHierarchy) return;

        var texts = GetDonationLocaleTexts();

        // 보상 그리드가 열려 있으면 fortune 텍스트, 아니면 confirm 텍스트
        if (donationGrid != null && donationGrid.activeInHierarchy)
            donationTxt.SetText(texts.fortune);
        else
            donationTxt.SetText(texts.confirm);
    }

    private struct DonationTexts
    {
        public string confirm; // 기부 여부 질문
        public string fortune; // 보상 시 표시 문구
    }

    private DonationTexts GetDonationLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        if (code.StartsWith("ko"))
        {
            return new DonationTexts
            {
                confirm = "이 조각을 기부해 주시겠습니까?",
                fortune = "당신의 여정에 행운을 빕니다."
            };
        }
        if (code.StartsWith("en"))
        {
            return new DonationTexts
            {
                confirm = "Would you like to donate this piece?",
                fortune = "May fortune favor your journey."
            };
        }
        if (code.StartsWith("fr"))
        {
            return new DonationTexts
            {
                confirm = "Voulez-vous faire don de ce fragment ?",
                fortune = "Que la chance accompagne votre voyage."
            };
        }
        if (code.StartsWith("it"))
        {
            return new DonationTexts
            {
                confirm = "Vuoi donare questo frammento?",
                fortune = "Che la fortuna accompagni il tuo viaggio."
            };
        }
        if (code.StartsWith("de"))
        {
            return new DonationTexts
            {
                confirm = "Möchten Sie dieses Fragment spenden?",
                fortune = "Möge das Glück deine Reise begleiten."
            };
        }
        if (code.StartsWith("es"))
        {
            return new DonationTexts
            {
                confirm = "¿Desea donar esta pieza?",
                fortune = "Que la fortuna acompañe tu viaje."
            };
        }
        if (code.StartsWith("ja"))
        {
            return new DonationTexts
            {
                confirm = "この破片を寄付しますか？",
                fortune = "あなたの旅に幸運を。"
            };
        }
        if (code.StartsWith("pt"))
        {
            return new DonationTexts
            {
                confirm = "Deseja doar este fragmento?",
                fortune = "Que a sorte acompanhe sua jornada."
            };
        }
        if (code.StartsWith("ru"))
        {
            return new DonationTexts
            {
                confirm = "Вы хотите пожертвовать этот фрагмент?",
                fortune = "Пусть удача сопутствует вашему пути."
            };
        }
        if (code.StartsWith("zh"))
        {
            return new DonationTexts
            {
                confirm = "您想要捐赠此碎片吗？",
                fortune = "愿好运伴随你的旅程。"
            };
        }

        // fallback (영어)
        return new DonationTexts
        {
            confirm = "Would you like to donate this piece?",
            fortune = "May fortune favor your journey."
        };
    }

    private string GetNeedPieceEventDescLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "누군가 당신에게 있는 조각 중 하나를 필요로 합니다.";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "Someone needs one of the fragments you possess.";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Quelqu'un a besoin de l'un des fragments que vous possédez.";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Qualcuno ha bisogno di uno dei frammenti che possiedi.";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Jemand benötigt eines der Fragmente, die Sie besitzen.";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "Alguien necesita uno de los fragmentos que posees.";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "誰かがあなたの所持している断片の一つを必要としています。";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Alguém precisa de um dos fragmentos que você possui.";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Кому-то нужен один из фрагментов, которыми вы владеете.";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "有人需要你所持有的某个碎片。";

        // fallback: English
        return "Someone needs one of the fragments you possess.";
    }
}