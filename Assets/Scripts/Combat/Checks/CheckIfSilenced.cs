using Simulator.Combat;

public class CheckIfSilenced : Check
{
    public CheckIfSilenced(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("Silence");
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}