using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RelicsReward : MonoBehaviour
{
    public List<Relics> relicsList;
    public List<Relics> enemyRelicsList;
    public List<Transform> rewardSlot;
    public GameObject getRewardBtn;
    public TextMeshProUGUI getRewardBtnTxt;
    public TextMeshProUGUI rewardTxt;
    public Transform dragCan;

    public AudioClip rewardEnableClip;
    public AudioClip rewardDisableClip;
    public AudioClip getFriendlyRelicsClip;
    public AudioClip getEnemyRelicsClip;

    RewardType rewardType;

    GameObject selectedRelics;

    bool isAchieveReward;

    public void GetRewardBtnClick()
    {
        getRewardBtn.SetActive(false);

        if (selectedRelics == null) {
            gameObject.SetActive(false);
            return;
        }

        foreach (Transform slot in rewardSlot) {
            if (slot.childCount != 1)
                slot.GetChild(1).GetComponent<Button>().enabled = false;
        }

        Relics relicsComp = selectedRelics.GetComponent<Relics>();
        if (rewardType == RewardType.eliteMonster) {
            foreach (Relics relics in relicsList) {
                if (relicsComp.title == relics.title) {
                    relicsComp.transform.SetParent(dragCan);

                    relicsComp.transform.DOMove(relics.transform.position, 1f)
                        .SetEase(Ease.InOutSine);
                        
                    relicsComp.transform.DOScale(1f, 1f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            AudioManager.instance.PlaySfx(getFriendlyRelicsClip);

                            foreach (RelicsProb relicsProb in GameManager.instance.relicsProbs) {
                                if (relicsComp.title == relicsProb.title) {
                                    relicsProb.prob++;
                                    RelicsManager.instance.SetRelicsProb();

                                    Destroy(relicsComp.gameObject);
                                    gameObject.SetActive(false);

                                    if (relicsProb.prob == 5)
                                        SteamAchievement.Unlock("운명을 고르다");

                                    break;
                                }
                            }
                        });

                    break;
                }
            }
        }
        else {
            foreach (Relics relics in enemyRelicsList) {
                if (relicsComp.title == relics.title) {
                    relicsComp.transform.SetParent(dragCan);

                    relicsComp.transform.DOMove(relics.transform.position, 1f)
                        .SetEase(Ease.InOutSine);
                        
                    relicsComp.transform.DOScale(1f, 1f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            AudioManager.instance.PlaySfx(getEnemyRelicsClip);

                            foreach (RelicsProb relicsProb in GameManager.instance.enemyRelicsProbs) {
                                if (relicsComp.title == relicsProb.title) {
                                    relicsProb.prob += 0.5f;
                                    RelicsManager.instance.SetRelicsProb();

                                    Destroy(relicsComp.gameObject);

                                    gameObject.SetActive(false);

                                    if (relicsProb.prob == 0f) {
                                        if (++StaticManager.relicsSealCnt == 2) {
                                            SteamAchievement.Unlock("봉인 완료");
                                        }
                                    }

                                    if (isAchieveReward) {
                                        isAchieveReward = false;
                                    }
                                    else if (SaveController.instance.saveDataAchi.isCheck[5] && Random.Range(1, 101) <= 50) {
                                        isAchieveReward = true;
                                        gameObject.SetActive(true);
                                    }
                                    break;
                                }
                            }
                        });

                    break;
                }
            }
        }
    }

    private void OnEnable()
    {
        List<RelicsProb> relicsProbs;

        rewardType = StaticManager.rewardType;
        if (rewardType == RewardType.eliteMonster)
        {
            relicsProbs = new(GameManager.instance.relicsProbs);
            ApplyLocalization();
        }
        else
        {
            relicsProbs = new(GameManager.instance.enemyRelicsProbs);

            for (int i = relicsProbs.Count - 1; i >= 0; i--)
            {
                if (relicsProbs[i].prob == 0)
                    relicsProbs.RemoveAt(i);
            }

            ApplyLocalization();

            getRewardBtn.SetActive(false);
        }

        for (int i = 0, j = relicsProbs.Count - 1; i < rewardSlot.Count; i++, j--)
        {
            int randNum = Random.Range(0, j + 1);
            relicsProbs.TrySwap(randNum, j, out _);

            GameObject prefab = null;
            for (int k = rewardType == RewardType.eliteMonster ? 0 : 11; k < GameManager.instance.relicsList.Count; k++)
            {
                if (relicsProbs[j].title == GameManager.instance.relicsList[k].GetComponent<Relics>().title)
                {
                    prefab = GameManager.instance.relicsList[k];
                    break;
                }
            }

            GameObject relics = Instantiate(prefab, rewardSlot[i]);
            relics.transform.localScale *= 3f;

            relics.AddComponent<Button>().onClick.AddListener(() =>
            {
                foreach (Transform slot in rewardSlot)
                {
                    if (slot.transform == relics.transform.parent)
                    {
                        if (slot.GetChild(0).gameObject.activeSelf)
                        {
                            AudioManager.instance.PlaySfx(rewardDisableClip);

                            slot.GetChild(0).gameObject.SetActive(false);
                            selectedRelics = null;

                            if (rewardType != RewardType.eliteMonster) {
                                getRewardBtn.SetActive(false);
                            }
                            else {
                                getRewardBtnTxt.SetText(
                                    Localization.instance.index == 0 ? "넘어가기" :
                                    Localization.instance.index == 1 ? "Skip" :
                                    Localization.instance.index == 2 ? "Passer" :
                                    Localization.instance.index == 3 ? "Salta" :
                                    Localization.instance.index == 4 ? "Überspringen" :
                                    Localization.instance.index == 5 ? "Saltar" :
                                    Localization.instance.index == 6 ? "スキップ" :
                                    Localization.instance.index == 7 ? "Pular" :
                                    Localization.instance.index == 8 ? "Пропустить" :
                                                                       "跳过"
                                );
                            }
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(rewardEnableClip);

                            slot.GetChild(0).gameObject.SetActive(true);
                            selectedRelics = relics;

                            getRewardBtn.SetActive(true);

                            getRewardBtnTxt.SetText(
                                Localization.instance.index == 0 ? "유물 선택" :
                                Localization.instance.index == 1 ? "Select Relic" :
                                Localization.instance.index == 2 ? "Choisir la relique" :
                                Localization.instance.index == 3 ? "Seleziona reliquia" :
                                Localization.instance.index == 4 ? "Relikt wählen" :
                                Localization.instance.index == 5 ? "Seleccionar reliquia" :
                                Localization.instance.index == 6 ? "遺物を選択" :
                                Localization.instance.index == 7 ? "Selecionar relíquia" :
                                Localization.instance.index == 8 ? "Выбрать реликвию" :
                                                                   "选择遗物"
                            );
                        }
                    }
                    else
                    {
                        slot.GetChild(0).gameObject.SetActive(false);
                    }
                }
            });
        }
    }

    private void OnDisable()
    {
        foreach (Transform slot in rewardSlot) {
            slot.GetChild(0).gameObject.SetActive(false);

            if (slot.childCount != 1)
                Destroy(slot.GetChild(1).gameObject);
        }

        ApplyLocalization();
        getRewardBtn.SetActive(true);
    }
    void ApplyLocalization()
    {
        int idx = Localization.instance.index;

        // rewardTxt
        if (rewardType == RewardType.eliteMonster)
        {
            rewardTxt.SetText(
                idx == 0 ? "등장 확률을 높일 유물을 선택하세요." :
                idx == 1 ? "Select a relic to increase spawn chance." :
                idx == 2 ? "Sélectionnez une relique pour augmenter le taux d’apparition." :
                idx == 3 ? "Seleziona una reliquia per aumentare la probabilità di comparsa." :
                idx == 4 ? "Wähle ein Relikt, um die Erscheinungsrate zu erhöhen." :
                idx == 5 ? "Selecciona una reliquia para aumentar la probabilidad de aparición." :
                idx == 6 ? "出現確率を上げる遺物を選択してください。" :
                idx == 7 ? "Selecione uma relíquia para aumentar a chance de aparecimento." :
                idx == 8 ? "Выберите реликвию для увеличения шанса появления." :
                           "选择一个遗物来提高出现概率。"
            );
        }
        else
        {
            rewardTxt.SetText(
                idx == 0 ? "파괴할 유물을 선택하세요." :
                idx == 1 ? "Select a relic to destroy." :
                idx == 2 ? "Sélectionnez une relique à détruire." :
                idx == 3 ? "Seleziona una reliquia da distruggere." :
                idx == 4 ? "Wähle ein Relikt zum Zerstören." :
                idx == 5 ? "Selecciona una reliquia para destruir." :
                idx == 6 ? "破壊する遺物を選択してください。" :
                idx == 7 ? "Selecione uma relíquia para destruir." :
                idx == 8 ? "Выберите реликвию для уничтожения." :
                           "选择一个要摧毁的遗物。"
            );
        }

        // 버튼 텍스트 (선택 안 했을 때 기본값)
        getRewardBtnTxt.SetText(
            idx == 0 ? "넘어가기" :
            idx == 1 ? "Skip" :
            idx == 2 ? "Passer" :
            idx == 3 ? "Salta" :
            idx == 4 ? "Überspringen" :
            idx == 5 ? "Saltar" :
            idx == 6 ? "スキップ" :
            idx == 7 ? "Pular" :
            idx == 8 ? "Пропустить" :
                       "跳过"
        );
    }
}