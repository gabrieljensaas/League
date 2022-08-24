using Simulator.Combat;

public class CheckIfUnableToAct : Check
{
    public CheckIfUnableToAct(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("UnableToAct");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}