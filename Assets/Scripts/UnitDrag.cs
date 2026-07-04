using UnityEngine;
using UnityEngine.EventSystems;

public class UnitDrag : Drag
{
    UnitDefault unitComp;

    protected override void Awake()
    {
        base.Awake();

        unitComp = GetComponent<UnitDefault>();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        GameObject dropObj = eventData.pointerEnter;
        if (dropObj == null) {
            InitPos();
            return;
        }

        // 필드에 장비를 장착한 유닛을 드랍 했으면 통과(기본 유닛은 장비를 장착한 것으로 간주)
        if (dropObj.CompareTag("Field") && unitComp.isEquipmentUnit) {
            transform.SetParent(beginParent);
            return;
        }
        // 필드의 자식으로 있는 아군 유닛에게 드랍했을 때
        else if (dropObj.CompareTag("Friendly Unit") && dropObj.transform.parent.CompareTag("Field")) {
            // 장비를 장착했으면 통과
            if (unitComp.isEquipmentUnit){
                transform.SetParent(beginParent);
                return;
            }
            // 장비를 장착하지 않았고 기본 유닛 위에 드랍했을 때
            else if (dropObj.TryGetComponent<DefaultUnit>(out DefaultUnit defaultUnitComp)) {
                if (!GameManager.instance.SetCost(-unitComp.cost)) {
                    InitPos();
                    return;
                }

                AudioManager.instance.PlaySfx(GameManager.instance.unitEquipClip);

                // 새로운 카드 드로우
                DeckManager.instance.CardDraw(beginParent.parent);

                // 부모를 필드로 변경
                transform.SetParent(dropObj.transform.parent);
                transform.position = dropObj.transform.position;
                // 장비 장착 완료 플래그
                unitComp.isEquipmentUnit = true;

                // 스테로이드 효과 옮겨주기
                unitComp.steroid *= defaultUnitComp.steroid;
                if (unitComp.steroid != 0) {
                    for (int i = 2; i <= unitComp.steroid; i *= 2) {
                        unitComp.SetStateBar(UnitDefault.State.steroid, -2f);
                    }
                }

                unitComp.itemStat = defaultUnitComp.itemStat;

                unitComp.SetPotionStateBar();

                GameManager.instance.SBTN.interactable = true;
                // 기본 유닛, 빈 카드 오브젝트 제거
                Destroy(dropObj);
                Destroy(beginParent.gameObject);

                AudioManager.instance.PlaySfx(defaultUnitComp.equipSound);

                GameObject effect = Instantiate(GameManager.instance.effectObj, transform);
                effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(defaultUnitComp.equipAnimClip);

                if (unitComp.rank == UnitDefault.Rank.legendary && ++StaticManager.legendaryUnitCnt == 3) {
                    SteamAchievement.Unlock("전설을 부르다");
                }
            }
            else {
                InitPos();
            }
        }
        // 쓰레기통에 버렸을 때
        else if (dropObj.CompareTag("Trash Can")) {
            // 필드에 배치한 유닛은 코스트 소모 없이 버리기 가능
            if (unitComp.isEquipmentUnit) {
                Destroy(gameObject);

                if (GameManager.instance.unitField.childCount == 0)
                    GameManager.instance.SBTN.interactable = false;

                if (unitComp.rank == UnitDefault.Rank.legendary)
                    StaticManager.legendaryUnitCnt--;

                return;
            }
            else if (!GameManager.instance.SetGold(-200)) {
                InitPos();
                return;
            }

            RemainingDeck.instance.useDestroyCardGold += 200;

            // 새로운 카드 드로우
            DeckManager.instance.CardDraw(beginParent.parent);

            // 카드 제거
            Destroy(beginParent.gameObject);
            Destroy(gameObject);
        }
        // 그 외에 드래그 전 위치로 되돌리기
        else {
            InitPos();
        }
    }
}