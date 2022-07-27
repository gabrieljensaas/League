using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public string[] champions = {
  "Aatrox",
  "Ahri",
  "Akali",
  "Akshan",
  "Alistar",
  "Amumu",
  "Anivia",
  "Annie",
  "Aphelios",
  "Ashe",
  "AurelionSol",
  "Azir",
  "Bard",
  "Belveth",
  "Blitzcrank",
  "Brand",
  "Braum",
  "Caitlyn",
  "Camille",
  "Cassiopeia",
  "Chogath",
  "Corki",
  "Darius",
  "Diana",
  "DrMundo",
  "Draven",
  "Ekko",
  "Elise",
  "Evelynn",
  "Ezreal",
  "Fiddlesticks",
  "Fiora",
  "Fizz",
  "Galio",
  "Gangplank",
  "Garen",
  "Gnar",
  "GnarBig",
  "Gragas",
  "Graves",
  "Gwen",
  "Hecarim",
  "Heimerdinger",
  "Illaoi",
  "Irelia",
  "Ivern",
  "Janna",
  "JarvanIV",
  "Jax",
  "Jayce",
  "Jhin",
  "Jinx",
  "Kaisa",
  "Kalista",
  "Karma",
  "Karthus",
  "Kassadin",
  "Katarina",
  "Kayle",
  "Kayn",
  "Kennen",
  "Khazix",
  "Kindred",
  "Kled",
  "KogMaw",
  "Leblanc",
  "LeeSin",
  "Leona",
  "Lillia",
  "Lissandra",
  "Lucian",
  "Lulu",
  "Lux",
  "Malphite",
  "Malzahar",
  "Maokai",
  "MasterYi",
  "MissFortune",
  "MonkeyKing",
  "Mordekaiser",
  "Morgana",
  "Nami",
  "Nasus",
  "Nautilus",
  "Neeko",
  "Nidalee",
  "Nilah",
  "Nocturne",
  "Nunu",
  "Olaf",
  "Orianna",
  "Ornn",
  "Pantheon",
  "Poppy",
  "Pyke",
  "Qiyana",
  "Quinn",
  "Rakan",
  "Rammus",
  "RekSai",
  "Rell",
  "Renata",
  "Renekton",
  "Rengar",
  "Riven",
  "Rumble",
  "Ryze",
  "Samira",
  "Sejuani",
  "Senna",
  "Seraphine",
  "Sett",
  "Shaco",
  "Shen",
  "Shyvana",
  "Singed",
  "Sion",
  "Sivir",
  "Skarner",
  "Sona",
  "Soraka",
  "Swain",
  "Sylas",
  "Syndra",
  "TahmKench",
  "Taliyah",
  "Talon",
  "Taric",
  "Teemo",
  "Thresh",
  "Tristana",
  "Trundle",
  "Tryndamere",
  "TwistedFate",
  "Twitch",
  "Udyr",
  "Urgot",
  "Varus",
  "Vayne",
  "Veigar",
  "Velkoz",
  "Vex",
  "Vi",
  "Viego",
  "Viktor",
  "Vladimir",
  "Volibear",
  "Warwick",
  "Xayah",
  "Xerath",
  "XinZhao",
  "Yasuo",
  "Yone",
  "Yorick",
  "Yuumi",
  "Zac",
  "Zed",
  "Zeri",
  "Ziggs",
  "Zilean",
  "Zoe",
  "Zyra"
    };
    RiotAPIChampionDataResponse api = new RiotAPIChampionDataResponse();
    //RiotOptimized api = new RiotOptimized();
    public static Skills instance;
    public List<PassiveList> passives;
    public List<SkillList> qSkills;
    public List<SkillList> wSkills;
    public List<SkillList> eSkills;
    public List<SkillList> rSkills;

    public string[] folderName = { "Q", "W", "E", "R", "P" };
    public string ver = "12.13.1";

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        #region Skills Loading
        Object[] obj;

        //Load All Q
        obj = Resources.LoadAll("Skills/12.13.1/Q", typeof(SkillList));
        foreach(SkillList skill in obj)
        {
            qSkills.Add(skill);
        }

        //Load All W
        obj = Resources.LoadAll("Skills/12.13.1/W", typeof(SkillList));
        foreach (SkillList skill in obj)
        {
            wSkills.Add(skill);
        }

        //Load All E
        obj = Resources.LoadAll("Skills/12.13.1/E", typeof(SkillList));
        foreach (SkillList skill in obj)
        {
            eSkills.Add(skill);
        }

        //Load All R
        obj = Resources.LoadAll("Skills/12.13.1/R", typeof(SkillList));
        foreach (SkillList skill in obj)
        {
            rSkills.Add(skill);
        }

        //Load All P
        obj = Resources.LoadAll("Skills/12.13.1/P", typeof(PassiveList));
        foreach (PassiveList skill in obj)
        {
            passives.Add(skill);
        }
#endregion

        StartCoroutine(LoadData());
    }

    public void CreateAsset(int num, string champName, string skillId, SkillList asset, PassiveList passive)
    {
        string _folderName = folderName[num];
        Debug.Log(passive);
        if (asset != null)
        {
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Skills/" + ver + "/" + _folderName + "/" + champName + " " + _folderName + "[" + skillId + "].asset");
        }
        else
        {
            AssetDatabase.CreateAsset(passive, "Assets/Resources/Skills/" + ver + "/" + _folderName + "/" + champName + " " + _folderName + "[" + skillId + "].asset");
        }
        AssetDatabase.SaveAssets();
    }

    IEnumerator LoadData()
    {
        for (int i = 0; i < champions.Length; i++)
        {
            if (champions[i] == "GnarBig") continue;
            StartCoroutine(SetStats(champions[i]));            
        }
        yield return null;
    }

    IEnumerator SetStats(string champName)
    {
        string url = "https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/champions/" + champName + ".json";
        WWW www = new WWW(url);
        yield return www;
        api = JsonConvert.DeserializeObject<RiotAPIChampionDataResponse>(www.text);

        var QSkill = Resources.Load<SkillList>("Skills/" + ver + "/Q/" + champName + " Q[0]");
        var WSkill = Resources.Load<SkillList>("Skills/" + ver + "/W/" + champName + " W[0]");
        var ESkill = Resources.Load<SkillList>("Skills/" + ver + "/E/" + champName + " E[0]");
        var RSkill = Resources.Load<SkillList>("Skills/" + ver + "/R/" + champName + " R[0]");
        var PSkill = Resources.Load<PassiveList>("Skills/" + ver + "/P/" + champName + " P[0]");

        if (QSkill == null) QSkill = Resources.Load<SkillList>("Skills/" + ver + "/Q/" + champName + " Q[1]");
        if (WSkill == null) WSkill = Resources.Load<SkillList>("Skills/" + ver + "/W/" + champName + " W[1]");
        if (ESkill == null) ESkill = Resources.Load<SkillList>("Skills/" + ver + "/E/" + champName + " E[1]");
        if (RSkill == null) RSkill = Resources.Load<SkillList>("Skills/" + ver + "/R/" + champName + " R[1]");
        if (PSkill == null) PSkill = Resources.Load<PassiveList>("Skills/" + ver + "/P/" + champName + " P[1]");

        #region Q
        for (int i = 0; i < api.abilities["Q"].Count; i++)
        {
            //Debug.Log(api.key + ": " + api.abilities["Q"][i].name);

            if (QSkill == null)
            {
                SkillList asset = ScriptableObject.CreateInstance<SkillList>();
                asset.basic = new SkillBasic();
                asset.basic.champion = champName;
                asset.basic.name = api.abilities["Q"][i].name;
                InitializeSkill(champName, i, asset, null, 0);
            }
        }
        #endregion

        #region W
        for (int i = 0; i < api.abilities["W"].Count; i++)
        {
            //Debug.Log(api.key + ": " + api.abilities["W"][i].name);

            if (WSkill == null)
            {
                SkillList asset = ScriptableObject.CreateInstance<SkillList>();
                asset.basic = new SkillBasic();
                asset.basic.champion = champName;
                asset.basic.name = api.abilities["W"][i].name;
                InitializeSkill(champName, i, asset, null, 1);
            }
        }
        #endregion

        #region E
        for (int i = 0; i < api.abilities["E"].Count; i++)
        {
            //Debug.Log(api.key + ": " + api.abilities["E"][i].name);

            if (ESkill == null)
            {
                SkillList asset = ScriptableObject.CreateInstance<SkillList>();
                asset.basic = new SkillBasic();
                asset.basic.champion = champName;
                asset.basic.name = api.abilities["E"][i].name;
                InitializeSkill(champName, i, asset, null, 2);
            }
        }
        #endregion

        #region R
        for (int i = 0; i < api.abilities["R"].Count; i++)
        {
            //Debug.Log(api.key + ": " + api.abilities["R"][i].name);

            if (RSkill == null)
            {
                SkillList asset = ScriptableObject.CreateInstance<SkillList>();
                asset.basic = new SkillBasic();
                asset.basic.champion = champName;
                asset.basic.name = api.abilities["R"][i].name;
                InitializeSkill(champName, i, asset, null, 3);
            }
        }
        #endregion

        #region Passive
        for (int i = 0; i < api.abilities["P"].Count; i++)
        {
            //Debug.Log(api.key + ": " + api.abilities["P"][i].name);

            if (PSkill == null)
            {
                PassiveList asset = ScriptableObject.CreateInstance<PassiveList>();
                asset.championName = champName;
                InitializeSkill(champName, i, null, asset, 4);
            }
        }
        #endregion

    }
    void InitializeSkill(string champName, int i, SkillList skill, PassiveList passive, int skillIndex)
    {
        string[] skillType = { "Q", "W", "E", "R", "P" };

        if (skill != null)
        {
            try
            {
                skill.basic.castTime = float.Parse(api.abilities[skillType[skillIndex]][i].castTime);
            }
            catch
            {
                skill.basic.castTime = 0;
            }

            if (api.abilities[skillType[skillIndex]][i].cooldown != null)
            {
                for (int i2 = 0; i2 < api.abilities[skillType[skillIndex]][i].cooldown.modifiers[0].values.Count; i2++)
                {
                    try
                    {
                        skill.basic.coolDown[i2] = (float)api.abilities[skillType[skillIndex]][i].cooldown.modifiers[0].values[i2];
                    }
                    catch
                    {
                        if (api.abilities[skillType[skillIndex]][i].targeting == "Passive") skill.basic.inactive = true;
                        if (i2 >= 5) continue;
                        skill.basic.coolDown[i2] = 0;
                    }
                }
            }
            else
            {
                skill.basic.coolDown[i] = 0;
                skill.basic.inactive = true;
            }
            CreateAsset(skillIndex, champName, i.ToString(), skill, null);
        }
        else
        {

            try
            {
                passive.castTime = float.Parse(api.abilities[skillType[skillIndex]][i].castTime);
            }
            catch
            {
                passive.castTime = 0;
            }

            if (api.abilities[skillType[skillIndex]][i].cooldown != null)
            {
                for (int i2 = 0; i2 < api.abilities[skillType[skillIndex]][i].cooldown.modifiers[0].values.Count; i2++)
                {
                    try
                    {
                        passive.coolDown = (float)api.abilities[skillType[skillIndex]][i].cooldown.modifiers[0].values[i2];
                    }
                    catch
                    {
                        if (api.abilities[skillType[skillIndex]][i].targeting == "Passive") passive.inactive = true;
                        if (i2 >= 5) continue;
                        passive.coolDown = 0;
                    }
                }
            }
            else
            {
                passive.coolDown = 0;
                passive.inactive = true;
            }
            CreateAsset(skillIndex, champName, i.ToString(), null, passive);
        }
    }

}
