using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Phantom : UnitDefault
{
    public int count;
    public float duration;
    public GameObject clonePrefab;
    
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 3; }
    float Duration { get => duration + StaticManager.skillPoint[unitIdx] / 5; }

    WaitForSeconds wait;
    readonly List<GameObject> cloneList = new();
    bool isPhantom;

    protected override void Start()
    {
        base.Start();

        wait = new(Duration);
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        CreateAttackBox(-1f, null, transform.position);
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        for (int i = 0; i < Count; i++) {
            cloneList.Add(Instantiate(clonePrefab, transform.position, Quaternion.identity, unitField));
            Phantom phantomComp = cloneList[i].GetComponent<Phantom>();

            RelicsManager.instance.UnitEquipment(phantomComp);

            phantomComp.dealMetricsIdx = dealMetricsIdx;
            phantomComp.itemStat = itemStat;
            phantomComp.steroid = steroid;
            phantomComp.health = health;
            phantomComp.updateHpBarAction(phantomComp.MaxHealth, phantomComp.Health);
            phantomComp.SetStateBar(State.phantom, Duration);
            phantomComp.enabled = true;
        }

        Teleport();
        StartCoroutine(DestroyClone());
    }

    void Teleport()
    {
        UnitDefault moveTarget = SelectTarget();
        if (moveTarget == null)
            return;

        Vector2 movePos = moveTarget.transform.position;
        movePos += moveTarget.curFlipX ? Vector2.left : Vector2.right;

        transform.position = movePos;
        foreach (GameObject clone in cloneList) {
            if (clone != null)
                clone.transform.position = movePos;
        }

        CreateAttackBox(-1f, null, transform.position);
    }

    IEnumerator DestroyClone()
    {
        yield return wait;

        foreach (GameObject clone in cloneList) {
            if (clone != null) {
                friends.Remove(clone.GetComponent<Phantom>());
                Destroy(clone);
            }
        }
    }

    UnitDefault SelectTarget()
    {
        // 힐러 유닛들을 필터링
        var healers = enemies.Where(e => e.isHealer).ToList();

        if (healers.Count > 0) {
            // 힐러 중에서 랜덤 선택
            return healers[Random.Range(0, healers.Count)];
        }
        else if (enemies.Count != 0) {
            // 최대 사거리 찾기
            float maxRange = enemies.Max(e => e.AttackRange);
            // 최대 사거리와 같은 유닛들만 필터링
            var longRangeUnits = enemies.Where(e => Mathf.Approximately(e.AttackRange, maxRange)).ToList();
            // 그 중 랜덤 선택
            return longRangeUnits[Random.Range(0, longRangeUnits.Count)];
        }
        else
            return null;
    }

    public override void Death()
    {
        if (clonePrefab == null) {
            isDeath = true;
            gameObject.layer = 0;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
            uiGaugeBar.SetActive(false);
            tag = "Untagged";

            if (friends.Contains(this))
                friends.Remove(this);

            StartCoroutine(OnDeathAnim());
        }
        else {
            foreach (GameObject clone in cloneList) {
                if (clone != null)
                    clone.GetComponent<UnitDefault>().Death();
            }
            base.Death();
        }
    }

    protected override IEnumerator ChargingMana()
    {
        if (clonePrefab == null)
            yield break;

        WaitForSeconds wait = new(1f);

        while (true) {
            yield return wait;

            while (isSilence)
                yield return new WaitForSeconds(silenceRemainingTime);

            updateMpBarAction(MaxMana, ++CurrentMana); // 마나바 최신화
            UpdateStatInfo();

            if (CurrentMana == MaxMana) {
                isPhantom = true;
                isSkillAvaliable = true;
                yield break;
            }
        }
    }

    public override void SetMana(int amount)
    {
        if (isPhantom)
            return;

        base.SetMana(amount);
    }

    protected override void SkillUseAfterState()
    {
        isSkillAvaliable = false;
        CurrentMana = 0;
        updateMpBarAction(MaxMana, CurrentMana);
        UpdateStatInfo();
    }

    protected override void OnEnable()
    {
        if (clonePrefab == null)
            friends.Add(this);
        else
            base.OnEnable();
    }

    protected override void OnDestroy()
    {
        if (clonePrefab == null) {
            if (StaticManager.unitSpecificComp.TryGetValue(unitName, out var list)) {
                list.Remove(this);
            }
            if (friends.Contains(this)) {
                friends.Remove(this);
            }
        }
        else
            base.OnDestroy();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"마나가 모두 차면 최대 <color=yellow>{Duration}초</color> 동안 유지되는 도플갱어를 <color=yellow>{Count}명</color> 소환하고, 적 유닛의 뒤로 순간이동합니다.\n" +
                    "(순간이동 우선순위: 힐러 > 원거리 > 근거리)\n" +
                    "(스폰당 1회 사용 가능하며, 본체가 사망 시 도플갱어는 모두 사망합니다.)";
        
        ENSkillDesc = $"When mana is full, summons <color=yellow>{Count} doppelgangers</color> that last for up to <color=yellow>{Duration} seconds</color> and teleports behind an enemy unit.\n" +
                       "(Teleportation priority: Healer > Ranged > Melee)\n" +
                       "(Can be used once per spawn, and all doppelgangers die when the main body dies.)";

        FRSkillDesc = $"Lorsque la mana est pleine, invoque <color=yellow>{Count} doppelgängers</color> qui durent jusqu'à <color=yellow>{Duration} secondes</color> et se téléporte derrière une unité ennemie.\n" +
                       "(Priorité de téléportation : Soigneur > À distance > Mêlée)\n" +
                       "(Peut être utilisé une fois par apparition, et tous les doppelgängers meurent lorsque le corps principal meurt.)";

        ITSkillDesc = $"Quando la mana è piena, evoca <color=yellow>{Count} doppelgänger</color> che durano fino a <color=yellow>{Duration} secondi</color> e si teletrasporta dietro un'unità nemica.\n" +
                       "(Priorità di teletrasporto: Guaritore > A distanza > Mischia)\n" +
                       "(Può essere usato una volta per spawn, e tutti i doppelgänger muoiono quando il corpo principale muore.)";

        DESkillDesc = $"Wenn das Mana voll ist, beschwört es <color=yellow>{Count} Doppelgänger</color>, die bis zu <color=yellow>{Duration} Sekunden</color> dauern, und teleportiert sich hinter eine feindliche Einheit.\n" +
                       "(Teleportationspriorität: Heiler > Fernkampf > Nahkampf)\n" +
                       "(Kann einmal pro Spawn verwendet werden, und alle Doppelgänger sterben, wenn der Hauptkörper stirbt.)";

        ESSkillDesc = $"Cuando el maná está lleno, invoca <color=yellow>{Count} dobles</color> que duran hasta <color=yellow>{Duration} segundos</color> y se teletransporta detrás de una unidad enemiga.\n" +
                       "(Prioridad de teletransporte: Sanador > A distancia > Cuerpo a cuerpo)\n" +
                       "(Se puede usar una vez por aparición, y todos los dobles mueren cuando el cuerpo principal muere.)";

        JASkillDesc = $"マナが満タンになると、最大<color=yellow>{Duration}秒</color>間持続するドッペルゲンガーを<color=yellow>{Count}体</color>召喚し、敵ユニットの後ろに瞬間移動します。\n" +
                       "(瞬間移動の優先順位: ヒーラー > 遠距離 > 近距離)\n" +
                       "(スポーンごとに1回使用可能で、本体が死亡するとドッペルゲンガーはすべて死亡します。)";

        PT_BRSkillDesc = $"Quando sua mana está cheia, invoca <color=yellow>{Count} doppelgängers</color> que duram até <color=yellow>{Duration} segundos</color> e se teletransporta atrás de uma unidade inimiga.\n" +
                          "(Prioridade de teletransporte: Curandeiro > À distância > Corpo a corpo)\n" +
                          "(Pode ser usado uma vez por spawn, e todos os doppelgängers morrem quando o corpo principal morre.)";

        RUSkillDesc = $"Когда мана полна, призывает <color=yellow>{Count} двойников</color>, которые длятся до <color=yellow>{Duration} секунд</color>, и телепортируется за вражеский юнит.\n" +
                       "(Приоритет телепортации: целитель > дальний бой > ближний бой)\n" +
                       "(Можно использовать один раз за спаун, и все двойники умирают, когда умирает основное тело.)";

        ZH_HANSSkillDesc = $"当法力值充满时，召唤<color=yellow>{Count}个</color>持续时间最长为<color=yellow>{Duration}秒</color>的分身，并传送到敌方单位的后方。\n" +
                            "（传送优先级：治疗者>远程>近战）\n" +
                            "（每次生成可使用一次，主身体死亡时所有分身都会死亡。）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>3P</color>당 효과\n" +
                   "분신 수 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>5P</color>당 효과\n" +
                   "지속 시간 <color=yellow>+1</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Number of clones <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effect per 5P</color>\n" +
                   "Duration <color=yellow>+1</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Nombre de clones <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effet par 5P</color>\n" +
                   "Durée <color=yellow>+1</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Numero di cloni <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Effetto per 5P</color>\n" +
                   "Durata <color=yellow>+1</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Anzahl der Klone <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Wirkung pro 5P</color>\n" +
                   "Dauer <color=yellow>+1</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Número de clones <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Efecto por 5P</color>\n" +
                   "Duración <color=yellow>+1</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "分身数 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>5Pごとの効果</color>\n" +
                   "持続時間 <color=yellow>+1</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Número de clones <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Efeito por 5P</color>\n" +
                   "Duração <color=yellow>+1</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Количество клонов <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>Эффект за 5P</color>\n" +
                   "Длительность <color=yellow>+1</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每3P效果</color>\n" +
                   "分身数量 <color=yellow>+1</color>\n" +
                   "<color=#FFBF00>每5P效果</color>\n" +
                   "持续时间 <color=yellow>+1</color>";

        return "<color=#FFBF00>Effect per 3P</color>\n" +
               "Number of clones <color=yellow>+1</color>\n" +
               "<color=#FFBF00>Effect per 5P</color>\n" +
               "Duration <color=yellow>+1</color>";
    }
}