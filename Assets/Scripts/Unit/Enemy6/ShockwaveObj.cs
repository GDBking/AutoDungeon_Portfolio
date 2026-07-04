using UnityEngine;

public class ShockwaveObj : MonoBehaviour
{
    public AnimationClip skillAnimClip;

    [HideInInspector] public Shockwave unitComp;

    float attackPower;
    float criticalPer;
    float decAttackPer;
    float duration;
    float size;

    AudioClip skillHitClip;

    private void Start()
    {
        attackPower = unitComp.AttackPower * unitComp.attackCoeff;
        criticalPer = unitComp.CriticalPer;
        decAttackPer = unitComp.decAttackPer;
        duration = unitComp.duration;
        size = unitComp.size;

        skillHitClip = unitComp.skillBulletHitClip;
    }

    private void Update()
    {
        float time = Time.deltaTime * size;
        Vector2 scale = transform.localScale;
        transform.localScale = new Vector2(scale.x + time, scale.y + time);
        if (transform.localScale.x >= size) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Friendly Unit")) {
            GameObject effect = Instantiate(GameManager.instance.effectObj, collision.transform.position, Quaternion.identity);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);
            effect.transform.localScale *= 0.5f;

            UnitDefault enemy = collision.GetComponent<UnitDefault>();
            enemy.SetStat(enemy.attackPowerStat, decAttackPer, true, false, duration);
            enemy.SetStateBar(UnitDefault.State.decAttack, duration);
            enemy.Hit(attackPower, criticalPer, unitComp);
        }
    }
}
