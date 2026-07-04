using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationMsgUi : MonoBehaviour
{
    public static InformationMsgUi instance;

    public GameObject informationMsg;
    public TMP_Text text;
    public Button okBtn;

    public AudioClip setSound;

    private void Awake()
    {
        instance = this;
        okBtn.onClick.AddListener(OkBtnClick);
    }

    public void SetMsg(string msg)
    {
        AudioManager.instance.PlaySfx(setSound);
        text.text = msg;
        informationMsg.SetActive(true);
    }

    void OkBtnClick()
    {
        informationMsg.SetActive(false);
    }
}
