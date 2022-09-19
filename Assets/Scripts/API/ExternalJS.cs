using System.Runtime.InteropServices;
using UnityEngine;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string[] str);
    [DllImport("__Internal")]
    private static extern void HelloString2(string[] str2);

    public void SendData(string[] str)
    {
        HelloString(str);
    }
    public void SendLogs(string[] s)
    {
        //HelloString2(s);
    }
}
