using System.Collections;
using UnityEngine;

public class RoseObj : MonoBehaviour
{
    [HideInInspector]
    public Rose roseComp;
    public Animator anim;

    AudioClip skillClip;

    private void Start()
    {
        skillClip = roseComp.skillBulletHitClip;
    }

    IEnumerator TriggerRoutine(UnitDefault unitComp)
    {
        anim.SetBool("isActive", true);
        yield return null;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        if (unitComp == null) {
            Destroy(gameObject);
            yield break;
        }

        unitComp.Enthrallment(roseComp.duration);
        roseComp.CreateAttackBox(roseComp.AttackPower * roseComp.attackCoeff, unitComp.gameObject, unitComp.transform.position, isPenetration: true);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!anim.GetBool("isActive") && collision.CompareTag("Friendly Unit")) {
            AudioManager.instance.PlaySfx(skillClip);

            StartCoroutine(TriggerRoutine(collision.GetComponent<UnitDefault>()));
        }
    }
}
