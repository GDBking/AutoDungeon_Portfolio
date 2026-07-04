using UnityEngine;

public class SmokeShell : MonoBehaviour
{
    public Ninja unitComp;

    float incDefenseAmount;
    float incAttackSpeedPer;
    int dealIdx;

    int timeCount;
    float elapsedTime;

    private void Start()
    {
        incDefenseAmount = unitComp.IncDefenseAmount;
        incAttackSpeedPer = unitComp.IncAttackSpeedPer;
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

            Collider2D[] hitFriends = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, LayerMask.GetMask("Friendly Unit"));
            foreach (Collider2D friendly in hitFriends) {
                UnitDefault friendlyComp = friendly.GetComponent<UnitDefault>();
                friendlyComp.SetStat(friendlyComp.defenseStat, incDefenseAmount, false, true, 1f);
                friendlyComp.SetStat(friendlyComp.attackSpeedStat, incAttackSpeedPer, true, true, 1f);
                friendlyComp.SetStateBar(UnitDefault.State.incDefense, 1f);
                friendlyComp.SetStateBar(UnitDefault.State.incAttackSpeed, 1f);
            }
        }
    }
}
