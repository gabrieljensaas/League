using Simulator.Combat;

public class MasterYiAACheck : Check
{
    public MasterYiAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        combat.myStats.qCD--;
        if (combat.myStats.buffManager.buffs.TryGetValue("DoubleStrike", out Buff value))
        {
            if(value.value == 3)
            {
                combat.simulationManager.ShowText($"{combat.myStats.name} Used His Stacks of Double Strike, His Next Attack Will Deal Extra Damage!");
                combat.myStats.qCD--;
                return damage * 1.5f;
            }
            else
            {
                value.value++;
                value.duration = 4;
                combat.simulationManager.ShowText($"{combat.myStats.name} Gained a Stack of Double Strike!");
                return damage;
            }
        }
        else
        {
            combat.myStats.buffManager.buffs.Add("DoubleStrike", new DoubleStrikeBuff(4, combat.myStats.buffManager, "Double Strike"));
            return damage;
        }
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}