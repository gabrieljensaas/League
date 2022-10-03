using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class ChampionDataGenerator : MonoBehaviour
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
    public string version = "12.17.1";

    RiotAPIChampionDataResponse api = new();

    private void Start()
    {
        StartCoroutine(LoadChampions());
    }

    private void CreateAsset(int num, string champName, string skillId, SkillList asset, PassiveList passive)
    {
        string abilityKey = folderName[num];
#if UNITY_EDITOR
        if (asset != null)
            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Skills/{version}/{abilityKey}/{champName} {abilityKey}[{skillId}].asset");
        else
            AssetDatabase.CreateAsset(passive, $"Assets/Resources/Skills/{version}/P/{champName} P[{skillId}].asset");

        AssetDatabase.SaveAssets();
#endif
    }

    private IEnumerator LoadChampions()
    {
        for (int i = 0; i < champions.Length; i++)
        {
            if (champions[i] == "GnarBig") continue; //Q2 BUG
            StartCoroutine(LoadChampionData(champions[i]));
        }
        yield return null;
    }

    private IEnumerator LoadChampionData(string champName)
    {
        string url = "https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/champions/" + champName + ".json";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        api = JsonConvert.DeserializeObject<RiotAPIChampionDataResponse>(www.downloadHandler.text);

        if (www.result != UnityWebRequest.Result.Success) yield break;

        LoadChampionStats(champName);
        LoadChampionSkills(champName);
    }

    private void LoadChampionStats(string champName)
    {
        if (Resources.Load<StatsList>($"Stats/{version}/{champName}]") == null)
        {
            StatsList asset = ScriptableObject.CreateInstance<StatsList>();
            asset.health = api.stats.health;
            asset.healthRegen = api.stats.healthRegen;
            asset.mana = api.stats.mana;
            asset.manaRegen = api.stats.manaRegen;
            asset.armor = api.stats.armor;
            asset.magicResistance = api.stats.magicResistance;
            asset.attackDamage = api.stats.attackDamage;
            asset.movespeed = api.stats.movespeed;
            asset.acquisitionRadius = api.stats.acquisitionRadius;
            asset.selectionRadius = api.stats.selectionRadius;
            asset.pathingRadius = api.stats.pathingRadius;
            asset.gameplayRadius = api.stats.gameplayRadius;
            asset.criticalStrikeDamage = api.stats.criticalStrikeDamage;
            asset.criticalStrikeDamageModifier = api.stats.criticalStrikeDamageModifier;
            asset.attackSpeed = api.stats.attackSpeed;
            asset.attackSpeedRatio = api.stats.attackSpeedRatio;
            asset.attackCastTime = api.stats.attackCastTime;
            asset.attackTotalTime = api.stats.attackTotalTime;
            asset.attackDelayOffset = api.stats.attackDelayOffset;
            asset.attackRange = api.stats.attackRange;
            asset.aramDamageTaken = api.stats.aramDamageTaken;
            asset.aramDamageDealt = api.stats.aramDamageDealt;
            asset.aramHealing = api.stats.aramHealing;
            asset.aramShielding = api.stats.aramShielding;
            asset.urfDamageTaken = api.stats.urfDamageTaken;
            asset.urfDamageDealt = api.stats.urfDamageDealt;
            asset.urfHealing = api.stats.urfHealing;
            asset.urfShielding = api.stats.urfShielding;
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Stats/{version}/{champName}.asset");
            AssetDatabase.SaveAssets();
#endif
        }
    }

    private void LoadChampionSkills(string champName)
    {
        GenerateKeyAbilities("Q", 0);
        GenerateKeyAbilities("W", 1);
        GenerateKeyAbilities("E", 2);
        GenerateKeyAbilities("R", 3);

        for (int i = 0; i < api.abilities["P"].Count; i++)
        {
            if (Resources.Load<SkillList>($"Skills/{version}/P/{champName} P[{i}]") == null)
            {
                PassiveList asset = ScriptableObject.CreateInstance<PassiveList>();
                asset.championName = champName;
                GenerateSkill(champName, i, null, asset, 4);
            }
        }

        void GenerateKeyAbilities(string abilityKey, int skillIndex)
        {
            for (int i = 0; i < api.abilities[abilityKey].Count; i++)
            {
                //if (Resources.Load<SkillList>($"Skills/{version}/{abilityKey}/{champName} {abilityKey}[{i}]") == null)
                //{
                SkillList asset = ScriptableObject.CreateInstance<SkillList>();
                //asset.unit = new UnitList();
                asset.atrList = new AttributeList();
                asset.basic = new SkillBasic
                {
                    champion = champName,
                    name = api.abilities[abilityKey][i].name
                };
                GenerateSkill(champName, i, asset, null, skillIndex);
                //}
            }
        }
    }

    private void GenerateSkill(string champName, int i, SkillList skill, PassiveList passive, int skillIndex)
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

            try
            {
                switch (api.abilities[skillType[skillIndex]][i].damageType)
                {
                    case "PHYSICAL_DAMAGE":
                        skill.skillDamageType = SkillDamageType.Phyiscal;
                        break;
                    case "MAGIC_DAMAGE":
                        skill.skillDamageType = SkillDamageType.Spell;
                        break;
                    case "TRUE_DAMAGE":
                        skill.skillDamageType = SkillDamageType.True;
                        break;
                    case "MIXED_DAMAGE": //TODO: check for mixed damage physical, spell, and true.
                        skill.skillDamageType = SkillDamageType.PhysAndSpell;
                        break;
                    default:
                        skill.skillDamageType = SkillDamageType.Phyiscal;
                        break;
                }

            }
            catch
            {
                Debug.LogError($"{api.abilities[skillType[skillIndex]][i].damageType} does not have an equivalent damage type!");
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

            if (skillIndex == 3)
            {
                limit = 3;
            }

            for (int effect = 0; effect < effectInAbilities.Count; effect++)
            {
                for (int level = 0; level < effectInAbilities[effect].leveling.Count; level++)
                {
                    AbilityEffect abilityEffect = new()
                    {
                        attribute = effectInAbilities[effect].leveling[level].attribute
                    };

                    for (int modifier = 0; modifier < effectInAbilities[effect].leveling[level].modifiers.Count; modifier++)
                    {
                        string unit = effectInAbilities[effect].leveling[level].modifiers[modifier].units[0];
                        List<float> value = effectInAbilities[effect].leveling[level].modifiers[modifier].values;

                        if (unit == "" || unit == "flat")
                            abilityEffect.flat = value;
                        else if (unit == "%")
                            abilityEffect.percent = value;
                        else if (unit == "AD")
                            abilityEffect.AD = value;
                        else if (unit == "% AD")
                            abilityEffect.percentAD = value;
                        else if (unit == "% AP" || unit == " (+ 100% AP)")
                            abilityEffect.percentAP = value;
                        else if (unit == "% bonus AD" || unit == "%  bonus AD" || unit == "[ 1% per 35 ][ 2.86% per 100 ]bonus AD")
                            abilityEffect.percentBonusAD = value;
                        else if (unit == "  �" || unit == "  � ")
                            abilityEffect.x = value;
                        else if (unit == "1 + 0.3 per 100% bonus attack speed" || unit == "% per 100% bonus attack speed")
                            abilityEffect.percentBonusAS = value;
                        else if (unit == "1 + (0.5 + 0.175) per 100% critical strike chance" ||
                                        unit == "% (+ 0% ? 25% (based on critical strike chance)" ||
                                        unit == "1 + (100% + 0%) critical strike chance")
                            abilityEffect.percentCritStrikeChance = value;
                        else if (unit == "%  of target's maximum health" ||
                                unit == "% of target's maximum health" ||
                                unit == "%  of the target's maximum health" ||
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
                                unit == "2% (+ 2 / 3 / 4 / 5 / 6% per 100 AD) of target's maximum health")
                            abilityEffect.percentTargetMaxHP = value;
                        else if (unit == "% bonus armor")
                            abilityEffect.percentBonusArmor = value;
                        else if (unit == " units")
                            abilityEffect.units = value;
                        else if (unit == " chunks of ice")
                            abilityEffect.chunckOfIce = value;
                        else if (unit == " soldiers")
                            abilityEffect.soldiers = value;
                        else if (unit == "% of target's missing health" ||
                                unit == "%  of target's missing health" ||
                                unit == "% (+ 0.5% per Mark) of target's missing health" ||
                                unit == "12% (+2.8%) (+ 0.75% (+0.175%) per Mark) of target's missing health")
                            abilityEffect.percentTargetMissingHP = value;
                        else if (unit == "% of Braum's maximum health" || unit == "% of her maximum health")
                            abilityEffect.percentOwnMaxHP = value;
                        else if (unit == "% per 100 bonus AD")
                            abilityEffect.percentPer100BonusAD = value;
                        else if (unit == "% of target's current health" ||
                                unit == "%  of target's current health" ||
                                unit == "% (+ 3% per 100 AP) of target's current health" ||
                                unit == "% (+ 1% per Mark) of target's current health" ||
                                unit == "% (+ 1.5% per Mark) of target's current health")
                            abilityEffect.percentTargetCurrentHP = value;
                        else if (unit == "% bonus health" || unit == " (+ 3% bonus health)")
                            abilityEffect.percentBonusHP = value;
                        else if (unit == " bonus health")
                            abilityEffect.bonusHP = value;
                        else if (unit == "% of damage taken")
                            abilityEffect.percentDamageTaken = value;
                        else if (unit == "% missing health" ||
                            unit == "% of missing health" ||
                            unit == "%  of missing health" ||
                            unit == " per 1% missing health")
                            abilityEffect.percentMissingHP = value;
                        else if (unit == "% maximum health" || unit == "% of maximum health" || unit == "% of turret's maximum health")
                            abilityEffect.percentMaxHP = value;
                        else if (unit == "3% per 1% of health lost in the past 4 seconds")
                            abilityEffect.percentHPLost = value;
                        else if (unit == "% per 100 AP")
                            abilityEffect.percentPer100AP = value;
                        else if (unit == "% per 100 bonus magic resistance")
                            abilityEffect.percentAD = value;
                        else if (unit == "% per 100 AD")
                            abilityEffect.percentPer100AD = value;
                        else if (unit == "% of target's armor")
                            abilityEffect.percentTargetArmor = value;
                        else if (unit == "% of missing mana")
                            abilityEffect.percentMissingMana = value;
                        else if (unit == "% maximum mana")
                            abilityEffect.percentMaxMana = value;
                        else if (unit == "% of primary target's bonus health")
                            abilityEffect.percentPrimaryTargetBonusHP = value;
                        else if (unit == "% armor")
                            abilityEffect.percentArmor = value;
                        else if (unit == "Siphoning Strike stacks")
                            abilityEffect.siphoningStrikeStacks = value;
                        else if (unit == "% total armor")
                            abilityEffect.percentTotalArmor = value;
                        else if (unit == "% total magic resistance")
                            abilityEffect.percentTotalMR = value;
                        else if (unit == "% bonus mana")
                            abilityEffect.percentBonusMana = value;
                        else if (unit == " per 1 Lethality")
                            abilityEffect.lethality = value;
                        else if (unit == " per Mist collected")
                            abilityEffect.mist = value;
                        else if (unit == "% (+ 20% per 100 bonus AD) of expended Grit")
                            abilityEffect.expendedGrit = value;
                        else if (unit == "% of his bonus health")
                            abilityEffect.percentOwnBonusHP = value;
                        else if (unit == "% of Taric's armor")
                            abilityEffect.percentArmor = value;
                        else if (unit == " per Soul collected")
                            abilityEffect.soul = value;
                        else if (unit == "% of his missing health")
                            abilityEffect.percentOwnMissingHP = value;
                    }

                    skill.effects.Add(abilityEffect);

                    #region OLD SKILL EFFECTS
                    //for (int modifier = 0; modifier < effectInAbilities[effect].leveling[level].modifiers.Count; modifier++)
                    //{
                    //    foreach (var atr in attributesName)
                    //    {
                    //        if (effectInAbilities[effect].leveling[level].attribute == atr)
                    //        {
                    //            if (!skill.atrList.attibutes.Contains(atr))
                    //                skill.atrList.attibutes.Add(atr);

                    //            for (int i3 = 0; i3 < limit; i3++)
                    //            {
                    //                var unit = effectInAbilities[effect].leveling[level].modifiers[modifier].units[0];
                    //                if (i3 >= effectInAbilities[effect].leveling[level].modifiers[modifier].values.Count) continue;

                    //                var value = effectInAbilities[effect].leveling[level].modifiers[modifier].values[i3];
                    //                if (unit == "" || unit == "flat")
                    //                {
                    //                    if (!skill.unit.flat.ContainsKey(atr))
                    //                        skill.unit.flat.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.flat[atr].Add((float)value);
                    //                }
                    //                if (unit == "% AD")
                    //                {
                    //                    if (!skill.unit.percentAD.ContainsKey(atr))
                    //                        skill.unit.percentAD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentAD[atr].Add((float)value);
                    //                }
                    //                if (unit == "%")
                    //                {
                    //                    if (!skill.unit.percent.ContainsKey(atr))
                    //                        skill.unit.percent.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percent[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% AP" ||
                    //                    unit == " (+ 100% AP)"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentAP.ContainsKey(atr))
                    //                        skill.unit.percentAP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentAP[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% bonus AD" ||
                    //                    unit == "%  bonus AD" ||
                    //                    unit == "[ 1% per 35 ][ 2.86% per 100 ]bonus AD"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentBonusAD.ContainsKey(atr))
                    //                        skill.unit.percentBonusAD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentBonusAD[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "  �" ||
                    //                    unit == "  � "
                    //                )
                    //                {
                    //                    if (!skill.unit.x.ContainsKey(atr))
                    //                        skill.unit.x.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.x[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "1 + 0.3 per 100% bonus attack speed" ||
                    //                    unit == "% per 100% bonus attack speed"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentBonusAS.ContainsKey(atr))
                    //                        skill.unit.percentBonusAS.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentBonusAS[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "1 + (0.5 + 0.175) per 100% critical strike chance" ||
                    //                    unit == "% (+ 0% ? 25% (based on critical strike chance)" ||
                    //                    unit == "1 + (100% + 0%) critical strike chance"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentCritStrikeChance.ContainsKey(atr))
                    //                        skill.unit.percentCritStrikeChance.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentCritStrikeChance[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "%  of target's maximum health" ||
                    //                    unit == "% of target's maximum health" ||
                    //                    unit == "%  of the target's maximum health" ||
                    //                    unit == "% (+ 0.25% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 0.46% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 0.7% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 0.8% per 100 AP) of the target's maximum health" ||
                    //                    unit == "% (+ 1.4% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 1.5% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 1.6% per 100 AP) of the target's maximum health" ||
                    //                    unit == "% (+ 2% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 2.4% per 100 AP) of the target's maximum health" ||
                    //                    unit == "% (+ 4% per 100 AP) of the target's maximum health" ||
                    //                    unit == "% (+ 4.5% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 6% per 100 AP) of target's maximum health" ||
                    //                    unit == "% (+ 7.2% per 100 AP) of the target's maximum health" ||
                    //                    unit == "% (+ 0.5% per Feast stack) of target's maximum health" ||
                    //                    unit == "% (+ 1.5% per Feast stack) of target's maximum health" ||
                    //                    unit == "% (+ 5% per 100 bonus AD) of target's maximum health" ||
                    //                    unit == "1% (+ 1 / 1.5 / 2 / 2.5 / 3% per 100 AD) of target's maximum health" ||
                    //                    unit == "2% (+ 2 / 3 / 4 / 5 / 6% per 100 AD) of target's maximum health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentTargetMaxHP.ContainsKey(atr))
                    //                        skill.unit.percentTargetMaxHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTargetMaxHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% bonus armor")
                    //                {
                    //                    if (!skill.unit.percentBonusArmor.ContainsKey(atr))
                    //                        skill.unit.percentBonusArmor.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentBonusArmor[atr].Add((float)value);
                    //                }
                    //                if (unit == " units")
                    //                {
                    //                    if (!skill.unit.units.ContainsKey(atr))
                    //                        skill.unit.units.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.units[atr].Add((float)value);
                    //                }
                    //                if (unit == " chunks of ice")
                    //                {
                    //                    if (!skill.unit.chunckOfIce.ContainsKey(atr))
                    //                        skill.unit.chunckOfIce.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.chunckOfIce[atr].Add((float)value);
                    //                }
                    //                if (unit == " soldiers")
                    //                {
                    //                    if (!skill.unit.soldiers.ContainsKey(atr))
                    //                        skill.unit.soldiers.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.soldiers[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% of target's missing health" ||
                    //                    unit == "%  of target's missing health" ||
                    //                    unit == "% (+ 0.5% per Mark) of target's missing health" ||
                    //                    unit == "12% (+2.8%) (+ 0.75% (+0.175%) per Mark) of target's missing health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentTargetMissingHP.ContainsKey(atr))
                    //                        skill.unit.percentTargetMissingHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTargetMissingHP[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% of Braum's maximum health" ||
                    //                    unit == "% of her maximum health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentOwnMaxHP.ContainsKey(atr))
                    //                        skill.unit.percentOwnMaxHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentOwnMaxHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% per 100 bonus AD")
                    //                {
                    //                    if (!skill.unit.percentPer100BonusAD.ContainsKey(atr))
                    //                        skill.unit.percentPer100BonusAD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentPer100BonusAD[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% of target's current health" ||
                    //                    unit == "%  of target's current health" ||
                    //                    unit == "% (+ 3% per 100 AP) of target's current health" ||
                    //                    unit == "% (+ 1% per Mark) of target's current health" ||
                    //                    unit == "% (+ 1.5% per Mark) of target's current health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentTargetCurrentHP.ContainsKey(atr))
                    //                        skill.unit.percentTargetCurrentHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTargetCurrentHP[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% bonus health" ||
                    //                    unit == " (+ 3% bonus health)"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentBonusHP.ContainsKey(atr))
                    //                        skill.unit.percentBonusHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentBonusHP[atr].Add((float)value);
                    //                }
                    //                if (unit == " bonus health")
                    //                {
                    //                    if (!skill.unit.bonusHP.ContainsKey(atr))
                    //                        skill.unit.bonusHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.bonusHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of damage taken")
                    //                {
                    //                    if (!skill.unit.percentDamageTaken.ContainsKey(atr))
                    //                        skill.unit.percentDamageTaken.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentDamageTaken[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% missing health" ||
                    //                    unit == "% of missing health" ||
                    //                    unit == "%  of missing health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentMissingHP.ContainsKey(atr))
                    //                        skill.unit.percentMissingHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentMissingHP[atr].Add((float)value);
                    //                }
                    //                if (
                    //                    unit == "% maximum health" ||
                    //                    unit == "% of maximum health"
                    //                )
                    //                {
                    //                    if (!skill.unit.percentMaxHP.ContainsKey(atr))
                    //                        skill.unit.percentMaxHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentMaxHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "3% per 1% of health lost in the past 4 seconds")
                    //                {
                    //                    if (!skill.unit.percentHPLost.ContainsKey(atr))
                    //                        skill.unit.percentHPLost.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentHPLost[atr].Add((float)value);
                    //                }
                    //                if (unit == "% per 100 AP")
                    //                {
                    //                    if (!skill.unit.percentPer100AP.ContainsKey(atr))
                    //                        skill.unit.percentPer100AP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentPer100AP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% per 100 bonus magic resistance")
                    //                {
                    //                    if (!skill.unit.percentAD.ContainsKey(atr))
                    //                        skill.unit.percentAD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentAD[atr].Add((float)value);
                    //                }
                    //                if (unit == "% per 100 AD")
                    //                {
                    //                    if (!skill.unit.percentPer100AD.ContainsKey(atr))
                    //                        skill.unit.percentPer100AD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentPer100AD[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of target's armor")
                    //                {
                    //                    if (!skill.unit.percentTargetArmor.ContainsKey(atr))
                    //                        skill.unit.percentTargetArmor.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTargetArmor[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of missing mana")
                    //                {
                    //                    if (!skill.unit.percentMissingMana.ContainsKey(atr))
                    //                        skill.unit.percentMissingMana.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentMissingMana[atr].Add((float)value);
                    //                }
                    //                if (unit == "% maximum mana")
                    //                {
                    //                    if (!skill.unit.percentMaxMana.ContainsKey(atr))
                    //                        skill.unit.percentMaxMana.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentMaxMana[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of primary target's bonus health")
                    //                {
                    //                    if (!skill.unit.percentPrimaryTargetBonusHP.ContainsKey(atr))
                    //                        skill.unit.percentPrimaryTargetBonusHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentPrimaryTargetBonusHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% armor")
                    //                {
                    //                    if (!skill.unit.percentArmor.ContainsKey(atr))
                    //                        skill.unit.percentArmor.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentArmor[atr].Add((float)value);
                    //                }
                    //                if (unit == "Siphoning Strike stacks")
                    //                {
                    //                    if (!skill.unit.siphoningStrikeStacks.ContainsKey(atr))
                    //                        skill.unit.siphoningStrikeStacks.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.siphoningStrikeStacks[atr].Add((float)value);
                    //                }
                    //                if (unit == "% total armor")
                    //                {
                    //                    if (!skill.unit.percentTotalArmor.ContainsKey(atr))
                    //                        skill.unit.percentTotalArmor.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTotalArmor[atr].Add((float)value);
                    //                }
                    //                if (unit == "% total magic resistance")
                    //                {
                    //                    if (!skill.unit.percentTotalMR.ContainsKey(atr))
                    //                        skill.unit.percentTotalMR.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentTotalMR[atr].Add((float)value);
                    //                }
                    //                if (unit == "% bonus mana")
                    //                {
                    //                    if (!skill.unit.percentBonusMana.ContainsKey(atr))
                    //                        skill.unit.percentBonusMana.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentBonusMana[atr].Add((float)value);
                    //                }
                    //                if (unit == " per 1 Lethality")
                    //                {
                    //                    if (!skill.unit.lithality.ContainsKey(atr))
                    //                        skill.unit.lithality.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.lithality[atr].Add((float)value);
                    //                }
                    //                if (unit == " per Mist collected")
                    //                {
                    //                    if (!skill.unit.mist.ContainsKey(atr))
                    //                        skill.unit.mist.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.mist[atr].Add((float)value);
                    //                }
                    //                if (unit == "% (+ 20% per 100 bonus AD) of expended Grit")
                    //                {
                    //                    if (!skill.unit.expendedGrit.ContainsKey(atr))
                    //                        skill.unit.expendedGrit.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.expendedGrit[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of his bonus health")
                    //                {
                    //                    if (!skill.unit.percentOwnBonusHP.ContainsKey(atr))
                    //                        skill.unit.percentOwnBonusHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentOwnBonusHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of Taric's armor")
                    //                {
                    //                    if (!skill.unit.percentArmor.ContainsKey(atr))
                    //                        skill.unit.percentArmor.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentArmor[atr].Add((float)value);
                    //                }
                    //                if (unit == " per Soul collected")
                    //                {
                    //                    if (!skill.unit.soul.ContainsKey(atr))
                    //                        skill.unit.soul.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.soul[atr].Add((float)value);
                    //                }
                    //                if (unit == " AD")
                    //                {
                    //                    if (!skill.unit.AD.ContainsKey(atr))
                    //                        skill.unit.AD.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.AD[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of his missing health")
                    //                {
                    //                    if (!skill.unit.percentOwnMissingHP.ContainsKey(atr))
                    //                        skill.unit.percentOwnMissingHP.Add(atr, new System.Collections.Generic.List<float>());
                    //                    skill.unit.percentOwnMissingHP[atr].Add((float)value);
                    //                }
                    //                if (unit == "% of turret's maximum health")
                    //                {
                    //                    // skill.unit.percentMaxHP.Add((float)value);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
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
