using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;
    public ScrollRect scrollRect1;
    public Scrollbar scrollbar1;
    bool scrollable = false;
    
    void Update()
    {
        scrollRect1.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
        scrollbar1.value = scrollbar.value;
        
        if(scrollable)
        {
            scrollRect.verticalNormalizedPosition = 0.0f;
            scrollbar.value = 0.0f;
        }

        if(SimManager.battleStarted == true) 
        {
            scrollable = true;
        }
        else
        {
            scrollable = false;
            //StartCoroutine(EndBattle());
        }
    }

    IEnumerator EndBattle()
    {
        yield return new WaitForSeconds(0.5f);
        scrollable = false;
    }
}
