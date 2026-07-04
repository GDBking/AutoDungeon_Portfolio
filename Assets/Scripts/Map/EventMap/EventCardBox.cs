using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EventCardBox : MonoBehaviour
{
    public Image boxImg;
    public Button btn;

    public AudioClip getCardClip;

    public void OnClickBox()
    {
        btn.interactable = false;

        StaticManager.eventDOTWeenCnt++;

        EventCardDrop.instance.exitBtn.interactable = false;

        boxImg.sprite = EventCardDrop.instance.boxOpenSprite;
        GameObject cardFrontObj = Instantiate(DeckManager.instance.cardFrontList[2], transform);
        GameObject contents = Instantiate(EventCardDrop.instance.eventCardPrefabList[Random.Range(0, EventCardDrop.instance.eventCardPrefabList.Count)], cardFrontObj.transform);
        contents.GetComponent<EventDrag>().enabled = false;

        RectTransform rect = cardFrontObj.GetComponent<RectTransform>();
        float targetY = rect.position.y + 115f;

        rect.DOAnchorPosY(targetY, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => GetDeck(cardFrontObj.transform, contents.GetComponent<EventItem>().ID));
    }

    void GetDeck(Transform cardFront, string ID)
    {
        cardFront.SetParent(DeckManager.instance.deckObj.transform);

        cardFront.DOMove(DeckManager.instance.deckObj.transform.position, DeckManager.instance.cardAnimList[0].GetComponent<AnimationToImage>().GetAnimationLength())
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                AudioManager.instance.PlaySfx(getCardClip);

                StaticManager.allDeckList.Add(ID);
                DeckManager.instance.DeckSort();

                if (--StaticManager.eventDOTWeenCnt == 0)
                    EventCardDrop.instance.exitBtn.interactable = true;

                Destroy(cardFront.gameObject);
                Destroy(gameObject);
            });
    }
}
