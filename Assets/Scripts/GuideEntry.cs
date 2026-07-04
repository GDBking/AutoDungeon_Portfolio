using UnityEngine;

[CreateAssetMenu(fileName = "GuideEntry", menuName = "Scriptable Object/GuideEntryData")]
public class GuideEntry : ScriptableObject
{
    [Header("한국어")]
    public string Title;
    [TextArea]
    public string Desc;

    [Header("영어")]
    public string ENTitle;
    [TextArea]
    public string ENDesc;

    [Header("프랑스어")]
    public string FRTitle;
    [TextArea]
    public string FRDesc;

    [Header("이탈리아어")]
    public string ITTitle;
    [TextArea]
    public string ITDesc;

    [Header("독일어")]
    public string DETitle;
    [TextArea]
    public string DEDesc;

    [Header("스페인어")]
    public string ESTitle;
    [TextArea]
    public string ESDesc;

    [Header("일본어")]
    public string JATitle;
    [TextArea]
    public string JADesc;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRTitle;
    [TextArea]
    public string PT_BRDesc;

    [Header("러시아어")]
    public string RUTitle;
    [TextArea]
    public string RUDesc;

    [Header("중국어 (간체)")]
    public string ZH_HANSTitle;
    [TextArea]
    public string ZH_HANSDesc;
}