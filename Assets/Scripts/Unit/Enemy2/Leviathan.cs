using TMPro;
using UnityEngine;

public class Leviathan : UnitDefault
{
    [Header("패시브")]
    public float decSpeedPer;
    public float decDefenseAmount;
    public float passiveDuration;
    [Header("해일")]
    public float attackCoeff;
    public float skillDuration;
    public Tsunami tsunamiPrefab;
    public GameObject tsunamiState;
    public TextMeshProUGUI tsunamiCntTxt;

    public static int cnt;

    protected override void Awake()
    {
        base.Awake();

        if (GameManager.instance.isEnd)
            cnt = 0;
    }

    protected override void Start()
    {
        base.Start();

        tsunamiCntTxt.SetText(cnt.ToString());
        tsunamiState.SetActive(true);
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        cnt = (cnt + 1) % 3;
        tsunamiCntTxt.SetText(cnt.ToString());

        Tsunami tsunamiObj = Instantiate(tsunamiPrefab, GameManager.instance.skillEffect);
        tsunamiObj.unitComp = this;
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        SoundPlay(attackSoundClip);

        AttackTarget.GetComponent<UnitDefault>().LeviathanDebuff(decSpeedPer, decDefenseAmount, passiveDuration);
        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, false);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"패시브: 기본 공격에 맞은 적은 <color=yellow>{passiveDuration}초</color> 동안 방어력이 <color=red>{decDefenseAmount}</color>, 이동 속도가 <color=red>{decSpeedPer}%</color> 감소합니다.\n" +
                    $"(디버프는 중첩되지 않음)\n\n" +
                    $"해일: 거대한 해일이 밀려와 해일에 닿은 적들은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입습니다.\n" +
                    $"<color=yellow>3번째</color> 해일일 경우 추가로 <color=yellow>{skillDuration}초</color> 동안 기절합니다.";
        
        ENSkillDesc = $"Passive: Enemies hit by basic attacks have their defense reduced by <color=red>{decDefenseAmount}</color> and movement speed reduced by <color=red>{decSpeedPer}%</color> for <color=yellow>{passiveDuration} seconds.</color>\n" +
                      $"(Debuffs do not stack)\n\n" +
                      $"Tsunami: A massive tsunami rushes in, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies hit by the tsunami.\n" +
                      $"On the <color=yellow>3rd</color> tsunami, they are also stunned for <color=yellow>{skillDuration} seconds</color>.";

        FRSkillDesc = $"Passif : Les ennemis touchés par les attaques de base voient leur défense réduite de <color=red>{decDefenseAmount}</color> et leur vitesse de déplacement réduite de <color=red>{decSpeedPer}%</color> pendant <color=yellow>{passiveDuration} secondes.</color>\n" +
                      $"(Les débuffs ne se cumulent pas)\n\n" +
                      $"Tsunami : Un tsunami massif déferle, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis touchés par le tsunami.\n" +
                      $"Au <color=yellow>3ème</color> tsunami, ils sont également étourdis pendant <color=yellow>{skillDuration} secondes</color>.";

        ITSkillDesc = $"Passivo: I nemici colpiti dagli attacchi base vedono la loro difesa ridotta di <color=red>{decDefenseAmount}</color> e la loro velocità di movimento ridotta di <color=red>{decSpeedPer}%</color> per <color=yellow>{passiveDuration} secondi.</color>\n" +
                      $"(I debuff non si accumulano)\n\n" +
                      $"Tsunami: Un enorme tsunami si abbatte, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici colpiti dallo tsunami.\n" +
                      $"Al <color=yellow>3°</color> tsunami, vengono anche storditi per <color=yellow>{skillDuration} secondi</color>.";

        DESkillDesc = $"Passiv: Gegner, die von Basisangriffen getroffen werden, haben ihre Verteidigung um <color=red>{decDefenseAmount}</color> und ihre Bewegungsgeschwindigkeit um <color=red>{decSpeedPer}%</color> für <color=yellow>{passiveDuration} Sekunden</color> reduziert.\n" +
                      $"(Debuffs stapeln sich nicht)\n\n" +
                      $"Tsunami: Ein massiver Tsunami strömt herein und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft bei Gegnern, die vom Tsunami getroffen werden.\n" +
                      $"Beim <color=yellow>3.</color> Tsunami werden sie außerdem für <color=yellow>{skillDuration} Sekunden</color> betäubt.";

        ESSkillDesc = $"Pasivo: Los enemigos golpeados por ataques básicos tienen su defensa reducida en <color=red>{decDefenseAmount}</color> y su velocidad de movimiento reducida en <color=red>{decSpeedPer}%</color> durante <color=yellow>{passiveDuration} segundos.</color>\n" +
                      $"(Los debuffs no se acumulan)\n\n" +
                      $"Tsunami: Un enorme tsunami irrumpe, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos golpeados por el tsunami.\n" +
                      $"En el <color=yellow>3º</color> tsunami, también quedan aturdidos durante <color=yellow>{skillDuration} segundos</color>.";

        JASkillDesc = $"パッシブ: 通常攻撃に当たった敵は<color=yellow>{passiveDuration}秒</color>の間、防御力が<color=red>{decDefenseAmount}</color>、移動速度が<color=red>{decSpeedPer}%</color>減少します。\n" +
                      $"(デバフは重複しません)\n\n" +
                      $"津波: 巨大な津波が押し寄せ、津波に触れた敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。\n" +
                      $"<color=yellow>3回目</color>の津波では、追加で<color=yellow>{skillDuration}秒</color>間気絶します。";

        PT_BRSkillDesc = $"Passivo: Inimigos atingidos por ataques básicos têm sua defesa reduzida em <color=red>{decDefenseAmount}</color> e velocidade de movimento reduzida em <color=red>{decSpeedPer}%</color> por <color=yellow>{passiveDuration} segundos.</color>\n" +
                         $"(Os debuffs não se acumulam)\n\n" +
                         $"Tsunami: Um enorme tsunami avança, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos atingidos pelo tsunami.\n" +
                         $"No <color=yellow>3º</color> tsunami, eles também ficam atordoados por <color=yellow>{skillDuration} segundos</color>.";

        RUSkillDesc = $"Пассив: Враги, пораженные базовыми атаками, имеют снижение защиты на <color=red>{decDefenseAmount}</color> и снижение скорости передвижения на <color=red>{decSpeedPer}%</color> на <color=yellow>{passiveDuration} секунд.</color>\n" +
                      $"(Дебаффы не складываются)\n\n" +
                      $"Цунами: Огромное цунами обрушивается, нанося урон, равный <color=red>{attackCoeff} раза</color> силы атаки врагам, пораженным цунами.\n" +
                      $"При <color=yellow>3-м</color> цунами они также оглушаются на <color=yellow>{skillDuration} секунд</color>.";

        ZH_HANSSkillDesc = $"被动：被基础攻击击中的敌人防御力降低<color=red>{decDefenseAmount}</color>，移动速度降低<color=red>{decSpeedPer}%</color>，持续<color=yellow>{passiveDuration}秒。</color>\n" +
                           $"（减益效果不叠加）\n\n" +
                           $"海啸：一股巨大的海啸涌来，对被海啸击中的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。\n" +
                           $"在第<color=yellow>3</color>次海啸中，它们还会被眩晕<color=yellow>{skillDuration}秒</color>。";
    }
}