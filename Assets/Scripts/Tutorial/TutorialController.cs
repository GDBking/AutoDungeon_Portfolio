using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private List<TutorialBase> tutorials;

    private TutorialBase currentTutorial = null;
    private int currentIndex = -1;

    private const string TutorialKey = "Tutorial_Completed_v1"; // 버전 붙이면 업데이트 시 재실행 가능

    public GameObject canvas;

    private void Awake()
    {
        // 이미 완료했으면 튜토리얼 건너뛰기
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1)
        {
            CompletedAllTutorials();
            if (canvas != null) canvas.SetActive(false);
            return;
        }

        // 튜토리얼이 아예 없으면 아무 동작도 하지 않음 (자동 완료 플래그 설정 X)
        if (tutorials == null || tutorials.Count == 0)
        {
            Debug.LogWarning("TutorialController: tutorials list is empty.");
            if (canvas != null) canvas.SetActive(false);
            return;
        }

        // 튜토리얼 UI 켜기 (인스펙터에서 꺼져있을 수 있으니 안전하게 처리)
        if (canvas != null) canvas.SetActive(true);

        SetNextTutorial();
    }

    private void Update()
    {
        if (currentTutorial != null)
        {
            currentTutorial.Exeute(this);
        }
    }

    public void SetNextTutorial()
    {
        if (currentTutorial != null)
        {
            currentTutorial.Exit();
        }

        if (currentIndex >= tutorials.Count - 1)
        {
            CompletedAllTutorials();
            return;
        }

        currentIndex++;
        currentTutorial = tutorials[currentIndex];
        currentTutorial.Enter();
    }

    public void CompletedAllTutorials()
    {
        currentTutorial = null;

        // 실제 완료 시에만 플래그를 세움
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();

        if (canvas != null) canvas.SetActive(false);

        // 추가 행동 (예: 이벤트 발행, analytics 등)
    }

    /*[ContextMenu("ResetTutorials")]
    public void ResetTutorials()
    {
        PlayerPrefs.DeleteKey(TutorialKey);
        PlayerPrefs.Save();
        Debug.Log("Tutorials reset.");
    }*/
}