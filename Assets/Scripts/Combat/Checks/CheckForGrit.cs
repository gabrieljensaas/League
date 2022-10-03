using Simulator.Combat;

public class CheckForGrit : Check
{
    private Sett sett;
    public CheckForGrit(ChampionCombat ccombat, Sett sett) : base(ccombat)
    {
        this.sett = sett;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (damage < (sett.myStats.maxHealth * 0.5f) - sett.grit)
        {
            sett.grit += damage;
            sett.gritList.Add(new GritBuff(4, combat.myStats.buffManager, "Haymaker", damage, sett));
        }
        else
        {
            sett.gritList.Add(new GritBuff(4, combat.myStats.buffManager, "Haymaker", (sett.myStats.maxHealth * 0.5f) - sett.grit, sett));
            sett.grit = sett.myStats.maxHealth * 0.5f;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}