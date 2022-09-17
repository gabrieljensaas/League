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

[System.Serializable]
public class AbilityEffect
{
    public string attribute;

    public List<float> flat = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percent = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> AD = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> bonusAD = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentAD = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentBonusAD = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentPer100AD = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentPer100BonusAD = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentAP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentPer100AP = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> bonusHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentBonusHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentMaxHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentMissingHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentHPLost = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentOwnMaxHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentOwnBonusHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentOwnMissingHP = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentTargetMaxHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentTargetMissingHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentTargetCurrentHP = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentPrimaryTargetBonusHP = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentMissingMana = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentMaxMana = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentBonusAS = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> armor = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentArmor = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentBonusArmor = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentTargetArmor = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentTotalArmor = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentBonusMR = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentTotalMR = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> percentDamageTaken = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentDmgDealt = new() { 0, 0, 0, 0, 0, 0 };

    public List<float> x = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentCritStrikeChance = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> units = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> chunckOfIce = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> soldiers = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> siphoningStrikeStacks = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> percentBonusMana = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> lethality = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> mist = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> expendedGrit = new() { 0, 0, 0, 0, 0, 0 };
    public List<float> soul = new() { 0, 0, 0, 0, 0, 0 };
}