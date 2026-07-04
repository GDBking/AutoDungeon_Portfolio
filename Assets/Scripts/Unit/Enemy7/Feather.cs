using UnityEngine;

public class Feather : UnitDefault
{
    [Header("깃털")]
    public float attackCoeff;
    public float bleedingDamage;
    public int count;
    public int featherCount;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        for (int i = 0; i < featherCount; i++) {
            Vector2 targetPos = (Vector2)transform.position + (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
            StartCoroutine(OnFire(null, targetPos, true, true));
        }
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        unitComp.Bleeding(bleedingDamage, count, this);
        unitComp.SetStateBar(State.bleeding, count);

        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"랜덤한 방향으로 <color=yellow>{featherCount}개</color>의 유닛을 관통하는 깃털을 날립니다.\n" +
                    $"피격된 적들은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입고, <color=yellow>{count}초</color> 동안 출혈 상태가 됩니다.\n" +
                    $"초당 출혈 데미지: <color=red>{bleedingDamage}</color>";
        
        ENSkillDesc = $"Launches <color=yellow>{featherCount} feathers</color> that pierce through units in random directions.\n" +
                      $"Hit enemies take damage equal to <color=red>{attackCoeff} times</color> the attack power and enter a bleeding state for <color=yellow>{count} seconds</color>.\n" +
                      $"Bleeding damage per second: <color=red>{bleedingDamage}</color>";

        FRSkillDesc = $"Lance <color=yellow>{featherCount} plumes</color> qui traversent les unités dans des directions aléatoires.\n" +
                        $"Les ennemis touchés subissent des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque et entrent dans un état de saignement pendant <color=yellow>{count} secondes</color>.\n" +
                        $"Dégâts d'hémorragie par seconde : <color=red>{bleedingDamage}</color>";

        ITSkillDesc = $"Lancia <color=yellow>{featherCount} piume</color> che trapassano le unità in direzioni casuali.\n" +
                        $"I nemici colpiti subiscono danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco e entrano in uno stato di sanguinamento per <color=yellow>{count} secondi</color>.\n" +
                        $"Danno da sanguinamento al secondo: <color=red>{bleedingDamage}</color>";

        DESkillDesc = $"Startet <color=yellow>{featherCount} Federn</color>, die Einheiten in zufälligen Richtungen durchdringen.\n" +
                        $"Getroffene Feinde erleiden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft und geraten für <color=yellow>{count} Sekunden</color> in einen Blutungszustand.\n" +
                        $"Blutungsschaden pro Sekunde: <color=red>{bleedingDamage}</color>";

        ESSkillDesc = $"Lanza <color=yellow>{featherCount} plumas</color> que atraviesan unidades en direcciones aleatorias.\n" +
                        $"Los enemigos alcanzados reciben daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque y entran en un estado de sangrado durante <color=yellow>{count} segundos</color>.\n" +
                        $"Daño por sangrado por segundo: <color=red>{bleedingDamage}</color>";

        JASkillDesc = $"ランダムな方向に<color=yellow>{featherCount}個</color>のユニットを貫通する羽根を飛ばします。\n" +
                      $"被弾した敵は攻撃力の<color=red>{attackCoeff}倍</color>のダメージを受け、<color=yellow>{count}秒</color>間、出血状態になります。\n" +
                      $"毎秒の出血ダメージ: <color=red>{bleedingDamage}</color>";

        PT_BRSkillDesc = $"Lança <color=yellow>{featherCount} penas</color> que perfuram unidades em direções aleatórias.\n" +
                         $"Inimigos atingidos recebem dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e entram em um estado de sangramento por <color=yellow>{count} segundos</color>.\n" +
                         $"Dano por sangramento por segundo: <color=red>{bleedingDamage}</color>";

        RUSkillDesc = $"Запускает <color=yellow>{featherCount} перьев</color>, которые пронизывают юниты в случайных направлениях.\n" +
                          $"Попавшие враги получают урон, равный <color=red>{attackCoeff} раз</color> силе атаки, и впадают в состояние кровотечения на <color=yellow>{count} секунд</color>.\n" +
                          $"Урон от кровотечения в секунду: <color=red>{bleedingDamage}</color>";

        ZH_HANSSkillDesc = $"向随机方向发射<color=yellow>{featherCount}根</color>穿透单位的羽毛。\n" +
                             $"被击中的敌人会受到相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并在<color=yellow>{count}秒</color>内进入出血状态。\n" +
                             $"每秒出血伤害：<color=red>{bleedingDamage}</color>";
    }
}