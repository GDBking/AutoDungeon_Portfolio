using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BlackKnight : UnitDefault
{
    [Header("처형")]
    public float executionPer;

    float ExecutionPer { get => Mathf.Min(executionPer + StaticManager.skillPoint[unitIdx] * 1.5f, 100f); }

    UnitDefault unitComp;

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        CreateAttackBox(unitComp.MaxHealth, SkillTarget, SkillTarget.transform.position, isPenetration: true);
    }

    protected override IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = AttackTarget.transform.position;

        unitComp = AttackTarget.GetComponent<UnitDefault>();
        if (unitComp.Health / unitComp.MaxHealth < ExecutionPer / 100f) {
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

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 남은 체력이 <color=red>{ExecutionPer}%</color> 미만일 경우 처형(즉사)시킵니다.";
        
        ENSkillDesc = $"If the remaining health of the target being attacked is below <color=red>{ExecutionPer}%</color>, execute (instant kill) them.";

        FRSkillDesc = $"Si la santé restante de la cible attaquée est inférieure à <color=red>{ExecutionPer}%</color>, exécutez-la (tuez-la instantanément).";

        ITSkillDesc = $"Se la salute rimanente del bersaglio attaccato è inferiore a <color=red>{ExecutionPer}%</color>, eseguilo (uccidilo istantaneamente).";

        DESkillDesc = $"Wenn die verbleibende Gesundheit des angegriffenen Ziels unter <color=red>{ExecutionPer}%</color> liegt, führe es aus (sofort töten).";

        ESSkillDesc = $"Si la salud restante del objetivo atacado está por debajo de <color=red>{ExecutionPer}%</color>, ejecútalo (mátalo al instante).";

        JASkillDesc = $"攻撃中の対象の残り体力が<color=red>{ExecutionPer}%</color>未満の場合、処刑（即死）します。";

        PT_BRSkillDesc = $"Se a saúde restante do alvo atacado estiver abaixo de <color=red>{ExecutionPer}%</color>, execute-o (mate-o instantaneamente).";

        RUSkillDesc = $"Если оставшееся здоровье атакуемой цели ниже <color=red>{ExecutionPer}%</color>, казните его (моментально убейте).";

        ZH_HANSSkillDesc = $"如果被攻击目标的剩余生命值低于<color=red>{ExecutionPer}%</color>，则处决（瞬间杀死）他们。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "처형 비율 <color=red>+1.5%p</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Execution rate <color=red>+1.5%p</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Taux d'exécution <color=red>+1.5%p</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Percentuale di esecuzione <color=red>+1.5%p</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Exekutionsrate <color=red>+1.5%p</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Tasa de ejecución <color=red>+1.5%p</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "処刑率 <color=red>+1.5%p</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Taxa de execução <color=red>+1.5%p</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Шанс казни <color=red>+1.5%p</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "处决概率 <color=red>+1.5%p</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Execution rate <color=red>+1.5%p</color>";
    }
}