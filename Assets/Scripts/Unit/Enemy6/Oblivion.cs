using System.Collections;
using UnityEngine;

public class Oblivion : UnitDefault
{
    [Header("망각")]
    public float decSpeedPer;
    public float decAttackRangePer;
    public GameObject fog;

    static int fogCnt;
    static GameObject fogObj;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(OnSkillAnim());
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        if (fogCnt++ == 0) {
            fogObj = Instantiate(fog, GameManager.instance.skillEffect);
        }

        StartCoroutine(FogRoutine());
    }

    IEnumerator FogRoutine()
    {
        WaitForSeconds wait = new(1f);
        while (true) {
            foreach (UnitDefault enemy in friends) {
                enemy.SetStat(enemy.speedStat, decSpeedPer, true, false, 1f);
                enemy.SetStat(enemy.attackRangeStat, decAttackRangePer, true, false, 1f);
                enemy.SetStateBar(State.decSpeed, 1f);
                enemy.SetStateBar(State.decAttackRange, 1f);
            }

            yield return wait;
        }
    }

    protected override void OnDestroy()
    {
        if (--fogCnt == 0 && fogObj != null) {
            Destroy(fogObj);
        }

        base.OnDestroy();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"전투 시작 시 안개를 펼쳐 모든 적 유닛의 이동 속도를 <color=red>{decSpeedPer}%</color>, 사정거리를 <color=red>{decAttackRangePer}%</color> 감소시킵니다.";
        
        ENSkillDesc = $"At the start of combat, spreads fog that reduces the movement speed of all enemy units by <color=red>{decSpeedPer}%</color> and their attack range by <color=red>{decAttackRangePer}%</color>.";

        FRSkillDesc = $"Au début du combat, déploie un brouillard qui réduit la vitesse de déplacement de toutes les unités ennemies de <color=red>{decSpeedPer}%</color> et leur portée d'attaque de <color=red>{decAttackRangePer}%</color>.";

        ITSkillDesc = $"All'inizio del combattimento, diffonde una nebbia che riduce la velocità di movimento di tutte le unità nemiche del <color=red>{decSpeedPer}%</color> e la loro gittata di attacco del <color=red>{decAttackRangePer}%</color>.";

        DESkillDesc = $"Zu Beginn des Kampfes breitet sich Nebel aus, der die Bewegungsgeschwindigkeit aller feindlichen Einheiten um <color=red>{decSpeedPer}%</color> und ihre Angriffsreichweite um <color=red>{decAttackRangePer}%</color> verringert.";

        ESSkillDesc = $"Al comienzo del combate, se extiende una niebla que reduce la velocidad de movimiento de todas las unidades enemigas en <color=red>{decSpeedPer}%</color> y su alcance de ataque en <color=red>{decAttackRangePer}%</color>.";

        JASkillDesc = $"戦闘開始時に霧を広げ、すべての敵ユニットの移動速度を<color=red>{decSpeedPer}%</color>、攻撃範囲を<color=red>{decAttackRangePer}%</color>減少させます。";

        PT_BRSkillDesc = $"No início do combate, espalha névoa que reduz a velocidade de movimento de todas as unidades inimigas em <color=red>{decSpeedPer}%</color> e seu alcance de ataque em <color=red>{decAttackRangePer}%</color>.";

        RUSkillDesc = $"В начале боя распространяет туман, который уменьшает скорость передвижения всех вражеских юнитов на <color=red>{decSpeedPer}%</color> и их дальность атаки на <color=red>{decAttackRangePer}%</color>.";

        ZH_HANSSkillDesc = $"在战斗开始时，散布雾气，降低所有敌方单位的移动速度<color=red>{decSpeedPer}%</color>，攻击范围<color=red>{decAttackRangePer}%</color>。";
    }
}