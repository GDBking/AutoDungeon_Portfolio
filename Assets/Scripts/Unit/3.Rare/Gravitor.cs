using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Gravitor : UnitDefault
{
    [Header("블랙홀")]
    public float attackCoeff;
    public float dotDamage;
    public float size;
    public int count;
    public GameObject gravityFieldPrefab;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.1f; }
    public float DotDamage { get => dotDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    public float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.2f; }
    public int Count { get => count + StaticManager.skillPoint[unitIdx] / 6; }

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
        while (elapsedTime < duration)
        {
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

        if (bullet != null)
        {
            // 마지막 위치를 타겟의 위치로 설정 (정확한 위치 보정)
            bullet.transform.position = targetPos;

            // 발사체 제거
            Destroy(bullet);

            if (isSkill) {
                GameObject effect = Instantiate(gravityFieldPrefab, targetPos, Quaternion.identity, GameManager.instance.skillEffect);

                attackSize = size;
                CreateAttackBox(AttackPower * AttackCoeff, null, targetPos, AttackStyle.range);
                attackSize = 0.5f;

                GravityField gravityComp = effect.GetComponent<GravityField>();
                gravityComp.unitComp = this;
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상의 위치에 구를 던져 <color=#AEEAFF>{Size}X{Size}</color>크기의 블랙홀을 생성하여 적들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 입히고, <color=yellow>{Count}초</color> 동안 적들을 블랙홀의 중심으로 끌어당기며 매 초 <color=red>{DotDamage}</color>의 피해를 입힙니다.";
        
        ENSkillDesc = $"Throws a sphere at the location of the attacking target to create a black hole of size <color=#AEEAFF>{Size}X{Size}</color>, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power to enemies, pulling them toward the center of the black hole for <color=yellow>{Count} seconds</color> and dealing <color=red>{DotDamage}</color> damage per second.";

        FRSkillDesc = $"Lance une sphère à l'emplacement de la cible attaquée pour créer un trou noir de taille <color=#AEEAFF>{Size}X{Size}</color>, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque aux ennemis, les attirant vers le centre du trou noir pendant <color=yellow>{Count} secondes</color> et infligeant <color=red>{DotDamage}</color> dégâts par seconde.";

        ITSkillDesc = $"Lancia una sfera nella posizione del bersaglio attaccante per creare un buco nero di dimensioni <color=#AEEAFF>{Size}X{Size}</color>, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco ai nemici, attirandoli verso il centro del buco nero per <color=yellow>{Count} secondi</color> e infliggendo <color=red>{DotDamage}</color> danni al secondo.";

        DESkillDesc = $"Wirft eine Kugel an die Position des angreifenden Ziels, um ein schwarzes Loch der Größe <color=#AEEAFF>{Size}X{Size}</color> zu erzeugen, das Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft an Feinden verursacht, sie für <color=yellow>{Count} Sekunden</color> zum Zentrum des schwarzen Lochs zieht und <color=red>{DotDamage}</color> Schaden pro Sekunde verursacht.";

        ESSkillDesc = $"Lanza una esfera en la ubicación del objetivo atacante para crear un agujero negro de tamaño <color =#AEEAFF>{Size}X{Size}</color>, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque a los enemigos, atrayéndolos hacia el centro del agujero negro durante <color=yellow>{Count} segundos</color> e infligiendo <color=red>{DotDamage}</color> de daño por segundo.";

        JASkillDesc = $"攻撃中の対象の位置に球を投げて<color=#AEEAFF>{Size}X{Size}</color>サイズのブラックホールを生成し、敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを与え、<color=yellow>{Count}秒</color>間敵をブラックホールの中心に引き寄せ、毎秒<color=red>{DotDamage}</color>のダメージを与えます。";

        PT_BRSkillDesc = $"Lança uma esfera no local do alvo atacado, criando um buraco negro de tamanho<color=#AEEAFF>{Size}X{Size}</color>, causando <color=red>{AttackCoeff}x o seu dano de ataque</color> aos inimigos e puxando-os para o centro do buraco negro por <color=yellow>{Count} segundos</color>, causando <color=red>{DotDamage}</color> de dano a cada segundo.";

        RUSkillDesc = $"Бросает сферу в место нахождения атакующей цели, чтобы создать черную дыру размером <color=#AEEAFF>{Size}X{Size}</color>, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки врагам, притягивая их к центру черной дыры в течение <color=yellow>{Count} секунд</color> и нанося <color=red>{DotDamage}</color> урона в секунду.";

        ZH_HANSSkillDesc = $"向正在攻击的目标的位置投掷一个球体，在该位置创建一个<color=#AEEAFF>{Size}X{Size}</color>大小的黑洞，对敌人造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，并在<color=yellow>{Count}秒</color>内将它们拉向黑洞的中心，每秒造成<color=red>{DotDamage}</color>的伤害。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "도트 데미지 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>6P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "DOT damage <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effect per 6P</color>\n" +
                   "Duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "Dégâts sur la durée <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effet par 6P</color>\n" +
                   "Durée <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "Danno per tick <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effetto per 6P</color>\n" +
                   "Durata <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "DOT-Schaden <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Wirkung pro 6P</color>\n" +
                   "Dauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Daño DOT <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efecto por 6P</color>\n" +
                   "Duración <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "DOTダメージ <color=red>+5</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>6Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "Dano DOT <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efeito por 6P</color>\n" +
                   "Duração <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "DOT урон <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Эффект за 6P</color>\n" +
                   "Длительность <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "DOT伤害 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>每6P效果</color>\n" +
                   "持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "DOT damage <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.2</color>\n" +
               "<color=#FFBF00>Effect per 6P</color>\n" +
               "Duration <color=yellow>+1</color>";
    }
}