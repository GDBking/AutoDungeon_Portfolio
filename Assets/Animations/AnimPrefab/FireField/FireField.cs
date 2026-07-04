using UnityEngine;

public class FireField : MonoBehaviour
{
    public FireWizard unitComp;

    float burnDamage;
    int dealIdx;

    int timeCount;
    float elapsedTime;

    private void Start()
    {
        burnDamage = unitComp.BurnDamage;
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
                enemy.GetComponent<UnitDefault>().Burn(burnDamage, 1, unitComp, dealIdx);
        }
    }
}
