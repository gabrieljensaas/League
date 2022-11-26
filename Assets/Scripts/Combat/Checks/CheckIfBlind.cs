using Simulator.Combat;

public class CheckIfBlind : Check
{
    public CheckIfBlind(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        damage.value = combat.TargetBuffManager.buffs.ContainsKey("Blind") ? 0 : damage.value;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}