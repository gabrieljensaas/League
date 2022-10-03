using Simulator.Combat;

public class CheckKindredR : Check
{
    public CheckKindredR(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        double maxHealthLimit = combat.myStats.maxHealth * 0.01;
        if (combat.myStats.currentHealth == maxHealthLimit)
        {
            damage.value = combat.myStats.buffManager.buffs.ContainsKey("Untargetable") ? 0 : damage.value;
            return damage;
        }
        else
        {
            return damage;
        }
    }
}