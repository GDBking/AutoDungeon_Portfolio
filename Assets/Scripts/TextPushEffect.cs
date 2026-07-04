using UnityEngine;
using UnityEngine.UI;

public class TextPushEffect : MonoBehaviour
{
    public RectTransform textRect; // 자식 Text
    public Vector3 pressedOffset = new Vector3(0, -2, 0); // 눌렸을 때 Y값 이동
    private Vector3 originalPos;

    void Start()
    {
        if (textRect != null)
            originalPos = textRect.localPosition;
    }

    public void OnButtonPressed()
    {
        if(gameObject.GetComponent<Button>().interactable == false)
            return;
        if (textRect != null)
            textRect.localPosition = originalPos + pressedOffset;
    }

    public void OnButtonReleased()
    {
        if (gameObject.GetComponent<Button>().interactable == false)
            return;
        if (textRect != null)
            textRect.localPosition = originalPos;
    }
}