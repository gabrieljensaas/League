using Simulator.Combat;

public class JaxAACheck : Check
{
    public JaxAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.TargetBuffManager.buffs.TryGetValue("EmpowerBuff", out Buff buff))
        {
            damage.value = combat.WSkill().UseSkill(combat.myStats.wLevel, combat.wKeys[0], combat.myStats, combat.targetStats);
            buff.Kill();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}