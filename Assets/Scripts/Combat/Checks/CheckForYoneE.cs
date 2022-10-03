using Simulator.Combat;

public class CheckForYoneE : Check
{
    private Yone yone;
    public CheckForYoneE(ChampionCombat ccombat, Yone yone) : base(ccombat)
    {
        this.yone = yone;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (yone.InE) yone.EDamage += damage * yone.myStats.eSkill[0].UseSkill(4, yone.eKeys[0], yone.myStats, combat.myStats);
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}