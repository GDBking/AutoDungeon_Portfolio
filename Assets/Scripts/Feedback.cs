using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using SFB; // StandaloneFileBrowser 사용

public class Feedback : MonoBehaviour
{
    public TMP_InputField feedbackInputField;
    public Button selectImagesButton;
    public Button sendButton;

    // 이미지 이름 리스트를 보여줄 부모 UI (Vertical Layout Group 붙은 GameObject)
    public Transform fileNameListContainer;

    // 이미지 이름 + 삭제 버튼 UI 프리팹 (Text + Button 포함)
    public GameObject fileNameItemPrefab;

    public GameObject loadingPanel;

    private readonly List<Texture2D> selectedTextures = new();
    private readonly List<string> selectedFileNames = new();

    [System.Serializable]
    public class FeedbackData
    {
        public string feedbackText;
        public List<string> images = new();
        public List<string> fileNames = new();
        public string steamId;
    }

    private string steamUserId = "unknown";

    void Start()
    {
        // 스팀 아이디 가져오기 예시 (Steamworks 초기화 체크)
        if (SteamManager.Initialized) {
            steamUserId = Steamworks.SteamUser.GetSteamID().ToString();
        }
        else {
            Debug.LogWarning("Steam API not initialized!");
            steamUserId = "unknown";
        }

        selectImagesButton.onClick.AddListener(SelectImageFiles);
        sendButton.onClick.AddListener(() => StartCoroutine(SendFeedback()));
    }

    void SelectImageFiles()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Select Images", "", extensions, true);

        if (paths.Length == 0) return;

        foreach (var path in paths) {
            if (!File.Exists(path)) continue;

            if (selectedTextures.Count >= 5)
            {
                if (Localization.instance.index == 0)
                    InformationMsgUi.instance.SetMsg("최대 5장까지 추가 가능합니다.");
                else if (Localization.instance.index == 1)
                    InformationMsgUi.instance.SetMsg("You can upload up to 5 images.");
                break;
            }

            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new(2, 2);
            if (tex.LoadImage(data)) {
                selectedTextures.Add(tex);
                selectedFileNames.Add(Path.GetFileName(path));
                AddFileNameItemUI(Path.GetFileName(path));
            }
        }
    }

    void AddFileNameItemUI(string fileName)
    {
        GameObject item = Instantiate(fileNameItemPrefab, fileNameListContainer);

        TMP_Text text = item.GetComponentInChildren<TMP_Text>();
        text.text = fileName;

        Button deleteButton = item.GetComponentInChildren<Button>();
        deleteButton.onClick.AddListener(() =>
        {
            int index = -1;
            for (int i = 0; i < fileNameListContainer.childCount; i++) {
                if (fileNameListContainer.GetChild(i).gameObject == item) {
                    index = i;
                    break;
                }
            }
            if (index >= 0) {
                selectedTextures.RemoveAt(index);
                selectedFileNames.RemoveAt(index);
                Destroy(item);
            }
        });
    }

    IEnumerator SendFeedback()
    {
        if (string.IsNullOrWhiteSpace(feedbackInputField.text))
        {
            if (Localization.instance.index == 0)
                InformationMsgUi.instance.SetMsg("피드백 내용을 입력해 주세요.");
            else if (Localization.instance.index == 1)
                InformationMsgUi.instance.SetMsg("Please enter your feedback.");
            yield break;
        }

        loadingPanel.SetActive(true);

        FeedbackData data = new()
        {
            feedbackText = feedbackInputField.text
        };

        for (int i = 0; i < selectedTextures.Count; i++) {
            string base64 = System.Convert.ToBase64String(selectedTextures[i].EncodeToPNG());
            data.images.Add(base64);
            data.fileNames.Add(selectedFileNames[i]);
        }

        data.steamId = steamUserId;

        string json = JsonConvert.SerializeObject(data);

        using UnityWebRequest request = new("https://script.google.com/macros/s/AKfycbxYsVr7QoC1FRebtdJtiGBxtFUbHjmLWFl4ZXmI0ChWg17sekm9Et40pdR1YyH6-jPHcw/exec", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        loadingPanel.SetActive(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (Localization.instance.index == 0)
                InformationMsgUi.instance.SetMsg($"피드백 전송 실패: {request.error}");
            else if (Localization.instance.index == 1)
                InformationMsgUi.instance.SetMsg($"Failed to send feedback: {request.error}");
        }
        else
        {
            if (Localization.instance.index == 0)
                InformationMsgUi.instance.SetMsg("피드백 전송 성공!\n소중한 의견 감사합니다.");
            else if (Localization.instance.index == 1)
                InformationMsgUi.instance.SetMsg($"Your feedback has been sent!\nThank you for your valuable input.");

            // 초기화
            selectedTextures.Clear();
            selectedFileNames.Clear();

            // UI 목록 전부 삭제
            foreach (Transform child in fileNameListContainer)
            {
                Destroy(child.gameObject);
            }

            feedbackInputField.text = "";
        }
    }
}