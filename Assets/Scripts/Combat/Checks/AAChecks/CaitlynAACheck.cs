using Simulator.Combat;

public class CaitlynAACheck : Check
{
    public CaitlynAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("NetHeadshot", out Buff v))
        {
            damage.value += (Caitlyn.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD;                  //critical chance 0 for now since no item added
            v.Kill();
            return damage;
        }

        if (combat.myStats.buffManager.buffs.TryGetValue("TrapHeadshot", out Buff val))
        {
            damage.value += ((Caitlyn.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD) + combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[1], combat.myStats, combat.targetStats);                  //critical chance 0 for now since no item added
            val.Kill();
            return damage;
        }

        if (combat.myStats.buffManager.buffs.TryGetValue("Headshot", out Buff value))
        {
            if (value.value == 6)
            {
                damage.value += (Caitlyn.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD;                   //critical chance 0 for now since no item added
                value.Kill();
            }
            else
            {
                value.value++;
            }
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}