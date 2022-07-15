using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBasicDamage
{
    [Header("Phyiscal")]
    public float[] flatAD = new float[5];
    public float[] bonusAD = new float[5];
    public float[] percentAD = new float[5];

    [Header("Magical")]
    public float[] flatAP = new float[5];
    public float[] bonusAP = new float[5];
    public float[] percentAP = new float[5];
}
