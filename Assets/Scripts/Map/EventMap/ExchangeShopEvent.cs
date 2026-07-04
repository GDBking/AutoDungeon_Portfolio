using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ExchangeShopEvent : MonoBehaviour
{
    [Tooltip("몇 G당 1코스트로 교환할 것인 지")]
    public int moneyToCostRatio;

    public TextMeshProUGUI moneyTxt;
    public Slider moneySlider;
    public TMP_InputField moneyInputField;

    public TextMeshProUGUI costTxt;
    public Slider costSlider;
    public TMP_InputField costInputField;

    public AudioClip sliderClip;

    void SetSlider()
    {
        moneySlider.maxValue = StaticManager.gold / moneyToCostRatio;
        moneySlider.value = 1;

        costSlider.maxValue = StaticManager.curCost;
        costSlider.value = 1;
    }

    public void MoneySliderValueChange()
    {
        AudioManager.instance.PlaySfx(sliderClip);

        moneyInputField.SetTextWithoutNotify((moneySlider.value * moneyToCostRatio).ToString());

        var tpl = GetLocaleTexts();
        moneyTxt.SetText(string.Format(tpl.moneyTemplate, (int)moneySlider.value));
    }

    public void MoneyInputFieldValueChange()
    {
        // 입력값 파싱, 예외 처리
        if (!int.TryParse(moneyInputField.text, out int value))
            value = 0;

        // 오버/언더 플로우 제한
        value = Mathf.Clamp(value, 0, (int)moneySlider.maxValue * moneyToCostRatio);

        // 인풋필드와 슬라이더 동기화
        moneyInputField.SetTextWithoutNotify((value / moneyToCostRatio).ToString());
        moneySlider.value = value / moneyToCostRatio;
    }

    public void MoneyBtnClick()
    {
        GameManager.instance.SetGold(-(int)moneySlider.value * moneyToCostRatio);
        GameManager.instance.SetCost((int)moneySlider.value);

        SetSlider();

        SteamAchievement.Unlock("등가 교환");
    }

    public void CostSliderValueChange()
    {
        AudioManager.instance.PlaySfx(sliderClip);

        costInputField.SetTextWithoutNotify(costSlider.value.ToString());

        var tpl = GetLocaleTexts();
        costTxt.SetText(string.Format(tpl.costTemplate, (int)(costSlider.value * moneyToCostRatio)));
    }

    public void CostInputFieldValueChange()
    {
        // 입력값 파싱, 예외 처리
        if (!int.TryParse(costInputField.text, out int value))
            value = 0;

        // 오버/언더 플로우 제한
        value = Mathf.Clamp(value, 0, (int)costSlider.maxValue);

        // 인풋필드와 슬라이더 동기화
        costInputField.SetTextWithoutNotify(value.ToString());
        costSlider.value = value;
    }

    public void CostBtnClick()
    {
        GameManager.instance.SetCost(-(int)costSlider.value);
        GameManager.instance.SetGold((int)costSlider.value * moneyToCostRatio);
        GameManager.instance.maxCost = StaticManager.curCost;

        SetSlider();

        SteamAchievement.Unlock("등가 교환");
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetExchangeLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        SetSlider();

        // 로케일 변경 시 텍스트를 즉시 갱신하도록 이벤트 구독
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        // 초기 표시 갱신
        UpdateLocalizedTexts();
    }
    private void OnDisable()
    {
        // 기존 로직 유지...
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        UpdateLocalizedTexts();

        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetExchangeLocaleText());
    }

    private void UpdateLocalizedTexts()
    {
        var tpl = GetLocaleTexts();

        // money 텍스트: 현재 슬라이더 값(코스트) 사용
        if (moneyTxt != null)
        {
            moneyTxt.SetText(string.Format(tpl.moneyTemplate, (int)moneySlider.value));
        }

        // cost 텍스트: 현재 슬라이더 값 * 비율
        if (costTxt != null)
        {
            costTxt.SetText(string.Format(tpl.costTemplate, (int)(costSlider.value * moneyToCostRatio)));
        }
    }

    private struct ExLocaleTexts
    {
        public string moneyTemplate; // {0} = moneySlider.value (보라/파란색 적용된 부분)
        public string costTemplate;  // {0} = costSlider.value * moneyToCostRatio (노란색)
    }

    private ExLocaleTexts GetLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        if (code.StartsWith("ko"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "G를 <color=#000DFF>{0}</color>코스트로 교환",
                costTemplate = "코스트를 <color=#FBF236>{0}</color>G로 교환"
            };
        }
        if (code.StartsWith("en"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Exchange G for <color=#000DFF>{0}</color> cost",
                costTemplate = "Exchange <color=#FBF236>{0}</color> G for cost"
            };
        }
        if (code.StartsWith("fr"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Échanger G pour <color=#000DFF>{0}</color> coût",
                costTemplate = "Échanger <color=#FBF236>{0}</color> G pour le coût"
            };
        }
        if (code.StartsWith("it"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Scambia G per <color=#000DFF>{0}</color> costo",
                costTemplate = "Scambia <color=#FBF236>{0}</color> G per costo"
            };
        }
        if (code.StartsWith("de"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Tausche G gegen <color=#000DFF>{0}</color> Kosten",
                costTemplate = "Tausche <color=#FBF236>{0}</color> G gegen Kosten"
            };
        }
        if (code.StartsWith("es"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Intercambia G por <color=#000DFF>{0}</color> coste",
                costTemplate = "Intercambia <color=#FBF236>{0}</color> G por coste"
            };
        }
        if (code.StartsWith("ja"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Gを<color=#000DFF>{0}</color>コストと交換",
                costTemplate = "コストを<color=#FBF236>{0}</color>Gに交換"
            };
        }
        if (code.StartsWith("pt"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Trocar G por <color=#000DFF>{0}</color> custo",
                costTemplate = "Trocar <color=#FBF236>{0}</color> G por custo"
            };
        }
        if (code.StartsWith("ru"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "Обменять G на <color=#000DFF>{0}</color> стоимости",
                costTemplate = "Обменять <color=#FBF236>{0}</color> G на стоимость"
            };
        }
        if (code.StartsWith("zh"))
        {
            return new ExLocaleTexts
            {
                moneyTemplate = "将 G 兑换为 <color=#000DFF>{0}</color> 点费用",
                costTemplate = "将 <color=#FBF236>{0}</color> G 兑换为费用"
            };
        }

        // fallback (영어)
        return new ExLocaleTexts
        {
            moneyTemplate = "Exchange G for <color=#000DFF>{0}</color> cost",
            costTemplate = "Exchange <color=#FBF236>{0}</color> G for cost"
        };
    }

    private string GetExchangeLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", StringComparison.OrdinalIgnoreCase))
            return "골드와 코스트를 원하는 만큼 교환하세요!\n수수료는 없습니다!";
        if (code.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            return "Exchange gold and cost as much as you like!\nThere are no fees!";
        if (code.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
            return "Échangez de l'or et des coûts autant que vous le souhaitez !\nAucuns frais !";
        if (code.StartsWith("it", StringComparison.OrdinalIgnoreCase))
            return "Scambia oro e costo quanto vuoi!\nNessuna commissione!";
        if (code.StartsWith("de", StringComparison.OrdinalIgnoreCase))
            return "Tauschen Sie Gold und Kosten so viel Sie möchten!\nEs fallen keine Gebühren an!";
        if (code.StartsWith("es", StringComparison.OrdinalIgnoreCase))
            return "¡Cambia oro y coste tanto como quieras!\n¡No hay tarifas!";
        if (code.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
            return "ゴールドとコストを好きなだけ交換してください！\n手数料はかかりません！";
        if (code.StartsWith("pt", StringComparison.OrdinalIgnoreCase))
            return "Troque ouro e custo quanto desejar!\nSem taxas!";
        if (code.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
            return "Обменивайте золото и стоимость в любом количестве!\nКомиссия отсутствует!";
        if (code.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            return "按您想要的数量兑换金币和费用！\n不收取手续费！";

        return "Exchange gold and cost as much as you like!\nThere are no fees!";
    }
}