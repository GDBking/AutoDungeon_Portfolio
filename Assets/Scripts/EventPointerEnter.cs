using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventPointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    EventItem eventItemComp;
    GameObject unitInfo;
    GameObject itemInfo;
    GameObject eventInfo;
    GameObject relicsInfo;
    List<TextMeshProUGUI> eventTxt;

    Image imageComp;

    private void Awake()
    {
        eventItemComp = GetComponent<EventItem>();
        unitInfo = GameManager.instance.unitInfo;
        itemInfo = GameManager.instance.itemInfo;
        eventInfo = GameManager.instance.eventInfo;
        relicsInfo = GameManager.instance.relicsInfo;
        eventTxt = GameManager.instance.eventTxt;

        imageComp = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageComp.color = Color.gray;

        unitInfo.SetActive(false);
        itemInfo.SetActive(false);
        relicsInfo.SetActive(false);

        eventTxt[0].SetText(eventItemComp.GetLocalizedName());
        eventTxt[1].SetText(eventItemComp.GetLocalizedDesc());

        eventInfo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageComp.color = Color.white;
    }
}