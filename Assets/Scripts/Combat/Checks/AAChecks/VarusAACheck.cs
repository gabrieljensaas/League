using Simulator.Combat;

public class VarusAACheck : Check
{
    public VarusAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        damage.value += combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[0], combat.myStats, combat.targetStats);
        if (combat.targetStats.buffManager.buffs.TryGetValue("Blight", out Buff value))
        {
            if (value.value != 3)
            {
                value.value++;
            }
            value.duration = 6;
        }
        else
        {
            combat.targetStats.buffManager.buffs.Add("Blight", new BlightBuff(6, combat.targetStats.buffManager, "Varus's Auto Attack"));
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}