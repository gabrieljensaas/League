using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string name;
    public int flatHP;
    public int flatAD;
    public int flatAP;
    public int flatASpd;
    public int flatArmor;
    public int flatSpellBlock;

    public float percentHP;
    public float percentAD;
    public float percentAP;
    public float percentASpd;
    public float percentArmor;
    public float percentSpellBlock;

    void Start()
    {
        Debug.Log(this.transform.parent.transform.parent.transform.parent.name);
    }
}
