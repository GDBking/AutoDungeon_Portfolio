using UnityEngine;

public class PoisonPool : MonoBehaviour
{
    public Stinger unitComp;

    float poisonDamage;
    int dealIdx;

    int timeCount;
    float elapsedTime;

    private void Start()
    {
        poisonDamage = unitComp.PoisonDamage;
        dealIdx = unitComp.dealMetricsIdx;

        transform.localScale *= unitComp.Size;
        timeCount = unitComp.Count;
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

            Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Enemy Unit"));
            foreach (Collider2D enemy in hitEnemys)
                enemy.GetComponent<UnitDefault>().Poison(poisonDamage, 1, unitComp, dealIdx);
        }
    }
}