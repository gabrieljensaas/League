using System.Collections.Generic;

[System.Serializable]
public class UnitList
{
    public Dictionary<string, List<float>> flat = new();
    public Dictionary<string, List<float>> percent = new();

    public Dictionary<string, List<float>> AD = new();
    public Dictionary<string, List<float>> bonusAD = new();
    public Dictionary<string, List<float>> percentAD = new();
    public Dictionary<string, List<float>> percentBonusAD = new();
    public Dictionary<string, List<float>> percentPer100AD = new();
    public Dictionary<string, List<float>> percentPer100BonusAD = new();

    public Dictionary<string, List<float>> percentAP = new();
    public Dictionary<string, List<float>> percentPer100AP = new();

    public Dictionary<string, List<float>> bonusHP = new();
    public Dictionary<string, List<float>> percentBonusHP = new();
    public Dictionary<string, List<float>> percentMaxHP = new();
    public Dictionary<string, List<float>> percentMissingHP = new();
    public Dictionary<string, List<float>> percentHPLost = new();

    public Dictionary<string, List<float>> percentOwnMaxHP = new();
    public Dictionary<string, List<float>> percentOwnBonusHP = new();
    public Dictionary<string, List<float>> percentOwnMissingHP = new();

    public Dictionary<string, List<float>> percentTargetMaxHP = new();
    public Dictionary<string, List<float>> percentTargetMissingHP = new();
    public Dictionary<string, List<float>> percentTargetCurrentHP = new();
    public Dictionary<string, List<float>> percentPrimaryTargetBonusHP = new();

    public Dictionary<string, List<float>> percentMissingMana = new();
    public Dictionary<string, List<float>> percentMaxMana = new();

    public Dictionary<string, List<float>> percentBonusAS = new();

    public Dictionary<string, List<float>> armor = new();
    public Dictionary<string, List<float>> percentArmor = new();
    public Dictionary<string, List<float>> percentBonusArmor = new();
    public Dictionary<string, List<float>> percentTargetArmor = new();
    public Dictionary<string, List<float>> percentTotalArmor = new();

    public Dictionary<string, List<float>> percentBonusMR = new();
    public Dictionary<string, List<float>> percentTotalMR = new();

    public Dictionary<string, List<float>> percentDamageTaken = new();
    public Dictionary<string, List<float>> percentDmgDealt = new();

    public Dictionary<string, List<float>> x = new();
    public Dictionary<string, List<float>> percentCritStrikeChance = new();
    public Dictionary<string, List<float>> units = new();
    public Dictionary<string, List<float>> chunckOfIce = new();
    public Dictionary<string, List<float>> soldiers = new();
    public Dictionary<string, List<float>> siphoningStrikeStacks = new();
    public Dictionary<string, List<float>> percentBonusMana = new();
    public Dictionary<string, List<float>> lithality = new();
    public Dictionary<string, List<float>> mist = new();
    public Dictionary<string, List<float>> expendedGrit = new();
    public Dictionary<string, List<float>> soul = new();
}