using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feast : UnitDefault
{
    [Header("만찬")]
    public float incHealthPer;

    List<UnitDefault> enemyUnits;
    List<UnitDefault> friendlyUnits;
    Vector2 deathUnitPos;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void Start()
    {
        base.Start();

        enemyUnits = new(friends);
        friendlyUnits = new(enemies);
    }

    private void Update()
    {
        if (isDeath)
            return;

        if (enemyUnits.Count != friends.Count) {
            if (friends.Count < enemyUnits.Count) {
                foreach (UnitDefault enemy in enemyUnits) {
                    if (enemy.isDeath) {
                        deathUnitPos = enemy.transform.position;
                        UseSkill();
                        break;
                    }
                }
            }
            enemyUnits = new(friends);
        }

        if (friendlyUnits.Count != enemies.Count) {
            if (enemies.Count < friendlyUnits.Count) {
                foreach (UnitDefault friendly in friendlyUnits) {
                    if (friendly.isDeath) {
                        deathUnitPos = friendly.transform.position;
                        UseSkill();
                        break;
                    }
                }
            }
            friendlyUnits = new(enemies);
        }
    }

    protected override void UseSkill()
    {
        if (Health == MaxHealth)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        StartCoroutine(OnFire(gameObject, transform.position));
    } 

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(-1f, null, target.transform.position);
        Healing(MaxHealth / 100f * incHealthPer);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = deathUnitPos;

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
        skillDesc = $"적군 또는 아군 유닛이 사망 시 영혼을 흡수하여 최대 체력의 <color=green>{incHealthPer}%</color>를 회복합니다.";

        ENSkillDesc = $"Absorbs the soul of a fallen enemy or ally to heal for <color=green>{incHealthPer}%</color> of max health when a unit dies.";

        FRSkillDesc = $"Absorbe l'âme d'une unité alliée ou ennemie tombée pour se soigner de <color=green>{incHealthPer}%</color> de la santé maximale lorsqu'une unité meurt.";

        ITSkillDesc = $"Assorbe l'anima di un'unità alleata o nemica caduta per curarsi del <color=green>{incHealthPer}%</color> della salute massima quando un'unità muore.";

        DESkillDesc = $"Absorbiert die Seele einer gefallenen verbündeten oder feindlichen Einheit, um beim Tod einer Einheit <color=green>{incHealthPer}%</color> der maximalen Gesundheit zu heilen.";

        ESSkillDesc = $"Absorbe el alma de una unidad aliada o enemiga caída para curar <color=green>{incHealthPer}%</color> de la salud máxima cuando una unidad muere.";

        JASkillDesc = $"敵または味方のユニットが死亡した際に魂を吸収し、最大体力の<color=green>{incHealthPer}%</color>を回復します。";

        PT_BRSkillDesc = $"Absorve a alma de uma unidade aliada ou inimiga caída para curar <color=green>{incHealthPer}%</color> da vida máxima quando uma unidade morre.";

        RUSkillDesc = $"Поглощает душу павшей вражеской или союзной единицы, исцеляясь на <color=green>{incHealthPer}%</color> от максимального здоровья при смерти юнита.";

        ZH_HANSSkillDesc = $"当敌方或友方单位死亡时，吸收其灵魂，恢复最大生命值的<color=green>{incHealthPer}%</color>。";
    }
}
