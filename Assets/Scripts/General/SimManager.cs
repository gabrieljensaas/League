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

        ExtraStats(newChampStats);
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

    private void GetStatsByLevel(ChampionStats champ, ChampionsRe stats)
    {
        int level = champ.level;

        if (level == 1) return;
        double[] mFactor = { 0, 0.72, 1.4750575, 2.2650575, 3.09, 3.95, 4.8450575, 5.7750575, 6.74, 7.74, 8.7750575, 9.8450575, 10.95, 12.09, 13.2650575, 14.4750575, 15.72, 17 };
        champ.maxHealth += (float)(stats.champData.data.Champion.stats.hpperlevel * mFactor[level - 1]);
        champ.attackSpeed = (float)(champ.baseAttackSpeed * (1 + (stats.champData.data.Champion.stats.attackspeedperlevel * (level - 1)) / 100));
        champ.armor += (float)stats.champData.data.Champion.stats.armorperlevel * (float)mFactor[level - 1];
        champ.AD += (float)stats.champData.data.Champion.stats.attackdamageperlevel * (float)mFactor[level - 1];
        champ.spellBlock += (float)stats.champData.data.Champion.stats.spellblockperlevel * (float)mFactor[level - 1];
    }

    private void ExtraStats(ChampionStats champStats)
    {
        if (champStats.name == "Garen")
        {
            champStats.armor += 30;
            champStats.spellBlock += 30;
            champStats.armor *= 1.1f;
            champStats.spellBlock *= 1.1f;
        }

        if (champStats.name == "Aatrox")
        {
            champStats.passiveSkill.coolDown = Aatrox.AatroxPassiveCooldownByLevelTable[champStats.level - 1];
        }

        if (champStats.name == "Olaf")
        {
            champStats.armor += 30;
            champStats.spellBlock += 30;
        }
    }

    public void ShowText(string text)
    {
        outputText.text += $"[{timer:F2}] {text}\n\n";
        TextFileManager.WriteString("Logs", $"[{timer:F2}] {text}");
    }

    public void StartBattle()
    {
        Time.timeScale = _simulatorTimeScale;
        TextFileManager.DeleteFileExists("Logs");
        ShowText($"TargetFrameRate = {_simulatorTargetedFPS}; TimeScale = {Time.timeScale}; TimeStep = {Time.fixedDeltaTime}");

        champStats[0].MyCombat.UpdateTarget(1);
        champStats[1].MyCombat.UpdateTarget(0);
        champStats[0].MyCombat.UpdatePriorityAndChecks();
        champStats[1].MyCombat.UpdatePriorityAndChecks();

        OutputField.SetActive(true);
        resetButton.interactable = true;
        isSimulating = true;
        timer = 0f;
    }

    public void Reset() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void LoadStats(ChampionsRe response, int index)
    {
        var champName = response.champData.data.Champion.name;
        var statsToLoad = response;
        ChampionStats newChampStats;

        newChampStats = champStats[index];
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
            case "Master Yi":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<MasterYi>();
                break;
            case "Darius":
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<Darius>();
                break;
            default:
                newChampStats.MyCombat = newChampStats.gameObject.AddComponent<PracticeDummy>();
                champStats[index] = newChampStats;
                champCombat[index] = newChampStats.MyCombat;
                return;
        }

        champStats[index] = newChampStats;
        champCombat[index] = newChampStats.MyCombat;

        FindSkills(champName, newChampStats);

        newChampStats.name = champName;
        newChampStats.level = 18;                      //change from the lss response later

        newChampStats.baseHealth = (float)statsToLoad.champData.data.Champion.stats.hp;
        newChampStats.baseAD = (float)statsToLoad.champData.data.Champion.stats.attackdamage;
        newChampStats.baseArmor = (float)statsToLoad.champData.data.Champion.stats.armor;
        newChampStats.baseSpellBlock = (float)(statsToLoad.champData.data.Champion.stats.spellblock);
        newChampStats.baseAttackSpeed = (float)statsToLoad.champData.data.Champion.stats.attackspeed;

        newChampStats.maxHealth = newChampStats.baseHealth;
        newChampStats.AD = newChampStats.baseAD;
        newChampStats.armor = newChampStats.baseArmor;
        newChampStats.spellBlock = newChampStats.baseSpellBlock;
        newChampStats.attackSpeed = newChampStats.baseAttackSpeed;

        GetStatsByLevel(newChampStats, statsToLoad);
        newChampStats.currentHealth = newChampStats.maxHealth;

        ExtraStats(newChampStats);
        newChampStats.StaticUIUpdate();
    }
}