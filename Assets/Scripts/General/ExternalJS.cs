using UnityEngine;
using System.Runtime.InteropServices;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);
    [DllImport("__Internal")]
    private static extern void HelloString2(string str);

    public void SendData(string s)
    { 
        HelloString(s);
    }

    public void SendLogs(string s)
    {
        HelloString2(s);
    }
}
