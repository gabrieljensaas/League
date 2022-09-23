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
    public override float Control(float damage)
    {
        return combat.myStats.buffManager.buffs.ContainsKey("Invulnerable") ? 0 : damage;
    }
}