//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
//public class ChampData
//{
//    public string type { get; set; }
//    public string format { get; set; }
//    public string version { get; set; }
//    public Data data { get; set; }
//}
//
//public class Champion
//{
//    public string _id { get; set; }
//    public ChampData champData { get; set; }
//}
//
//public class Champion2
//{
//    public string id { get; set; }
//    public string key { get; set; }
//    public string name { get; set; }
//    public string title { get; set; }
//    public Image image { get; set; }
//    public List<Skin> skins { get; set; }
//    public string lore { get; set; }
//    public string blurb { get; set; }
//    public List<string> allytips { get; set; }
//    public List<string> enemytips { get; set; }
//    public List<string> tags { get; set; }
//    public string partype { get; set; }
//    public Info info { get; set; }
//    public Stats stats { get; set; }
//    public List<Spell> spells { get; set; }
//    public Passive passive { get; set; }
//    public List<object> recommended { get; set; }
//}
//
//public class Data
//{
//    public Champion Champion { get; set; }
//}
//
//public class Datavalues
//{
//}
//
//public class Image
//{
//    public string full { get; set; }
//    public string sprite { get; set; }
//    public string group { get; set; }
//    public int x { get; set; }
//    public int y { get; set; }
//    public int w { get; set; }
//    public int h { get; set; }
//}
//
//public class Info
//{
//    public int attack { get; set; }
//    public int defense { get; set; }
//    public int magic { get; set; }
//    public int difficulty { get; set; }
//}
//
//public class Leveltip
//{
//    public List<string> label { get; set; }
//    public List<string> effect { get; set; }
//}
//
//public class Passive
//{
//    public string name { get; set; }
//    public string description { get; set; }
//    public Image image { get; set; }
//}
//
//public class ChampAPIRes
//{
//    public List<Champion> champions { get; set; }
//}
//
//public class Skin
//{
//    public string id { get; set; }
//    public int num { get; set; }
//    public string name { get; set; }
//    public bool chromas { get; set; }
//}
//
//public class Spell
//{
//    public string id { get; set; }
//    public string name { get; set; }
//    public string description { get; set; }
//    public string tooltip { get; set; }
//    public Leveltip leveltip { get; set; }
//    public int maxrank { get; set; }
//    public List<double> cooldown { get; set; }
//    public string cooldownBurn { get; set; }
//    public List<int> cost { get; set; }
//    public string costBurn { get; set; }
//    public Datavalues datavalues { get; set; }
//    public List<List<double>> effect { get; set; }
//    public List<string> effectBurn { get; set; }
//    public List<object> vars { get; set; }
//    public string costType { get; set; }
//    public string maxammo { get; set; }
//    public List<int> range { get; set; }
//    public string rangeBurn { get; set; }
//    public Image image { get; set; }
//    public string resource { get; set; }
//}
//
//public class Stats
//{
//    public int hp { get; set; }
//    public int hpperlevel { get; set; }
//    public int mp { get; set; }
//    public double mpperlevel { get; set; }
//    public int movespeed { get; set; }
//    public int armor { get; set; }
//    public double armorperlevel { get; set; }
//    public int spellblock { get; set; }
//    public double spellblockperlevel { get; set; }
//    public int attackrange { get; set; }
//    public double hpregen { get; set; }
//    public double hpregenperlevel { get; set; }
//    public double mpregen { get; set; }
//    public double mpregenperlevel { get; set; }
//    public int crit { get; set; }
//    public int critperlevel { get; set; }
//    public int attackdamage { get; set; }
//    public double attackdamageperlevel { get; set; }
//    public double attackspeedperlevel { get; set; }
//    public double attackspeed { get; set; }
//}