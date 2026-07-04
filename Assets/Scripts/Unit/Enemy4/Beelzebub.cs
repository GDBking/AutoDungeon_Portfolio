using UnityEngine;

public class Beelzebub : UnitDefault
{
    [Header("패시브")]
    public float executionPer;
    public float incHealthPer;
    public AnimationClip executionAnimClip;
    public AnimationClip healAnimClip;
    [Header("심연의 포식")]
    public float attackHealthPer;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);

        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        CreateAttackBox(unitComp.MaxHealth / 100f * attackHealthPer, SkillTarget, SkillTarget.transform.position, isPenetration: true);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        bool isExecutionPer = false;
        UnitDefault enemyComp = target.GetComponent<UnitDefault>();
        if (enemyComp.Health / enemyComp.MaxHealth <= executionPer / 100f)
            isExecutionPer = true;

        Quaternion rotation = Quaternion.identity;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

        // 공격 타입에 맞는 클립 할당
        if (!isExecutionPer)
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        else
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(executionAnimClip);

        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1f : 1f;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize / 2f, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            foreach (Collider2D enemy in hitEnemies) {
                if (enemy.gameObject == target) {
                    if (isExecutionPer) {
                        damage = enemyComp.Hit(enemyComp.MaxHealth, CriticalPer, this, true);

                        GameObject healEffect = Instantiate(attackObj, transform);
                        healEffect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(healAnimClip);
                        Healing(MaxHealth / 100f * incHealthPer);
                    }
                    else
                        damage = enemyComp.Hit(attackPower, CriticalPer, this, isPenetration);
                    break;
                }
            }

            // 피흡 계산
            float lifeSteal = damage * LifeStealPer / 100f;
            Healing(lifeSteal);

            DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
        }
        return damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"패시브: 공격 중인 대상의 체력이 <color=red>{executionPer}%</color> 미만이면 대상을 처형하고, 본인의 체력을 <color=green>{incHealthPer}%</color> 회복합니다.\n\n" +
                    $"심연의 포식: 공격 중인 대상의 최대 체력의 <color=red>{attackHealthPer}%</color> 만큼 방어력 관통 피해를 입힙니다.";
        
        ENSkillDesc = $"Passive: If the health of the current target is below <color=red>{executionPer}%</color>, execute the target and heal this unit for <color=green>{incHealthPer}%</color> of max health.\n\n" +
                        $"Abyssal Devour: Deals defense-piercing damage equal to <color=red>{attackHealthPer}%</color> of the current target's max health.";

        FRSkillDesc = $"Passif : Si la santé de la cible actuelle est inférieure à <color=red>{executionPer}%</color>, exécutez la cible et soignez cette unité de <color=green>{incHealthPer}%</color> de la santé maximale.\n\n" +
                        $"Dévoration Abyssale : Inflige des dégâts perforants égaux à <color=red>{attackHealthPer}%</color> de la santé maximale de la cible actuelle.";

        ITSkillDesc = $"Passivo: Se la salute del bersaglio attuale è inferiore a <color=red>{executionPer}%</color>, esegui il bersaglio e cura questa unità per <color=green>{incHealthPer}%</color> della salute massima.\n\n" +
                        $"Divoramento Abissale: Infligge danni perforanti pari a <color=red>{attackHealthPer}%</color> della salute massima del bersaglio attuale.";

        DESkillDesc = $"Passiv: Wenn die Gesundheit des aktuellen Ziels unter <color=red>{executionPer}%</color> liegt, führe das Ziel aus und heile diese Einheit um <color=green>{incHealthPer}%</color> der maximalen Gesundheit.\n\n" +
                        $"Abgrundverschlingung: Verursacht durchdringenden Schaden in Höhe von <color=red>{attackHealthPer}%</color> der maximalen Gesundheit des aktuellen Ziels.";

        ESSkillDesc = $"Pasivo: Si la salud del objetivo actual está por debajo de <color=red>{executionPer}%</color>, ejecuta al objetivo y cura a esta unidad por <color=green>{incHealthPer}%</color> de la salud máxima.\n\n" +
                        $"Devoración Abisal: Inflige daño que atraviesa la defensa igual a <color=red>{attackHealthPer}%</color> de la salud máxima del objetivo actual.";

        JASkillDesc = $"パッシブ: 現在の対象の体力が<color=red>{executionPer}%</color>未満の場合、対象を処刑し、自身の体力を最大体力の<color=green>{incHealthPer}%</color>回復します。\n\n" +
                        $"深淵の捕食: 攻撃中の対象の最大体力の<color=red>{attackHealthPer}%</color>分の防御力貫通ダメージを与えます。";

        PT_BRSkillDesc = $"Passivo: Se a saúde do alvo atual estiver abaixo de <color=red>{executionPer}%</color>, execute o alvo e cure esta unidade por <color=green>{incHealthPer}%</color> da saúde máxima.\n\n" +
                        $"Devoramento Abissal: Causa dano que perfura a defesa igual a <color=red>{attackHealthPer}%</color> da saúde máxima do alvo atual.";

        RUSkillDesc = $"Пассив: Если здоровье текущей цели ниже <color=red>{executionPer}%</color>, казните цель и исцелите эту единицу на <color=green>{incHealthPer}%</color> от максимального здоровья.\n\n" +
                        $"Бездна Поглощения: Наносит пробивающий защиту урон, равный <color=red>{attackHealthPer}%</color> от максимального здоровья текущей цели.";

        ZH_HANSSkillDesc = $"被动: 如果当前目标的生命值低于<color=red>{executionPer}%</color>，则处决该目标并回复相当于最大生命<color=green>{incHealthPer}%</color>的生命。\n\n" +
                        $"深渊吞噬: 对攻击中的目标造成相当于其最大生命值<color=red>{attackHealthPer}%</color>的防御力穿透伤害。";

    }
}