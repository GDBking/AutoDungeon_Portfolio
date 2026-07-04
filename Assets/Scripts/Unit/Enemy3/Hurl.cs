using UnityEngine;

public class Hurl : UnitDefault
{
    [Header("투척")]
    public float burnDamage;
    public int count;
    public float stunDuration;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
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
        foreach (UnitDefault unitComp in friends)
            StartCoroutine(OnFire(unitComp.gameObject, unitComp.transform.position));

        OnStunAnim(stunDuration);
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(-1f, target, target.transform.position);

        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        unitComp.Burn(burnDamage, count, this);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"모든 적 유닛에게 파이어볼을 투척합니다.\n" +
                    $"피격된 적들은 <color=yellow>{count}초</color> 동안 화상 피해를 입습니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>\n" +
                    $"스킬 사용 후 본인은 <color=yellow>{stunDuration}초</color> 동안 기절합니다.";

        ENSkillDesc = $"Throws fireballs at all enemy units.\n" +
                      $"Hit enemies are burned for <color=yellow>{count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{burnDamage}</color>\n" +
                      $"After using the skill, this unit is stunned for <color=yellow>{stunDuration} seconds</color>.";

        FRSkillDesc = $"Lance des boules de feu sur toutes les unités ennemies.\n" +
                      $"Les ennemis touchés brûlent pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dommages de brûlure par seconde : <color=red>{burnDamage}</color>\n" +
                      $"Après avoir utilisé la compétence, cette unité est étourdie pendant <color=yellow>{stunDuration} secondes</color>.";

        ITSkillDesc = $"Lancia palle di fuoco su tutte le unità nemiche.\n" +
                      $"I nemici colpiti bruciano per <color=yellow>{count} secondi</color>.\n" +
                      $"Danno da bruciatura al secondo: <color=red>{burnDamage}</color>\n" +
                      $"Dopo aver usato l'abilità, questa unità è stordita per <color=yellow>{stunDuration} secondi</color>.";

        DESkillDesc = $"Wirft Feuerbälle auf alle feindlichen Einheiten.\n" +
                      $"Getroffene Gegner brennen für <color=yellow>{count} Sekunden</color>.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>\n" +
                      $"Nach der Verwendung der Fähigkeit ist diese Einheit <color=yellow>{stunDuration} Sekunden</color> lang betäubt.";

        ESSkillDesc = $"Lanza bolas de fuego a todas las unidades enemigas.\n" +
                      $"Los enemigos alcanzados arden durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>\n" +
                      $"Después de usar la habilidad, esta unidad queda aturdida durante <color=yellow>{stunDuration} segundos</color>.";

        JASkillDesc = $"すべての敵ユニットにファイアボールを投げます。\n" +
                      $"命中した敵は<color=yellow>{count}秒</color>間火傷状態になります。\n" +
                      $"火傷ダメージ（秒）：<color=red>{burnDamage}</color>\n" +
                      $"スキル使用後、このユニットは<color=yellow>{stunDuration}秒</color>間気絶します。";

        PT_BRSkillDesc = $"Lança bolas de fogo em todas as unidades inimigas.\n" +
                          $"Inimigos atingidos ficam queimados por <color=yellow>{count} segundos</color>.\n" +
                          $"Dano por queimação por segundo: <color=red>{burnDamage}</color>\n" +
                          $"Após usar a habilidade, esta unidade fica atordoada por <color=yellow>{stunDuration} segundos</color>.";

        RUSkillDesc = $"Бросает файерболы во все вражеские юниты.\n" +
                      $"Попавшие враги горят в течение <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от горения в секунду: <color=red>{burnDamage}</color>\n" +
                      $"После использования умения эта единица оглушается на <color=yellow>{stunDuration} секунд</color>.";

        ZH_HANSSkillDesc = $"向所有敌方单位投掷火球。\n" +
                           $"被命中的敌人在<color=yellow>{count}秒</color>内灼烧。\n" +
                           $"每秒灼烧伤害：<color=red>{burnDamage}</color>\n" +
                           $"使用技能后，该单位在<color=yellow>{stunDuration}秒</color>内被眩晕。";
    }
}