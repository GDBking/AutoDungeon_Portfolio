using System;
using UnityEngine;
using UnityEngine.UI;

public class RemainingTime : MonoBehaviour
{
    Image imgComp;
    [NonSerialized] public float totalTime;
    [NonSerialized] public float time;

    private void Awake()
    {
        imgComp = GetComponent<Image>();
    }

    void Update()
    {
        if (totalTime == -2f) {
            imgComp.fillAmount = 0f;
            this.enabled = false;
        }
        else {
            time += Time.deltaTime;
            imgComp.fillAmount = time / totalTime;

            if (imgComp.fillAmount == 1f)
                Destroy(transform.parent.gameObject);
        }
    }
}