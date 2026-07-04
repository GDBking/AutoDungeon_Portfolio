using System.Collections;
using UnityEngine;

public class DivineThunder : UnitDefault
{
    [Header("천뢰")]
    public float attackCoeff;
    public int count;
    public float size;

    readonly WaitForSeconds wait = new(0.1f);

    protected override void Awake()
    {
        base.Awake();

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
        CameraShake.instance.Shake(2f, 2f);
        StartCoroutine(DivineThunderRoutine());
    }

    IEnumerator DivineThunderRoutine()
    {
        for (int i = 0; i < count; i++) {
            Vector2 randPos = RandomPositionManager.instance.GetRandomBattleFieldPos();

            SoundPlay(skillSoundClip);
            attackSize = size;
            CreateAttackBox(AttackPower * attackCoeff, null, randPos, AttackStyle.range);
            attackSize = 0.5f;

            yield return wait;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"하늘에서 무작위 위치에 레이저를 <color=yellow>{count}개</color> 발사하여 <color=#AEEAFF>{size}X{size}</color>범위에 피격된 적들은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입습니다.";
        
        ENSkillDesc = $"Fires <color=yellow>{count} lasers</color> at random locations from the sky, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies hit within a <color=#AEEAFF>{size}X{size}</color> area.";

        FRSkillDesc = $"Tire <color=yellow>{count} lasers</color> à des emplacements aléatoires depuis le ciel, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis touchés dans une zone de <color=#AEEAFF>{size}X{size}</color>.";

        ITSkillDesc = $"Spara <color=yellow>{count} laser</color> in posizioni casuali dal cielo, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici colpiti in un'area di <color=#AEEAFF>{size}X{size}</color>.";

        DESkillDesc = $"Feuert <color=yellow>{count} Laser</color> an zufällige Orte vom Himmel und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft bei Feinden, die in einem Bereich von <color=#AEEAFF>{size}X{size}</color> getroffen werden.";

        ESSkillDesc = $"Dispara <color=yellow>{count} láseres</color> en ubicaciones aleatorias desde el cielo, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos alcanzados dentro de un área de <color=#AEEAFF>{size}X{size}</color>.";

        JASkillDesc = $"空からランダムな位置に<color=yellow>{count}個</color>のレーザーを発射し、<color=#AEEAFF>{size}X{size}</color>の範囲に当たった敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Dispara <color=yellow>{count} lasers</color> em locais aleatórios do céu, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos atingidos dentro de uma área de <color=#AEEAFF>{size}X{size}</color>.";

        RUSkillDesc = $"Выпускает <color=yellow>{count} лазеров</color> в случайные места с неба, нанося урон, равный <color=red>{attackCoeff} раз</color> силе атаки врагам, пораженным в области <color=#AEEAFF>{size}X{size}</color>.";

        ZH_HANSSkillDesc = $"从天空向随机位置发射<color=yellow>{count}个</color>激光，对<color=#AEEAFF>{size}X{size}</color>范围内被击中的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}
