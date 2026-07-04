using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AchieveShop : MonoBehaviour
{
    public AchievementData achieveData;
    public int idx;
    public int point;

    [HideInInspector] public Image iconImg;
    [HideInInspector] public Image btnImg;
    TextMeshProUGUI title;
    TextMeshProUGUI desc;

    public Sprite onButton;
    public Sprite offButton;

    private void Awake()
    {
        iconImg = transform.GetChild(0).GetComponent<Image>();
        btnImg = transform.GetChild(3).GetComponent<Image>();
        title = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        desc = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        btnImg.GetComponent<Button>().onClick.AddListener(() => BtnClick());
    }

    private void Start()
    {
        if (SaveController.instance.saveDataAchi.isCheck[idx]) {
            iconImg.color = Color.white;
            //btnImg.color = Color.white;
            btnImg.sprite = onButton;

            if (idx == 1 || idx == 6 || idx == 8) {
                transform.parent.GetChild(idx + 1).gameObject.SetActive(true);
            }
        }

        SetAchiPointTxt();
    }

    public void BtnClick()
    {
        // 비활성화 상태인 버튼 클릭 시
        if (!SaveController.instance.saveDataAchi.isCheck[idx]) {
            // 포인트가 충분할 시
            if (SaveController.instance.saveDataAchi.achiPoint >= point) {
                SaveController.instance.saveDataAchi.achiPoint -= point;
                SaveController.instance.saveDataAchi.isCheck[idx] = true;
                SetAchiPointTxt();

                iconImg.color = Color.white;
                //btnImg.color = Color.white;
                btnImg.sprite = onButton;

                if (idx == 1 || idx == 6 || idx == 8) {
                    transform.parent.GetChild(idx + 1).gameObject.SetActive(true);
                }
            }
        }
        // 활성화 상태인 버튼 클릭 시
        else {
            SaveController.instance.saveDataAchi.isCheck[idx] = false;
            SaveController.instance.saveDataAchi.achiPoint += point;
            SetAchiPointTxt();

            iconImg.color = Color.black;
            //btnImg.color = Color.black;
            btnImg.sprite = offButton;

            if (idx == 1 || idx == 6 || idx == 8) {
                Transform nextUp = transform.parent.GetChild(idx + 1);
                nextUp.gameObject.SetActive(false);

                // 다음 단계 업그레이드를 활성화 한 상태일 경우
                if (SaveController.instance.saveDataAchi.isCheck[idx + 1]) {
                    AchieveShop achieveShop = nextUp.GetComponent<AchieveShop>();

                    SaveController.instance.saveDataAchi.isCheck[achieveShop.idx] = false;
                    SaveController.instance.saveDataAchi.achiPoint += achieveShop.point;
                    SetAchiPointTxt();

                    achieveShop.iconImg.color = Color.black;
                    //achieveShop.btnImg.color = Color.black;
                    achieveShop.btnImg.sprite = offButton;
                }
            }
        }
    }

    void SetAchiPointTxt()
    {
        AchievePointTxt.instance.SetAchiPointTxt();
    }

    private void OnEnable()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        switch (code[..2]) {
            case "ko":
                title.SetText(achieveData.Title);
                desc.SetText(achieveData.Desc);
                break;
            case "en":
                title.SetText(achieveData.ENTitle);
                desc.SetText(achieveData.ENDesc);
                break;
            case "fr":
                title.SetText(achieveData.FRTitle);
                desc.SetText(achieveData.FRDesc);
                break;
            case "it":
                title.SetText(achieveData.ITTitle);
                desc.SetText(achieveData.ITDesc);
                break;
            case "de":
                title.SetText(achieveData.DETitle);
                desc.SetText(achieveData.DEDesc);
                break;
            case "es":
                title.SetText(achieveData.ESTitle);
                desc.SetText(achieveData.ESDesc);
                break;
            case "ja":
                title.SetText(achieveData.JATitle);
                desc.SetText(achieveData.JADesc);
                break;
            case "pt":
                title.SetText(achieveData.PT_BRTitle);
                desc.SetText(achieveData.PT_BRDesc);
                break;
            case "ru":
                title.SetText(achieveData.RUTitle);
                desc.SetText(achieveData.RUDesc);
                break;
            case "zh":
                title.SetText(achieveData.ZH_HANSTitle);
                desc.SetText(achieveData.ZH_HANSDesc);
                break;
            default:
                title.SetText(achieveData.ENTitle);
                desc.SetText(achieveData.ENDesc);
                break;
        }
    }
}
