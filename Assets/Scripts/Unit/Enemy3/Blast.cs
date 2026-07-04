using UnityEngine;

public class Blast : UnitDefault
{
    [Header("폭렬")]
    public float attackCoeff;
    public float size;
    public float burnDamage;
    public int count;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(size / 2f, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        attackSize = size;
        CreateAttackBox(AttackPower * attackCoeff, null, transform.position, AttackStyle.range);
        attackSize = 0.5f;

        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        foreach (Collider2D hitEnemy in hitEnemys)
            hitEnemy.GetComponent<UnitDefault>().Burn(burnDamage, count, this);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{size}X{size}</color>범위에 폭발을 일으켜 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{count}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>";
        
        ENSkillDesc = $"Causes an explosion in a <color=#AEEAFF>{size}X{size}</color> area around itself, dealing enemies damage equal to <color=red>{attackCoeff} times</color> the attack power and burning them for <color=yellow>{count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Fait exploser une zone de <color=#AEEAFF>{size}X{size}</color> autour de soi, infligeant aux ennemis des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque et les brûlant pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Fa esplodere un'area di <color=#AEEAFF>{size}X{size}</color> intorno a sé, infliggendo ai nemici danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco e bruciandoli per <color=yellow>{count} secondi</color>.\n" +
                      $"Danni da bruciatura al secondo: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Löst eine Explosion in einem Bereich von <color=#AEEAFF>{size}X{size}</color> um sich herum aus, die Feinden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft zufügt und sie für <color=yellow>{count} Sekunden</color> verbrennt.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Hace explotar un área de <color=#AEEAFF>{size}X{size}</color> a su alrededor, infligiendo a los enemigos un daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque y quemándolos durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>";

        JASkillDesc = $"自身の半径<color=#AEEAFF>{size}X{size}</color>範囲に爆発を起こし、敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{count}秒</color>間、火傷状態にします。\n" +
                      $"秒間火傷ダメージ: <color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Faz explodir uma área de <color=#AEEAFF>{size}X{size}</color> ao seu redor, causando aos inimigos um dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e queimando-os por <color=yellow>{count} segundos</color>.\n" +
                         $"Dano por queimadura por segundo: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Взрывает область радиусом <color=#AEEAFF>{size}X{size}</color> вокруг себя, нанося врагам урон, равный <color=red>{attackCoeff} раза</color> силе атаки, и поджигая их на <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от ожога в секунду: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"在自身半径<color=#AEEAFF>{size}X{size}</color>范围内引发爆炸，对敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并使其燃烧<color=yellow>{count}秒</color>。\n" +
                           $"每秒燃烧伤害：<color=red>{burnDamage}</color>";
    }
}