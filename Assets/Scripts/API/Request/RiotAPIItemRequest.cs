using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;

public class RiotAPIItemRequest : MonoBehaviour
{
    string apiURL;
    RiotAPIItemResponse itemResponse;
    public TextMeshProUGUI[] champ1Items;
    public TextMeshProUGUI[] champ2Items;

    //public string[] itemId ={"1001","1004","1006","1011","1018","1026","1027","1028","1029","1031","1033","1035","1036","1037","1038","1039","1040","1042","1043","1052","1053","1054","1055","1056","1057","1058","1082","1083","1500","1501","1502","1503","1504","1505","1506","1507","1508","1509","1510","1511","1512","1515","1516","1517","1518","1519","2003","2010","2015","2031","2033","2051","2052","2055","2065","2138","2139","2140","2403","2419","2420","2421","2422","2423","2424","3001","3003","3004","3006","3009","3011","3020","3024","3026","3031","3033","3035","3036","3040","3041","3042","3044","3046","3047","3050","3051","3053","3057","3065","3066","3067","3068","3070","3071","3072","3074","3075","3076","3077","3078","3082","3083","3085","3086","3089","3091","3094","3095","3100","3102","3105","3107","3108","3109","3110","3111","3112","3113","3114","3115","3116","3117","3119","3121","3123","3124","3133","3134","3135","3139","3140","3142","3143","3145","3152","3153","3155","3156","3157","3158","3165","3177","3179","3181","3184","3190","3191","3193","3211","3222","3330","3340","3363","3364","3400","3504","3508","3513","3599","3600","3742","3748","3801","3802","3814","3850","3851","3853","3854","3855","3857","3858","3859","3860","3862","3863","3864","3901","3902","3903","3916","4005","4401","4403","4628","4629","4630","4632","4633","4635","4636","4637","4638","4641","4642","4643","4644","4645","6029","6035","6333","6609","6616","6617","6630","6631","6632","6653","6655","6656","6660","6662","6664","6670","6671","6672","6673","6675","6676","6677","6691","6692","6693","6694","6695","6696","7000","7001","7002","7003","7004","7005","7006","7007","7008","7009","7010","7011","7012","7013","7014","7015","7016","7017","7018","7019","7020","7021","7022","7023","7024","7050","8001","8020"};
    
    public List<String> allItems;

    string s;


    void Start()
    {        
        Request("12.10.1");
        //dropdownlist.AddOption()
        //itemName.Length = itemId.Length;
    }

    public void Request(string ver)
    {        
        apiURL = "http://ddragon.leagueoflegends.com/cdn/"+ ver +"/data/en_US/item.json";
        StartCoroutine(ItemAPIRequest());
    }

    IEnumerator ItemAPIRequest() 
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(apiURL);
        
        yield return unityWebRequest.SendWebRequest();

        if(unityWebRequest.result == UnityWebRequest.Result.ConnectionError || 
            unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(unityWebRequest.result);
        }
        else
        {                           
            bool success = false;
            
            itemResponse = JsonConvert.DeserializeObject<RiotAPIItemResponse>(unityWebRequest.downloadHandler.text);
        }
    }

    public void FetchStats(int _num, int loopNum, ChampStats champStats,int _type, int _id)
    {
        string type = "";
        object val = itemResponse.data[_id].stats;
        switch (_type)
        {
            case 1:
            type = "FlatHPPoolMod";
            break;

            case 2:
            type = "rFlatHPModPerLevel";
            break;

            case 3:
            type = "FlatMPPoolMod";
            break;

            case 4:
            type = "rFlatMPModPerLevel";
            break;

            case 5:
            type = "PercentHPPoolMod";
            break;

            case 6:
            type = "PercentMPPoolMod";
            break;

            case 7:
            type = "FlatHPRegenMod";
            break;

            case 8:
            type = "rFlatHPRegenModPerLevel";
            break;

            case 9:
            type = "PercentHPRegenMod";
            break;

            case 10:
            type = "FlatMPRegenMod";
            break;

            case 11:
            type = "rFlatMPRegenModPerLevel";
            break;

            case 12:
            type = "PercentMPRegenMod";
            break;

            case 13:
            type = "FlatArmorMod";
            break;

            case 14:
            type = "rFlatArmorModPerLevel";
            break;

            case 15:
            type = "PercentArmorMod";
            break;

            case 16:
            type = "rFlatArmorPenetrationMod";
            break;

            case 17:
            type = "rFlatArmorPenetrationModPerLevel";
            break;

            case 18:
            type = "rPercentArmorPenetrationMod";
            break;

            case 19:
            type = "rPercentArmorPenetrationModPerLevel";
            break;

            case 20:
            type = "FlatPhysicalDamageMod";
            break;

            case 21:
            type = "rFlatPhysicalDamageModPerLevel";
            break;

            case 22:
            type = "PercentPhysicalDamageMod";
            break;

            case 23:
            type = "FlatMagicDamageMod";
            break;

            case 24:
            type = "rFlatMagicDamageModPerLevel";
            break;

            case 25:
            type = "PercentMagicDamageMod";
            break;

            case 26:
            type = "FlatMovementSpeedMod";
            break;

            case 27:
            type = "rFlatMovementSpeedModPerLevel";
            break;

            case 28:
            type = "PercentMovementSpeedMod";
            break;

            case 29:
            type = "rPercentMovementSpeedModPerLevel";
            break;

            case 30:
            type = "FlatAttackSpeedMod";
            break;

            case 31:
            type = "PercentAttackSpeedMod";
            break;

            case 32:
            type = "rPercentAttackSpeedModPerLevel";
            break;

            case 33:
            type = "rFlatDodgeMod";
            break;

            case 34:
            type = "rFlatDodgeModPerLevel";
            break;

            case 35:
            type = "PercentDodgeMod";
            break;

            case 36:
            type = "FlatCritChanceMod";
            break;

            case 37:
            type = "rFlatCritChanceModPerLevel";
            break;

            case 38:
            type = "PercentCritChanceMod";
            break;

            case 39:
            type = "FlatCritDamageMod";
            break;

            case 40:
            type = "rFlatCritDamageModPerLevel";
            break;

            case 41:
            type = "PercentCritDamageMod";
            break;

            case 42:
            type = "FlatBlockMod";
            break;

            case 43:
            type = "PercentBlockMod";
            break;

            case 44:
            type = "FlatSpellBlockMod";
            break;

            case 45:
            type = "rFlatSpellBlockModPerLevel";
            break;

            case 46:
            type = "PercentSpellBlockMod";
            break;

            case 47:
            type = "FlatEXPBonus";
            break;

            case 48:
            type = "PercentEXPBonus";
            break;

            case 49:
            type = "rPercentCooldownMod";
            break;

            case 50:
            type = "rPercentCooldownModPerLevel";
            break;

            case 51:
            type = "rFlatTimeDeadMod";
            break;

            case 52:
            type = "rFlatTimeDeadModPerLevel";
            break;

            case 53:
            type = "rPercentTimeDeadMod";
            break;

            case 54:
            type = "rPercentTimeDeadModPerLevel";
            break;

            case 55:
            type = "rFlatItemGoldPer10Mod";
            break;

            case 56:
            type = "rFlatMagicPenetrationMod";
            break;

            case 57:
            type = "rFlatMagicPenetrationModPerLevel";
            break;

            case 58:
            type = "rPercentMagicPenetrationMod";
            break;

            case 59:
            type = "rPercentMagicPenetrationModPerLevel";
            break;

            case 60:
            type = "FlatEnergyRegenMod";
            break;

            case 61:
            type = "rFlatEnergyRegenModPerLevel";
            break;

            case 62:
            type = "FlatEnergyPoolMod";
            break;

            case 63:
            type = "rFlatEnergyModPerLevel";
            break;

            case 64:
            type = "PercentLifeStealMod";
            break;

            case 65:
            type = "PercentSpellVampMod";
            break;
        }        
        

        float value = float.Parse(GetPropValue(val,type).ToString());

        if(_num == 0)
        {
            champ1Items[loopNum].text = itemResponse.data[_id].name.ToString();
        }
        
        if(_num == 1)
        {
            champ2Items[loopNum].text = itemResponse.data[_id].name.ToString();
        }
        switch (_type)
        {
            case 1:
            champStats.FlatHPPoolMod += value;
            break;

            case 2:
            champStats.rFlatHPModPerLevel += value;
            break;

            case 3:
            champStats.FlatMPPoolMod += value;
            break;

            case 4:
            champStats.rFlatMPModPerLevel += value;
            break;

            case 5:
            champStats.PercentHPPoolMod += value;
            break;

            case 6:
            champStats.PercentMPPoolMod += value;
            break;

            case 7:
            champStats.FlatHPRegenMod += value;
            break;

            case 8:
            champStats.rFlatHPRegenModPerLevel += value;
            break;

            case 9:
            champStats.PercentHPRegenMod += value;
            break;

            case 10:
            champStats.FlatMPRegenMod += value;
            break;

            case 11:
            champStats.rFlatMPRegenModPerLevel += value;
            break;

            case 12:
            champStats.PercentMPRegenMod += value;
            break;

            case 13:
            champStats.FlatArmorMod += value;
            break;            

            case 14:
            champStats.rFlatArmorModPerLevel += value;
            break;

            case 15:
            champStats.PercentArmorMod += value;
            break;

            case 16:
            champStats.rFlatArmorPenetrationMod += value;
            break;

            case 17:
            champStats.rFlatArmorPenetrationModPerLevel += value;
            break;

            case 18:
            champStats.rPercentArmorPenetrationMod += value;
            break;

            case 19:
            champStats.rPercentArmorPenetrationModPerLevel += value;
            break;

            case 20:
            champStats.FlatPhysicalDamageMod += value;
            break;

            case 21:
            champStats.rFlatPhysicalDamageModPerLevel += value;
            break;

            case 22:
            champStats.PercentPhysicalDamageMod += value;
            break;

            case 23:
            champStats.FlatMagicDamageMod += value;
            break;

            case 24:
            champStats.rFlatMagicDamageModPerLevel += value;
            break;

            case 25:
            champStats.PercentMagicDamageMod += value;
            break;

            case 26:
            champStats.FlatMovementSpeedMod += value;
            break;

            case 27:
            champStats.rFlatMovementSpeedModPerLevel += value;
            break;

            case 28:
            champStats.PercentMovementSpeedMod += value;
            break;

            case 29:
            champStats.rPercentMovementSpeedModPerLevel += value;
            break;

            case 30:
            champStats.FlatAttackSpeedMod += value;
            break;

            case 31:
            champStats.PercentAttackSpeedMod += value;
            break;

            case 32:
            champStats.rPercentAttackSpeedModPerLevel += value;
            break;

            case 33:
            champStats.rFlatDodgeMod += value;
            break;

            case 34:
            champStats.rFlatDodgeModPerLevel += value;
            break;

            case 35:
            champStats.PercentDodgeMod += value;
            break;

            case 36:
            champStats.FlatCritChanceMod += value;
            break;

            case 37:
            champStats.rFlatCritChanceModPerLevel += value;
            break;

            case 38:
            champStats.PercentCritChanceMod += value;
            break;

            case 39:
            champStats.FlatCritDamageMod += value;
            break;

            case 40:
            champStats.rFlatCritDamageModPerLevel += value;
            break;

            case 41:
            champStats.PercentCritDamageMod += value;
            break;

            case 42:
            champStats.FlatBlockMod += value;
            break;

            case 43:
            champStats.PercentBlockMod += value;
            break;

            case 44:
            champStats.FlatSpellBlockMod += value;
            break;

            case 45:
            champStats.rFlatSpellBlockModPerLevel += value;
            break;

            case 46:
            champStats.PercentSpellBlockMod += value;
            break;

            case 47:
            champStats.FlatEXPBonus += value;
            break;

            case 48:
            champStats.PercentEXPBonus += value;
            break;

            case 49:
            champStats.rPercentCooldownMod += value;
            break;

            case 50:
            champStats.rPercentCooldownModPerLevel += value;
            break;

            case 51:
            champStats.rFlatTimeDeadMod += value;
            break;

            case 52:
            champStats.rFlatTimeDeadModPerLevel += value;
            break;

            case 53:
            champStats.rPercentTimeDeadMod += value;
            break;

            case 54:
            champStats.rPercentTimeDeadModPerLevel += value;
            break;

            case 55:
            champStats.rFlatItemGoldPer10Mod += value;
            break;

            case 56:
            champStats.rFlatMagicPenetrationMod += value;
            break;

            case 57:
            champStats.rFlatMagicPenetrationModPerLevel += value;
            break;

            case 58:
            champStats.rPercentMagicPenetrationMod += value;
            break;

            case 59:
            champStats.rPercentMagicPenetrationModPerLevel += value;
            break;

            case 60:
            champStats.FlatEnergyRegenMod += value;
            break;

            case 61:
            champStats.rFlatEnergyRegenModPerLevel += value;
            break;

            case 62:
            champStats.FlatEnergyPoolMod += value;
            break;

            case 63:
            champStats.rFlatEnergyModPerLevel += value;
            break;

            case 64:
            champStats.PercentLifeStealMod += value;
            break;

            case 65:
            champStats.PercentSpellVampMod += value;
            break;
        }
    }

    public static string ucfirst(string s)
    {
        bool IsNewSentense = true;
        var result = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            if (IsNewSentense && char.IsLetter(s[i]))
            {
                result.Append (char.ToUpper (s[i]));
                IsNewSentense = false;
            }
            else
                result.Append (s[i]);

            if (s[i] == '!' || s[i] == '?' || s[i] == '.')
            {
                IsNewSentense = true;
            }
        }
        return result.ToString().Replace("\u200B", "");;
    }

    static string CapitalizeFirstLetter(string value)
    {
        value = value.ToLower();
        char[] array = value.ToCharArray();
        if (array.Length >= 1)
        {
            if (char.IsLower(array[0]))
            {
                array[0] = char.ToUpper(array[0]);
            }
        }
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1] == ' ')
            {
                if (char.IsLower(array[i]))
                {
                    array[i] = char.ToUpper(array[i]);
                }
            }
        }
        return new string(array);
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }
}