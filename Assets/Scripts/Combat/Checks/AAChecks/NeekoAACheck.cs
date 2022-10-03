using Simulator.Combat;

public class NeekoAACheck : Check
{
    private Neeko neeko;
    public NeekoAACheck(ChampionCombat ccombat, Neeko neeko) : base(ccombat)
    {
        this.neeko = neeko;
    }

    public override Damage Control(Damage damage)
    {
        if (neeko.HasWPassive)
        {
            damage.value += combat.WSkill().UseSkill(4, combat.wKeys[0], combat.myStats, combat.targetStats);
            neeko.HasWPassive = false;
            neeko.NeekoWPassive();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}