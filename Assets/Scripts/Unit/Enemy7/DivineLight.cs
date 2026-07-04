using UnityEngine;

public class DivineLight : UnitDefault
{
    [Header("천광")]
    public float attackCoeff;
    public float size;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(size, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * attackCoeff, SkillTarget, pos, AttackStyle.range);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;
        Vector2 thisPos = Vector2.zero;
        Vector2 dir = Vector2.zero;
        float angle = 0f;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            thisPos = transform.position;

            // 타겟 방향 계산
            dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, isSkill ? thisPos : targetPos, rotation);

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
        if (!isSkill)
            scale *= attackSize; // 스프라이트 크기 조정
        else
            scale = new Vector2(size, 7f);
        effect.transform.localScale = scale;

        // 박스 캐스트 수행 (회전 적용)
        Collider2D[] hitEnemies;
        if (!isSkill)
            hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize, LayerMask.GetMask(enemyTag));
        else
            hitEnemies = Physics2D.OverlapBoxAll(thisPos + dir * effect.transform.localScale.x / 2f, new Vector2(size, 1f), angle, LayerMask.GetMask(enemyTag));

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
        skillDesc = $"공격 중인 대상에게 <color=#AEEAFF>{size}X1</color>크기의 일직선 레이저를 발사해 피격된 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Fires a straight laser beam of size <color=#AEEAFF>{size}X1</color> at the attacking target, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to hit enemies.";

        FRSkillDesc = $"Tire un faisceau laser droit de taille <color=#AEEAFF>{size}X1</color> sur la cible attaquée, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis touchés.";

        ITSkillDesc = $"Spara un raggio laser dritto di dimensioni <color=#AEEAFF>{size}X1</color> al bersaglio attaccante, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici colpiti.";

        DESkillDesc = $"Feuert einen geraden Laserstrahl der Größe <color=#AEEAFF>{size}X1</color> auf das angreifende Ziel und verursacht Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft bei getroffenen Feinden.";

        ESSkillDesc = $"Dispara un rayo láser recto de tamaño <color=#AEEAFF>{size}X1</color> al objetivo atacado, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos alcanzados.";

        JASkillDesc = $"攻撃中の対象に<color=#AEEAFF>{size}X1</color>の直線レーザーを発射し、命中した敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Dispara um feixe de laser reto de tamanho <color=#AEEAFF>{size}X1</color> no alvo atacante, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos atingidos.";

        RUSkillDesc = $"Выстреливает прямым лазерным лучом размером <color=#AEEAFF>{size}X1</color> в атакуемую цель, нанося урон, равный <color=red>{attackCoeff} раз</color> силе атаки пораженным врагам.";

        ZH_HANSSkillDesc = $"向被攻击目标发射<color=#AEEAFF>{size}X1</color>大小的直线激光，对命中的敌人造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }
}