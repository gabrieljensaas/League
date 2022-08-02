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

    public string[] attributesName = {
        "First Cast Damage",
  "First Sweetspot Damage",
  "Second Cast Damage",
  "Second Sweetspot Damage",
  "Third Cast Damage",
  "Third Sweetspot Damage",
  "Physical Damage",
  "Minion Damage",
  "Total Damage",
  "Healing",
  "World Ender Increased Healing",
  "Bonus Movement Speed",
  "Bonus AD",
  "Increased Healing",
  "Damage Per Pass",
  "Total Mixed Damage",
  "Magic Damage",
  "Additional Magic Damage",
  "Total Single Target Damage",
  "Disable Duration",
  "Magic damage",
  "Shroud Duration",
  "Total Magic Damage",
  "Minimum Magic Damage",
  "Maximum Magic Damage",
  "Total Physical Damage",
  "Non-Champion Damage",
  "Physical Damage per Shot",
  "Maximum Bullets Stored",
  "Minimum Physical Damage per Bullet",
  "Maximum Physical Damage per Bullet",
  "Minimum Charged Physical Damage",
  "Damage to target with 67% missing hp",
  "Magic Damage Per Tick",
  "Damage Reduction",
  "Physical Damage Reduction",
  "Stun Duration",
  "Width",
  "Number of ice segments",
  "Distance between outermost segments",
  "Distance between individual segments",
  "Enhanced Damage",
  "Magic Damage per Tick",
  "Slow",
  "Empowered Damage per Tick",
  "Empowered Slow",
  "Shield",
  "Initial Magic Damage",
  "Bonus Attack Damage",
  "Bonus Attack Speed",
  "Lethality",
  "Damage Per Arrow",
  "Total Damage Per Flurry",
  "Arrows",
  "Secondary Magic Damage",
  "Minimum Stun Duration",
  "Maximum Stun Duration",
  "Bonus Damage",
  "Static Movement Speed",
  "Increased Attack Speed",
  "Shield Strength",
  "Width (charge)",
  "Width (impassable wall)",
  "Minimum Heal",
  "Maximum Heal",
  "Monster Damage",
  "Modified Minion Damage",
  "Slow Duration",
  "Minimum Physical Damage per hit",
  "Maximum Physical Damage per hit",
  "Minimum Monster Damage per hit",
  "Maximum Monster Damage per hit",
  "Bonus True Damage",
  "Maximum Monster Damage",
  "True Damage",
  "Bonus Health",
  "Increased Total Attack Speed",
  "Heal",
  "Attack Speed",
  "Movement Speed",
  "Bolt Magic Damage",
  "Detonation Magic Damage",
  "Increased Damage",
  "Total Single-Target Damage",
  "Ally Bonus Armor",
  "Ally Bonus Magic Resistance",
  "Self Bonus Armor",
  "Self Bonus Magic Resistance",
  "Damage reduction",
  "Duration",
  "Maximum Knockup Duration",
  "Reduced Damage",
  "Trap Duration",
  "Maximum Traps",
  "Headshot Damage Increase",
  "Physical damage",
  "Bonus Physical Damage",
  "Increased Mixed Damage",
  "Outer Cone Bonus Damage",
  "Non-Epic Monster Damage",
  "Bonus Non-Epic Monster Damage",
  "Zone Duration",
  "Bonus Magic Damage",
  "Reduced Healing",
  "Silence Duration",
  "Champion True Damage",
  "Non-Champion True Damage",
  "Bonus Health Per Stack",
  "Bonus Attack Range Per Stack",
  "Bonus Size Per Stack",
  "Mixed Damage Per Tick",
  "Resistance Reduction Per Tick",
  "Total Resistance Reduction",
  "Big One Magic Damage",
  "Blade Physical Damage",
  "Handle Physical Damage",
  "Armor Penetration",
  "Bonus Damage Per Stack",
  "Maximum True Damage",
  "Total Shield Strength",
  "Magic Damage per Orb",
  "Bonus Damage Per Champion",
  "Total Damage Vs. 5 Champions",
  "Minimum Damage",
  "Capped Monster Damage",
  "Damage Stored",
  "Maximum Additional Bonus AD",
  "Maximum Total Bonus AD",
  "Minimum Bonus Physical Damage",
  "Maximum Bonus Physical Damage",
  "Minimum Non-Champion Bonus Damage",
  "Maximum Non-Champion Bonus Damage",
  "Increased Base Health",
  "Total Health Regeneration",
  "Regeneration per Second",
  "Minimum Physical Damage",
  "Minimum Total Damage",
  "Outward Magic Damage",
  "Returning Magic Damage",
  "Max. Monster Total Damage",
  "Dart Damage",
  "Total Bonus Damage",
  "Spike Damage",
  "Maximum Damage",
  "Charm Duration",
  "Monster Duration",
  "Magic Resistance Reduction",
  "Empowered Damage",
  "Fear Duration",
  "Increased Magic Damage",
  "Increased Minimum Damage",
  "Last Tick of Damage",
  "Champion Heal Percentage",
  "Total Heal per Champion",
  "Total Heal per Monster",
  "Total Heal per Minion",
  "Critical damage",
  "Additional Bonus Movement Speed",
  "Heal per Tick",
  "Mana Refunded",
  "Bonus On-Hit Damage",
  "Guppy Damage",
  "Chomper Damage",
  "Gigalodon Damage",
  "Gust Magic Damage",
  "Tornado Magic Damage Per Tick",
  "Total Tornado Magic Damage",
  "Magic Damage Shield",
  "Magic Damage Reduction",
  "Champion Magic Damage",
  "Non-Champion Magic Damage",
  "Critical Damage",
  "(bug)",
  "Gold Plunder",
  "Silver Serpent Plunder",
  "Maximum charges",
  "Champion Bonus Damage",
  "Magic Damage Per Wave",
  "Magic Damage Per Cluster",
  "True Damage with Death's Daughter",
  "Total Mixed Damage with Death's Daughter",
  "Total Magic Damage with Fire at Will",
  "Maximum Mixed Total Damage with Fire at Will and Death's Daughter",
  "Movement Speed Duration",
  "Physical Damage Per Spin",
  "Increased Damage Per Spin",
  "Hyper Movement Speed",
  "Bonus Movespeed",
  "Total Movespeed",
  "Bonus Range",
  "Maximum Minion Damage",
  "Maximum Slow",
  "Maximum Damage to Monsters",
  "Initial Physical Damage",
  "Detonation Damage",
  "Bonus Armor",
  "Maximum Armor",
  "Explosion Damage",
  "Damage per Snip",
  "Center Damage per Snip",
  "Final Snip Damage",
  "Final Snip Center Damage",
  "Minimum Center Damage",
  "Maximum Center Damage",
  "Bonus Resistances",
  "Cooldown Refund",
  "Magic Damage per Needle",
  "Damage with Thousand Cuts",
  "Reduced Slow",
  "Second Cast Total Damage",
  "Third Cast Total Damage",
  "Maximum Total Damage",
  "Minion damage",
  "Capped Healing",
  "Maximum Physical Damage",
  "Initial Rocket Magic Damage",
  "Additional Damage",
  "Additional Minion Damage",
  "Total Minion Damage",
  "Damage Increase",
  "Damage Transmission",
  "Cooldown Reduction",
  "Barrage Damage",
  "Perimeter Damage",
  "Root Duration",
  "Bonus Magic Damage Per Second",
  "Heal Per Tick",
  "Total Heal",
  "Armor Reduction",
  "Base Shield",
  "Bonus Magic Resistance",
  "Mana Restore",
  "Maximum Attack Speed",
  "Maximum Secondary Damage",
  "Minimum Secondary Damage",
  "Physical Damage Per Missile",
  "Reduced Damage Per Missile",
  "Total Evolved Single-Target Damage",
  "Minimum Movement Speed",
  "Maximum Movement Speed",
  "Maximum Non-Champion Damage",
  "Damage per Additional Spear",
  "Knockup Duration",
  "Enhanced Monster Damage",
  "Wall Length",
  "Mana Restored",
  "Damage Per Second",
  "Magic Shield",
  "Increased Bonus Magic Damage",
  "Mana Restored Against Champions",
  "Bonus Movement speed",
  "Physical Damage Per Dagger",
  "Maximum Single-Target Physical Damage",
  "Magic Damage Per Dagger",
  "Maximum Single-Target Magic Damage",
  "On-Attack/On-Hit Effectiveness",
  "Passive Damage",
  "Invulnerability Duration",
  "Total Non-Champion Damage",
  "Capped Monster Damage per Hit",
  "Total Capped Monster Damage",
  "Magic Damage Per Bolt",
  "Isolation Physical Damage",
  "Static Cooldown",
  "Additional Physical Damage",
  "Enhanced damage below threshold",
  "Tether Damage",
  "Minion and Small Monster Damage",
  "Pull Damage",
  "Maximum Damage Against Monsters",
  "Minimum Shield",
  "Maximum Shield",
  "Resistances Reduction",
  "Bonus Attack Range",
  "Delayed Damage",
  "Mark Damage",
  "Root Damage",
  "Collision Damage",
  "Flat Damage Reduction",
  "Maximum Bonus Movement Speed",
  "Increased Minion Damage",
  "Sleep Duration",
  "Minimum Self-Heal",
  "Maximum Self-Heal",
  "Physical Damage Per Shot",
  "Minion Damage Per Shot",
  "Increased Armor",
  "Cripple",
  "Voidling Duration",
  "Damage Per Tick",
  "Total Increased Damage",
  "Increased Damage Per Tick",
  "Reduced Damage per hit",
  "Maximum Single-Target Damage",
  "Monster Bonus Damage",
  "Monster Total Damage",
  "Reduced Monster Damage per hit",
  "Max Monster Single-Target Damage",
  "Minimum Healing Per Half Second",
  "Maximum Healing Per Half Second",
  "Minimum Total Healing",
  "Maximum Total Healing",
  "Turret Damage Reduction",
  "Increased Bonus Movement Speed",
  "Total Waves",
  "Maximum Total Physical Damage",
  "Wave Interval Time",
  "Clone Damage",
  "Shield to Healing",
  "Magic Penetration",
  "Minimum Damage Per Tick",
  "Maximum Damage Per Tick",
  "Bonus Magic Damage Per Hit",
  "Total Bonus Magic Damage",
  "Additional Slow Per Second",
  "Maximum Cripple",
  "Additional Cripple Per Second",
  "Knock Up Duration",
  "Additional Bloom Damage",
  "Total Maximum Damage",
  "Passive Movement Speed",
  "Active Movement Speed",
  "Empowered Root Duration",
  "Bonus Shield Per Champion",
  "Total Shield vs. 5 Champions",
  "Whirl Total Physical Damage",
  "Physical Damage per Tick",
  "Burst Physical Damage",
  "Enhanced Bonus Attack Speed",
  "Base Healing from Non-Champions",
  "Empowered Healing from Non-Champions",
  "Champion Damage",
  "Base Healing from Champions",
  "Empowered Healing from Champions",
  "Minimum Explosion Damage",
  "Maximum Explosion Damage",
  "Minimum Rollover Damage",
  "Maximum Rollover Damage",
  "Damage Per Snowball",
  "Damage Per Volley",
  "Movement Speed Modifier",
  "Total Minimum/Minion Damage",
  "Minimum/Minion Damage Per Instance",
  "Total Monster Damage",
  "Monster Damage Per Instance",
  "Secondary Physical Damage",
  "Increased Secondary Damage",
  "Slam Damage",
  "Maximum Initial Monster Damage",
  "Uncharged Physical Damage",
  "Capped percentage monster damage",
  "Total Movement Speed Increase",
  "Taunt Duration",
  "Bonus Attack Speed Duration",
  "Impact Magic Damage",
  "Center Minimum Damage",
  "Center Maximum Damage",
  "Aftershock Magic Damage",
  "Maximum Aftershock Damage",
  "Increased Total Damage",
  "Maximum Turret Damage",
  "Total Turret Damage",
  "Impact Turret Damage",
  "Aftershock Turret Damage",
  "Total Aftershock Turret Damage",
  "Secondary Damage",
  "Heal Per Champion",
  "Total Magic Damage Per Target",
  "Maximum Bonus Attack Speed",
  "Berserk Duration",
  "Enhanced Physical Damage",
  "Healing Cap",
  "Enhanced Healing Cap",
  "Non-Champion Healing",
  "Enhanced Non-Champion Healing",
  "Champion Healing",
  "Enhanced Champion Healing",
  "Physical Damage Per Hit",
  "Empowered Bonus Physical Damage",
  "Empowered Magic Damage",
  "Empowered Physical Damage",
  "Minion Damage Percentage",
  "Minion Damage Per Tick",
  "Total Enhanced Damage",
  "Enhanced Damage Per Tick",
  "Total Enhanced Minion Damage",
  "Enhanced Shield Strength",
  "Enhanced Bonus Movement Speed",
  "Total Slow",
  "Enhanced Magic Damage",
  "Total Enhanced Magic Damage",
  "Enhanced Slow",
  "Total Enhanced Slow",
  "Minimum Magic damage",
  "Bonus Overload Damage",
  "Slash Damage",
  "Total Damage Per Target",
  "Total Damage Per Minion",
  "Swing Damage",
  "Lash Damage",
  "Effect Duration",
  "Heal Per Ally",
  "Damage",
  "Invisibility Duration",
  "Champion Disable Duration",
  "Mini-Box Magic Damage",
  "Mini-Box Increased Damage",
  "Increased Bonus Damage",
  "Bonus Movement Speed Decay",
  "Fury Generation per Second",
  "Size Increase",
  "Slow Strength",
  "Bonus Stats",
  "Total Regeneration",
  "Max. Base Damage Increase",
  "Minimum Monster Damage",
  "Minimum Minion Damage",
  "Champion Maximum Damage",
  "Bounce Damage",
  "Minion Bounce Damage",
  "Buff Duration",
  "Initial Bonus Movement Speed",
  "Minimum Damage Blocked",
  "Aura Bonus Movement Speed",
  "Health Cost Reduction",
  "Reduced Health Cost",
  "Enhanced Heal",
  "Bonus Damage Per Additional Bolt",
  "Reveal Duration",
  "Non-Champion Detonation Damage",
  "Maximum Champion Damage",
  "Magic Damage per Sphere",
  "Damage Stored into Grey Health",
  "Increased Damage Stored into Grey Health",
  "Primary Target Damage",
  "Secondary Target Damage",
  "Minimum Detonation Damage",
  "Maximum Detonation Damage",
  "Return Physical Damage",
  "Maximum Charges",
  "Blind Duration",
  "Increased Blind Duration",
  "Passive Bonus Movement Speed",
  "Active Bonus Movement Speed",
  "Magic Damage On-Hit",
  "Damage per Tick",
  "Total DoT Damage",
  "Monster Damage On-Hit",
  "Monster Damage per Tick",
  "Total DoT Monster Damage",
  "Bounce Range",
  "Minimum Bonus Magic Damage",
  "Maximum Bonus Magic Damage",
  "Knockback Distance",
  "Attack Damage Reduction",
  "Magic Damage Per Second",
  "Bonus AD Per Missing Health",
  "Maximum Bonus AD",
  "Heal Per Fury",
  "AD Reduction",
  "Fury Gained",
  "Minimum Health Threshold",
  "Stealth Duration",
  "Base Physical Damage",
  "Physical Damage Per Stack",
  "Minimum Mixed Damage",
  "Maximum Mixed Damage",
  "Total Bonus Physical Damage",
  "Bonus Physical Damage per Tick",
  "Cone Damage",
  "Modified Physical Damage",
  "Minimum Fully Reduced Damage",
  "Maximum Fully Reduced Damage",
  "Bonus Magic Damage per Stack",
  "Minimum True Damage",
  "Tumble Cooldown Reduction",
  "Mana Restore per Kill",
  "Maximum Mana Restored",
  "Minimum Bonus Damage",
  "Physical Damage to Monsters",
  "Discharge Damage",
  "Reduced Heal",
  "Increased Movement Speed",
  "Minion Heal",
  "Turret Disable Duration",
  "Cap Against Monsters",
  "Healing Percentage",
  "Damage Per Blade",
  "Minimum Damage Per Blade",
  "Physical Damage Per Feather",
  "Minion Damage Per Feather",
  "Number of Recasts",
  "Distance",
  "Thrust Damage",
  "Wall Width",
  "Maximum Bonus Damage",
  "Wall Health",
  "Mist Walkers",
  "Bonus Ability Power",
  "Adaptive Force",
  "Adaptive Force per 100 bonus AD",
  "Adaptive Force per 100 AP",
  "Reduced Damage Per Wave",
  "Capped Damage",
  "Maximum Range Channel Duration",
  "Chunk Healing",
  "Energy Restored",
  "Physical Damage per Bullet",
  "Pierce Damage",
  "Demolition Threshold",
  "Magic Damage per Mine",
  "Reduced Damage per Mine",
  "Bonus Movement Speed Duration"
    };

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
        //Debug.Log(passive);
        if (asset != null)
        {
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Skills/" + ver + "/" + _folderName + "/" + champName + " " + _folderName + "[" + skillId + "].asset");
        }
        else
        {
            AssetDatabase.CreateAsset(passive, "Assets/Resources/Skills/" + ver + "/" + _folderName + "/" + champName + " " + _folderName + "[" + skillId + "].asset");
        }
        AssetDatabase.SaveAssets();
        // #endif
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
                asset.unit = new UnitList();
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
                asset.unit = new UnitList();
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
                asset.unit = new UnitList();
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
                asset.unit = new UnitList();
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

            var effectInAbilities = api.abilities[skillType[skillIndex]][i].effects;
            var limit = 5;

            if(skillIndex == 3) {
                limit = 3;
            }

            for (int i3 = 0; i3 < limit; i3++)
            {
                for (int effect = 0; effect < effectInAbilities.Count; effect++)
                {
                    if (effectInAbilities[effect].leveling.Count > 0)
                    {
                        for (int level = 0; level < effectInAbilities[effect].leveling.Count; level++)
                        {
                            for (int modifier = 0; modifier < effectInAbilities[effect].leveling[level].modifiers.Count; modifier++)
                            {
                                foreach(var atr in attributesName) {                                    
                                    if (effectInAbilities[effect].leveling[level].attribute == atr)
                                    {
                                        var unit = effectInAbilities[effect].leveling[level].modifiers[modifier].units[0];
                                        var value = effectInAbilities[effect].leveling[level].modifiers[modifier].values[i3];

                                        if (unit == "" || unit == "flat")
                                        {                                            
                                            skill.unit.flat[i3] = (float)value;
                                        }
                                        if (unit == "% AD")
                                        {
                                            skill.unit.percentAD[i3] = (float)value;
                                        }
                                        if (unit == "%")
                                        {
                                            skill.unit.percent[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% AP" ||
                                            unit == " (+ 100% AP)"
                                        )
                                        {
                                            skill.unit.percentAP[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% bonus AD" ||
                                            unit == "%  bonus AD" ||
                                            unit == "[ 1% per 35 ][ 2.86% per 100 ]bonus AD"
                                        )
                                        {
                                            skill.unit.percentBonusAD[i3] = (float)value;
                                        }
                                        if (
                                            unit == "  ×" || 
                                            unit == "  × "
                                        )
                                        {
                                            skill.unit.x[i3] = (float)value;
                                        }
                                        if (
                                            unit == "1 + 0.3 per 100% bonus attack speed" || 
                                            unit == "% per 100% bonus attack speed"
                                        )
                                        {
                                            skill.unit.percentBonusAS[i3] = (float)value;
                                        }
                                        if (
                                            unit == "1 + (0.5 + 0.175) per 100% critical strike chance" ||
                                            unit == "% (+ 0% − 25% (based on critical strike chance)" ||
                                            unit == "1 + (100% + 0%) critical strike chance"
                                        )
                                        {
                                            skill.unit.percentCritStrikeChance[i3] = (float)value;
                                        }                                       
                                        if (
                                            unit == "%  of target's maximum health" ||
                                            unit == "% of target's maximum health" ||
                                            unit == "%  of the target's maximum health"  ||
                                            unit == "% (+ 0.25% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 0.46% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 0.7% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 0.8% per 100 AP) of the target's maximum health" ||
                                            unit == "% (+ 1.4% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 1.5% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 1.6% per 100 AP) of the target's maximum health" ||
                                            unit == "% (+ 2% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 2.4% per 100 AP) of the target's maximum health" ||
                                            unit == "% (+ 4% per 100 AP) of the target's maximum health" ||
                                            unit == "% (+ 4.5% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 6% per 100 AP) of target's maximum health" ||
                                            unit == "% (+ 7.2% per 100 AP) of the target's maximum health" ||
                                            unit == "% (+ 0.5% per Feast stack) of target's maximum health" ||
                                            unit == "% (+ 1.5% per Feast stack) of target's maximum health" ||
                                            unit == "% (+ 5% per 100 bonus AD) of target's maximum health" ||
                                            unit == "1% (+ 1 / 1.5 / 2 / 2.5 / 3% per 100 AD) of target's maximum health" ||
                                            unit == "2% (+ 2 / 3 / 4 / 5 / 6% per 100 AD) of target's maximum health"
                                        )
                                        {
                                            skill.unit.percentTargetMaxHP[i3] = (float)value;
                                        }
                                        if (unit == "% bonus armor")
                                        {
                                            skill.unit.percentBonusArmor[i3] = (float)value;
                                        }
                                        if (unit == " units")
                                        {
                                            skill.unit.units[i3] = (float)value;
                                        }
                                        if (unit == " chunks of ice")
                                        {
                                            skill.unit.chunckOfIce[i3] = (float)value;
                                        }
                                        if (unit == " soldiers")
                                        {
                                            skill.unit.soldiers[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% of target's missing health" ||
                                            unit == "%  of target's missing health" ||
                                            unit == "% (+ 0.5% per Mark) of target's missing health" ||
                                            unit == "12% (+2.8%) (+ 0.75% (+0.175%) per Mark) of target's missing health"
                                        )
                                        {
                                            skill.unit.percentTargetMissingHP[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% of Braum's maximum health" ||
                                            unit == "% of her maximum health"
                                        )
                                        {
                                            skill.unit.percentOwnMaxHP[i3] = (float)value;
                                        }
                                        if (unit == "% per 100 bonus AD")
                                        {
                                            skill.unit.percentPer100BonusAD[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% of target's current health" ||
                                            unit == "%  of target's current health" ||
                                            unit == "% (+ 3% per 100 AP) of target's current health" ||
                                            unit == "% (+ 1% per Mark) of target's current health" ||
                                            unit == "% (+ 1.5% per Mark) of target's current health"
                                        )
                                        {
                                            skill.unit.percentTargetCurrentHP[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% bonus health" ||
                                            unit == " (+ 3% bonus health)"
                                        )
                                        {
                                            skill.unit.percentBonusHP[i3] = (float)value;
                                        }
                                        if (unit == " bonus health")
                                        {
                                            skill.unit.bonusHP[i3] = (float)value;
                                        }
                                        if (unit == "% of damage taken")
                                        {
                                            skill.unit.percentDamageTaken[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% missing health" ||
                                            unit == "% of missing health" ||
                                            unit == "%  of missing health"
                                        )
                                        {
                                            skill.unit.percentMissingHP[i3] = (float)value;
                                        }
                                        if (
                                            unit == "% maximum health" ||
                                            unit == "% of maximum health"
                                        )
                                        {
                                            skill.unit.percentMaxHP[i3] = (float)value;
                                        }
                                        if (unit == "3% per 1% of health lost in the past 4 seconds")
                                        {
                                            skill.unit.percentHPLost[i3] = (float)value;
                                        }
                                        if (unit == "% per 100 AP")
                                        {
                                            skill.unit.percentPer100AP[i3] = (float)value;
                                        }
                                        if (unit == "% per 100 bonus magic resistance")
                                        {
                                            skill.unit.percentBonusMR[i3] = (float)value;
                                        }
                                        if (unit == "% per 100 AD")
                                        {
                                            skill.unit.percentPer100AD[i3] = (float)value;
                                        }
                                        if (unit == "% of target's armor")
                                        {
                                            skill.unit.percentTargetArmor[i3] = (float)value;
                                        }
                                        if (unit == "% of missing mana")
                                        {
                                            skill.unit.percentMissingMana[i3] = (float)value;
                                        }
                                        if (unit == "% maximum mana")
                                        {
                                            skill.unit.percentMaxMana[i3] = (float)value;
                                        }
                                        if (unit == "% of primary target's bonus health")
                                        {
                                            skill.unit.percentPrimaryTargetBonusHP[i3] = (float)value;
                                        }
                                        if (unit == "% armor")
                                        {
                                            skill.unit.percentArmor[i3] = (float)value;
                                        }
                                        if (unit == "Siphoning Strike stacks")
                                        {
                                            skill.unit.siphoningStrikeStacks[i3] = (float)value;
                                        }
                                        if (unit == "% total armor")
                                        {
                                            skill.unit.percentTotalArmor[i3] = (float)value;
                                        }
                                        if (unit == "% total magic resistance")
                                        {
                                            skill.unit.percentTotalMR[i3] = (float)value;
                                        }
                                        if (unit == "% bonus mana")
                                        {
                                            skill.unit.percentBonusMana[i3] = (float)value;
                                        }
                                        if (unit == " per 1 Lethality")
                                        {
                                            skill.unit.lithality[i3] = (float)value;
                                        }
                                        if (unit == " per Mist collected")
                                        {
                                            skill.unit.mist[i3] = (float)value;
                                        }
                                        if (unit == "% (+ 20% per 100 bonus AD) of expended Grit")
                                        {
                                            skill.unit.expendedGrit[i3] = (float)value;
                                        }
                                        if (unit == "% of his bonus health")
                                        {
                                            skill.unit.percentOwnBonusHP[i3] = (float)value;
                                        }
                                        if (unit == "% of Taric's armor")
                                        {
                                            skill.unit.percentArmor[i3] = (float)value;
                                        }
                                        if (unit == " per Soul collected")
                                        {
                                            skill.unit.soul[i3] = (float)value;
                                        }
                                        if (unit == " AD")
                                        {
                                            skill.unit.AD[i3] = (float)value;
                                        }
                                        if (unit == "% of his missing health")
                                        {
                                            skill.unit.percentOwnMissingHP[i3] = (float)value;
                                        }
                                        if (unit == "% of turret's maximum health")
                                        {
                                            // skill.unit.percentMaxHP[i3] = (float)value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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
