using Simulator.Combat;

public class SylasAACheck : Check
{
    private Sylas sylas;
    public SylasAACheck(ChampionCombat ccombat, Sylas sylas) : base(ccombat)
    {
        this.sylas = sylas;
    }

    public override Damage Control(Damage damage)
    {
        if (sylas.UnshackledStack > 0)
        {
            sylas.UnshackledStack--;
            if (sylas.UnshackledStack == 0)
            {
                sylas.StopCoroutine(sylas.Unshackled());
                sylas.myStats.buffManager.buffs["Unshackled"].Kill();
            }

            damage.value = (damage.value * 1.3f) + (combat.myStats.AP * 0.2f);
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}