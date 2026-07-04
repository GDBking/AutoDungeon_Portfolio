using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System.Collections;

public class ChaosRoulette : MonoBehaviour
{
    public TextMeshProUGUI chaosTxt;
    public Button OKBtn;

    public AudioClip destroyClip;

    private struct ChaosLocaleTexts
    {
        public string initial;    // 처음 설명문 (여러 줄 포함)
        public string noCard;     // 파괴할 카드가 없을 때 메시지
        public string final;      // 완료 메시지
    }

    void SetText()
    {
        var texts = GetChaosLocaleTexts();
        chaosTxt.SetText(texts.initial);
    }

    public void OKBtnClick()
    {
        OKBtn.interactable = false;

        GetComponent<Animator>().SetTrigger("Use");

        List<string> cardList = new();
        for (int i = 0; i < StaticManager.allDeckList.Count - 1; i++) {
            if (StaticManager.allDeckList[i][0] == 'e')
                break;
            if (StaticManager.allDeckList[i] != StaticManager.allDeckList[i + 1])
                cardList.Add(StaticManager.allDeckList[i]);
        }
        if (StaticManager.allDeckList.Count != 0 && StaticManager.allDeckList[^1][0] != 'e')
            cardList.Add(StaticManager.allDeckList[^1]);

        // 카드가 하나도 없어서 파괴 불가 시
        if (cardList.Count == 0)
        {
            var texts = GetChaosLocaleTexts();
            chaosTxt.SetText(texts.noCard);
            return;
        }

        string destroyCardID = cardList[Random.Range(0, cardList.Count)];

        int cnt = StaticManager.allDeckList.FindAll(x => x == destroyCardID).Count;
        StaticManager.allDeckList.RemoveAll(x => x == destroyCardID);
        DeckManager.instance.DeckSort();

        int idx = CardReward.instance.rewardList.FindIndex(x => x.ID == destroyCardID);
        CardReward.instance.rewardList[idx].probability = 0f;
        CardReward.instance.SetProbability();

        StaticManager.eventDOTWeenCnt++;
        EventCardDrop.instance.exitBtn.interactable = false;

        StartCoroutine(DestroyCardRoutine(destroyCardID, cnt));
    }

    readonly WaitForSeconds wait = new(0.2f);
    IEnumerator DestroyCardRoutine(string ID, int cnt)
    {
        string cardPath = DeckManager.instance.GetCardTypeAndResourcesPath(ID, out _, out GameObject cardFront);

        for (int i = 0; i < cnt; i++) {
            GameObject cardFrontObj = Instantiate(cardFront, DeckManager.instance.deckObj.transform);
            Instantiate(Resources.Load<GameObject>(cardPath), cardFrontObj.transform);

            cardFrontObj.transform.DOMove(Vector2.zero, 1.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    AudioManager.instance.PlaySfx(destroyClip);

                    Destroy(cardFrontObj);
                });

            yield return wait;
        }

        if (--StaticManager.eventDOTWeenCnt == 0)
            EventCardDrop.instance.exitBtn.interactable = true;

        GameManager.instance.SetGold(1000);
        GameManager.instance.SetCost(20);
        GameManager.instance.SetPlayerHP(++StaticManager.curHP);

        var textsEnd = GetChaosLocaleTexts();
        chaosTxt.SetText(textsEnd.final);

        SteamAchievement.Unlock("드래곤과의 만남");
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetDestroyFragmentLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        SetText();
        OKBtn.interactable = true;

        // 로케일 변경 시 텍스트 갱신을 위해 이벤트 구독
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        // 창이 활성화된 경우에만 갱신
        if (gameObject.activeInHierarchy)
            SetText();

        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetDestroyFragmentLocaleText());
    }

    private ChaosLocaleTexts GetChaosLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        if (code.StartsWith("ko"))
        {
            return new ChaosLocaleTexts
            {
                initial = "현재 보유 조각 중 랜덤한 한 종류의 조각을 모두 파괴하고 앞으로 등장하지 않는 조건으로 생명 하나와 20코스트, 1000G를 주겠노라.\n(이벤트 조각은 제외)",
                noCard = "파괴할 조각이 남아 있지 않군.",
                final = "만족스러운 거래였길..."
            };
        }
        if (code.StartsWith("en"))
        {
            return new ChaosLocaleTexts
            {
                initial = "I will destroy all pieces of one random type you own and ban them from future appearances — in return I grant one life, 20 cost, and 1000G.\n(Event pieces excluded.)",
                noCard = "There are no pieces to destroy.",
                final = "May the deal be satisfactory..."
            };
        }
        if (code.StartsWith("fr"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Je détruirai toutes les pièces d’un type aléatoire que vous possédez et les empêcherai d’apparaître à l’avenir — en échange, je vous accorde une vie, 20 de coût et 1000G.\n(Les pièces d’événement sont exclues.)",
                noCard = "Il ne reste aucune pièce à détruire.",
                final = "Que cet échange soit à votre satisfaction..."
            };
        }
        if (code.StartsWith("it"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Distruggerò tutte le pezze di un tipo casuale che possiedi e impedirò loro di ricomparire in futuro — in cambio ti concedo una vita, 20 di costo e 1000G.\n(Pezze evento escluse.)",
                noCard = "Non ci sono pezze da distruggere.",
                final = "Spero che l'accordo sia soddisfacente..."
            };
        }
        if (code.StartsWith("de"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Ich werde alle Stücke eines zufälligen Typs, die du besitzt, vernichten und dafür sorgen, dass sie künftig nicht mehr erscheinen — im Gegenzug erhältst du ein Leben, 20 Kosten und 1000G.\n(Ereignisstücke ausgenommen.)",
                noCard = "Es sind keine Stücke zum Zerstören vorhanden.",
                final = "Möge der Handel zu deiner Zufriedenheit sein..."
            };
        }
        if (code.StartsWith("es"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Destruiré todas las piezas de un tipo aleatorio que poseas y las impediré de aparecer en el futuro — a cambio te concedo una vida, 20 de coste y 1000G.\n(Piezas de evento excluidas.)",
                noCard = "No quedan piezas para destruir.",
                final = "Que el trato sea satisfactorio..."
            };
        }
        if (code.StartsWith("ja"))
        {
            return new ChaosLocaleTexts
            {
                initial = "所持している断片のうちランダムで1種類の断片を全て破壊し、今後出現しない条件で命1、コスト20、1000Gを与えよう。\n（イベント断片は除く）",
                noCard = "破壊する断片が残っていません。",
                final = "満足のいく取引であったことを願う..."
            };
        }
        if (code.StartsWith("pt"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Destruirei todas as peças de um tipo aleatório que você possui e impedirei que reapareçam no futuro — em troca concedo uma vida, 20 de custo e 1000G.\n(Peças de evento excluídas.)",
                noCard = "Não há peças para destruir.",
                final = "Que o acordo seja satisfatório..."
            };
        }
        if (code.StartsWith("ru"))
        {
            return new ChaosLocaleTexts
            {
                initial = "Я уничтожу все фрагменты одного случайного типа из тех, что у вас есть, и запрету их появление в будущем — взамен дарую одну жизнь, 20 очков стоимости и 1000G.\n(Фрагменты событий не учитываются.)",
                noCard = "Нет фрагментов для уничтожения.",
                final = "Пусть сделка будет к вашему удовлетворению..."
            };
        }
        if (code.StartsWith("zh"))
        {
            return new ChaosLocaleTexts
            {
                initial = "我将摧毁你拥有的某一种随机碎片，并禁止其今后出现——作为回报，我给予你一条生命、20点费用和1000G。\n（事件碎片除外）",
                noCard = "没有可摧毁的碎片。",
                final = "愿这笔交易令你满意…"
            };
        }

        // fallback (영어)
        return new ChaosLocaleTexts
        {
            initial = "I will destroy all pieces of one random type you own and ban them from future appearances — in return I grant one life, 20 cost, and 1000G.\n(Event pieces excluded.)",
            noCard = "There are no pieces to destroy.",
            final = "May the deal be satisfactory..."
        };
    }

    private string GetDestroyFragmentLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "보유 중인 조각 중 랜덤한 한 종류의 조각을 모두 파괴하고,\n이번 모험에서 영구적으로 등장하지 않게 합니다. (이벤트 조각 제외)\n보상으로 1 생명, 20 코스트, 1000G를 받습니다.";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "Destroys all fragments of one random type you own and removes them permanently from this adventure. (Event fragments excluded)\nAs a reward, you gain 1 life, 20 cost, and 1000G.";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Détruit tous les fragments d'un type aléatoire que vous possédez et les supprime définitivement de cette aventure. (Fragments d'événement exclus)\nEn récompense, vous obtenez 1 vie, 20 coûts et 1000G.";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Distrugge tutti i frammenti di un tipo casuale in tuo possesso e li rimuove permanentemente da questa avventura. (Frammenti evento esclusi)\nCome ricompensa, ottieni 1 vita, 20 costo e 1000G.";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Zerstört alle Fragmente eines zufälligen Typs, den Sie besitzen, und entfernt sie dauerhaft aus diesem Abenteuer. (Ereignisfragmente ausgeschlossen)\nAls Belohnung erhalten Sie 1 Leben, 20 Kosten und 1000G.";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "Destruye todos los fragmentos de un tipo aleatorio que posees y los elimina permanentemente de esta aventura. (Fragmentos de evento excluidos)\nComo recompensa, obtienes 1 vida, 20 coste y 1000G.";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "所持している断片の中からランダムで1種類をすべて破壊し、\n今回の冒険中は永久に出現しなくなります。（イベント断片を除く）\n報酬として、ライフ1、コスト20、1000Gを獲得します。";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Destrói todos os fragmentos de um tipo aleatório que você possui e os remove permanentemente desta aventura. (Fragmentos de evento excluídos)\nComo recompensa, você recebe 1 vida, 20 custo e 1000G.";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Уничтожает все фрагменты одного случайного типа, которыми вы владеете, и навсегда убирает их из этого приключения. (Фрагменты событий не учитываются)\nВ награду вы получаете 1 жизнь, 20 стоимости и 1000G.";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "随机摧毁你所持有的一种碎片的全部数量，并在本次冒险中永久不再出现。（不包含事件碎片）\n作为奖励，你将获得1点生命、20费用和1000G。";

        // fallback: English
        return "Destroys all fragments of one random type you own and removes them permanently from this adventure. (Event fragments excluded)\nAs a reward, you gain 1 life, 20 cost, and 1000G.";
    }
}