using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class Relics : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Title title;

    [Header("한국어")]
    public string relicsName;
    [TextArea] public string desc;

    [Header("영어")]
    public string ENRelicsName;
    [TextArea] public string ENDesc;

    [Header("프랑스어")]
    public string FRRelicsName;
    [TextArea] public string FRDesc;

    [Header("이탈리아어")]
    public string ITRelicsName;
    [TextArea] public string ITDesc;

    [Header("독일어")]
    public string DERelicsName;
    [TextArea] public string DEDesc;

    [Header("스페인어")]
    public string ESRelicsName;
    [TextArea] public string ESDesc;

    [Header("일본어")]
    public string JARelicsName;
    [TextArea] public string JADesc;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRRelicsName;
    [TextArea] public string PT_BRDesc;

    [Header("러시아어")]
    public string RURelicsName;
    [TextArea] public string RUDesc;

    [Header("중국어 (간체)")]
    public string ZH_HANSRelicsName;
    [TextArea] public string ZH_HANSDesc;

    [System.NonSerialized] public string activeDesc;
    [HideInInspector] public bool isActive;

    GameObject unitInfo;
    GameObject itemInfo;
    GameObject eventInfo;
    GameObject relicsInfo;
    List<TextMeshProUGUI> relicsTxt;

    Image imgComp;

    [HideInInspector] public Action<UnitDefault> useEffect;

    private struct LocalizedPair { public string name; public string description; public LocalizedPair(string n, string d) { name = n; description = d; } }
    private Dictionary<string, LocalizedPair> _localeMap;

    private void Awake()
    {
        unitInfo = GameManager.instance.unitInfo;
        itemInfo = GameManager.instance.itemInfo;
        eventInfo = GameManager.instance.eventInfo;
        relicsInfo = GameManager.instance.relicsInfo;
        relicsTxt = GameManager.instance.relicsTxt;

        imgComp = GetComponent<Image>();

        _localeMap = new Dictionary<string, LocalizedPair>(StringComparer.OrdinalIgnoreCase)
        {
            { "ko", new LocalizedPair(relicsName, desc) },
            { "en", new LocalizedPair(ENRelicsName, ENDesc) },
            { "fr", new LocalizedPair(FRRelicsName, FRDesc) },
            { "it", new LocalizedPair(ITRelicsName, ITDesc) },
            { "de", new LocalizedPair(DERelicsName, DEDesc) },
            { "es", new LocalizedPair(ESRelicsName, ESDesc) },
            { "ja", new LocalizedPair(JARelicsName, JADesc) },
            { "pt-BR", new LocalizedPair(PT_BRRelicsName, PT_BRDesc) },
            { "pt", new LocalizedPair(PT_BRRelicsName, PT_BRDesc) },
            { "ru", new LocalizedPair(RURelicsName, RUDesc) },
            { "zh-Hans", new LocalizedPair(ZH_HANSRelicsName, ZH_HANSDesc) },
            { "zh", new LocalizedPair(ZH_HANSRelicsName, ZH_HANSDesc) }
        };
    }

    public void SetRelicsEffect(UnitDefault unitComp)
    {
        if (title == Title.Courage || title == Title.Wisdom) {
            if (title == Title.Courage && ((int)unitComp.rank == 0 || (int)unitComp.rank == 3)) {
                useEffect(unitComp);
                return;
            }
            else if (title == Title.Wisdom && ((int)unitComp.rank == 1 || (int)unitComp.rank == 2)) {
                useEffect(unitComp);
                return;
            }
        }

        foreach (Title matchTitle in StaticManager.title[unitComp.unitIdx]) {
            if (title == matchTitle) {
                useEffect(unitComp);
                break;
            }
        }
        //useEffect(unitComp);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imgComp.color = Color.gray;

        unitInfo.SetActive(false);
        eventInfo.SetActive(false);
        itemInfo.SetActive(false);

        var pair = GetLocalizedPairForCurrentLocale();

        relicsTxt[0].SetText(pair.name);
        relicsTxt[1].SetText(pair.description);

        // 활성화 설명이 있으면 덧붙임 (원래 코드와 동일)
        if (isActive)
        {
            // SetText로 안전하게 갱신 — string concatenation 대신 SetText 권장
            relicsTxt[1].SetText($"{pair.description}\n\n{activeDesc}");
        }

        relicsInfo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imgComp.color = Color.white;
    }

    private LocalizedPair GetLocalizedPairForCurrentLocale()
    {
        string localeCode = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : null;

        if (string.IsNullOrEmpty(localeCode))
        {
            localeCode = "en";
        }

        if (_localeMap.TryGetValue(localeCode, out var pairExact))
        {
            return pairExact;
        }

        int dash = localeCode.IndexOf('-');
        if (dash > 0)
        {
            string prefix = localeCode.Substring(0, dash);
            if (_localeMap.TryGetValue(prefix, out var pairPrefix))
            {
                return pairPrefix;
            }
        }

        if (localeCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            if (_localeMap.TryGetValue("zh", out var pairZh))
            {
                return pairZh;
            }
        }

        if (_localeMap.TryGetValue("en", out var pairEn))
        {
            return pairEn;
        }

        if (_localeMap.TryGetValue("ko", out var pairKo))
        {
            return pairKo;
        }

        foreach (var kv in _localeMap)
        {
            return kv.Value;
        }

        return new LocalizedPair(relicsName, desc);
    }
}