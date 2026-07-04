using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Object/AchievementData")]
public class AchievementData : ScriptableObject
{
    public Sprite Icon;

    [Header("한국어")]
    [TextArea]
    public string Title;
    [TextArea]
    public string Desc;

    [Header("영어")]
    [TextArea]
    public string ENTitle;
    [TextArea]
    public string ENDesc;

    [Header("프랑스어")]
    [TextArea]
    public string FRTitle;
    [TextArea]
    public string FRDesc;

    [Header("이탈리아어")]
    [TextArea]
    public string ITTitle;
    [TextArea]
    public string ITDesc;

    [Header("독일어")]
    [TextArea]
    public string DETitle;
    [TextArea]
    public string DEDesc;

    [Header("스페인어")]
    [TextArea]
    public string ESTitle;
    [TextArea]
    public string ESDesc;

    [Header("일본어")]
    [TextArea]
    public string JATitle;
    [TextArea]
    public string JADesc;

    [Header("포르투갈어 (브라질)")]
    [TextArea]
    public string PT_BRTitle;
    [TextArea]
    public string PT_BRDesc;

    [Header("러시아어")]
    [TextArea]
    public string RUTitle;
    [TextArea]
    public string RUDesc;

    [Header("중국어 (간체)")]
    [TextArea]
    public string ZH_HANSTitle;
    [TextArea]
    public string ZH_HANSDesc;
}