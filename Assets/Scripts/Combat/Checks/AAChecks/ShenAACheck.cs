using Simulator.Combat;

public class ShenAACheck : Check
{
    public ShenAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if(combat.myStats.buffManager.buffs.TryGetValue("TwilightAssault", out Buff value))
        {
            damage += combat.myStats.qSkill[0].UseSkill(4, combat.qKeys[0], combat.myStats, combat.targetStats) - Shen.GetShenQBaseDamageByLevel(4) + Shen.GetShenQBaseDamageByLevel(combat.myStats.level);
            value.value--;
            if (value.value == 0)
            {
                value.Kill();
                combat.myStats.buffManager.buffs[combat.myStats.qSkill[0].basic.name].Kill();
            }
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}