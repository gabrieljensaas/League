using Simulator.Combat;

public class CheckMoundingDread : Check
{
    public CheckMoundingDread(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("MoundingDread", out Buff value))
        {
            return value.value == 4;
        }
        return false;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}