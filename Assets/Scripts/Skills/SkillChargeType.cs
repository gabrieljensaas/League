using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillChargeType
{

    public bool chargeable;
    public int charges;
    public float[] maxCharges = new float[5];
}
