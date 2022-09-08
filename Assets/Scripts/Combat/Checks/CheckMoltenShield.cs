using Simulator.Combat;

public class CheckMoltenShield : Check
{
    public CheckMoltenShield(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.shields.TryGetValue("Molten Shield", out ShieldBuff value))
        {
            float reflectedDamage = combat.myStats.eSkill[0].unit.flat[combat.eKeys[0]][4] + (combat.myStats.eSkill[0].unit.percentAP[combat.eKeys[0]][4] / 100 * combat.myStats.AP) * (100 / (100 + combat.targetStats.spellBlock));
            combat.targetCombat.TakeDamage(reflectedDamage, "Molten Shied", SkillDamageType.Spell);
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