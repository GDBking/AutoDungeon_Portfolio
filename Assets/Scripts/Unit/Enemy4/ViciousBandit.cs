using UnityEngine;

public class ViciousBandit : UnitDefault
{
    [Header("흉적")]
    public float decAttackPer;
    public float reflectDamagePer;
    public float duration;

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        float damage = base.Hit(attack, criticalPer, hitUnit, isPenetration);

        if (hitUnit != null) {
            SoundPlay(skillSoundClip);
            CreateAttackBox(-1f, null, hitUnit.transform.position);

            hitUnit.ViciousDebuff(decAttackPer, duration);

            hitUnit.Hit(damage / 100f * reflectDamagePer, 0f, this);
        }

        return damage;

    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"피격 시 <color=yellow>{duration}초</color> 동안 상대의 공격력을 <color=red>{decAttackPer}%</color> 감소시키며, 받은 데미지의 <color=red>{reflectDamagePer}%</color>를 반사합니다.\n" +
                    $"(공격력 감소는 중첩되지 않음)";

        ENSkillDesc = $"When hit, reduces the opponent's attack power by <color=red>{decAttackPer}%</color> for <color=yellow>{duration} seconds</color> and reflects <color=red>{reflectDamagePer}%</color> of the damage received.\n" +
                      $"(Attack power reduction does not stack)"; 

        FRSkillDesc = $"Lorsqu'il est touché, réduit la puissance d'attaque de l'adversaire de <color=red>{decAttackPer}%</color> pendant <color=yellow>{duration} secondes</color> et renvoie <color=red>{reflectDamagePer}%</color> des dégâts reçus.\n" +
                        $"(La réduction de la puissance d'attaque ne s'empile pas)";

        ITSkillDesc = $"Quando viene colpito, riduce la potenza di attacco dell'avversario del <color=red>{decAttackPer}%</color> per <color=yellow>{duration} secondi</color> e riflette il <color=red>{reflectDamagePer}%</color> dei danni ricevuti.\n" +
                        $"(La riduzione della potenza di attacco non si accumula)";

        DESkillDesc = $"Wenn er getroffen wird, wird die Angriffskraft des Gegners um <color=red>{decAttackPer}%</color> für <color=yellow>{duration} Sekunden</color> reduziert und <color=red>{reflectDamagePer}%</color> des erhaltenen Schadens reflektiert.\n" +
                        $"(Die Reduzierung der Angriffskraft stapelt sich nicht)";

        ESSkillDesc = $"Cuando es golpeado, reduce el poder de ataque del oponente en <color=red>{decAttackPer}%</color> durante <color=yellow>{duration} segundos</color> y refleja el <color=red>{reflectDamagePer}%</color> del daño recibido.\n" +
                        $"(La reducción del poder de ataque no se acumula)";

        JASkillDesc = $"被撃時に相手の攻撃力を<color=red>{decAttackPer}%</color>、<color=yellow>{duration}秒</color>間減少させ、受けたダメージの<color=red>{reflectDamagePer}%</color>を反射します。\n" +
                        $"（攻撃力減少は重複しません）";

        PT_BRSkillDesc = $"Quando atingido, reduz o poder de ataque do oponente em <color=red>{decAttackPer}%</color> por <color=yellow>{duration} segundos</color> e reflete <color=red>{reflectDamagePer}%</color> do dano recebido.\n" +
                        $"(A redução do poder de ataque não acumula)";

        RUSkillDesc = $"При попадании снижает силу атаки противника на <color=red>{decAttackPer}%</color> на <color=yellow>{duration} секунд</color> и отражает <color=red>{reflectDamagePer}%</color> полученного урона.\n" +
                        $"(Снижение силы атаки не складывается)";

        ZH_HANSSkillDesc = $"被击中时，将对手的攻击力降低<color=red>{decAttackPer}%</color>，持续<color=yellow>{duration}秒</color>，并反弹所受伤害的<color=red>{reflectDamagePer}%</color>。\n" +
                        $"（攻击力降低不叠加）";
    }
}
