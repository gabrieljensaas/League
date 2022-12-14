using Simulator.Combat;

public class AsheAACheck : Check
{
    public AsheAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Flurry", out Buff value))
        {
            damage *= value.value / 100;
            combat.qSum += value.value * damage / 100;
            combat.myUI.abilitySum[0].text = combat.qSum.ToString();
        }
        else
        {
            if (combat.myStats.buffManager.buffs.TryGetValue("AsheQ", out Buff qstack))
            {
                if (qstack.value != 4) qstack.value++;
                combat.myStats.buffManager.buffs["AsheQ"].duration = 4;
            }
            else
            {
                combat.myStats.buffManager.buffs.Add("AsheQ" ,new AsheQBuff(4, combat.myStats.buffManager, "Ashe's Auto Attack"));
            }
        }

        if(!combat.targetStats.buffManager.buffs.TryAdd("Frosted" ,new FrostedBuff(2, combat.targetStats.buffManager, "Ashe's Auto Attack")))
        {
            combat.targetStats.buffManager.buffs["Frosted"].duration = 2;
            combat.targetStats.buffManager.buffs["Frosted"].source = "Ashe's Auto Attack";
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}