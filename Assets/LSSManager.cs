using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSSManager : MonoBehaviour
{
    public GameObject[] summaryUI;
    public GameObject outputUI;

    public void ShowSummary()
    {
        outputUI.SetActive(false);
        foreach (GameObject item in summaryUI)
        {
            item.SetActive(true);
        }
    }

    public void ShowOutput()
    {
        foreach (GameObject item in summaryUI)
        {
            item.SetActive(false);
        }
        outputUI.SetActive(true);
    }
}
