using UnityEngine;

public class Calamity : UnitDefault
{
    [Header("참화")]
    public float attackCoeff;
    public float poisonDamage;
    public int count;
    public MapPoisonFog poisonFog;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        MapPoisonFog poisonFogObj = Instantiate(poisonFog, GameManager.instance.skillEffect);
        poisonFogObj.unitComp = this;

        for (int i = friends.Count - 1; i >= 0; i--) {
            CreateAttackBox(AttackPower * attackCoeff, friends[i].gameObject, friends[i].transform.position);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"맵 전체에 독가스를 살포하여 모든 적 유닛에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{count}초</color> 동안 독 효과를 입힙니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";
        
        ENSkillDesc = $"Releases poisonous gas over the entire map, dealing <color=red>{attackCoeff}x</color> attack damage to all enemy units and inflicting poison for <color=yellow>{count} seconds</color>.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"Libère un gaz toxique sur toute la carte, infligeant des dégâts d'attaque de <color=red>{attackCoeff}x</color> à toutes les unités ennemies et infligeant du poison pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Rilascia gas velenoso su tutta la mappa, infliggendo danni d'attacco di <color=red>{attackCoeff}x</color> a tutte le unità nemiche e infliggendo veleno per <color=yellow>{count} secondi</color>.\n" +
                      $"Danni da veleno al secondo: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Setzt die gesamte Karte mit giftigem Gas frei, das allen feindlichen Einheiten <color=red>{attackCoeff}x</color> Angriffsschaden zufügt und für <color=yellow>{count} Sekunden</color> Gift verursacht.\n" +
                      $"Gift-Schaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Libera gas venenoso en todo el mapa, infligiendo <color=red>{attackCoeff}x</color> de daño de ataque a todas las unidades enemigas e infligiendo veneno durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"マップ全体に毒ガスを撒き散らし、すべての敵ユニットに攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{count}秒</color>間、毒効果を与えます。\n" +
                      $"秒間毒ダメージ: <color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Libera gás venenoso por todo o mapa, causando <color=red>{attackCoeff}x</color> de dano de ataque a todas as unidades inimigas e infligindo veneno por <color=yellow>{count} segundos</color>.\n" +
                         $"Dano por veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"Выпускает ядовитый газ по всей карте, нанося всем вражеским юнитам <color=red>{attackCoeff}x</color> урона от атаки и нанося яд на <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от яда в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"在整个地图上释放有毒气体，对所有敌方单位造成<color=red>{attackCoeff}倍</color>的攻击伤害，并在<color=yellow>{count}秒</color>内施加中毒效果。\n" +
                           $"每秒中毒伤害：<color=red>{poisonDamage}</color>";
    }
}