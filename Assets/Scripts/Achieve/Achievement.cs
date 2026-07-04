using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    public AchievementData achieveData;

    TextMeshProUGUI title;
    TextMeshProUGUI desc;

    Image icon;
    GameObject checkIcon;

    private void Awake()
    {
        title = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        desc = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        icon = transform.GetChild(0).GetComponent<Image>();
        checkIcon = transform.GetChild(3).gameObject;
    }

    private void Start()
    {
        SteamUserStats.GetAchievement(achieveData.Title, out bool achieved);

        if (achieved) {
            AchieveCntTxt.instance.cnt++;
            AchieveCntTxt.instance.SetTxt();

            icon.sprite = achieveData.Icon;
            checkIcon.SetActive(true);
        }
    }

    private void OnEnable()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        switch (code[..2]) {
            case "ko":
                title.SetText(achieveData.Title);
                desc.SetText(achieveData.Desc);
                break;
            case "en":
                title.SetText(achieveData.ENTitle);
                desc.SetText(achieveData.ENDesc);
                break;
            case "fr":
                title.SetText(achieveData.FRTitle);
                desc.SetText(achieveData.FRDesc);
                break;
            case "it":
                title.SetText(achieveData.ITTitle);
                desc.SetText(achieveData.ITDesc);
                break;
            case "de":
                title.SetText(achieveData.DETitle);
                desc.SetText(achieveData.DEDesc);
                break;
            case "es":
                title.SetText(achieveData.ESTitle);
                desc.SetText(achieveData.ESDesc);
                break;
            case "ja":
                title.SetText(achieveData.JATitle);
                desc.SetText(achieveData.JADesc);
                break;
            case "pt":
                title.SetText(achieveData.PT_BRTitle);
                desc.SetText(achieveData.PT_BRDesc);
                break;
            case "ru":
                title.SetText(achieveData.RUTitle);
                desc.SetText(achieveData.RUDesc);
                break;
            case "zh":
                title.SetText(achieveData.ZH_HANSTitle);
                desc.SetText(achieveData.ZH_HANSDesc);
                break;
            default:
                title.SetText(achieveData.ENTitle);
                desc.SetText(achieveData.ENDesc);
                break;
        }
    }
}
