using System.Runtime.InteropServices;
using UnityEngine;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);
    //[DllImport("__Internal")]

    public void SendData(WebData data)
    {
        HelloString(JsonUtility.ToJson(data));
        //Debug.Log(JsonUtility.ToJson(data));      // for test comment out the line in the up and use this
    }
}

public class WebData
{
    public string[] logs;
    public WebData(string[] logs)
    {
        this.logs = logs;
    }
}