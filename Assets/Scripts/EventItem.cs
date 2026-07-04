using UnityEngine;
using UnityEngine.Localization.Settings;

public class EventItem : MonoBehaviour
{
    public enum EventItemType { cost, reroll, steroid, gold, mercenary, sacrifice, masterKey }

    public EventItemType type;
    public int money;
    public string ID;

    [Header("한국어")]
    public string itemName;
    [TextArea] public string desc;

    [Header("영어")]
    public string ENItemName;
    [TextArea] public string ENDesc;

    [Header("프랑스어")]
    public string FRItemName;
    [TextArea] public string FRDesc;

    [Header("이탈리아어")]
    public string ITItemName;
    [TextArea] public string ITDesc;

    [Header("독일어")]
    public string DEItemName;
    [TextArea] public string DEDesc;

    [Header("스페인어")]
    public string ESItemName;
    [TextArea] public string ESDesc;

    [Header("일본어")]
    public string JAItemName;
    [TextArea] public string JADesc;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRItemName;
    [TextArea] public string PT_BRDesc;

    [Header("러시아어")]
    public string RUItemName;
    [TextArea] public string RUDesc;

    [Header("중국어 (간체)")]
    public string ZH_HANSItemName;
    [TextArea] public string ZH_HANSDesc;

    public string GetLocalizedName()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        if (code.StartsWith("ko")) return itemName;
        if (code.StartsWith("en")) return ENItemName;
        if (code.StartsWith("fr")) return FRItemName;
        if (code.StartsWith("it")) return ITItemName;
        if (code.StartsWith("de")) return DEItemName;
        if (code.StartsWith("es")) return ESItemName;
        if (code.StartsWith("ja")) return JAItemName;
        if (code.StartsWith("pt")) return PT_BRItemName;
        if (code.StartsWith("ru")) return RUItemName;
        if (code.StartsWith("zh")) return ZH_HANSItemName;

        return ENItemName;
    }

    public string GetLocalizedDesc()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        if (code.StartsWith("ko")) return desc;
        if (code.StartsWith("en")) return ENDesc;
        if (code.StartsWith("fr")) return FRDesc;
        if (code.StartsWith("it")) return ITDesc;
        if (code.StartsWith("de")) return DEDesc;
        if (code.StartsWith("es")) return ESDesc;
        if (code.StartsWith("ja")) return JADesc;
        if (code.StartsWith("pt")) return PT_BRDesc;
        if (code.StartsWith("ru")) return RUDesc;
        if (code.StartsWith("zh")) return ZH_HANSDesc;

        return ENDesc;
    }
}