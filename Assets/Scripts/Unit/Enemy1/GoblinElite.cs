using UnityEngine;

public class GoblinElite : UnitDefault
{
    [Header("과다출혈")]
    public float bleedingDamage;
    public int count;

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
        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        unitComp.Bleeding(bleedingDamage, count, this);
        unitComp.SetStateBar(State.bleeding, count);

        CreateAttackBox(AttackPower, SkillTarget, SkillTarget.transform.position, isSkill: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 <color=yellow>{count}초</color> 동안 출혈을 입힙고, 기본 공격 피해를 입힙니다.\n" +
                    $"초당 출혈 데미지: <color=red>{bleedingDamage}</color>";

        ENSkillDesc = $"Inflicts bleeding on the attacking target for <color=yellow>{count} seconds</color> and deals basic attack damage.\n" +
                      $"Bleeding damage per second: <color=red>{bleedingDamage}</color>";

        FRSkillDesc = $"Inflige une hémorragie à la cible attaquante pendant <color=yellow>{count} secondes</color> et inflige des dégâts d'attaque de base.\n" +
                      $"Dégâts d'hémorragie par seconde : <color=red>{bleedingDamage}</color>";

        ITSkillDesc = $"Infligge sanguinamento al bersaglio attaccante per <color=yellow>{count} secondi</color> e infligge danni di attacco base.\n" +
                      $"Danno da sanguinamento al secondo: <color=red>{bleedingDamage}</color>";

        DESkillDesc = $"Fügt dem angegriffenen Ziel für <color=yellow>{count} Sekunden</color> Blutung zu und verursacht normalen Angriffsschaden.\n" +
                      $"Blutungsschaden pro Sekunde: <color=red>{bleedingDamage}</color>";

        ESSkillDesc = $"Inflige sangrado al objetivo atacado durante <color=yellow>{count} segundos</color> y causa daño de ataque básico.\n" +
                      $"Daño por sangrado por segundo: <color=red>{bleedingDamage}</color>";

        JASkillDesc = $"攻撃中の対象に<color=yellow>{count}秒</color>の出血を与え、通常攻撃ダメージを与えます。\n" +
                      $"毎秒の出血ダメージ: <color=red>{bleedingDamage}</color>";

        PT_BRSkillDesc = $"Inflige sangramento no alvo atacado por <color=yellow>{count} segundos</color> e causa dano de ataque básico.\n" +
                         $"Dano de sangramento por segundo: <color=red>{bleedingDamage}</color>";

        RUSkillDesc = $"Накладывает кровотечение на атакуемую цель на <color=yellow>{count} секунд</color> и наносит базовый урон атаки.\n" +
                      $"Урон от кровотечения в секунду: <color=red>{bleedingDamage}</color>";

        ZH_HANSSkillDesc = $"对被攻击目标造成<color=yellow>{count}秒</color>的出血，并造成基础攻击伤害。\n" +
                           $"每秒出血伤害：<color=red>{bleedingDamage}</color>";
    }
}