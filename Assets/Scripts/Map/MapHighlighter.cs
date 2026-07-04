using System.Collections.Generic;
using UnityEngine;

public class MapHighlighter : MonoBehaviour
{
    public static MapHighlighter Instance;

    public GameObject node;

    private readonly HashSet<MapInfo> visited = new();
    private MapInfo currentHighlightedRoot = null;

    private void Awake()
    {
        Instance = this;
    }

    public void HighlightFrom(MapInfo start)
    {
        if (currentHighlightedRoot == start)
            return;

        ResetAll();
        currentHighlightedRoot = start;
        visited.Clear();
        DFS(start);
    }

    void DFS(MapInfo node)
    {
        if (visited.Contains(node)) return;

        visited.Add(node);
        node.Highlight();

        foreach (GameObject nextObj in node.toButton) {
            if (nextObj.TryGetComponent<MapInfo>(out var next)) {
                DFS(next);
            }
        }
    }

    public void ResetAll()
    {
        MapInfo[] all = node.GetComponentsInChildren<MapInfo>();
        foreach (var node in all) {
            node.Reset();
        }

        visited.Clear();
        currentHighlightedRoot = null;
    }
}