using Simulator.Combat;

public class CheckIfBlind : Check
{
    public CheckIfBlind(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        return combat.myStats.buffManager.buffs.ContainsKey("Blind") ? 0 : damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}