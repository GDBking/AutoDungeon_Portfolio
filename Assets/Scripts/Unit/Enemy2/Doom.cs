using System.Collections.Generic;
using UnityEngine;

public class Doom : UnitDefault
{
    [Header("파멸")]
    public float burnDamage;
    public int count;

    [HideInInspector]
    public List<UnitDefault> doomList = new();

    int curEnemyCnt;

    protected override void Awake()
    {
        base.Awake();
        onSkillFireAction = OnSkillFire;
        isKiting = true;

        isSkillAvaliable = true;
    }

    private void Update()
    {
        if (curEnemyCnt != friends.Count) {
            curEnemyCnt = friends.Count;
            foreach (UnitDefault unitComp in friends) {
                if (unitComp.onSkillAction.GetInvocationList().Length == 1)
                    unitComp.onSkillAction += unitComp.AddDoomList;
            }
        }
    }

    protected override void UseSkill()
    {
        doomList.RemoveAll(enemy => enemy == null || enemy.isDeath);

        if (doomList.Count == 0)
            return;

        SkillTarget = doomList[0].gameObject;
        doomList.RemoveAt(0);
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        CreateAttackBox(-1f, null, target.transform.position);
        unitComp.Burn(burnDamage, count, this, dealMetricsIdx);
    }

    protected override void SkillUseAfterState()
    {
        
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"스킬을 사용한 적 유닛에게 <color=yellow>{count}초</color> 동안 화상을 입히는 검은 불을 발사합니다.\n" +
                    $"화상 데미지: <color=red>{burnDamage}</color>";
        
        ENSkillDesc = $"Fires a black flame at an enemy unit that used a skill, burning them for <color=yellow>{count} seconds</color>.\n" +
                       $"Burn damage: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Tire une flamme noire sur une unité ennemie qui a utilisé une compétence, la brûlant pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dommages de brûlure : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Lancia una fiamma nera su un'unità nemica che ha usato un'abilità, bruciandola per <color=yellow>{count} secondi</color>.\n" +
                      $"Danno da bruciatura: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Schießt eine schwarze Flamme auf eine feindliche Einheit, die eine Fertigkeit eingesetzt hat, und verbrennt sie für <color=yellow>{count} Sekunden</color>.\n" +
                      $"Brandschaden: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Lanza una llama negra sobre una unidad enemiga que usó una habilidad, quemándola durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura: <color=red>{burnDamage}</color>";

        JASkillDesc = $"スキルを使用した敵ユニットに黒い炎を放ち、<color=yellow>{count}秒</color>間火傷状態にします。\n" +
                      $"火傷ダメージ：<color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Lanza una llama negra a una unidad enemiga que usó una habilidad, quemándola durante <color=yellow>{count} segundos</color>.\n" +
                          $"Daño por quemadura: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Выпускает черное пламя по вражеской единице, использовавшей умение, поджигая её на <color=yellow>{count} секунд</color>.\n" +
                       $"Урон от горения: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"向使用了技能的敌方单位射黑色火焰，使其在<color=yellow>{count}秒</color>内灼烧。\n" +
                          $"灼烧伤害：<color=red>{burnDamage}</color>";
    }
}
