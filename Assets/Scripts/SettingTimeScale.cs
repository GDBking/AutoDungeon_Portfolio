using UnityEngine;

public class SettingTimeScale : MonoBehaviour
{
    private void OnEnable()
    {
        if (!GameManager.instance.isEnd)
            Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        if (!GameManager.instance.isEnd)
            Time.timeScale = GameManager.instance.timeScale;
    }
}
