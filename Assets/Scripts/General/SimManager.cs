using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Simulator.API;
using System;
using Simulator.Combat;

public class SimManager : MonoBehaviour
{
    [Header("Simulation Time")]
    [SerializeField] private int _simulatorTargetedFPS = 60;
    [SerializeField] private float _simulatorTimeScale = 10;
    [SerializeField] private float _simulatorFixedTimeStep = 0.01f; //original time step = 0.02f

    public static bool isLoaded = false;
    public static bool battleStarted = false;
    public static float timer;

    [SerializeField] private ChampionStats[] championStats;

    private ChampionManager championManager;

    [SerializeField] private TMP_Dropdown[] championsDropdowns;
    [SerializeField] private TMP_InputField[] championsExperienceInput;

    private float time;
    public bool ongoing;
    public APIRequestManager riotAPI;
    public static TextMeshProUGUI outputText;
    public static TextMeshProUGUI timeText;
    public TextMeshProUGUI ver;
    public TextMeshProUGUI[] champ1Items;
    public TextMeshProUGUI[] champ2Items;  
    public GameObject InputField;
    public GameObject OutputField;
    public Button startBtn;
    public Button loadBtn;
    public Button resetBtn;
    public GameObject outputUI;
    public ChampionStats[] champStats;
    public ChampionCombat[] champCombat;
    public GameObject matchIDGO;
    public GameObject[] matchID;
    public TextMeshProUGUI[] output;
    public static bool isNew = true;
    public GameObject champSelectGO;
    public GameObject sliderGO;
    public GameObject sliderParent;
    public GameObject[] champOutput;
    public GameObject[] champOutput1;
    public TextMeshProUGUI timerTest;

    RiotAPIItemRequest itemRequest;
    RiotAPIItemResponse itemResponse;    
    RiotAPIMatchRequest matchRequest;
    APIRequestManager apiRequest;

    int champ1ItemNum;
    int champ2ItemNum;
    public int[] storedXP = {0,0};
    public string[] storedName = {"",""};

    public List<ChampionInput> champ;

    public static string MatchID = "";
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

        //matchIDGO.SetActive(false);
        ShowInput();
        itemRequest = GetComponent<RiotAPIItemRequest>();
        matchRequest = GetComponent<RiotAPIMatchRequest>();
        apiRequest = GetComponent<APIRequestManager>();
        outputText = output[0];
        timeText = output[1];
        championManager = ChampionManager.Instance;
        //Time.timeScale = speed;
        //foreach (GameObject item in champOutput)
        //{
        //    item.SetActive(false);
        //}
    }

    /*public void GetMatchData(Button button)
    {
        Clear();
        matchRequest.GetMatchData(button.GetComponentsInChildren<TextMeshProUGUI>()[0].text);
        MatchID = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchIDGO.SetActive(false);
        champSelectGO.SetActive(true);
        sliderParent.SetActive(true);
    }

    public void SelectChampion1(int id)
    {
        RiotAPIMatchRequest.selectedChamp[0] = id;
    }

    public void SelectChampion2(int id)
    {
        RiotAPIMatchRequest.selectedChamp[1] = id;
    }*/

    private IEnumerator LoadMockStatsEnumerator(int championIndex)
    {
        var champName = championsDropdowns[championIndex].options[championsDropdowns[championIndex].value].text;
        var exp = Int32.Parse(championsExperienceInput[championIndex].text);
        StatsList stats = championManager.ChampionStats(champName);

        ChampionStats newChampStats;

        newChampStats = championStats[championIndex];
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
                champStats[championIndex] = newChampStats;
                champCombat[championIndex] = newChampStats.MyCombat;
                yield break;
        }

        champStats[championIndex] = newChampStats;
        champCombat[championIndex] = newChampStats.MyCombat;

        FindSkills(champName, newChampStats);

        newChampStats.name = champName;
        int level = newChampStats.level = GetLevel(exp);
        newChampStats.baseHealth = (float)stats.health.flat + ((float) stats.health.perLevel * (level - 1));
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

    public void LoadMockStats(int championIndex)
    {
        StartCoroutine(LoadMockStatsEnumerator(championIndex));
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
        champ.armor += ((float)(stats.champData.data.Champion.stats.armorperlevel) * (float)mFactor[level - 1]);
        champ.AD += ((float)(stats.champData.data.Champion.stats.attackdamageperlevel) * (float)mFactor[level - 1]);
        champ.spellBlock += ((float)(stats.champData.data.Champion.stats.spellblockperlevel) * (float)mFactor[level - 1]);
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

        if(champStats.name == "Aatrox")
        {
            champStats.passiveSkill.coolDown = Constants.AatroxPassiveCooldownByLevelTable[champStats.level - 1];
        }

        if(champStats.name == "Olaf")
        {
            champStats.armor += 30;
            champStats.spellBlock += 30;
        }
    }

    /*public void LoadChampion1(Button button)
    {
        champStats[0].isLoaded = false;
        string name = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchRequest.champName[0] = name;
        int value = (int)sliderGO.GetComponent<Slider>().value;
        int xp = matchRequest.timeline.info.frames[value].participantFrames[RiotAPIMatchRequest.selectedChamp[0]+1].xp;
        storedXP[0] = xp;
        storedName[0] = name;
        //champStats[0].Reset(0);
        //apiRequest.GetRiotAPIRequest("12.10.1", storedName[0], storedName[1], storedXP[0], storedXP[1]);
        //Debug.Log(storedName[0]);
        //apiRequest.SimulateFight(0, name, xp,1);
        for (int i = 0; i<4; i++)
        {
            apiRequest.champAbilities[0].champSkills[i].text = apiRequest.allAbilities[RiotAPIMatchRequest.selectedChamp[0]].name[i];
        }
        //apiRequest.LoadItems();
    }*/

    /*public void LoadChampion2(Button button)
    {
        champStats[1].isLoaded = false;
        string name = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchRequest.champName[1] = name;
        int value = (int)sliderGO.GetComponent<Slider>().value;
        int xp = matchRequest.timeline.info.frames[value].participantFrames[RiotAPIMatchRequest.selectedChamp[1]+1].xp;
        storedXP[1] = xp;
        storedName[1] = name;
        //champStats[1].Reset(0);
        //apiRequest.GetRiotAPIRequest("12.10.1", storedName[0], storedName[1], storedXP[0], storedXP[1]);
        //apiRequest.SimulateFight(1, name, xp,1);
        for(int i = 0; i<4; i++)
        {
            apiRequest.champAbilities[1].champSkills[i].text = apiRequest.allAbilities[RiotAPIMatchRequest.selectedChamp[1]].name[i];
        }
        //apiRequest.LoadItems();
    }*/

    public void Back()
    {
        ongoing = false;
        loadBtn.interactable = true;
        resetBtn.interactable = true;
        timer = 0;
        output[0].text = "";
        output[1].text = "";
        matchIDGO.SetActive(true);
        champSelectGO.SetActive(false);
        sliderParent.SetActive(false);
        ShowInput();
    }



    public void ShowMatches(int num)
    {
        matchIDGO.SetActive(true);
        for(int i = 0; i < num; i++)
        {
            matchID[i].SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (battleStarted)
        {
            time += Time.fixedDeltaTime;
            champCombat[0].CombatUpdate();
            champCombat[1].CombatUpdate();
        }
    }

    public void ShowText(string text)
    {
        outputText.text += $"[{time:F2}] {text}\n\n";
        TextFileManager.WriteString("Logs", $"[{time:F2}] {text}");
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

        ShowOutput();
        ongoing = true;
        loadBtn.interactable = false;
        resetBtn.interactable = true;
        battleStarted = true;
        time = 0f;
    }

    public void Clear()
    {
        //champStats[0].Reset(0);
        //champStats[1].Reset(0);
    }

    public void Reset()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
        isLoaded = false;
        battleStarted = false;
        timer = 0;
        RiotAPIMatchRequest.selectedChamp[0] = 0;
        RiotAPIMatchRequest.selectedChamp[1] = 0;
    }

    void ShowInput()
    {
        OutputField.SetActive(false);
        //InputField.SetActive(true);
    }   

    void ShowOutput()
    {
        OutputField.SetActive(true);
        InputField.SetActive(false);
    } 

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    public static void WriteTime()
    {
        timeText.text += timer.ToString() + "/n";
    }

    public void LoadStats(ChampionsRe response, int index)
    {
        var champName = response.champData.data.Champion.name;
        var statsToLoad = response;
        ChampionStats newChampStats;

        newChampStats = championStats[index];
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

    public void LoadChampionData(string data)
    {
        apiRequest.LoadChampionData(data);
    }
}

[System.Serializable]
public class ChampionInput
{
    [HideInInspector] public string name;
    [HideInInspector] public int exp;
    [HideInInspector] public string spell1;
    [HideInInspector] public string spell2;
    [HideInInspector] public int q;
    [HideInInspector] public int w;
    [HideInInspector] public int e;
    [HideInInspector] public int r;
    [HideInInspector] public string item1;
    [HideInInspector] public string item2;
    [HideInInspector] public string item3;
    [HideInInspector] public string item4;
    [HideInInspector] public string item5;
    [HideInInspector] public string item6;
    public TextMeshProUGUI tname;
    public TextMeshProUGUI texp;
    public TextMeshProUGUI tspell1;
    public TextMeshProUGUI tspell2;
    public TextMeshProUGUI tq;
    public TextMeshProUGUI tw;
    public TextMeshProUGUI te;
    public TextMeshProUGUI tr;
    public TextMeshProUGUI titem1;
    public TextMeshProUGUI titem2;
    public TextMeshProUGUI titem3;
    public TextMeshProUGUI titem4;
    public TextMeshProUGUI titem5;
    public TextMeshProUGUI titem6;

    public void GetData()
    {
        name = tname.text;
        exp = int.Parse(texp.text.Replace("Exp","0").Replace("\u200B", ""));
        spell1 = tspell1.text;
        spell2 = tspell2.text;
        q = int.Parse(tq.text.Replace("\u200B", "0"));
        w = int.Parse(tw.text.Replace("\u200B", "0"));
        e = int.Parse(te.text.Replace("\u200B", "0"));
        r = int.Parse(tr.text.Replace("\u200B", "0"));
        item1 = titem1.text;
        item2 = titem2.text;
        item3 = titem3.text;
        item4 = titem4.text;
        item5 = titem5.text;
        item6 = titem6.text;
    }
}
