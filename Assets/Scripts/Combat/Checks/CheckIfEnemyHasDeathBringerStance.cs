using Simulator.Combat;

public class CheckIfEnemyHasDeathBringerStance : Check
{
    private Aatrox aatrox;
    public CheckIfEnemyHasDeathBringerStance(ChampionCombat ccombat, Aatrox aatrox) : base(ccombat)
    {
        this.aatrox = aatrox;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override Damage Control(Damage damage)
    {
        if (combat.targetStats.buffManager.buffs.TryGetValue("DeathbringerStance", out Buff value))
        {
            aatrox.UpdateTotalHeal(ref aatrox.hSum, damage.value, "Deathbringer Stance");
            value.Kill();
            aatrox.pCD = Aatrox.AatroxPassiveCooldownByLevelTable[aatrox.myStats.level - 1];
        }
        return damage;
    }
}