using UnityEngine;

public class Daniel : UnitDefault
{
    [Header("부정")]
    public float attackCoeff;
    public float duration;

    protected override void Awake()
    {
        base.Awake();

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
        if (friends.Count == 0)
            return;

        SoundPlay(skillSoundClip);
        for (int i = friends.Count - 1; i >= 0; i--) {
            friends[i].AntiHeal(duration);
            CreateAttackBox(AttackPower * attackCoeff, friends[i].gameObject, friends[i].transform.position);
        }
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
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
        GameObject effect;
        if (!isSkill)
            effect = Instantiate(attackObj, targetPos, rotation);
        else
            effect = Instantiate(attackObj, target.transform);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
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
            if (attackStyle == AttackStyle.single) {
                foreach (Collider2D enemy in hitEnemies) {
                    if (enemy.gameObject == target) {
                        damage = target.GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
                        break;
                    }
                }
            }
            else {
                for (int i = hitEnemies.Length - 1; i >= 0; i--)
                    damage += hitEnemies[i].GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
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
        skillDesc = $"모든 적에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고 <color=yellow>{duration}초</color> 동안 치유 방해 저주를 겁니다.";

        ENSkillDesc = $"Deals damage equal to <color=red>{attackCoeff} times</color> the attack power to all enemies and applies an anti-heal curse for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Inflige des degats egaux a <color=red>{attackCoeff} fois</color> la puissance d'attaque a tous les ennemis et applique une malediction d'empechement de soin pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Infligge danno pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco a tutti i nemici e applica una maledizione che impedisce la guarigione per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Verursacht Schaden in Hohe von <color=red>{attackCoeff} mal</color> der Angriffskraft an allen Gegnern und verhangt einen Heilungsverhinderungsfluch fur <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Inflige dano igual a <color=red>{attackCoeff} veces</color> el poder de ataque a todos los enemigos y aplica una maldicion de anti-curacion durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"すべての敵に攻撃力<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{duration}秒</color>間、治癒妨害の呪いをかけます。";

        PT_BRSkillDesc = $"Inflige dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque a todos os inimigos e aplica uma maldicao de anti-curacion durante <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Наносит урон, равный <color=red>{attackCoeff} раза</color> силы атаки всем врагам и накладывает проклятие против исцеления на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"对所有敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并在<color=yellow>{duration}秒</color>内施加禁止治疗的诅咒。";
    }
}
