using UnityEngine;
using UnityEngine.EventSystems;

public class SetSliderSound : MonoBehaviour, IPointerUpHandler
{
    public AudioClip testSound;

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.instance.PlaySfx(testSound);
    }
}