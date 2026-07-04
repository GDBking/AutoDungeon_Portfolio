using UnityEngine;

public class GravityField : MonoBehaviour
{
    public Gravitor unitComp;

    float dotDamage;
    float criticalPer;
    int dealIdx;

    int timeCount;
    float elapsedTime;

    private void Start()
    {
        dotDamage = unitComp.DotDamage;
        criticalPer = unitComp.CriticalPer;
        dealIdx = unitComp.dealMetricsIdx;

        transform.localScale *= unitComp.Size;
        timeCount = unitComp.Count;
    }

    private void Update()
    {
        elapsedTime -= Time.deltaTime;

        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Enemy Unit"));
        foreach (Collider2D enemy in hitEnemys) {
            UnitDefault enemyComp = enemy.transform.GetComponent<UnitDefault>();
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, transform.position, Time.deltaTime * enemyComp.Speed * 1.1f);
        }

        if (elapsedTime <= 0f) {
            elapsedTime = 1f;

            if (timeCount-- == 0) {
                Destroy(gameObject);
                return;
            }

            foreach (Collider2D enemy in hitEnemys) {
                float damage = enemy.GetComponent<UnitDefault>().Hit(dotDamage, criticalPer, unitComp);
                DealMetrics.instance.UpdateDealMetrics(dealIdx, damage);
            }
        }
    }
}
