using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "UpgradeOptionData", menuName = "Scriptable Object/UpgradeData")]
public class UpgradeOptionData : ScriptableObject
{
    public enum UpgradeType { skill, healthPer, attackPer, defensePer, attackSpeedPer, attackRange,
                              criticalPer, lifeStealPer }
    public UpgradeType upgradeType;

    [Header("한국어")]
    public string upgradeName;
    [TextArea] public string description;

    [Header("영어")]
    public string ENupgradeName;
    [TextArea] public string ENdescription;

    [Header("프랑스어")]
    public string FRUpgradeName;
    [TextArea] public string FRDescription;

    [Header("이탈리아어")]
    public string ITUpgradeName;
    [TextArea] public string ITDescription;

    [Header("독일어")]
    public string DEUpgradeName;
    [TextArea] public string DEDescription;

    [Header("스페인어")]
    public string ESUpgradeName;
    [TextArea] public string ESDescription;

    [Header("일본어")]
    public string JAUpgradeName;
    [TextArea] public string JADescription;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRUpgradeName;
    [TextArea] public string PT_BRDescription;

    [Header("러시아어")]
    public string RUUpgradeName;
    [TextArea] public string RUDescription;

    [Header("중국어 (간체)")]
    public string ZH_HANSUpgradeName;
    [TextArea] public string ZH_HANSDescription;

    public Sprite icon;
    public float value;
    public Action<UnitDefault> upgradeAction;

    public string GetLocalizedUpgradeName()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko")) return upgradeName;
        if (code.StartsWith("en")) return ENupgradeName;
        if (code.StartsWith("fr")) return FRUpgradeName;
        if (code.StartsWith("it")) return ITUpgradeName;
        if (code.StartsWith("de")) return DEUpgradeName;
        if (code.StartsWith("es")) return ESUpgradeName;
        if (code.StartsWith("ja")) return JAUpgradeName;
        if (code.StartsWith("pt")) return PT_BRUpgradeName;
        if (code.StartsWith("ru")) return RUUpgradeName;
        if (code.StartsWith("zh")) return ZH_HANSUpgradeName;

        // fallback
        return ENupgradeName;
    }

    public string GetLocalizedDescription()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        string template;

        if (code.StartsWith("ko")) template = description;
        else if (code.StartsWith("en")) template = ENdescription;
        else if (code.StartsWith("fr")) template = FRDescription;
        else if (code.StartsWith("it")) template = ITDescription;
        else if (code.StartsWith("de")) template = DEDescription;
        else if (code.StartsWith("es")) template = ESDescription;
        else if (code.StartsWith("ja")) template = JADescription;
        else if (code.StartsWith("pt")) template = PT_BRDescription;
        else if (code.StartsWith("ru")) template = RUDescription;
        else if (code.StartsWith("zh")) template = ZH_HANSDescription;
        else template = ENdescription; // fallback

        if (string.IsNullOrEmpty(template))
            template = ENdescription ?? description ?? string.Empty;

        try
        {
            return string.Format(template, value);
        }
        catch
        {
            return template;
        }
    }
}