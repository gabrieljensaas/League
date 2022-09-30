using Simulator.Combat;
using System.Collections;
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

    public static bool isSimulating = false;

    [SerializeField] private TMP_Dropdown[] championsDropdowns;
    [SerializeField] private TMP_InputField[] championsExperienceInput;

    private float timer;
    public TextMeshProUGUI outputText;
    public GameObject OutputField;
    public Button startButton;
    public Button resetButton;

    private ChampionManager championManager;
    public ChampionStats[] champStats;
    public ChampionCombat[] champCombat;

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
                champStats.qSkill[skillIndex] = championManager.qSkills[i];
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.wSkills.Count; i++)
        {
            if (championManager.wSkills[i].basic.champion == champName)
            {
                champStats.wSkill[skillIndex] = championManager.wSkills[i];
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.eSkills.Count; i++)
        {
            if (championManager.eSkills[i].basic.champion == champName)
            {
                champStats.eSkill[skillIndex] = championManager.eSkills[i];
                skillIndex++;
            }
        }
        skillIndex = 0;
        for (int i = 0; i < championManager.rSkills.Count; i++)
        {
            if (championManager.rSkills[i].basic.champion == champName)
            {
                champStats.rSkill[skillIndex] = championManager.rSkills[i];
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
        champ.attackSpeed = champ.bonusAS + champ.baseAttackSpeed;
        champ.baseArmor = (float)stats.armor.flat + ((float)(stats.armor.perLevel * mFactor[champ.level - 1]));
        champ.baseAD = (float)stats.attackDamage.flat + ((float)(stats.attackDamage.perLevel * mFactor[champ.level - 1]));
        champ.baseSpellBlock = (float)stats.magicResistance.flat + ((float)(stats.magicResistance.perLevel * mFactor[champ.level - 1]));
        champ.hpRegen = (float)stats.healthRegen.flat + ((float)(stats.healthRegen.perLevel * mFactor[champ.level - 1]));
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
        isSimulating = true;
        champStats[0].MyCombat.StartCoroutine(champStats[0].MyCombat.StartHPRegeneration());
        champStats[1].MyCombat.StartCoroutine(champStats[1].MyCombat.StartHPRegeneration());
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
        stats2.name = champName2;
        stats2.level = response.APIMatchInfo.championInfo[1].champLevel;

        GetStatsByLevel(stats1, so1);
        stats1.currentHealth = stats1.maxHealth;
        GetStatsByLevel(stats2, so2);
        stats2.currentHealth = stats2.maxHealth;

        stats1.StaticUIUpdate();
        stats2.StaticUIUpdate();
    }

    private void LoadCharacterScript(string charName, ChampionStats stats)
    {
        switch (charName)
        {
            case "Ashe":
                stats.MyCombat = stats.gameObject.AddComponent<Ashe>();
                break;
            case "Aatrox":
                stats.MyCombat = stats.gameObject.AddComponent<Aatrox>();
                break;
            case "Garen":
                stats.MyCombat = stats.gameObject.AddComponent<Garen>();
                break;
            case "Annie":
                stats.MyCombat = stats.gameObject.AddComponent<Annie>();
                break;
            case "Master Yi":
                stats.MyCombat = stats.gameObject.AddComponent<MasterYi>();
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
}