using UnityEngine;

public class Tsunami : MonoBehaviour
{
    public SpriteRenderer sprRenderer;
    public AnimationClip skillAnimClip;
    [HideInInspector] public Leviathan unitComp;

    float attackPower;
    float criticalPer;
    float duration;
    Color32 color = new(150, 0, 0, 190);

    private void Start()
    {
        attackPower = unitComp.AttackPower * unitComp.attackCoeff;
        criticalPer = unitComp.CriticalPer;
        duration = unitComp.skillDuration;

        if (Leviathan.cnt == 0) {
            sprRenderer.color = color;
        }
    }

    private void Update()
    {
        transform.Translate(4f * Time.deltaTime * Vector2.left);

        if (transform.position.x <= -8.7f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Friendly Unit"))
            return;

        if (collision.TryGetComponent<UnitDefault>(out UnitDefault enemy)) {
            GameObject effect = Instantiate(GameManager.instance.effectObj, enemy.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            if (Leviathan.cnt == 0) {
                effect.GetComponent<SpriteRenderer>().color = color;
                enemy.OnStunAnim(duration);
            }

            enemy.Hit(attackPower, criticalPer, unitComp);
        }
    }
}
