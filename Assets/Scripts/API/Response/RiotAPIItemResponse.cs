using System.Collections.Generic;

    public class ItemData
    {
        public string name { get; set; }
        public string description { get; set; }
        public string colloq { get; set; }
        public string plaintext { get; set; }
        public List<string> into { get; set; }
        public ItemImage image { get; set; }
        public ItemGold gold { get; set; }
        public List<string> tags { get; set; }
        public ItemMaps maps { get; set; }
        public ItemStats stats { get; set; }
    }

    public class Basic
    {
        public string name { get; set; }
        public Rune rune { get; set; }
        public ItemGold gold { get; set; }
        public string group { get; set; }
        public string description { get; set; }
        public string colloq { get; set; }
        public string plaintext { get; set; }
        public bool consumed { get; set; }
        public int stacks { get; set; }
        public int depth { get; set; }
        public bool consumeOnFull { get; set; }
        public List<object> from { get; set; }
        public List<object> into { get; set; }
        public int specialRecipe { get; set; }
        public bool inStore { get; set; }
        public bool hideFromAll { get; set; }
        public string requiredChampion { get; set; }
        public string requiredAlly { get; set; }
        public ItemStats stats { get; set; }
        public List<object> tags { get; set; }
        public ItemMaps maps { get; set; }
    }

    public class ItemEffect
    {
        public string ItemEffect1Amount { get; set; }
        public string ItemEffect2Amount { get; set; }
        public string ItemEffect3Amount { get; set; }
        public string ItemEffect4Amount { get; set; }
        public string ItemEffect5Amount { get; set; }
        public string ItemEffect6Amount { get; set; }
        public string ItemEffect7Amount { get; set; }
        public string ItemEffect8Amount { get; set; }
        public string ItemEffect9Amount { get; set; }
        public string ItemEffect10Amount { get; set; }
        public string ItemEffect11Amount { get; set; }
        public string ItemEffect12Amount { get; set; }
        public string ItemEffect13Amount { get; set; }
        public string ItemEffect14Amount { get; set; }
        public string ItemEffect15Amount { get; set; }
        public string ItemEffect16Amount { get; set; }
        public string ItemEffect17Amount { get; set; }
        public string ItemEffect18Amount { get; set; }
    }

    public class ItemGold
    {
        public int @base { get; set; }
        public int total { get; set; }
        public int sell { get; set; }
        public bool purchasable { get; set; }
    }

    public class Group
    {
        public string id { get; set; }
        public string MaxGroupOwnable { get; set; }
    }

    public class ItemImage
    {
        public string full { get; set; }
        public string sprite { get; set; }
        public string group { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }

    public class ItemMaps
    {
        public bool _1 { get; set; }
        public bool _8 { get; set; }
        public bool _10 { get; set; }
        public bool _12 { get; set; }
        public bool _11 { get; set; }
        public bool _21 { get; set; }
        public bool _22 { get; set; }
    }

    public class RiotAPIItemResponse
    {
        public string type { get; set; }
        public string version { get; set; }
        public Basic basic { get; set; }
        public Dictionary<int, ItemData> data{get; set; }
        public List<Group> groups { get; set; }
        public List<Tree> tree { get; set; }
    }

    public class Rune
    {
        public bool isrune { get; set; }
        public int tier { get; set; }
        public string type { get; set; }
    }

    public class ItemStats
    {
        public int FlatHPPoolMod { get; set; }
        public int rFlatHPModPerLevel { get; set; }
        public int FlatMPPoolMod { get; set; }
        public int rFlatMPModPerLevel { get; set; }
        public float PercentHPPoolMod { get; set; }
        public float PercentMPPoolMod { get; set; }
        public float FlatHPRegenMod { get; set; }
        public int rFlatHPRegenModPerLevel { get; set; }
        public float PercentHPRegenMod { get; set; }
        public float FlatMPRegenMod { get; set; }
        public int rFlatMPRegenModPerLevel { get; set; }
        public float PercentMPRegenMod { get; set; }
        public int FlatArmorMod { get; set; }
        public int rFlatArmorModPerLevel { get; set; }
        public float PercentArmorMod { get; set; }
        public int rFlatArmorPenetrationMod { get; set; }
        public int rFlatArmorPenetrationModPerLevel { get; set; }
        public int rPercentArmorPenetrationMod { get; set; }
        public int rPercentArmorPenetrationModPerLevel { get; set; }
        public int FlatPhysicalDamageMod { get; set; }
        public int rFlatPhysicalDamageModPerLevel { get; set; }
        public float PercentPhysicalDamageMod { get; set; }
        public int FlatMagicDamageMod { get; set; }
        public int rFlatMagicDamageModPerLevel { get; set; }
        public float PercentMagicDamageMod { get; set; }
        public int FlatMovementSpeedMod { get; set; }
        public int rFlatMovementSpeedModPerLevel { get; set; }
        public float PercentMovementSpeedMod { get; set; }
        public int rPercentMovementSpeedModPerLevel { get; set; }
        public int FlatAttackSpeedMod { get; set; }
        public float PercentAttackSpeedMod { get; set; }
        public int rPercentAttackSpeedModPerLevel { get; set; }
        public int rFlatDodgeMod { get; set; }
        public int rFlatDodgeModPerLevel { get; set; }
        public float PercentDodgeMod { get; set; }
        public float FlatCritChanceMod { get; set; }
        public int rFlatCritChanceModPerLevel { get; set; }
        public float PercentCritChanceMod { get; set; }
        public int FlatCritDamageMod { get; set; }
        public int rFlatCritDamageModPerLevel { get; set; }
        public float PercentCritDamageMod { get; set; }
        public int FlatBlockMod { get; set; }
        public float PercentBlockMod { get; set; }
        public int FlatSpellBlockMod { get; set; }
        public int rFlatSpellBlockModPerLevel { get; set; }
        public float PercentSpellBlockMod { get; set; }
        public int FlatEXPBonus { get; set; }
        public float PercentEXPBonus { get; set; }
        public int rPercentCooldownMod { get; set; }
        public int rPercentCooldownModPerLevel { get; set; }
        public int rFlatTimeDeadMod { get; set; }
        public int rFlatTimeDeadModPerLevel { get; set; }
        public int rPercentTimeDeadMod { get; set; }
        public int rPercentTimeDeadModPerLevel { get; set; }
        public int rFlatItemGoldPer10Mod { get; set; }
        public int rFlatMagicPenetrationMod { get; set; }
        public int rFlatMagicPenetrationModPerLevel { get; set; }
        public int rPercentMagicPenetrationMod { get; set; }
        public int rPercentMagicPenetrationModPerLevel { get; set; }
        public int FlatEnergyRegenMod { get; set; }
        public int rFlatEnergyRegenModPerLevel { get; set; }
        public int FlatEnergyPoolMod { get; set; }
        public int rFlatEnergyModPerLevel { get; set; }
        public float PercentLifeStealMod { get; set; }
        public float PercentSpellVampMod { get; set; }
    }

    public class Tree
    {
        public string header { get; set; }
        public List<string> tags { get; set; }
    }

