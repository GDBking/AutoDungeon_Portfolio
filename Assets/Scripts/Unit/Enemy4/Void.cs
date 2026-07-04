using System.Collections;
using UnityEngine;

public class Void : UnitDefault
{
    [Header("공허")]
    public float attackCoeff;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(3f, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
        StartCoroutine(OnFire(SkillTarget, pos, true, true));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position, isPenetration: true);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = transform.position; // 시작 위치
        if (isPenetration)
            targetPos = startPos + (targetPos - startPos).normalized * 3f;

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
        if (isSkill)
            bullet.tag = "Skill Bullet";
        if (isPenetration)
            bullet.tag = "Penetration Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(isSkill ? skillBulletAnimClip : bulletAnimClip, attackSize);
        bulletComp.unitDefault = this;
        bulletComp.target = target;
        if (isPenetration)
            bulletComp.enemyTag = enemyTag;

        if (!isSkill)
            SoundPlay(attackSoundClip);
        else
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
        skillDesc = $"유닛과 방어력을 관통하는 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히는 짧은 사거리를 가진 가시를 발사합니다.";

        ENSkillDesc = $"Fires short-range spikes that deal damage equal to <color=red>{attackCoeff} times</color> the attack and penetrate units and defense.";

        FRSkillDesc = $"Lance des épines à courte portée qui infligent des dégâts égaux à <color=red>{attackCoeff} fois</color> l'attaque et traversent les unités et la défense.";

        ITSkillDesc = $"Spara spine a corto raggio che infliggono danni pari a <color=red>{attackCoeff} volte</color> l'attacco e penetrano unità e difesa.";

        DESkillDesc = $"Feuert kurzreichweitige Stacheln ab, die Schaden in Höhe von <color=red>{attackCoeff} mal</color> des Angriffs verursachen und Einheiten sowie Verteidigung durchdringen.";

        ESSkillDesc = $"Dispara picos de corto alcance que infligen daño igual a <color=red>{attackCoeff} veces</color> el ataque y penetran unidades y defensa.";

        JASkillDesc = $"ユニットと防御力を貫通する攻撃力<color=red>{attackCoeff}倍</color>のダメージを与える短距離のトゲを発射します。";

        PT_BRSkillDesc = $"Dispara espinhos de curto alcance que causam dano igual a <color=red>{attackCoeff} vezes</color> o ataque e penetram unidades e defesa.";

        RUSkillDesc = $"Выпускает короткодействующие шипы, которые наносят урон, равный <color=red>{attackCoeff} раза</color> атаке, и проникают через юниты и защиту.";

        ZH_HANSSkillDesc = $"发射短程尖刺，造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，并穿透单位和防御。";
    }
}
