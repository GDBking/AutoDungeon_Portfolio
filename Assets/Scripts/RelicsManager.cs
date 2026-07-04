using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public enum StatType { MaxHealth, AttackPower, Defense, AttackSpeed, CriticalPer, LifeStealPer }

[System.Serializable]
public class RelicsProb
{
    public Title title;
    public float prob;
    public float sumProb;
}

public class RelicsEffect
{
    public Relics relicsComp;
    public StatType statType;
    public float value;
}

public class RelicsManager : MonoBehaviour
{
    public static RelicsManager instance;

    public List<TextMeshProUGUI> relicsProbTxt;
    public List<TextMeshProUGUI> enemyRelicsProbTxt;
    public List<Transform> relicsSlots;
    List<RelicsProb> relicsProbs;
    List<RelicsProb> enemyRelicsProbs;

    readonly RelicsEffect[] relicsEffects = new RelicsEffect[2];
    readonly RelicsEffect[] enemyRelicsEffects = new RelicsEffect[2];

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < relicsEffects.Length; i++)
            relicsEffects[i] = new RelicsEffect();

        for (int i = 0; i < enemyRelicsEffects.Length; i++)
            enemyRelicsEffects[i] = new RelicsEffect();

        relicsProbs = GameManager.instance.relicsProbs;
        enemyRelicsProbs = GameManager.instance.enemyRelicsProbs;

        SetRelicsProb();
    }

    public void SetRelicsProb()
    {
        relicsProbs[0].sumProb = relicsProbs[0].prob;
        for (int i = 1; i < relicsProbs.Count; i++) {
            relicsProbs[i].sumProb = relicsProbs[i - 1].sumProb + relicsProbs[i].prob;
        }

        for (int i = 0; i < relicsProbTxt.Count; i++) {
            relicsProbTxt[i].SetText(relicsProbs[i].prob.ToString());
        }

        for (int i = 0; i < enemyRelicsProbTxt.Count; i++) {
            enemyRelicsProbTxt[i].SetText(enemyRelicsProbs[i].prob.ToString());
        }
    }

    // 전투 맵 선택 시 호출
    int slotIdx;
    public void SetRelicsAvtive()
    {
        slotIdx = 0;

        for (int i = 0; i < relicsEffects.Length; i++) {
            SetRandomRelics(relicsProbs, relicsEffects[i], false);
        }
        for (int i = 0; i < enemyRelicsEffects.Length; i++) {
            SetRandomRelics(enemyRelicsProbs, enemyRelicsEffects[i], true);
        }
    }

    void SetRandomRelics(List<RelicsProb> relicsProbs, RelicsEffect relicsEffect, bool isEnemy)
    {
        relicsEffect.statType = (StatType)Random.Range(0, 6);
        if (!isEnemy) {
            float randNum = Random.Range(0, relicsProbs[^1].sumProb);
            foreach (RelicsProb relicsProb in relicsProbs) {
                if (relicsProb.sumProb >= randNum) {
                    GameObject prefab = GameManager.instance.relicsList[(int)relicsProb.title];
                    GameObject relicsObj = Instantiate(prefab, relicsSlots[slotIdx++]);

                    relicsEffect.relicsComp = relicsObj.GetComponent<Relics>();

                    int cnt = int.Parse(DeckManager.instance.allDeckRelicsCountTxt[(int)relicsEffect.relicsComp.title].text);

                    switch (relicsEffect.relicsComp.title) {
                        case Title.Light:
                        case Title.Salvation:
                        case Title.Courage:
                        case Title.Wisdom:
                            relicsEffect.value = 1f;
                            break;
                        case Title.Mercy:
                        case Title.Diligence:
                        case Title.Kindness:
                            relicsEffect.value = 1.5f;
                            break;
                        case Title.Humility:
                        case Title.Purity:
                        case Title.Temperance:
                        case Title.Patience:
                            relicsEffect.value = 2f;
                            break;
                    }
                    relicsEffect.value *= cnt;

                    break;
                }
            }
        }
        else {
            int randNum = Random.Range(0, relicsProbs.Count);

            GameObject prefab = GameManager.instance.relicsList[randNum + 11];
            GameObject relicsObj = Instantiate(prefab, relicsSlots[slotIdx++]);

            relicsEffect.relicsComp = relicsObj.GetComponent<Relics>();
            int cnt = int.Parse(DeckManager.instance.allDeckRelicsCountTxt[(int)relicsEffect.relicsComp.title].text);

            relicsEffect.value = cnt * enemyRelicsProbs[(int)relicsEffect.relicsComp.title].prob;
        }

        SetActiveRelicsTxt(relicsEffect, isEnemy);
    }

    void SetActiveRelicsTxt(RelicsEffect relicsEffect, bool isEnemy)
    {
        relicsEffect.relicsComp.isActive = true;

        // 로케일별로 출력할 문구를 생성하는 헬퍼 호출
        string activeDesc = GetLocalizedStatText(relicsEffect.statType, relicsEffect.value, isEnemy);

        relicsEffect.relicsComp.activeDesc = activeDesc;

        // useEffect 바인딩 (기존 로직 유지)
        switch (relicsEffect.statType)
        {
            case StatType.MaxHealth:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.maxHealthStat.coeff += relicsEffect.value / 100f;
                break;
            case StatType.AttackPower:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.attackPowerStat.coeff += relicsEffect.value / 100f;
                break;
            case StatType.Defense:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.defenseStat.extra += relicsEffect.value;
                break;
            case StatType.AttackSpeed:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.attackSpeedStat.coeff += relicsEffect.value / 100f;
                break;
            case StatType.CriticalPer:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.criticalPerStat.extra += relicsEffect.value;
                break;
            case StatType.LifeStealPer:
                relicsEffect.relicsComp.useEffect = unitComp =>
                    unitComp.lifeStealPerStat.extra += relicsEffect.value;
                break;
        }
    }

    public void UnitEquipment(UnitDefault unitComp)
    {
        if (unitComp.unitIdx == 48)
            return;

        foreach (RelicsEffect relicsEffect in relicsEffects) {
            relicsEffect.relicsComp.SetRelicsEffect(unitComp);
        }

        foreach (RelicsEffect relicsEffect in enemyRelicsEffects) {
            relicsEffect.relicsComp.SetRelicsEffect(unitComp);
        }
    }

    public void ResetRelics()
    {
        foreach (Transform relics in relicsSlots) {
            Destroy(relics.GetChild(0).gameObject);
        }
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        RefreshActiveRelicsText();
    }
    void RefreshActiveRelicsText()
    {
        foreach (var effect in relicsEffects)
        {
            if (effect?.relicsComp == null) continue;

            effect.relicsComp.activeDesc =
                GetLocalizedStatText(effect.statType, effect.value, false);
        }

        foreach (var effect in enemyRelicsEffects)
        {
            if (effect?.relicsComp == null) continue;

            effect.relicsComp.activeDesc =
                GetLocalizedStatText(effect.statType, effect.value, true);
        }
    }

    private string GetLocalizedStatText(StatType statType, float value, bool isEnemy)
    {
        string localeCode = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        string valueStr = value.ToString();
        string sign = isEnemy ? "" : "+";
        string codeLower = localeCode.ToLower();
        string tpl;

        if (codeLower.StartsWith("ko"))
        {
            // 한국어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"체력 {sign}{valueStr}%p"; break;
                case StatType.AttackPower:
                    tpl = $"공격력 {sign}{valueStr}%p"; break;
                case StatType.Defense:
                    tpl = $"방어력 {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"공격 속도 {sign}{valueStr}%p"; break;
                case StatType.CriticalPer:
                    tpl = $"치명타 확률 {sign}{valueStr}%p"; break;
                case StatType.LifeStealPer:
                    tpl = $"피해 흡혈 {sign}{valueStr}%p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("en"))
        {
            // 영어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Health {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Attack Power {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Defense {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Attack Speed {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Critical Chance {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Life Steal {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("fr"))
        {
            // 프랑스어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Santé {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Puissance d'attaque {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Défense {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Vitesse d'attaque {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Chance de critique {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Vol de vie {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("it"))
        {
            // 이탈리아어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Salute {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Potenza d'attacco {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Difesa {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Velocità d'attacco {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Probabilità critica {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Rubavita {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("de"))
        {
            // 독일어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Leben {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Angriffskraft {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Verteidigung {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Angriffsgeschw. {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Kritische Chance {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Lebensraub {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("es"))
        {
            // 스페인어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Salud {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Poder de ataque {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Defensa {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Velocidad de ataque {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Prob. Crítico {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Robo de vida {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("ja"))
        {
            // 일본어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"体力 {sign}{valueStr}%p"; break;
                case StatType.AttackPower:
                    tpl = $"攻撃力 {sign}{valueStr}%p"; break;
                case StatType.Defense:
                    tpl = $"防御力 {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"攻撃速度 {sign}{valueStr}%p"; break;
                case StatType.CriticalPer:
                    tpl = $"会心率 {sign}{valueStr}%p"; break;
                case StatType.LifeStealPer:
                    tpl = $"ライフ吸収 {sign}{valueStr}%p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("pt"))
        {
            // 포르투갈(브라질) 및 pt 접두사
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Vida {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Poder de ataque {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Defesa {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Velocidade de ataque {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Chance crítica {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Roubo de vida {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("ru"))
        {
            // 러시아어
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Здоровье {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Сила атаки {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Защита {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Скорость атаки {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Крит шанс {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Воровство жизни {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else if (codeLower.StartsWith("zh"))
        {
            // 중국어 간체 (zh-Hans 포함)
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"生命值 {sign}{valueStr}%p"; break;
                case StatType.AttackPower:
                    tpl = $"攻击力 {sign}{valueStr}%p"; break;
                case StatType.Defense:
                    tpl = $"防御 {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"攻击速度 {sign}{valueStr}%p"; break;
                case StatType.CriticalPer:
                    tpl = $"暴击率 {sign}{valueStr}%p"; break;
                case StatType.LifeStealPer:
                    tpl = $"生命偷取 {sign}{valueStr}%p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }
        else
        {
            // 기본 fallback (영어)
            switch (statType)
            {
                case StatType.MaxHealth:
                    tpl = $"Health {sign}{valueStr} %p"; break;
                case StatType.AttackPower:
                    tpl = $"Attack Power {sign}{valueStr} %p"; break;
                case StatType.Defense:
                    tpl = $"Defense {sign}{valueStr}"; break;
                case StatType.AttackSpeed:
                    tpl = $"Attack Speed {sign}{valueStr} %p"; break;
                case StatType.CriticalPer:
                    tpl = $"Critical Chance {sign}{valueStr} %p"; break;
                case StatType.LifeStealPer:
                    tpl = $"Life Steal {sign}{valueStr} %p"; break;
                default:
                    tpl = $"{sign}{valueStr}"; break;
            }
        }

        return tpl;
    }
}