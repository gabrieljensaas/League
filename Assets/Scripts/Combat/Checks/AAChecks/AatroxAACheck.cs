using Simulator.Combat;

public class AatroxAACheck : Check
{
    private Aatrox aatrox;
    public AatroxAACheck(ChampionCombat ccombat, Aatrox aatrox) : base(ccombat)
    {
        this.aatrox = aatrox;
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("DeathbringerStance"))
        {
            damage += combat.targetStats.maxHealth * (5 + (7 / 17 * (combat.myStats.level - 1)));
        }
        else aatrox.pCD -= 2;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}