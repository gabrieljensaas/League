using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillMultiHit
{
    public bool multiHit;
    public int[] hits = new int[5];
    public bool multiHitHasInterval; 
    public float multiHitInterval;
}
