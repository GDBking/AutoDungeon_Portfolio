using UnityEngine;
using UnityEngine.Localization.Settings;

public class Druid : UnitDefault
{
    [Header("조화")]
    public float incHealthPer;
    public int incManaAmount;
    public float size;
    
    float IncHealthPer { get => Mathf.Min(incHealthPer + StaticManager.skillPoint[unitIdx] * 4f); }
    int IncManaAmount { get => incManaAmount + StaticManager.skillPoint[unitIdx] / 3; }
    float Size { get => size + StaticManager.skillPoint[unitIdx] / 2 * 0.2f; }

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
        isHealer = true;
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        FindLesserHpGaugePerFriendly();
        if (FriendlyTarget == gameObject) {
            SoundPlay(skillBulletHitClip);
            OnSkillFire(gameObject);
        }
        else
            StartCoroutine(OnFire(FriendlyTarget, friendlyTargetPos));
    }

    void OnSkillFire(GameObject target)
    {
        GameObject effect = Instantiate(attackObj, target.transform.position, Quaternion.identity);
        effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
        effect.transform.localScale *= Size;
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

        Collider2D[] hitFriends = Physics2D.OverlapCircleAll(target.transform.position, Size / 2f, LayerMask.GetMask(tag));
        foreach (Collider2D friendly in hitFriends) {
            UnitDefault unitComp = friendly.GetComponent<UnitDefault>();
            unitComp.Healing(unitComp.MaxHealth / 100f * IncHealthPer, unitIdx);
            if (unitComp is not Druid && unitComp is not Priest) {
                unitComp.SetMana(IncManaAmount);
            }
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인을 포함한 가장 체력이 낮은 아군 주위 <color=#AEEAFF>{Size}X{Size}</color>범위에 조화를 일으켜 체력을 <color=green>{IncHealthPer}%</color>, 마나를 <color=green>{IncManaAmount}</color>만큼 회복시킵니다.\n" +
                    $"(드루이드, 성직자는 마나 회복 제한)";
        
        ENSkillDesc = $"Creates harmony in a <color=#AEEAFF>{Size}X{Size}</color> area around the ally with the lowest health, including yourself, restoring <color=green>{IncHealthPer}%</color> health and <color=green>{IncManaAmount}</color> mana.\n" +
                      $"(Druids and Priests have a mana recovery limit)";

        FRSkillDesc = $"Crée une harmonie dans une zone de <color=#AEEAFF>{Size}X{Size}</color> autour de l'allié ayant la santé la plus basse, y compris vous-même, restaurant <color=green>{IncHealthPer}%</color> de santé et <color=green>{IncManaAmount}</color> de mana.\n" +
                      $"(Les druides et les prêtres ont une limite de récupération de mana)";

        ITSkillDesc = $"Crea armonia in un'area di <color=#AEEAFF>{Size}X{Size}</color> intorno all'alleato con la salute più bassa, incluso te stesso, ripristinando <color=green>{IncHealthPer}%</color> di salute e <color=green>{IncManaAmount}</color> di mana.\n" +
                      $"(Druidi e sacerdoti hanno un limite di recupero del mana)";

        DESkillDesc = $"Erzeugt Harmonie in einem <color=#AEEAFF>{Size}X{Size}</color>-Bereich um die verbündete Einheit mit dem niedrigsten Gesundheitswert, einschließlich dir selbst, und stellt <color=green>{IncHealthPer}%</color> Gesundheit und <color=green>{IncManaAmount}</color> Mana wieder her.\n" +
                      $"(Druiden und Priester haben ein Mana-Wiederherstellungslimit)";

        ESSkillDesc = $"Crea armonía en un área de <color=#AEEAFF>{Size}X{Size}</color> alrededor del aliado con la salud más baja, incluido tú mismo, restaurando <color=green>{IncHealthPer}%</color> de salud y <color=green>{IncManaAmount}</color> de maná.\n" +
                      $"(Los druidas y sacerdotes tienen un límite de recuperación de maná)";

        JASkillDesc = $"自分を含む最も体力が低い味方の周囲<color=#AEEAFF>{Size}X{Size}</color>範囲に調和をもたらし、体力を<color=green>{IncHealthPer}%</color>、マナを<color=green>{IncManaAmount}</color>回復します。\n" +
                      $"(ドルイド、プリーストはマナ回復制限)";

        PT_BRSkillDesc = $"Cria harmonia em uma área de <color=#AEEAFF>{Size}X{Size}</color> ao redor do aliado com a saúde mais baixa, incluindo você mesmo, restaurando <color=green>{IncHealthPer}%</color> de saúde e <color=green>{IncManaAmount}</color> de mana.\n" +
                         $"(Druidas e sacerdotes têm um limite de recuperação de mana)";

        RUSkillDesc = $"Создает гармонию в области <color=#AEEAFF>{Size}X{Size}</color> вокруг союзника с наименьшим здоровьем, включая вас, восстанавливая <color=green>{IncHealthPer}%</color> здоровья и <color=green>{IncManaAmount}</color> маны.\n" +
                      $"(У друидов и жрецов есть ограничение на восстановление маны)";

        ZH_HANSSkillDesc = $"在包括你自己在内的生命值最低的盟友周围的<color=#AEEAFF>{Size}X{Size}</color>范围内创造和谐，恢复<color=green>{IncHealthPer}%</color>生命值和<color=green>{IncManaAmount}</color>法力值。\n" +
                           $"（德鲁伊和牧师有法力恢复限制）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "체력 회복량 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "범위 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "마나 회복량 <color=green>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Health recovery <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Range <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Mana recovery <color=green>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Récupération de vie <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Portée <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Récupération de mana <color=green>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Recupero salute <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Raggio <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Recupero mana <color=green>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Gesundheitserholung <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Reichweite <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Mana-Erholung <color=green>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Recuperación de salud <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Recuperación de mana <color=green>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "体力回復量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "範囲 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "マナ回復量 <color=green>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Recuperação de vida <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Alcance <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Recuperação de mana <color=green>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Восстановление здоровья <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Дальность <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Восстановление маны <color=green>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "生命恢复量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "范围 <color=#AEEAFF>+0.2</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "法力恢复量 <color=green>+1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Health recovery <color=green>+4%p</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Range <color=#AEEAFF>+0.2</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Mana recovery <color=green>+1</color>";
    }
}