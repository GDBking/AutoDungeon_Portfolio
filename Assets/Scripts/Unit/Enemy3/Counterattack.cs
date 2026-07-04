using System.Collections;
using UnityEngine;

public class Counterattack : UnitDefault
{
    [Header("반격")]
    public float attackCoeff;
    public float duration;

    float saveDamage;
    WaitForSeconds wait;

    protected override void Start()
    {
        base.Start();

        wait = new(duration);
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

        SoundPlay(skillSoundClip);

        CreateAttackBox(saveDamage * attackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        float damage = base.Hit(attack, criticalPer, hitUnit, isPenetration);
        StartCoroutine(SaveDamage(damage));
        return damage;
    }

    IEnumerator SaveDamage(float damage)
    {
        saveDamage += damage;
        yield return wait;
        saveDamage -= damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 스킬 사용 <color=yellow>{duration}초</color> 전까지 받은 모든 피해량의 <color=red>{attackCoeff}배</color> 피해를 입힙니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the total damage this unit received during the previous <color=yellow>{duration} seconds</color> to the current target when using the skill.";

        FRSkillDesc = $"Inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la quantité totale de dégâts que cette unité a subie au cours des <color=yellow>{duration} secondes</color> précédentes à la cible actuelle lors de l'utilisation de la compétence.";

        ITSkillDesc = $"Infligge danni pari a <color=red>{attackCoeff} volte</color> il totale dei danni ricevuti da questa unità nei precedenti <color=yellow>{duration} secondi</color> al bersaglio corrente quando si usa l'abilità.";

        DESkillDesc = $"Verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Gesamtschadensmenge, die diese Einheit in den vorherigen <color=yellow>{duration} Sekunden</color> erlitten hat, am aktuellen Ziel beim Einsatz der Fähigkeit.";

        ESSkillDesc = $"Inflige daño igual a <color=red>{attackCoeff} veces</color> el total de daño que esta unidad recibió durante los <color=yellow>{duration} segundos</color> anteriores al objetivo actual al usar la habilidad.";

        JASkillDesc = $"スキル使用時に、このユニットが直前の<color=yellow>{duration}秒</color>間に受けたダメージの<color=red>{attackCoeff}倍</color>を現在の対象に与えます。";

        PT_BRSkillDesc = $"Causa dano igual a <color=red>{attackCoeff} vezes</color> o total de dano que esta unidade recebeu durante os <color=yellow>{duration} segundos</color> anteriores ao alvo atual ao usar a habilidade.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> суммарному урону, полученному этой единицей за предыдущие <color=yellow>{duration} секунд</color>, текущей цели при использовании навыка.";

        ZH_HANSSkillDesc = $"在使用技能时，向当前目标造成在之前<color=yellow>{duration}秒</color>内受到的伤害的<color=red>{attackCoeff}倍</color>的伤害。";
    }
}