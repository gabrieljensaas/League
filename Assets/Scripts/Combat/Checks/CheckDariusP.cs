using Simulator.Combat;

public class CheckDariusP : Check
{
    public CheckDariusP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.targetStats.buffManager.buffs.TryGetValue("Hemorrhage", out Buff value))
        {
            return value.value == 5;
        }
        return false;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}
