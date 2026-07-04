using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    public Texture2D cursorTexture; // 바꿀 커서 이미지
    public Vector2 hotspot = Vector2.zero; // 클릭 좌표 기준점
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
