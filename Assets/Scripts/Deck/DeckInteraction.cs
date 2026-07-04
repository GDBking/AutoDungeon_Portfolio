using UnityEngine;
using UnityEngine.EventSystems;

public class DeckInteraction : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        DeckManager.instance.DeckClick();
    }
}
