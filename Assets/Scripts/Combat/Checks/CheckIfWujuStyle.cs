using Simulator.Combat;

public class CheckIfWujuStyle : Check
{
    public CheckIfWujuStyle(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        return !combat.myStats.buffManager.buffs.ContainsKey("WujuStyle");
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}