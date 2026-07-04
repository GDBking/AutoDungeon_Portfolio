using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class Localization : MonoBehaviour
{
    public static Localization instance;
    public int index;

    [System.Serializable]
    public class LocaleFontPair
    {
        public string localeCode; // ex: "en", "ko", "ja", "zh-Hans", "pt-BR", "ru"
        public TMP_FontAsset font;
    }

    [Header("Font Mapping (set in Inspector)")]
    public List<LocaleFontPair> localeFonts = new();

    [Header("Options")]
    public bool applyFontsToAllTMP = true; // true면 씬의 모든 TMP_Text에 적용
    public bool applyOnStart = true;       // Start 시 폰트 자동 적용 여부

    private void Awake()
    {
        // 기존 인스턴스 처리 (중복 방지)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        index = PlayerPrefs.GetInt("localeIndex", -1);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        // Unity Localization 초기화 대기
        yield return LocalizationSettings.InitializationOperation;

        // SteamManager 초기화 대기 (최대 2초)
        int waitFrame = 0;
        while (!SteamManager.Initialized && waitFrame < 120)
        {
            waitFrame++;
            yield return null;
        }

        // 언어 설정 필요할 경우 (최초 실행)
        if (SteamManager.Initialized && index == -1)
        {
            string lang = SteamApps.GetCurrentGameLanguage().ToLower();
            Debug.Log("[Steam] Game Language: " + lang);

            // 스팀 언어에 따라 Locale 인덱스 결정
            if (lang == "korean" || lang == "koreana")
            {
                index = FindLocaleIndex("ko");
            }
            else if (lang == "english")
            {
                index = FindLocaleIndex("en");
            }
            else
            {
                // 지원되지 않는 언어 → 기본값 영어로 fallback
                index = FindLocaleIndex("en");
            }

            // 그래도 못 찾으면 ko → 그마저도 없으면 첫 번째
            if (index == -1)
            {
                index = FindLocaleIndex("ko");
                if (index == -1) index = 0;
            }

            PlayerPrefs.SetInt("localeIndex", index);
            PlayerPrefs.Save();
        }

        // 이벤트 구독 (선택된 Locale 변경 시)
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

        // 언어 적용
        if (LocalizationSettings.InitializationOperation.Status == AsyncOperationStatus.Succeeded)
        {
            // index 유효성 체크
            if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count)
            {
                index = 0;
            }

            var selected = LocalizationSettings.AvailableLocales.Locales[index];
            if (LocalizationSettings.SelectedLocale != selected)
            {
                LocalizationSettings.SelectedLocale = selected;
            }
            else if (applyOnStart)
            {
                // 이미 선택된 Locale인 경우에도 폰트 적용
                ApplyFontsForLocale(selected);
            }
        }
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LocalizationSettings.SelectedLocale != null)
        {
            ApplyFontsForLocale(LocalizationSettings.SelectedLocale);
        }
    }

    // --- 변경된 부분: 왼/오 버튼으로 전환할 수 있는 메서드들 ---
    // UI 버튼에서 이 메서드를 호출하세요: (+1 = 오른쪽/다음, -1 = 왼쪽/이전)
    public void ChangeLocaleByDelta(int delta)
    {
        int count = LocalizationSettings.AvailableLocales.Locales.Count;
        if (count == 0) return;

        // index가 유효하지 않으면 현재 SelectedLocale 기준으로 초기화
        if (index < 0 || index >= count)
        {
            index = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            if (index < 0) index = 0;
        }

        index += delta;

        // 순환 처리
        if (index < 0) index = count - 1;
        if (index >= count) index = 0;

        // Locale 설정 (이로 인해 SelectedLocaleChanged 이벤트가 발생하며 폰트 적용)
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        // 저장
        PlayerPrefs.SetInt("localeIndex", index);
        PlayerPrefs.Save();
    }

    // 편의 메서드: UI Button에 바로 연결 가능
    public void OnClickNextLocale() => ChangeLocaleByDelta(1);
    public void OnClickPrevLocale() => ChangeLocaleByDelta(-1);
    // ------------------------------------------------------------

    private int FindLocaleIndex(string code)
    {
        return LocalizationSettings.AvailableLocales.Locales
            .FindIndex(locale => locale.Identifier.Code == code);
    }

    private void OnSelectedLocaleChanged(Locale newLocale)
    {
        int found = LocalizationSettings.AvailableLocales.Locales.IndexOf(newLocale);
        if (found >= 0) index = found;

        TMP_FontAsset targetFont = FindFontForLocale(newLocale.Identifier.Code);

        if (targetFont != null)
        {
            // TMP 전역 기본 폰트 교체
            TMP_Settings.defaultFontAsset = targetFont;
        }

        ApplyFontsForLocale(newLocale);
    }

    // 실제 폰트 적용 로직
    private void ApplyFontsForLocale(Locale locale)
    {
        if (locale == null) return;

        TMP_FontAsset targetFont = FindFontForLocale(locale.Identifier.Code);

        if (targetFont == null)
        {
            Debug.LogWarning($"[Localization] No font mapped for locale '{locale.Identifier.Code}'. Skipping font apply.");
            return;
        }

        if (applyFontsToAllTMP)
        {
            // 활성화/비활성화 포함 모든 TMP_Text에 적용
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (var t in allTexts)
            {
                // 에디터에서 prefab assets까지 나올 수 있으므로 씬에 존재하는 객체만 필터
                if (t.gameObject.scene.isLoaded)
                {
                    t.font = targetFont;
                }
            }
        }
    }

    // localeCode에 맞는 폰트 찾기: 정확 매칭 -> 접두사 매칭 (pt-BR -> pt) -> zh 계열 처리
    private TMP_FontAsset FindFontForLocale(string localeCode)
    {
        if (string.IsNullOrEmpty(localeCode)) return null;

        // 1) 정확 매칭
        foreach (var p in localeFonts)
        {
            if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals(localeCode, System.StringComparison.OrdinalIgnoreCase))
            {
                if (p.font != null) return p.font;
            }
        }

        // 2) 접두사 매칭 (ex: "pt-BR" -> "pt")
        int dash = localeCode.IndexOf('-');
        if (dash > 0)
        {
            string prefix = localeCode.Substring(0, dash);
            foreach (var p in localeFonts)
            {
                if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals(prefix, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (p.font != null) return p.font;
                }
            }
        }

        // 3) zh 계열(간체/번체) 처리 예시: "zh-Hans" 우선, 그다음 "zh"
        if (localeCode.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
        {
            foreach (var p in localeFonts)
            {
                if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (p.font != null) return p.font;
                }
            }
        }

        // 4) fallback: "en"
        foreach (var p in localeFonts)
        {
            if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals("en", System.StringComparison.OrdinalIgnoreCase))
            {
                if (p.font != null) return p.font;
            }
        }

        return null;
    }
}