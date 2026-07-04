using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemData;

public class Item : Drag
{
    public ItemData data;
    public ItemType type;
    public int itemMoney;

    public AudioClip itemUseSound;
    public AnimationClip itemUseEffect;

    protected override void Awake()
    {
        base.Awake();

        gameObject.GetComponent<Image>().sprite = data.itemIcon;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        GameObject hit = eventData.pointerEnter;
        if (hit == null) {
            InitPos();
            return;
        }

        // 장비를 장착한 아군 유닛에게 사용 시
        if (hit.CompareTag("Friendly Unit") && hit.TryGetComponent<UnitDefault>(out UnitDefault unitComp) && unitComp.isEquipmentUnit) {
            UnitUpgradeStat itemStat = unitComp.itemStat;
            if (itemStat.maxHealth != 0 || itemStat.attackPower != 0 || itemStat.defense != 0 || itemStat.attackSpeed != 0 ||
                itemStat.lifeStealPer != 0 || itemStat.criticalPer != 0) {
                AudioManager.instance.PlaySfx(GameManager.instance.resourceClipList[3]);
                InitPos();
                return;
            }

            if (!GameManager.instance.SetCost(-data.cost)) {
                AudioManager.instance.PlaySfx(GameManager.instance.resourceClipList[3]);
                InitPos();
                return;
            }

            AudioManager.instance.PlaySfx(itemUseSound);
            switch (type) {
                case ItemType.HPPotion:
                    itemStat.maxHealth += data.heal;
                    unitComp.SetStateBar(UnitDefault.State.HPPotion, -2f);
                    break;
                case ItemType.ATKPotion:
                    itemStat.attackPower += data.ATK;
                    unitComp.SetStateBar(UnitDefault.State.ATKPotion, -2f);
                    break;
                case ItemType.DEFPotion:
                    itemStat.defense += data.DEF;
                    unitComp.SetStateBar(UnitDefault.State.DEFPotion, -2f);
                    break;
                case ItemType.DPSPotion:
                    itemStat.attackSpeed += data.DPS;
                    unitComp.SetStateBar(UnitDefault.State.DPSPotion, -2f);
                    break;
                case ItemType.LSPotion:
                    itemStat.lifeStealPer += data.LS;
                    unitComp.SetStateBar(UnitDefault.State.LSPotion, -2f);
                    break;
                case ItemType.CRTPotion:
                    itemStat.criticalPer += data.CRT;
                    unitComp.SetStateBar(UnitDefault.State.CRTPotion, -2f);
                    break;
            }

            GameObject effect = Instantiate(GameManager.instance.effectObj, unitComp.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(itemUseEffect);

            unitComp.UpdateStatInfo();

            DeckManager.instance.CardDraw(beginParent.parent);

            Destroy(beginParent.gameObject);
            Destroy(gameObject);

            AnalyticsInitializer.instance.UseItem(data.itemID);
        }
        // 쓰레기통에 버렸을 때
        else if (hit.CompareTag("Trash Can")) {
            if (!GameManager.instance.SetGold(-200)) {
                InitPos();
                return;
            }

            RemainingDeck.instance.useDestroyCardGold += 200;

            DeckManager.instance.CardDraw(beginParent.parent);
            Destroy(beginParent.gameObject);
            Destroy(gameObject);
        }
        else {
            InitPos();
        }
    }
}