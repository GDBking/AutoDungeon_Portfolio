using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Guide : MonoBehaviour
{
    public GuideEntry guideEntry;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    private void Awake()
    {
        title = GetComponent<TextMeshProUGUI>();
    }

    public void GuideClick()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        switch (code[..2])
        {
            case "ko":
                desc.SetText(guideEntry.Desc);
                break;
            case "en":
                desc.SetText(guideEntry.ENDesc);
                break;
            case "fr":
                desc.SetText(guideEntry.FRDesc);
                break;
            case "it":
                desc.SetText(guideEntry.ITDesc);
                break;
            case "de":
                desc.SetText(guideEntry.DEDesc);
                break;
            case "es":
                desc.SetText(guideEntry.ESDesc);
                break;
            case "ja":
                desc.SetText(guideEntry.JADesc);
                break;
            case "pt":
                desc.SetText(guideEntry.PT_BRDesc);
                break;
            case "ru":
                desc.SetText(guideEntry.RUDesc);
                break;
            case "zh":
                desc.SetText(guideEntry.ZH_HANSDesc);
                break;
            default:
                desc.SetText(guideEntry.ENDesc);
                break;
        }
    }
}