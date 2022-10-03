using Simulator.Combat;
using System.Linq;

public class CheckShield : Check
{
    public CheckShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override Damage Control(Damage damage)
    {
        foreach (var item in combat.myStats.buffManager.shields.Values.ToList())
        {
            if (item.shieldType == ShieldBuff.ShieldType.Magic && damage.damageType != SkillDamageType.Spell) continue;
            if (item.shieldType == ShieldBuff.ShieldType.Physical && damage.damageType != SkillDamageType.Phyiscal) continue;
            if (item.value >= damage.value)
            {
                item.value -= damage.value;
                combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {damage} Damage!");
                break;
            }
            else
            {
                combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {item.value} Damage!");
                damage.value -= item.value;
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