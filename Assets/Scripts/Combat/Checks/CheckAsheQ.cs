using Simulator.Combat;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CheckAsheQ : Check
{
    public CheckAsheQ(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("AsheQ", out Buff value))
        {
            return value.value == 4 ? true : false;
        }
        else return false;
    }

    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}