using UnityEngine;
using UnityEngine.Localization.Settings;

public class Swift : UnitDefault
{
    [Header("장막")]
    public float incAttackSpeedPer;
    public float size;
    public float duration;
    public GameObject windBarrierPrefab;

    float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 3f; }
    float Size { get => size + StaticManager.skillPoint[unitIdx] * 0.15f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }

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

        Vector2 dir = (pos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        SoundPlay(skillSoundClip);
        GameObject windBarrier = Instantiate(windBarrierPrefab, (Vector2)transform.position + (dir * 0.5f), rotation, GameManager.instance.skillEffect);
        windBarrier.tag = "Friendly Barrier";
        windBarrier.transform.localScale = new Vector2(1f, Size);

        Destroy(windBarrier, Duration);

        foreach (UnitDefault friend in friends) {
            friend.SetStat(friend.attackSpeedStat, IncAttackSpeedPer, true, true, Duration);
            friend.SetStateBar(State.incAttackSpeed, Duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인의 앞에 <color=yellow>{Duration}초</color> 동안 탄막을 막는 <color=#AEEAFF>1X{Size}</color>크기의 장막을 생성하며, 모든 아군 유닛의 공격 속도를 <color=green>{IncAttackSpeedPer}%</color> 증가시킵니다.";
        
        ENSkillDesc = $"Creates a <color=#AEEAFF>1X{Size}</color>-sized barrier that blocks projectiles in front of the user for <color=yellow>{Duration} seconds</color>, increasing the attack speed of all allied units by <color=green>{IncAttackSpeedPer}%</color>.";

        FRSkillDesc = $"Crée une barrière de taille <color=#AEEAFF>1X{Size}</color> qui bloque les projectiles devant l'utilisateur pendant <color=yellow>{Duration} secondes</color>, augmentant la vitesse d'attaque de toutes les unités alliées de <color=green>{IncAttackSpeedPer}%</color>.";

        ITSkillDesc = $"Crea una barriera di dimensioni <color=#AEEAFF>1X{Size}</color> che blocca i proiettili di fronte all'utilizzatore per <color=yellow>{Duration} secondi</color>, aumentando la velocità di attacco di tutte le unità alleate del <color=green>{IncAttackSpeedPer}%</color>.";

        DESkillDesc = $"Erstellt eine <color=#AEEAFF>1X{Size}</color>-große Barriere, die Projektile vor dem Benutzer für <color=yellow>{Duration} Sekunden</color> blockiert und die Angriffsgeschwindigkeit aller verbündeten Einheiten um <color=green>{IncAttackSpeedPer}%</color> erhöht.";

        ESSkillDesc = $"Crea una barrera de tamaño <color=#AEEAFF>1X{Size}</color> que bloquea los proyectiles frente al usuario durante <color=yellow>{Duration} segundos</color>, aumentando la velocidad de ataque de todas las unidades aliadas en <color=green>{IncAttackSpeedPer}%</color>.";

        JASkillDesc = $"自身の前方に弾幕を防ぐ<color=#AEEAFF>1X{Size}</color>の大きさの幕を<color=yellow>{Duration}秒</color>間生成し、すべての味方ユニットの攻撃速度を<color=green>{IncAttackSpeedPer}%</color>増加させます。";

        PT_BRSkillDesc = $"Cria uma barreira de tamanho <color=#AEEAFF>1X{Size}</color> que bloqueia projéteis na frente do usuário por <color=yellow>{Duration} segundos</color>, aumentando a velocidade de ataque de todas as unidades aliadas em <color=green>{IncAttackSpeedPer}%</color>.";

        RUSkillDesc = $"Создает барьер размером <color=#AEEAFF>1X{Size}</color>, который блокирует снаряды перед пользователем в течение <color=yellow>{Duration} секунд</color>, увеличивая скорость атаки всех союзных юнитов на <color=green>{IncAttackSpeedPer}%</color>.";

        ZH_HANSSkillDesc = $"在自身前方创建一个<color=#AEEAFF>1X{Size}</color>大小的屏障，阻挡弹幕，持续<color=yellow>{Duration}秒</color>，并将所有友军单位的攻击速度提高<color=green>{IncAttackSpeedPer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격 속도 증가량 <color=green>+3%p</color>\n" +
                   "장막 길이 <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack speed increase <color=green>+3%p</color>\n" +
                   "Veil length <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de la vitesse d'attaque <color=green>+3%p</color>\n" +
                   "Longueur du voile <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento velocità d'attacco <color=green>+3%p</color>\n" +
                   "Lunghezza velo <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffsgeschwindigkeit + <color=green>3%p</color>\n" +
                   "Schleierlänge <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de velocidad de ataque <color=green>+3%p</color>\n" +
                   "Longitud del velo <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃速度増加量 <color=green>+3%p</color>\n" +
                   "幕の長さ <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de velocidade de ataque <color=green>+3%p</color>\n" +
                   "Comprimento do véu <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение скорости атаки <color=green>+3%p</color>\n" +
                   "Длина завесы <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击速度增加量 <color=green>+3%p</color>\n" +
                   "幕长度 <color=#AEEAFF>+0.15</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack speed increase <color=green>+3%p</color>\n" +
               "Veil length <color=#AEEAFF>+0.15</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}