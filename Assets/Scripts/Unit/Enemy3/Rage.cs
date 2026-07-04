using UnityEngine;

public class Rage : UnitDefault
{
    float beforeAttackPower;
    float beforeDefense;
    float beforeAttackSpeed;

    protected override void Awake()
    {
        base.Awake();

        beforeAttackPower = attackPower;
        beforeDefense = defense;
        beforeAttackSpeed = attackSpeed;
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        if (isDeath)
            return 0;

        if (isInvincible) {
            GameManager.instance.ShowDamage(0f, transform, Color.white);
            return 0;
        }

        // 치명타 확률 계산
        bool isCritical = Random.Range(1f, 100f) <= criticalPer;
        float damage = attack;
        if (!isPenetration) {
            damage = attack * (100f / (Defense + 100f));
        }
        // 데미지 텍스트 색상 설정
        Color color = Color.red;
        // 치명타 발동 시 치명타 계수 곱셈
        if (isCritical) {
            damage *= criticalCoeff;
            color = Color.yellow;
        }
        // 데미지는 1보다 작을 수 없음
        if (damage <= 1f)
            damage = 1f;

        // 사망시
        if (Health - damage <= 0) {
            // 현재 체력보다 데미지가 높을 경우 데미지는 현재 체력
            damage = Health;
            health -= damage;
            Death();
        }
        // 피격시
        else {
            health -= damage;
        }
        // 체력바 최신화
        updateHpBarAction(MaxHealth, Health);
        // 데미지 표시
        GameManager.instance.ShowDamage(damage, transform, color); // 현재 유닛의 위치에서 데미지 표시

        float healthRatio = 1f - (Health / MaxHealth);
        attackPower = beforeAttackPower * (1f + healthRatio);
        defense = beforeDefense * (1f + healthRatio);
        attackSpeed = beforeAttackSpeed * (1f + healthRatio);

        UpdateStatInfo();

        // 총 데미지 return
        return damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인이 잃은 체력에 비례하여 공격력, 방어력, 공격 속도가 상승합니다.";

        ENSkillDesc = $"Increases attack power, defense, and attack speed proportionally to the amount of health lost by this unit.";

        FRSkillDesc = $"Augmente la puissance d'attaque, la défense et la vitesse d'attaque proportionnellement à la quantité de santé perdue par cette unité.";

        ITSkillDesc = $"Aumenta attacco, difesa e velocità d'attacco in proporzione alla quantità di salute persa da questa unità.";

        DESkillDesc = $"Erhöht Angriffskraft, Verteidigung und Angriffsgeschwindigkeit proportional zur Menge an verlorener Gesundheit dieser Einheit.";

        ESSkillDesc = $"Aumenta el ataque, la defensa y la velocidad de ataque proporcionalmente a la cantidad de vida perdida por esta unidad.";

        JASkillDesc = $"失った体力に比例して攻撃力、防御力、攻撃速度が上昇します。";

        PT_BRSkillDesc = $"Aumenta o ataque, a defesa e a velocidade de ataque proporcionalmente à quantidade de vida perdida por esta unidade.";

        RUSkillDesc = $"Увеличивает атаку, защиту и скорость атаки пропорционально количеству потерянного здоровья этой единицы.";

        ZH_HANSSkillDesc = $"根据失去的生命值比例提升攻击力、防御和攻击速度。";
    }
}
