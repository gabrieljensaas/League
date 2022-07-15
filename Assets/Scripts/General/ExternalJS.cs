using UnityEngine;
using System.Runtime.InteropServices;

public class ExternalJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    public void SendData(string s)
    { 
        HelloString(s);
    }
}
