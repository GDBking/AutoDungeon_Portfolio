using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SetUnitLayer : MonoBehaviour
{
    SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = transform.GetChild(0).GetComponent<SortingGroup>();
        
        StartCoroutine(SetLayer());
    }

    IEnumerator SetLayer()
    {
        yield return null;

        Canvas pCanvas = transform.parent.GetComponentInParent<Canvas>();
        int sortingID = pCanvas.sortingLayerID;

        int sortingOrder;
        if (transform.parent.CompareTag("Field") || transform.parent.CompareTag("Enemy Field"))
            sortingOrder = Mathf.RoundToInt(transform.localPosition.y * -1f);
        else
            sortingOrder = pCanvas.sortingOrder + 1;

        sortingGroup.sortingLayerID = sortingID;
        sortingGroup.sortingOrder = sortingOrder;
    }

    private void OnTransformParentChanged()
    {
        StartCoroutine(SetLayer());
    }
}