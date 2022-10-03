using Simulator.Combat;

public class RenektonAACheck : Check
{
    private Renekton renekton;
    public RenektonAACheck(ChampionCombat ccombat, Renekton renekton) : base(ccombat)
    {
        this.renekton = renekton;
    }

    public override Damage Control(Damage damage)
    {
        if (renekton.wStun == 2)
        {
            combat.targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.5f, combat.targetStats.buffManager, combat.myStats.wSkill[0].basic.name));
            combat.targetStats.buffManager.shields.Clear();
            damage.value += combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[1], combat.myStats, combat.targetStats);
            renekton.wStun = 0;
        }
        else if (renekton.wStun == 1)
        {
            combat.targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, combat.targetStats.buffManager, combat.myStats.wSkill[0].basic.name));
            damage.value += combat.myStats.wSkill[0].UseSkill(4, combat.wKeys[0], combat.myStats, combat.targetStats);
            renekton.wStun = 0;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}