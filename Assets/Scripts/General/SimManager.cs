using Simulator.Combat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimManager : MonoBehaviour
{
    [Header("Simulation Time")]
    [SerializeField] private int _simulatorTargetedFPS = 60;
    [SerializeField] private float _simulatorTimeScale = 10;
    [SerializeField] private float _simulatorFixedTimeStep = 0.01f; //original time step = 0.02f

    public bool isSimulating = false;

    [SerializeField] private TMP_Dropdown[] championsDropdowns;
    [SerializeField] private TMP_InputField[] championsExperienceInput;

    public float timer;
    public TextMeshProUGUI outputText;
    public GameObject OutputField;
    public Button startButton;
    public Button resetButton;

    private ChampionManager championManager;
    public ChampionStats[] champStats;
    public ChampionCombat[] champCombat;

    public List<SnapShot> snaps = new List<SnapShot>();
    public List<DamageLog> damagelogs = new List<DamageLog>();
    public List<HealLog> heallogs = new List<HealLog>();
    public List<BuffLog> bufflogs = new List<BuffLog>();
    public CastLog castlog1;
    public CastLog castlog2;
    public List<Tooltip> tooltips = new List<Tooltip>();
    public List<ChampionStatsExternal> championStats = new List<ChampionStatsExternal>();

    #region Singleton
    private static SimManager _instance;
    public static SimManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private void Start()
    {
        Application.targetFrameRate = _simulatorTargetedFPS;
        Time.fixedDeltaTime = _simulatorFixedTimeStep;

        championManager = ChampionManager.Instance;
    }

    private void FixedUpdate()
    {
        if (isSimulating)
        {
            timer += Time.fixedDeltaTime;
            champCombat[0].CombatUpdate();
            champCombat[1].CombatUpdate();
        }
    }

    public void LoadMockStats(int championIndex) => StartCoroutine(LoadMockStatsEnumerator(championIndex));

    private IEnumerator LoadMockStatsEnumerator(int championIndex)
    {
        string champName = championsDropdowns[championIndex].options[championsDropdowns[championIndex].value].text;
        int exp = int.Parse(championsExperienceInput[championIndex].text);
        StatsList stats = championManager.ChampionStats(champName);

        ChampionStats newChampStats = champStats[championIndex];

        switch (champName)
        {
            case "Ashe":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Ashe>();
                break;
            case "Garen":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Garen>();
                break;
            case "Annie":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Annie>();
                break;
            case "MasterYi":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<MasterYi>();
                break;
            case "Darius":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Darius>();
                break;
            case "Fiora":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Fiora>();
                break;
            default:
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<PracticeDummy>();
                champCombat[championIndex] = newChampStats.MyCombat;
                yield break;
        }
        champCombat[championIndex] = newChampStats.MyCombat;

        FindSkills(champName, newChampStats);

        newChampStats.name = champName;
        int level = newChampStats.level = GetLevel(exp);
        newChampStats.baseHealth = (float)stats.health.flat + ((float)stats.health.perLevel * (level - 1));
        newChampStats.baseAD = (float)stats.attackDamage.flat + ((float)stats.attackDamage.perLevel * (level - 1));
        newChampStats.baseArmor = (float)stats.armor.flat + ((float)stats.armor.perLevel * (level - 1));
        newChampStats.baseSpellBlock = (float)stats.magicResistance.flat + ((float)stats.magicResistance.perLevel * (level - 1));
        newChampStats.baseAttackSpeed = (float)stats.attackSpeed.flat * (1 + ((float)stats.attackSpeed.perLevel * (level - 1) * (0.7025f + (0.0175f * (level - 1))) / 100));

        newChampStats.maxHealth = newChampStats.baseHealth;
        newChampStats.AD = newChampStats.baseAD;
        newChampStats.armor = newChampStats.baseArmor;
        newChampStats.spellBlock = newChampStats.baseSpellBlock;
        newChampStats.attackSpeed = newChampStats.baseAttackSpeed;

        //GetStatsByLevel(newChampStats, statsToLoad);
        newChampStats.currentHealth = newChampStats.maxHealth;

        newChampStats.StaticUIUpdate();
    }

    private void FindSkills(string champName, ChampionStats champStats)
    {
        for (int i = 0; i < championManager.passives.Count; i++)
        {
            if (championManager.passives[i].championName == champName)
            {
                champStats.passiveSkill = championManager.passives[i];
                break;
            }
        }
        int skillIndex = 0;
        for (int i = 0; i < championManager.qSkills.Count; i++)
        {
            if (championManager.qSkills[i].basic.champion == champName)
            {
                if (skillIndex == 0)
                    champStats.qSkill[skillIndex] = (championManager.qSkills[i]);
                else champStats.qSkill.Add(championManager.qSkills[i]);
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.wSkills.Count; i++)
        {
            if (championManager.wSkills[i].basic.champion == champName)
            {
                if (skillIndex == 0)
                    champStats.wSkill[skillIndex] = (championManager.wSkills[i]);
                else champStats.wSkill.Add(championManager.wSkills[i]);
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.eSkills.Count; i++)
        {
            if (championManager.eSkills[i].basic.champion == champName)
            {
                if (skillIndex == 0)
                    champStats.eSkill[skillIndex] = (championManager.eSkills[i]);
                else champStats.eSkill.Add(championManager.eSkills[i]);
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.rSkills.Count; i++)
        {
            if (championManager.rSkills[i].basic.champion == champName)
            {
                if (skillIndex == 0)
                    champStats.rSkill[skillIndex] = (championManager.rSkills[i]);
                else champStats.rSkill.Add(championManager.rSkills[i]);
                skillIndex++;
            }
        }
    }

    private int GetLevel(int exp)
    {
        int level = 0;
        for (int i = 0; i < Constants.MaxLevel; i++)
        {
            if (exp >= Constants.ExpTable[i])
                level++;
            else
                break;
        }
        return level;
    }

    private void GetStatsByLevel(ChampionStats champ, StatsList stats)
    {
        double[] mFactor = { 0, 0.72, 1.4750575, 2.2650575, 3.09, 3.95, 4.8450575, 5.7750575, 6.74, 7.74, 8.7750575, 9.8450575, 10.95, 12.09, 13.2650575, 14.4750575, 15.72, 17 };
        champ.baseHealth = (float)stats.health.flat + ((float)(stats.health.perLevel * mFactor[champ.level - 1]));
        champ.baseAttackSpeed = (float)stats.attackSpeed.flat;
        champ.bonusAS = (champ.level - 1) * (float)stats.attackSpeed.perLevel * 0.01f * champ.baseAttackSpeed;
        champ.baseArmor = (float)stats.armor.flat + ((float)(stats.armor.perLevel * mFactor[champ.level - 1]));
        champ.baseAD = (float)stats.attackDamage.flat + ((float)(stats.attackDamage.perLevel * mFactor[champ.level - 1]));
        champ.baseSpellBlock = (float)stats.magicResistance.flat + ((float)(stats.magicResistance.perLevel * mFactor[champ.level - 1]));
        champ.hpRegen = (float)stats.healthRegen.flat + ((float)(stats.healthRegen.perLevel * mFactor[champ.level - 1]));
        champ.maxHealth = champ.baseHealth;
        champ.currentHealth = champ.maxHealth;
        champ.attackSpeed = champ.bonusAS + champ.baseAttackSpeed;
        champ.armor = champ.baseArmor;
        champ.AD = champ.baseAD;
        champ.spellBlock = champ.baseSpellBlock;
    }

    public void ShowText(string text)
    {
        outputText.text += $"[{timer:F2}] {text} \n";
        TextFileManager.WriteString("Logs", $"[{timer:F2}] {text}");
    }

    public void StartBattle()
    {
        Time.timeScale = _simulatorTimeScale;
        TextFileManager.DeleteFileExists("Logs");
        ShowText($"TargetFrameRate = {_simulatorTargetedFPS}, TimeScale = {Time.timeScale}, TimeStep = {Time.fixedDeltaTime}");

        champStats[0].MyCombat.UpdateTarget(1);
        champStats[1].MyCombat.UpdateTarget(0);
        champStats[0].MyCombat.UpdatePriorityAndChecks();
        champStats[1].MyCombat.UpdatePriorityAndChecks();

        OutputField.SetActive(true);
        resetButton.interactable = true;
        tooltips.Add(champStats[0].MyCombat.UpdateTooltip());
        tooltips.Add(champStats[1].MyCombat.UpdateTooltip());
        championStats.Add(champStats[0].MyCombat.UpdateStatsExternal());
        championStats.Add(champStats[1].MyCombat.UpdateStatsExternal());
        
        StartCoroutine(TakeSnapShot());
        champStats[0].MyCombat.StartCoroutine(champStats[0].MyCombat.StartHPRegeneration());
        champStats[1].MyCombat.StartCoroutine(champStats[1].MyCombat.StartHPRegeneration());
        isSimulating = true;
        timer = 0f;
    }

    public void Reset() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void LoadStats(LSSAPIResponse response)
    {
        var champName1 = response.APIMatchInfo.championInfo[0].champName;
        var champName2 = response.APIMatchInfo.championInfo[1].champName;
        var stats1 = champStats[0];
        var stats2 = champStats[1];
        var so1 = Resources.Load<StatsList>($"Stats/{response.APIMatchInfo.version}/{champName1}");
        var so2 = Resources.Load<StatsList>($"Stats/{response.APIMatchInfo.version}/{champName2}");

        LoadCharacterScript(champName1, stats1);
        LoadCharacterScript(champName2, stats2);

        champStats[0] = stats1;
        champCombat[0] = stats1.MyCombat;
        champStats[1] = stats2;
        champCombat[1] = stats2.MyCombat;

        FindSkills(champName1, stats1);
        FindSkills(champName2, stats2);

        stats1.name = champName1;
        stats1.level = response.APIMatchInfo.championInfo[0].champLevel;
        stats1.qLevel = response.APIMatchInfo.championInfo[0].ability[0] - 1;
        stats1.wLevel = response.APIMatchInfo.championInfo[0].ability[1] - 1;
        stats1.eLevel = response.APIMatchInfo.championInfo[0].ability[2] - 1;
        stats1.rLevel = response.APIMatchInfo.championInfo[0].ability[3] - 1;
        stats2.name = champName2;
        stats2.level = response.APIMatchInfo.championInfo[1].champLevel;
        stats2.qLevel = response.APIMatchInfo.championInfo[1].ability[0] - 1;
        stats2.wLevel = response.APIMatchInfo.championInfo[1].ability[1] - 1;
        stats2.eLevel = response.APIMatchInfo.championInfo[1].ability[2] - 1;
        stats2.rLevel = response.APIMatchInfo.championInfo[1].ability[3] - 1;

        GetStatsByLevel(stats1, so1);
        GetStatsByLevel(stats2, so2);

        castlog1 = new CastLog(champName1);
        castlog2 = new CastLog(champName2);
        stats1.MyCombat.myCastLog = castlog1;
        stats2.MyCombat.myCastLog = castlog2;

        stats1.StaticUIUpdate();
        stats2.StaticUIUpdate();
    }

    private void LoadCharacterScript(string charName, ChampionStats stats)
    {
        switch (charName)
        {
            case "Kayn":
                stats.MyCombat = stats.gameObject.AddComponent<Kayn>();
                break;
            case "Akshan":
                stats.MyCombat = stats.gameObject.AddComponent<Akshan>();
                break;
            case "Anivia":
                stats.MyCombat = stats.gameObject.AddComponent<Anivia>();
                break;
            case "Graves":
                stats.MyCombat = stats.gameObject.AddComponent<Graves>();
                break;
            case "Yasuo":
                stats.MyCombat = stats.gameObject.AddComponent<Yasuo>();
                break;
            case "Zoe":
                stats.MyCombat = stats.gameObject.AddComponent<Zoe>();
                break;
            case "Ziggs":
                stats.MyCombat = stats.gameObject.AddComponent<Ziggs>();
                break;
            case "Zed":
                stats.MyCombat = stats.gameObject.AddComponent<Zed>();
                break;
            case "Zac":
                stats.MyCombat = stats.gameObject.AddComponent<Zac>();
                break;
            case "Yorick":
                stats.MyCombat = stats.gameObject.AddComponent<Yorick>();
                break;
            case "XinZhao":
                stats.MyCombat = stats.gameObject.AddComponent<XinZhao>();
                break;
            case "Xerath":
                stats.MyCombat = stats.gameObject.AddComponent<Xerath>();
                break;
            case "Xayah":
                stats.MyCombat = stats.gameObject.AddComponent<Xayah>();
                break;
            case "Warwick":
                stats.MyCombat = stats.gameObject.AddComponent<Warwick>();
                break;
            case "Vladimir":
                stats.MyCombat = stats.gameObject.AddComponent<Vladimir>();
                break;
            case "Viktor":
                stats.MyCombat = stats.gameObject.AddComponent<Viktor>();
                break;
            case "Vi":
                stats.MyCombat = stats.gameObject.AddComponent<Vi>();
                break;
            case "Veigar":
                stats.MyCombat = stats.gameObject.AddComponent<Veigar>();
                break;
            case "Varus":
                stats.MyCombat = stats.gameObject.AddComponent<Varus>();
                break;
            case "Vayne":
                stats.MyCombat = stats.gameObject.AddComponent<Vayne>();
                break;
            case "Twitch":
                stats.MyCombat = stats.gameObject.AddComponent<Twitch>();
                break;
            case "TwistedFate":
                stats.MyCombat = stats.gameObject.AddComponent<TwistedFate>();
                break;
            case "Tryndamere":
                stats.MyCombat = stats.gameObject.AddComponent<Tryndamere>();
                break;
            case "Trundle":
                stats.MyCombat = stats.gameObject.AddComponent<Trundle>();
                break;
            case "Tristana":
                stats.MyCombat = stats.gameObject.AddComponent<Tristana>();
                break;
            case "Teemo":
                stats.MyCombat = stats.gameObject.AddComponent<Teemo>();
                break;
            case "Talon":
                stats.MyCombat = stats.gameObject.AddComponent<Talon>();
                break;
            case "Skarner":
                stats.MyCombat = stats.gameObject.AddComponent<Skarner>();
                break;
            case "Sion":
                stats.MyCombat = stats.gameObject.AddComponent<Sion>();
                break;
            case "Singed":
                stats.MyCombat = stats.gameObject.AddComponent<Singed>();
                break;
            case "Shyvana":
                stats.MyCombat = stats.gameObject.AddComponent<Shyvana>();
                break;
            case "Sett":
                stats.MyCombat = stats.gameObject.AddComponent<Sett>();
                break;
            case "Sejuani":
                stats.MyCombat = stats.gameObject.AddComponent<Sejuani>();
                break;
            case "Ryze":
                stats.MyCombat = stats.gameObject.AddComponent<Ryze>();
                break;
            case "Riven":
                stats.MyCombat = stats.gameObject.AddComponent<Riven>();
                break;
            case "Renekton":
                stats.MyCombat = stats.gameObject.AddComponent<Renekton>();
                break;
            case "RekSai":
                stats.MyCombat = stats.gameObject.AddComponent<RekSai>();
                break;
            case "Rammus":
                stats.MyCombat = stats.gameObject.AddComponent<Rammus>();
                break;
            case "Qiyana":
                stats.MyCombat = stats.gameObject.AddComponent<Qiyana>();
                break;
            case "Ornn":
                stats.MyCombat = stats.gameObject.AddComponent<Ornn>();
                break;
            case "Orianna":
                stats.MyCombat = stats.gameObject.AddComponent<Orianna>();
                break;
            case "Olaf":
                stats.MyCombat = stats.gameObject.AddComponent<Olaf>();
                break;
            case "Nidalee":
                stats.MyCombat = stats.gameObject.AddComponent<Nidalee>();
                break;
            case "Neeko":
                stats.MyCombat = stats.gameObject.AddComponent<Neeko>();
                break;
            case "Nasus":
                stats.MyCombat = stats.gameObject.AddComponent<Nasus>();
                break;
            case "Mordekaiser":
                stats.MyCombat = stats.gameObject.AddComponent<Mordekaiser>();
                break;
            case "MissFortune":
                stats.MyCombat = stats.gameObject.AddComponent<MissFortune>();
                break;
            case "MasterYi":
                stats.MyCombat = stats.gameObject.AddComponent<MasterYi>();
                break;
            case "Malphite":
                stats.MyCombat = stats.gameObject.AddComponent<Malphite>();
                break;
            case "Lux":
                stats.MyCombat = stats.gameObject.AddComponent<Lux>();
                break;
            case "Lucian":
                stats.MyCombat = stats.gameObject.AddComponent<Lucian>();
                break;
            case "Lissandra":
                stats.MyCombat = stats.gameObject.AddComponent<Lissandra>();
                break;
            case "Lillia":
                stats.MyCombat = stats.gameObject.AddComponent<Lillia>();
                break;
            case "LeeSin":
                stats.MyCombat = stats.gameObject.AddComponent<LeeSin>();
                break;
            case "Leblanc":
                stats.MyCombat = stats.gameObject.AddComponent<LeBlanc>();
                break;
            case "KogMaw":
                stats.MyCombat = stats.gameObject.AddComponent<KogMaw>();
                break;
            case "Kennen":
                stats.MyCombat = stats.gameObject.AddComponent<Kennen>();
                break;
            case "Kayle":
                stats.MyCombat = stats.gameObject.AddComponent<Kayle>();
                break;
            case "Katarina":
                stats.MyCombat = stats.gameObject.AddComponent<Katarina>();
                break;
            case "Kassadin":
                stats.MyCombat = stats.gameObject.AddComponent<Kassadin>();
                break;
            case "Karthus":
                stats.MyCombat = stats.gameObject.AddComponent<Karthus>();
                break;
            case "Kalista":
                stats.MyCombat = stats.gameObject.AddComponent<Kalista>();
                break;
            case "Kaisa":
                stats.MyCombat = stats.gameObject.AddComponent<Kaisa>();
                break;
            case "Jinx":
                stats.MyCombat = stats.gameObject.AddComponent<Jinx>();
                break;
            case "Jhin":
                stats.MyCombat = stats.gameObject.AddComponent<Jhin>();
                break;
            case "Jayce":
                stats.MyCombat = stats.gameObject.AddComponent<Jayce>();
                break;
            case "Jax":
                stats.MyCombat = stats.gameObject.AddComponent<Jax>();
                break;
            case "Irelia":
                stats.MyCombat = stats.gameObject.AddComponent<Irelia>();
                break;
            case "Gragas":
                stats.MyCombat = stats.gameObject.AddComponent<Gragas>();
                break;
            case "GnarBig":
                stats.MyCombat = stats.gameObject.AddComponent<Gnar>();
                break;
            case "Gnar":
                stats.MyCombat = stats.gameObject.AddComponent<Gnar>();
                break;
            case "Garen":
                stats.MyCombat = stats.gameObject.AddComponent<Garen>();
                break;
            case "Gangplank":
                stats.MyCombat = stats.gameObject.AddComponent<Gangplank>();
                break;
            case "Galio":
                stats.MyCombat = stats.gameObject.AddComponent<Galio>();
                break;
            case "Fizz":
                stats.MyCombat = stats.gameObject.AddComponent<Fizz>();
                break;
            case "Fiora":
                stats.MyCombat = stats.gameObject.AddComponent<Fiora>();
                break;
            case "Ezreal":
                stats.MyCombat = stats.gameObject.AddComponent<Ezreal>();
                break;
            case "Draven":
                stats.MyCombat = stats.gameObject.AddComponent<Draven>();
                break;
            case "DrMundo":
                stats.MyCombat = stats.gameObject.AddComponent<DrMundo>();
                break;
            case "Diana":
                stats.MyCombat = stats.gameObject.AddComponent<Diana>();
                break;
            case "Chogath":
                stats.MyCombat = stats.gameObject.AddComponent<ChoGath>();
                break;
            case "Cassiopeia":
                stats.MyCombat = stats.gameObject.AddComponent<Cassiopeia>();
                break;
            case "Caitlyn":
                stats.MyCombat = stats.gameObject.AddComponent<Caitlyn>();
                break;
            case "Azir":
                stats.MyCombat = stats.gameObject.AddComponent<Azir>();
                break;
            case "Akali":
                stats.MyCombat = stats.gameObject.AddComponent<Akali>();
                break;
            case "Viego":
                stats.MyCombat = stats.gameObject.AddComponent<Viego>();
                break;
            case "Velkoz":
                stats.MyCombat = stats.gameObject.AddComponent<VelKoz>();
                break;
            case "Ekko":
                stats.MyCombat = stats.gameObject.AddComponent<Ekko>();
                break;
            case "Ivern":
                stats.MyCombat = stats.gameObject.AddComponent<Ivern>();
                break;
            case "Amumu":
                stats.MyCombat = stats.gameObject.AddComponent<Amumu>();
                break;
            case "Ashe":
                stats.MyCombat = stats.gameObject.AddComponent<Ashe>();
                break;
            case "Aatrox":
                stats.MyCombat = stats.gameObject.AddComponent<Aatrox>();
                break;
            case "Ahri":
                stats.MyCombat = stats.gameObject.AddComponent<Ahri>();
                break;
            case "Annie":
                stats.MyCombat = stats.gameObject.AddComponent<Annie>();
                break;
            case "Darius":
                stats.MyCombat = stats.gameObject.AddComponent<Darius>();
                break;
            default:
                stats.MyCombat = stats.gameObject.AddComponent<PracticeDummy>();
                champStats[0] = stats;
                champCombat[0] = stats.MyCombat;
                return;
        }
    }

    public IEnumerator TakeSnapShot()
    {
        yield return new WaitForSeconds(0.5f);
        snaps.Add(new SnapShot("", new ChampionSnap(champStats[0].name, champStats[0].PercentCurrentHealth * 100f), new ChampionSnap(champStats[1].name, champStats[1].PercentCurrentHealth * 100f), timer));
        StartCoroutine(TakeSnapShot());
    }

    public void AddDamageLog(DamageLog log)
    {
        damagelogs.Add(log);
    }

    public void AddHealLog(HealLog log)
    {
        heallogs.Add(log);
    }

    public void AddBuffLog(BuffLog log)
    {
        bufflogs.Add(log);
    }

    public void AddCastLog(CastLog castLog, int skillIndex)
    {
        switch (skillIndex)
        {
            case 5:
                castLog.ACast++;
                break;
            case 4:
                castLog.PCast++;
                break;
            case 0:
                castLog.QCast++;
                break;
            case 1:
                castLog.WCast++;
                break;
            case 2:
                castLog.ECast++;
                break;
            case 3:
                castLog.RCast++;
                break;
            default:
                break;
        }
    }
}