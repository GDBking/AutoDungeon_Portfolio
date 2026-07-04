using UnityEngine;
using UnityEngine.UI;

public class UiGaugeBar : MonoBehaviour
{
    Image hpImage;
    Image mpImage;

    private void Awake()
    {
        hpImage = transform.Find("GaugeBar/HpBar/Hp").GetComponent<Image>();
        mpImage = transform.Find("GaugeBar/MpBar/Mp").GetComponent<Image>();
    }

    public void UpdateHpBar(float maxHp, float curHp)
    {
        hpImage.fillAmount = curHp / maxHp;
    }

    public void UpdateMpBar(float maxMp, float curMp)
    {
        if (maxMp == 0)
            return;

        mpImage.fillAmount = curMp / maxMp;
    }
}