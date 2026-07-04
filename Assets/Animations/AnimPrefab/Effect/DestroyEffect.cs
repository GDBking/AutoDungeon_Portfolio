using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    Animator animator;
    AnimatorOverrideController animatorOverrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new(animator.runtimeAnimatorController);
    }
    
    public void SetAnimatorOverrideController(AnimationClip clip, bool isDestroy = true)
    {
        animatorOverrideController["SoldierEffect"] = clip;
        animator.runtimeAnimatorController = animatorOverrideController;

        if (isDestroy)
            Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}