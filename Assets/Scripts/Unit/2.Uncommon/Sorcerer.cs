using UnityEngine;
using UnityEngine.Localization.Settings;

public class Sorcerer : UnitDefault
{
    [Header("프리즘 레이")]
    public float attackCoeff;
    public float burnDamage;
    public int count;
    public float size;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float BurnDamage { get => burnDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 5; }
    float Size { get => size + StaticManager.skillPoint[unitIdx] / 3 * 0.5f; }

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(Size, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        SoundPlay(skillSoundClip);
        CreateAttackBox(AttackPower * AttackCoeff, null, pos, AttackStyle.range);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;
        Vector2 thisPos = Vector2.zero;
        Vector2 dir = Vector2.zero;
        float angle = 0f;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill)) {
            thisPos = transform.position;

            // 타겟 방향 계산
            dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, isSkill ? thisPos : targetPos, rotation);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1 : 1;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        if (!isSkill)
            scale *= attackSize; // 스프라이트 크기 조정
        else
            scale = new Vector2(Size, 3f);
        effect.transform.localScale = scale;

        // 박스 캐스트 수행 (회전 적용)
        Collider2D[] hitEnemies;
        if (!isSkill)
            hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize, LayerMask.GetMask(enemyTag));
        else
            hitEnemies = Physics2D.OverlapBoxAll(thisPos + dir * effect.transform.localScale.x / 2f, new Vector2(Size, 1f), angle, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            if (attackStyle == AttackStyle.single) {
                foreach (Collider2D enemy in hitEnemies) {
                    if (enemy.gameObject == target) {
                        damage = target.GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
                        break;
                    }
                }
            }
            else {
                for (int i = hitEnemies.Length - 1; i >= 0; i--) { 
                    UnitDefault unitComp = hitEnemies[i].GetComponent<UnitDefault>();
                    unitComp.Burn(BurnDamage, count, this, dealMetricsIdx);
                    damage += unitComp.Hit(attackPower, CriticalPer, this, isPenetration);
                }
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
        skillDesc = $"공격 중인 대상에게 <color=#AEEAFF>{Size}X1</color>크기의 프리즘 레이를 발사해 피격한 적들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, <color=yellow>{Count}초</color> 동안 화상을 입힙니다.\n" +
                    $"초당 화상 데미지: <color=red>{BurnDamage}</color>";

        ENSkillDesc = $"Fires a prism ray of size <color=#AEEAFF>{Size}X1</color> at the attacking target, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power to hit enemies and inflicting burns for <color=yellow>{Count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{BurnDamage}</color>";

        FRSkillDesc = $"Tire un rayon prisme de taille <color=#AEEAFF>{Size}X1</color> sur la cible attaquée, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque aux ennemis touchés et leur infligeant des brûlures pendant <color=yellow>{Count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{BurnDamage}</color>";

        ITSkillDesc = $"Spara un raggio prisma di dimensioni <color=#AEEAFF>{Size}X1</color> al bersaglio attaccante, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco ai nemici colpiti e infliggendo ustioni per <color=yellow>{Count} secondi</color>.\n" +
                      $"Danno da ustione al secondo: <color=red>{BurnDamage}</color>";

        DESkillDesc = $"Feuert einen Prisma-Strahl der Größe <color=#AEEAFF>{Size}X1</color> auf das angreifende Ziel ab, verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft an getroffenen Feinden und fügt für <color=yellow>{Count} Sekunden</color> Verbrennungen zu.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{BurnDamage}</color>";

        ESSkillDesc = $"Dispara un rayo prisma de tamaño <color=#AEEAFF>{Size}X1</color> al objetivo atacado, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque a los enemigos alcanzados e infligiendo quemaduras durante <color=yellow>{Count} segundos</color>.\n" +
                      $"Daño de quemadura por segundo: <color=red>{BurnDamage}</color>";

        JASkillDesc = $"攻撃中の対象に<color=#AEEAFF>{Size}X1</color>サイズのプリズムレイを発射し、命中した敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、<color=yellow>{Count}秒</color>間火傷を負わせます。\n" +
                      $"火伤的秒间伤害：<color=red>{BurnDamage}</color>";

        PT_BRSkillDesc = $"Dispara um raio prisma de tamanho <color=#AEEAFF>{Size}X1</color> no alvo atacante, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque aos inimigos atingidos e causando queimaduras por <color=yellow>{Count} segundos</color>.\n" +
                         $"Dano por queimadura por segundo: <color=red>{BurnDamage}</color>";

        RUSkillDesc = $"Выстреливает призмальным лучом размером <color=#AEEAFF>{Size}X1</color> в атакующую цель, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки пораженным врагам и вызывая ожоги на <color=yellow>{Count} секунд</color>.\n" +
                      $"Урон от ожога в секунду: <color=red>{BurnDamage}</color>";

        ZH_HANSSkillDesc = $"向正在攻击的目标发射<color=#AEEAFF>{Size}X1</color>大小的棱镜光线，对命中的敌人造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，并在<color=yellow>{Count}秒</color>内造成灼烧效果。\n" +
                           $"每秒灼烧伤害：<color=red>{BurnDamage}</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "초당 화상 데미지 <color=red>+5</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "길이 <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "화상 지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "Burn damage per second <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Length <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Burn duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Dégâts de brûlure par seconde <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Longueur <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée brûlure <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Danno da bruciatura al secondo <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Lunghezza <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata bruciatura <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Brandschaden pro Sekunde <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Länge <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Branddauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Daño por quemadura por segundo <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Longitud <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración quemadura <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "秒間火傷ダメージ <color=red>+5</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "長さ <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "火傷持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Dano de queimadura por segundo <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Comprimento <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração queimadura <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "Урон от ожога в секунду <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Длина <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность ожога <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "每秒灼烧伤害 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "长度 <color=#AEEAFF>+0.5</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "灼烧持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "Burn damage per second <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Length <color=#AEEAFF>+0.5</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Burn duration <color=yellow>+1</color>";
    }
}