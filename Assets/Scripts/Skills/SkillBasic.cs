using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBasic
{
    public string champion;
    public string name;
    public float castTime;
    public bool inactive;

    public float[] coolDown = new float[5];
    public float[] cost = new float[5];
    public float[] hpCost = new float[5];
}
