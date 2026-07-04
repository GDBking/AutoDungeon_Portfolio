using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Steamworks;

public class CloudLogger : MonoBehaviour
{
    public static CloudLogger instance;

    private readonly string url = "https://theseven-log-538369852576.asia-northeast3.run.app";
    private readonly string apiKey = "pZffzJRcYCNOeZAD94LV";
    private readonly string secretKey = "B4Oey2ziw1t46cHY8xLrwyDpb1SRzK";

    private void Awake()
    {
        instance = this;
    }

    public void SendGameLog()
    {
        StartCoroutine(PostLog());
    }

    IEnumerator PostLog()
    {
        LogWrapper wrapper = new()
        {
            saveData = SaveController.instance.saveData,
            saveDataAchi = SaveController.instance.saveDataAchi
        };

        string json = JsonUtility.ToJson(wrapper);
        string timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        string signature = HMACUtility.GenerateSignature(json + timestamp, secretKey);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = 10;

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);
        request.SetRequestHeader("x-timestamp", timestamp);
        request.SetRequestHeader("x-signature", signature);
        request.SetRequestHeader("steamid", SteamUser.GetSteamID().ToString());
        request.SetRequestHeader("stage", StaticManager.curStage.ToString());

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("로그 전송 성공");
        else
            Debug.LogError($"에러: {request.responseCode} / {request.downloadHandler.text}");
    }


    [System.Serializable]
    public class LogWrapper
    {
        public SaveData saveData;
        public SaveDataAchi saveDataAchi;
    }
}