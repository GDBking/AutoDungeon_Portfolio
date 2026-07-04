using UnityEngine;

public class Apparition : UnitDefault
{
    [Header("환영")]
    public ApparitionClone apparitionClone;
    public float duration;
    public float size;

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
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
        CreateAttackBox(-1f, null, pos);

        ApparitionClone apparitionCloneObj = Instantiate(apparitionClone, pos, Quaternion.identity, enemyField);
        apparitionCloneObj.attackSize = size;
        apparitionCloneObj.TauntTarget(duration, size);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 위치에 <color=yellow>{duration}초</color> 동안 유지되는 환영을 소환해 환영 반경 <color=#AEEAFF>{size}X{size}</color>범위의 적들을 도발합니다.";
        
        ENSkillDesc = $"Summons an apparition at the position of the attacked target that lasts for <color=yellow>{duration} seconds</color> and taunts enemies in an area of <color=#AEEAFF>{size}X{size}</color> around the apparition.";

        FRSkillDesc = $"Invoque une apparition à la position de la cible attaquée, qui dure <color=yellow>{duration} secondes</color> et attire les ennemis dans une zone de <color=#AEEAFF>{size}X{size}</color> autour de l'apparition.";

        ITSkillDesc = $"Evoca un'apparizione nella posizione del bersaglio attaccato, che dura <color=yellow>{duration} secondi</color> e provoca i nemici in un'area di <color=#AEEAFF>{size}X{size}</color> intorno all'apparizione.";

        DESkillDesc = $"Beschwört eine Erscheinung an der Position des angegriffenen Ziels, die <color=yellow>{duration} Sekunden</color> anhält und Feinde in einem Bereich von <color=#AEEAFF>{size}X{size}</color> um die Erscheinung herum anzieht.";

        ESSkillDesc = $"Invoca una aparición en la posición del objetivo atacado, que dura <color=yellow>{duration} segundos</color> y atrae a los enemigos en un área de <color=#AEEAFF>{size}X{size}</color> alrededor de la aparición.";

        JASkillDesc = $"攻撃中の対象の位置に<color=yellow>{duration}秒</color>間維持される幻影を召喚し、幻影の半径<color=#AEEAFF>{size}X{size}</color>範囲の敵を挑発します。";

        PT_BRSkillDesc = $"Invoca uma aparição na posição do alvo atacado, que dura <color=yellow>{duration} segundos</color> e atrai inimigos em uma área de <color=#AEEAFF>{size}X{size}</color> ao redor da aparição.";

        RUSkillDesc = $"Призывает призрак в позицию атакуемой цели, который длится <color=yellow>{duration} секунд</color> и привлекает врагов в области <color=#AEEAFF>{size}X{size}</color> вокруг призрака.";

        ZH_HANSSkillDesc = $"在被攻击目标的位置召唤一个持续<color=yellow>{duration}秒</color>的幻影，吸引幻影周围<color=#AEEAFF>{size}X{size}</color>范围内的敌人。";
    }
}