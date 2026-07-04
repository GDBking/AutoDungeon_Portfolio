using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Belphegor : UnitDefault
{
    [Header("패시브")]
    public float attackCoeff;
    public float incHealthPer;
    public float attackCoeff2;
    public float stunDuration;
    public float incAttackAmount;
    public float incDefenseAmount;
    public float incSpeedAmount;
    public float incAttackSpeedAmount;
    public float size;
    public GameObject circleObj;
    public GameObject stackState;
    public TextMeshProUGUI stackCntTxt;
    public Image stackCooldownImg;
    public AnimationClip stackUpAnimClip;
    public AnimationClip stunAnimClip;
    [Header("악령 흡수")]
    public float incHealthAmountSkill;
    public float incAttackAmountSkill;
    public float incDefenseAmountSkill;
    public float buffDuration;
    public float decDefenseAmount;
    public float decSpeedPer;
    public float debuffDuration;
    public AnimationClip circleSkillAnimClip;
    public AudioClip wakeupSoundClip;

    bool isWakeup;
    int stack;
    float elapsedTime;
    readonly WaitForSeconds wait = new(1f);

    protected override void Start()
    {
        base.Start();

        stackState.SetActive(true);
        circleObj.transform.localScale *= size;
    }

    private void Update()
    {
        if (isWakeup)
            return;

        elapsedTime += Time.deltaTime;
        stackCooldownImg.fillAmount = elapsedTime / 5f;

        if (Health / MaxHealth <= 0.8f || stack == 4)
        {
            isWakeup = true;
            stackCooldownImg.fillAmount = 0f;

            SoundPlay(wakeupSoundClip);

            circleObj.SetActive(true);

            StartCoroutine(Wakeup());
            if (stack >= 2)
            {
                attackSize = size;
                CreateAttackBox(AttackPower * attackCoeff2, gameObject, transform.position, AttackStyle.range);
                attackSize = 0.5f;

                if (stack == 4)
                {
                    CameraShake.instance.Shake(2f, 2f);

                    foreach (UnitDefault unitComp in friends)
                    {
                        GameObject effect = Instantiate(attackObj, unitComp.transform);
                        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(stunAnimClip);

                        unitComp.OnStunAnim(stunDuration);
                    }
                }

                StartCoroutine(ChargingMana());
            }

            return;
        }

        if (elapsedTime >= 5f)
        {
            elapsedTime = 0f;

            stackCntTxt.SetText((++stack).ToString());
            GameObject effect = Instantiate(attackObj, transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(stackUpAnimClip);

            SetStat(attackPowerStat, incAttackAmount, false, true);
            SetStat(defenseStat, incDefenseAmount, false, true);
            SetStat(speedStat, incSpeedAmount, false, true);
            SetStat(attackSpeedStat, incAttackSpeedAmount, false, true);
        }
    }

    protected override void FixedUpdate()
    {
        if (!isWakeup)
            return;

        base.FixedUpdate();
    }

    protected override void UseSkill()
    {
        if ((stack < 4 && !CheckForTargetRange(size / 2f, FindNearestFriendly())) || (stack == 4 && !CheckForTargetRange(size / 2f, FindNearestFriendly()) && !CheckForTargetRange(size / 2f, Target)))
            return;

        StartCoroutine(OnSkillAnim());
    }

    GameObject FindNearestFriendly()
    {
        float nearest = float.MaxValue; // 현재 가장 가까운 거리
        GameObject nearestFriendly = null; // 현재 가장 가까운 유닛
        // 모든 적 유닛을 검사하며 가장 가까운 적 유닛을 판별
        foreach (UnitDefault unitComp in enemies)
        {
            if (unitComp.CompareTag("Untagged") || unitComp == this)
                continue;

            float dist = Vector2.Distance(transform.position, unitComp.transform.position);
            // 현재 거리보다 가까우면 최신화
            if (dist < nearest)
            {
                nearest = dist;
                nearestFriendly = unitComp.gameObject;
            }
        }

        return nearestFriendly;
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        GameObject effect = Instantiate(attackObj, transform);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(circleSkillAnimClip);
        effect.transform.localScale *= size;

        Collider2D[] hitFriends = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(tag));
        foreach (Collider2D friendly in hitFriends)
        {
            UnitDefault friendlyComp = friendly.GetComponent<UnitDefault>();
            if (friendlyComp == this)
                continue;

            friendlyComp.Healing(incHealthAmountSkill);
            friendlyComp.SetStat(friendlyComp.attackPowerStat, incAttackAmountSkill, false, true, buffDuration);
            friendlyComp.SetStat(friendlyComp.defenseStat, incDefenseAmountSkill, false, true, buffDuration);

            friendlyComp.SetStateBar(State.incAttack, buffDuration);
            friendlyComp.SetStateBar(State.incDefense, buffDuration);
        }

        if (stack == 4)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
            foreach (Collider2D enemy in hitEnemies)
            {
                UnitDefault enemyComp = enemy.GetComponent<UnitDefault>();

                enemyComp.SetStat(enemyComp.defenseStat, decDefenseAmount, false, false, debuffDuration);
                enemyComp.SetStat(enemyComp.speedStat, decSpeedPer, true, false, debuffDuration);

                enemyComp.SetStateBar(State.decDefense, debuffDuration);
                enemyComp.SetStateBar(State.decSpeed, debuffDuration);
            }
        }
    }

    IEnumerator Wakeup()
    {
        while (true)
        {
            attackSize = size;
            CreateAttackBox(AttackPower * attackCoeff, null, transform.position, AttackStyle.range);
            attackSize = 0.5f;

            yield return wait;
        }
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation))
        {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation))
        {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1f : 1f;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else
        {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize / 2f, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0)
        {
            if (attackStyle == AttackStyle.single)
            {
                foreach (Collider2D enemy in hitEnemies)
                {
                    if (enemy.gameObject == target)
                    {
                        damage = target.GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
                        break;
                    }
                }
            }
            else
            {
                for (int i = hitEnemies.Length - 1; i >= 0; i--)
                    damage += hitEnemies[i].GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
            }

            // 피흡 계산
            float lifeSteal;
            if (isSkill && target == null)
            {
                lifeSteal = damage / 100f * incHealthPer;
            }
            else
            {
                lifeSteal = damage * LifeStealPer / 100f;
            }
            Healing(lifeSteal);

            DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
        }
        return damage;
    }

    protected override IEnumerator ChargingMana()
    {
        if (!isWakeup)
            yield break;

        yield return base.ChargingMana();
    }

    public override void SetMana(int amount)
    {
        if (!isWakeup)
            return;

        base.SetMana(amount);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"패시브: <color=yellow>5초</color> 마다 악령이 <color=yellow>1개</color>씩 쌓입니다.\n" +
                    $"악령 1개당 본인의 공격력이 <color=green>{incAttackAmount}</color>, 방어력이 <color=green>{incDefenseAmount}</color>, 이동 속도가 <color=green>{incSpeedAmount}</color>, 공격 속도가 <color=green>{incAttackSpeedAmount}</color>만큼 증가합니다.\n" +
                    $"악령이 <color=yellow>4개</color>가 되거나 체력이 <color=red>80%</color> 이하로 떨어지면 벨페고르가 깨어납니다.\n" +
                    $"벨페고르가 깨어난 후 아래 효과가 발동됩니다.\n" +
                    $"벨페고르는 <color=#AEEAFF>{size}X{size}</color>크기의 써클을 갖고 있습니다.\n" +
                    $"<color=yellow>0스택</color>: 매초 써클 안의 적들에게 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고, 입힌 피해량의 <color=green>{incHealthPer}%</color>만큼 체력을 회복합니다.\n" +
                    $"<color=yellow>2스택</color>: 써클 안의 적들에게 공격력 <color=red>{attackCoeff2}배</color>의 피해를 입힙니다.(1회)\n" +
                    $"<color=yellow>4스택</color>: 모든 적 유닛들을 <color=yellow>{stunDuration}초</color> 동안 기절시킵니다.(1회)\n" +
                    $"(효과는 중첩됩니다.)\n\n" +
                    $"악령 흡수: 스택에 따라 아래 효과가 발동됩니다.\n" +
                    $"<color=yellow>0스택</color>: 스킬 없음\n" +
                    $"<color=yellow>2스택</color>: 써클 안의 아군 유닛들의 체력을 <color=green>{incHealthAmountSkill}</color>만큼 회복시키고, <color=yellow>{buffDuration}초</color> 동안 공격력을 <color=green>{incAttackAmountSkill}</color>, 방어력을 <color=green>{incDefenseAmountSkill}</color>만큼 증가시킵니다.\n" +
                    $"<color=yellow>4스택</color>: 써클 안의 적군 유닛들은 <color=yellow>{debuffDuration}초</color> 동안 방어력이 <color=red>{decDefenseAmount}</color>, 이동 속도가 <color=red>{decSpeedPer}%</color> 감소합니다.\n" +
                    $"(효과는 중첩됩니다.)";

        ENSkillDesc = $"Passive: Every <color=yellow>5 seconds</color>, Belphegor gains <color=yellow>1 stack</color> of Evil Spirit.\n" +
                      $"Each stack increases his Attack by <color=green>{incAttackAmount}</color>, Defense by <color=green>{incDefenseAmount}</color>, Speed by <color=green>{incSpeedAmount}</color>, and Attack Speed by <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"When he reaches <color=yellow>4 stacks</color> or his HP drops below <color=red>80%</color>, Belphegor awakens.\n" +
                      $"Upon awakening, the following effects are activated:\n" +
                      $"Belphegor has a circle of size <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 stacks</color>: Deals damage equal to <color=red>{attackCoeff}x</color> of his Attack to enemies within the circle every second, healing himself for <color=green>{incHealthPer}%</color> of the damage dealt.\n" +
                      $"<color=yellow>2 stacks</color>: Deals damage equal to <color=red>{attackCoeff2}x</color> of his Attack to enemies within the circle. (Once)\n" +
                      $"<color=yellow>4 stacks</color>: Stuns all enemy units for <color=yellow>{stunDuration} seconds</color>. (Once)\n" +
                      $"(Effects can stack.)\n\n" +
                      $"Evil Spirit Absorption: The following effects are activated based on the number of stacks:\n" +
                      $"<color=yellow>0 stacks</color>: No skill\n" +
                      $"<color=yellow>2 stacks</color>: Heals allied units within the circle for <color=green>{incHealthAmountSkill}</color> and increases their Attack by <color=green>{incAttackAmountSkill}</color> and Defense by <color=green>{incDefenseAmountSkill}</color> for <color=yellow>{buffDuration} seconds</color>.\n" +
                      $"<color=yellow>4 stacks</color>: Decreases the Defense of enemy units within the circle by <color=red>{decDefenseAmount}</color> and Speed by <color=red>{decSpeedPer}%</color> for <color=yellow>{debuffDuration} seconds</color>.\n" +
                      $"(Effects can stack.)";

        FRSkillDesc = $"Passif : Toutes les <color=yellow>5 secondes</color>, Belphégor gagne <color=yellow>1 pile</color> d'Esprit Maléfique.\n" +
                      $"Chaque pile augmente son Attaque de <color=green>{incAttackAmount}</color>, sa Défense de <color=green>{incDefenseAmount}</color>, sa Vitesse de <color=green>{incSpeedAmount}</color> et sa Vitesse d'Attaque de <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Lorsqu'il atteint <color=yellow>4 piles</color> ou que sa HP tombe en dessous de <color=red>80%</color>, Belphégor s'éveille.\n" +
                      $"Lors de son éveil, les effets suivants sont activés :\n" +
                      $"Belphégor a un cercle de taille <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 piles</color> : Inflige des dégâts égaux à <color=red>{attackCoeff}x</color> de son Attaque aux ennemis dans le cercle chaque seconde, se soignant de <color=green>{incHealthPer}%</color> des dégâts infligés.\n" +
                      $"<color=yellow>2 piles</color> : Inflige des dégâts égaux à <color=red>{attackCoeff2}x</color> de son Attaque aux ennemis dans le cercle. (Une fois)\n" +
                      $"<color=yellow>4 piles</color> : Étourdit toutes les unités ennemies pendant <color=yellow>{stunDuration} secondes</color>. (Une fois)\n" +
                      $"(Les effets peuvent s'empiler.)\n\n" +
                      $"Absorption d'Esprit Maléfique : Les effets suivants sont activés en fonction du nombre de piles :\n" +
                      $"<color=yellow>0 piles</color> : Pas de compétence\n" +
                      $"<color=yellow>2 piles</color> : Soigne les unités alliées dans le cercle de <color=green>{incHealthAmountSkill}</color> et augmente leur Attaque de <color=green>{incAttackAmountSkill}</color> et leur Défense de <color=green>{incDefenseAmountSkill}</color> pendant <color=yellow>{buffDuration} secondes</color>.\n" +
                      $"<color=yellow>4 piles</color> : Diminue la Défense des unités ennemies dans le cercle de <color=red>{decDefenseAmount}</color> et la Vitesse de <color=red>{decSpeedPer}%</color> pendant <color=yellow>{debuffDuration} secondes</color>.\n" +
                      $"(Les effets peuvent s'empiler.)";

        ITSkillDesc = $"Passivo: Ogni <color=yellow>5 secondi</color>, Belphegor guadagna <color=yellow>1 stack</color> di Spirito Malvagio.\n" +
                      $"Ogni stack aumenta il suo Attacco di <color=green>{incAttackAmount}</color>, la Difesa di <color=green>{incDefenseAmount}</color>, la Velocità di <color=green>{incSpeedAmount}</color> e la Velocità d'Attacco di <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Quando raggiunge <color=yellow>4 stack</color> o il suo HP scende sotto il <color=red>80%</color>, Belphegor si risveglia.\n" +
                      $"Al risveglio, si attivano i seguenti effetti:\n" +
                      $"Belphegor ha un cerchio di dimensioni <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 stack</color>: Infligge danni pari a <color=red>{attackCoeff}x</color> del suo Attacco ai nemici all'interno del cerchio ogni secondo, curandosi per <color=green>{incHealthPer}%</color> dei danni inflitti.\n" +
                      $"<color=yellow>2 stack</color>: Infligge danni pari a <color=red>{attackCoeff2}x</color> del suo Attacco ai nemici all'interno del cerchio. (Una volta)\n" +
                      $"<color=yellow>4 stack</color>: Stordisce tutte le unità nemiche per <color=yellow>{stunDuration} secondi</color>. (Una volta)\n" +
                      $"(Gli effetti possono accumularsi.)\n\n" +
                      $"Assorbimento di Spirito Malvagio: Si attivano i seguenti effetti in base al numero di stack:\n" +
                      $"<color=yellow>0 stack</color>: Nessuna abilità\n" +
                      $"<color=yellow>2 stack</color>: Cura le unità alleate all'interno del cerchio per <color=green>{incHealthAmountSkill}</color> e aumenta il loro Attacco di <color=green>{incAttackAmountSkill}</color> e la loro Difesa di <color=green>{incDefenseAmountSkill}</color> per <color=yellow>{buffDuration} secondi</color>.\n" +
                      $"<color=yellow>4 stack</color>: Diminuisce la Difesa delle unità nemiche all'interno del cerchio di <color=red>{decDefenseAmount}</color> e la Velocità di <color=red>{decSpeedPer}%</color> per <color=yellow>{debuffDuration} secondi</color>.\n" +
                      $"(Gli effetti possono accumularsi.)";

        DESkillDesc = $"Passiv: Alle <color=yellow>5 Sekunden</color> erhält Belphegor <color=yellow>1 Stapel</color> des Bösen Geistes.\n" +
                      $"Jeder Stapel erhöht seinen Angriff um <color=green>{incAttackAmount}</color>, seine Verteidigung um <color=green>{incDefenseAmount}</color>, seine Geschwindigkeit um <color=green>{incSpeedAmount}</color> und seine Angriffsgeschwindigkeit um <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Wenn er <color=yellow>4 Stapel</color> erreicht oder seine HP unter <color=red>80%</color> fällt, erwacht Belphegor.\n" +
                      $"Beim Erwachen werden die folgenden Effekte aktiviert:\n" +
                      $"Belphegor hat einen Kreis der Größe <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 Stapel</color>: Verursacht allen Feinden im Kreis jeden zweiten Schaden in Höhe von <color=red>{attackCoeff}x</color> seines Angriffs und heilt sich um <color=green>{incHealthPer}%</color> des verursachten Schadens.\n" +
                      $"<color=yellow>2 Stapel</color>: Verursacht allen Feinden im Kreis Schaden in Höhe von <color=red>{attackCoeff2}x</color> seines Angriffs. (Einmal)\n" +
                      $"<color=yellow>4 Stapel</color>: Betäubt alle feindlichen Einheiten für <color=yellow>{stunDuration} Sekunden</color>. (Einmal)\n" +
                      $"(Effekte können gestapelt werden.)\n\n" +
                      $"Absorption des Bösen Geistes: Die folgenden Effekte werden basierend auf der Anzahl der Stapel aktiviert:\n" +
                      $"<color=yellow>0 Stapel</color>: Keine Fähigkeit\n" +
                      $"<color=yellow>2 Stapel</color>: Heilt verbündete Einheiten im Kreis um <color=green>{incHealthAmountSkill}</color> und erhöht ihren Angriff um <color=green>{incAttackAmountSkill}</color> und ihre Verteidigung um <color=green>{incDefenseAmountSkill}</color> für <color=yellow>{buffDuration} Sekunden</color>.\n" +
                      $"<color=yellow>4 Stapel</color>: Verringert die Verteidigung feindlicher Einheiten im Kreis um <color=red>{decDefenseAmount}</color> und die Geschwindigkeit um <color=red>{decSpeedPer}%</color> für <color=yellow>{debuffDuration} Sekunden</color>.\n" +
                      $"(Effekte können gestapelt werden.)";

        ESSkillDesc = $"Pasivo: Cada <color=yellow>5 segundos</color>, Belphegor gana <color=yellow>1 pila</color> de Espíritu Maligno.\n" +
                      $"Cada pila aumenta su Ataque en <color=green>{incAttackAmount}</color>, Defensa en <color=green>{incDefenseAmount}</color>, Velocidad en <color=green>{incSpeedAmount}</color> y Velocidad de Ataque en <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Cuando alcanza <color=yellow>4 pilas</color> o su HP cae por debajo del <color=red>80%</color>, Belphegor despierta.\n" +
                      $"Al despertar, se activan los siguientes efectos:\n" +
                      $"Belphegor tiene un círculo de tamaño <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 pilas</color>: Inflige daño igual a <color=red>{attackCoeff}x</color> de su Ataque a los enemigos dentro del círculo cada segundo, curándose por <color=green>{incHealthPer}%</color> del daño infligido.\n" +
                      $"<color=yellow>2 pilas</color>: Inflige daño igual a <color=red>{attackCoeff2}x</color> de su Ataque a los enemigos dentro del círculo. (Una vez)\n" +
                      $"<color=yellow>4 pilas</color>: Aturde a todas las unidades enemigas durante <color=yellow>{stunDuration} segundos</color>. (Una vez)\n" +
                      $"(Los efectos pueden acumularse.)\n\n" +
                      $"Absorción de Espíritu Maligno: Se activan los siguientes efectos según el número de pilas:\n" +
                      $"<color=yellow>0 pilas</color>: Sin habilidad\n" +
                      $"<color=yellow>2 pilas</color>: Cura a las unidades aliadas dentro del círculo por <color=green>{incHealthAmountSkill}</color> y aumenta su Ataque en <color=green>{incAttackAmountSkill}</color> y su Defensa en <color=green>{incDefenseAmountSkill}</color> durante <color=yellow>{buffDuration} segundos</color>.\n" +
                      $"<color=yellow>4 pilas</color>: Disminuye la Defensa de las unidades enemigas dentro del círculo en <color=red>{decDefenseAmount}</color> y la Velocidad en <color=red>{decSpeedPer}%</color> durante <color=yellow>{debuffDuration} segundos</color>.\n" +
                      $"(Los efectos pueden acumularse.)";

        JASkillDesc = $"パッシブ: <color=yellow>5秒</color>ごとにベルフェゴールは悪霊を<color=yellow>1個</color>獲得します。\n" +
                      $"悪霊1個ごとに攻撃力が<color=green>{incAttackAmount}</color>、防御力が<color=green>{incDefenseAmount}</color>、移動速度が<color=green>{incSpeedAmount}</color>、攻撃速度が<color=green>{incAttackSpeedAmount}</color>増加します。\n" +
                      $"悪霊が<color=yellow>4個</color>になるか、体力が<color=red>80%</color>以下になるとベルフェゴールが覚醒します。\n" +
                      $"覚醒後、以下の効果が発動します:\n" +
                      $"ベルフェゴールは<color=#AEEAFF>{size}X{size}</color>のサークルを持っています。\n" +
                      $"<color=yellow>0個</color>: サークル内の敵に毎秒攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、与えたダメージの<color=green>{incHealthPer}%</color>分体力を回復します。\n" +
                      $"<color=yellow>2個</color>: サークル内の敵に攻撃力の<color=red>{attackCoeff2}倍</color>のダメージを与えます。（1回）\n" +
                      $"<color=yellow>4個</color>: すべての敵ユニットを<color=yellow>{stunDuration}秒</color>間スタンさせます。（1回）\n" +
                      $"（効果は重複します。）\n\n" +
                      $"悪霊吸収: 悪霊の数に応じて以下の効果が発動します:\n" +
                      $"<color=yellow>0個</color>: スキルなし\n" +
                      $"<color=yellow>2個</color>: サークル内の味方ユニットの体力を<color=green>{incHealthAmountSkill} </color>回復し、<color=yellow>{buffDuration}秒</color>間攻撃力を<color=green>{incAttackAmountSkill}</color>、防御力を<color=green>{incDefenseAmountSkill}</color>増加させます。\n" +
                      $"<color=yellow>4個</color>: サークル内の敵ユニットの防御力を<color=red>{decDefenseAmount}</color>、移動速度を<color=red>{decSpeedPer}%</color>減少させます。<color=yellow>{debuffDuration}秒</color>間。\n" +
                      $"（効果は重複します。）";

        PT_BRSkillDesc = $"Passivo: A cada <color=yellow>5 segundos</color>, Belphegor ganha <color=yellow>1 pilha</color> de Espírito Maligno.\n" +
                         $"Cada pilha aumenta seu Ataque em <color=green>{incAttackAmount}</color>, Defesa em <color=green>{incDefenseAmount}</color>, Velocidade em <color=green>{incSpeedAmount}</color> e Velocidade de Ataque em <color=green>{incAttackSpeedAmount}</color>.\n" +
                         $"Quando ele alcança <color=yellow>4 pilhas</color> ou seu HP cai abaixo de <color=red>80%</color>, Belphegor desperta.\n" +
                         $"Ao despertar, os seguintes efeitos são ativados:\n" +
                         $"Belphegor tem um círculo de tamanho <color=#AEEAFF>{size}X{size}</color>.\n" +
                         $"<color=yellow>0 pilhas</color>: Causa dano igual a <color=red>{attackCoeff}x</color> do seu Ataque aos inimigos dentro do círculo a cada segundo, curando-se por <color=green>{incHealthPer}%</color> do dano causado.\n" +
                         $"<color=yellow>2 pilhas</color>: Causa dano igual a <color=red>{attackCoeff2}x</color> do seu Ataque aos inimigos dentro do círculo. (Uma vez)\n" +
                         $"<color=yellow>4 pilhas</color>: Atordoa todas as unidades inimigas por <color=yellow>{stunDuration} segundos</color>. (Uma vez)\n" +
                         $"(Os efeitos podem se acumular.)\n\n" +
                         $"Absorção de Espírito Maligno: Os seguintes efeitos são ativados com base no número de pilhas:\n" +
                         $"<color=yellow>0 pilhas</color>: Sem habilidade\n" +
                         $"<color=yellow>2 pilhas</color>: Cura unidades aliadas dentro do círculo por <color=green>{incHealthAmountSkill}</color> e aumenta seu Ataque em <color=green>{incAttackAmountSkill}</color> e Defesa em <color=green>{incDefenseAmountSkill}</color> por <color=yellow>{buffDuration} segundos</color>.\n" +
                         $"<color=yellow>4 pilhas</color>: Diminui a Defesa de unidades inimigas dentro do círculo em <color=red>{decDefenseAmount}</color> e Velocidade em <color=red>{decSpeedPer}%</color> por <color=yellow>{debuffDuration} segundos</color>.\n" +
                         $"(Os efeitos podem se acumular.)";

        RUSkillDesc = $"Пассив: Каждые <color=yellow>5 секунд</color> Бельфегор получает <color=yellow>1 стэк</color> Злого Духа.\n" +
                      $"Каждый стэк увеличивает его Атаку на <color=green>{incAttackAmount}</color>, Защиту на <color=green>{incDefenseAmount}</color>, Скорость на <color=green>{incSpeedAmount}</color> и Скорость Атаки на <color=green>{incAttackSpeedAmount}</color>.\n" +
                      $"Когда он достигает <color=yellow>4 стэков</color> или его HP падает ниже <color=red>80%</color>, Бельфегор пробуждается.\n" +
                      $"При пробуждении активируются следующие эффекты:\n" +
                      $"У Бельфегора есть круг размером <color=#AEEAFF>{size}X{size}</color>.\n" +
                      $"<color=yellow>0 стэков</color>: Наносит урон, равный <color=red>{attackCoeff}x</color> его Атаки врагам внутри круга каждую секунду, исцеляя себя на <color=green>{incHealthPer}%</color> от нанесенного урона.\n" +
                      $"<color=yellow>2 стэка</color>: Наносит урон, равный <color=red>{attackCoeff2}x</color> его Атаки врагам внутри круга. (Один раз)\n" +
                      $"<color=yellow>4 стэка</color>: Оглушает все вражеские юниты на <color=yellow>{stunDuration} секунд</color>. (Один раз)\n" +
                      $"(Эффекты могут накапливаться.)\n\n" +
                      $"Поглощение Злого Духа: В зависимости от количества стэков активируются следующие эффекты:\n" +
                      $"<color=yellow>0 стэков</color>: Нет навыка\n" +
                      $"<color=yellow>2 стэка</color>: Исцеляет союзные юниты внутри круга на <color=green>{incHealthAmountSkill}</color> и увеличивает их Атаку на <color=green>{incAttackAmountSkill}</color> и Защиту на <color=green>{incDefenseAmountSkill}</color> на <color=yellow>{buffDuration} секунд</color>.\n" +
                      $"<color=yellow>4 стэка</color>: Уменьшает Защиту вражеских юнитов внутри круга на <color=red>{decDefenseAmount}</color> и Скорость на <color=red>{decSpeedPer}%</color> на <color=yellow>{debuffDuration} секунд</color>.\n" +
                      $"(Эффекты могут накапливаться.)";

        ZH_HANSSkillDesc = $"被动：每隔<color=yellow>5秒</color>，贝尔菲戈尔获得<color=yellow>1层</color>恶灵。\n" +
                           $"每层增加他的攻击力<color=green>{incAttackAmount}</color>，防御力<color=green>{incDefenseAmount}</color>，速度<color=green>{incSpeedAmount}</color>和攻击速度<color=green>{incAttackSpeedAmount}</color>。\n" +
                           $"当他达到<color=yellow>4层</color>或他的生命值低于<color=red>80%</color>时，贝尔菲戈尔觉醒。\n" +
                           $"觉醒时，激活以下效果：\n" +
                           $"贝尔菲戈尔拥有一个大小为<color=#AEEAFF>{size}X{size}</color>的圆圈。\n" +
                           $"<color=yellow>0层</color>：每秒对圆圈内的敌人造成相当于其攻击力<color=red>{attackCoeff}倍</color>的伤害，并根据所造成伤害的<color=green>{incHealthPer}%</color>治疗自己。\n" +
                           $"<color=yellow>2层</color>：对圆圈内的敌人造成相当于其攻击力<color=red>{attackCoeff2}倍</color>的伤害。（一次）\n" +
                           $"<color=yellow>4层</color>：使所有敌方单位眩晕<color=yellow>{stunDuration}秒</color>。（一次）\n" +
                           $"（效果可以叠加。）\n\n" +
                           $"恶灵吸收：根据层数激活以下效果：\n" +
                           $"<color=yellow>0层</color>：无技能\n" +
                           $"<color=yellow>2层</color>：治疗圆圈内的友方单位<color=green>{incHealthAmountSkill}</color>，并在<color=yellow>{buffDuration}秒</color>内增加其攻击力<color=green>{incAttackAmountSkill}</color>和防御力<color=green>{incDefenseAmountSkill}</color>。\n" +
                           $"<color=yellow>4层</color>：降低圆圈内敌方单位的防御力<color=red>{decDefenseAmount}</color>和速度<color=red>{decSpeedPer}%</color>。\n" +
                           $"（效果可以叠加。）";
    }
}