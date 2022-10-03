using Simulator.Combat;

public class OlafAACheck : Check
{
    public OlafAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        combat.myStats.eCD--;

        if (combat.myStats.buffManager.buffs.TryGetValue(combat.myStats.rSkill[0].basic.name, out Buff value))
        {
            value.duration += 2.5f;
        }
        if (combat.myStats.buffManager.buffs.TryGetValue(combat.myStats.rSkill[0].basic.name + " ", out Buff val))
        {
            val.duration += 2.5f;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}