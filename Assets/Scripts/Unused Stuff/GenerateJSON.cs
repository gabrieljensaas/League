using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;

public class GenerateJSON : MonoBehaviour
{    
    public string fileName = "jsonOutput";
    string fileEXT = ".json";
    string folderName = "/JSON/";
    string assetsPath = null;
    int dataNum = 0;
    int spellNum = 0;
    int passiveNum = 0;
    
    OutputJSON data;

    List<int> damage = new List<int>();
    List<float> attackTime = new List<float>();
    List<string> championName = new List<string>();

    List<int> spellDamage = new List<int>();
    List<float> spellTime = new List<float>();
    List<string> spellChampName = new List<string>();
    List<string> _spellName = new List<string>();

    List<int> passiveDamage = new List<int>();
    List<float> passiveTime = new List<float>();
    List<string> passiveChampName = new List<string>();
    List<string> _passiveName = new List<string>();
    int type = 0;


    private void Awake()
    {
        assetsPath = Application.dataPath;        
    }

    private void Start()
    {
        //data = new OutputJSON { fightData = new FightData { } };        
    }

    public void CreateFile(object obj)
    {
        string result = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

        Debug.Log(result);
        string fileCreateLocation = assetsPath + folderName + fileName + fileEXT;
        File.WriteAllText(fileCreateLocation, result);
    }


    // type 1 auto attack, 2 spell, 3 passives           
    public void SendData(bool _new, string _champion, int _value, float _time, int _type, string spellName)
    {
        if (_new == true)
        {
            data = new OutputJSON { fightData = new FightData { } };
            SimManager.isNew = false;
        }
        if (_type == 1)
        {
            damage.Add(_value);
            attackTime.Add(_time);
            championName.Add(_champion);
            type = _type;
            dataNum++;
        }
        if (_type == 2)
        {
            spellDamage.Add(_value);
            spellTime.Add(_time);
            spellChampName.Add(_champion);
            _spellName.Add(spellName);
            spellNum++;
        }
        if (_type == 3)
        {
            passiveDamage.Add(_value);
            passiveTime.Add(_time);
            passiveChampName.Add(_champion);
            _passiveName.Add(spellName);
            passiveNum++;
        }
    }

    public void DataSerialize()
    {
        data.matchID = SimManager.MatchID;
        var autoAttack = data.fightData.autoAttackData = new List<AutoAttackDatum>();
        var spellAttack = data.fightData.spellData = new List<SpellDatum>();
        var passiveData = data.fightData.passiveData = new List<PassiveDatum>();

        for(int i = 0; i<dataNum; i++)
        {
            autoAttack.Add(new AutoAttackDatum() { champName = championName[i], value = damage[i], time = attackTime[i].ToString("F3") });
        }

        for (int i = 0; i < spellNum; i++)
        {
            spellAttack.Add(new SpellDatum() { champName = spellChampName[i], spellName = _spellName[i], value = spellDamage[i], time = spellTime[i].ToString("F3") });
        }

        for (int i = 0; i < passiveNum; i++)
        {
            passiveData.Add(new PassiveDatum() { champName = passiveChampName[i], spellName = _passiveName[i], value = passiveDamage[i], time = passiveTime[i].ToString("F3") });
        }

        CreateFile(data);
    }
}
