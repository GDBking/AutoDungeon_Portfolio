using System.Collections;
using UnityEngine;

public class Rose : UnitDefault
{
    [Header("장미")]
    public float attackCoeff;
    public float duration;
    public int count;
    public GameObject roseObj;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        for (int i = 0; i < count; i++) {
            Vector2 randPos = RandomPositionManager.instance.GetRandomBattleFieldPos();
            StartCoroutine(OnFire(null, randPos));
        }
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = transform.position; // 시작 위치

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

            if (isSkill){
                GameObject rose = Instantiate(roseObj, targetPos, Quaternion.identity, GameManager.instance.skillEffect);
                rose.GetComponent<RoseObj>().roseComp = this;
                bulletList.Add(rose);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"무작위 위치에 <color=yellow>{count}개</color>의 장미를 뿌립니다.\n" +
                    $"장미를 밟은 적은 공격력 <color=red>{attackCoeff}배</color>의 방어력 관통 피해를 입고, <color=yellow>{duration}초</color> 동안 매혹됩니다.";

        ENSkillDesc = $"Scatters <color=yellow>{count} roses</color> at random locations.\n" +
                       $"Enemies that step on the roses take defense-piercing damage equal to <color=red>{attackCoeff} times</color> the attack power and become charmed for <color=yellow>{duration} seconds.</color>";

        FRSkillDesc = $"Éparpille <color=yellow>{count} roses</color> à des emplacements aléatoires.\n" +
                          $"Les ennemis qui marchent sur les roses subissent des dégâts perforants égaux à <color=red>{attackCoeff} fois</color> la puissance d'attaque et deviennent charmés pendant <color=yellow>{duration} secondes.</color>";

        ITSkillDesc = $"Sparge <color=yellow>{count} rose</color> in posizioni casuali.\n" +
                            $"I nemici che calpestano le rose subiscono danni perforanti pari a <color=red>{attackCoeff} volte</color> la potenza d'attacco e diventano affascinati per <color=yellow>{duration} secondi.</color>";

        DESkillDesc = $"Streut <color=yellow>{count} Rosen</color> an zufälligen Orten.\n" +
                            $"Feinde, die auf die Rosen treten, erleiden verteidigungsdurchdringenden Schaden in Höhe von <color=red>{attackCoeff} mal</color> der Angriffskraft und werden für <color=yellow>{duration} Sekunden</color> verzaubert.";

        ESSkillDesc = $"Esparce <color=yellow>{count} rosas</color> en ubicaciones aleatorias.\n" +
                            $"Los enemigos que pisan las rosas reciben daño que penetra la defensa igual a <color=red>{attackCoeff} veces</color> el poder de ataque y quedan encantados durante <color=yellow>{duration} segundos.</color>";

        JASkillDesc = $"ランダムな位置に<color=yellow>{count}個</color>のバラを撒き散らします。\n" +
                            $"バラを踏んだ敵は攻撃力の<color=red>{attackCoeff}倍</color>の防御貫通ダメージを受け、<color=yellow>{duration}秒</color>間、魅了状態になります。";

        PT_BRSkillDesc = $"Espalha <color=yellow>{count} rosas</color> em locais aleatórios.\n" +
                            $"Os inimigos que pisam nas rosas recebem dano que penetra a defesa igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e ficam encantados por <color=yellow>{duration} segundos.</color>";

        RUSkillDesc = $"Разбрасывает <color=yellow>{count} роз</color> в случайных местах.\n" +
                            $"Враги, наступившие на розы, получают урон, игнорирующий защиту, равный <color=red>{attackCoeff} раз</color> от силы атаки, и становятся очарованными на <color=yellow>{duration} секунд.</color>";

        ZH_HANSSkillDesc = $"在随机位置散布<color=yellow>{count}朵玫瑰</color>。\n" +
                            $"踩到玫瑰的敌人会受到相当于攻击力<color=red>{attackCoeff}倍</color>的防御穿透伤害，并在<color=yellow>{duration}秒</color>内被迷惑。";
    }
}
