public class GoblinGold : DefaultEnemy
{
    public enum GoldType { attackPower, defense, attackSpeed, silencer }
    public GoldType goldType;
    public float decPer;
    string decStat;
    string ENDecStat;

    UnitDefault greed;

    protected override void Awake()
    {
        base.Awake();

        switch (goldType) {
            case GoldType.attackPower:
                decStat = "공격력을";
                ENDecStat = "Attack Power";
                break;
            case GoldType.defense:
                decStat = "방어력을";
                ENSkillDesc = "Defense";
                break;
            case GoldType.attackSpeed:
                decStat = "공격 속도를";
                ENSkillDesc = "Attack Speed";
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        foreach (UnitDefault unitComp in enemies) {
            if (unitComp is Mammon) {
                greed = unitComp;
                break;
            }
        }
    }

    public override void Death()
    {
        if (greed != null) {
            SoundPlay(skillSoundClip);
            CreateAttackBox(-1f, null, greed.transform.position);

            switch (goldType) {
                case GoldType.attackPower:
                    greed.SetStat(greed.attackPowerStat, decPer, true, false);
                    greed.SetStateBar(State.decAttack, -2f);
                    break;
                case GoldType.defense:
                    greed.SetStat(greed.defenseStat, decPer, true, false);
                    greed.SetStateBar(State.decDefense, -2f);
                    break;
                case GoldType.attackSpeed:
                    greed.SetStat(greed.attackSpeedStat, decPer, true, false);
                    greed.SetStateBar(State.decAttackSpeed, -2f);
                    break;
                case GoldType.silencer:
                    greed.Silence(decPer);
                    greed.SetStateBar(State.silence, decPer);
                    break;
            }
        }

        base.Death();
    }

    public override void UpdateSkillDesc()
    {
        if (goldType != GoldType.silencer)
        {
            skillDesc = $"사망 시 그리드의 {decStat} <color=red>{decPer}%</color> 감소시킵니다.";
            ENSkillDesc = $"When killed, decreases the Grid’s {ENDecStat} by {decPer}%.";
        }
        else
        {
            skillDesc = $"사망 시 그리드의 스킬을 <color=yellow>{decPer}초</color> 동안 봉인합니다.";
        }
    }
}
