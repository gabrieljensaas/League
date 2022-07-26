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

        StartCoroutine(LoadData());
    }

    public void CreateAsset(int num, string champName, string skillId, SkillList asset)
    {
        string _folderName = folderName[num];

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Skills/"+ver+"/" + _folderName+ "/"+champName + " "+_folderName+"["+ skillId + "].asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        ReferenceEquals(asset,this);
    }

    IEnumerator LoadData()
    {
        for (int i = 0; i < champions.Length; i++)
        {
            if (champions[i] == "GnarBig") continue;
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(SetStats(champions[i]));            
        }
    }

    IEnumerator SetStats(string champName)
    {
        string url = "https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/champions/" + champName + ".json";
        WWW www = new WWW(url);
        yield return www;
        api = JsonConvert.DeserializeObject<RiotAPIChampionDataResponse>(www.text);

        var QSkill = Resources.Load("Skills/" + ver + "/Q/" + champName);
        var WSkill = Resources.Load("Skills/" + ver + "/W/" + champName);
        var ESkill = Resources.Load("Skills/" + ver + "/E/" + champName);
        var RSkill = Resources.Load("Skills/" + ver + "/R/" + champName);
        var PSkill = Resources.Load("Skills/" + ver + "/P/" + champName);

        #region Q
        for (int i = 0; i < api.abilities["Q"].Count; i++)
        {
            Debug.Log(api.key + ": " + api.abilities["Q"][i].name);
            SkillList asset = ScriptableObject.CreateInstance<SkillList>();
            asset.name = champName;
            //asset.damage.percentAP = api.effects.

            if (QSkill == null)
            {
                CreateAsset(0, champName,i.ToString(), asset);
            }
        }
        #endregion

        #region W
        for (int i = 0; i < api.abilities["W"].Count; i++)
        {
            Debug.Log(api.key + ": " + api.abilities["W"][i].name);
            SkillList asset = ScriptableObject.CreateInstance<SkillList>();

            if (WSkill == null)
            {
                CreateAsset(1, champName, i.ToString(), asset);
            }
        }
        #endregion

        #region E
        for (int i = 0; i < api.abilities["E"].Count; i++)
        {
            Debug.Log(api.key + ": " + api.abilities["E"][i].name);
            SkillList asset = ScriptableObject.CreateInstance<SkillList>();

            if (ESkill == null)
            {
                instance.CreateAsset(2, champName, i.ToString(), asset);
            }
        }
        #endregion

        #region R
        for (int i = 0; i < api.abilities["R"].Count; i++)
        {
            Debug.Log(api.key + ": " + api.abilities["R"][i].name);
            SkillList asset = ScriptableObject.CreateInstance<SkillList>();

            if (RSkill == null)
            {
                instance.CreateAsset(3, champName, i.ToString(), asset);
            }
        }
        #endregion

        #region Passive
        for (int i = 0; i < api.abilities["P"].Count; i++)
        {
            Debug.Log(api.key + ": " + api.abilities["P"][i].name);
            SkillList asset = ScriptableObject.CreateInstance<SkillList>();

            if (PSkill == null)
            {
                instance.CreateAsset(4, champName, i.ToString(), asset);
            }
        }
        #endregion

    }

}
