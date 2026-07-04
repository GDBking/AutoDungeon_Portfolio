using UnityEngine;

public class SelfDestruction : UnitDefault
{
    [Header("자폭")]
    public float attackCoeff;
    public float burnDamage;
    public int count;
    public float size;

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void Death()
    {
        SoundPlay(skillSoundClip);

        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, transform.position, AttackStyle.range);

        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        foreach (Collider2D enemy in hitEnemys)
            enemy.GetComponent<UnitDefault>().Burn(burnDamage, count, this);

        base.Death();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"사망 시 폭발하여 본인 반경 <color=#AEEAFF>{size}x{size}</color>범위의 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{count}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>";

        ENSkillDesc = $"Explodes on death dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies within a <color=#AEEAFF>{size}x{size}</color> area around itself and burns them for <color=yellow>{count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Explose à la mort en infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis dans une zone de <color=#AEEAFF>{size}x{size}</color> autour de soi et les brûle pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Esplode alla morte infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici in un'area di <color=#AEEAFF>{size}x{size}</color> intorno a sé e li brucia per <color=yellow>{count} secondi</color>.\n" +
                    $"Danno da bruciatura per secondo: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Explodiert beim Tod und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft an Gegnern in einem Bereich von <color=#AEEAFF>{size}x{size}</color> um sich herum und verbrennt sie für <color=yellow>{count} Sekunden</color>.\n" +
                    $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Explota al morir, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos dentro de un área de <color=#AEEAFF>{size}x{size}</color> a su alrededor y los quema durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>";

        JASkillDesc = $"死亡時に爆発し、自身の周囲<color=#AEEAFF>{size}x{size}</color>範囲内の敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{count}秒</color>間火傷状態にします。\n" +
                      $"秒の火傷ダメージ：<color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Explode ao morrer, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque a os inimigos dentro de um área de <color=#AEEAFF>{size}x{size}</color> a sua volta e os queima durante <color=yellow>{count} segundos</color>.\n" +
                          $"Dano por queimadura por segundo: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Взрывается при смерти, нанося урон, равный <color=red>{attackCoeff} раза</color> силы атаки, врагам в области <color=#AEEAFF>{size}x{size}</color> вокруг себя и поджигая их на <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от горения в секунду: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"死亡时爆炸，对自身周围<color=#AEEAFF>{size}x{size}</color>范围内的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并使其在<color=yellow>{count}秒</color>内灼烧。\n" +
                             $"每秒灼烧伤害：<color=red>{burnDamage}</color>";
    }
}
