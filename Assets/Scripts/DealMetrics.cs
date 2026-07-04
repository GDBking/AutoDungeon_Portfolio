using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DealMetrics : MonoBehaviour
{
    public static DealMetrics instance;

    public GameObject dealMetricsPrefab;

    readonly List<Image> dealGaugebarImgList = new();
    readonly List<Text> dealGaugebarTextList = new();

    int idx;

    private void Awake()
    {
        instance = this;
    }

    public void CreateDealMetrics(UnitDefault unit)
    {
        GameObject unitDealMetrics;
        if (unit.dealMetricsIdx == -1)
            unitDealMetrics = Instantiate(dealMetricsPrefab, transform);
        else {
            unitDealMetrics = transform.GetChild(unit.dealMetricsIdx).gameObject;
            unitDealMetrics.transform.GetComponent<Button>().interactable = true;
        }

        DealMetricsPointerEnter pointerComp = unitDealMetrics.GetComponent<DealMetricsPointerEnter>();
        pointerComp.unitComp = unit;
        pointerComp.outlineComp = unit.GetComponent<SPUMOutline>();

        if (unit.dealMetricsIdx != -1)
            return;

        Transform unitHeadImg = unitDealMetrics.transform.GetChild(0);
        dealGaugebarImgList.Add(unitDealMetrics.transform.GetChild(1).GetComponent<Image>());
        dealGaugebarTextList.Add(unitDealMetrics.transform.GetChild(1).GetChild(0).GetComponent<Text>());

        GameObject unitHead;
        if (unit.transform.GetChild(0).name == "HorseRoot")
            unitHead = unit.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
        else
            unitHead = unit.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;

        GameObject unitHeadPrefab = Instantiate(unitHead, unitHeadImg);
        unitHeadPrefab.transform.localPosition = new Vector3(0, -15f, 0);
        unitHeadPrefab.transform.localScale = new Vector3(70f, 70f, 1f);
        SortingGroup sortingGroup = unitHeadPrefab.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Field";
        sortingGroup.sortingOrder = 400;

        unit.dealMetricsIdx = idx++;
    }

    public void UpdateDealMetrics(int idx, float value)
    {
        if (idx == -1)
            return;

        float deal = float.Parse(dealGaugebarTextList[idx].text) + value;
        dealGaugebarTextList[idx].text = deal.ToString("F1");

        float maxDeal = MaxDealGauge();
        if (maxDeal == 0f)
            return;

        for (int i = 0; i < dealGaugebarTextList.Count; i++) {
            float ratio = float.Parse(dealGaugebarTextList[i].text) / maxDeal;
            dealGaugebarImgList[i].fillAmount = ratio;
        }
    }

    float MaxDealGauge()
    {
        float max = 0f;

        foreach (Text deal in dealGaugebarTextList)
        {
            if (float.Parse(deal.text) > max) {
                max = float.Parse(deal.text);
            } 
        }

        return max;
    }

    public void ResetDealMetrics()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }

        dealGaugebarImgList.Clear();
        dealGaugebarTextList.Clear();

        idx = 0;
    }
}