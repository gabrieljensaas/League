using Simulator.Combat;

public class CheckKayleR : Check
{
    public CheckKayleR(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        damage.value = combat.myStats.buffManager.buffs.ContainsKey("Untargetable") ? 0 : damage.value;
        return damage;
    }
}