using Simulator.Combat;

public class CheckKindredR : Check
{
    public CheckKindredR(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        double maxHealthLimit = combat.myStats.maxHealth * 0.01 ;
        if (combat.myStats.currentHealth == maxHealthLimit)
        {
            return combat.myStats.buffManager.buffs.ContainsKey("Untargetable") ? 0 : damage;
        }
        else
        {
            return damage;
        }
    }
}