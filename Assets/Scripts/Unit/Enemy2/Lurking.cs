using TMPro;
using UnityEngine;

public class Lurking : UnitDefault
{
    [Header("잠복")]
    public int count;
    public float incHealthAmount;
    public float incAttackAmount;
    public float incDefenseAmount;
    public float incAttackSpeedAmount;
    public GameObject lurkingState;
    public TextMeshProUGUI lurkingCntTxt;

    int curFriendlyCount;
    int useSkillCnt;

    protected override void Start()
    {
        base.Start();

        lurkingState.SetActive(true);
        curFriendlyCount = enemies.Count;
    }

    protected override void FixedUpdate()
    {
        if (useSkillCnt == count) {
            base.FixedUpdate();
        }
        else {
            if (enemies.Count != curFriendlyCount) {
                if (!isDeath && enemies.Count < curFriendlyCount)
                    UseSkill();

                curFriendlyCount = enemies.Count;
            }
        }
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());

        lurkingCntTxt.SetText((++useSkillCnt).ToString());

        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, transform.position);

        SetStat(maxHealthStat, incHealthAmount, false, true);
        SetStat(attackPowerStat, incAttackAmount, false, true);
        SetStat(defenseStat, incDefenseAmount, false, true);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true);
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"아군 유닛이 사망할 때마다 체력이 <color=green>{incHealthAmount}</color>, 공격력이 <color=green>{incAttackAmount}</color>, 방어력이 <color=green>{incDefenseAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>만큼 상승합니다.\n" +
                    $"아군 유닛이 {count}명 사망하기 전까지 제자리에서 대기합니다.";

        ENSkillDesc = $"Each time an allied unit dies, increases max health by <color=green>{incHealthAmount}</color>, attack by <color=green>{incAttackAmount}</color>, defense by <color=green>{incDefenseAmount}</color>, and attack speed by <color=green>{incAttackSpeedAmount}</color>.\n" +
                       $"Waits in place until {count} allied units have died.";

        FRSkillDesc = $"Jedes Mal, wenn eine verbündete Einheit stirbt, erhöht sich die maximale Gesundheit um <color=green>{incHealthAmount}</color>, der Angriff um <color=green>{incAttackAmount}</color>, die Verteidigung um <color=green>{incDefenseAmount}</color> und die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color>.\n" +
                        $"Wartet an Ort und Stelle, bis {count} verbündete Einheiten gestorben sind.";

        ITSkillDesc = $"Ogni volta che un'unità alleata muore, aumenta la salute massima di <color=green>{incHealthAmount}</color>, l'attacco di <color=green>{incAttackAmount}</color>, la difesa di <color=green>{incDefenseAmount}</color> e la velocità d'attacco di <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Rimane fermo finché non sono morte {count} unità alleate.";

        DESkillDesc = $"Jedes Mal, wenn eine verbündete Einheit stirbt, erhöht sich die maximale Gesundheit um <color=green>{incHealthAmount}</color>, der Angriff um <color=green>{incAttackAmount}</color>, die Verteidigung um <color=green>{incDefenseAmount}</color> und die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Wartet an Ort und Stelle, bis {count} verbündete Einheiten gestorben sind.";

        ESSkillDesc = $"Cada vez que una unidad aliada muere, aumenta la salud máxima en <color=green>{incHealthAmount}</color>, el ataque en <color=green>{incAttackAmount}</color>, la defensa en <color=green>{incDefenseAmount}</color> y la velocidad de ataque en <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Espera en su posición hasta que hayan muerto {count} unidades aliadas.";

        JASkillDesc = $"味方ユニットが死亡するたびに、最大体力が<color=green>{incHealthAmount}</color>、攻撃力が<color=green>{incAttackAmount}</color>、防御力が<color=green>{incDefenseAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>増加します。\n" +
                      $"{count}体の味方ユニットが死亡するまで、その場で待機します。";

        PT_BRSkillDesc = $"Cada vez que uma unidade aliada morre, aumenta a saúde máxima em <color=green>{incHealthAmount}</color>, o ataque em <color=green>{incAttackAmount}</color>, a defesa em <color=green>{incDefenseAmount}</color> e a velocidade de ataque em <color=green>{incAttackSpeedAmount}</color>.\n" +
                          $"Espera no local até que {count} unidades aliadas tenham morrido.";

        RUSkillDesc = $"Каждый раз, когда умирает союзная единица, увеличивает здоровье на <color=green>{incHealthAmount}</color>, атаку на <color=green>{incAttackAmount}</color>, защиту на <color=green>{incDefenseAmount}</color> и скорость атаки на <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Ожидает на месте, пока не погибнут {count} союзных юнитов.";

        ZH_HANSSkillDesc = $"每当一名友方单位死亡时，最大生命值增加<color=green>{incHealthAmount}</color>，攻击力增加<color=green>{incAttackAmount}</color>，防御增加<color=green>{incDefenseAmount}</color>，攻击速度增加<color=green>{incAttackSpeedAmount}</color>。\n" +
                           $"在有 {count} 名友方单位死亡之前，停留在原地等待。";
    }
}