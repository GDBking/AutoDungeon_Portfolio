using UnityEngine;

public class Bullet : MonoBehaviour
{
    public UnitDefault unitDefault;
    public GameObject target;
    public string enemyTag;

    Animator animator;
    AnimatorOverrideController animatorOverrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new(animator.runtimeAnimatorController);
    }

    public void SetAnimatorOverrideController(AnimationClip clip, float size)
    {
        animatorOverrideController["ArcherBullet"] = clip;
        animator.runtimeAnimatorController = animatorOverrideController;
        transform.localScale *= size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Friendly Barrier") && unitDefault.CompareTag("Enemy Unit")) {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject != target) {
            if (enemyTag == "")
                return;
            else if (!collision.gameObject.CompareTag(enemyTag))
                return;
        }

        switch (tag) {
            case "Bullet":
                unitDefault.SoundPlay(unitDefault.bulletHitClip);
                unitDefault.CreateAttackBox(unitDefault.AttackPower, target, target.transform.position, isSkill: false);
                Destroy(gameObject);
                break;
            case "Skill Bullet":
                unitDefault.SoundPlay(unitDefault.skillBulletHitClip);
                unitDefault.onSkillFireAction(target);
                Destroy(gameObject);
                break;
            case "Penetration Bullet":
                unitDefault.SoundPlay(unitDefault.skillBulletHitClip);
                unitDefault.onSkillFireAction(collision.gameObject);
                break;
        }
    }
}