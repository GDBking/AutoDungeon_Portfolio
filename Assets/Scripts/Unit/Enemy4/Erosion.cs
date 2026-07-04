using System.Collections;
using UnityEngine;

public class Erosion : UnitDefault
{
    [Header("부식")]
    public float poisonDamage;
    public int count;

    readonly int amount = 30;
    readonly WaitForSeconds wait = new(0.1f);

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
        CameraShake.instance.Shake(2f, 2f);
        ErosionState();
        StartCoroutine(ErosionRain());
    }

    IEnumerator ErosionRain()
    {
        for (int i = 0; i < amount; i++) {
            Vector2 randPos = RandomPositionManager.instance.GetRandomBattleFieldPos();
            StartCoroutine(OnFire(null, randPos));

            yield return wait;
        }
    }

    void ErosionState()
    {
        foreach (UnitDefault enemy in friends)
            enemy.Poison(poisonDamage, count, this);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos;
        if (!isSkill)
            startPos = transform.position; // 시작 위치
        else
            startPos = targetPos + (Vector2.up * 5f);

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation, isSkill ? GameManager.instance.skillEffect : null);

        bulletList.Add(bullet);

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
        skillDesc = $"맵 전체에 부식 비가 내려 모든 적 유닛에게 <color=yellow>{count}초</color> 동안 독 효과를 입힙니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";

        ENSkillDesc = $"Summons an erosion rain across the map that applies poison to all enemy units for <color=yellow>{count} seconds</color>.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"Invoque une pluie de corrosion sur toute la carte qui applique du poison à toutes les unités ennemies pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dommages de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Evoca una pioggia di corrosione su tutta la mappa che applica veleno a tutte le unità nemiche per <color=yellow>{count} secondi</color>.\n" +
                      $"Danno da veleno per secondo: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Beschwört einen Verfallsregen über der ganzen Karte, der allen feindlichen Einheiten für <color=yellow>{count} Sekunden</color> Gift zufügt.\n" +
                      $"Giftschaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Invoca una lluvia de corrosión en todo el mapa que aplica veneno a todas las unidades enemigas durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"マップ全体に腐食の雨を降らせ、すべての敵ユニットに<color=yellow>{count}秒</color>の間毒を付与します。\n" +
                      $"毎秒の毒ダメージ：<color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Invoca uma chuva de erosão por todo o mapa que aplica veneno a todas as unidades inimigas por <color=yellow>{count} segundos</color>.\n" +
                          $"Dano de veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"Вызывает дождь эрозии по всей карте, накладывающий яд на все вражеские юниты на <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от яда в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"在整个地图上召唤腐蚀之雨，所有敌方单位施加<color=yellow>{count}秒</color>的中毒效果。\n" +
                           $"每秒中毒伤害：<color=red>{poisonDamage}</color>";
    }
}
