using TMPro;
using UnityEngine;

public class Fragment : UnitDefault
{
    [Header("파편")]
    public float attackCoeff;
    public float decDefenseAmount;
    public float incAttackAmount;
    public float incAttackSpeedAmount;
    public float size;

    public GameObject fragmentState;
    public TextMeshProUGUI cntTxt;
    int cnt;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void Start()
    {
        base.Start();

        fragmentState.SetActive(true);
    }

    protected override void UseSkill()
    {
        if (Target == null || Defense < decDefenseAmount)
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
        SetStat(defenseStat, decDefenseAmount, false, false);
        SetStat(attackPowerStat, incAttackAmount, false, true);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true);

        cntTxt.SetText((++cnt).ToString());
    }

    void OnSkillFire(GameObject target)
    {
        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, target.transform.position, AttackStyle.range);
        attackSize = 0.5f;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"자신의 방어력을 <color=red>{decDefenseAmount}</color>만큼 깎아 공격 중인 대상에게 파편을 던집니다.\n" +
                    $"대상 반경 <color=#AEEAFF>{size}X{size}</color>범위의 적들은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입고, 본인은 공격력이 <color=green>{incAttackAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>만큼 증가합니다.\n" +
                    $"(파편 효과 중첩 가능, 방어력이 <color=red>{decDefenseAmount}</color>보다 낮을 경우 사용 불가)";
        
        ENSkillDesc = $"Reduces your defense by <color=red>{decDefenseAmount}</color> to throw fragments at the attacking target.\n" +
                       $"Enemies within a radius of <color=#AEEAFF>{size}X{size}</color> of the target take damage equal to <color=red>{attackCoeff} times</color> the attack power, and you gain <color=green>{incAttackAmount}</color> attack power and <color=green>{incAttackSpeedAmount}</color> attack speed.\n" +
                       $"(Fragment effects can stack; cannot be used if defense is lower than <color=red>{decDefenseAmount}</color>)";

        FRSkillDesc = $"Réduit votre défense de <color=red>{decDefenseAmount}</color> pour lancer des fragments sur la cible attaquante.\n" +
                      $"Les ennemis dans un rayon de <color=#AEEAFF>{size}X{size}</color> de la cible subissent des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque, et vous gagnez <color=green>{incAttackAmount}</color> de puissance d'attaque et <color=green>{incAttackSpeedAmount}</color> de vitesse d'attaque.\n" +
                      $"(Les effets des fragments peuvent s'empiler ; ne peut pas être utilisé si la défense est inférieure à <color=red>{decDefenseAmount}</color>)";

        ITSkillDesc = $"Riduce la tua difesa di <color=red>{decDefenseAmount}</color> per lanciare frammenti al bersaglio attaccante.\n" +
                          $"I nemici entro un raggio di <color=#AEEAFF>{size}X{size}</color> dal bersaglio subiscono danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco, e tu guadagni <color=green>{incAttackAmount}</color> di potenza d'attacco e <color=green>{incAttackSpeedAmount}</color> di velocità d'attacco.\n" +
                          $"(Gli effetti dei frammenti possono accumularsi; non può essere usato se la difesa è inferiore a <color=red>{decDefenseAmount}</color>)";

        DESkillDesc = $"Reduziert deine Verteidigung um <color=red>{decDefenseAmount}</color>, um Fragmente auf das angreifende Ziel zu werfen.\n" +
                          $"Feinde in einem Radius von <color=#AEEAFF>{size}X{size}</color> um das Ziel erleiden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft, und du erhältst <color=green>{incAttackAmount}</color> Angriffskraft und <color=green>{incAttackSpeedAmount}</color> Angriffsgeschwindigkeit.\n" +
                          $"(Fragmenteffekte können gestapelt werden; kann nicht verwendet werden, wenn die Verteidigung niedriger als <color=red>{decDefenseAmount}</color> ist)";

        ESSkillDesc = $"Reduce tu defensa en <color=red>{decDefenseAmount}</color> para lanzar fragmentos al objetivo atacante.\n" +
                            $"Los enemigos dentro de un radio de <color=#AEEAFF>{size}X{size}</color> del objetivo reciben daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque, y tú ganas <color=green>{incAttackAmount}</color> de poder de ataque y <color=green>{incAttackSpeedAmount}</color> de velocidad de ataque.\n" +
                            $"(Los efectos de los fragmentos pueden acumularse; no se puede usar si la defensa es inferior a <color=red>{decDefenseAmount}</color>)";

        JASkillDesc = $"自身の防御力を<color=red>{decDefenseAmount}</color>だけ下げて、攻撃中の対象に破片を投げつけます。\n" +
                       $"対象の半径<color=#AEEAFF>{size}X{size}</color>範囲内の敵は攻撃力の<color=red>{attackCoeff}倍</color>のダメージを受け、自身は攻撃力が<color=green>{incAttackAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>増加します。\n" +
                       $"（破片効果は重複可能、防御力が<color=red>{decDefenseAmount}</color>より低い場合は使用不可）";

        PT_BRSkillDesc = $"Reduz sua defesa em <color=red>{decDefenseAmount}</color> para lançar fragmentos no alvo atacante.\n" +
                             $"Inimigos dentro de um raio de <color=#AEEAFF>{size}X{size}</color> do alvo recebem dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque, e você ganha <color=green>{incAttackAmount}</color> de poder de ataque e <color=green>{incAttackSpeedAmount}</color> de velocidade de ataque.\n" +
                             $"(Os efeitos dos fragmentos podem se acumular; não pode ser usado se a defesa for inferior a <color=red>{decDefenseAmount}</color>)";

        RUSkillDesc = $"Уменьшает вашу защиту на <color=red>{decDefenseAmount}</color>, чтобы бросить осколки в атакующую цель.\n" +
                            $"Враги в радиусе <color=#AEEAFF>{size}X{size}</color> от цели получают урон, равный <color=red>{attackCoeff} раз</color> силе атаки, а вы получаете <color=green>{incAttackAmount}</color> силы атаки и <color=green>{incAttackSpeedAmount}</color> скорости атаки.\n" +
                            $"(Эффекты осколков могут накапливаться; не может быть использовано, если защита ниже <color=red>{decDefenseAmount}</color>)";

        ZH_HANSSkillDesc = $"将你的防御力降低<color=red>{decDefenseAmount}</color>，以向攻击目标投掷碎片。\n" +
                            $"目标半径<color=#AEEAFF>{size}X{size}</color>内的敌人受到相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，你获得<color=green>{incAttackAmount}</color>攻击力和<color=green>{incAttackSpeedAmount}</color>攻击速度。\n" +
                            $"（碎片效果可以叠加；如果防御力低于<color=red>{decDefenseAmount}</color>，则无法使用）";
    }
}