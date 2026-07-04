using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[System.Serializable]
public struct Dialog
{
    public LocalizedString localizedString; // Inspector에서 Table + Entry 선택
}

public class DialogSystem : MonoBehaviour
{
    [Header("Dialog Data")]
    [SerializeField] private Dialog[] dialogs;

    [Header("UI")]
    [SerializeField] private Image dialogImage;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject nextArrow;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private KeyCode skipKey = KeyCode.Mouse0;

    private int currentIndex = -1;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private string currentFullText = "";

    // 초기화 (Tutorial에서 호출)
    public void Setup()
    {
        HideUI();
        currentIndex = -1;
        SetNextDialog();
    }

    // 매 프레임 호출 (TutorialController에서 호출)
    public bool UpdateDialog()
    {
        if (!Input.GetKeyDown(skipKey) && !Input.GetMouseButtonDown(0))
            return false;

        // 타이핑 중이면 즉시 완료(전체 텍스트 표시)
        if (isTyping)
        {
            FinishTypingImmediate();
            return false;
        }

        // 타이핑이 끝난 상태 — 다음 대사로
        if (currentIndex < dialogs.Length - 1)
        {
            SetNextDialog();
            return false;
        }

        // 모든 대사 종료
        // HideUI();
        return true;
    }

    private void SetNextDialog()
    {
        HideUI();
        currentIndex++;

        if (currentIndex < 0 || currentIndex >= dialogs.Length)
            return;

        if (dialogImage != null) dialogImage.gameObject.SetActive(true);
        if (dialogText != null) dialogText.gameObject.SetActive(true);

        // 이전 코루틴 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(TypingCoroutine(currentIndex));
    }

    private IEnumerator TypingCoroutine(int index)
    {
        isTyping = true;
        if (nextArrow != null) nextArrow.SetActive(false);

        // 안전하게 비동기 로드로 문자열을 얻음
        var ls = dialogs[index].localizedString;

        // 디버그: 로컬 및 식별자 확인(문제점 파악에 도움)
        Debug.Log($"[DialogSystem] Request localized string index={index}. Current Locale: {LocalizationSettings.SelectedLocale.Identifier}.");

        var handle = ls.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded &&
            !string.IsNullOrEmpty(handle.Result))
        {
            currentFullText = handle.Result;
        }
        else
        {
            // 실패 시 폴백 메시지 (문제 파악용)
            currentFullText = $"#NO_ENTRY_{index}";
            Debug.LogWarning($"[DialogSystem] Localized string load failed for index={index}. Check that the LocalizedString has a valid Table+Entry in Inspector.");
        }

        // 실제 타이핑
        dialogText.text = "";
        for (int i = 1; i <= currentFullText.Length; i++)
        {
            dialogText.text = currentFullText.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (nextArrow != null) nextArrow.SetActive(true);
    }

    private void FinishTypingImmediate()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        dialogText.text = currentFullText;
        if (nextArrow != null) nextArrow.SetActive(true);
    }

    public void HideUI()
    {
        if (dialogImage != null) dialogImage.gameObject.SetActive(false);
        if (dialogText != null) dialogText.gameObject.SetActive(false);
        if (nextArrow != null) nextArrow.SetActive(false);
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale locale)
    {
        // 타이핑 중이 아니고 현재 대사가 있으면 현재 대사 텍스트를 갱신
        if (!isTyping && currentIndex >= 0 && currentIndex < dialogs.Length)
        {
            StartCoroutine(UpdateTextForCurrentLocale());
        }
    }

    private IEnumerator UpdateTextForCurrentLocale()
    {
        var ls = dialogs[currentIndex].localizedString;
        var handle = ls.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded &&
            !string.IsNullOrEmpty(handle.Result))
        {
            currentFullText = handle.Result;
        }
        else
        {
            currentFullText = $"#NO_ENTRY_{currentIndex}";
            Debug.LogWarning($"[DialogSystem] Locale change: failed to get localized string for index={currentIndex}.");
        }

        dialogText.text = currentFullText;
    }
}
