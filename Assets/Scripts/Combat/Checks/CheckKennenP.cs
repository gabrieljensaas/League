using Simulator.Combat;

public class CheckKennenP : Check
{
    public CheckKennenP(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("MarkOfTheStorm", out Buff value))
        {
            return value.value == 3;
        }
        return false;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        throw new System.NotImplementedException();
    }
}