using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Hunter : UnitDefault
{
    [Header("현상 수배")]
    public float attackCoeff;
    public int count;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.15f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 4; }

    List<GameObject> wantedList = new();

    protected override void Start()
    {
        base.Start();

        // 현상 수배 목록 랜덤 생성
        wantedList = enemies.Select(enemy => enemy.gameObject).ToList();
        while (wantedList.Count > Count)
            wantedList.RemoveAt(Random.Range(0, wantedList.Count));

        foreach (GameObject enemy in wantedList)
            enemy.GetComponent<UnitDefault>().SetStateBar(State.wantedList, -2f);
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * AttackCoeff, SkillTarget, SkillTarget.transform.position);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;

        if (wantedList.Contains(AttackTarget)) {
            SkillTarget = AttackTarget;
            UseSkill();
            yield break;
        }

        isAttackAnim = true;
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Attack");
        yield return null;
        // 공격 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);

        if (!isStunAnim && !isDeath && !isBondage && !isTaunt && Random.Range(1, 101) <= 40) {
            isMovingAnim = true;
            StartCoroutine(OnMovingAnim()); // 무빙 애니메이션 실행
        }
        isAttackAnim = false;
    }

    protected override IEnumerator OnDeathAnim()
    {
        foreach (GameObject unit in wantedList) {
            if (unit != null) {
                UnitDefault unitComp = unit.GetComponent<UnitDefault>();
                Destroy(unitComp.uiStateBar.Find("wantedList").gameObject);
            }
        }

        return base.OnDeathAnim();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"스폰 시 무작위 <color=yellow>{Count}명</color>의 적 유닛에게 현상수배를 걸고 해당 유닛을 공격 시 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";

        ENSkillDesc = $"Upon spawning, places a bounty on <color=yellow>{Count} random</color> enemy units. When attacking those units, deals damage equal to <color=red>{AttackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Lors de l'apparition, place une prime sur <color=yellow>{Count} unités ennemies aléatoires</color>. Lors de l'attaque de ces unités, inflige des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Al momento della comparsa, pone una taglia su <color=yellow>{Count} unità nemiche casuali</color>. Quando si attacca quelle unità, infligge danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Beim Spawnen wird auf <color=yellow>{Count} zufällige</color> feindliche Einheiten eine Belohnung ausgesetzt. Beim Angriff auf diese Einheiten werden Schäden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft verursacht.";

        ESSkillDesc = $"Al aparecer, coloca una recompensa sobre <color=yellow>{Count} unidades enemigas aleatorias</color>. Al atacar a esas unidades, inflige daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"スポーン時にランダムな<color=yellow>{Count}体</color>の敵ユニットに現金強奪をかけ、そのユニットを攻撃する際に攻撃力<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Ao surgir, coloca uma recompensa em <color=yellow>{Count} unidades inimigas aleatórias</color>. Ao atacar essas unidades, causa dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"При появлении накладывает награду на <color=yellow>{Count} случайных</color> вражеских юнитов. При атаке этих юнитов наносит урон, равный <color=red>{AttackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"生成时，对<color=yellow>{Count}个随机</color>敌方单位下达通缉令。攻击这些单位时，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "현상수배 수 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Bounty count <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Nombre de primes <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Conteggio taglie <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Kopfgeldanzahl <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Cantidad de recompensas <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "指名手配数 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Contagem de recompensas <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Количество наград <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.15</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "赏金数量 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.15</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Bounty count <color=yellow>+1</color>";
    }
}