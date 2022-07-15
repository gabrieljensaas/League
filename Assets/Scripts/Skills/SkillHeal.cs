using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillHeal
{
    public float[] flat = new float[5];
    public float[] percent = new float[5];
    public float[] flatByAP = new float[5];
    public float[] percentByAP = new float[5];
    public float[] percentByMissingHealth = new float[5];
}
