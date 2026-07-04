using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public enum Title { Light, Salvation, Mercy, Diligence, Kindness, Humility, Purity, Temperance, Patience, Courage, Wisdom }
public enum RewardType { monster, eliteMonster, bossMonster }

[System.Serializable]
public class FloatArray
{
    public string unitName;
    public int cnt;
    public float[] values;
}

public static class StaticManager
{
    public static UnitUpgradeStat[] stat;
    public static List<UnitUpgradeStat> itemStat = new();
    public static int[] maxLevel;
    public static int[] skillPoint;
    public static Title[][] title;
    public static string[] titleString;

    public static bool isDeckInit;  // 전투 스테이지에서 덱 세팅을 이미 했는지 여부
                                    // 게임 종료 시 true, 전투 맵 선택 시 false
    public static List<string> allDeckList;
    public static List<string> remainingDeckList;

    public static bool isShop;
    public static bool isMasterKey; // 전투 시작, 종료 시 false로 변경
    public static bool isPray;
    public static bool isDuplicate;
    public static bool isManaReduce;
    public static int donationProb;

    public static int eventDOTWeenCnt;
    public static int shopDOTWeenCnt;

    public static bool isWin;

    public static Dictionary<string, List<UnitDefault>> unitSpecificComp;
    public static UnitDefault activeUnit;

    public static int slotmachineCnt;

    public static int HPPrice;

    public static int mapSeed;

    public static int curHP;
    public static int gold;
    public static int curCost;
    public static int upStone;
    public static int curStage;

    public static List<RelicsProb> relicsProbs;
    public static List<RelicsProb> enemyRelicsProbs;

    public static RewardType rewardType;
    public static List<RewardProbability> rewardList;

    static List<List<Title>> remainingTitle;

    public static int legendaryUnitCnt;
    public static int relicsSealCnt;

    public static int[] potionLevel;

    public static FloatArray[] totalScore;
    public static int playTime;

    public static int visitedElite;
    public static int visitedEvent;

    static StaticManager()
    {
        title = new Title[48][];
        for (int i = 0; i < title.Length; i++)
            title[i] = new Title[3];
    }

    public static void StaticInit()
    {
        stat = new UnitUpgradeStat[153];
        for (int i = 0; i < stat.Length; i++)
            stat[i] = new();

        EnemyUpgradeManager.instance.EnemyUpgrade();

        maxLevel = new int[48];
        for (int i = 0; i < maxLevel.Length; i++)
            maxLevel[i] = 5;

        skillPoint = new int[48];

        title = new Title[48][];
        for (int i = 0; i < title.Length; i++)
            title[i] = new Title[3];

        titleString = new string[48];

        allDeckList = null;

        isShop = false;
        isMasterKey = false;
        isPray = false;
        isDuplicate = false;
        isManaReduce = false;
        donationProb = 25;

        eventDOTWeenCnt = 0;
        shopDOTWeenCnt = 0;

        unitSpecificComp = new();
        activeUnit = null;

        slotmachineCnt = 0;

        HPPrice = 500;

        mapSeed = 0;

        curHP = 3;
        gold = 500;
        curCost = 60;
        upStone = 0;
        curStage = 1;

        relicsProbs = null;
        enemyRelicsProbs = null;

        rewardList = null;

        for (int i = 0; i < 4; i++) {
            RemainingTitleSet();
            for (int j = 0; j < 12; j++) {
                Title randTitle1 = remainingTitle[0][Random.Range(0, remainingTitle[0].Count)];
                Title randTitle2 = remainingTitle[1][Random.Range(0, remainingTitle[1].Count)];
                Title randTitle3 = remainingTitle[2][Random.Range(0, remainingTitle[2].Count)];

                title[i * 12 + j][0] = randTitle1;
                title[i * 12 + j][1] = randTitle2;
                title[i * 12 + j][2] = randTitle3;

                remainingTitle[0].Remove(randTitle1);
                remainingTitle[1].Remove(randTitle2);
                remainingTitle[2].Remove(randTitle3);

                switch (title[i * 12 + j][1]) {
                    case Title.Mercy:
                        titleString[i * 12 + j] = "자비롭고";
                        break;
                    case Title.Diligence:
                        titleString[i * 12 + j] = "근면하고";
                        break;
                    case Title.Kindness:
                        titleString[i * 12 + j] = "친절하고";
                        break;
                }

                switch (title[i * 12 + j][2]) {
                    case Title.Humility:
                        titleString[i * 12 + j] += " 겸손한";
                        break;
                    case Title.Purity:
                        titleString[i * 12 + j] += " 순결한";
                        break;
                    case Title.Temperance:
                        titleString[i * 12 + j] += " 절제하는";
                        break;
                    case Title.Patience:
                        titleString[i * 12 + j] += " 인내하는";
                        break;
                }

                switch (title[i * 12 + j][0]) {
                    case Title.Light:
                        titleString[i * 12 + j] += "\n빛의";
                        break;
                    case Title.Salvation:
                        titleString[i * 12 + j] += "\n구원의";
                        break;
                }
            }
        }

        legendaryUnitCnt = 0;
        relicsSealCnt = 0;

        potionLevel = new int[6];

        totalScore = new FloatArray[48];
        for (int i = 0; i < totalScore.Length; i++) {
            totalScore[i] = new()
            {
                values = new float[3]
            };
        }
        playTime = 0;

        visitedElite = 0;
        visitedEvent = 0;

        SaveController.instance.GetSaveData();
    }

    static void RemainingTitleSet()
    {
        remainingTitle = new();

        List<Title> title1 = new()
        {
            Title.Light,
            Title.Light,
            Title.Light,
            Title.Light,
            Title.Light,
            Title.Light,
            Title.Salvation,
            Title.Salvation,
            Title.Salvation,
            Title.Salvation,
            Title.Salvation,
            Title.Salvation,
        };

        List<Title> title2 = new()
        {
            Title.Mercy,
            Title.Mercy,
            Title.Mercy,
            Title.Mercy,
            Title.Diligence,
            Title.Diligence,
            Title.Diligence,
            Title.Diligence,
            Title.Kindness,
            Title.Kindness,
            Title.Kindness,
            Title.Kindness,
        };

        List<Title> title3 = new()
        {
            Title.Humility,
            Title.Humility,
            Title.Humility,
            Title.Purity,
            Title.Purity,
            Title.Purity,
            Title.Temperance,
            Title.Temperance,
            Title.Temperance,
            Title.Patience,
            Title.Patience,
            Title.Patience,
        };

        remainingTitle.Add(title1);
        remainingTitle.Add(title2);
        remainingTitle.Add(title3);
    }

    private static string GetLocaleCode()
    {
        var sel = LocalizationSettings.SelectedLocale;
        return sel != null ? sel.Identifier.Code.ToLower() : "en";
    }

    private static readonly Dictionary<Title, (string ko, string en, string fr, string it, string de, string es, string ja, string pt, string ru, string zh)> _titleWordMap =
    new()
    {
        { Title.Light,      ("빛의",    "of Light",    "de Lumière",    "della Luce", "des Lichts", "de la Luz", "光の", "da Luz", "Света", "光之") },
        { Title.Salvation,  ("구원의",  "of Salvation","du Salut",      "della Salvezza","der Rettung","de la Salvación","救いの","da Salvação","Спасения","救赎之") },
        { Title.Mercy,      ("자비롭고","Merciful",    "Compatissant",  "Misericordioso","Barmherzig","Misericordioso","慈悲深い","Misericordioso","Милосердный","仁慈") },
        { Title.Diligence,  ("근면하고","Diligent",    "Diligent",      "Diligente",    "Fleißig",    "Diligente", "勤勉な","Diligente","Прилежный","勤勉") },
        { Title.Kindness,   ("친절하고","Kind",        "Chaleureux",    "Gentile",      "Freundlich", "Amable",    "親切な","Amável","Добрый","善良") },
        { Title.Humility,   ("겸손한",  "Humble",      "Humilité",      "Umile",       "Demütig",    "Humilde",   "謙虚な","Humilde","Смиренный","谦逊") },
        { Title.Purity,     ("순결한",  "Pure",        "Pure",          "Puro",         "Rein",       "Puro",      "純粋な","Puro","Чистый","纯洁") },
        { Title.Temperance, ("절제하는","Temperate",   "Tempérant",     "Temperante",   "Mäßig",      "Templado",  "節制する","Temperante","Умеренный","节制") },
        { Title.Patience,   ("인내하는","Patient",     "Patient",       "Paziente",     "Geduldig",   "Paciente",  "忍耐強い","Paciente","Терпеливый","耐心") },
        { Title.Courage,    ("용기의",  "of Courage",  "de Courage",    "del Coraggio", "des Mutes",  "del Valor",  "勇気の", "da Coragem","Храбрости","勇气") },
        { Title.Wisdom,     ("지혜의",  "of Wisdom",   "de la Sagesse",  "della Saggezza","der Weisheit","de la Sabiduría","知恵の","da Sabedoria","Мудрости","智慧") },
    };

    private static readonly Dictionary<string, string> _titleTemplates = new()
    {
        { "ko", "{0} {1}\n{2}" },
        { "en", "{0} {1} {2}" },
        { "fr", "{0} {1}\n{2}" },
        { "it", "{0} {1}\n{2}" },
        { "de", "{0} {1}\n{2}" },
        { "es", "{0} {1}\n{2}" },
        { "ja", "{2}\n{0} {1}" },
        { "pt", "{0} {1}\n{2}" },
        { "ru", "{0} {1}\n{2}" },
        { "zh", "{0} {1}\n{2}" }
    };

    public static string GetLocalizedTitleString(int unitIdx)
    {
        if (title == null || unitIdx < 0 || unitIdx >= title.Length) return string.Empty;

        var parts = title[unitIdx]; // parts[0]=first, parts[1]=middle, parts[2]=third
        if (parts == null || parts.Length < 3) return string.Empty;

        string code = GetLocaleCode();
        string langKey = code;
        // 접두사 처리 (pt-BR -> pt, zh-Hans -> zh)
        int dash = code.IndexOf('-');
        if (dash > 0) langKey = code.Substring(0, dash);

        if (!_titleTemplates.TryGetValue(langKey, out string template))
            template = _titleTemplates["en"];

        // 각 Title를 로컬 단어로 변환
        string part0 = _titleWordMap.TryGetValue(parts[0], out var w0) ? GetByLang(w0, langKey) : parts[0].ToString();
        string part1 = _titleWordMap.TryGetValue(parts[1], out var w1) ? GetByLang(w1, langKey) : parts[1].ToString();
        string part2 = _titleWordMap.TryGetValue(parts[2], out var w2) ? GetByLang(w2, langKey) : parts[2].ToString();

        // 템플릿에 맞춰 조립
        string result = string.Format(template, part1, part2, part0);
        return result;
    }

    // helper: tuple에서 언어 선택
    private static string GetByLang((string ko, string en, string fr, string it, string de, string es, string ja, string pt, string ru, string zh) tup, string langKey)
    {
        switch (langKey)
        {
            case "ko": return tup.ko;
            case "fr": return tup.fr;
            case "it": return tup.it;
            case "de": return tup.de;
            case "es": return tup.es;
            case "ja": return tup.ja;
            case "pt": return tup.pt;
            case "ru": return tup.ru;
            case "zh": return tup.zh;
            default: return tup.en;
        }
    }
}