using UnityEngine;
using UnityEngine.EventSystems;

public class PointerSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlaySfx(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlaySfx(clickSound);
    }
}