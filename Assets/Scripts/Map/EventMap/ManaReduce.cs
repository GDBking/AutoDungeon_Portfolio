using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class ManaReduce : MonoBehaviour
{
    public static ManaReduce instance;

    public GameObject manaReduceCheckUI;
    public TextMeshProUGUI manaReduceTxt;
    public Sprite mainBG;
    public Sprite useBG;
    public GameObject panel;
    public GameObject drinkBtn;

    UnitDefault unitComp;

    private void Awake()
    {
        instance = this;
    }

    public void CardClick(UnitDefault unitComp)
    {
        this.unitComp = unitComp;

        var texts = GetManaReduceLocaleTexts();

        string localizedName = unitComp.GetLocalizedUnitName(); // 로컬라이즈된 이름 (순수 문자열)
        string highlightName = $"<b>‘{localizedName}’</b>";

        int stones = unitComp.Level * (unitComp.Level + 1) / 2; // 강화석 수 계산

        // {0} = 이름(강조된), {1} = 강화석 수
        manaReduceTxt.SetText(
            string.Format(texts.confirmTemplate, highlightName, stones)
        );

        manaReduceCheckUI.SetActive(true);
    }

    public void OKBtnClick()
    {
        drinkBtn.SetActive(false);

        manaReduceCheckUI.SetActive(false);
        StaticManager.isManaReduce = false;

        GameManager.instance.SetUpgradeStone(unitComp.Level * (unitComp.Level + 1) / 2);
        StaticManager.skillPoint[unitComp.unitIdx] = 0;
        StaticManager.stat[unitComp.unitIdx] = new();
        unitComp.SetLevelEffect(0);

        unitComp.UpdateStatInfo(true);

        panel.SetActive(true);
        GetComponent<Image>().sprite = useBG;
        StartCoroutine(DisableAfter());

        SteamAchievement.Unlock("시간 역행");
    }

    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(0.5f);
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        MapLoad.instance.eventMapDescTxt.SetText(GetManaReduceEventDescLocaleText());
        MapLoad.instance.eventMapDescTxt.gameObject.SetActive(true);

        StaticManager.isManaReduce = true;
        GetComponent<Image>().sprite = mainBG;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        StaticManager.isManaReduce = false;

        drinkBtn.SetActive(true);

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private struct ManaReduceTexts
    {
        public string confirmTemplate; // {0} = 유닛 이름(강조), {1} = 강화석 개수
    }

    private void OnLocaleChanged(Locale locale)
    {
        // 설명 텍스트도 로케일 변경 시 갱신
        if (MapLoad.instance != null && MapLoad.instance.eventMapDescTxt != null)
            MapLoad.instance.eventMapDescTxt.SetText(GetManaReduceEventDescLocaleText());

        if (!manaReduceCheckUI.activeInHierarchy || unitComp == null)
            return;

        var texts = GetManaReduceLocaleTexts();
        string highlightName = $"<b>‘{unitComp.GetLocalizedUnitName()}’</b>";
        int stones = unitComp.Level * (unitComp.Level + 1) / 2;

        manaReduceTxt.SetText(
            string.Format(texts.confirmTemplate, highlightName, stones)
        );
    }

    private ManaReduceTexts GetManaReduceLocaleTexts()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code.ToLower()
            : "en";

        // 각 문장에 {0} = 이름, {1} = 강화석 수 가 들어갑니다.
        if (code.StartsWith("ko"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "{0} 조각의 레벨을 초기화하고 강화석 {1}개를 돌려받으시겠습니까?"
            };
        }

        if (code.StartsWith("en"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Would you like to reset the level of the {0} piece and receive {1} upgrade stones?"
            };
        }

        if (code.StartsWith("fr"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Souhaitez-vous réinitialiser le niveau du fragment {0} et récupérer {1} pierres d'amélioration ?"
            };
        }

        if (code.StartsWith("it"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Vuoi reimpostare il livello del frammento {0} e ricevere {1} pietre di potenziamento?"
            };
        }

        if (code.StartsWith("de"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Möchten Sie das Level des Fragments {0} zurücksetzen und {1} Aufwertungssteine zurückerhalten?"
            };
        }

        if (code.StartsWith("es"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "¿Desea restablecer el nivel de la pieza {0} y recibir {1} piedras de mejora?"
            };
        }

        if (code.StartsWith("ja"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "{0} の断片のレベルを初期化して、強化石 {1}個を受け取りますか？"
            };
        }

        // pt-BR 및 "pt" 접두사 처리
        if (code.StartsWith("pt"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Deseja redefinir o nível do fragmento {0} e receber {1} pedras de melhoria?"
            };
        }

        if (code.StartsWith("ru"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "Хотите сбросить уровень фрагмента {0} и получить {1} камней улучшения?"
            };
        }

        if (code.StartsWith("zh"))
        {
            return new ManaReduceTexts
            {
                confirmTemplate = "是否要将 {0} 碎片的等级重置并收回其 {1} 个强化石？"
            };
        }

        // fallback (영어)
        return new ManaReduceTexts
        {
            confirmTemplate = "Would you like to reset the level of the {0} piece and receive {1} upgrade stones?"
        };
    }

    private string GetManaReduceEventDescLocaleText()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "유닛 조각의 업그레이드 수치가 마음에 들지 않으신가요?\n원하는 유닛 조각의 레벨을 초기화하고 강화석을 모두 돌려받으세요.";
        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "Are you dissatisfied with a unit fragment's upgrade values?\nReset its level and recover all upgrade stones.";
        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "Les valeurs d'amélioration du fragment d'unité ne vous conviennent pas ?\nRéinitialisez son niveau et récupérez toutes les pierres d'amélioration.";
        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "Non sei soddisfatto dei valori di potenziamento del frammento di unità?\nReimposta il livello e recupera tutte le pietre di potenziamento.";
        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "Sind Sie mit den Aufwertungswerten eines Einheitenfragments unzufrieden?\nSetzen Sie dessen Level zurück und erhalten Sie alle Aufwertungssteine zurück.";
        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "¿No estás satisfecho con los valores de mejora de un fragmento de unidad?\nRestablece su nivel y recupera todas las piedras de mejora.";
        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "ユニット断片の強化値に満足していませんか？\nレベルを初期化して、すべての強化石を取り戻しましょう。";
        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "Não está satisfeito com os valores de aprimoramento do fragmento de unidade?\nRedefina o nível e recupere todas as pedras de aprimoramento.";
        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "Вас не устраивают значения улучшения фрагмента юнита?\nСбросьте уровень и получите обратно все камни улучшения.";
        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "对单位碎片的强化数值不满意吗？\n重置其等级并取回所有强化石。";

        // fallback: English
        return "Are you dissatisfied with a unit fragment's upgrade values?\nReset its level and recover all upgrade stones.";
    }
}
