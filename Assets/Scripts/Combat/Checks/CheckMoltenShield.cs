using Simulator.Combat;
using static UnityEngine.GraphicsBuffer;

public class CheckMoltenShield : Check
{
    public CheckMoltenShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.sheilds.TryGetValue("Molten Shield", out ShieldBuff value))
        {
            float reflectedDamage = combat.myStats.eSkill.unit.flat[4] + (combat.myStats.eSkill.unit.percentAP[4] / 100 * combat.myStats.AP) * (100 / (100 + combat.targetStats.spellBlock));
            combat.targetCombat.TakeDamage(reflectedDamage, "Molten Shied");
            combat.qSum += reflectedDamage;
            combat.myUI.abilitySum[0].text = combat.qSum.ToString();
        }

        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}