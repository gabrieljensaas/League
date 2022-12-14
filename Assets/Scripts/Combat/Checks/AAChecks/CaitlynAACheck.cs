using Simulator.Combat;
using System;
using Unity.VisualScripting;

public class CaitlynAACheck : Check
{
    public CaitlynAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("NetHeadshot", out Buff v))
        {
            damage += (Constants.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD;                  //critical chance 0 for now since no item added
            v.Kill();
            return damage;
        }

        if (combat.myStats.buffManager.buffs.TryGetValue("TrapHeadshot", out Buff val))
        {
            damage += ((Constants.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD) + combat.myStats.wSkill[0].UseSkill(4, combat.myStats, combat.targetStats);                  //critical chance 0 for now since no item added
            val.Kill();
            return damage;
        }

        if (combat.myStats.buffManager.buffs.TryGetValue("Headshot", out Buff value))
        {
            if(value.value == 6)
            {
                damage += (Constants.GetCaitlynPassivePercent(combat.myStats.level) + 0) * combat.myStats.AD;                   //critical chance 0 for now since no item added
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