using System.Collections.Generic;

public class OutputJSON
{
    public string matchID { get; set; }
    public FightData fightData { get; set; }
}

public class AutoAttackDatum
{
    public string champName { get; set; }
    public int value { get; set; }
    public string time { get; set; }
}

public class FightData
{
    public List<AutoAttackDatum> autoAttackData { get; set; }
    public List<SpellDatum> spellData { get; set; }
    public List<PassiveDatum> passiveData { get; set; }
}

public class PassiveDatum
{
    public string champName { get; set; }
    public string spellName { get; set; }
    public int value { get; set; }
    public string time { get; set; }
}

public class SpellDatum
{
    public string champName { get; set; }
    public string spellName { get; set; }
    public int value { get; set; }
    public string time { get; set; }
}

public class SpellE
{
    public int value { get; set; }
    public double time { get; set; }
}

public class SpellQ
{
    public int value { get; set; }
    public double time { get; set; }
}

public class SpellR
{
    public int value { get; set; }
    public double time { get; set; }
}

public class SpellW
{
    public int value { get; set; }
    public double time { get; set; }
}