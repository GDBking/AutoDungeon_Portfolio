using UnityEngine;

public class PrisonObj : UnitDefault
{
    [HideInInspector]
    public UnitDefault prisonUnit;

    protected override void Start()
    {
        prisonUnit.isKiting = false;
    }

    private void Update()
    {
        if (prisonUnit == null || prisonUnit.isDeath) {
            Death();
        }
    }

    protected override void FixedUpdate()
    {
        
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        float damage = 1f;

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
        GameManager.instance.ShowDamage(damage, transform, Color.red); // 현재 유닛의 위치에서 데미지 표시
        // 총 데미지 return
        return damage;
    }

    public override void Death()
    {
        if (prisonUnit != null) {
            prisonUnit.isKiting = isKitingUnit;
            Prison.prisonList.Remove(prisonUnit.gameObject);
        }
        else {
            Prison.prisonList.RemoveAll(x => x == null);
        }

        Destroy(gameObject);
    }

    public override void Healing(float healingAmount, int idx = -1)
    {
        
    }

    public override void UpdateSkillDesc()
    {
        
    }
}
