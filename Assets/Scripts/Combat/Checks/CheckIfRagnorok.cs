using Simulator.Combat;

public class CheckIfRagnorok : Check
{
    public CheckIfRagnorok(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("Ragnorok"))
        {
            if (combat.myStats.buffManager.buffs.TryGetValue("Disarm", out Buff disarm)) disarm.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Silence", out Buff silence)) silence.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Stun", out Buff stun)) stun.Kill();
            return false;
        }
        return true;
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}