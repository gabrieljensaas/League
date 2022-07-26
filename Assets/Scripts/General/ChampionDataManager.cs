using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class ChampionDataManager : MonoBehaviour
{
    /*
    public RiotAPIChampionDataResponse championDataResponse;
    Skills skills;
    public Dictionary<string, List<SkillList>> skillList = new();
    public new string name;
    private string apiURL = "";
    private string[] skillTitle = { "qSkills", "wSkills", "eSkills", "rSkills" };
    private string[] skillKey = { "Q", "W", "E", "R" };
    private int counter = 0;

    private void Awake()
    {
        skills = GetComponent<Skills>();

        skillList.Add("qSkills", skills.qSkills);
        skillList.Add("wSkills", skills.wSkills);
        skillList.Add("eSkills", skills.eSkills);
        skillList.Add("rSkills", skills.rSkills);

        //ChampStats champStats = new ChampStats();
        //champStats.dynamicStatus["Bonus Physical Damage"] = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach(var title in skillTitle)
        {
            if(skillList[title].Count > 0)
            {
                name = skillList[title][^1].basic.champion;
            }
        }
        GetChampionDataRequest(name);
    }

    public void GetChampionDataRequest(string champion)
    {
        apiURL = "https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/champions/" + champion + ".json";

        StartCoroutine(MakeChampionDataRequest());
    }

    IEnumerator MakeChampionDataRequest()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(apiURL);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
            unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(unityWebRequest.result);
        }
        else
        {
            championDataResponse = JsonConvert.DeserializeObject<RiotAPIChampionDataResponse>(unityWebRequest.downloadHandler.text);

            SetChampionData();
        }
    }

    void SetChampionData()
    {
        var abilities = championDataResponse.abilities;
        int limit = 5;
        foreach (var title in skillTitle)
        {
            if (!(skillList[title].Count > 0)) return;

            var effectInAbilities = abilities[skillKey[counter]][0].effects;

            for (int effect = 0; effect < effectInAbilities.Count; effect++)
            {
                for (int i = 0; i < limit; i++)
                {
                    if (title == "rSkills") limit = 3;
                    //if(i == 4)
                    // Basic
                    //Debug.Log(title);
                    if (abilities[skillKey[counter]][0].castTime != "none")
                        skillList[title][^1].basic.castTime = float.Parse(abilities[skillKey[counter]][0].castTime);

                    if (abilities[skillKey[counter]][0].cost != null)
                        skillList[title][^1].basic.cost[i] = (float)abilities[skillKey[counter]][0].cost.modifiers[0].values[i];
                    else
                        skillList[title][^1].basic.cost[i] = 0;

                    skillList[title][^1].basic.name = abilities[skillKey[counter]][0].name;
                    if ((abilities[skillKey[counter]][0].cooldown != null))
                    {
                        skillList[title][^1].basic.coolDown[i] = (float)abilities[skillKey[counter]][0].cooldown.modifiers[0].values[i];
                    }

                    // Damage
                    // Effect need loop
                    // Modifiers need loop
                    if (effectInAbilities[effect].leveling.Count > 0)
                    {
                        for (int modifier = 0; modifier < effectInAbilities[effect].leveling[0].modifiers.Count; modifier++)
                        {
                            if (effectInAbilities[effect].leveling[0].attribute == "Physical Damage" || effectInAbilities[effect].leveling[0].attribute == "Bonus Physical Damage")
                            {
                                // Set Damage Type
                                //skillList[title][^1].skillDamageType = "Physical";

                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[1] == "")
                                {
                                    skillList[title][^1].damage.flatAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% bonus AD")
                                {
                                    skillList[title][^1].damage.bonusAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% AD")
                                {
                                    skillList[title][^1].damage.percentAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                            }
                            if (effectInAbilities[effect].leveling[0].attribute == "Magic Damage" || effectInAbilities[effect].leveling[0].attribute == "Total Magic Damage")
                            {
                                // Set Damage Type
                                //skillList[title][^1].skillDamageType = "Physical";

                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "")
                                {
                                    skillList[title][^1].damage.flatAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% bonus AP")
                                {
                                    skillList[title][^1].damage.bonusAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% AP")
                                {
                                    skillList[title][^1].damage.percentAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                            }
                            if (effectInAbilities[effect].leveling[0].attribute == "Bonus Attack Speed")
                            {
                                skillList[title][^1].selfEffects.ASIncrease = true;
                                // Set Damage Type
                                //skillList[title][^1].skillDamageType = "Physical";

                                if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "%")
                                {
                                    skillList[title][^1].selfEffects.ASIncreasePercent[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                                }
                            }
                        }
                    }
                }
            }
            counter++;
        }        
    }
    */
}
