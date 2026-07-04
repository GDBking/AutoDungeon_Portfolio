using System.Collections;
using UnityEngine;

public class Starlight : UnitDefault
{
    [Header("별광")]
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

    Coroutine co;
    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
        co = StartCoroutine(CreateSkillAttackBox(AttackPower * attackCoeff, pos));
    }

    GameObject effect;
    IEnumerator CreateSkillAttackBox(float attackPower, Vector2 targetPos)
    {
        Vector2 thisPos = transform.position;

        // 타겟 방향 계산
        Vector2 dir = (targetPos - thisPos).normalized;

        // 회전 방향 계산
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // 공격 이펙트 오브젝트 생성
        effect = Instantiate(attackObj, thisPos, rotation);
        bulletList.Add(effect);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);
        effect.transform.localScale = new Vector2(size, 2f);

        for (int i = 0; i < 3; i++) {
            // 박스 캐스트 수행
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(thisPos + dir * effect.transform.localScale.x / 2f, effect.transform.localScale, angle, LayerMask.GetMask(enemyTag));

            float damage = 0f;

            // 범위 내 적이 있을 경우 처리
            if (hitEnemies.Length > 0) {
                for (int j = hitEnemies.Length - 1; j >= 0; j--) {
                    UnitDefault unitComp = hitEnemies[j].GetComponent<UnitDefault>();
                    damage += unitComp.Hit(attackPower, CriticalPer, this);
                }

                // 피흡 계산
                float lifeSteal = damage * LifeStealPer / 100f;
                Healing(lifeSteal);

                DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
            }

            yield return new WaitForSeconds(1f);
        }

        co = null;
    }

    protected override IEnumerator OnSkillAnim()
    {
        isSkillAnim = true;
        SkillUseAfterState(); // 마나, isSkillAvailable 등 초기화
        animator.speed = 1f;
        animator.SetTrigger("2_Skill");
        yield return null;
        // 스킬 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        isSkillAnim = false;
        if (!isStunAnim && !isDeath && !isBondage && isKiting && Random.Range(1, 101) <= 40) {
            isKitingAnim = true;
            StartCoroutine(OnKitingAnim()); // 카이팅 실행
        }
    }

    public override void Death()
    {
        // 사망 시 스킬 효과 제거
        Destroy(effect);

        if (co != null)
            StopCoroutine(co);

        base.Death();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 방향으로 <color=#AEEAFF>{size}X2</color>크기의 레이저를 <color=yellow>3초</color> 동안 발사해 초당 공격력 <color=red>{attackCoeff}배</color>의 피해를 입힙니다.";
    
        ENSkillDesc = $"Fires a laser of size <color=#AEEAFF>{size}X2</color> in the attack direction for <color=yellow>3 seconds</color>, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power per second.";

        FRSkillDesc = $"Tire un laser de taille <color=#AEEAFF>{size}X2</color> dans la direction de l'attaque pendant <color=yellow>3 secondes</color>, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque par seconde.";

        ITSkillDesc = $"Spara un laser di dimensioni <color=#AEEAFF>{size}X2</color> nella direzione dell'attacco per <color=yellow>3 secondi</color>, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco al secondo.";

        DESkillDesc = $"Feuert einen Laser der Größe <color=#AEEAFF>{size}X2</color> in Angriffsrichtung für <color=yellow>3 Sekunden</color> ab und verursacht pro Sekunde Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Dispara un láser de tamaño <color=#AEEAFF>{size}X2</color> en la dirección del ataque durante <color=yellow>3 segundos</color>, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque por segundo.";

        JASkillDesc = $"攻撃方向に<color=#AEEAFF>{size}X2</color>のレーザーを<color=yellow>3秒間</color>発射し、毎秒攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Dispara um laser de tamanho <color=#AEEAFF>{size}X2</color> na direção do ataque por <color=yellow>3 segundos</color>, causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque por segundo.";

        RUSkillDesc = $"Выпускает лазер размером <color=#AEEAFF>{size}X2</color> в направлении атаки на <color=yellow>3 секунды</color>, нанося урон, равный <color=red>{attackCoeff} раза</color> силе атаки в секунду.";

        ZH_HANSSkillDesc = $"以攻击方向发射<color=#AEEAFF>{size}X2</color>大小的激光，持续<color=yellow>3秒</color>，每秒造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害。";
    }

}