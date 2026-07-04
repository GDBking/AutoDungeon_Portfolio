using TMPro;
using UnityEngine;

public class AchievePointTxt : MonoBehaviour
{
    public static AchievePointTxt instance;

    public TextMeshProUGUI pointTxt;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetAchiPointTxt();
    }

    public void SetAchiPointTxt()
    {
        pointTxt.SetText($"{SaveController.instance.saveDataAchi.achiPoint}");
        SaveController.instance.OnSaveDataAchi();
    }
}
