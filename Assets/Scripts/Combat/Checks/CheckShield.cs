using Simulator.Combat;
using System.Linq;

public class CheckShield : Check
{
    public CheckShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        foreach (var item in combat.myStats.buffManager.shields.Values.ToList())
        {
            if (item.value >= damage)
            {
                item.value -= damage;
                combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {damage} Damage!");
                break;
            }
            else
            {
                combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {item.value} Damage!");
                damage -= item.value;
                combat.myStats.buffManager.shields.Remove(item.uniqueKey);
            }
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}