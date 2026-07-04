using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Shadow : UnitDefault
{
    [Header("스텔스")]
    public float incCriticalPer;
    public float incAttackAmount;
    public float incDefenseAmount;
    public float duration;
    public AnimationClip stealthEffectClip;

    Vector2 prePos;

    float IncCriticalPer { get => incCriticalPer + StaticManager.skillPoint[unitIdx] * 3f; }
    float IncAttackAmount { get => incAttackAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float IncDefenseAmount { get => incDefenseAmount + StaticManager.skillPoint[unitIdx] * 5f; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5 * 0.5f; }

    WaitForSeconds wait;
    Coroutine co;

    protected override void Start()
    {
        base.Start();

        wait = new(Duration);
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0 || co != null)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        GameObject effect = Instantiate(attackObj, transform.position, Quaternion.identity, transform);
        effect.transform.localScale *= 2f;
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(stealthEffectClip);

        FindFarthestEnemy(enemies);
        if (SkillTarget != null) {
            prePos = transform.position;
            transform.position = (Vector2)SkillTarget.transform.position + (SkillTarget.GetComponent<UnitDefault>().curFlipX ? Vector2.right * 0.5f : Vector2.left * 0.5f);
        }

        SetStat(criticalPerStat, IncCriticalPer, false, true, Duration);
        SetStat(attackPowerStat, IncAttackAmount, false, true, Duration);

        SetStateBar(State.stealth, Duration);
        SetStateBar(State.incCriticalPer, Duration);
        SetStateBar(State.incAttack, Duration);

        foreach (UnitDefault friend in friends) {
            if (friend == this)
                continue;

            friend.SetStat(friend.defenseStat, IncDefenseAmount, false, true, Duration);
            friend.SetStateBar(State.incDefense, Duration);
        }

        if (co != null) {
            friends.Add(this);
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(Stealth());
    }

    IEnumerator Stealth()
    {
        friends.Remove(this);
        yield return wait;
        friends.Add(this);

        co = null;

        transform.position = prePos;
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        SoundPlay(attackSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, CompareTag("Untagged"));
    }

    public override void Death()
    {
        isDeath = true;
        gameObject.layer = 0;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        uiGaugeBar.SetActive(false);
        tag = "Untagged";

        GameManager.instance.SetScore((int)rank + 1, false);

        StaticManager.itemStat.Add(itemStat);
        RespawnManager.instance.RespawnUnit(unitID, dealMetricsIdx, CurrentMana, steroid, StaticManager.itemStat[^1], transform.parent.GetComponent<RectTransform>());

        if (friends.Contains(this))
            friends.Remove(this);

        if (co != null)
            StopCoroutine(co);

        StartCoroutine(OnDeathAnim());
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{Duration}초</color> 동안 타겟팅이 되지 않는 스텔스 상태가 되며, 본인 기준 가장 먼 적에게 순간이동 합니다.\n" +
                    $"지속 시간 동안 본인의 치명타 확률이 <color=green>{IncCriticalPer}%p</color>, 공격력이 <color=green>{IncAttackAmount}</color>만큼 증가하고, 모든 아군 유닛의 방어력이 <color=green>{IncDefenseAmount}</color>만큼 증가합니다.\n" +
                    $"스텔스 지속 시간이 끝나면 기존 위치로 되돌아옵니다.";

        ENSkillDesc = $"Becomes stealth and untargetable for <color=yellow>{Duration} seconds</color>, then teleports to the farthest enemy from the caster.\n" +
                        $"During this time, critical hit rate increases by <color=green>{IncCriticalPer}%p</color>, attack power by <color=green>{IncAttackAmount}</color>, and the defense of all allied units by <color=green>{IncDefenseAmount}</color>.\n" +
                        $"Returns to the original position when the stealth duration ends.";

        FRSkillDesc = $"Devient furtif et impossible à cibler pendant <color=yellow>{Duration} secondes</color>, puis se téléporte vers l’ennemi le plus éloigné de lui.\n" +
                        $"Pendant cette durée, la probabilité de coup critique augmente de <color=green>{IncCriticalPer}%p</color>, la puissance d’attaque de <color=green>{IncAttackAmount}</color> et la défense de toutes les unités alliées de <color=green>{IncDefenseAmount}</color>.\n" +
                        $"Retourne à sa position initiale à la fin de la furtivité.";

        ITSkillDesc = $"Diventa furtivo e non bersagliabile per <color=yellow>{Duration} secondi</color>, quindi si teletrasporta verso il nemico più lontano da sé.\n" +
                        $"Durante questo periodo, la probabilità di colpo critico aumenta di <color=green>{IncCriticalPer}%p</color>, la potenza d’attacco di <color=green>{IncAttackAmount}</color> e la difesa di tutte le unità alleate di <color=green>{IncDefenseAmount}</color>.\n" +
                        $"Al termine della furtività, ritorna alla posizione originale.";

        DESkillDesc = $"Wird für <color=yellow>{Duration} Sekunden</color> unsichtbar und nicht anvisierbar und teleportiert sich zum am weitesten entfernten Gegner.\n" +
                        $"Während dieser Zeit erhöhen sich die kritische Trefferchance um <color=green>{IncCriticalPer}%p</color>, die Angriffskraft um <color=green>{IncAttackAmount}</color> sowie die Verteidigung aller verbündeten Einheiten um <color=green>{IncDefenseAmount}</color>.\n" +
                        $"Nach Ablauf der Unsichtbarkeit kehrt er an seine ursprüngliche Position zurück.";

        ESSkillDesc = $"Se vuelve sigiloso e imposible de seleccionar como objetivo durante <color=yellow>{Duration} segundos</color> y se teletransporta al enemigo más lejano.\n" +
                        $"Durante este tiempo, la probabilidad de golpe crítico aumenta en <color=green>{IncCriticalPer}%p</color>, el poder de ataque en <color=green>{IncAttackAmount}</color> y la defensa de todas las unidades aliadas en <color=green>{IncDefenseAmount}</color>.\n" +
                        $"Al finalizar la duración del sigilo, regresa a su posición original.";

        JASkillDesc = $"<color=yellow>{Duration}秒</color>の間、ターゲットにされないステルス状態となり、自身から最も遠い敵のもとへ瞬間移動する。\n" +
                        $"持続時間中、自身のクリティカル確率が<color=green>{IncCriticalPer}%p</color>、攻撃力が<color=green>{IncAttackAmount}</color>、さらにすべての味方ユニットの防御力が<color=green>{IncDefenseAmount}</color>増加する。\n" +
                        $"ステルス終了時、元の位置に戻る。";

        PT_BRSkillDesc = $"Torna-se furtivo e impossível de ser alvo por <color=yellow>{Duration} segundos</color>, teleportando-se para o inimigo mais distante.\n" +
                            $"Durante esse período, a taxa de acerto crítico aumenta em <color=green>{IncCriticalPer}%p</color>, o poder de ataque em <color=green>{IncAttackAmount}</color> e a defesa de todas as unidades aliadas em <color=green>{IncDefenseAmount}</color>.\n" +
                            $"Ao término da furtividade, retorna à posição original.";

        RUSkillDesc = $"Становится невидимым и недоступным для выбора целью на <color=yellow>{Duration} секунд</color>, после чего телепортируется к самому дальнему противнику.\n" +
                        $"В течение этого времени шанс критического удара увеличивается на <color=green>{IncCriticalPer}%p</color>, сила атаки — на <color=green>{IncAttackAmount}</color>, а защита всех союзных юнитов — на <color=green>{IncDefenseAmount}</color>.\n" +
                        $"По окончании действия возвращается на исходную позицию.";

        ZH_HANSSkillDesc = $"在<color=yellow>{Duration}秒</color>内进入隐身且无法被选为目标的状态，并瞬移至距离自身最远的敌人处。\n" +
                            $"持续时间内，自身暴击率提高<color=green>{IncCriticalPer}%p</color>，攻击力提高<color=green>{IncAttackAmount}</color>，所有友军单位的防御力提高<color=green>{IncDefenseAmount}</color>。\n" +
                            $"隐身结束后返回原来的位置。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko"))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "치명타 확률 증가량 <color=green>+3%p</color>\n" +
                   "공격력 증가량 <color=green>+5</color>\n" +
                   "방어력 증가량 <color=green>+5</color>\n" +
                   "<color=#FFBF00>P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en"))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Critical chance increase <color=green>+3 percentage points</color>\n" +
                   "Attack power increase <color=green>+5</color>\n" +
                   "Defense increase <color=green>+5</color>\n" +
                   "<color=#FFBF00>Effect per P</color>\n" +
                   "Duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr"))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Chance de coup critique augmentée <color=green>+3 points de pourcentage</color>\n" +
                   "Attaque augmentée <color=green>+5</color>\n" +
                   "Défense augmentée <color=green>+5</color>\n" +
                   "<color=#FFBF00>Effet par P</color>\n" +
                   "Durée <color=yellow>+0.5</color>";

        if (code.StartsWith("it"))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Probabilità di colpo critico aumentata <color=green>+3 punti percentuali</color>\n" +
                   "Attacco aumentato <color=green>+5</color>\n" +
                   "Difesa aumentata <color=green>+5</color>\n" +
                   "<color=#FFBF00>Effetto per P</color>\n" +
                   "Durata <color=yellow>+0.5</color>";

        if (code.StartsWith("de"))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Kritische Trefferchance erhöht <color=green>+3 Prozentpunkte</color>\n" +
                   "Angriff erhöht <color=green>+5</color>\n" +
                   "Verteidigung erhöht <color=green>+5</color>\n" +
                   "<color=#FFBF00>Wirkung pro P</color>\n" +
                   "Dauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es"))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Probabilidad de crítico aumentada <color=green>+3 puntos porcentuales</color>\n" +
                   "Ataque aumentado <color=green>+5</color>\n" +
                   "Defensa aumentada <color=green>+5</color>\n" +
                   "<color=#FFBF00>Efecto por P</color>\n" +
                   "Duración <color=yellow>+0.5</color>";

        if (code.StartsWith("ja"))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "クリティカル確率増加 <color=green>+3パーセンテージポイント</color>\n" +
                   "攻撃力増加 <color=green>+5</color>\n" +
                   "防御力増加 <color=green>+5</color>\n" +
                   "<color=#FFBF00>Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt"))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Chance de crítico aumentada <color=green>+3 pontos percentuais</color>\n" +
                   "Ataque aumentado <color=green>+5</color>\n" +
                   "Defesa aumentada <color=green>+5</color>\n" +
                   "<color=#FFBF00>Efeito por P</color>\n" +
                   "Duração <color=yellow>+0.5</color>";

        if (code.StartsWith("ru"))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Шанс критического удара увеличен <color=green>+3 процентных пункта</color>\n" +
                   "Атака увеличена <color=green>+5</color>\n" +
                   "Защита увеличена <color=green>+5</color>\n" +
                   "<color=#FFBF00>Эффект за P</color>\n" +
                   "Длительность <color=yellow>+0.5</color>";

        if (code.StartsWith("zh"))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "暴击率提升 <color=green>+3 个百分点</color>\n" +
                   "攻击力提升 <color=green>+5</color>\n" +
                   "防御力提升 <color=green>+5</color>\n" +
                   "<color=#FFBF00>每P效果</color>\n" +
                   "持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Critical chance increase <color=green>+3 percentage points</color>\n" +
               "Attack power increase <color=green>+5</color>\n" +
               "Defense increase <color=green>+5</color>\n" +
               "<color=#FFBF00>Effect per P</color>\n" +
               "Duration <color=yellow>+0.5</color>";
    }
}