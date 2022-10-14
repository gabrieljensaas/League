using System;
using System.Collections.Generic;
[Serializable]
public class APIMatchInfo
{
    public string version;
    public List<ChampionInfo> championInfo;
}
[Serializable]
public class ChampionInfo
{
    public List<int> ability;
    public string champName;
    public int champLevel;
    public List<int> items;
}
[Serializable]
public class LSSAPIResponse
{
    public APIMatchInfo APIMatchInfo;
}