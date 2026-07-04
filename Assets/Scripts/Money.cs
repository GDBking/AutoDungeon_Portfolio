/*using UnityEngine;
using UnityEngine.EventSystems;

public class Money : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int money;
    // 캔버스
    Transform canvas;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("DungeonCan").transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CompareTag("Money"))
        {
            GameManager.instance.moneyInfo.SetActive(true);

            // 스크린 좌표(eventData.position)를 UI의 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas as RectTransform,    // 기준이 될 캔버스의 RectTransform
                        new Vector2(eventData.position.x + 4, eventData.position.y - 4),         // 변환할 스크린 좌표 (마우스/터치 위치)
                        eventData.pressEventCamera, // 이벤트가 발생한 카메라 (UI가 'Screen Space - Overlay'라면 null이어야 함)
                        out Vector2 localPoint);            // 변환된 로컬 좌표를 저장할 변수

            // 변환된 좌표를 적용하여 UI(itemInfo)의 위치 변경
            GameManager.instance.moneyInfo.transform.localPosition = localPoint;

            GameManager.instance.rewardTextMoney.text = GetComponent<Money>().money.ToString() + "G";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.moneyInfo.SetActive(false);
    }
}*/