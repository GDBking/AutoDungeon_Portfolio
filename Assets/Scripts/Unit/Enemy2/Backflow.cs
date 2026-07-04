using UnityEngine;

public class Backflow : UnitDefault
{
    [Header("역류")]
    public float decAttackPer;
    public float decAttackSpeedPer;
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        CreateAttackBox(-1f, null, transform.position);
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindStrongestEnemy();
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        Vector2 targetPos = SkillTarget.transform.position;
        targetPos += transform.position.x < targetPos.x ? Vector2.right : Vector2.left;
        transform.position = targetPos;
        CreateAttackBox(-1f, null, transform.position);

        UnitDefault enemyComp = SkillTarget.GetComponent<UnitDefault>();

        enemyComp.SetStat(enemyComp.attackPowerStat, decAttackPer, true, false, duration);
        enemyComp.SetStat(enemyComp.attackSpeedStat, decAttackSpeedPer, true, false, duration);

        enemyComp.SetStateBar(State.decAttack, duration);
        enemyComp.SetStateBar(State.decAttackSpeed, duration);
    }

    void FindStrongestEnemy()
    {
        float strongest = -1f; // 현재 가장 강한 적 유닛의 공격력
        GameObject strongestEnemy = null; // 현재 가장 강한 적 유닛
        // 모든 적 유닛을 검사하며 가장 강한 적 유닛을 판별
        foreach (UnitDefault unitComp in friends) {
            if (unitComp.AttackPower > strongest) {
                strongest = unitComp.AttackPower;
                strongestEnemy = unitComp.gameObject;
            }
        }

        // 가장 강한 적 유닛 SkillTarget에 저장
        SkillTarget = strongestEnemy;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격력이 가장 높은 적 유닛 뒤로 순간이동 하고, <color=yellow>{duration}초</color> 동안 공격력을 <color=red>{decAttackPer}%</color>, 공격 속도를 <color=red>{decAttackSpeedPer}%</color> 감소시킵니다.";

        ENSkillDesc = $"Teleports behind the enemy unit with the highest attack power and reduces its attack power by <color=red>{decAttackPer}%</color> and attack speed by <color=red>{decAttackSpeedPer}%</color> for <color=yellow>{duration} seconds.</color>";

        FRSkillDesc = $"Se téléporte derrière l'unité ennemie avec la plus haute puissance d'attaque et réduit sa puissance d'attaque de <color=red>{decAttackPer}%</color> et sa vitesse d'attaque de <color=red>{decAttackSpeedPer}%</color> pendant <color=yellow>{duration} secondes.</color>";

        ITSkillDesc = $"Si teletrasporta dietro l'unità nemica con la potenza di attacco più alta e riduce la sua potenza di attacco del <color=red>{decAttackPer}%</color> e la sua velocità di attacco del <color=red>{decAttackSpeedPer}%</color> per <color=yellow>{duration} secondi.</color>";

        DESkillDesc = $"Teleportiert sich hinter die feindliche Einheit mit der höchsten Angriffskraft und reduziert deren Angriffskraft um <color=red>{decAttackPer}%</color> und Angriffsgeschwindigkeit um <color=red>{decAttackSpeedPer}%</color> für <color=yellow>{duration} Sekunden.</color>";

        ESSkillDesc = $"Se teletransporta detrás de la unidad enemiga con el mayor poder de ataque y reduce su poder de ataque en <color=red>{decAttackPer}%</color> y su velocidad de ataque en <color=red>{decAttackSpeedPer}%</color> durante <color=yellow>{duration} segundos.</color>";

        JASkillDesc = $"攻撃力が最も高い敵ユニットの後ろに瞬間移動し、<color=yellow>{duration}秒</color>の間、攻撃力を<color=red>{decAttackPer}%</color>、攻撃速度を<color=red>{decAttackSpeedPer}%</color>減少させます。";

        PT_BRSkillDesc = $"Teletransporta-se atrás da unidade inimiga com o maior poder de ataque e reduz seu poder de ataque em <color=red>{decAttackPer}%</color> e sua velocidade de ataque em <color=red>{decAttackSpeedPer}%</color> por <color=yellow>{duration} segundos.</color>";

        RUSkillDesc = $"Телепортируется за вражеский юнит с наибольшей атакующей силой и снижает его атакующую силу на <color=red>{decAttackPer}%</color> и скорость атаки на <color=red>{decAttackSpeedPer}%</color> на <color=yellow>{duration} секунд.</color>";

        ZH_HANSSkillDesc = $"瞬移到攻击力最高的敌方单位后方，<color=yellow>{duration}秒</color>内降低其攻击力<color=red>{decAttackPer}%</color>和攻击速度<color=red>{decAttackSpeedPer}%</color>。";
    }
}