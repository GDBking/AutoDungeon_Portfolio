using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform draggingCavas;
    CanvasGroup canvasGroup;
    protected Transform beginParent;
    Vector2 beginDragPosition;

    protected virtual void Awake()
    {
        draggingCavas = GameObject.Find("DraggingCanvas").transform;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        beginParent = transform.parent;
        transform.SetParent(draggingCavas);
        beginDragPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    protected void InitPos()
    {
        transform.SetParent(beginParent);
        transform.position = beginDragPosition;
    }
}