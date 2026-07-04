using UnityEngine;

public class GoblinShaman : UnitDefault
{
    [Header("저주의 화염")]
    public float decDefenseAmount;
    public float burnDamage;
    public int count;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(-1f, target, target.transform.position);

        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        unitComp.Burn(burnDamage, count, this);
        unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, count);
        unitComp.SetStateBar(State.decDefense, count);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 화염구를 던져 <color=yellow>{count}초</color> 동안 화상을 입히고, 방어력을 <color=red>{decDefenseAmount}</color>만큼 감소시킵니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>";

        ENSkillDesc = $"Throws a fireball at the current target, burning them for <color=yellow>{count} seconds</color> and reducing their defense by <color=red>{decDefenseAmount}</color>.\n" +
                       $"Burn damage per second: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Lance une boule de feu sur la cible actuelle, la brûlant pendant <color=yellow>{count} secondes</color> et réduisant sa défense de <color=red>{decDefenseAmount}</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Lancia una palla di fuoco sul bersaglio attuale, bruciandolo per <color=yellow>{count} secondi</color> e riducendo la sua difesa di <color=red>{decDefenseAmount}</color>.\n" +
                      $"Danno da bruciatura per secondo: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Schießt einen Feuerball auf das aktuelle Ziel, verbrennt es für <color=yellow>{count} Sekunden</color> und reduziert seine Verteidigung um <color=red>{decDefenseAmount}</color>.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Lanza una bola de fuego al objetivo actual, lo quema durante <color=yellow>{count} segundos</color> y reduce su defensa en <color=red>{decDefenseAmount}</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>";

        JASkillDesc = $"現在攻撃中の対象にファイアボールを投げ、<color=yellow>{count}秒</color>間火傷を負わせ、防御力を<color=red>{decDefenseAmount}</color>だけ減少させます。\n" +
                      $"秒の火傷ダメージ：<color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Lança uma bola de fogo no alvo atual, queimando-o por <color=yellow>{count} segundos</color> e reduzindo sua defesa em <color=red>{decDefenseAmount}</color>.\n" +
                          $"Dano de queimadura por segundo: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Выпускает огненный шар по текущей цели, поджигая её на <color=yellow>{count} секунд</color> и уменьшая её защиту на <color=red>{decDefenseAmount}</color>.\n" +
                       $"Урон от горения в секунду: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"向目标投掷一个火球，使其在 <color=yellow>{count} 秒</color> 内着火，防御降低 <color=red>{decDefenseAmount}</color>。\n" +
                          $"每秒灼烧伤害：<color=red>{burnDamage}</color>";
    }
}
