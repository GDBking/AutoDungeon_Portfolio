using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { HPPotion, ATKPotion, DEFPotion, DPSPotion,
                           LSPotion, CRTPotion }

    [Header("아이템 정보")]
    public ItemType itemType;
    public int itemLevel;

    [Header("한국어")]
    public string itemName;
    public string itemDesc;

    [Header("영어")]
    public string ENItemName;
    public string ENItemDesc;

    [Header("프랑스어")]
    public string FRItemName;
    public string FRItemDesc;

    [Header("이탈리아어")]
    public string ITItemName;
    public string ITItemDesc;

    [Header("독일어")]
    public string DEItemName;
    public string DEItemDesc;

    [Header("스페인어")]
    public string ESItemName;
    public string ESItemDesc;

    [Header("일본어")]
    public string JAItemName;
    public string JAItemDesc;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRItemName;
    public string PT_BRItemDesc;

    [Header("러시아어")]
    public string RUItemName;
    public string RUItemDesc;

    [Header("중국어 (간체)")]
    public string ZH_HANSItemName;
    public string ZH_HANStemDesc;

    public Sprite itemIcon;
    public int cost;

    [Header("아이템 ID")]
    public string itemID;

    [Header("포션 능력")]
    public int heal;    // 힐
    public int ATK;     // 공격력
    public int DEF;     // 방어력
    public float DPS;     // 공격 속도
    public int LS;      // 피해 흡혈
    public int CRT;     // 크리티컬

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

        if (code.StartsWith("ko")) return itemDesc;
        if (code.StartsWith("en")) return ENItemDesc;
        if (code.StartsWith("fr")) return FRItemDesc;
        if (code.StartsWith("it")) return ITItemDesc;
        if (code.StartsWith("de")) return DEItemDesc;
        if (code.StartsWith("es")) return ESItemDesc;
        if (code.StartsWith("ja")) return JAItemDesc;
        if (code.StartsWith("pt")) return PT_BRItemDesc;
        if (code.StartsWith("ru")) return RUItemDesc;
        if (code.StartsWith("zh")) return ZH_HANStemDesc;

        return ENItemDesc;
    }
}