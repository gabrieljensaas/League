using Simulator.Combat;

public class ZedAACheck : Check
{
    private Zed zed;
    public ZedAACheck(ChampionCombat ccombat, Zed zed) : base(ccombat)
    {
        this.zed = zed;
    }

    public override Damage Control(Damage damage)
    {
        if (!zed.usedPassive && combat.targetStats.currentHealth <= combat.targetStats.maxHealth * 0.5f)
        {
            damage.value += Zed.GetZedPassivePercentByLevel(combat.myStats.level) * combat.targetStats.maxHealth * 0.01f;
            zed.StartCoroutine(zed.ContempForTheWeak());
        }
        if (zed.markedForDeath) zed.markedRawDamage += damage.value;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}