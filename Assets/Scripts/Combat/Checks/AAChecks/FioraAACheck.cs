using Simulator.Combat;

public class FioraAACheck : Check
{
    public FioraAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Bladework", out Buff value))
        {
            if (value.value == 1)
            {
                damage.value *= combat.myStats.eSkill[0].UseSkill(4, combat.eKeys[1], combat.myStats, combat.targetStats); //level 5 bladework
                value.Kill();
            }
            else
            {
                combat.targetStats.buffManager.buffs.Add("Slow", new SlowBuff(1, combat.targetStats.buffManager, "Bladework"));
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