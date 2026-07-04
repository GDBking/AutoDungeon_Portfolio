using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class FireWizard : UnitDefault
{
    [Header("메테오")]
    public float attackCoeff;
    public float burnDamage;
    public float size;
    public int count;
    public GameObject fireFieldPrefab;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    public float BurnDamage { get => burnDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    public float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.2f; }
    public int Count { get => count + StaticManager.skillPoint[unitIdx] / 5; }

    protected override void Awake()
    {
        base.Awake();

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

        StartCoroutine(OnFire(null, pos));
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
                GameObject effect = Instantiate(fireFieldPrefab, targetPos, Quaternion.identity, GameManager.instance.skillEffect);

                attackSize = Size;
                CreateAttackBox(AttackPower * AttackCoeff, null, targetPos, AttackStyle.range);
                attackSize = 0.5f;

                FireField fireComp = effect.GetComponent<FireField>();
                fireComp.unitComp = this;
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 위치에 메테오를 떨어뜨려 반경 <color=#AEEAFF>{Size}X{Size}</color>범위에 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, 해당 범위는 <color=yellow>{count}초</color> 동안 화염 지형이 됩니다.\n" +
                    $"초당 화상 데미지: <color=red>{BurnDamage}</color>";
        
        ENSkillDesc = $"Drops a meteor on the location of the attacking target, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power in a <color=#AEEAFF>{Size}X{Size}</color> area, which becomes a fiery terrain for <color=yellow>{count} seconds</color>.\n" +
                      $"Burn damage per second: <color=red>{BurnDamage}</color>";

        FRSkillDesc = $"Fait tomber une météore à l'emplacement de la cible attaquante, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque dans une zone de <color=#AEEAFF>{Size}X{Size}</color>, qui devient un terrain enflammé pendant <color=yellow>{count} secondes</color>.\n" +
                      $"Dégâts de brûlure par seconde : <color=red>{BurnDamage}</color>";

        ITSkillDesc = $"Fa cadere una meteora sulla posizione del bersaglio attaccante, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco in un'area di <color=#AEEAFF>{Size}X{Size}</color>, che diventa un terreno infuocato per <color=yellow>{count} secondi</color>.\n" +
                      $"Danno da ustione al secondo: <color=red>{BurnDamage}</color>";

        DESkillDesc = $"Lässt einen Meteor an der Position des angreifenden Ziels fallen, der in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft verursacht, der für <color=yellow>{count} Sekunden</color> zu einem feurigen Terrain wird.\n" +
                      $"Brandschaden pro Sekunde: <color=red>{BurnDamage}</color>";

        ESSkillDesc = $"Deja caer un meteorito en la ubicación del objetivo atacante, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque en un área de <color=#AEEAFF>{Size}X{Size}</color>, que se convierte en un terreno ardiente durante <color=yellow>{count} segundos</color>.\n" +
                      $"Daño por quemadura por segundo: <color=red>{BurnDamage}</color>";

        JASkillDesc = $"攻撃中の対象の位置にメテオを落とし、<color=#AEEAFF>{Size}X{Size}</color>範囲に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、その範囲は<color=yellow>{count}秒</color>間、火炎地形になります。\n" +
                      $"秒間火傷ダメージ: <color=red>{BurnDamage}</color>";

        PT_BRSkillDesc = $"Deixa cair um meteoro na localização do alvo atacante, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque em uma área de <color=#AEEAFF>{Size}X{Size}</color>, que se torna um terreno flamejante por <color=yellow>{count} segundos</color>.\n" +
                         $"Dano por queimadura por segundo: <color=red>{BurnDamage}</color>";

        RUSkillDesc = $"Роняет метеор на место нахождения атакующей цели, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки в области <color=#AEEAFF>{Size}X{Size}</color>, которая становится огненной территорией на <color=yellow>{count} секунд</color>.\n" +
                      $"Урон от ожога в секунду: <color=red>{BurnDamage}</color>";

        ZH_HANSSkillDesc = $"在正在攻击的目标位置上投下陨石，在<color=#AEEAFF>{Size}X{Size}</color>范围内造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，该范围在<color=yellow>{count}秒</color>内变成炽热地形。\n" +
                           $"每秒灼烧伤害: <color=red>{BurnDamage}</color>";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "화상 데미지 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "Burn damage <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Dégâts de brûlure <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Danno da bruciatura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Brandschaden <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Daño por quemadura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "火傷ダメージ <color=red>+5</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Dano de queimadura <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "Урон от ожога <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "灼烧伤害 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "Burn damage <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.2</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+1</color>";
    }
}