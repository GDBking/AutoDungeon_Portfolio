using UnityEngine;

public class MireObj : MonoBehaviour
{
    [HideInInspector]
    public Mire mireComp;

    float decSpeedAmount;
    float decDefenseAmount;
    int cnt;
    int destroyCnt;
    float elapsedTime = 1f;

    private void Start()
    {
        transform.localScale *= mireComp.size;

        decSpeedAmount = mireComp.decSpeedAmount;
        decDefenseAmount = mireComp.decDefenseAmount;
        cnt = mireComp.count;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 1f) {
            elapsedTime = 0f;

            if (destroyCnt++ == cnt) {
                Destroy(gameObject);
                return;
            }

            Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Friendly Unit"));
            foreach (Collider2D enemy in hitEnemys) {
                UnitDefault unitComp = enemy.GetComponent<UnitDefault>();
                unitComp.SetStat(unitComp.defenseStat, decDefenseAmount, false, false, 1f);
                unitComp.SetStat(unitComp.speedStat, decSpeedAmount, false, false, 1f);
                unitComp.SetStateBar(UnitDefault.State.decDefense, 1f);
                unitComp.SetStateBar(UnitDefault.State.decSpeed, 1f);
            }
        }
    }
}
