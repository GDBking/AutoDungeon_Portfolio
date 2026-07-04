using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Item itemComp;
    GameObject unitInfo;
    GameObject itemInfo;
    GameObject eventInfo;
    GameObject relicsInfo;
    List<TextMeshProUGUI> itemTxt;

    Image imageComp;

    private void Awake()
    {
        itemComp = GetComponent<Item>();
        eventInfo = GameManager.instance.eventInfo;
        relicsInfo = GameManager.instance.relicsInfo;

        imageComp = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageComp.color = Color.gray;

        SetItemInfo();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageComp.color = Color.white;
    }

    public void SetItemInfo(bool isUpgrade = false)
    {
        if (isUpgrade)
        {
            unitInfo = GameManager.instance.upUnitInfo;
            itemInfo = GameManager.instance.upItemInfo;
            itemTxt = GameManager.instance.upItemTxt;
        }
        else
        {
            unitInfo = GameManager.instance.unitInfo;
            itemInfo = GameManager.instance.itemInfo;
            itemTxt = GameManager.instance.itemTxt;
        }

        unitInfo.SetActive(false);
        eventInfo.SetActive(false);
        relicsInfo.SetActive(false);

        itemTxt[0].SetText(itemComp.data.itemLevel.ToString());
        itemTxt[1].text = itemComp.data.cost.ToString();
        itemTxt[2].SetText($"<b>{itemComp.data.GetLocalizedName()}</b>");
        itemTxt[3].SetText(itemComp.data.GetLocalizedDesc());

        itemInfo.SetActive(true);
    }
    
}
