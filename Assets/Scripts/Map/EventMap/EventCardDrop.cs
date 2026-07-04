using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCardDrop : MonoBehaviour
{
    public static EventCardDrop instance;

    public List<GameObject> eventCardPrefabList;
    public RectTransform eventBoxZone;
    public GameObject eventCardBoxPrefab;
    public Sprite boxOpenSprite;
    public Button exitBtn;

    [HideInInspector] public GameObject activePanel;

    int eventCardAmount = 2;

    private void Awake()
    {
        instance = this;

        if (SaveController.instance.saveDataAchi.isCheck[6]) {
            eventCardAmount++;

            if (SaveController.instance.saveDataAchi.isCheck[7])
                eventCardAmount++;
        }
    }

    public void EventBoxSpawn()
    {
        int randNum = Random.Range(1, eventCardAmount);
        for (int i = 0; i < randNum; i++) {
            Vector2 randPos = RandomPositionManager.instance.GetRandomFieldPos(eventBoxZone);
            GameObject box = Instantiate(eventCardBoxPrefab, eventBoxZone);
            box.GetComponent<RectTransform>().anchoredPosition = randPos;
        }
    }

    public void EventBoxClear()
    {
        foreach (Transform eventBox in eventBoxZone) {
            Destroy(eventBox.gameObject);
        }
    }

    public void ExitBtnClick()
    {
        exitBtn.gameObject.SetActive(false);
        activePanel.SetActive(false);
        EventBoxClear();

        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(false);

        GameManager.instance.UpdateStageUI(++GameManager.instance.currentLevel);
        MapLoad.instance.mapCan.SetActive(true);
    }
}
