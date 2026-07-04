using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Ranger : UnitDefault
{
    [Header("템페스트")]
    public float attackCoeff;
    public float decAttackSpeedAmount;
    public float decSpeedAmount;
    public float decDefenseAmount;
    public float duration;
    public int count;
    public float interval;
    public float size;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float DecAttackSpeedAmount { get => decAttackSpeedAmount + StaticManager.skillPoint[unitIdx] / 2 * 0.1f; }
    float DecSpeedAmount { get => decSpeedAmount + StaticManager.skillPoint[unitIdx] / 2 * 0.1f; }
    float DecDefenseAmount { get => decDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 3; }

    WaitForSeconds wait;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
        wait = new(interval);
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
        StartCoroutine(Tempest());
    }

    IEnumerator Tempest()
    {
        for (int i = 0; i < Count; i++) {
            Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
            float posX = Random.Range(pos.x - size / 2f, pos.x + size / 2f);
            float posY = Random.Range(pos.y - size / 2f, pos.y + size / 2f);
            StartCoroutine(OnFire(null, new Vector2(posX, posY)));
            yield return wait;
        }
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
            elapsedTime += UnityEngine.Time.fixedDeltaTime;

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

            if (isSkill) {
                Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, attackSize / 2f, LayerMask.GetMask(enemyTag));

                Collider2D closest = null;
                float minDist = float.MaxValue;

                foreach (var hit in hits) {
                    float dist = Vector2.Distance(targetPos, hit.transform.position);
                    if (dist < minDist) {
                        minDist = dist;
                        closest = hit;
                    }
                }

                if (closest != null) {
                    SoundPlay(skillBulletHitClip);
                    closest.GetComponent<UnitDefault>().DecreaseTempest(DecAttackSpeedAmount, DecSpeedAmount, DecDefenseAmount, Duration);
                    CreateAttackBox(AttackPower * AttackCoeff, closest.gameObject, closest.transform.position);
                }
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상 반경 <color=#AEEAFF>{size}X{size}</color>범위에 <color=yellow>{Count}개</color>의 별들을 떨어뜨려 피격 시 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, " +
                    $"<color=yellow>{Duration}초</color> 동안 공격 속도를 <color=red>{DecAttackSpeedAmount}</color>, 이동 속도를 <color=red>{DecSpeedAmount}</color>, 방어력을 <color=red>{DecDefenseAmount}</color>만큼 감소시킵니다.\n" +
                    $"(디버프는 중첩되지 않음)";
        
        ENSkillDesc = $"Drops <color=yellow>{Count}</color> stars in a <color=#AEEAFF>{size}X{size}</color> area around the attacking target. When hit, deals damage equal to <color=red>{AttackCoeff} times</color> the attack power, and reduces attack speed by <color=red>{DecAttackSpeedAmount}</color>, movement speed by <color=red>{DecSpeedAmount}</color>, and defense by <color=red>{DecDefenseAmount}</color> for <color=yellow>{Duration} seconds.</color>\n" +
                      $"(Debuffs do not stack)";

        FRSkillDesc = $"Lance <color=yellow>{Count}</color> étoiles dans une zone de <color=#AEEAFF>{size}X{size}</color> autour de la cible attaquée. Lorsqu'elles sont touchées, elles infligent des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque et réduisent la vitesse d'attaque de <color=red>{DecAttackSpeedAmount}</color>, la vitesse de déplacement de <color=red>{DecSpeedAmount}</color> et la défense de <color=red>{DecDefenseAmount}</color> pendant <color=yellow>{Duration} secondes.</color>\n" +
                      $"(Les débuffs ne se cumulent pas)";

        ITSkillDesc = $"Lancia <color=yellow>{Count}</color> stelle in un'area di <color=#AEEAFF>{size}X{size}</color> intorno al bersaglio attaccante. Quando vengono colpite, infliggono danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco e riducono la velocità di attacco di <color=red>{DecAttackSpeedAmount}</color>, la velocità di movimento di <color=red>{DecSpeedAmount}</color> e la difesa di <color=red>{DecDefenseAmount}</color> per <color=yellow>{Duration} secondi.</color>\n" +
                      $"(I debuff non si accumulano)";

        DESkillDesc = $"Lässt <color=yellow>{Count}</color> Sterne in einem <color=#AEEAFF>{size}X{size}</color>-Bereich um das angreifende Ziel fallen. Bei Treffer verursachen sie Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft und verringern die Angriffsgeschwindigkeit um <color=red>{DecAttackSpeedAmount}</color>, die Bewegungsgeschwindigkeit um <color=red>{DecSpeedAmount}</color> und die Verteidigung um <color=red>{DecDefenseAmount}</color> für <color=yellow>{Duration} Sekunden.</color>\n" +
                      $"(Debuffs stapeln sich nicht)";

        ESSkillDesc = $"Deja caer <color=yellow>{Count}</color> estrellas en un área de <color=#AEEAFF>{size}X{size}</color> alrededor del objetivo atacado. Al ser golpeadas, infligen daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque y reducen la velocidad de ataque en <color=red>{DecAttackSpeedAmount}</color>, la velocidad de movimiento en <color=red>{DecSpeedAmount}</color> y la defensa en <color=red>{DecDefenseAmount}</color> durante <color=yellow>{Duration} segundos.</color>\n" +
                      $"(Los debuffs no se acumulan)";

        JASkillDesc = $"攻撃中の対象の周囲<color=#AEEAFF>{size}X{size}</color>範囲に<color=yellow>{Count}個</color>の星を落とし、命中時に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、" +
                      $"<color=yellow>{Duration}秒</color>間、攻撃速度を<color=red>{DecAttackSpeedAmount}</color>、移動速度を<color=red>{DecSpeedAmount}</color>、防御力を<color=red>{DecDefenseAmount}</color>だけ減少させます。\n" +
                      $"(デバフは重複しません)";

        PT_BRSkillDesc = $"Deixa cair <color=yellow>{Count}</color> estrelas em uma área de <color=#AEEAFF>{size}X{size}</color> ao redor do alvo atacante. Quando atingidas, causam dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque e reduzem a velocidade de ataque em <color=red>{DecAttackSpeedAmount}</color>, a velocidade de movimento em <color=red>{DecSpeedAmount}</color> e a defesa em <color=red>{DecDefenseAmount}</color> por <color=yellow>{Duration} segundos.</color>\n" +
                         $"(Os debuffs não se acumulam)";

        RUSkillDesc = $"Роняет <color=yellow>{Count}</color> звезд в область <color=#AEEAFF>{size}X{size}</color> вокруг атакующей цели. При попадании они наносят урон, равный <color=red>{AttackCoeff} раза</color> силы атаки, и уменьшают скорость атаки на <color=red>{DecAttackSpeedAmount}</color>, скорость передвижения на <color=red>{DecSpeedAmount}</color> и защиту на <color=red>{DecDefenseAmount}</color> на <color=yellow>{Duration} секунд.</color>\n" +
                      $"(Дебаффы не складываются)";

        ZH_HANSSkillDesc = $"在攻击目标周围的<color=#AEEAFF>{size}X{size}</color>范围内掉落<color=yellow>{Count}个</color>星星。命中时，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，并在<color=yellow>{Duration}秒</color>内将攻击速度降低<color=red>{DecAttackSpeedAmount}</color>、移动速度降低<color=red>{DecSpeedAmount}</color>、防御力降低<color=red>{DecDefenseAmount}</color>。\n" +
                           $"（减益效果不叠加）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "방어력 감소량 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "공격 속도 감소량 <color=red>+0.1</color>\n" +
                   "이동 속도 감소량 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "별 개수 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "Defense reduction <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Attack speed decrease <color=red>+0.1</color>\n" +
                   "Movement speed decrease <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Star count <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Réduction de défense <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Réduction de la vitesse d'attaque <color=red>+0.1</color>\n" +
                   "Réduction de la vitesse de déplacement <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Nombre d'étoiles <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Riduzione difesa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Riduzione velocità d'attacco <color=red>+0.1</color>\n" +
                   "Riduzione velocità di movimento <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Numero di stelle <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Verteidigungsreduktion <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Angriffsgeschwindigkeit - <color=red>+0.1</color>\n" +
                   "Bewegungsgeschwindigkeit - <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Anzahl der Sterne <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Reducción de defensa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Reducción de velocidad de ataque <color=red>+0.1</color>\n" +
                   "Reducción de velocidad de movimiento <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Número de estrellas <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "防御力減少量 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "攻撃速度減少量 <color=red>+0.1</color>\n" +
                   "移動速度減少量 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "星の数 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Redução de defesa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Redução de velocidade de ataque <color=red>+0.1</color>\n" +
                   "Redução de velocidade de movimento <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Número de estrelas <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "Снижение защиты <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Снижение скорости атаки <color=red>+0.1</color>\n" +
                   "Снижение скорости передвижения <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Количество звёзд <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "防御力减少 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "攻击速度减少量 <color=red>+0.1</color>\n" +
                   "移动速度减少量 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "星数 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "Defense reduction <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Attack speed decrease <color=red>+0.1</color>\n" +
               "Movement speed decrease <color=red>+0.1</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Star count <color=yellow>+1</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}