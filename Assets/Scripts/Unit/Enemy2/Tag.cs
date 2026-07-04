using System.Collections.Generic;
using UnityEngine;

public class Tag : UnitDefault
{
    [Header("표식")]
    public float decAttackAmount;
    public float decDefenseAmount;
    public float decAttackSpeedPer;

    Rank maxTier;
    List<UnitDefault> enemiesList = new();

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (friends.Count == 0)
            return;

        maxTier = Rank.common;
        enemiesList.Clear();
        foreach (UnitDefault unitComp in friends) {
            if (unitComp.rank > maxTier) {
                maxTier = unitComp.rank;

                enemiesList = new() { unitComp };
            }
            else if (unitComp.rank == maxTier) {
                enemiesList.Add(unitComp);
            }
        }

        SkillTarget = enemiesList[Random.Range(0, enemiesList.Count)].gameObject;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        GameObject effect = Instantiate(attackObj, target.transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        switch (Random.Range(0, 3)) {
            case 0:
                unitComp.SetStat(unitComp.attackPowerStat, decAttackAmount, false, false);
                unitComp.SetStateBar(State.decAttack, -2f);
                break;
            case 1:
                unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false);
                unitComp.SetStateBar(State.decDefense, -2f);
                break;
            case 2:
                unitComp.SetStat(unitComp.attackSpeedStat, decAttackSpeedPer, true, false);
                unitComp.SetStateBar(State.decAttackSpeed, -2f);
                break;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"가장 강한 적 유닛(티어가 가장 높은 유닛)에게 표식을 날려 다음 세 가지 효과 중 한 가지를 랜덤하게 적용합니다.(중첩 가능)\n" +
                    $"1. 공격력 <color=red>{decAttackAmount}</color>감소\n" +
                    $"2. 방어력 <color=red>{decDefenseAmount}</color>감소\n" +
                    $"3. 공격 속도 <color=red>{decAttackSpeedPer}%</color> 감소";

        ENSkillDesc = $"Marks the strongest enemy (highest tier) and randomly applies one of three effects (can stack):\n" +
                      $"1. Decrease attack by <color=red>{decAttackAmount}</color>\n" +
                      $"2. Decrease defense by <color=red>{decDefenseAmount}</color>\n" +
                      $"3. Decrease attack speed by <color=red>{decAttackSpeedPer}%</color>";

        FRSkillDesc = $"Marque l'ennemi le plus fort (de plus haut rang) et applique au hasard l'un des trois effets suivants (peut s'empiler) :\n" +
                      $"1. Réduit l'attaque de <color=red>{decAttackAmount}</color>\n" +
                      $"2. Réduit la défense de <color=red>{decDefenseAmount}</color>\n" +
                      $"3. Réduit la vitesse d'attaque de <color=red>{decAttackSpeedPer}%</color>";

        ITSkillDesc = $"Segna il nemico più forte (di grado più alto) e applica casualmente uno dei tre effetti (può accumularsi):\n" +
                      $"1. Riduce l'attacco di <color=red>{decAttackAmount}</color>\n" +
                      $"2. Riduce la difesa di <color=red>{decDefenseAmount}</color>\n" +
                      $"3. Riduce la velocità d'attacco di <color=red>{decAttackSpeedPer}%</color>";

        DESkillDesc = $"Kennzeichnet den stärksten Gegner (höchster Rang) und wendet zufällig einen von drei Effekten an (stapelbar):\n" +
                      $"1. Angriff um <color=red>{decAttackAmount}</color> verringern\n" +
                      $"2. Verteidigung um <color=red>{decDefenseAmount}</color> verringern\n" +
                      $"3. Angriffsgeschwindigkeit um <color=red>{decAttackSpeedPer}%</color> verringern";

        ESSkillDesc = $"Marca al enemigo más fuerte (de mayor nivel) y aplica aleatoriamente uno de tres efectos (puede acumularse):\n" +
                      $"1. Reducir ataque en <color=red>{decAttackAmount}</color>\n" +
                      $"2. Reducir defensa en <color=red>{decDefenseAmount}</color>\n" +
                      $"3. Reducir la velocidad de ataque en <color=red>{decAttackSpeedPer}%</color>";

        JASkillDesc = $"最も強い敵ユニット（最も高いティア）に印を付け、次の3つの効果のうち1つをランダムに適用します（重複可能）：\n" +
                      $"1. 攻撃力を<color=red>{decAttackAmount}</color>減少\n" +
                      $"2. 防御力を<color=red>{decDefenseAmount}</color>減少\n" +
                      $"3. 攻撃速度を<color=red>{decAttackSpeedPer}%</color>減少";

        PT_BRSkillDesc = $"Marca o inimigo mais forte (maior tier) e aplica aleatoriamente um dos três efeitos (pode empilhar):\n" +
                          $"1. Reduz o ataque em <color=red>{decAttackAmount}</color>\n" +
                          $"2. Reduz a defesa em <color=red>{decDefenseAmount}</color>\n" +
                          $"3. Reduz a velocidade de ataque em <color=red>{decAttackSpeedPer}%</color>";

        RUSkillDesc = $"Отмечает самого сильного врага (высший ранг) и случайно применяет один из трёх эффектов (может складываться):\n" +
                      $"1. Снизить атаку на <color=red>{decAttackAmount}</color>\n" +
                      $"2. Снизить защиту на <color=red>{decDefenseAmount}</color>\n" +
                      $"3. Снизить скорость атаки на <color=red>{decAttackSpeedPer}%</color>";

        ZH_HANSSkillDesc = $"标记最强的敌人（最高等级）并随机施加三种效果之一（可叠加）：\n" +
                           $"1. 攻击力降低 <color=red>{decAttackAmount}</color>\n" +
                           $"2. 防御力降低 <color=red>{decDefenseAmount}</color>\n" +
                           $"3. 攻击速度降低 <color=red>{decAttackSpeedPer}%</color>";
    }
}
