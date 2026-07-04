using UnityEngine;

public class Extortion : UnitDefault
{
    [Header("갈취")]
    public int manaExploitationAmount;
    public float incAttackPer;
    public float incAttackSpeedPer;
    public float duration;
    public AnimationClip extortionEffectAnimClip;
    public AnimationClip friendlySkillEffectAnimClip;

    protected override void Start()
    {
        base.Start();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        foreach (UnitDefault enemy in friends) {
            GameObject effect2 = Instantiate(attackObj, enemy.transform);
            effect2.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            enemy.SetMana(-manaExploitationAmount);
        }

        GameObject effect = Instantiate(attackObj, transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(extortionEffectAnimClip);

        SetStat(attackPowerStat, incAttackPer, true, true, duration);
        SetStat(attackSpeedStat, incAttackSpeedPer, true, true, duration);
        SetStateBar(State.incAttack, duration);
        SetStateBar(State.incAttackSpeed, duration);

        foreach (UnitDefault friendly in enemies) {
            if (friendly == this || friendly is Extortion)
                continue;

            GameObject effect2 = Instantiate(attackObj, friendly.transform);
            effect2.GetComponent<DestroyEffect>().SetAnimatorOverrideController(friendlySkillEffectAnimClip);

            friendly.SetMana(manaExploitationAmount);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"모든 적 유닛의 마나를 <color=yellow>{manaExploitationAmount}</color>씩 흡수하여 모든 아군 유닛의 마나를 <color=yellow>{manaExploitationAmount}</color>씩 충전시킵니다.\n" +
                    $"본인은 {duration}초 동안 공격력이 <color=green>{incAttackPer}%</color>, 공격 속도가 <color=green>{incAttackSpeedPer}%</color> 증가합니다.\n" +
                    $"(갈취는 마나 회복 제한)";

        ENSkillDesc = $"Absorbs <color=yellow>{manaExploitationAmount}</color> mana from all enemy units and restores <color=yellow>{manaExploitationAmount}</color> mana to all allied units.\n" +
                       $"Increases own attack power by <color=green>{incAttackPer}%</color> and attack speed by <color=green>{incAttackSpeedPer}% for {duration} seconds.\n" +
                       $"(Extortion has mana recovery limit)";

        FRSkillDesc = $"Absorbe <color=yellow>{manaExploitationAmount}</color> de mana de toutes les unités ennemies et restaure <color=yellow>{manaExploitationAmount}</color> de mana à toutes les unités alliées.\n" +
                       $"Augmente sa puissance d'attaque de <color=green>{incAttackPer}%</color> et sa vitesse d'attaque de <color=green>{incAttackSpeedPer}%</color> pendant {duration} secondes.\n" +
                       $"(L'extorsion a une limite de récupération de mana)";

        ITSkillDesc = $"Assorbe <color=yellow>{manaExploitationAmount}</color> di mana da tutte le unità nemiche e ripristina <color=yellow>{manaExploitationAmount}</color> di mana a tutte le unità alleate.\n" +
                          $"Aumenta la propria potenza di attacco di <color=green>{incAttackPer}%</color> e la velocità di attacco di <color=green>{incAttackSpeedPer}%</color> per {duration} secondi.\n" +
                          $"(L'estorsione ha un limite di recupero del mana)";

        DESkillDesc = $"Entzieht allen feindlichen Einheiten <color=yellow>{manaExploitationAmount}</color> Mana und stellt allen verbündeten Einheiten <color=yellow>{manaExploitationAmount}</color> Mana wieder her.\n" +
                          $"Erhöht die eigene Angriffskraft um <color=green>{incAttackPer}%</color> und die Angriffsgeschwindigkeit um <color=green>{incAttackSpeedPer}%</color> für {duration} Sekunden.\n" +
                          $"(Erpressung hat ein Mana-Wiederherstellungslimit)";

        ESSkillDesc = $"Absorbe <color=yellow>{manaExploitationAmount}</color> de maná de todas las unidades enemigas y restaura <color=yellow>{manaExploitationAmount}</color> de maná a todas las unidades aliadas.\n" +
                       $"Aumenta su poder de ataque en <color=green>{incAttackPer}%</color> y su velocidad de ataque en <color=green>{incAttackSpeedPer}%</color> durante {duration} segundos.\n" +
                       $"(La extorsión tiene un límite de recuperación de maná)";

        JASkillDesc = $"すべての敵ユニットからマナを<color=yellow>{manaExploitationAmount}</color>吸収し、すべての味方ユニットにマナを<color=yellow>{manaExploitationAmount}</color>回復します。\n" +
                       $"自身の攻撃力が<color=green>{incAttackPer}%</color>、攻撃速度が<color=green>{incAttackSpeedPer}%</color>、{duration}秒間増加します。\n" +
                       $"（搾取はマナ回復制限があります）";

        PT_BRSkillDesc = $"Absorve <color=yellow>{manaExploitationAmount}</color> de mana de todas as unidades inimigas e restaura <color=yellow>{manaExploitationAmount}</color> de mana para todas as unidades aliadas.\n" +
                          $"Aumenta o poder de ataque em <color=green>{incAttackPer}%</color> e a velocidade de ataque em <color=green>{incAttackSpeedPer}%</color> por {duration} segundos.\n" +
                          $"(Extorsão tem limite de recuperação de mana)";

        RUSkillDesc = $"Поглощает <color=yellow>{manaExploitationAmount}</color> маны у всех вражеских юнитов и восстанавливает <color=yellow>{manaExploitationAmount}</color> маны всем союзным юнитам.\n" +
                          $"Увеличивает собственную силу атаки на <color=green>{incAttackPer}%</color> и скорость атаки на <color=green>{incAttackSpeedPer}%</color> на {duration} секунд.\n" +
                          $"(Вымогательство имеет ограничение на восстановление маны)";

        ZH_HANSSkillDesc = $"从所有敌方单位吸取<color=yellow>{manaExploitationAmount}</color>点法力，并为所有友方单位恢复<color=yellow>{manaExploitationAmount}</color>点法力。\n" +
                           $"自身的攻击力提高<color=green>{incAttackPer}%</color>，攻击速度提高<color=green>{incAttackSpeedPer}%</color>，持续{duration}秒。\n" +
                           $"（勒索有法力恢复限制）";
    }
}