using Simulator.Combat;

public class CheckTryndamereUndyingRage : Check
{
    public CheckTryndamereUndyingRage(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("UndyingRage") && combat.myStats.currentHealth - damage.value < combat.myStats.rSkill[0].UseSkill(2, combat.rKeys[1], combat.myStats, combat.targetStats))
            damage.value = combat.myStats.currentHealth + combat.myStats.rSkill[0].UseSkill(2, combat.rKeys[1], combat.myStats, combat.targetStats);

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}