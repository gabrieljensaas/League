using Simulator.Combat;

public class JaxAACheck : Check
{
    public JaxAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.targetStats.buffManager.buffs.TryGetValue("EmpowerBuff", out Buff buff))
        {
            damage = combat.myStats.wSkill[0].UseSkill(5, combat.wKeys[0], combat.myStats, combat.targetStats);
            buff.Kill();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}