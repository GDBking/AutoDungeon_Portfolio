using UnityEngine;
using UnityEngine.EventSystems;

public class SPUMOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SpriteRenderer[] renderers;
    Color[] originalColors;

    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    public void PointerEnter()
    {
        foreach (var r in renderers) {
            if (r == null)
                continue;
            Color c = r.color;
            c.r *= 0.6f;
            c.g *= 0.6f;
            c.b *= 0.6f;
            r.color = c;
        }
    }

    public void PointerExit()
    {
        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] == null)
                continue;
            renderers[i].color = originalColors[i]; // º¹¿ø
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit();
    }
}
