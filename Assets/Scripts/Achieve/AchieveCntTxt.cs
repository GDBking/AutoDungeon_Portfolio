using TMPro;
using UnityEngine;

public class AchieveCntTxt : MonoBehaviour
{
    public static AchieveCntTxt instance;

    public TextMeshProUGUI cntTxt;

    [HideInInspector] public int cnt;

    private void Awake()
    {
        instance = this;
    }

    public void SetTxt()
    {
        cntTxt.SetText($"{cnt}/37");
    }
}
