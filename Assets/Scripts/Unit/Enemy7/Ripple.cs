using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : UnitDefault
{
    readonly List<(float dist, UnitDefault enemy)> unitDist = new();

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower, target, target.transform.position);
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        base.OnShortAttack();

        unitDist.Clear();

        foreach (UnitDefault enemy in friends) {
            if (enemy.gameObject == attackTarget) continue;

            float dist = Vector2.Distance(attackTargetPos, enemy.transform.position);
            unitDist.Add((dist, enemy));
        }

        if (unitDist.Count == 0)
            return;

        // 거리 기준 오름차순 정렬
        unitDist.Sort((a, b) => a.dist.CompareTo(b.dist));

        SkillTarget = unitDist[0].enemy.gameObject;
        skillTargetPos = attackTargetPos;
        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = skillTargetPos; // 시작 위치

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation);
        bulletList.Add(bullet);

        bullet.tag = "Skill Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(skillBulletAnimClip, attackSize);
        bulletComp.unitDefault = this;
        bulletComp.target = target;

        SoundPlay(skillSoundClip);

        WaitForFixedUpdate wait = new();
        // 발사체가 타겟으로 이동하는 로직
        while (elapsedTime < duration) {
            if (bullet == null)
                yield break;

            // 경과 시간 업데이트
            elapsedTime += Time.fixedDeltaTime;

            // 현재 시간에 비례하여 이동할 거리 계산
            float t = elapsedTime / duration;

            // 발사체의 위치 계산 (Lerp 사용)
            bullet.transform.position = Vector2.Lerp(startPos, targetPos, t);

            // 다음 프레임 대기
            yield return wait;
        }

        if (bullet != null) {
            // 마지막 위치를 타겟의 위치로 설정 (정확한 위치 보정)
            bullet.transform.position = targetPos;

            // 발사체 제거
            Destroy(bullet);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"기본 공격 시 공격 중인 대상과 가장 가까운 적 유닛도 같은 피해를 입습니다.";
        
        ENSkillDesc = $"When performing a basic attack, the nearest enemy unit to the target being attacked also takes the same damage.";

        FRSkillDesc = $"Lors d'une attaque de base, l'unité ennemie la plus proche de la cible attaquée subit également les mêmes dégâts.";

        ITSkillDesc = $"Durante un attacco base, l'unità nemica più vicina al bersaglio attaccato subisce lo stesso danno.";

        DESkillDesc = $"Bei einem Grundangriff erleidet die dem angegriffenen Ziel am nächsten gelegene feindliche Einheit ebenfalls den gleichen Schaden.";

        ESSkillDesc = $"Al realizar un ataque básico, la unidad enemiga más cercana al objetivo atacado también recibe el mismo daño.";

        JASkillDesc = $"通常攻撃時、攻撃中の対象に最も近い敵ユニットも同じダメージを受けます。";

        PT_BRSkillDesc = $"Ao realizar um ataque básico, a unidade inimiga mais próxima do alvo atacado também sofre o mesmo dano.";

        RUSkillDesc = $"При выполнении базовой атаки ближайший вражеский юнит к атакуемой цели также получает такой же урон.";

        ZH_HANSSkillDesc = $"在进行基础攻击时，攻击目标最近的敌方单位也会受到相同的伤害。";
    }
}
