using System.Collections;
using TMPro;
using UnityEngine;

public class Satan : UnitDefault
{
    [Header("패시브")]
    public float attackXSize;
    public float attackYSize;
    public float burnDamage;
    [Header("지옥불")]
    public float attackCoeff;
    public float meteorBurnDamage;
    public int meteorBurnCnt;
    public float meteorCnt;
    public float size;
    public GiantMeteor giantMeteorPrefab;

    public GameObject killState;
    public TextMeshProUGUI killCntTxt;

    static int cnt;

    protected override void Awake()
    {
        base.Awake();

        attackStyle = AttackStyle.range;
        if (GameManager.instance.isEnd)
            cnt = 0;
        attackSize = size;
    }

    protected override void Start()
    {
        base.Start();

        killCntTxt.SetText(cnt.ToString());
        killState.SetActive(true);
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
        SoundPlay(skillSoundClip);

        StartCoroutine(Meteor());
    }

    IEnumerator Meteor()
    {
        if (cnt >= 5) {
            cnt -= 5;
            killCntTxt.SetText(cnt.ToString());

            GiantMeteor giantMeteor = Instantiate(giantMeteorPrefab, GameManager.instance.skillEffect);
            giantMeteor.unitComp = this;
        }
        else {
            WaitForSeconds wait = new(0.1f);
            for (int i = 0; i < meteorCnt; i++) {
                Vector2 randPos = RandomPositionManager.instance.GetRandomBattleFieldPos();
                StartCoroutine(OnFire(null, randPos));

                yield return wait;
            }
        }
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = targetPos + (Vector2.up * 5f); // 시작 위치

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리

        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation, GameManager.instance.skillEffect);
        bulletList.Add(bullet);
        bullet.GetComponent<BoxCollider2D>().enabled = false;

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(isSkill ? skillBulletAnimClip : bulletAnimClip, size);

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

            SoundPlay(skillBulletHitClip);
            CreateAttackBox(AttackPower * attackCoeff, null, targetPos, attackStyle);
        }
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;
        float angle = 0f;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

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
            scale = new Vector2(attackXSize, attackYSize);
        else
            scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(targetPos, scale, angle, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            for (int i = hitEnemies.Length - 1; i >= 0; i--) {
                UnitDefault enemyComp = hitEnemies[i].GetComponent<UnitDefault>();
                if (!isSkill)
                    enemyComp.Burn(burnDamage, 1, this);
                else
                    enemyComp.Burn(meteorBurnDamage, meteorBurnCnt, this);

                damage += enemyComp.Hit(attackPower, CriticalPer, this, isPenetration);
                if (enemyComp.isDeath)
                    cnt++;
                killCntTxt.SetText(cnt.ToString());
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
        skillDesc = $"패시브: 기본 공격이 <color=#AEEAFF>{attackXSize}X{attackYSize}</color>의 범위 공격이 되며, <color=yellow>1초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>\n\n" +
                    $"지옥불: 랜덤 위치에 <color=#AEEAFF>{size}X{size}</color>크기의 메테오를 <color=yellow>{meteorCnt}개</color> 떨어뜨려 피격된 적들에게 공격력 <color=red>{attackCoeff}배</color>의 데미지를 주고, <color=yellow>{meteorBurnCnt}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{meteorBurnDamage}</color>\n" +
                    $"적을 <color=yellow>5명</color> 이상 처치 후 스킬을 사용하면 거대한 메테오를 떨어뜨려 위 효과를 모든 적 유닛에게 적용합니다.";
        ENSkillDesc = $"Passive: Basic attacks become a <color=#AEEAFF>{attackXSize}X{attackYSize}</color> area attack that applies burn for <color=yellow>1 second</color>.\n" +
                      $"Burn damage per second: <color=red>{burnDamage}</color>\n\n" +
                      $"Hellfire: Drops <color=yellow>{meteorCnt}</color> meteors of size <color=#AEEAFF>{size}X{size}</color> at random locations, dealing damage equal to <color=red>{attackCoeff} times</color> the attack power to hit enemies and burning them for <color=yellow>{meteorBurnCnt} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{meteorBurnDamage}</color>\n" +
                      $"If you have killed <color=yellow>5</color> or more enemies and use the skill, a giant meteor falls and applies the above effects to all enemy units.";

        FRSkillDesc = $"Passif : Les attaques de base deviennent une attaque de zone de <color=#AEEAFF>{attackXSize}X{attackYSize}</color> qui applique une brûlure pendant <color=yellow>1 seconde</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>\n\n" +
                      $"Feu infernal : Fait tomber <color=yellow>{meteorCnt}</color> météores de taille <color=#AEEAFF>{size}X{size}</color> à des emplacements aléatoires, infligeant des dégâts égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque aux ennemis touchés et les brûlant pendant <color=yellow>{meteorBurnCnt} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{meteorBurnDamage}</color>\n" +
                      $"Si vous avez tué <color=yellow>5</color> ennemis ou plus et utilisez la compétence, une météore géante tombe et applique les effets ci-dessus à toutes les unités ennemies.";

        ITSkillDesc = $"Passivo: Gli attacchi base diventano un attacco ad area di <color=#AEEAFF>{attackXSize}X{attackYSize}</color> che applica bruciatura per <color=yellow>1 secondo</color>.\n" +
                      $"Danno da bruciatura per secondo: <color=red>{burnDamage}</color>\n\n" +
                      $"Fuoco infernale: Fa cadere <color=yellow>{meteorCnt}</color> meteoriti di dimensione <color=#AEEAFF>{size}X{size}</color> in posizioni casuali, infliggendo danni pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco ai nemici colpiti e bruciandoli per <color=yellow>{meteorBurnCnt} secondi</color>.\n" +
                      $"Danno da bruciatura per secondo: <color=red>{meteorBurnDamage}</color>\n" +
                      $"Se hai ucciso <color=yellow>5</color> o più nemici e usi l'abilità, cade una meteora gigante e applica gli effetti di cui sopra a tutte le unità nemiche.";

        DESkillDesc = $"Passiv: Grundangriffe werden zu einem Flächenangriff von <color=#AEEAFF>{attackXSize}X{attackYSize}</color>, der für <color=yellow>1 Sekunde</color> Verbrennung anwendet.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{burnDamage}</color>\n\n" +
                      $"Höllenfeuer: Lässt <color=yellow>{meteorCnt}</color> Meteore der Größe <color=#AEEAFF>{size}X{size}</color> an zufälligen Orten fallen, die Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft an getroffene Gegner verursachen und sie für <color=yellow>{meteorBurnCnt} Sekunden</color> verbrennen.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{meteorBurnDamage}</color>\n" +
                      $"Wenn Sie <color=yellow>5</color> oder mehr Gegner getötet haben und die Fähigkeit einsetzen, fällt ein riesiger Meteorit und wendet die oben genannten Effekte auf alle Feinde an.";

        ESSkillDesc = $"Pasivo: Los ataques básicos se convierten en un ataque de <color=#AEEAFF>{attackXSize}X{attackYSize}</color> que aplica quemadura durante <color=yellow>1 segundo</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{burnDamage}</color>\n\n" +
                      $"Fuego infernal: Deja caer <color=yellow>{meteorCnt}</color> meteoritos de tamaño <color=#AEEAFF>{size}X{size}</color> en ubicaciones aleatorias, infligiendo daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque a los enemigos alcanzados y quemándolos durante <color=yellow>{meteorBurnCnt} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{meteorBurnDamage}</color>\n" +
                      $"Si has matado a <color=yellow>5</color> o más enemigos y usas la habilidad, cae un meteoro gigante y aplica los efectos anteriores a todos los enemigos.";

        JASkillDesc = $"パッシブ: 通常攻撃が<color=#AEEAFF>{attackXSize}X{attackYSize}</color>の範囲攻撃になり、<color=yellow>1秒</color>間火傷を与えます。\n" +
                      $"1秒の火傷ダメージ: <color=red>{burnDamage}</color>\n\n" +
                      $"地獄の火: ランダムな位置に<color=#AEEAFF>{size}X{size}</color>サイズの隕石を<color=yellow>{meteorCnt}個</color>落とし、命中した敵に攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、<color=yellow>{meteorBurnCnt}秒</color>間火傷を与えます。\n" +
                      $"1秒の火傷ダメージ: <color=red>{meteorBurnDamage}</color>\n" +
                      $"敵を<color=yellow>5</color>以上倒してからスキルを使用すると、巨大な隕石が落ち、上記の効果をすべての敵ユニットに適用します。";

        PT_BRSkillDesc = $"Passivo: Os ataques básicos se tornam um ataque de área de <color=#AEEAFF>{attackXSize}X{attackYSize}</color> que aplica queimadura por <color=yellow>1 segundo</color>.\n" +
                          $"Dano de queimadura por segundo: <color=red>{burnDamage}</color>\n\n" +
                          $"Fogo do inferno: Lança <color=yellow>{meteorCnt}</color> meteoros de tamanho <color=#AEEAFF>{size}X{size}</color> em locais aleatórios, infligindo dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque aos inimigos atingidos e queimando-os por <color=yellow>{meteorBurnCnt} segundos</color>.\n" +
                          $"Dano de queimadura por segundo: <color=red>{meteorBurnDamage}</color>\n" +
                          $"Se você matou <color=yellow>5</color> ou mais inimigos e usar a habilidade, um meteoro gigante cairá e aplicará os efeitos acima a todas as unidades inimigas.";

        RUSkillDesc = $"Пассив: Базовые атаки становятся атакой по области <color=#AEEAFF>{attackXSize}X{attackYSize}</color>, накладывающей горение на <color=yellow>1 секунду</color>.\n" +
                      $"Урон от горения в секунду: <color=red>{burnDamage}</color>\n\n" +
                      $"Адский огонь: Оставляет <color=yellow>{meteorCnt}</color> метеоров размером <color=#AEEAFF>{size}X{size}</color> в случайных местах, наносящих урон, равный <color=red>{attackCoeff} раза</color> силы атаки поражённым врагам и поджигая их на <color=yellow>{meteorBurnCnt} секунд</color>.\n" +
                      $"Урон от горения в секунду: <color=red>{meteorBurnDamage}</color>\n" +
                      $"Если вы убили <color=yellow>5</color> или более врагов и используете умение, падает гигантский метеор и применяет вышеуказанные эффекты ко всем врагам.";

        ZH_HANSSkillDesc = $"被动: 基础攻击变为<color=#AEEAFF>{attackXSize}X{attackYSize}</color>的范围攻击，造成<color=yellow>1秒</color>的灼烧效果。\n" +
                           $"每秒灼燒傷害: <color=red>{burnDamage}</color>\n\n" +
                           $"地獄之火: 在隨機位置降下<color=yellow>{meteorCnt}</color>個大小為<color=#AEEAFF>{size}X{size}</color>的隕石，對命中的敵人造成相當於攻擊力<color=red>{attackCoeff}倍</color>的傷害，並在<color=yellow>{meteorBurnCnt}秒</color>內使其灼燒。\n" +
                           $"每秒灼燒傷害: <color=red>{meteorBurnDamage}</color>\n" +
                           $"如果在使用技能前已擊殺<color=yellow>5</color>名或更多敵人，則降下巨型隕石，對所有敵方單位應用上述效果。";
    }
}
