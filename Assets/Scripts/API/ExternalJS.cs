using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);
    //[DllImport("__Internal")]

    public void SendData(WebData data)
    {
        HelloString(JsonUtility.ToJson(data));
        //Debug.Log(JsonUtility.ToJson(data));      // for test comment out the line in the up and use this
    }
}
[System.Serializable]
public class WebData
{
    public string[] logs;
    public SnapShot[] snapShots;
    public WebData(string[] logs, SnapShot[] snap)
    {
        this.logs = logs;
        this.snapShots = snap;
    }
}
[System.Serializable]
public class SnapShot
{
    public string log;
    public ChampionSnap champion1;
    public ChampionSnap champion2;

    public SnapShot(string log, ChampionSnap champion1, ChampionSnap champion2)
    {
        this.log = log;
        this.champion1 = champion1;
        this.champion2 = champion2;
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