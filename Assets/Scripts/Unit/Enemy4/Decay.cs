using System.Collections;
using UnityEngine;

public class Decay : UnitDefault
{
    [Header("부패")]
    public float poisonDamage;
    public float decAttackPer;
    public float duration;

    WaitForSeconds wait;
    bool isDecay;
    Coroutine co;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
        onSkillFireAction = OnSkillFire;
    }

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SetStateBar(State.decayArrow, duration);

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(DecayTime());
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        unitComp.SetStat(unitComp.attackPowerStat, decAttackPer, true, false, 1f);
        unitComp.SetStateBar(State.decAttack, 1f);
        unitComp.Poison(poisonDamage, 1, this);
        
        CreateAttackBox(AttackPower, target, target.transform.position);
    }

    IEnumerator DecayTime()
    {
        isDecay = true;
        yield return wait;
        isDecay = false;

        co = null;
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        return base.OnFire(target, targetPos, isDecay, isPenetration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{duration}초</color> 동안 기본 화살이 부패 화살로 변해 적중 시 기본 피해와 더불어 <color=yellow>1초</color> 동안 독 효과를 입히고 공격력을 <color=red>{decAttackPer}%</color> 감소시킵니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";

        ENSkillDesc = $"For <color=yellow>{duration} seconds</color>, basic arrows become decay arrows: on hit they deal normal damage and apply a poison for <color=yellow>1 second</color>, while reducing the target's attack by <color=red>{decAttackPer}%</color>.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"Pendant <color=yellow>{duration} secondes</color>, les flèches de base deviennent des flèches de décomposition : à l'impact, elles infligent des dégâts normaux et appliquent un poison pendant <color=yellow>1 seconde</color>, tout en réduisant l'attaque de la cible de <color=red>{decAttackPer}%</color>.\n" +
                      $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Per <color=yellow>{duration} secondi</color>, le frecce base diventano frecce di decomposizione: all'impatto infliggono danni normali e applicano un veleno per <color=yellow>1 secondo</color>, riducendo l'attacco del bersaglio di <color=red>{decAttackPer}%</color>.\n" +
                      $"Danno da veleno per secondo: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Für <color=yellow>{duration} Sekunden</color> werden Grundpfeile zu Verfallspfeilen: Bei Treffer verursachen sie normalen Schaden und legen für <color=yellow>1 Sekunde</color> Gift an, während sie den Angriff des Ziels um <color=red>{decAttackPer}%</color> verringern.\n" +
                      $"Giftschaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Durante <color=yellow>{duration} segundos</color>, las flechas básicas se convierten en flechas de descomposición: al impactar infligen daño normal y aplican veneno durante <color=yellow>1 segundo</color>, reduciendo el ataque del objetivo en <color=red>{decAttackPer}%</color>.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"<color=yellow>{duration}秒</color>の間、通常矢が腐敗の矢に変化し、命中時に通常ダメージに加えて<color=yellow>1秒</color>間毒効果を付与し、対象の攻撃力を<color=red>{decAttackPer}%</color>減少させます。\n" +
                      $"毎秒の毒ダメージ：<color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Durante <color=yellow>{duration} segundos</color>, las flechas básicas se convierten en flechas de decadencia: al impactar infligen daño normal y aplican veneno por <color=yellow>1 segundo</color>, reduciendo el ataque del objetivo en <color=red>{decAttackPer}%</color>.\n" +
                          $"Daño de veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"В течение <color=yellow>{duration} секунд</color> обычные стрелы превращаются в стрелы разложения: при попадании они наносят обычный урон и накладывают яд на <color=yellow>1 секунду</color>, снижая атаку цели на <color=red>{decAttackPer}%</color>.\n" +
                      $"Урон от яда в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"在<color=yellow>{duration}秒</color>内，基本箭矢变为腐朽箭：命中时造成普通伤害，并在<color=yellow>1秒</color>内施加中毒，降低目标的攻击力<color=red>{decAttackPer}%</color>。\n" +
                           $"每秒中毒伤害：<color=red>{poisonDamage}</color>";
    }
}
