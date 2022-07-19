using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Text;

public class RiotAPIRequest : MonoBehaviour
{
    public RiotAPIResponse champion;
    public LSSAPIResponse LSSAPI;
    SimManager simManager;
    ChampionAPI myObj = new ChampionAPI();

    public Skills skills;

    public int[] testExp = new int[2];
    public string[] testName = new string[2];

    public int[] ExpTable = {0,280,660,1140,1720,2400,3180,4060,5040,6120,7300,8580,9960,11440,13020,14700,16480,18360};
    public int MaxLevel = 18;

    private string apiURL = "";
    private string _champ1;
    private string _champ2;
    public static bool Load1, Load2;

    public TMP_Dropdown dd1;
    public TMP_Dropdown dd2;
    public TMP_InputField if1;
    public TMP_InputField if2;

    int[] champ1Items = { 0, 0, 0, 0, 0, 0};
    int[] champ2Items = { 0, 0, 0, 0, 0, 0};

    public GameObject[] Champions;
    public ChampStats[] champStats;
    RiotAPIMatchRequest matchRequest;
    RiotAPIItemRequest itemRequest;

    public List<ChampionAbilities> champAbilities;
    public List<AllAbilities> allAbilities;

    void Update()
    {
        //string s = "APIMatchInfo: {version: \"12.10.1\",championInfo:[{champName: {\"Ashe\"},champLevel: 18,items: [3119, 2115],},{champName: {\"Jax\"},champLevel: 18,},],},};";

        if(Load1 && Load2)
        {
            //SimManager.isLoaded = true;
        }
        if (Input.GetMouseButton(0))
        {
            //LoadData(JsonUtility.ToJson(s));
            Test();
        }
    }

    void Start()
    {        
        Champions = GameObject.FindGameObjectsWithTag("Champion");
        itemRequest = GetComponent<RiotAPIItemRequest>();
        matchRequest = GetComponent<RiotAPIMatchRequest>();
        simManager = GetComponent<SimManager>();
        StartCoroutine(TestRepeat());
        //+Test();
    }

    IEnumerator TestRepeat()
    {
        yield return new WaitForSeconds(5f);
        Test();
    }

    public void Test()
    {
        Load1 = false;
        Load2 = false;
        simManager.outputText.text = "";
        champStats[0].isLoaded = false;
        champStats[1].isLoaded = false;
        SimManager.isLoaded = false;
        simManager.ongoing = false;
        Time.timeScale = 2f;
        simManager.timeText.gameObject.SetActive(false);
        string _c1 = dd1.captionText.text.ToString();
        string _c2 = dd2.captionText.text.ToString();
        int _e1 = int.Parse(if1.text);
        int _e2 = int.Parse(if2.text);
        GetRiotAPIRequest("12.10.1", "Garen", "Jax", 18360, 18360);
    }

    public void ManualSimulate()
    {
        Time.timeScale = 1f;
        SimManager.timer = 0;
        SimManager._timer = 0;
        simManager.ongoing = false;
        simManager.timeText.gameObject.SetActive(true);
        simManager.outputText.text = "";
        simManager.timeText.text = "";
        simManager.StartBattle();
    }

    public void LoadData(string data)
    {
        LoadingScreenHandler.Show();
        simManager.timeText.gameObject.SetActive(false);
        SimManager.timer = 0;
        SimManager._timer = 0;
        simManager.ongoing = false;
        simManager.outputText.text = "";
        simManager.timeText.text = "";
        Time.timeScale = 100f;
        LSSAPI = JsonConvert.DeserializeObject<LSSAPIResponse>(data);
        _champ1 = LSSAPI.APIMatchInfo.championInfo[0].champName;
        _champ2 = LSSAPI.APIMatchInfo.championInfo[1].champName;
        int _exp1 = ExpTable[(LSSAPI.APIMatchInfo.championInfo[0].champLevel)-1];
        int _exp2 = ExpTable[(LSSAPI.APIMatchInfo.championInfo[1].champLevel)-1];


        for (int i = 0; i < 6; i++)
        {
            champ1Items[i] = LSSAPI.APIMatchInfo.championInfo[0].items[i];
            champ2Items[i] = LSSAPI.APIMatchInfo.championInfo[1].items[i];
        }
        
        for (int i = 1; i < 66; i++)
        {
            for (int i2 = 0; i2 < 6; i2++)
            {
                if (champ1Items[i2] != 0)
                {
                    itemRequest.FetchStats(0, i2, champStats[0], i, champ1Items[i2]);
                }
        
                if (champ2Items[i2] != 0)
                {
                    itemRequest.FetchStats(1, i2, champStats[1], i, champ2Items[i2]);
                }
            }
        }

        champStats[0].GetComponent<ChampCombat>().PriorityList(_champ1);
        champStats[1].GetComponent<ChampCombat>().PriorityList(_champ2);

        apiURL = "https://lss-server-test.herokuapp.com/getSelectedChamps";

        StartCoroutine(MakeRiotAPIRequest1(_champ1, _exp1));
        StartCoroutine(MakeRiotAPIRequest2(_champ2, _exp2));

    }

    public void GetRiotAPIRequest(string ver, string champ1, string champ2, int exp1, int exp2)
    {        
        _champ1 = CapitalizeFirstLetter(champ1);
        _champ2 = CapitalizeFirstLetter(champ2);

        champStats[0].GetComponent<ChampCombat>().PriorityList(_champ1);
        champStats[1].GetComponent<ChampCombat>().PriorityList(_champ2);

        champ1 = CapitalizeFirstLetter(champ1).Trim().Replace(" ","").Replace("\u200B","");
        champ2 = CapitalizeFirstLetter(champ2).Trim().Replace(" ","").Replace("\u200B","");

        apiURL = "https://lss-server-test.herokuapp.com/getSelectedChamps";

        StartCoroutine(MakeRiotAPIRequest1(champ1,exp1));
        StartCoroutine(MakeRiotAPIRequest2(champ2,exp2));
    }

    //Champion 1
    IEnumerator MakeRiotAPIRequest1(string name, int _exp) 
    {
        myObj = new ChampionAPI();
        myObj.namesArr.Add(name);
        string jsonString = JsonUtility.ToJson(myObj);
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);
        WWW www = new WWW(apiURL, formData, postHeader);
        yield return www;
        champion = JsonConvert.DeserializeObject<RiotAPIResponse>(www.text);
        Load1 = true;

        SimulateFight(0, _champ1, _exp,0);
    }

    //Champion 2
    IEnumerator MakeRiotAPIRequest2(string name, int _exp)
    {
        myObj = new ChampionAPI();
        myObj.namesArr.Add(name);
        string jsonString = JsonUtility.ToJson(myObj);
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);
        WWW www = new WWW(apiURL, formData, postHeader);
        yield return www;
        champion = JsonConvert.DeserializeObject<RiotAPIResponse>(www.text);
        //Debug.Log(www.text);
        Load2 = true;

        SimulateFight(1, _champ2, _exp, 0);
    }

    public void SimulateFight(int num,string champName, int _exp, int _type)
    {
        ChampStats myStats = Champions[num].GetComponent<ChampStats>();
        //myStats.totalDamage = 0;
        //FOR TEST ONLY
        myStats.isLoaded = false;
        //
        for (int i = 0; i < skills.qSkills.Count; i++)
        {
            try
            {
                if (skills.qSkills[i].basic.champion.Replace(" ","") == champName)
                {
                    champStats[num].qSkill = skills.qSkills[i];
                }

                if (skills.wSkills[i].basic.champion.Replace(" ", "") == champName)
                {
                    champStats[num].wSkill = skills.wSkills[i];
                }

                if (skills.eSkills[i].basic.champion.Replace(" ", "") == champName)
                {
                    champStats[num].eSkill = skills.eSkills[i];
                }

                if (skills.rSkills[i].basic.champion.Replace(" ", "") == champName)
                {
                    champStats[num].rSkill = skills.rSkills[i];
                }
            }
            catch 
            { 
                //Debug.Log("No Skill Yet");
            }
        }

        //if (champStats[num].eSkill.chargeable)
        //{
        //    champStats[num].eSkill.charges = (int)champStats[num].eSkill.maxCharges[4];
        //    //myStats.eSkill.charges = (int)myStats.eSkill.maxCharges[4];
        //}

        for (int i = 0; i < skills.passives.Count; i++)
        {
            try
            {
                if (skills.passives[i].championName.Replace(" ", "") == champName)
                {
                    champStats[num].passiveSkill = skills.passives[i];
                }
            }
            catch { Debug.Log("No Skill Yet"); }
        }

        if (myStats.isLoaded) return;
        if (SimManager.isLoaded) return;
        myStats.name = champName;
        myStats.level = GetLevel(_exp);
        if(_type == 0)
        {
            myStats.baseHealth = (float)champion.ChampionsRes[0].champData.data.Champion.stats.hp;
            myStats.baseAD = (float)champion.ChampionsRes[0].champData.data.Champion.stats.attackdamage;
            myStats.baseArmor = (float)champion.ChampionsRes[0].champData.data.Champion.stats.armor;
            myStats.baseSpellBlock = (float)(champion.ChampionsRes[0].champData.data.Champion.stats.spellblock);
            myStats.baseAttackSpeed = (float)champion.ChampionsRes[0].champData.data.Champion.stats.attackspeed;

            myStats.maxHealth = myStats.baseHealth;
            myStats.AD = myStats.baseAD;
            myStats.armor = myStats.baseArmor;
            myStats.spellBlock = myStats.baseSpellBlock;
            myStats.attackSpeed = myStats.baseAttackSpeed;
        }
        else if(_type == 1)
        {
            myStats.maxHealth = myStats.baseHealth;
            myStats.AD = myStats.baseAD;
            myStats.armor = myStats.baseArmor;
            myStats.spellBlock = myStats.baseSpellBlock;
            myStats.attackSpeed = myStats.baseAttackSpeed;
        }
        else if(_type == 0 || _type == 2)
        {
            myStats.maxHealth = (float)champion.ChampionsRes[0].champData.data.Champion.stats.hp;
            myStats.AD = (float)champion.ChampionsRes[0].champData.data.Champion.stats.attackdamage;
            myStats.armor = (float)champion.ChampionsRes[0].champData.data.Champion.stats.armor;
            myStats.spellBlock = (float)(champion.ChampionsRes[0].champData.data.Champion.stats.spellblock);
            myStats.attackSpeed = (float)champion.ChampionsRes[0].champData.data.Champion.stats.attackspeed;
        }
        GetStatsByLevel(myStats);
        myStats.currentHealth = myStats.maxHealth;

        if (myStats.name == "Garen")
        {
            myStats.armor += 30;
            myStats.spellBlock += 30;
            myStats.armor *= 1.1f;
            myStats.spellBlock *= 1.1f;
        }

        if (myStats.name == "Aatrox")
        {
            myStats.PercentLifeStealMod = 26;
            myStats.PercentSpellVampMod = 26;
        }
        myStats.FinalizeStats(true);
        myStats.isMatchLoaded = true;

        if (champStats[0].isLoaded == true && champStats[1].isLoaded == true)
        {
            SimManager.isLoaded = true;
            StopAllCoroutines();
        }
    }

    public void LoadItems()
    {        
        //RiotAPIMatchResponse matchResponse = matchRequest.matchResponse;

        //int[] champExp = {0,0};
        //int[] champ1Items = {0,0,0,0,0,0};
        //int[] champ2Items = {0,0,0,0,0,0};

        //champExp[0] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].champExperience;
        //champExp[1] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].champExperience;
        //champ1Items[0] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item0;
        //champ1Items[1] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item1;
        //champ1Items[2] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item2;
        //champ1Items[3] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item3;
        //champ1Items[4] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item4;
        //champ1Items[5] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[0]].item5;
        //champ2Items[0] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item0;
        //champ2Items[1] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item1;
        //champ2Items[2] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item2;
        //champ2Items[3] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item3;
        //champ2Items[4] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item4;
        //champ2Items[5] = matchResponse.info.participants[RiotAPIMatchRequest.selectedChamp[1]].item5;

        for(int i = 1; i<66; i++)
        {
            for(int i2 = 0; i2<6; i2++)
            {
                if(champ1Items[i2] != 0)
                {
                    itemRequest.FetchStats(0,i2,champStats[0],i,champ1Items[i2]);
                }
            }
        }
        
        for(int i = 1; i<66; i++)
        {
            for(int i2 = 0; i2<6; i2++)
            {
                if(champ2Items[i2] != 0)
                {
                    itemRequest.FetchStats(1,i2,champStats[1],i,champ2Items[i2]);
                }
            }
        } 
    }

    int GetLevel(int _exp)
    {
        int _level = 0;
        for(int i = 0; i<MaxLevel; i++)
        {
            if(_exp >= ExpTable[i])
            {
                _level++;
            }
        }
        return _level;
    }

    void GetStatsByLevel(ChampStats myStats)
    {
        int _level = myStats.level;

        double[] mFactor ={0, 0.72, 1.4750575, 2.2650575, 3.09, 3.95, 4.8450575, 5.7750575, 6.74, 7.74, 8.7750575, 9.8450575, 10.95, 12.09, 13.2650575, 14.4750575, 15.72, 17};
        if(_level == 1) return;
        myStats.maxHealth += (float)(champion.ChampionsRes[0].champData.data.Champion.stats.hpperlevel * mFactor[_level-1]);
        myStats.attackSpeed = (float)(myStats.baseAttackSpeed * (1 + (champion.ChampionsRes[0].champData.data.Champion.stats.attackspeedperlevel * (_level - 1)) / 100));
        myStats.armor += ((float) (champion.ChampionsRes[0].champData.data.Champion.stats.armorperlevel) * (float)  mFactor[_level-1]);
        myStats.AD += ((float)  (champion.ChampionsRes[0].champData.data.Champion.stats.attackdamageperlevel) * (float)  mFactor[_level-1]);
        myStats.spellBlock += ((float)  (champion.ChampionsRes[0].champData.data.Champion.stats.spellblockperlevel) * (float)  mFactor[_level-1]);
    }

    public static string ucfirst(string s)
    {
        bool IsNewSentense = true;
        var result = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            if (IsNewSentense && char.IsLetter(s[i]))
            {
                result.Append (char.ToUpper (s[i]));
                IsNewSentense = false;
            }
            else
                result.Append (s[i]);

            if (s[i] == '!' || s[i] == '?' || s[i] == '.')
            {
                IsNewSentense = true;
            }
        }

        return result.ToString().Replace("\u200B", "");
    }

    static string CapitalizeFirstLetter(string value)
        {
            //In Case if the entire string is in UpperCase then convert it into lower
            value = value.ToLower();
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
}

[System.Serializable]
public class ChampionAbilities
{    
    public TextMeshProUGUI[] champSkills;
}

[System.Serializable]
public class AllAbilities
{
    public string[] name;
}

public class ChampionAPI
{
    public List<string> namesArr = new List<string>();
}
