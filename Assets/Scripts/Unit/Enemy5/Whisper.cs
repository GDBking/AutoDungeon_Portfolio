using UnityEngine;

public class Whisper : UnitDefault
{
    [Header("밀담")]
    public float attackCoeff;
    public float incLifeStealPer;
    public float stunDuration;
    public AnimationClip whisperAnimClip;

    bool isWhisper;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        RandomEnemyUnit(friends);
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, transform.position, Quaternion.identity);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(whisperAnimClip);
        effect.transform.localScale *= attackSize;

        Vector2 targetPos = skillTargetPos;
        targetPos += transform.position.x < targetPos.x ? Vector2.right : Vector2.left;
        transform.position = targetPos;

        GameObject effect2 = Instantiate(attackObj, transform.position, Quaternion.identity);
        effect2.GetComponent<DestroyEffect>().SetAnimatorOverrideController(whisperAnimClip);
        effect2.transform.localScale *= attackSize;

        isWhisper = true;
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        if (!isWhisper)
            return base.CreateAttackBox(attackPower, target, targetPos, attackStyle, isSkill, isPenetration);
        else {
            isWhisper = false;

            target.GetComponent<UnitDefault>().OnStunAnim(stunDuration);

            float damage = base.CreateAttackBox(attackPower, target, targetPos, attackStyle, true, isPenetration);
            Healing(damage / 100f * incLifeStealPer);

            return damage;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"랜덤 적 유닛의 뒤로 순간이동합니다.\n" +
                    $"다음 공격은 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 피해량의 <color=green>{incLifeStealPer}%</color>를 흡혈하며 <color=yellow>{stunDuration}초</color> 동안 기절시킵니다.";
        
        ENSkillDesc = $"Teleports behind a random enemy unit.\n" +
                      $"The next attack deals damage equal to <color=red>{attackCoeff} times</color> the attack power, steals <color=green>{incLifeStealPer}%</color> of the damage dealt as life steal, and stuns the target for <color=yellow>{stunDuration} seconds</color>.";

        FRSkillDesc = $"Se téléporte derrière une unité ennemie aléatoire.\n" +
                        $"La prochaine attaque inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque, vole <color=green>{incLifeStealPer}%</color> des dégâts infligés en tant que vol de vie, et étourdit la cible pendant <color=yellow>{stunDuration} secondes</color>.";

        ITSkillDesc = $"Si teletrasporta dietro un'unità nemica casuale.\n" +
                        $"Il prossimo attacco infligge danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco, ruba <color=green>{incLifeStealPer}%</color> dei danni inflitti come furto di vita e stordisce il bersaglio per <color=yellow>{stunDuration} secondi</color>.";

        DESkillDesc = $"Teleportiert sich hinter eine zufällige feindliche Einheit.\n" +
                        $"Der nächste Angriff verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft, stiehlt <color=green>{incLifeStealPer}%</color> des zugefügten Schadens als Lebensraub und betäubt das Ziel für <color=yellow>{stunDuration} Sekunden</color>.";

        ESSkillDesc = $"Se teletransporta detrás de una unidad enemiga aleatoria.\n" +
                        $"El siguiente ataque inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque, roba <color=green>{incLifeStealPer}%</color> del daño infligido como robo de vida y aturde al objetivo durante <color=yellow>{stunDuration} segundos</color>.";

        JASkillDesc = $"ランダムな敵ユニットの後ろに瞬間移動します。\n" +
                        $"次の攻撃は攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、与えたダメージの<color=green>{incLifeStealPer}%</color>をライフスティールし、<color=yellow>{stunDuration}秒</color>間、対象をスタンさせます。";

        PT_BRSkillDesc = $"Teletransporta-se atrás de uma unidade inimiga aleatória.\n" +
                         $"O próximo ataque causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque, rouba <color=green>{incLifeStealPer}%</color> do dano causado como roubo de vida e atordoa o alvo por <color=yellow>{stunDuration} segundos</color>.";

        RUSkillDesc = $"Телепортируется за случайную вражескую единицу.\n" +
                        $"Следующая атака наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки, крадет <color=green>{incLifeStealPer}%</color> нанесенного урона в качестве кражи жизни и оглушает цель на <color=yellow>{stunDuration} секунд</color>.";

        ZH_HANSSkillDesc = $"传送到一个随机敌方单位的后面。\n" +
                           $"下一次攻击造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，吸取所造成伤害的<color=green>{incLifeStealPer}%</color>作为生命偷取，并使目标昏迷<color=yellow>{stunDuration}秒</color>。";
    }
}
