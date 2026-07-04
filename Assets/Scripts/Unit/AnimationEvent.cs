using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    UnitDefault unitDefault;

    private void Awake()
    {
        unitDefault = transform.parent.GetComponent<UnitDefault>();
    }

    void OnShortAttack()
    {
        unitDefault.OnShortAttack();
    }

    void OnFire()
    {
        if (unitDefault.AttackTarget == null)
            return;

        StartCoroutine(unitDefault.OnFire(unitDefault.AttackTarget, unitDefault.AttackTarget.transform.position, false));
    }

    void OnSkillAttack()
    {
        unitDefault.onSkillAction();
    }
}
