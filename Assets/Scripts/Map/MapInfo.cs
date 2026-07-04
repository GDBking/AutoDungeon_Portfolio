using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum MapType { monster, monsterElite, shop, eventRoom, boss }
    public MapType mapType;
    public List<GameObject> toButton;
    public List<GameObject> toLine;

    public AudioClip btnHoverSound;
    public AudioClip btnClickSound;

    Button button;
    ColorBlock buttonOriginalColor;

    // 하이라이트/비활성 색상 정의 (알파 포함)
    Color highlightColor = Color.white;
    Color disableColor = new(1f, 1f, 1f, 0.8f);  // 흰색, alpha 0.5

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonOriginalColor = button.colors;

        // 클릭 이벤트
        button.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySfx(btnClickSound);
            GameManager.instance.toButton = toButton;
            RemainingDeck.instance.gameObject.SetActive(false);
        });

        // 방 타입별 클릭 이벤트
        switch (mapType) {
            case MapType.monster:
                button.onClick.AddListener(() => { GetComponentInParent<MapLoad>().MonsterRoom(); });
                break;
            case MapType.monsterElite:
                button.onClick.AddListener(() => { GetComponentInParent<MapLoad>().MonsterRoomElite(); });
                break;
            case MapType.shop:
                button.onClick.AddListener(() => { GetComponentInParent<MapLoad>().ShopRoom(); });
                break;
            case MapType.eventRoom:
                button.onClick.AddListener(() => { GetComponentInParent<MapLoad>().EventRoom(); });
                break;
            case MapType.boss:
                button.onClick.AddListener(() => { GetComponentInParent<MapLoad>().BossRoom(); });
                break;
        }
    }

    // 마우스 오버
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        MapHighlighter.Instance.HighlightFrom(this);
        AudioManager.instance.PlaySfx(btnHoverSound);

        // 호버 키기
        transform.GetChild(0).gameObject.SetActive(true);
    }

    // 호버 끄기
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // 하이라이트 처리
    public void Highlight()
    {
        ColorBlock cb = button.colors;
        cb.disabledColor = disableColor;
        button.colors = cb;

        foreach (GameObject line in toLine) {
            line.transform.SetAsLastSibling();
            Animator anim = line.GetComponent<Animator>();
            anim.enabled = true;
            Image img = line.GetComponent<Image>();
            img.color = highlightColor; // alpha = 1
        }
    }

    // 하이라이트 해제 / 기본 상태로 리셋
    public void Reset()
    {
        button.colors = buttonOriginalColor;

        foreach (GameObject line in toLine) {
            Animator anim = line.GetComponent<Animator>();
            anim.enabled = false;
            Image img = line.GetComponent<Image>();
            Color c = img.color;
            c.a = 0.3f;
            img.color = c;
        }

        // 호버 끄기
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // 버튼 클릭 시 처리(호버 끄기 및 라인 비활성화)
    public void BtnClick()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        
        button.colors = buttonOriginalColor;

        foreach (GameObject line in toLine)
        {
            Animator anim = line.GetComponent<Animator>();
            anim.enabled = false;
            Image img = line.GetComponent<Image>();
            Color c = img.color;
            c.a = 0.3f;
            img.color = c;
        }
    }
}