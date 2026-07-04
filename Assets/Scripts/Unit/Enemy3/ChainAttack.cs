using System.Collections;
using UnityEngine;

public class ChainAttack : UnitDefault
{
    [Header("연쇄")]
    public float attackCoeff;
    public float burnDamage;
    public int count;
    public int chainCount;

    int chainCount2;
    GameObject skillBullet;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target) || skillBullet != null)
            return;

        chainCount2 = 0;
        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        StartCoroutine(OnFire(SkillTarget, SkillTarget.transform.position));
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        unitComp.Burn(burnDamage, count, this);
        CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);

        if (chainCount2 == chainCount)
            return;

        chainCount2++;

        RandomEnemyUnit(friends, unitComp);
        skillTargetPos = target.transform.position;

        OnSkillAttack();
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = (isSkill && chainCount2 != 0) ? skillTargetPos : transform.position; // 시작 위치

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration;
        if (!isSkill)
            duration = distance / ShotSpeed; // 총 이동 시간 계산
        else
            duration = distance / 5f;

        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation);
        bulletList.Add(bullet);
        if (isSkill) {
            bullet.tag = "Skill Bullet";
            skillBullet = bullet;
        }
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
        skillDesc = $"공격 중인 대상에게 화염을 발사해 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, <color=yellow>{count}초</color> 동안 화상을 입힙니다.\n" +
                    $"적중 시 다른 랜덤 적 유닛에게 최대 <color=yellow>{chainCount}번</color> 까지 튕기며 같은 효과를 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>";

        ENSkillDesc = $"Fires a flame at the current target dealing damage equal to <color=red>{attackCoeff} times</color> the attack power and burning them for <color=yellow>{count} seconds</color>.\n" +
                       $"On hit, it can bounce to other random enemies up to <color=yellow>{chainCount} times</color>, applying the same effect.\n" +
                       $"Burn damage per second: <color=red>{burnDamage}</color>";

        FRSkillDesc = $"Tire une flamme sur la cible actuelle, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque et la brûlant pendant <color=yellow>{count} secondes</color>.\n" +
                      $"À l'impact, elle peut rebondir sur d'autres ennemis aléatoires jusqu'à <color=yellow>{chainCount} fois</color>, appliquant le même effet.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>";

        ITSkillDesc = $"Spara una fiamma sul bersaglio corrente infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco e bruciandolo per <color=yellow>{count} secondi</color>.\n" +
                      $"All'impatto può rimbalzare su altri nemici casuali fino a <color=yellow>{chainCount} volte</color>, applicando lo stesso effetto.\n" +
                      $"Danno da bruciatura per secondo: <color=red>{burnDamage}</color>";

        DESkillDesc = $"Feuert eine Flamme auf das aktuelle Ziel, die Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft verursacht und es für <color=yellow>{count} Sekunden</color> verbrennt.\n" +
                      $"Bei einem Treffer kann sie bis zu <color=yellow>{chainCount} Mal</color> auf andere zufällige Gegner überspringen und den gleichen Effekt anwenden.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>";

        ESSkillDesc = $"Dispara una llama al objetivo actual que inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque y lo quema durante <color=yellow>{count} segundos</color>.\n" +
                      $"Al impactar, puede rebotar hacia otros enemigos aleatorios hasta <color=yellow>{chainCount} veces</color>, aplicando el mismo efecto.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>";

        JASkillDesc = $"攻撃対象に火炎を発射し、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{count}秒</color>間火傷させます。\n" +
                      $"命中時に他のランダムな敵に最大<color=yellow>{chainCount}回</color>跳ね返り、同じ効果を与えます。\n" +
                      $"毎秒の火傷ダメージ：<color=red>{burnDamage}</color>";

        PT_BRSkillDesc = $"Dispara uma chama no alvo atual causando dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e queimando-o por <color=yellow>{count} segundos</color>.\n" +
                          $"Ao atingir, pode ricochetear para outros inimigos aleatórios até <color=yellow>{chainCount} vezes</color>, aplicando o mesmo efeito.\n" +
                          $"Dano de queimadura por segundo: <color=red>{burnDamage}</color>";

        RUSkillDesc = $"Выпускает пламя по текущей цели, наносящее урон в размере <color=red>{attackCoeff} раза</color> силы атаки и поджигающее её на <color=yellow>{count} секунд</color>.\n" +
                      $"При попадании оно может отскочить на других случайных врагов до <color=yellow>{chainCount} раз</color>, нанося тот же эффект.\n" +
                      $"Урон от горения в секунду: <color=red>{burnDamage}</color>";

        ZH_HANSSkillDesc = $"向前目标射火焰，其造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，在<color=yellow>{count}秒</color>内使其灼烧。\n" +
                          $"命中可在其他随机敌人之间最多弹射<color=yellow>{chainCount}次</color>，施加相同效果。\n" +
                          $"每秒灼烧伤害：<color=red>{burnDamage}</color>";
    }
}