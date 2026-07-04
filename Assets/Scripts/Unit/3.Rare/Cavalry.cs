using UnityEngine;
using UnityEngine.Localization.Settings;

public class Cavalry : UnitDefault
{
    [Header("돌진")]
    public float attackCoeff;
    public float size;
    public int count;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] / 0.1f; }
    float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.15f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 4; }


    bool isRush;
    Vector2 dir;
    float attackTime;
    int attackCount;

    protected override void Start()
    {
        base.Start();

        attackTime = duration / Count;
    }

    protected override void FixedUpdate()
    {
        if (isRush) {
            animator.speed = Speed;
            animator.SetBool("1_Move", true);
            transform.Translate(Speed * Time.fixedDeltaTime * dir);
            attackTime -= Time.fixedDeltaTime;
            if (attackTime <= 0f) {
                attackCount++;
                SoundPlay(skillSoundClip);
                attackTime = duration / Count;
                attackSize = Size;
                CreateAttackBox(AttackPower * AttackCoeff, null, transform.position, AttackStyle.range);
                attackSize = 0.5f;
            }
            if (attackCount == Count || isDeath) {
                attackCount = 0;
                rigid2D.mass = 1f;
                isRush = false;
            }
            sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);
        }
        else
            base.FixedUpdate();
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(Size / 2f, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        dir = (pos - (Vector2)transform.position).normalized;
        rigid2D.mass = 3f;
        isRush = true;

        // 변경하지 않아도 되면 무시
        nextFlipX = dir.x > 0f;
        if (nextFlipX == curFlipX)
            return;
        // 변경해야 하면 localScale.x값의 부호를 변경, UiGaugeBar도 변경
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        uiGaugeBar.transform.localScale = new Vector2(-uiGaugeBar.transform.localScale.x, uiGaugeBar.transform.localScale.y);
        relicsState.localScale = new Vector2(-relicsState.localScale.x, relicsState.localScale.y);

        curFlipX = nextFlipX; // 현재 상태를 최신화
    }

    public override void OnStunAnim(float stunTime)
    {
        if (isRush)
            return;

        base.OnStunAnim(stunTime);
    }

    public override void Bondage(float time, AnimationClip animClip = null)
    {
        if (isRush)
            return;

        base.Bondage(time, animClip);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 방향으로 <color=yellow>{duration}초</color> 동안 돌진하며 본인 반경 <color=#AEEAFF>{Size}X{Size}</color>범위의 적들에게 공격력 <color=red>{AttackCoeff}배</color>의 피해를 총 <color=yellow>{Count}회</color> 입힙니다.\n" +
                    $"(돌진 시 기절, 속박 면역)";
        
        ENSkillDesc = $"Rush in the attack direction for <color=yellow>{duration} seconds</color>, dealing damage equal to <color=red>{AttackCoeff} times</color> the attack power to enemies in a <color=#AEEAFF>{Size}X{Size}</color> area around yourself a total of <color=yellow>{Count} times</color>.\n" +
                      $"(Immune to stun and bondage while rushing)";

        FRSkillDesc = $"Charge dans la direction de l'attaque pendant <color=yellow>{duration} secondes</color>, infligeant des dégâts égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque aux ennemis dans une zone de <color=#AEEAFF>{Size}X{Size}</color> autour de vous un total de <color=yellow>{Count} fois</color>.\n" +
                      $"(Immunisé contre l'étourdissement et la contrainte pendant la charge)";

        ITSkillDesc = $"Carica nella direzione dell'attacco per <color=yellow>{duration} secondi</color>, infliggendo danni pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco ai nemici in un'area di <color=#AEEAFF>{Size}X{Size}</color> intorno a te per un totale di <color=yellow>{Count} volte</color>.\n" +
                      $"(Immune a stordimento e costrizione durante la carica)";

        DESkillDesc = $"Stürmt für <color=yellow>{duration} Sekunden</color> in Angriffsrichtung und verursacht Schaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft an Feinden in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich um dich herum insgesamt <color=yellow>{Count} mal</color>.\n" +
                      $"(Während des Sturms immun gegen Betäubung und Fesselung)";

        ESSkillDesc = $"Carga en la dirección del ataque durante <color=yellow>{duration} segundos</color>, infligiendo daño igual a <color=red>{AttackCoeff} veces</color> el poder de ataque a los enemigos en un área de <color=#AEEAFF>{Size}X{Size}</color> alrededor de ti un total de <color=yellow>{Count} veces</color>.\n" +
                        $"(Inmune a aturdimiento y atadura mientras carga)";

        JASkillDesc = $"攻撃方向に<color=yellow>{duration}秒</color>突進し、自身の周囲<color=#AEEAFF>{Size}X{Size}</color>範囲の敵に攻撃力の<color=red>{AttackCoeff}倍</color>のダメージを合計<color=yellow>{Count}回</color>与えます。\n" +
                      $"(突進中は気絶、束縛無効)";

        PT_BRSkillDesc = $"Avança na direção do ataque por <color=yellow>{duration} segundos</color>, causando dano igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque aos inimigos em uma área de <color=#AEEAFF>{Size}X{Size}</color> ao seu redor um total de <color=yellow>{Count} vezes</color>.\n" +
                         $"(Imune a atordoamento e amarração enquanto avança)";

        RUSkillDesc = $"Рвется в направлении атаки в течение <color=yellow>{duration} секунд</color>, нанося урон, равный <color=red>{AttackCoeff} раза</color> силы атаки врагам в области <color=#AEEAFF>{Size}X{Size}</color> вокруг себя всего <color=yellow>{Count} раз</color>.\n" +
                      $"(Иммунитет к оглушению и связыванию во время рывка)";

        ZH_HANSSkillDesc = $"向攻击方向冲锋<color=yellow>{duration}秒</color>，对自身周围<color=#AEEAFF>{Size}X{Size}</color>范围内的敌人造成相当于攻击力<color=red>{AttackCoeff}倍</color>的伤害，共计<color=yellow>{Count}次</color>。\n" +
                           $"（冲锋时免疫眩晕和束缚）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "공격 횟수 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Attack count <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Nombre d'attaques <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Numero di attacchi <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Angriffszahl <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Número de ataques <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "攻撃回数 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Número de ataques <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Количество атак <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.1</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "攻击次数 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.1</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.15</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Attack count <color=yellow>+1</color>";
    }
}