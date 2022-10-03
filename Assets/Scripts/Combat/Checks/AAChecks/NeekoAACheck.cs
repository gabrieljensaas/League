using Simulator.Combat;

public class NeekoAACheck : Check
{
    private Neeko neeko;
    public NeekoAACheck(ChampionCombat ccombat, Neeko neeko) : base(ccombat)
    {
        this.neeko = neeko;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (neeko.HasWPassive)
        {
            damage += combat.WSkill().UseSkill(4, combat.wKeys[0], combat.myStats, combat.targetStats);
            neeko.HasWPassive = false;
            neeko.NeekoWPassive();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}