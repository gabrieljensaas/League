using System.Collections.Generic;

[System.Serializable]
public class UnitList
{
    // public 
    // public float[][] test = 
    public List<float> flat = new();
    public List<float> percent = new();

    public List<float> AD = new();
    public List<float> bonusAD = new();
    public List<float> percentAD = new();
    public List<float> percentBonusAD = new();
    public List<float> percentPer100AD = new();
    public List<float> percentPer100BonusAD = new();

    public List<float> percentAP = new();
    public List<float> percentPer100AP = new();

    public List<float> bonusHP = new();
    public List<float> percentBonusHP = new();
    public List<float> percentMaxHP = new();
    public List<float> percentMissingHP = new();
    public List<float> percentHPLost = new();

    public List<float> percentOwnMaxHP = new();
    public List<float> percentOwnBonusHP = new();
    public List<float> percentOwnMissingHP = new();

    public List<float> percentTargetMaxHP = new();
    public List<float> percentTargetMissingHP = new();
    public List<float> percentTargetCurrentHP = new();
    public List<float> percentPrimaryTargetBonusHP = new();

    public List<float> percentMissingMana = new();
    public List<float> percentMaxMana = new();

    public List<float> percentBonusAS = new();

    public List<float> armor = new();
    public List<float> percentArmor = new();
    public List<float> percentBonusArmor = new();
    public List<float> percentTargetArmor = new();
    public List<float> percentTotalArmor = new();

    public List<float> percentBonusMR = new();
    public List<float> percentTotalMR = new();

    public List<float> percentDamageTaken = new();
    public List<float> percentDmgDealt = new();

    public List<float> x = new();
    public List<float> percentCritStrikeChance = new();
    public List<float> units = new();
    public List<float> chunckOfIce = new();
    public List<float> soldiers = new();
    public List<float> siphoningStrikeStacks = new();
    public List<float> percentBonusMana = new();
    public List<float> lithality = new();
    public List<float> mist = new();
    public List<float> expendedGrit = new();
    public List<float> soul = new();
}