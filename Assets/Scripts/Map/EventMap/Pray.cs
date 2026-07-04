using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Pray : MonoBehaviour
{
    public static Pray instance;

    public TextMeshProUGUI remainingPrayTxt;
    public TextMeshProUGUI prayTxt;
    public GameObject PrayCheckPanel;
    public Button prayBtn;
    public GameObject effect;

    int remainingPrayCnt;
    UnitDefault unitComp;

    private void Awake()
    {
        instance = this;
    }

    public void CardClick(UnitDefault unitComp)
    {
        this.unitComp = unitComp;

        var texts = GetPrayLocaleTexts();

        if (remainingPrayCnt == 0)
        {
            prayTxt.SetText(texts.noEnergy);
            prayBtn.gameObject.SetActive(false);
        }
        else if (StaticManager.maxLevel[unitComp.unitIdx] == 9)
        {
            prayTxt.SetText(texts.maxLevelReached);
            prayBtn.gameObject.SetActive(false);
        }
        else
        {
            int nextLevel = StaticManager.maxLevel[unitComp.unitIdx] + 1;
            string Highlight = $"<b>‘{unitComp.GetLocalizedUnitName()}’</b>";

            prayTxt.SetText(
            string.Format(
                texts.confirmTemplate,
                Highlight,
                nextLevel
                )
            );
            prayBtn.gameObject.SetActive(true);
        }

        PrayCheckPanel.SetActive(true);
    }

    public void PrayBtnClick()
    {
        effect.SetActive(true);
        
        PrayCheckPanel.SetActive(false);

        StaticManager.maxLevel[unitComp.unitIdx]++;

        SetRemainingPrayTxt(--remainingPrayCnt);

        SteamAchievement.Unlock("신의 속삭임");

        unitComp.UpdateStatInfo();
    }

    void SetRemainingPrayTxt(int cnt)
    {
        var texts = GetPrayLocaleTexts();
        remainingPrayTxt.SetText(string.Format(texts.remainingTemplate, cnt));
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetPrayLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        StaticManager.isPray = true;
        remainingPrayCnt = Random.Range(1, 3);
        SetRemainingPrayTxt(remainingPrayCnt);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        effect.SetActive(false);
        StaticManager.isPray = false;
    }

    private struct PrayLocaleTexts
    {
        public string noEnergy;
        public string maxLevelReached;
        public string confirmTemplate; // {0} = unitName, {1} = nextLevel
        public string remainingTemplate; // {0} = cnt
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        // 남은 기도 횟수 텍스트 갱신
        SetRemainingPrayTxt(remainingPrayCnt);

        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetPrayLocaleText());

        // 현재 확인 팝업이 열려 있으면 내용 갱신 (unitComp가 null이면 무시)
        if (PrayCheckPanel != null && PrayCheckPanel.activeInHierarchy && unitComp != null)
        {
            // CardClick은 prayBtn.interactable까지 포함해 상태를 재세팅하므로 재사용
            CardClick(unitComp);
        }
    }

    private PrayLocaleTexts GetPrayLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        if (code.StartsWith("ko"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "남은 기력이 없습니다.",
                maxLevelReached = "유닛 조각의 최대 레벨은 9까지입니다.",
                confirmTemplate = "{0} 조각의 최대 레벨을 {1}까지 올리시겠습니까?",
                remainingTemplate = "남은 기도 횟수 : {0}"
            };
        }
        if (code.StartsWith("en"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "No remaining energy.",
                maxLevelReached = "Unit piece maximum level is 9.",
                confirmTemplate = "Raise {0} piece max level to {1}?",
                remainingTemplate = "Remaining prayers: {0}"
            };
        }
        if (code.StartsWith("fr"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "Aucune énergie restante.",
                maxLevelReached = "Le niveau maximal des fragments est 9.",
                confirmTemplate = "Augmenter le niveau max du fragment {0} à {1} ?",
                remainingTemplate = "Prières restantes : {0}"
            };
        }
        if (code.StartsWith("it"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "Energia residua insufficiente.",
                maxLevelReached = "Il livello massimo del frammento è 9.",
                confirmTemplate = "Portare il livello massimo di {0} a {1}?",
                remainingTemplate = "Preghiere rimanenti: {0}"
            };
        }
        if (code.StartsWith("de"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "Keine verbleibende Energie.",
                maxLevelReached = "Maximales Fragmentlevel ist 9.",
                confirmTemplate = "Maximales Level von {0} auf {1} erhöhen?",
                remainingTemplate = "Verbleibende Gebete: {0}"
            };
        }
        if (code.StartsWith("es"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "No queda energía.",
                maxLevelReached = "El nivel máximo de la pieza es 9.",
                confirmTemplate = "¿Subir el nivel máximo de {0} a {1}?",
                remainingTemplate = "Oraciones restantes: {0}"
            };
        }
        if (code.StartsWith("ja"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "残りの気力がありません。",
                maxLevelReached = "ユニットの破片の最大レベルは9です。",
                confirmTemplate = "{0}の破片の最大レベルを{1}まで上げますか？",
                remainingTemplate = "残りの祈り回数 : {0}"
            };
        }
        if (code.StartsWith("pt"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "Sem energia restante.",
                maxLevelReached = "O nível máximo do fragmento é 9.",
                confirmTemplate = "Aumentar o nível máximo de {0} para {1}?",
                remainingTemplate = "Orações restantes: {0}"
            };
        }
        if (code.StartsWith("ru"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "Нет оставшейся энергии.",
                maxLevelReached = "Максимальный уровень фрагмента — 9.",
                confirmTemplate = "Повысить макс. уровень {0} до {1}?",
                remainingTemplate = "Осталось молитв: {0}"
            };
        }
        if (code.StartsWith("zh"))
        {
            return new PrayLocaleTexts
            {
                noEnergy = "没有剩余的精力。",
                maxLevelReached = "碎片的最大等级为9。",
                confirmTemplate = "将{0}碎片的最高等级提高到{1}？",
                remainingTemplate = "剩余祈祷次数：{0}"
            };
        }

        // fallback(영어)
        return new PrayLocaleTexts
        {
            noEnergy = "No remaining energy.",
            maxLevelReached = "Unit piece maximum level is 9.",
            confirmTemplate = "Raise {0} piece max level to {1}?",
            remainingTemplate = "Remaining prayers: {0}"
        };
    }

    private string GetPrayLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "유닛 조각을 더욱 강력하게 만들고 싶으신가요?\n원하는 유닛 조각의 최대 레벨을 증가시킬 수 있도록 기도해 보세요.";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "Would you like to make a unit fragment more powerful?\nPray to increase the maximum level of the unit fragment you want.";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Souhaitez-vous rendre un fragment d'unité plus puissant ?\nPriez pour augmenter le niveau maximal du fragment d'unité souhaité.";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Vuoi rendere più potente il frammento di un'unità?\nPrega per aumentare il livello massimo del frammento dell'unità desiderata.";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Möchten Sie ein Einheitenfragment stärker machen?\nBeten Sie, um die maximale Stufe des gewünschten Einheitenfragments zu erhöhen.";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "¿Desea que el fragmento de una unidad sea más poderoso?\nOre para aumentar el nivel máximo del fragmento de unidad que desee.";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "ユニットの断片をさらに強くしたいですか？\n欲しいユニット断片の最大レベルを上げるために祈ってください。";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Deseja tornar o fragmento de uma unidade mais poderoso?\nReze para aumentar o nível máximo do fragmento da unidade desejada.";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Хотите сделать фрагмент юнита более мощным?\nПомолитесь, чтобы повысить максимальный уровень нужного фрагмента юнита.";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "想让某个单位的碎片更强力吗？\n祈祷以提高所需单位碎片的最大等级。";

        // fallback: English
        return "Would you like to make a unit fragment more powerful?\nPray to increase the maximum level of the unit fragment you want.";
    }
}