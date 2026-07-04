using UnityEngine;
using UnityEngine.Localization.Settings;

public class Walker : UnitDefault
{
    [Header("섀도우 킥")]
    public float attackCoeff;
    public float decDefenseAmount;
    public float duration;

    float AttackCoeff { get => attackCoeff + StaticManager.skillPoint[unitIdx] * 0.2f; }
    float DecDefenseAmount { get => decDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 2 * 0.5f; }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();

        unitComp.SetStat(unitComp.defenseStat, DecDefenseAmount, false, false, Duration);
        unitComp.SetStateBar(State.decDefense, Duration);
        CreateAttackBox(AttackPower * AttackCoeff, SkillTarget, SkillTarget.transform.position, isPenetration: true);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 공격력 <color=red>{AttackCoeff}배</color>의 방어력 관통 피해를 입히고, <color=yellow>{Duration}초</color> 동안 방어력을 <color=red>{DecDefenseAmount}</color>만큼 감소시킵니다.";
        
        ENSkillDesc = $"Deals defense-piercing damage equal to <color=red>{AttackCoeff} times</color> the attack power to the attacking target and reduces defense by <color=red>{DecDefenseAmount}</color> for <color=yellow>{Duration} seconds</color>.";

        FRSkillDesc = $"Inflige des dégâts contournant l'armure égaux à <color=red>{AttackCoeff} fois</color> la puissance d'attaque à la cible attaquante et réduit la défense de <color=red>{DecDefenseAmount}</color> pendant <color=yellow>{Duration} secondes</color>.";

        ITSkillDesc = $"Infligge danni perforanti l'armatura pari a <color=red>{AttackCoeff} volte</color> la potenza d'attacco al bersaglio attaccante e riduce la difesa di <color=red>{DecDefenseAmount}</color> per <color=yellow>{Duration} secondi</color>.";

        DESkillDesc = $"Verursacht Rüstungsdurchdringungsschaden in Höhe von <color=red>{AttackCoeff} mal</color> der Angriffskraft am angreifenden Ziel und reduziert die Verteidigung um <color=red>{DecDefenseAmount}</color> für <color=yellow>{Duration} Sekunden</color>.";

        ESSkillDesc = $"Inflige daño que penetra la armadura igual a <color=red>{AttackCoeff} veces</color> el poder de ataque al objetivo atacante y reduce la defensa en <color=red>{DecDefenseAmount}</color> durante <color=yellow>{Duration} segundos</color>.";

        JASkillDesc = $"攻撃中の対象に攻撃力の<color=red>{AttackCoeff}倍</color>の防御力貫通ダメージを与え、<color=yellow>{Duration}秒</color>間防御力を<color=red>{DecDefenseAmount}</color>だけ減少させます。";

        PT_BRSkillDesc = $"Inflige dano que perfura a armadura igual a <color=red>{AttackCoeff} vezes</color> o poder de ataque ao alvo atacante e reduz a defesa em <color=red>{DecDefenseAmount}</color> durante <color=yellow>{Duration} segundos</color>.";

        RUSkillDesc = $"Наносит атакующей цели урон, пробивающий броню, равный <color=red>{AttackCoeff} раза</color> силы атаки, и снижает броню на <color=red>{DecDefenseAmount}</color> в течение <color=yellow>{Duration} секунд</color>.";

        ZH_HANSSkillDesc = $"对攻击目标造成相当于<color=red>{AttackCoeff}倍</color>攻击力的破甲伤害，并在<color=yellow>{Duration}秒</color>内将防御力降低<color=red>{DecDefenseAmount}</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 계수 <color=red>+0.2</color>\n" +
                   "방어력 감소량 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack multiplier <color=red>+0.2</color>\n" +
                   "Defense reduction <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effect per 2P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque <color=red>+0.2</color>\n" +
                   "Réduction de défense <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effet par 2P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore d'attacco <color=red>+0.2</color>\n" +
                   "Riduzione difesa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Effetto per 2P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffs-Multiplikator <color=red>+0.2</color>\n" +
                   "Verteidigungsreduktion <color=red>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro 2P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Reducción de defensa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efecto por 2P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力係数 <color=red>+0.2</color>\n" +
                   "防御力減少量 <color=red>+5</color>\n" +
                   "<color=#FFBF00>2Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque <color=red>+0.2</color>\n" +
                   "Redução de defesa <color=red>+5</color>\n" +
                   "<color=#FFBF00>Efeito por 2P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель атаки <color=red>+0.2</color>\n" +
                   "Снижение защиты <color=red>+5</color>\n" +
                   "<color=#FFBF00>Эффект за 2P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力系数 <color=red>+0.2</color>\n" +
                   "防御力减少 <color=red>+5</color>\n" +
                   "<color=#FFBF00>每2P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack multiplier <color=red>+0.2</color>\n" +
               "Defense reduction <color=red>+5</color>\n" +
               "<color=#FFBF00>Effect per 2P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}