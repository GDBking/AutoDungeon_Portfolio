using UnityEngine;

public class Depreciation : UnitDefault
{
    [Header("절하")]
    public float decDefenseAmount;
    public float duration;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindGreaterHpEnemy();
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        CreateAttackBox(-1f, null, target.transform.position);
        unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, duration);
        unitComp.SetStateBar(State.decDefense, duration);
    }

    void FindGreaterHpEnemy()
    {
        float greaterHp = 0f; // 현재 가장 높은 체력
        GameObject greaterHpEnemy = null; // 현재 가장 체력이 높은 적 유닛
        // 모든 적 유닛을 검사하며 가장 체력이 높은 적 유닛을 판별
        foreach (UnitDefault unitComp in friends) {
            // 체력이 현재보다 높으면 최신화
            if (greaterHp < unitComp.Health) {
                greaterHp = unitComp.Health;
                greaterHpEnemy = unitComp.gameObject;
            }
        }
        
        SkillTarget = greaterHpEnemy;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"남은 체력이 가장 높은 적 유닛에게 창을 던져 <color=yellow>{duration}초</color> 동안 방어력을 <color=red>{decDefenseAmount}</color>만큼 감소시킵니다.";

        ENSkillDesc = $"Throws a spear at the enemy with the highest remaining health, reducing their defense by <color=red>{decDefenseAmount}</color> for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Lance une lance contre l'ennemi ayant le plus de points de vie restants, réduisant sa défense de <color=red>{decDefenseAmount}</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Lancia una lancia contro il nemico con la salute rimanente più alta, riducendo la sua difesa di <color=red>{decDefenseAmount}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Wirft einen Speer auf den Gegner mit den meisten verbleibenden Lebenspunkten und reduziert seine Verteidigung um <color=red>{decDefenseAmount}</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Lanza una lanza al enemigo con la mayor vida restante, reduciendo su defensa en <color=red>{decDefenseAmount}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"すべての敵に攻撃力<color=red>{decDefenseAmount}</color>の槍を投げ、<color=yellow>{duration}秒</color>の間、防御力を<color=red>{decDefenseAmount}</color>だけ減少させます。";

        PT_BRSkillDesc = $"Lança uma lança no inimigo com a maior vida restante, diminuindo sua defesa em <color=red>{decDefenseAmount}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Бросает копье в врага с наибольшим оставшимся здоровьем, снижая его защиту на <color=red>{decDefenseAmount}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"向剩余生命最高的敌人投掷矛，使其在<color=yellow>{duration}秒</color>内防御降低<color=red>{decDefenseAmount}</color>。";
    }
}
