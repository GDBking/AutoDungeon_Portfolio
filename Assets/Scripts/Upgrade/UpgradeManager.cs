using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public List<UpgradeOptionData> datas1;
    public List<UpgradeOptionData> datas2;
    public List<UpgradeOptionData> datas3;
    public GameObject upgradeUI;
    public List<UpgradeSlotUI> slotUI;
    public Button allRerollButton;
    public Button confirmButton;
    GameObject upStatPanel;

    [HideInInspector]
    public UnitDefault upgradeUnit;
    public AnimationClip upgradeEffect;

    readonly Dictionary<int, List<UpgradeOptionData>> allDatas = new();
    readonly List<List<UpgradeOptionData>> allDefaultDatas = new();

    public AudioClip upgradeSound1;
    public AudioClip upgradeSound2;

    [HideInInspector]
    public bool isAllReroll;

    public Transform tempUnitTransform;

    private void Awake()
    {
        instance = this;
        upStatPanel = GameManager.instance.upStatPanel;
        Init();
        allRerollButton.onClick.AddListener(() => AllRerollButton());
        confirmButton.onClick.AddListener(() => ConfirmButton());
    }

    void Init()
    {
        allDefaultDatas.Add(datas1);
        allDefaultDatas.Add(datas2);
        allDefaultDatas.Add(datas3);

        foreach (var datas in allDefaultDatas) {
            foreach (var data in datas) {
                switch (data.upgradeType) {
                    case UpgradeOptionData.UpgradeType.healthPer:
                        data.upgradeAction = unitComp =>
                        unitComp.maxHealthStat.coeff *= (100f + data.value) / 100f;
                        break;
                    case UpgradeOptionData.UpgradeType.attackPer:
                        data.upgradeAction = unitComp =>
                        unitComp.attackPowerStat.coeff *= (100f + data.value) / 100f;
                        break;
                    case UpgradeOptionData.UpgradeType.defensePer:
                        data.upgradeAction = unitComp =>
                        unitComp.defenseStat.extra += data.value;
                        break;
                    case UpgradeOptionData.UpgradeType.attackSpeedPer:
                        data.upgradeAction = unitComp =>
                        unitComp.attackSpeedStat.coeff *= (100f + data.value) / 100f;
                        break;
                    case UpgradeOptionData.UpgradeType.attackRange:
                        data.upgradeAction = unitComp =>
                        unitComp.attackRangeStat.extra += data.value;
                        break;
                    case UpgradeOptionData.UpgradeType.criticalPer:
                        data.upgradeAction = unitComp =>
                        unitComp.criticalPerStat.extra += data.value;
                        break;
                    case UpgradeOptionData.UpgradeType.lifeStealPer:
                        data.upgradeAction = unitComp =>
                        unitComp.lifeStealPerStat.extra += data.value;
                        break;
                    case UpgradeOptionData.UpgradeType.skill:
                        data.upgradeAction = unitComp =>
                        StaticManager.skillPoint[unitComp.unitIdx] += (int)data.value;
                        break;
                }
            }
        }
        allDatas[1] = allDefaultDatas[0];
        allDatas[2] = allDefaultDatas[1];
        allDatas[3] = allDefaultDatas[2];
    }

    int captureSkillPoint;
    public void SpecificUpgrade(UnitDefault unit)
    {
        upgradeUnit = unit;
        captureSkillPoint = StaticManager.skillPoint[unit.unitIdx];

        isAllReroll = false;
        upgradeUI.SetActive(true);
        SetTempUnit(true);
        upStatPanel.SetActive(true);

        foreach (UpgradeSlotUI slot in slotUI) {
            slot.gameObject.SetActive(true);
            slot.RandomSlot(unit, allDatas);
        }
    }

    void AllRerollButton()
    {
        allRerollButton.gameObject.SetActive(false);
        isAllReroll = true;
        foreach (var slot in slotUI)
            StartCoroutine(slot.RandomSlotRoutine());
    }

    public void ConfirmButton()
    {
        AudioManager.instance.PlaySfx(upgradeSound2);

        GameObject effect = Instantiate(GameManager.instance.UIEffectObj, upgradeUnit.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(upgradeEffect);

        upStatPanel.SetActive(false);

        upgradeUI.SetActive(false);

        foreach (var slot in slotUI)
            slot.rerollButton.gameObject.SetActive(true);
        allRerollButton.gameObject.SetActive(true);

        tempUnit.UpgradeApply();
        StaticManager.activeUnit.UpdateStatInfo();

        DeckManager.instance.deckObj.GetComponent<Button>().interactable = true;
    }

    UnitDefault tempUnit;
    public void SetTempUnit(bool isInit = false)
    {
        if (tempUnit != null)
            Destroy(tempUnit.gameObject);

        tempUnit = Instantiate(upgradeUnit.gameObject, tempUnitTransform).GetComponent<UnitDefault>();
        StaticManager.skillPoint[tempUnit.unitIdx] = captureSkillPoint;

        if (!isInit) {
            foreach (var slot in slotUI)
                slot.ConfirmButtonTemp(tempUnit);
        }

        tempUnit.UpdateStatInfo(true, true);
    }
}