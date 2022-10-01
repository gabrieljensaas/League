using Simulator.Combat;

public class CheckAniviaP : Check
{
    private Anivia anivia;
    public CheckAniviaP(ChampionCombat ccombat, Anivia anivia) : base(ccombat)
    {
        this.anivia = anivia;
    }

    public override float Control(float hp)
    {
        hp = combat.myStats.currentHealth;
        anivia.PassiveEgg(hp);
        return hp;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}