using Simulator.Combat;

public class ZedAACheck : Check
{
    private Zed zed;
    public ZedAACheck(ChampionCombat ccombat, Zed zed) : base(ccombat)
    {
        this.zed = zed;
    }

    public override float Control(float damage)
    {
        if(!zed.usedPassive && combat.targetStats.currentHealth <= combat.targetStats.maxHealth * 0.5f)
        {
            damage += Constants.GetZedPassivePercentByLevel(combat.myStats.level) * combat.targetStats.maxHealth * 0.01f;
            zed.StartCoroutine(zed.ContempForTheWeak());
        }
        if (zed.markedForDeath) zed.markedRawDamage += damage;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}