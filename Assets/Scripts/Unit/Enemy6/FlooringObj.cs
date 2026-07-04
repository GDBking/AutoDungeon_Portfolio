using UnityEngine;

public class FlooringObj : MonoBehaviour
{
    [HideInInspector]
    public Flooring flooringComp;

    float decDefensePer;
    int cnt;
    float elapsedTime = 1f;

    private void Start()
    {
        transform.localScale *= flooringComp.size;
        decDefensePer = flooringComp.decDefensePer;
        cnt = flooringComp.count;
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        if (elapsedTime >= 1f) {
            elapsedTime = 0f;

            if (cnt-- == 0) {
                Destroy(gameObject);
                return;
            }

            Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Friendly Unit"));
            foreach (Collider2D enemy in hitEnemys) {
                UnitDefault unitComp = enemy.GetComponent<UnitDefault>();
                unitComp.SetStat(unitComp.defenseStat, decDefensePer, true, false, 1f);
                unitComp.SetStateBar(UnitDefault.State.decDefense, 1f);
            }
        }
    }
}
