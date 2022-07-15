using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenHandler : MonoBehaviour
{
    static GameObject loadingScreen;

    void Start()
    {
        loadingScreen = GameObject.Find("Loading Screen");
    }

    public static void Show()
    {
        loadingScreen.SetActive(true);
    }
    public static void Hide()
    {
        loadingScreen.SetActive(false);
    }
}
