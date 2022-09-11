using Simulator.Combat;

public class DravenAACheck : Check
{
    private Draven draven;
    public DravenAACheck(ChampionCombat ccombat, Draven draven) : base(ccombat)
    {
        this.draven = draven;
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("SpinningAxe", out Buff value))
        {
            value.value--;
            combat.StartCoroutine(draven.SpinnigAxe());
            damage += combat.myStats.qSkill[0].UseSkill(4, combat.qKeys[0], combat.myStats, combat.targetStats);
            if (value.value == 0) value.Kill();
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}