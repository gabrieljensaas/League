using Simulator.Combat;

public class ZiggsAACheck : Check
{
    public ZiggsAACheck(ChampionCombat ccombat) : base(ccombat)
    {

    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if(combat.myStats.pCD == 0)
		{
            return damage + Ziggs.ShortFuseDamageByLevel(combat.myStats.level);
        }
		else
		{
            return damage;
		}

    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}
