using System.Collections;
using UnityEngine;

public class Queen : UnitDefault
{
    [Header("여왕")]
    public float incAttackPer;
    public float incAttackSpeedPer;
    public float incSpeedPer;

    bool isQueen;
    UnitDefault hitUnit;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(ChaseRoutine());
    }

    protected override void UseSkill()
    {
        if (!isQueen) {
            isQueen = true;
            StartCoroutine(OnSkillAnim());
        }
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        foreach (UnitDefault friendly in enemies) {
            if (friendly == this)
                continue;

            friendly.SetStat(friendly.attackPowerStat, incAttackPer, true, true);
            friendly.SetStat(friendly.attackSpeedStat, incAttackSpeedPer, true, true);
            friendly.SetStat(friendly.speedStat, incSpeedPer, true, true);
            friendly.SetStateBar(State.incAttack, -2f);
            friendly.SetStateBar(State.incAttackSpeed, -2f);
            friendly.SetStateBar(State.incSpeed, -2f);
        }
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        this.hitUnit = hitUnit;
        UseSkill();

        return base.Hit(attack, criticalPer, hitUnit, isPenetration);
    }

    IEnumerator ChaseRoutine()
    {
        WaitForSeconds wait = new(1f);

        while (true) {
            if (hitUnit != null) {
                if (FriendlyTarget == null) {
                    RandomFriendlyUnit(enemies);
                }

                if (FriendlyTarget != null) {
                    UnitDefault servantUnit = FriendlyTarget.GetComponent<UnitDefault>();
                    servantUnit.SetStateBar(State.servant, -2f);
                    servantUnit.Taunt(hitUnit.gameObject, 1f);
                }
            }

            yield return wait;
        }
    }

    protected override void OnDestroy()
    {
        if (FriendlyTarget != null && !GameManager.instance.isEnd) {
            UnitDefault unitComp = FriendlyTarget.GetComponent<UnitDefault>();
            foreach (Transform state in unitComp.uiStateBar) {
                if (state.name == "servant") {
                    Destroy(state.gameObject);
                    break;
                }
            }
        }

        base.OnDestroy();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"피해를 입으면 본인을 제외한 모든 아군 유닛의 공격력이 <color=green>{incAttackPer}%</color>, 공격 속도가 <color=green>{incAttackSpeedPer}%</color>, 이동 속도가 <color=green>{incSpeedPer}%</color> 증가합니다.(1회 한정)\n" +
                    $"또한 랜덤한 아군 유닛 한 명을 하수인으로 지정하여 본인을 마지막으로 공격한 적 유닛을 공격하도록 합니다.\n" +
                    $"(하수인이 사망 시 다른 유닛을 하수인으로 지정)";
        

        ENSkillDesc = $"When damaged, increases the attack power by <color=green>{incAttackPer}%</color>, attack speed by <color=green>{incAttackSpeedPer}%</color>, and movement speed by <color=green>{incSpeedPer}%</color> of all allied units except herself (one-time only).\n" +
                      $"Also designates a random allied unit as a servant to attack the enemy unit that last attacked her.\n" +
                      $"(If the servant dies, another unit is designated as the servant)";

        FRSkillDesc = $"Lorsqu'elle est endommagée, augmente la puissance d'attaque de <color=green>{incAttackPer}%</color>, la vitesse d'attaque de <color=green>{incAttackSpeedPer}%</color> et la vitesse de déplacement de <color=green>{incSpeedPer}%</color> de toutes les unités alliées sauf elle-même (une seule fois).\n" +
                      $"Désigne également une unité alliée aléatoire comme servante pour attaquer l'unité ennemie qui l'a attaquée en dernier.\n" +
                      $"(Si la servante meurt, une autre unité est désignée comme servante)";

        ITSkillDesc = $"Quando subisce danni, aumenta la potenza di attacco di <color=green>{incAttackPer}%</color>, la velocità di attacco di <color=green>{incAttackSpeedPer}%</color> e la velocità di movimento di <color=green>{incSpeedPer}%</color> di tutte le unità alleate tranne se stessa (una tantum).\n" +
                        $"Designa inoltre un'unità alleata casuale come servitore per attaccare l'unità nemica che l'ha attaccata per ultima.\n" +
                        $"(Se il servitore muore, un'altra unità viene designata come servitore)";

        DESkillDesc = $"Wenn sie Schaden erleidet, erhöht sie die Angriffskraft um <color=green>{incAttackPer}%</color>, die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> und die Bewegungsgeschwindigkeit um <color=green>{incSpeedPer}%</color> aller verbündeten Einheiten außer sich selbst (einmalig).\n" +
                        $"Weist außerdem eine zufällige verbündete Einheit als Diener zu, um die feindliche Einheit anzugreifen, die sie zuletzt angegriffen hat.\n" +
                        $"(Wenn der Diener stirbt, wird eine andere Einheit als Diener zugewiesen)";

        ESSkillDesc = $"Cuando recibe daño, aumenta el poder de ataque en <color=green>{incAttackPer}%</color>, la velocidad de ataque en <color=green>{incAttackSpeedPer}%</color> y la velocidad de movimiento en <color=green>{incSpeedPer}%</color> de todas las unidades aliadas excepto ella misma (solo una vez).\n" +
                      $"También designa a una unidad aliada aleatoria como sirviente para atacar a la unidad enemiga que la atacó por última vez.\n" +
                      $"(Si el sirviente muere, se designa a otra unidad como sirviente)";

        JASkillDesc = $"ダメージを受けると、自身を除くすべての味方ユニットの攻撃力が<color=green>{incAttackPer}%</color>、攻撃速度が<color=green>{incAttackSpeedPer}%</color>、移動速度が<color=green>{incSpeedPer}%</color>増加します。（1回限り）\n" +
                      $"また、ランダムな味方ユニット1名を下僕に指定し、自身を最後に攻撃した敵ユニットを攻撃させます。\n" +
                      $"（下僕が死亡した場合、別のユニットを下僕に指定）";

        PT_BRSkillDesc = $"Quando danificada, aumenta o poder de ataque em <color=green>{incAttackPer}%</color>, a velocidade de ataque em <color=green>{incAttackSpeedPer}%</color> e a velocidade de movimento em <color=green>{incSpeedPer}%</color> de todas as unidades aliadas, exceto ela mesma (uma única vez).\n" +
                         $"Também designa uma unidade aliada aleatória como servo para atacar a unidade inimiga que a atacou por último.\n" +
                         $"(Se o servo morrer, outra unidade é designada como servo)";

        RUSkillDesc = $"При получении урона увеличивает силу атаки на <color=green>{incAttackPer}%</color>, скорость атаки на <color=green>{incAttackSpeedPer}%</color> и скорость передвижения на <color=green>{incSpeedPer}%</color> всех союзных юнитов, кроме себя (однократно).\n" +
                        $"Также назначает случайный союзный юнит слугой для атаки вражеского юнита, который атаковал её последним.\n" +
                        $"(Если слуга умирает, другой юнит назначается слугой)";

        ZH_HANSSkillDesc = $"受到伤害时，除自身外，所有友军单位的攻击力提高<color=green>{incAttackPer}%</color>，攻击速度提高<color=green>{incAttackSpeedPer}%</color>，移动速度提高<color=green>{incSpeedPer}%</color>（仅限一次）。\n" +
                        $"还指定一个随机友军单位作为仆从，攻击最后攻击她的敌方单位。\n" +
                        $"（如果仆从死亡，则指定另一个单位作为仆从）";
    }
}