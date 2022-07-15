using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class RiotAPIMatchRequest : MonoBehaviour
{
    public RiotAPIMatchResponse matchResponse;
    public RiotAPITimeline timeline;
    //string selectedRegion = "EUW";
    string region;
    //string summoner = "Miritney";
    string url;
    public string key;
    string puuid;
    string continent;
    string[] matches = {"","","","","","","","","","","","","","","","","","","",""};
    public RiotAPIRequest apiRequest;
    public ChampStats[] champStats;
    RiotAPIItemRequest itemRequest;
    public TextMeshProUGUI[] matchID;
    public TextMeshProUGUI nameInput;
    public TextMeshProUGUI regionInput;
    public GameObject sliderValue;
    public Button[] button;
    SimManager simManager;
    string ver = "12.10.1";
    public bool isMatchLoaded;
    public static int[] selectedChamp = {0,5};
    public List<string> matchChampions;
    public string[] champName = {"",""};
    bool firstLoad = true;

    void Start()
    {
        itemRequest = GetComponent<RiotAPIItemRequest>();
        simManager = GetComponent<SimManager>();
    }

#region MatchListing
    public void RequestMatch()
    {    
        string summoner = nameInput.text.Replace("\u200B", "");
        string selectedRegion = regionInput.text.Replace("\u200B", "");

        switch (selectedRegion)
        {            
            case "NA":
            region = "na1";
            continent = "americas";
            break;

            case "EUW":
            region = "euw1";
            continent = "europe";
            break;

            case "EUN":
            region = "eun1";
            continent = "europe";
            break;

            case "KR":
            region = "kr";
            continent = "europe";
            break;

            case "BR":
            region = "br1";
            continent = "europe";
            break;

            case "JP":
            region = "jp1";
            continent = "asia";
            break;

            case "RU":
            region = "ru";
            break;

            case "OCE":
            region = "oc1";
            continent = "asia";
            break;

            case "TR":
            region = "tr1";
            continent = "europe";
            break;

            case "LAN":
            region = "la1";
            continent = "americas";
            break;

            case "LAS":
            region = "la2";
            continent = "americas";
            break;
        }
        url = "https://"+region+".api.riotgames.com/lol/summoner/v4/summoners/by-name/"+summoner+"?api_key="+key;        
        StartCoroutine(FetchMatch());
    }

    //Lists All the matches
    IEnumerator FetchMatch() 
    {
        SummonerInfo summonerInfo;
        MatchIDs matchIDs;
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        //Debug.Log(url);
        
        //Get PUUID
        yield return unityWebRequest.SendWebRequest();
        if(unityWebRequest.result == UnityWebRequest.Result.ConnectionError || 
            unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(unityWebRequest.result);
        }
        else
        {                            
            summonerInfo = JsonConvert.DeserializeObject<SummonerInfo>(unityWebRequest.downloadHandler.text);
            puuid=summonerInfo.puuid;

            //Get Matches
            url = "https://"+continent+".api.riotgames.com/lol/match/v5/matches/by-puuid/"+puuid+"/ids?start=0&count=10&api_key="+key;
            unityWebRequest = UnityWebRequest.Get(url);        
            yield return unityWebRequest.SendWebRequest();

            string s = "";
            s = unityWebRequest.downloadHandler.text.Replace("[\"","{\"matches\": [\"").Replace("]","]}");

            matchIDs = JsonConvert.DeserializeObject<MatchIDs>(s);
            int matchNum = matchIDs.matches.Count;
            for(int i = 0; i<matchNum; i++)
            {
                matchID[i].text = matchIDs.matches[i];
            }
            simManager.ShowMatches(matchNum);
        }
    }
#endregion

    public void SliderChange(Slider slider)
    {
        sliderValue.GetComponent<TextMeshProUGUI>().text = slider.value.ToString() + ":00";
        if(firstLoad)
        {
            firstLoad = false;
        }
        else
        {
            champStats[0].isLoaded = false;
            champStats[1].isLoaded = false;
            champStats[0].Reset(1);
            champStats[1].Reset(1);
            apiRequest.SimulateFight(0, champName[0],timeline.info.frames[int.Parse(slider.value.ToString())].participantFrames[selectedChamp[0]+1].xp,1);
            apiRequest.SimulateFight(1, champName[1],timeline.info.frames[int.Parse(slider.value.ToString())].participantFrames[selectedChamp[1]+1].xp,1);                    
        }
    }
    
    IEnumerator GetTimelineData(string _matchID)
    {
        
        UnityWebRequest unityWebRequest;
        url = "https://"+continent+".api.riotgames.com/lol/match/v5/matches/"+_matchID+"/timeline?api_key="+key;
        unityWebRequest = UnityWebRequest.Get(url);        
        yield return unityWebRequest.SendWebRequest();
        timeline = JsonConvert.DeserializeObject<RiotAPITimeline>(unityWebRequest.downloadHandler.text);
    }
    
    public void GetMatchData(string _matchID)
    {
        StartCoroutine(FetchMatchData(_matchID));
    }

#region GetMatch Info
    IEnumerator FetchMatchData(string _matchID)
    {
        UnityWebRequest unityWebRequest;
        url = "https://"+continent+".api.riotgames.com/lol/match/v5/matches/"+_matchID+"/?api_key="+key;
        unityWebRequest = UnityWebRequest.Get(url);        
        yield return unityWebRequest.SendWebRequest();

        string s = unityWebRequest.downloadHandler.text;
        matchResponse = JsonConvert.DeserializeObject<RiotAPIMatchResponse>(s);

        //Get Participant ID by PUUID
        int id = (matchResponse.metadata.participants.FindIndex(x => x.StartsWith(puuid)));
        string maxTimeStamp = ((float)(((matchResponse.info.gameEndTimestamp - matchResponse.info.gameStartTimestamp) / 60) /1000)).ToString("F2");

        simManager.sliderGO.GetComponent<Slider>().maxValue = int.Parse(((((matchResponse.info.gameEndTimestamp - matchResponse.info.gameStartTimestamp) / 60) /1000)).ToString("F0"));
        simManager.sliderGO.GetComponent<Slider>().value = int.Parse(((((matchResponse.info.gameEndTimestamp - matchResponse.info.gameStartTimestamp) / 60) /1000)).ToString("F0"));
        
        for(int i = 0; i<10; i++)
        {
            button[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = matchResponse.info.participants[i].championName;

        }
        
        ver = "12.10.1";
        int[] champExp = {0,0};
        int[] champ1Items = {0,0,0,0,0,0};
        int[] champ2Items = {0,0,0,0,0,0};

        
        matchChampions = new List<string>();

        for(int i = 0; i<10; i++)
        {
            matchChampions.Add(matchResponse.info.participants[i].championName);
        }

        champName[0] = matchResponse.info.participants[0].championName;
        champName[1] = matchResponse.info.participants[5].championName;

        champExp[0] = matchResponse.info.participants[0].champExperience;
        champExp[1] = matchResponse.info.participants[5].champExperience;

        simManager.storedName[0] = champName[0];
        simManager.storedName[1] = champName[1];
        simManager.storedXP[0] = champExp[0];
        simManager.storedXP[1] = champExp[1];
        
        champ1Items[0] = matchResponse.info.participants[0].item0;
        champ1Items[1] = matchResponse.info.participants[0].item1;
        champ1Items[2] = matchResponse.info.participants[0].item2;
        champ1Items[3] = matchResponse.info.participants[0].item3;
        champ1Items[4] = matchResponse.info.participants[0].item4;
        champ1Items[5] = matchResponse.info.participants[0].item5;
        champ2Items[0] = matchResponse.info.participants[5].item0;
        champ2Items[1] = matchResponse.info.participants[5].item1;
        champ2Items[2] = matchResponse.info.participants[5].item2;
        champ2Items[3] = matchResponse.info.participants[5].item3;
        champ2Items[4] = matchResponse.info.participants[5].item4;
        champ2Items[5] = matchResponse.info.participants[5].item5;

        for(int i = 1; i<66; i++)
        {
            for(int i2 = 0; i2<6; i2++)
            {
                if(champ1Items[i2] != 0)
                {
                    itemRequest.FetchStats(0,i2,champStats[0],i,champ1Items[i2]);
                }
            }
        }

        for(int i = 1; i<66; i++)
        {
            for(int i2 = 0; i2<6; i2++)
            {
                if(champ2Items[i2] != 0)
                {
                    itemRequest.FetchStats(1,i2,champStats[1],i,champ2Items[i2]);
                }
            }
        }
        champStats[0].FinalizeStats();
        champStats[1].FinalizeStats();
        //apiRequest.GetRiotAPIRequest(ver, champName[0], champName[1], champExp[0], champExp[1]);
        StartCoroutine(GetTimelineData(_matchID));
    }

    UnityEngine.Sprite SpriteFromTexture2D(Texture2D texture) 
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
#endregion

    public class SummonerInfo
    {
        public string id { get; set; }
        public string accountId { get; set; }
        public string puuid { get; set; }
        public string name { get; set; }
        public int profileIconId { get; set; }
        public long revisionDate { get; set; }
        public int summonerLevel { get; set; }
    }
    
    public class MatchIDs
    {
        public List<string> matches { get; set; }
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }
}
