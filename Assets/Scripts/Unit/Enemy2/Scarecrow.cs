using System.Collections;
using UnityEngine;

public class Scarecrow : UnitDefault
{
    [HideInInspector] public float duration;

    readonly WaitForSeconds wait = new(1f);

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(TauntRoutine());
        
        Destroy(gameObject, duration);
    }

    protected override void FixedUpdate()
    {
        
    }

    IEnumerator TauntRoutine()
    {
        while (!isDeath)
        {
            SoundPlay(skillSoundClip);
            CreateAttackBox(-1f, null, transform.position);

            foreach (UnitDefault unit in friends) {
                unit.Taunt(gameObject, 1f);
            }

            yield return wait;
        }
    }

    public override void UpdateSkillDesc()
    {

    }

    protected override void OnSkillAttack()
    {

    }

    protected override void UseSkill()
    {

    }
}
