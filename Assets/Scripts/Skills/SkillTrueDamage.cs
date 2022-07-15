using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTrueDamage
{

    public TrueDamageType TDType;
    public float[] flat = new float[5];
    public float[] percent = new float[5];
    public enum TrueDamageType
    {
        AttackDamage,
        AbilityPower,
        Mixed
    }
}
