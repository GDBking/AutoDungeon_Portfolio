using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

[System.Serializable]
public class RewardProbability
{
    public string ID;
    public float probability;
    public float sumProb;
}

public class CardReward : MonoBehaviour
{
    public static CardReward instance;

    public List<RewardProbability> rewardList;
    public Transform rewardGrid;
    public GameObject rewardCardBack;
    public GameObject allOpenBtnObj;
    public GameObject getRewardsBtnObj;
    public TextMeshProUGUI getRewardsBtnTxt;
    public TextMeshProUGUI rewardTxt;

    public AudioClip rewardEnableClip;
    public AudioClip rewardDisableClip;
    public AudioClip getRewardClip;

    RewardType rewardType;
    readonly List<Transform> getRewardCardList = new();
    readonly List<string> getRewardList = new();
    int createRewardCount;
    int getRewardCount;
    int backCardClickCount;
    int rewardClickCount;

    private void Awake()
    {
        if (StaticManager.rewardList != null) {
            rewardList = StaticManager.rewardList;
        }
        SetProbability();
    }

    private struct CardRewardLocaleTexts
    {
        public string selectPiecesTextWithCount; // "덱에 추가할 조각을 선택하세요. {cur}/{max}"
        public string selectPiecesText;
        public string alreadySelectedText; // "이미 모든 조각을 선택했습니다. {cur}/{max}"
        public string skipText; // "넘어가기" / "Skip"
        public string acceptText; // "보상 받기" / "Get Rewards"
    }

    private CardRewardLocaleTexts GetLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        // 한국어
        if (code.StartsWith("ko"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "덱에 추가할 조각을 선택하세요. {0} / {1}",
                selectPiecesText = "덱에 추가할 조각을 선택하세요.",
                alreadySelectedText = "이미 모든 조각을 선택했습니다. {0} / {1}",
                skipText = "넘어가기",
                acceptText = "보상 받기"
            };
        }

        // 영어
        if (code.StartsWith("en"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "Select pieces to add to your deck. {0} / {1}",
                selectPiecesText = "Select pieces to add to your deck.",
                alreadySelectedText = "You have already selected all pieces. {0} / {1}",
                skipText = "Skip",
                acceptText = "Get Rewards"
            };
        }

        // 프랑스어
        if (code.StartsWith("fr"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "Sélectionnez les pièces à ajouter au deck. {0} / {1}",
                selectPiecesText = "Sélectionnez les pièces à ajouter au deck.",
                alreadySelectedText = "Vous avez déjà sélectionné toutes les pièces. {0} / {1}",
                skipText = "Passer",
                acceptText = "Recevoir les récompenses"
            };
        }

        // 일본어
        if (code.StartsWith("ja"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "デッキに追加するピースを選択してください。 {0} / {1}",
                selectPiecesText = "デッキに追加するピースを選択してください。",
                alreadySelectedText = "すでにすべてのピースを選択しています。 {0} / {1}",
                skipText = "スキップ",
                acceptText = "報酬を受け取る"
            };
        }

        // 포르투갈(브라질) / pt 접두사
        if (code.StartsWith("pt"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "Selecione as peças para adicionar ao baralho. {0} / {1}",
                selectPiecesText = "Selecione as peças para adicionar ao baralho.",
                alreadySelectedText = "Você já selecionou todas as peças. {0} / {1}",
                skipText = "Pular",
                acceptText = "Receber recompensas"
            };
        }

        // 중국어 간체
        if (code.StartsWith("zh"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "请选择要加入卡组的碎片。{0} / {1}",
                selectPiecesText = "请选择要加入卡组的碎片。",
                alreadySelectedText = "您已选择所有碎片。 {0} / {1}",
                skipText = "跳过",
                acceptText = "领取奖励"
            };
        }

        // 러시아어
        if (code.StartsWith("ru"))
        {
            return new CardRewardLocaleTexts
            {
                selectPiecesTextWithCount = "Выберите фрагменты для добавления в колоду. {0} / {1}",
                selectPiecesText = "Выберите фрагменты для добавления в колоду.",
                alreadySelectedText = "Вы уже выбрали все фрагменты. {0} / {1}",
                skipText = "Пропустить",
                acceptText = "Получить награды"
            };
        }

        // 독일/스페인/이탈리아 등 기타는 영어로 fallback
        return new CardRewardLocaleTexts
        {
            selectPiecesTextWithCount = "Select pieces to add to your deck. {0} / {1}",
            selectPiecesText = "Select pieces to add to your deck.",
            alreadySelectedText = "You have already selected all pieces. {0} / {1}",
            skipText = "Skip",
            acceptText = "Get Rewards"
        };
    }

    public void SetProbability()
    {
        rewardList[0].sumProb = rewardList[0].probability;
        for (int i = 1; i < rewardList.Count; i++) {
            rewardList[i].sumProb = rewardList[i - 1].sumProb + rewardList[i].probability;
        }
    }

    public void RewardCardClick(Transform btn)
    {
        StartCoroutine(RewardCardClickRoutine(btn));
    }

    public void AllOpenBtnClick()
    {
        foreach (Transform backCard in rewardGrid) {
            Button btn = backCard.GetComponent<Button>();
            if (btn.enabled) {
                btn.onClick.Invoke();
            }
        }
    }

    IEnumerator RewardCardClickRoutine(Transform btn)
    {
        if (++backCardClickCount == createRewardCount) {
            allOpenBtnObj.SetActive(false);
        }

        string cardID = GetRewardID();

        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(cardID, out GameObject cardAnim, out _);

        GameObject cardAnimObj = Instantiate(cardAnim, btn);
        yield return new WaitForSeconds(cardAnimObj.GetComponent<AnimationToImage>().GetAnimationLength());

        GameObject contents = Instantiate(Resources.Load<GameObject>(cardPath), cardAnimObj.transform);

        Button btnComp = contents.AddComponent<Button>();
        btnComp.onClick.AddListener(() =>
        {
            RewardContentsClick(contents.transform.parent.parent.GetChild(0).gameObject, cardID);
        });
    }

    public string GetRewardID()
    {
        string cardID = null;
        float randRewardNum = Random.Range(0f, rewardList[^1].sumProb);
        foreach (RewardProbability reward in rewardList) {
            if (reward.sumProb >= randRewardNum) {
                cardID = reward.ID;
                break;
            }
        }

        return cardID;
    }

    void RewardContentsClick(GameObject edgeChild, string cardID)
    {
        // 맨 위에 texts를 얻어둡니다 (또는 매번 GetLocaleTexts() 호출)
        var texts = GetLocaleTexts();

        // 아직 클릭하지 않은 카드를 클릭했을 경우
        if (!edgeChild.activeSelf) {
            // 아직 모두 선택하지 않은 경우
            if (rewardClickCount < getRewardCount) {
                AudioManager.instance.PlaySfx(rewardEnableClip);

                rewardClickCount++;

                edgeChild.SetActive(true);
                getRewardList.Add(cardID);
                getRewardCardList.Add(edgeChild.transform.parent);

                rewardTxt.SetText(string.Format(texts.selectPiecesTextWithCount, rewardClickCount, getRewardCount));
            }
            // 이미 카드를 모두 선택한 경우
            else {
                rewardTxt.SetText(string.Format(texts.alreadySelectedText, rewardClickCount, getRewardCount));
            }
        }
        // 이미 선택한 카드를 다시 클릭했을 경우
        else {
            AudioManager.instance.PlaySfx(rewardDisableClip);

            rewardClickCount--;

            edgeChild.SetActive(false);
            getRewardList.Remove(cardID);
            getRewardCardList.Remove(edgeChild.transform.parent);

            rewardTxt.SetText(string.Format(texts.selectPiecesTextWithCount, rewardClickCount, getRewardCount));
        }

        if (rewardClickCount == 0)
            getRewardsBtnTxt.SetText(texts.skipText);
        else
            getRewardsBtnTxt.SetText(texts.acceptText);
    }

    public void GetRewardsBtnClick()
    {
        getRewardsBtnObj.SetActive(false);

        if (rewardClickCount == 0) {
            gameObject.SetActive(false);
            return;
        }

        foreach (Transform card in rewardGrid) {
            card.GetChild(1).GetChild(0).GetComponent<Button>().enabled = false;
        }

        StartCoroutine(GetRewardsAll());
    }

    IEnumerator GetRewardsAll()
    {
        WaitForSeconds wait = new(0.1f);

        for (int i = 0; i < getRewardList.Count; i++) {
            StartCoroutine(GetRewards(getRewardCardList[i], i == getRewardList.Count - 1));
            yield return wait;
        }
    }

    IEnumerator GetRewards(Transform getCard, bool isEndCard)
    {
        getCard.SetParent(DeckManager.instance.deckObj.transform);
        Vector2 startPos = getCard.position;
        Vector2 targetPos = DeckManager.instance.deckObj.transform.position;

        float elapsedTime = 0f;
        float t = 0f;
        while (t < 1f) {
            elapsedTime += Time.deltaTime;

            t = Mathf.Clamp01(elapsedTime);
            t = Mathf.SmoothStep(0f, 1f, t); // Ease In/Out 적용

            getCard.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        AudioManager.instance.PlaySfx(getRewardClip);

        // 카드 이동이 끝났으면 파괴
        Destroy(getCard.gameObject);

        if (isEndCard) {
            StaticManager.allDeckList.AddRange(getRewardList);
            DeckManager.instance.DeckSort();
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        rewardType = StaticManager.isWin ? StaticManager.rewardType : RewardType.eliteMonster;
        // 일반 맵이면 3개, 엘리트/보스 맵이면 5개 생성
        createRewardCount = rewardType == RewardType.monster ? 3 : 5;
        // 일반 맵이면 1개, 엘리트/보스 맵이면 2개 선택 가능
        getRewardCount = rewardType == RewardType.monster ? 1 : 2;

        for (int i = 0; i < createRewardCount; i++) {
            GameObject cardBack = Instantiate(rewardCardBack, rewardGrid);
            Button btn = cardBack.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                btn.enabled = false;

                // 알파를 0으로 바꿔 안 보이게 변경
                Image img = btn.GetComponent<Image>();
                Color c = img.color;
                c.a = 0f;   // 알파만 0으로
                img.color = c;

                RewardCardClick(btn.transform);
            });
        }

        // OnEnable() 내에 추가: 로케일 변경 시 UI 갱신을 위해 이벤트 구독
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        // 기존 rewardTxt.SetText(...) 대체:
        var texts = GetLocaleTexts();
        rewardTxt.SetText(texts.selectPiecesText);

        // 버튼 초기 텍스트도 로컬라이즈
        getRewardsBtnTxt.SetText(texts.skipText);
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        foreach (Transform reward in rewardGrid) {
            Destroy(reward.gameObject);
        }

        getRewardList.Clear();
        getRewardCardList.Clear();

        backCardClickCount = 0;
        rewardClickCount = 0;
        allOpenBtnObj.SetActive(true);
        getRewardsBtnObj.SetActive(true);
        
        var texts = GetLocaleTexts();
        getRewardsBtnTxt.SetText(texts.skipText);
    }
    private void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        // 현재 창이 활성화되어 있을 때만 갱신
        if (gameObject.activeInHierarchy)
        {
            var texts = GetLocaleTexts();
            // rewardTxt: 현재 선택 상태에 따라 적절한 문구로 갱신
            if (rewardClickCount == 0)
                rewardTxt.SetText(string.Format(texts.selectPiecesTextWithCount, rewardClickCount, getRewardCount));
            else if (rewardClickCount >= getRewardCount)
                rewardTxt.SetText(string.Format(texts.alreadySelectedText, rewardClickCount, getRewardCount));
            else
                rewardTxt.SetText(string.Format(texts.selectPiecesTextWithCount, rewardClickCount, getRewardCount));

            // 버튼 라벨 갱신
            getRewardsBtnTxt.SetText(rewardClickCount == 0 ? texts.skipText : texts.acceptText);
        }
    }
}