using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileText : MonoBehaviour, IPointerDownHandler {
    public Text output;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url) {
        using var request = UnityEngine.Networking.UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
            request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError) {
            output.text = $"Error: {request.error}";
        }
        else {
            output.text = request.downloadHandler.text;
        }
#else
            if (request.isNetworkError || request.isHttpError) {
                output.text = $"Error: {request.error}";
            } else {
                output.text = request.downloadHandler.text;
            }
#endif
    }
}