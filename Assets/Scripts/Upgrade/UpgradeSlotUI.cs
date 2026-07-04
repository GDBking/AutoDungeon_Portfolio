using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlotUI : MonoBehaviour
{
    public Image slot;
    public Image icon;
    public ScrollRect descScrollRect;
    public TextMeshProUGUI nameText, descText;
    public Button rerollButton;

    public AudioClip slotMachineSound1;
    public AudioClip slotMachineSound2;
    public AudioClip slotMachineSound3;
    bool isReroll;

    UnitDefault unit;
    UpgradeOptionData upgradeData;
    Dictionary<int, List<UpgradeOptionData>> allDatas;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        rerollButton.onClick.AddListener(() => 
        {
            rerollButton.gameObject.SetActive(false);
            isReroll = true;
            StartCoroutine(RandomSlotRoutine()); 
        });
    }

    public void RandomSlot(UnitDefault unit, Dictionary<int, List<UpgradeOptionData>> allDatas)
    {
        this.unit = unit;
        this.allDatas = allDatas;
        StartCoroutine(RandomSlotRoutine());
    }

    readonly WaitForSeconds wait = new(0.1f);
    public IEnumerator RandomSlotRoutine()
    {
        rerollButton.gameObject.SetActive(false);
        UpgradeManager.instance.allRerollButton.gameObject.SetActive(false);
        UpgradeManager.instance.confirmButton.gameObject.SetActive(false);
        slot.color = Color.white;
        nameText.text = null;
        descText.text = "";

        int tier = 1;

        if (SaveController.instance.saveDataAchi.isCheck[8]) {
            int tier1 = 80;
            int tier2 = 15;
            int tier3 = 5;
            int unitLevel = unit.Level;

            if (unitLevel >= 1) {
                tier1 -= unitLevel * 5;
                tier2 += (unitLevel / 2) * 5;
                if (unitLevel % 2 == 1)
                    tier2 += 5;
                tier3 += (unitLevel / 2) * 5;
            }

            if (!SaveController.instance.saveDataAchi.isCheck[9]) {
                tier2 += tier3;
            }

            int randTier = Random.Range(0, 101);
            switch (randTier) {
                case int n when (n < tier1):
                    tier = 1;
                    break;
                case int n when (n < tier1 + tier2):
                    tier = 2;
                    break;
                case int n when (n < tier1 + tier2 + tier3):
                    tier = 3;
                    break;
            }
        }

        StaticManager.slotmachineCnt++;
        for (int i = 0; i < 10 * tier; i++)
        {
            audioSource.volume = AudioManager.instance.sfxVolume;

            if (i == 0)
                audioSource.clip = slotMachineSound1;
            else if (i == 10) {
                //audioSource.clip = slotMachineSound2;
                slot.color = new Color32(76, 175, 80, 255);
            }
            else if (i == 20) {
                //audioSource.clip = slotMachineSound3;
                slot.color = new Color32(255, 193, 7, 255);
            }
            audioSource.Play();

            upgradeData = allDatas[i / 10 + 1][Random.Range(0, allDatas[i / 10 + 1].Count)];
            icon.sprite = upgradeData.icon;
            yield return wait;
        }
        StaticManager.slotmachineCnt--;
        // 스킬 포인트 업그레이드 플래그
        if (upgradeData.upgradeType == UpgradeOptionData.UpgradeType.skill)
        {
            // 스킬 포인트 계수 설명
            descText.SetText(unit.GetSkillUpDesc());

            LayoutRebuilder.ForceRebuildLayoutImmediate(descScrollRect.content);
            StartCoroutine(SetScrollTopNextFrame());
        }
        else
        {
            descText.text = upgradeData.GetLocalizedDescription();
        }

        // 업그레이드 이름
        nameText.text = upgradeData.GetLocalizedUpgradeName();
        // 해당 슬롯 리롤을 하지 않았으면 리롤 버튼 활성화
        if (!isReroll && SaveController.instance.saveDataAchi.isCheck[0])
            rerollButton.gameObject.SetActive(true);
        // 모든 슬롯이 멈춰 있다면 업그레이드 버튼 활성화 및 임시 유닛 스탯 변경
        if (StaticManager.slotmachineCnt == 0) {
            UpgradeManager.instance.confirmButton.gameObject.SetActive(true);
            UpgradeManager.instance.SetTempUnit();
            // 전체 리롤을 하지 않았다면 전체 리롤 버튼 활성화
            if (!UpgradeManager.instance.isAllReroll) {
                UpgradeManager.instance.allRerollButton.gameObject.SetActive(true);
            }
        }
    }

    public void ConfirmButton(UnitDefault unit)
    {
        upgradeData.upgradeAction(unit);
        gameObject.SetActive(false);
    }

    public void ConfirmButtonTemp(UnitDefault unit)
    {
        upgradeData.upgradeAction(unit);
    }

    IEnumerator SetScrollTopNextFrame()
    {
        yield return null;
        descScrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnEnable()
    {
        isReroll = false;
    }
}