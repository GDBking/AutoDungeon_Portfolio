using System.Collections;
using UnityEngine;

public class GiantMeteor : MonoBehaviour
{
    [HideInInspector] public Satan unitComp;
    public Animator anim;

    float attackPower;
    float meteorBurnDamage;
    int meteorBurnCnt;
    float criticalPer;

    private void Start()
    {
        attackPower = unitComp.AttackPower * unitComp.attackCoeff;
        meteorBurnDamage = unitComp.meteorBurnDamage;
        meteorBurnCnt = unitComp.meteorBurnCnt;
        criticalPer = unitComp.CriticalPer;

        StartCoroutine(MeteorRoutine());
    }

    IEnumerator MeteorRoutine()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        for (int i = UnitDefault.friends.Count - 1; i >= 0; i--) {
            UnitDefault.friends[i].Burn(meteorBurnDamage, meteorBurnCnt, unitComp);
            UnitDefault.friends[i].Hit(attackPower, criticalPer, unitComp);
        }

        Destroy(gameObject);
    }
}
