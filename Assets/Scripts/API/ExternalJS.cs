using System.Runtime.InteropServices;
using UnityEngine;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);
    [DllImport("__Internal")]
    private static extern void UnityReady();

    public void SendData(WebData data)
    {
        #if !UNITY_EDITOR
        HelloString(JsonUtility.ToJson(data));
        SimManager.Instance.Reset();
        #else
        Debug.Log(JsonUtility.ToJson(data));
        #endif
    }

    public void SendReady()
    {
        #if !UNITY_EDITOR
        UnityReady();
        #endif
    }
}
[System.Serializable]
public class WebData
{
    public string winner;
    public float fightDuration;
    public DamageLog[] DamageLogs;
    public SnapShot[] SnapShots;
    public HealLog[] HealLogs;
    public BuffLog[] BuffLogs;
    public CastLog[] CastLogs;
    public Tooltip[] Tooltips;
    public ChampionStatsExternal[] ChampionStats;
    public WebData(SnapShot[] snap, DamageLog[] dmglogs, HealLog[] hlogs, BuffLog[] bufflogs, string winner, float fightDuration, CastLog[] CastLogs, Tooltip[] tooltips, ChampionStatsExternal[] championStats)
    {
        this.DamageLogs = dmglogs;
        this.SnapShots = snap;
        this.HealLogs = hlogs;
        this.BuffLogs = bufflogs;
        this.winner = winner;
        this.fightDuration = fightDuration;
        this.CastLogs = CastLogs;
        Tooltips = tooltips;
        ChampionStats = championStats;
    }
}
[System.Serializable]
public class SnapShot
{
    public string log;
    public ChampionSnap champion1;
    public ChampionSnap champion2;
    public float time;

    public SnapShot(string log, ChampionSnap champion1, ChampionSnap champion2, float time)
    {
        this.log = log;
        this.champion1 = champion1;
        this.champion2 = champion2;
        this.time = time;
    }
}
[System.Serializable]
public class ChampionSnap
{
    public string championName;
    public float health;

    public ChampionSnap(string championName, float health)
    {
        this.championName = championName;
        this.health = health;
    }
}

[System.Serializable]
public class DamageLog
{
    public string championName;
    public string skillName;
    public string skillType;
    public float value;
    public float time;

    public DamageLog(string championName, string skillName, float value, float seconds, string skillType)
    {
        this.championName = championName;
        this.skillName = skillName;
        this.value = value;
        this.time = seconds;
        this.skillType = skillType;
    }
}

[System.Serializable]
public class HealLog
{
    public string championName;
    public string skillName;
    public float value;
    public float time;

    public HealLog(string championName, string skillName, float value, float seconds)
    {
        this.championName = championName;
        this.skillName = skillName;
        this.value = value;
        this.time = seconds;
    }
}

[System.Serializable]
public class BuffLog
{
    public string championName;
    public string skillName;
    public float duration;
    public float time;
    public string effect;

    public BuffLog(string championName, string skillName, float duration, float seconds, string effect)
    {
        this.championName = championName;
        this.skillName = skillName;
        this.effect = effect;
        this.duration = duration;
        this.time = seconds;
    }
}

[System.Serializable]
public class CastLog
{
    public string championName;
    public int ACast = 0;
    public int PCast = 0;
    public int QCast = 0;
    public int WCast = 0;
    public int ECast = 0;
    public int RCast = 0;

    public CastLog(string championName)
    {
        this.championName = championName;
    }
}

[System.Serializable]
public class Tooltip
{
    public string championName;
    public int[] QValues;
    public int[] WValues;
    public int[] EValues;
    public int[] RValues;

    public Tooltip(string championName, int[] qValues, int[] wValues, int[] eValues, int[] rValues)
    {
        this.championName = championName;
        QValues = qValues;
        WValues = wValues;
        EValues = eValues;
        RValues = rValues;
    }
}

[System.Serializable]
public class ChampionStatsExternal
{
    public string championName;
    public int MaxHP;
    public int AD;
    public int AP;
    public int Armor;
    public int Spellblock;
    public int AttackSpeed;
    public int Tenacity;
    public int Lifesteal;
    public int BaseHP;
    public int BaseArmor;
    public int BaseSpellblock;
    public int BaseAttackSpeed;
    public int BaseAD;
    public int CriticalStrikeChance;
    public int CriticalStrikeDamage;
    public int ArmorPenetrationFlat;
    public int ArmorPenetrationPercent;
    public int MagicPenetrationFlat;
    public int MagicPenetrationPercent;
    public int Omnivamp;
    public int Spellvamp;
    public int Physicalvamp;
    public int HealAndShieldPower;
    public int AbilityHaste;
    public int BasicAbilityHaste;
    public int UltimateAbilityHaste;
    public int ItemAbilityHaste;
    public int SummonerSpellAbilityHaste;
    public int HPRegeneration;

    public ChampionStatsExternal(string championName, int maxHP, int aD, int aP, int armor, int spellblock, int attackSpeed, int tenacity, int lifesteal, int baseHP, int baseArmor, int baseSpellblock, int baseAttackSpeed, int baseAD, int criticalStrikeChance, int criticalStrikeDamage, int armorPenetrationFlat, int armorPenetrationPercent, int magicPenetrationFlat, int magicPenetrationPercent, int omnivamp, int spellvamp, int physicalvamp, int healAndShieldPower, int abilityHaste, int basicAbilityHaste, int ultimateAbilityHaste, int itemAbilityHaste, int summonerSpellAbilityHaste, int hPRegeneration)
    {
        this.championName = championName;
        MaxHP = maxHP;
        AD = aD;
        AP = aP;
        Armor = armor;
        Spellblock = spellblock;
        AttackSpeed = attackSpeed;
        Tenacity = tenacity;
        Lifesteal = lifesteal;
        BaseHP = baseHP;
        BaseArmor = baseArmor;
        BaseSpellblock = baseSpellblock;
        BaseAttackSpeed = baseAttackSpeed;
        BaseAD = baseAD;
        CriticalStrikeChance = criticalStrikeChance;
        CriticalStrikeDamage = criticalStrikeDamage;
        ArmorPenetrationFlat = armorPenetrationFlat;
        ArmorPenetrationPercent = armorPenetrationPercent;
        MagicPenetrationFlat = magicPenetrationFlat;
        MagicPenetrationPercent = magicPenetrationPercent;
        Omnivamp = omnivamp;
        Spellvamp = spellvamp;
        Physicalvamp = physicalvamp;
        HealAndShieldPower = healAndShieldPower;
        AbilityHaste = abilityHaste;
        BasicAbilityHaste = basicAbilityHaste;
        UltimateAbilityHaste = ultimateAbilityHaste;
        ItemAbilityHaste = itemAbilityHaste;
        SummonerSpellAbilityHaste = summonerSpellAbilityHaste;
        HPRegeneration = hPRegeneration;
    }
}