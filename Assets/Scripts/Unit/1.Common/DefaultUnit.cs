using UnityEngine;

public class DefaultUnit : UnitDefault
{
    [Header("±âº» À¯´Ö")]
    public AnimationClip spawnAnimClip;
    public AnimationClip equipAnimClip;

    public AudioClip spawnSound;
    public AudioClip equipSound;

    protected override void Awake()
    {
        base.Awake();

        isEquipmentUnit = true;

        SoundPlay(spawnSound);
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
