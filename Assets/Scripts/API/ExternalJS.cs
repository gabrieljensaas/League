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
    public WebData(SnapShot[] snap, DamageLog[] dmglogs, HealLog[] hlogs, BuffLog[] bufflogs, string winner, float fightDuration, CastLog[] CastLogs)
    {
        this.DamageLogs = dmglogs;
        this.SnapShots = snap;
        this.HealLogs = hlogs;
        this.BuffLogs = bufflogs;
        this.winner = winner;
        this.fightDuration = fightDuration;
        this.CastLogs = CastLogs;
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