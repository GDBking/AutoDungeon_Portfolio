using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Stinger : UnitDefault
{
    [Header("독화살")]
    public float attackCoeff;
    public float poisonDamage;
    public float size;
    public int count;
    public GameObject poisonPoolPrefab;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.2f; }
    public float PoisonDamage { get => poisonDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    public float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.2f; }
    public int Count { get => count + StaticManager.skillPoint[unitIdx] / 5; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
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

        StartCoroutine(OnFire(SkillTarget, pos));
    }

    void OnSkillFire(GameObject target)
    {
        GameObject effect = Instantiate(poisonPoolPrefab, target.transform.position, Quaternion.identity, GameManager.instance.skillEffect);
        PoisonPool poisonComp = effect.GetComponent<PoisonPool>();
        poisonComp.unitComp = this;

        CreateAttackBox(AttackPower * AttackCoeff, target, target.transform.position);
    }

    public override IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = transform.position; // 시작 위치
        if (isPenetration)
            targetPos = startPos + (targetPos - startPos).normalized * 20f;

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

            if (isSkill) {
                GameObject effect = Instantiate(poisonPoolPrefab, targetPos, Quaternion.identity, GameManager.instance.skillEffect);
                PoisonPool poisonComp = effect.GetComponent<PoisonPool>();
                poisonComp.unitComp = this;
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 독화살을 날려 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, <color=#AEEAFF>{Size}X{Size}</color>크기의 독안개를 <color=yellow>{Count}초</color> 동안 퍼뜨립니다.\n" +
                    $"초당 독 데미지: <color=red>{PoisonDamage}</color>";
        
        ENSkillDesc = $"Shoots a poison dart at the target being attacked, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power and spreading a poison mist of size <color=#AEEAFF>{Size}X{Size}</color> for <color=yellow>{Count} seconds</color>.\n" +
                      $"Poison damage per second: <color=red>{PoisonDamage}</color>";

        FRSkillDesc = $"Tire un dard empoisonné sur la cible attaquée, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque et répandant une brume de poison de taille <color=#AEEAFF>{Size}X{Size}</color> pendant <color=yellow>{Count} secondes</color>.\n" +
                      $"Dégâts de poison par seconde : <color=red>{PoisonDamage}</color>";

        ITSkillDesc = $"Spara un dardo avvelenato al bersaglio attaccato, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco e diffondendo una nebbia velenosa di dimensioni <color=#AEEAFF>{Size}X{Size}</color> per <color=yellow>{Count} secondi</color>.\n" +
                      $"Danno da veleno al secondo: <color=red>{PoisonDamage}</color>";
            
        DESkillDesc = $"Schießt einen Giftpfeil auf das angegriffene Ziel, verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft und verbreitet für <color=yellow>{Count} Sekunden</color> einen Giftnebel der Größe <color=#AEEAFF>{Size}X{Size}</color>.\n" +
                      $"Giftschaden pro Sekunde: <color=red>{PoisonDamage}</color>";

        ESSkillDesc = $"Dispara un dardo venenoso al objetivo atacado, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque y esparciendo una niebla venenosa de tamaño <color=#AEEAFF>{Size}X{Size}</color> durante <color=yellow>{Count} segundos</color>.\n" +
                      $"Daño de veneno por segundo: <color=red>{PoisonDamage}</color>";

        JASkillDesc = $"攻撃中の対象に毒矢を放ち、攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、<color=#AEEAFF>{Size}X{Size}</color>の大きさの毒霧を<color=yellow>{Count}秒</color>間広げます。\n" +
                      $"秒間毒ダメージ: <color=red>{PoisonDamage}</color>";

        PT_BRSkillDesc = $"Atira um dardo venenoso no alvo atacado, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque e espalhando uma névoa venenosa de tamanho <color=#AEEAFF>{Size}X{Size}</color> por <color=yellow>{Count} segundos</color>.\n" +
                         $"Dano por veneno por segundo: <color=red>{PoisonDamage}</color>";

        RUSkillDesc = $"Выстреливает ядовитой стрелой в атакуемую цель, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки, и распространяя ядовитый туман размером <color=#AEEAFF>{Size}X{Size}</color> в течение <color=yellow>{Count} секунд</color>.\n" +
                      $"Урон от яда в секунду: <color=red>{PoisonDamage}</color>";

        ZH_HANSSkillDesc = $"向正在攻击的目标射出毒箭，造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，并在<color=yellow>{Count}秒</color>内散布<color=#AEEAFF>{Size}X{Size}</color>大小的毒雾。\n" +
                           $"每秒毒伤害: <color=red>{PoisonDamage}</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.2</color>\n" +
                   "독 데미지 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.2</color>\n" +
                   "Poison damage <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.2</color>\n" +
                   "Dégâts de poison <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Durée <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.2</color>\n" +
                   "Danno da veleno <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Durata <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.2</color>\n" +
                   "Gift-Schaden <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Dauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Daño de veneno <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Duración <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.2</color>\n" +
                   "毒ダメージ <color=red>+5</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Dano de veneno <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Duração <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.2</color>\n" +
                   "Ядовитый урон <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Длительность <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.2</color>\n" +
                   "毒伤害 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.2</color>\n" +
               "Poison damage <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.2</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Duration <color=yellow>+1</color>";
    }
}