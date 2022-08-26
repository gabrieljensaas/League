using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class APIMatchInfo
{
    public string version;
    public List<ChampionInfo> championInfo;
}
[Serializable]
public class ChampionInfo
{
    public string champName;
    public int champLevel;
    public List<int> items;
}
[Serializable]
public class LSSAPIResponse
{
    public APIMatchInfo APIMatchInfo;
}