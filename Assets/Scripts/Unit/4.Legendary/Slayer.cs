using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Slayer : UnitDefault
{
    [Header("집념")]
    public float attackCoeff;
    public int count;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 4; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        SoundPlay(skillSoundClip);
        StartCoroutine(FanShotToTarget(pos, Count, 55f, 3f));
    }

    void OnSkillFire(GameObject target)
    {
        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
    }

    public virtual IEnumerator FanShotToTarget(Vector2 targetPos, int bulletCount, float fanAngle, float splitDistance)
    {
        // 시작 위치와 타겟 위치 계산
        Vector2 startPos = transform.position;

        // 타겟을 향한 방향 계산
        Vector2 toTarget = (targetPos - startPos).normalized;

        // 기본 방향 각도 (라디안 -> 도) 계산
        float baseAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        // 탄환 간 각도 차이 계산 (탄환 1개일 경우 0)
        float angleStep = fanAngle / (bulletCount - 1);

        // 부채꼴의 시작 각도 (왼쪽 끝)
        float startAngle = baseAngle - fanAngle / 2f;

        // 생성된 탄환 오브젝트들 & 1차 목표 위치 저장용
        List<GameObject> bullets = new();
        List<Vector2> splitTargets = new();

        // [1단계] 부채꼴 형태로 탄환 발사
        for (int i = 0; i < bulletCount; i++) {
            // 현재 각도 계산
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // 방향 벡터 계산
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            // 1차 도착 지점 (퍼지는 위치)
            Vector2 splitTarget = startPos + dir * splitDistance;

            // 탄환의 회전 방향 설정
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // 탄환 생성 및 초기화
            GameObject bullet = Instantiate(bulletObj, startPos, rotation);
            bullet.tag = "Penetration Bullet";

            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.SetAnimatorOverrideController(skillBulletAnimClip, attackSize);
            bulletComp.unitDefault = this;
            bulletComp.enemyTag = enemyTag;

            // 리스트에 저장
            bulletList.Add(bullet);
            bullets.Add(bullet);
            splitTargets.Add(splitTarget);
        }

        // 탄환이 splitDistance만큼 퍼지는 데 걸리는 시간 계산
        float duration1 = splitDistance / ShotSpeed;
        float elapsedTime1 = 0f;
        WaitForFixedUpdate wait = new(); // 고정 프레임 간격 기다림 (물리적 이동 자연스러움)

        // [1단계 애니메이션] 부채꼴 방향으로 탄환이 흩어지게 이동
        while (elapsedTime1 < duration1) {
            elapsedTime1 += Time.fixedDeltaTime;
            float t = elapsedTime1 / duration1;

            // 각 탄환을 splitTarget 위치까지 선형 보간
            for (int i = 0; i < bullets.Count; i++) {
                if (bullets[i] != null)
                    bullets[i].transform.position = Vector2.Lerp(startPos, splitTargets[i], t);
            }

            yield return wait;
        }

        // [2단계 준비] split 위치에서 다시 타겟으로 이동
        RandomEnemyUnit(enemies);
        if (SkillTarget == null) {
            for (int i = 0; i < bullets.Count; i++) {
                if (bullets[i] != null) {
                    Destroy(bullets[i]);
                }
            }
            yield break;
        }

        Vector2 capturePos = skillTargetPos;

        float distance2 = Vector2.Distance(splitTargets[0], capturePos); // 한 발 기준 거리
        float duration2 = distance2 / ShotSpeed;
        float elapsedTime2 = 0f;

        // [2단계 애니메이션] split 위치 → targetPosition 으로 모여들게 이동
        while (elapsedTime2 < duration2) {
            elapsedTime2 += Time.fixedDeltaTime;
            float t = elapsedTime2 / duration2;

            for (int i = 0; i < bullets.Count; i++) {
                if (bullets[i] != null)
                    bullets[i].transform.position = Vector2.Lerp(splitTargets[i], capturePos, t);
            }

            yield return wait;
        }

        // 모든 탄환을 타겟 위치에 도달시킨 후 파괴
        for (int i = 0; i < bullets.Count; i++) {
            if (bullets[i] != null) {
                bullets[i].transform.position = capturePos;
                Destroy(bullets[i]);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"유닛을 관통하는 표창 <color=yellow>{Count}개</color>를 부채꼴 형태로 던진 후 무작위 적 유닛 한 명에게 표창이 모입니다.\n" +
                    $"각 표창은 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Throws <color=yellow>{Count}</color> throwing stars in a fan shape that pierce through units, then converge on a random enemy unit.\n" +
                      $"Each throwing star deals damage equal to <color=red>{AttackCoeff} times</color> the attack power.";

        FRSkillDesc = $"Lance <color=yellow>{Count}</color> étoiles de lancer en forme d'éventail qui traversent les unités, puis convergent vers une unité ennemie aléatoire.\n" +
                      $"Chaque étoile de lancer inflige des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque.";

        ITSkillDesc = $"Lancia <color=yellow>{Count}</color> stelle da lancio a forma di ventaglio che trapassano le unità, quindi convergono su un'unità nemica casuale.\n" +
                      $"Ogni stella da lancio infligge danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco.";

        DESkillDesc = $"Wirft <color=yellow>{Count}</color> Wurfsterne in Fächerform, die Einheiten durchdringen, und konvergiert dann auf eine zufällige feindliche Einheit.\n" +
                      $"Jeder Wurfstern verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft.";

        ESSkillDesc = $"Lanza <color=yellow>{Count}</color> estrellas arrojadizas en forma de abanico que atraviesan las unidades y luego convergen en una unidad enemiga aleatoria.\n" +
                      $"Cada estrella arrojadiza inflige daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque.";

        JASkillDesc = $"ユニットを貫通する<color=yellow>{Count}個</color>の手裏剣を扇形に投げ、ランダムな敵ユニット一体に手裏剣が集まります。\n" +
                      $"各手裏剣は攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Lança <color=yellow>{Count}</color> estrelas de arremesso em forma de leque que perfuram unidades e depois convergem em uma unidade inimiga aleatória.\n" +
                         $"Cada estrela de arremesso causa dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque.";

        RUSkillDesc = $"Бросает <color=yellow>{Count}</color> метательных звезд в форме веера, которые проникают через юниты, а затем сходятся на случайной вражеской единице.\n" +
                      $"Каждая метательная звезда наносит урон, равный <color=red>{AttackCoeff} раза</color> силы атаки.";

        ZH_HANSSkillDesc = $"以扇形投掷<color=yellow>{Count}个</color>能穿透单位的飞镖，然后汇聚到一个随机敌方单位上。\n" +
                           $"每个飞镖造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko"))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "표창 개수 <color=yellow>+1</color>";

        if (code.StartsWith("en"))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack power multiplier <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Shuriken count <color=yellow>+1</color>";

        if (code.StartsWith("fr"))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d’attaque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Nombre de shurikens <color=yellow>+1</color>";

        if (code.StartsWith("it"))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d’attacco <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Numero di shuriken <color=yellow>+1</color>";

        if (code.StartsWith("de"))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffsmultiplikator <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Shuriken-Anzahl <color=yellow>+1</color>";

        if (code.StartsWith("es"))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Cantidad de shurikens <color=yellow>+1</color>";

        if (code.StartsWith("ja"))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力倍率 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "手裏剣の数 <color=yellow>+1</color>";

        if (code.StartsWith("pt"))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Quantidade de shurikens <color=yellow>+1</color>";

        if (code.StartsWith("ru"))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Количество сюрикенов <color=yellow>+1</color>";

        if (code.StartsWith("zh"))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力倍率 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "手里剑数量 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack power multiplier <color=red>+0.1</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Shuriken count <color=yellow>+1</color>";
    }
}