using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;

public class APITest : MonoBehaviour
{

    void Start()
    {
        //StartCoroutine(StartRequest());
    }

    IEnumerator StartRequest()
    {
        string url = "https://lss-server-test.herokuapp.com/getSelectedChamps";
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");
        string jsonStr = "{\"namesArr\" : [\"Ahri\", \"Akali\", \"Amumu\", \"Ashe\", \"Katarina\", \"Garen\", \"Darius\", \"Zed\", \"Fiora\", \"Aatrox\"]}";
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);
        Debug.Log(jsonStr);
        WWW www = new WWW(url, formData, postHeader);

        yield return www;
        Debug.Log(www.text);
    }
}
