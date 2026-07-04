using UnityEngine;

public class DeadlyPoisonPool : MonoBehaviour
{
    public DeadlyPoison unitComp;

    int timeCount;
    float elapsedTime;
    float decSpeedPer;
    float poisonDamage;
    int dealIdx;

    private void Start()
    {
        transform.localScale *= unitComp.size;
        timeCount = unitComp.count;
        decSpeedPer = unitComp.decSpeedPer;
        poisonDamage = unitComp.poisonDamage;
        dealIdx = unitComp.dealMetricsIdx;
    }

    private void Update()
    {
        elapsedTime -= Time.deltaTime;

        if (elapsedTime <= 0f) {
            elapsedTime = 1f;

            if (timeCount-- == 0) {
                Destroy(gameObject);
                return;
            }

            Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Friendly Unit"));
            foreach (Collider2D enemy in hitEnemys) {
                UnitDefault enemyComp = enemy.GetComponent<UnitDefault>();
                enemyComp.SetStat(enemyComp.speedStat, decSpeedPer, true, false, 1f);
                enemyComp.SetStateBar(UnitDefault.State.decSpeed, 1f);
                enemyComp.Poison(poisonDamage, 1, unitComp, dealIdx);
            }
        }
    }
}
