using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChampionDataManager : MonoBehaviour
{
    /*
    public RiotAPIResponse championDataResponse;
    public Skills skills;
    public new string name;
    private string apiURL = "";
    public string[] skillCategory = { "qSkills", "wSkills" };

    // Start is called before the first frame update
    void Start()
    {
        name = skills.qSkills[^1].basic.champion;
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
            championDataResponse = JsonConvert.DeserializeObject<RiotAPIResponse>(unityWebRequest.downloadHandler.text);

            SetChampionData();
        }
    }

    void SetChampionData()
    {
        var abilities = championDataResponse.abilities;
        var effectInAbilities = abilities.Q[0].effects;

        for (int effect = 0; effect < effectInAbilities.Count; effect++)
        {
            for(int i = 0; i < 5; i++)
            {           
                // Basic
                if (abilities.Q[0].castTime != "none")
                    skills.qSkills[^1].basic.castTime = float.Parse(abilities.Q[0].castTime);

                if (abilities.Q[0].cost != null)
                    skills.qSkills[^1].basic.cost[i] = (float)abilities.Q[0].cost.modifiers[0].values[i];
                else
                    skills.qSkills[^1].basic.cost[i] = 0;

                skills.qSkills[^1].basic.name = abilities.Q[0].name;
                skills.qSkills[^1].basic.coolDown[i] = (float)abilities.Q[0].cooldown.modifiers[0].values[i];

                // Damage
                // Effect need loop
                // Modifiers need loop
                if(effectInAbilities[effect].leveling.Count > 0)
                {
                    for(int modifier = 0; modifier < effectInAbilities[effect].leveling[0].modifiers.Count; modifier++)
                    {
                        if (effectInAbilities[effect].leveling[0].attribute == "Physical Damage" || effectInAbilities[effect].leveling[0].attribute == "Bonus Physical Damage")
                        {
                            // Set Damage Type
                            //skills.qSkills[^1].skillDamageType = "Physical";

                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "")
                            {
                                skills.qSkills[^1].damage.flatAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% bonus AD")
                            {
                                skills.qSkills[^1].damage.bonusAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% AD")
                            {
                                skills.qSkills[^1].damage.percentAD[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                        }
                        if (effectInAbilities[effect].leveling[0].attribute == "Magic Damage" || effectInAbilities[effect].leveling[0].attribute == "Total Magic Damage")
                        {
                            // Set Damage Type
                            //skills.qSkills[^1].skillDamageType = "Physical";

                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "")
                            {
                                skills.qSkills[^1].damage.flatAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% bonus AP")
                            {
                                skills.qSkills[^1].damage.bonusAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "% AP")
                            {
                                skills.qSkills[^1].damage.percentAP[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }
                        }
                        if (effectInAbilities[effect].leveling[0].attribute == "Bonus Attack Speed")
                        {
                            skills.qSkills[^1].selfEffects.ASIncrease = true;
                            // Set Damage Type
                            //skills.qSkills[^1].skillDamageType = "Physical";

                            if (effectInAbilities[effect].leveling[0].modifiers[modifier].units[0] == "%")
                            {
                                skills.qSkills[^1].selfEffects.ASIncreasePercent[i] = (float)effectInAbilities[effect].leveling[0].modifiers[modifier].values[i];
                            }                           
                        }
                    }
                }                
            }
        }        
    }
    */
}
