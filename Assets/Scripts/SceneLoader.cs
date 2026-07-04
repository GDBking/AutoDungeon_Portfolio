using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;

    public GameObject continueBtn;

    public AudioClip lobbyBGM;

    private void Start()
    {
        AudioManager.instance.PlayBgm(lobbyBGM);

        if (SaveController.instance.saveData.curStage != 1)
            continueBtn.SetActive(true);
        else
            continueBtn.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone) {
            if (operation.progress >= 0.9f) {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    // 게임 시작 버튼 연동
    public void GameStartBtnClick()
    {
        SaveManager.DeleteAllSaves();
        StaticManager.StaticInit();
        LoadScene("2.Dungeon");

        SteamAchievement.Unlock("문을 열다");

        if (SteamUserStats.GetAchievement("지갑을 열다", out bool 과소비) && !과소비) {
            SteamUserStats.SetStat("지갑을 열다1", 0);
            SteamUserStats.StoreStats();
        }
    }

    // 이어하기 버튼 연동
    public void GameContinueBtnClick()
    {
        StaticManager.StaticInit();
        SaveController.instance.ApplyLoadedData();
        LoadScene("2.Dungeon");
    }

    public void GameExitBtnClick()
    {
        Application.Quit();
    }
}