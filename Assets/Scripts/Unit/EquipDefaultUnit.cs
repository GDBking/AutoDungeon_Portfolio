/*using UnityEngine;
using UnityEngine.EventSystems;

public class EquipDefaultUnit : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // 아군 유닛이 드랍되면(유닛 카드 사용)
        if (eventData.pointerDrag.CompareTag("Friendly Unit") && eventData.pointerDrag.TryGetComponent<UnitDefault>(out UnitDefault unitComp) && !unitComp.isEquipmentUnit) {
            // 드래그된 유닛의 부모 오브젝트를 바꿔주고 기본 유닛과 교체
            eventData.pointerDrag.transform.SetParent(transform.parent);
            eventData.pointerDrag.transform.position = transform.position;
            eventData.pointerDrag.GetComponent<UnitDefault>().isEquipmentUnit = true;

            // 드래그된 유닛의 기존 부모 오브젝트(빈 카드) 파괴
            Transform previousParent = eventData.pointerDrag.GetComponent<Drag2>().previousParent;
            Destroy(previousParent.gameObject);
            // 새로운 카드 드로우
            DeckManager.instance.CardDraw(previousParent.parent);

            // 기본 유닛 파괴
            Destroy(gameObject);
        }
    }
}
*/