using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIMatchInfo
{
    public string version { get; set; }
    public List<ChampionInfo> championInfo { get; set; }
}

public class ChampionInfo
{
    public string champName { get; set; }
    public int champLevel { get; set; }
}

public class LSSAPIResponse
{
    public APIMatchInfo APIMatchInfo { get; set; }
}