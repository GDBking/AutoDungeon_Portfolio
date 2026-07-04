using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Lucifer : UnitDefault
{
    [Header("패시브")]
    public float attackCoeff;
    public float decAttackAmount;
    public float decDefenseAmount;
    public float burnDamage;
    public int count;
    public GameObject bladeState;
    public Image bladeCooldownImg;
    public AnimationClip bladeHitAnimClip;
    [Header("타락의 검")]
    public LuciferBlade bladePrefab;
    public float skillAttackCoeff;
    public float incAttackAmount;
    public float incDefenseAmount;
    public float incAttackSpeedAmount;
    public float incSpeedAmount;
    public float incCriticalPer;
    public float incLifeStealPer;
    public float duration;

    static public float DecAttackAmount;
    static public float DecDefenseAmount;

    const float minX = -7.5f;
    const float maxX = 7.5f;
    const float minY = -4f;
    const float maxY = 5f;
    float elapsedTime;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;

        DecAttackAmount = decAttackAmount;
        DecDefenseAmount = decDefenseAmount;
    }

    protected override void Start()
    {
        base.Start();

        bladeState.SetActive(true);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        bladeCooldownImg.fillAmount = elapsedTime / 3f;

        if (elapsedTime >= 3f && friends.Count > 0) {
            elapsedTime = 0f;

            RandomEnemyUnit(friends);
            StartCoroutine(OnFire(SkillTarget, skillTargetPos));
        }
    }

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (friends.Count == 0)
            return;

        LuciferBuff();

        foreach (UnitDefault enemy in friends) {
            for (int i = 0; i < enemy.itemStat.level; i++) {
                int XY = Random.Range(0, 2);
                float X, Y;
                if (XY == 0) {
                    float temp = Random.Range(0, 2);
                    X = temp == 0 ? minX : maxX;
                    Y = Random.Range(minY, maxY);
                }
                else {
                    float temp = Random.Range(0, 2);
                    Y = temp == 0 ? minY : maxY;
                    X = Random.Range(minX, maxX);
                }
                Vector2 randPos = new(X, Y);

                Vector2 dir = ((Vector2)enemy.transform.position - randPos).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                LuciferBlade blade = Instantiate(bladePrefab, randPos, rotation, GameManager.instance.skillEffect);
                blade.unitComp = this;
            }
        }
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();

        if (++unitComp.itemStat.level == 3) {
            unitComp.itemStat.level = 0;

            CreateAttackBox(AttackPower * attackCoeff, target, target.transform.position);
            unitComp.LuciferDebuff(false);
        }
        else {
            GameObject effect = Instantiate(attackObj, target.transform.position, Quaternion.identity);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(bladeHitAnimClip);
            effect.transform.localScale *= attackSize;

            unitComp.Burn(burnDamage, count, this);
            unitComp.LuciferDebuff(false);
        }
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = targetPos + Vector2.up * 5f; // 시작 위치

        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation, target.transform);
        bulletList.Add(bullet);
        bullet.tag = "Skill Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(unitComp.itemStat.level == 2 ? skillBulletAnimClip : bulletAnimClip, attackSize);
        bulletComp.unitDefault = this;
        bulletComp.target = target;
        if (isPenetration)
            bulletComp.enemyTag = enemyTag;

        SoundPlay(skillSoundClip);

        WaitForFixedUpdate wait = new();
        // 발사체가 타겟으로 이동하는 로직
        while (bullet != null && bullet.transform.localPosition.y > 0f) {
            // 발사체의 위치 계산 (Lerp 사용)
            bullet.transform.Translate(ShotSpeed * Time.fixedDeltaTime * Vector2.down, Space.World);

            // 다음 프레임 대기
            yield return wait;
        }

        if (bullet != null) {
            Destroy(bullet);

            OnSkillFire(target);
        }
    }

    public void LuciferBuff()
    {
        SetStat(attackPowerStat, incAttackAmount, false, true, duration);
        SetStat(defenseStat, incDefenseAmount, false, true, duration);
        SetStat(attackSpeedStat, incAttackSpeedAmount, false, true, duration);
        SetStat(speedStat, incSpeedAmount, false, true, duration);
        SetStat(criticalPerStat, incCriticalPer, false, true, duration);
        SetStat(lifeStealPerStat, incLifeStealPer, false, true, duration);

        SetStateBar(State.luciferBuff, duration);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"패시브: <color=yellow>3초</color>마다 무작위 적 유닛 한 명에게 칼날을 떨어뜨려 공격력을 <color=red>{decAttackAmount}</color>, 방어력을 <color=red>{decDefenseAmount}</color>만큼 감소시키며, <color=yellow>{count}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{burnDamage}</color>\n" +
                    $"(디버프는 중첩되며 리스폰 시에도 사라지지 않습니다. <color=yellow>3스택</color>이 쌓이면 공격력 <color=red>{attackCoeff}배</color>의 피해를 입히고 스택이 사라집니다.)\n\n" +
                    $"타락의 검: 모든 적 유닛들의 스택 개수만큼 맵 끝에서 칼날이 날아옵니다.\n" +
                    $"칼날은 궤적을 남기며 잠시 뒤 궤적에 닿아 있는 적들은 공격력 <color=red>{skillAttackCoeff}배</color>의 피해를 입습니다.\n" +
                    $"또한 <color=yellow>{duration}초</color> 동안 본인의 모든 스탯이 증가합니다.\n" +
                    $"공격력 <color=green>+{incAttackAmount}</color>\n" +
                    $"방어력 <color=green>+{incDefenseAmount}</color>\n" +
                    $"공격 속도 <color=green>+{incAttackSpeedAmount}</color>\n" +
                    $"이동 속도 <color=green>+{incSpeedAmount}</color>\n" +
                    $"치명타 확률 <color=green>+{incCriticalPer}%p</color>\n" +
                    $"피해 흡혈 <color=green>+{incLifeStealPer}%p</color>";
        
        ENSkillDesc = $"Passive: Every <color=yellow>3 seconds</color>, drops a blade on a random enemy unit, reducing its attack power by <color=red>{decAttackAmount}</color> and defense by <color=red>{decDefenseAmount}</color>, and inflicting burn for <color=yellow>{count} seconds</color>.\n" +
                       $"Burn damage per second: <color=red>{burnDamage}</color>\n" +
                       $"(Debuffs stack and do not disappear upon respawn. When <color=yellow>3 stacks</color> are accumulated, deals damage equal to <color=red>{attackCoeff} times</color> the attack power and the stacks disappear.)\n\n" +
                       $"Blade of Corruption: Blades fly in from the edges of the map equal to the number of stacks on all enemy units.\n" +
                       $"The blades leave a trail, and enemies touching the trail after a short delay take damage equal to <color=red>{skillAttackCoeff} times</color> the attack power.\n" +
                       $"Also increases all own stats for <color=yellow>{duration} seconds</color>.\n" +
                       $"Attack Power <color=green>+{incAttackAmount}</color>\n" +
                       $"Defense <color=green>+{incDefenseAmount}</color>\n" +
                       $"Attack Speed <color=green>+{incAttackSpeedAmount}</color>\n" +
                       $"Movement Speed <color=green>+{incSpeedAmount}</color>\n" +
                       $"Critical Hit Rate <color=green>+{incCriticalPer}%p</color>\n" +
                       $"Life Steal <color=green>+{incLifeStealPer}%p</color>";

        FRSkillDesc = $"Passif : Toutes les <color=yellow>3 secondes</color>, une lame tombe sur une unité ennemie aléatoire, réduisant son attaque de <color=red>{decAttackAmount}</color> et sa défense de <color=red>{decDefenseAmount}</color>, et infligeant une brûlure pendant <color=yellow>{count} secondes</color>.\n" +
                     $"Dégâts de brûlure par seconde : <color=red>{burnDamage}</color>\n" +
                     $"(Les débuffs s'empilent et ne disparaissent pas lors de la réapparition. Lorsqu'<color=yellow>3 stacks</color> sont accumulés, inflige des dégâts égaux à <color=red>{attackCoeff} fois</color> l'attaque et les stacks disparaissent.)\n\n" +
                     $"Lame de la Corruption : Des lames volent depuis les bords de la carte en fonction du nombre de stacks sur toutes les unités ennemies.\n" +
                     $"Les lames laissent une traînée, et les ennemis touchant la traînée après un court délai subissent des dégâts égaux à <color=red>{skillAttackCoeff} fois</color> l'attaque.\n" +
                     $"Augmente également toutes les statistiques propres pendant <color=yellow>{duration} secondes</color>.\n" +
                     $"Puissance d'Attaque <color=green>+{incAttackAmount}</color>\n" +
                     $"Défense <color=green>+{incDefenseAmount}</color>\n" +
                     $"Vitesse d'Attaque <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Vitesse de Déplacement <color=green>+{incSpeedAmount}</color>\n" +
                     $"Taux de Coup Critique <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Vol de Vie <color=green>+{incLifeStealPer}%p</color>";

        ITSkillDesc = $"Passivo: Ogni <color=yellow>3 secondi</color>, una lama cade su un'unità nemica casuale, riducendo il suo attacco di <color=red>{decAttackAmount}</color> e la difesa di <color=red>{decDefenseAmount}</color>, infliggendo bruciature per <color=yellow>{count} secondi</color>.\n" +
                     $"Danno da bruciatura al secondo: <color=red>{burnDamage}</color>\n" +
                     $"(I debuff si accumulano e non scompaiono al respawn. Quando si accumulano <color=yellow>3 stack</color>, infligge danni pari a <color=red>{attackCoeff} volte</color> l'attacco e gli stack scompaiono.)\n\n" +
                     $"Lama della Corruzione: Lame volano dai bordi della mappa in base al numero di stack su tutte le unità nemiche.\n" +
                     $"Le lame lasciano una scia, e i nemici che toccano la scia dopo un breve ritardo subiscono danni pari a <color=red>{skillAttackCoeff} volte</color> l'attacco.\n" +
                     $"Inoltre aumenta tutte le proprie statistiche per <color=yellow>{duration} secondi</color>.\n" +
                     $"Potenza d'Attacco <color=green>+{incAttackAmount}</color>\n" +
                     $"Difesa <color=green>+{incDefenseAmount}</color>\n" +
                     $"Velocità d'Attacco <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Velocità di Movimento <color=green>+{incSpeedAmount}</color>\n" +
                     $"Probabilità di Colpo Critico <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Rubavita <color=green>+{incLifeStealPer}%p</color>";

        DESkillDesc = $"Passiv : Alle <color=yellow>3 Sekunden</color> fällt eine Klinge auf eine zufällige feindliche Einheit, die deren Angriff um <color=red>{decAttackAmount}</color> und Verteidigung um <color=red>{decDefenseAmount}</color> reduziert und für <color=yellow>{count} Sekunden</color> Verbrennungen verursacht.\n" +
                     $"Verbrennungs-Schaden pro Sekunde: <color=red>{burnDamage}</color>\n" +
                     $"(Debuffs stapeln sich und verschwinden nicht beim Respawn. Wenn <color=yellow>3 Stacks</color> angesammelt sind, wird Schaden in Höhe von <color=red>{attackCoeff} mal</color> dem Angriff verursacht und die Stacks verschwinden.)\n\n" +
                     $"Klinge der Korruption: Klingen fliegen von den Rändern der Karte entsprechend der Anzahl der Stacks auf allen feindlichen Einheiten.\n" +
                     $"Die Klingen hinterlassen eine Spur, und Feinde, die die Spur nach einer kurzen Verzögerung berühren, erleiden Schaden in Höhe von <color=red>{skillAttackCoeff} mal</color> dem Angriff.\n" +
                     $"Erhöht außerdem alle eigenen Werte für <color=yellow>{duration} Sekunden</color>.\n" +
                     $"Angriffskraft <color=green>+{incAttackAmount}</color>\n" +
                     $"Verteidigung <color=green>+{incDefenseAmount}</color>\n" +
                     $"Angriffsgeschwindigkeit <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Bewegungsgeschwindigkeit <color=green>+{incSpeedAmount}</color>\n" +
                     $"Kritische Trefferchance <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Lebensraub <color=green>+{incLifeStealPer}%p</color>";

        ESSkillDesc = $"Pasivo: Cada <color=yellow>3 segundos</color>, una hoja cae sobre una unidad enemiga aleatoria, reduciendo su poder de ataque en <color=red>{decAttackAmount}</color> y su defensa en <color=red>{decDefenseAmount}</color>, e infligiendo quemaduras durante <color=yellow>{count} segundos</color>.\n" +
                     $"Daño por quemaduras por segundo: <color=red>{burnDamage}</color>\n" +
                     $"(Los debuffs se acumulan y no desaparecen al reaparecer. Cuando se acumulan <color=yellow>3 stacks</color>, inflige daño igual a <color=red>{attackCoeff} veces</color> el poder de ataque y los stacks desaparecen.)\n\n" +
                     $"Hoja de la Corrupción: Hojas vuelan desde los bordes del mapa según el número de stacks en todas las unidades enemigas.\n" +
                     $"Las hojas dejan un rastro, y los enemigos que tocan el rastro después de un breve retraso reciben daño igual a <color=red>{skillAttackCoeff} veces</color> el poder de ataque.\n" +
                     $"También aumenta todas sus propias estadísticas durante <color=yellow>{duration} segundos</color>.\n" +
                     $"Poder de Ataque <color=green>+{incAttackAmount}</color>\n" +
                     $"Defensa <color=green>+{incDefenseAmount}</color>\n" +
                     $"Velocidad de Ataque <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Velocidad de Movimiento <color=green>+{incSpeedAmount}</color>\n" +
                     $"Probabilidad de Golpe Crítico <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Robo de Vida <color=green>+{incLifeStealPer}%p</color>";

        JASkillDesc = $"パッシブ: <color=yellow>3秒</color>ごとにランダムな敵ユニットに刃を落とし、攻撃力を<color=red>{decAttackAmount}</color>、防御力を<color=red>{decDefenseAmount}</color>減少させ、<color=yellow>{count}秒</color>間火傷を負わせます。\n" +
                     $"1秒あたりの火傷ダメージ: <color=red>{burnDamage}</color>\n" +
                     $"（デバフは重複し、リスポーン時にも消えません。<color=yellow>3スタック</color>がたまると、攻撃力の<color=red>{attackCoeff}倍</color>のダメージを与え、スタックが消えます。）\n\n" +
                     $"堕落の剣: すべての敵ユニットのスタック数に応じて、マップの端から刃が飛んできます。\n" +
                     $"刃は軌跡を残し、短時間後に軌跡に触れた敵は攻撃力の<color=red>{skillAttackCoeff}倍</color>のダメージを受けます。\n" +
                     $"また、<color=yellow>{duration}秒</color>間、自身のすべてのステータスが上昇します。\n" +
                     $"攻撃力 <color=green>+{incAttackAmount}</color>\n" +
                     $"防御力 <color=green>+{incDefenseAmount}</color>\n" +
                     $"攻撃速度 <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"移動速度 <color=green>+{incSpeedAmount}</color>\n" +
                     $"クリティカル確率 <color=green>+{incCriticalPer}%p</color>\n" +
                     $"ライフスティール <color=green>+{incLifeStealPer}%p</color>";

        PT_BRSkillDesc = $"Passivo: A cada <color=yellow>3 segundos</color>, uma lâmina cai sobre uma unidade inimiga aleatória, reduzindo seu poder de ataque em <color=red>{decAttackAmount}</color> e defesa em <color=red>{decDefenseAmount}</color>, e causando queimadura por <color=yellow>{count} segundos</color>.\n" +
                     $"Dano de queimadura por segundo: <color=red>{burnDamage}</color>\n" +
                     $"(Os debuffs se acumulam e não desaparecem ao ressurgir. Quando <color=yellow>3 stacks</color> são acumulados, causa dano igual a <color=red>{attackCoeff} vezes</color> o poder de ataque e os stacks desaparecem.)\n\n" +
                     $"Lâmina da Corrupção: Lâminas voam das bordas do mapa de acordo com o número de stacks em todas as unidades inimigas.\n" +
                     $"As lâminas deixam um rastro, e os inimigos que tocam o rastro após um breve atraso sofrem dano igual a <color=red>{skillAttackCoeff} vezes</color> o poder de ataque.\n" +
                     $"Também aumenta todas as próprias estatísticas por <color=yellow>{duration} segundos</color>.\n" +
                     $"Poder de Ataque <color=green>+{incAttackAmount}</color>\n" +
                     $"Defesa <color=green>+{incDefenseAmount}</color>\n" +
                     $"Velocidade de Ataque <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Velocidade de Movimento <color=green>+{incSpeedAmount}</color>\n" +
                     $"Taxa de Acerto Crítico <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Roubo de Vida <color=green>+{incLifeStealPer}%p</color>";

        RUSkillDesc = $"Пассив: Каждые <color=yellow>3 секунды</color> на случайную вражескую единицу падает клинок, уменьшая ее атаку на <color=red>{decAttackAmount}</color> и защиту на <color=red>{decDefenseAmount}</color>, а также нанося ожог на <color=yellow>{count} секунд</color>.\n" +
                     $"Урон от ожога в секунду: <color=red>{burnDamage}</color>\n" +
                     $"(Дебаффы накапливаются и не исчезают при возрождении. Когда накапливается <color=yellow>3 стека</color>, наносится урон, равный <color=red>{attackCoeff}-кратной</color> атаке, и стеки исчезают.)\n\n" +
                     $"Клинок Порчи: Клинки летят с краев карты в соответствии с количеством стеков на всех вражеских единицах.\n" +
                     $"Клинки оставляют след, и враги, касающиеся следа после короткой задержки, получают урон, равный <color=red>{skillAttackCoeff}-кратной</color> атаке.\n" +
                     $"Кроме того, все собственные характеристики увеличиваются на <color=yellow>{duration} секунд</color>.\n" +
                     $"Атака <color=green>+{incAttackAmount}</color>\n" +
                     $"Защита <color=green>+{incDefenseAmount}</color>\n" +
                     $"Скорость атаки <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"Скорость передвижения <color=green>+{incSpeedAmount}</color>\n" +
                     $"Шанс критического удара <color=green>+{incCriticalPer}%p</color>\n" +
                     $"Похищение жизни <color=green>+{incLifeStealPer}%p</color>";

        ZH_HANSSkillDesc = $"被动：每隔<color=yellow>3秒</color>，对一个随机敌方单位投下利刃，降低其攻击力<color=red>{decAttackAmount}</color>和防御力<color=red>{decDefenseAmount}</color>，并造成<color=yellow>{count}秒</color>的灼烧效果。\n" +
                     $"每秒灼烧伤害：<color=red>{burnDamage}</color>\n" +
                     $"（减益效果可叠加，并且在重生时不会消失。当积累到<color=yellow>3层</color>时，会造成相当于攻击力<color=red>{attackCoeff}倍</color>的伤害，层数消失。）\n\n" +
                     $"堕落之剑：根据所有敌方单位身上的层数，从地图边缘飞来利刃。\n" +
                     $"利刃会留下轨迹，在短暂延迟后触碰轨迹的敌人会受到相当于攻击力<color=red>{skillAttackCoeff}倍</color>的伤害。\n" +
                     $"此外，所有自身属性在<color=yellow>{duration}秒</color>内提升。\n" +
                     $"攻击力 <color=green>+{incAttackAmount}</color>\n" +
                     $"防御力 <color=green>+{incDefenseAmount}</color>\n" +
                     $"攻击速度 <color=green>+{incAttackSpeedAmount}</color>\n" +
                     $"移动速度 <color=green>+{incSpeedAmount}</color>\n" +
                     $"暴击率 <color=green>+{incCriticalPer}%p</color>\n" +
                     $"生命偷取 <color=green>+{incLifeStealPer}%p</color>";
    }
}