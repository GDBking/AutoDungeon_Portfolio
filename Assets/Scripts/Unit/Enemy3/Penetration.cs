using UnityEngine;

public class Penetration : UnitDefault
{
    [Header("관통")]
    public float attackCoeff;
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
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        StartCoroutine(OnFire(SkillTarget, pos, true, true));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        unitComp.Burn(burnDamage, count, this);
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position, isPenetration: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 방향으로 유닛과 방어력을 관통하는 불화살을 발사합니다.\n" +
                    $"피격된 적들은 공격력 <color=red>{attackCoeff}배</color>의 방어력 관통 피해를 입고, <color=yellow>{count}초</color> 동안 화상을 입습니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>";

        ENSkillDesc = $"Fires a flaming arrow that penetrates units and defense in the attacking direction.\n" +
                      $"Enemies hit take defense-piercing damage equal to <color=red>{attackCoeff} times</color> the attack power and are burned for <color=yellow>{count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Tire une flèche enflammée qui traverse les unités et la défense dans la direction de l'attaque.\n" +
                      $"Les ennemis touchés subissent des dégâts perforants égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque et brûlent pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Spara una freccia infuocata che penetra unità e difesa nella direzione dell'attacco.\n" +
                      $"I nemici colpiti subiscono danni perforanti pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco e bruciano per <color=yellow>{count} secondi</color>.\n" +
                      $"Danno da bruciatura per secondo: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Feuert einen Feuerpfeil ab, der Einheiten und Verteidigung in Angriffsrichtung durchdringt.\n" +
                      $"Getroffene Gegner erleiden verteidigungsdurchdringenden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft und brennen für <color=yellow>{count} Sekunden</color>.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Dispara una flecha llameante que penetra unidades y defensa en la dirección de ataque.\n" +
                      $"Los enemigos alcanzados reciben daño que ignora la defensa igual a <color=red>{attackCoeff} veces</color> el poder de ataque y arden durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>";

        JASkillDesc = $"攻撃方向にユニットと防御を貫通する炎の矢を放ちます。\n" +
                      $"命中した敵は攻撃力の<color=red>{attackCoeff}倍</color>の防御貫通ダメージを受け、<color=yellow>{count}秒</color>間火傷状態になります。\n" +
                      $"毎秒の火傷ダメージ：<color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Dispara uma flecha flamejante que perfura unidades e defesa na direção do ataque.\n" +
                          $"Los inimigos atingidos recebem dano que ignora a defesa igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e ficam queimados por <color=yellow>{count} segundos</color>.\n" +
                          $"Dano de queimadura por segundo: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Выпускает огненную стрелу, пробивающую юнитов и защиту в направлении атаки.\n" +
                      $"Попавшие враги получают урон, пробивающий защиту, равный <color=red>{attackCoeff} раза</color> силы атаки, и горят в течение <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от горения в секунду: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"向攻击方向射穿单位和防御的火焰箭。\n" +
                           $"被击中的敌人受到相当于攻击力<color=red>{attackCoeff}倍</color>的穿甲伤害，并在<color=yellow>{count}秒</color>内灼烧。\n" +
                           $"每秒灼烧伤害：<color=red>{burnDamage}</color>";
    }
}
