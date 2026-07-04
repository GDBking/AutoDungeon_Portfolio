using UnityEngine;

public class Shockwave : UnitDefault
{
    [Header("파동")]
    public float attackCoeff;
    public float decAttackPer;
    public float duration;
    public float size;
    public ShockwaveObj shockwavePrefab;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(size / 2f, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        ShockwaveObj shockwaveObj = Instantiate(shockwavePrefab, transform.position, Quaternion.identity, GameManager.instance.skillEffect);
        shockwaveObj.unitComp = this;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"고리 모양의 파동이 점점 커지며 파동에 닿은 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{duration}초</color> 동안 공격력을 <color=red>{decAttackPer}%</color> 감소시킵니다.\n" +
                    $"파동의 최대 크기: <color=#AEEAFF>{size}X{size}</color>";
        
        ENSkillDesc = $"A ring-shaped shockwave expands outward, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to enemies it touches and reducing their attack power by <color=red>{decAttackPer}%</color> for <color=yellow>{duration} seconds</color>.\n" +
                       $"Maximum size of the shockwave: <color=#AEEAFF>{size}X{size}</color>";

        FRSkillDesc = $"Une onde de choc en forme d'anneau s'étend vers l'extérieur, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis qu'elle touche et réduisant leur puissance d'attaque de <color=red>{decAttackPer}%</color> pendant <color=yellow>{duration} secondes</color>.\n" +
                        $"Taille maximale de l'onde de choc : <color=#AEEAFF>{size}X{size}</color>";

        ITSkillDesc = $"Un'onda d'urto a forma di anello si espande verso l'esterno, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici che tocca e riducendo la loro potenza d'attacco del <color=red>{decAttackPer}%</color> per <color=yellow>{duration} secondi</color>.\n" +
                          $"Dimensione massima dell'onda d'urto: <color=#AEEAFF>{size}X{size}</color>";

        DESkillDesc = $"Eine ringförmige Schockwelle breitet sich nach außen aus und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft bei den Feinden, die sie berührt, und reduziert deren Angriffskraft um <color=red>{decAttackPer}%</color> für <color=yellow>{duration} Sekunden</color>.\n" +
                       $"Maximale Größe der Schockwelle: <color=#AEEAFF>{size}X{size}</color>";

        ESSkillDesc = $"Una onda de choque en forma de anillo se expande hacia afuera, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos que toca y reduciendo su poder de ataque en <color=red>{decAttackPer}%</color> durante <color=yellow>{duration} segundos</color>.\n" +
                          $"Tamaño máximo de la onda de choque: <color=#AEEAFF>{size}X{size}</color>";

        JASkillDesc = $"リング状の衝撃波が外側に広がり、触れた敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{duration}秒</color>間、攻撃力を<color=red>{decAttackPer}%</color>減少させます。\n" +
                      $"衝撃波の最大サイズ: <color=#AEEAFF>{size}X{size}</color>";

        PT_BRSkillDesc = $"Uma onda de choque em forma de anel se expande para fora, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos que toca e reduzindo seu poder de ataque em <color=red>{decAttackPer}%</color> por <color=yellow>{duration} segundos</color>.\n" +
                         $"Tamanho máximo da onda de choque: <color=#AEEAFF>{size}X{size}</color>";

        RUSkillDesc = $"Кольцеобразная ударная волна расширяется наружу, нанося урон, равный <color=red>{attackCoeff} раз</color> силе атаки врагам, которых она касается, и снижая их силу атаки на <color=red>{decAttackPer}%</color> на <color=yellow>{duration} секунд</color>.\n" +
                        $"Максимальный размер ударной волны: <color=#AEEAFF>{size}X{size}</color>";

        ZH_HANSSkillDesc = $"环形冲击波向外扩展，对触及的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并在<color=yellow>{duration}秒</color>内将其攻击力降低<color=red>{decAttackPer}%</color>。\n" +
                          $"冲击波的最大尺寸：<color=#AEEAFF>{size}X{size}</color>";
    }
}
