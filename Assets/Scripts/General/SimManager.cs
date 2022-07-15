using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class SimManager : MonoBehaviour
{
    public static bool isLoaded = false;
    public static bool battleStarted = false;
    public static float timer;
    public static float _timer;
  
    public bool ongoing;
    public RiotAPIRequest riotAPI;
    public TextMeshProUGUI outputText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI ver;
    public TextMeshProUGUI[] champ1Items;
    public TextMeshProUGUI[] champ2Items;  
    public GameObject InputField;
    public GameObject OutputField;
    public Button startBtn;
    public Button loadBtn;
    public Button resetBtn;
    public GameObject outputUI;
    public ChampStats[] champStats;
    public GameObject matchIDGO;
    public GameObject[] matchID;
    public TextMeshProUGUI[] output;
    public static bool isNew = true;
    public GameObject champSelectGO;
    public GameObject sliderGO;
    public GameObject sliderParent;
    public GameObject[] champOutput;
    public GameObject[] champOutput1;
    public TextMeshProUGUI timerTest;

    RiotAPIItemRequest itemRequest;
    RiotAPIItemResponse itemResponse;    
    RiotAPIMatchRequest matchRequest;
    RiotAPIRequest apiRequest;

    int champ1ItemNum;
    int champ2ItemNum;
    public int[] storedXP = {0,0};
    public string[] storedName = {"",""};

    public List<ChampionInput> champ;

    public static string MatchID = "";

    public void GetMatchData(Button button)
    {
        Clear();
        matchRequest.GetMatchData(button.GetComponentsInChildren<TextMeshProUGUI>()[0].text);
        MatchID = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchIDGO.SetActive(false);
        champSelectGO.SetActive(true);
        sliderParent.SetActive(true);
    }

    public void SelectChampion1(int id)
    {
        RiotAPIMatchRequest.selectedChamp[0] = id;
    }

    public void SelectChampion2(int id)
    {
        RiotAPIMatchRequest.selectedChamp[1] = id;
    }

    public void LoadChampion1(Button button)
    {
        champStats[0].isLoaded = false;
        string name = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchRequest.champName[0] = name;
        int value = (int)sliderGO.GetComponent<Slider>().value;
        int xp = matchRequest.timeline.info.frames[value].participantFrames[RiotAPIMatchRequest.selectedChamp[0]+1].xp;
        storedXP[0] = xp;
        storedName[0] = name;
        champStats[0].Reset(0);
        //apiRequest.GetRiotAPIRequest("12.10.1", storedName[0], storedName[1], storedXP[0], storedXP[1]);
        //Debug.Log(storedName[0]);
        //apiRequest.SimulateFight(0, name, xp,1);
        for (int i = 0; i<4; i++)
        {
            apiRequest.champAbilities[0].champSkills[i].text = apiRequest.allAbilities[RiotAPIMatchRequest.selectedChamp[0]].name[i];
        }
        apiRequest.LoadItems();
    }

    public void LoadChampion2(Button button)
    {
        champStats[1].isLoaded = false;
        string name = button.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
        matchRequest.champName[1] = name;
        int value = (int)sliderGO.GetComponent<Slider>().value;
        int xp = matchRequest.timeline.info.frames[value].participantFrames[RiotAPIMatchRequest.selectedChamp[1]+1].xp;
        storedXP[1] = xp;
        storedName[1] = name;
        champStats[1].Reset(0);
        //apiRequest.GetRiotAPIRequest("12.10.1", storedName[0], storedName[1], storedXP[0], storedXP[1]);
        //apiRequest.SimulateFight(1, name, xp,1);
        for(int i = 0; i<4; i++)
        {
            apiRequest.champAbilities[1].champSkills[i].text = apiRequest.allAbilities[RiotAPIMatchRequest.selectedChamp[1]].name[i];
        }
        apiRequest.LoadItems();
    }

    public void Back()
    {
        ongoing = false;
        loadBtn.interactable = true;
        resetBtn.interactable = true;
        timer = 0;
        output[0].text = "";
        output[1].text = "";
        matchIDGO.SetActive(true);
        champSelectGO.SetActive(false);
        sliderParent.SetActive(false);
        ShowInput();
    }

    void Start()
    {
        //matchIDGO.SetActive(false);
        ShowInput();
        itemRequest = GetComponent<RiotAPIItemRequest>();
        matchRequest = GetComponent<RiotAPIMatchRequest>();
        apiRequest = GetComponent<RiotAPIRequest>();
        //Time.timeScale = speed;
        //foreach (GameObject item in champOutput)
        //{
        //    item.SetActive(false);
        //}
    }

    public void ShowMatches(int num)
    {
        matchIDGO.SetActive(true);
        for(int i = 0; i < num; i++)
        {
            matchID[i].SetActive(true);
        }
    }

    void Update()
    {        
        if(isLoaded)
        {
            if(!ongoing)
            {
                //startBtn.interactable = true;
                StartBattle();
                //StartCoroutine(StartingBattle());
            }
            else
            {
                startBtn.interactable = false;
            }
        }

        if (battleStarted)
        {
            _timer += Time.unscaledDeltaTime;
            timer = _timer * Time.timeScale;
            timerTest.text = timer.ToString();
            //Debug.Log(timerTest.text);
        }
        else
        {
            if (isNew)
            {
                timer = 0;
                battleStarted = false;
            }
        }
        //Debug.Log(isLoaded);
    }
    
    public void StartBattle()
    {
        ShowOutput();
        ongoing = true;
        loadBtn.interactable = false;
        resetBtn.interactable = true;
        battleStarted = true;
    }

    public void Clear()
    {
        champStats[0].Reset(0);
        champStats[1].Reset(0);
        champStats[0].isLoaded = false;
        champStats[1].isLoaded = false;
    }

    public void Reset()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
        isLoaded = false;
        battleStarted = false;
        timer = 0;
        RiotAPIMatchRequest.selectedChamp[0] = 0;
        RiotAPIMatchRequest.selectedChamp[1] = 0;
    }

    void ShowInput()
    {
        OutputField.SetActive(false);
        //InputField.SetActive(true);
    }   

    void ShowOutput()
    {
        OutputField.SetActive(true);
        InputField.SetActive(false);
    } 

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }
}

[System.Serializable]
public class ChampionInput
{
    [HideInInspector] public string name;
    [HideInInspector] public int exp;
    [HideInInspector] public string spell1;
    [HideInInspector] public string spell2;
    [HideInInspector] public int q;
    [HideInInspector] public int w;
    [HideInInspector] public int e;
    [HideInInspector] public int r;
    [HideInInspector] public string item1;
    [HideInInspector] public string item2;
    [HideInInspector] public string item3;
    [HideInInspector] public string item4;
    [HideInInspector] public string item5;
    [HideInInspector] public string item6;
    public TextMeshProUGUI tname;
    public TextMeshProUGUI texp;
    public TextMeshProUGUI tspell1;
    public TextMeshProUGUI tspell2;
    public TextMeshProUGUI tq;
    public TextMeshProUGUI tw;
    public TextMeshProUGUI te;
    public TextMeshProUGUI tr;
    public TextMeshProUGUI titem1;
    public TextMeshProUGUI titem2;
    public TextMeshProUGUI titem3;
    public TextMeshProUGUI titem4;
    public TextMeshProUGUI titem5;
    public TextMeshProUGUI titem6;

    public void GetData()
    {
        name = tname.text;
        exp = int.Parse(texp.text.Replace("Exp","0").Replace("\u200B", ""));
        spell1 = tspell1.text;
        spell2 = tspell2.text;
        q = int.Parse(tq.text.Replace("\u200B", "0"));
        w = int.Parse(tw.text.Replace("\u200B", "0"));
        e = int.Parse(te.text.Replace("\u200B", "0"));
        r = int.Parse(tr.text.Replace("\u200B", "0"));
        item1 = titem1.text;
        item2 = titem2.text;
        item3 = titem3.text;
        item4 = titem4.text;
        item5 = titem5.text;
        item6 = titem6.text;
    }
}
