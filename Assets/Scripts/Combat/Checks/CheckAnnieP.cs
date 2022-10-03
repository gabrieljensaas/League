using Simulator.Combat;

public class CheckAnnieP : Check
{
    public CheckAnnieP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff value))
        {
            return value.value == 4;
        }
        return false;
    }

    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}