using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    UnitDefault unitComp;

    private void Awake()
    {
        unitComp = GetComponent<UnitDefault>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unitComp.UpdateStatInfo(true);
    }
}
