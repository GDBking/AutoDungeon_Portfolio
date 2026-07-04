using UnityEngine;
using UnityEngine.EventSystems;

public class DealMetricsPointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnitDefault unitComp;
    public SPUMOutline outlineComp;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unitComp == null)
            return;

        unitComp.UpdateStatInfo(true);
        outlineComp.PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineComp == null)
            return;

        outlineComp.PointerExit();
    }
}