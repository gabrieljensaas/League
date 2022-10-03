using Simulator.Combat;

public class CheckAniviaP : Check
{
    private Anivia anivia;
    public CheckAniviaP(ChampionCombat ccombat, Anivia anivia) : base(ccombat)
    {
        this.anivia = anivia;
    }

    public override Damage Control(Damage hp)
    {
        hp.value = combat.myStats.currentHealth;
        anivia.PassiveEgg(hp.value);
        return hp;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}