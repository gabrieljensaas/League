using Simulator.Combat;

public class MalphiteAACheck : Check
{
    public MalphiteAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("ThunderclapBuff"))
            damage = combat.myStats.wSkill[0].UseSkill(5, combat.wKeys[2], combat.myStats, combat.targetStats);

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}