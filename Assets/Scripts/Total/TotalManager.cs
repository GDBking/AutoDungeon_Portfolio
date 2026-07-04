using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TotalManager : MonoBehaviour
{
    public Transform unitListContents;
    public TotalUnitList unitList;
    public Sprite winSpr;
    public Image totalImg;
    public TextMeshProUGUI totalTxt1;
    public Transform MVPSlot;
    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI timeTxt;

    GameObject MVPUnit;
    int MVPCnt;

    private void OnEnable()
    {
        for (int i = 0; i < StaticManager.totalScore.Length; i++)
        {
            if (StaticManager.totalScore[i].cnt == 0)
                continue;

            TotalUnitList unitListComp = Instantiate(unitList, unitListContents);
            string path = "Units/" + DeckManager.instance.initDeckList[i];
            UnitDefault unitObj = Resources.Load<UnitDefault>(path);

            if (StaticManager.totalScore[i].cnt > MVPCnt)
            {
                MVPCnt = StaticManager.totalScore[i].cnt;
                MVPUnit = unitObj.gameObject;
            }

            /*GameObject unitFace;
            if (unitObj.transform.GetChild(0).name == "HorseRoot")
                unitFace = unitObj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
            else
                unitFace = unitObj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
            GameObject unitFaceObj = Instantiate(unitFace, unitListComp.unitFace);
            SortingGroup sortingGroup = unitFaceObj.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Total";
            sortingGroup.sortingOrder = 10;*/

            unitListComp.unitName.SetText(unitObj.GetLocalizedUnitName());
            unitListComp.damage.SetText(StaticManager.totalScore[i].values[0].ToString("F0"));
            unitListComp.defense.SetText(StaticManager.totalScore[i].values[1].ToString("F0"));
            unitListComp.heal.SetText(StaticManager.totalScore[i].values[2].ToString("F0"));
        }

        if (StaticManager.curHP < 0)
        {
            GetLoseLocalization();
        }
        else
        {
            totalImg.sprite = winSpr;
            GetWinLocalization();
        }

        if (MVPUnit != null)
        {
            GameObject mvpInstance = Instantiate(MVPUnit, MVPSlot);
            Transform t = mvpInstance.transform;

            if (t.childCount > 2)
            {
                Destroy(t.GetChild(2).gameObject);
                Destroy(t.GetChild(1).gameObject);
            }
            else if (t.childCount > 1)
            {
                Destroy(t.GetChild(1).gameObject);
            }

            mvpInstance.transform.localScale = Vector3.one * 120f;
        }

        stageTxt.SetText($"{StaticManager.curStage} - {GameManager.instance.currentLevel}");
        timeTxt.SetText($"{StaticManager.playTime / 60}Min {StaticManager.playTime % 60}Sec");

        if (MVPUnit == null) {
            AnalyticsInitializer.instance.EndGame("");
        }
        else {
            AnalyticsInitializer.instance.EndGame(MVPUnit.GetComponent<UnitDefault>().unitName);
        }
    }

    public void GetLoseLocalization()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        switch (code[..2])
        {
            case "ko": totalTxt1.SetText("패배"); break;
            case "en": totalTxt1.SetText("Defeat"); break;
            case "fr": totalTxt1.SetText("Défaite"); break;
            case "it": totalTxt1.SetText("Sconfitta"); break;
            case "de": totalTxt1.SetText("Niederlage"); break;
            case "es": totalTxt1.SetText("Derrota"); break;
            case "ja": totalTxt1.SetText("敗北"); break;
            case "pt": totalTxt1.SetText("Derrota"); break;
            case "ru": totalTxt1.SetText("Поражение"); break;
            case "zh": totalTxt1.SetText("失败"); break;
            default: totalTxt1.SetText("Defeat"); break;
        }
    }

    public void GetWinLocalization()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        switch (code[..2])
        {
            case "ko": totalTxt1.SetText("승리"); break;
            case "en": totalTxt1.SetText("Victory"); break;
            case "fr": totalTxt1.SetText("Victoire"); break;
            case "it": totalTxt1.SetText("Vittoria"); break;
            case "de": totalTxt1.SetText("Sieg"); break;
            case "es": totalTxt1.SetText("Victoria"); break;
            case "ja": totalTxt1.SetText("勝利"); break;
            case "pt": totalTxt1.SetText("Vitória"); break;
            case "ru": totalTxt1.SetText("Победа"); break;
            case "zh": totalTxt1.SetText("胜利"); break;
            default: totalTxt1.SetText("Victory"); break;
        }
    }
}