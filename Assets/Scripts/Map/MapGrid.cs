using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MapList
{
    public List<GameObject> list = new();
}

public class MapGrid : MonoBehaviour
{
    public static MapGrid instance;

    [System.NonSerialized] public int maxLevel = 13;
    public Image mapBGImg;
    public List<Sprite> mapBGSprite;
    public List<GameObject> monsterRoom;
    public List<GameObject> monsterRoomElite;
    public GameObject shopRoom;
    public GameObject eventRoom;
    public List<GameObject> bossRoom;
    public List<GameObject> linePrefab;
    public ScrollRect scrollRect;

    private GameObject map;
    const int x = 50;
    const int y = 100;

    [HideInInspector] public List<MapList> mapList = new();
    readonly int[] randRemove = { 2, 3, 3, 3, 3, 4, 4, 4, 4, 5 };

    Transform node;
    Transform line;

    void Awake() => instance = this;

    void Start()
    {
        node = transform.GetChild(0);
        line = transform.GetChild(1);

        for (int i = 0; i < maxLevel; i++)
            mapList.Add(new MapList());

        GenerateMap();
        DrawAllLines();
    }

    void GenerateMap()
    {
        if (StaticManager.curStage == 1)
            StaticManager.mapSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(StaticManager.mapSeed);

        mapBGImg.sprite = mapBGSprite[StaticManager.curStage - 1];
        int roomPerLayer = 7;

        // --- 1~14Ãþ »ý¼º ---
        for (int i = 0; i < maxLevel - 1; i++) {
            mapList[i].list.Clear();
            for (int j = 0; j < roomPerLayer; j++) {
                int a = Random.Range(-30, 31);
                int b = Random.Range(-30, 31);
                int ran = Random.Range(1, 106);

                if (i == 0) {
                    map = Instantiate(monsterRoom[StaticManager.curStage - 1], node);
                    map.GetComponent<RectTransform>().localScale = Vector2.one * 1.5f;
                    map.GetComponent<Button>().interactable = true;
                }
                else if (i == maxLevel / 2 - 1 || i == maxLevel - 2) map = Instantiate(shopRoom, node);
                else if (ran <= 50) map = Instantiate(monsterRoom[StaticManager.curStage - 1], node);
                else if (ran <= 80) map = Instantiate(monsterRoomElite[StaticManager.curStage - 1], node);
                else map = Instantiate(eventRoom, node);

                map.GetComponent<RectTransform>().anchoredPosition = new Vector2(j * 150f + x + a, i * 300f + y + b);
                mapList[i].list.Add(map);
            }

            // ·£´ý Á¦°Å (1, 7, 14Ãþ Á¦¿Ü)
            if (i != 0 && i != maxLevel / 2 - 1 && i != maxLevel - 2) {
                int r = randRemove[Random.Range(0, randRemove.Length)];
                int[] numbers = Enumerable.Range(0, mapList[i].list.Count).ToArray();
                Shuffle(numbers);
                for (int j = 0; j < r && j < mapList[i].list.Count; j++) {
                    Destroy(mapList[i].list[numbers[j]]);
                    mapList[i].list[numbers[j]] = null;
                }
                ListRemove();
            }
        }

        // Ãþ °¹¼ö ¸ÂÃß±â 1:1
        SyncLayerCount(0, 1);
        SyncLayerCount(maxLevel / 2 - 1, maxLevel / 2);
        SyncLayerCount(maxLevel - 2, maxLevel - 3);

        // --- ¸¶Áö¸· Ãþ º¸½º ---
        mapList[maxLevel - 1].list.Clear();
        map = Instantiate(bossRoom[StaticManager.curStage - 1], node);
        map.GetComponent<RectTransform>().anchoredPosition = new Vector2(400f, 300f * maxLevel - 200f);
        mapList[maxLevel - 1].list.Add(map);

        scrollRect.verticalNormalizedPosition = 0f;
    }

    void ListRemove()
    {
        foreach (var map in mapList)
            map.list.RemoveAll(stage => stage == null);
    }

    void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--) {
            int randIndex = Random.Range(0, i + 1);
            (array[i], array[randIndex]) = (array[randIndex], array[i]);
        }
    }

    void SyncLayerCount(int idxA, int idxB)
    {
        int minCount = mapList[idxB].list.Count;

        while (mapList[idxA].list.Count > minCount) {
            int randNum = Random.Range(0, mapList[idxA].list.Count);
            Destroy(mapList[idxA].list[randNum]);
            mapList[idxA].list.RemoveAt(randNum);
        }
    }

    readonly Dictionary<GameObject, bool> connectedNodes = new();

    void MarkConnected(GameObject node)
    {
        if (!connectedNodes.ContainsKey(node))
            connectedNodes[node] = true;
    }

    bool IsConnected(GameObject node)
    {
        return connectedNodes.ContainsKey(node) && connectedNodes[node];
    }

    void DrawAllLines()
    {
        connectedNodes.Clear();

        for (int i = 0; i < mapList.Count - 1; i++) {
            var currentLayer = mapList[i].list;
            var nextLayer = mapList[i + 1].list;

            if (currentLayer.Count == 0 || nextLayer.Count == 0) continue;

            // Ãþ 1:1 ¿¬°á
            if (i == 0 || i == maxLevel / 2 - 1 || i == maxLevel - 3) {
                int count = Mathf.Min(currentLayer.Count, nextLayer.Count);
                for (int j = 0; j < count; j++)
                    DrawUILine(currentLayer[j], nextLayer[j]);
                continue;
            }

            // 14Ãþ ¡ê º¸½ºÃþ ¸ðµç ¿¬°á
            if (i == maxLevel - 2) {
                GameObject boss = nextLayer[0];
                foreach (var from in currentLayer)
                    DrawUILine(from, boss);
                continue;
            }

            // ÀÏ¹Ý ·£´ý ¿¬°á
            foreach (var from in currentLayer) {
                List<GameObject> sortedNext = nextLayer.OrderBy(n =>
                    Mathf.Abs(n.GetComponent<RectTransform>().anchoredPosition.x -
                              from.GetComponent<RectTransform>().anchoredPosition.x)).ToList();

                DrawUILine(from, sortedNext[0]);
                MarkConnected(sortedNext[0]);
            }

            foreach (var to in nextLayer) {
                if (!IsConnected(to)) {
                    GameObject from = currentLayer.OrderBy(f =>
                        Mathf.Abs(f.GetComponent<RectTransform>().anchoredPosition.x -
                                  to.GetComponent<RectTransform>().anchoredPosition.x)).First();

                    DrawUILine(from, to);
                    MarkConnected(to);
                }
            }
        }

        Random.InitState(System.DateTime.Now.ToString("yyyyMMddHHmmssfff").GetHashCode());
    }

    void DrawUILine(GameObject from, GameObject to)
    {
        RectTransform fromRT = from.GetComponent<RectTransform>();
        RectTransform toRT = to.GetComponent<RectTransform>();

        Vector2 start = fromRT.localPosition + Vector3.up * 50f;
        Vector2 end = toRT.localPosition + Vector3.down * 50f;

        Vector2 dir = end - start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float length = dir.magnitude;

        RectTransform lineRT = Instantiate(linePrefab[StaticManager.curStage - 1], line).GetComponent<RectTransform>();
        lineRT.sizeDelta = new Vector2(length - length % 32, 32f);
        lineRT.localPosition = start + (end - start) / 2f;
        lineRT.localRotation = Quaternion.Euler(0, 0, angle);

        MapInfo fromInfo = from.GetComponent<MapInfo>();
        fromInfo.toButton.Add(to);
        fromInfo.toLine.Add(lineRT.gameObject);
    }

    public void NextStage()
    {
        foreach (Transform map in node) {
            Destroy(map.gameObject);
        }

        foreach (Transform line2 in line) {
            Destroy(line2.gameObject);
        }

        GenerateMap();
        DrawAllLines();
    }
}
