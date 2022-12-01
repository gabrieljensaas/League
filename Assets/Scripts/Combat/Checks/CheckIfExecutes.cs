using Simulator.Combat;

public class CheckIfExecutes : Check
{
    private string skill;
    private int stack;
    private float multiplier;
    public CheckIfExecutes(ChampionCombat ccombat, string skill,ref int stack,ref float multiplier) : base(ccombat)
    {
        this.skill = skill;
        this.stack = stack;
        this.multiplier = multiplier;
    }

    public CheckIfExecutes(ChampionCombat ccombat, string skill, ref int stack) : base(ccombat)
    {
        this.skill = skill;
        this.stack = stack;
        this.multiplier = 1;
    }

    public CheckIfExecutes(ChampionCombat ccombat, string skill, ref float multiplier) : base(ccombat)
    {
        this.skill = skill;
        this.stack = 0;
        this.multiplier = multiplier;
    }

    public CheckIfExecutes(ChampionCombat ccombat, string skill) : base(ccombat)
    {
        this.skill = skill;
        this.stack = 0;
        this.multiplier = 1;
    }

    public override Damage Control(Damage damage)
    {
        throw new System.NotImplementedException();

    }

    public override bool Control()
    {
        switch (skill)
        {
            case "Q":
                var damageq = combat.QSkill().UseSkill(combat.myStats.qLevel, combat.qKeys[0], combat.myStats, combat.targetStats);
                if (combat.QSkill().skillDamageType == SkillDamageType.Phyiscal) damageq = (int)(damageq * 100 / (100 + combat.targetStats.armor));
                else if (combat.WSkill().skillDamageType == SkillDamageType.Spell) damageq = (int)(damageq * 100 / (100 + combat.targetStats.spellBlock));
                else if (combat.WSkill().skillDamageType == SkillDamageType.True) damageq = (int)damageq;
                return damageq >= combat.targetStats.currentHealth;
            case "W":
                var damagew = combat.WSkill().UseSkill(combat.myStats.qLevel, combat.qKeys[0], combat.myStats, combat.targetStats);
                if (combat.WSkill().skillDamageType == SkillDamageType.Phyiscal) damagew = (int)(damagew * 100 / (100 + combat.targetStats.armor));
                else if (combat.WSkill().skillDamageType == SkillDamageType.Spell) damagew = (int)(damagew * 100 / (100 + combat.targetStats.spellBlock));
                else if (combat.WSkill().skillDamageType == SkillDamageType.True) damagew = (int)damagew;
                return damagew >= combat.targetStats.currentHealth;
            case "E":
                var damagee = combat.WSkill().UseSkill(combat.myStats.qLevel, combat.qKeys[0], combat.myStats, combat.targetStats);
                if (combat.WSkill().skillDamageType == SkillDamageType.Phyiscal) damagee = (int)(damagee * 100 / (100 + combat.targetStats.armor));
                else if (combat.WSkill().skillDamageType == SkillDamageType.Spell) damagee = (int)(damagee * 100 / (100 + combat.targetStats.spellBlock));
                else if (combat.WSkill().skillDamageType == SkillDamageType.True) damagee = (int)damagee;
                return damagee >= combat.targetStats.currentHealth;
            case "R":
                var damager = (combat.RSkill().UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.myStats, combat.targetStats) * multiplier) + stack;
                if (combat.RSkill().skillDamageType == SkillDamageType.Phyiscal) damager = (int)(damager * 100 / (100 + combat.targetStats.armor));
                else if (combat.RSkill().skillDamageType == SkillDamageType.Spell) damager = (int)(damager * 100 / (100 + combat.targetStats.spellBlock));
                else if (combat.RSkill().skillDamageType == SkillDamageType.True) damager = (int)damager;
                return damager >= combat.targetStats.currentHealth;
            case "Riven":
                return (int)(combat.RSkill(1).UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.myStats, combat.targetStats) * (1 + ((combat.targetStats.maxHealth - combat.targetStats.currentHealth) / combat.targetStats.maxHealth) * 100 / (100 + combat.targetStats.armor)) > 0.75f ? 2 : (combat.targetStats.maxHealth - combat.targetStats.currentHealth) * 2.667f) >= combat.targetStats.currentHealth;
            /*case "SylasRiven":
                return combat.RSkill(1).UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.targetStats, combat.myStats) * (1 + ((combat.myStats.maxHealth - combat.myStats.currentHealth) / combat.myStats.maxHealth) > 0.75f ? 2 : (combat.myStats.maxHealth - combat.myStats.currentHealth) * 2.667f) >= combat.myStats.currentHealth;
            */
            case "Kalista":
                if (combat.TargetBuffManager.buffs.TryGetValue("Rend", out Buff value))
                {
                    return (int)(combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats) + ((value.value - 1) * combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[1], combat.myStats, combat.targetStats)) * 100 / (100 + combat.targetStats.armor)) >= combat.targetStats.currentHealth;
                }
                return (int)(combat.ESkill().UseSkill(combat.myStats.eLevel, combat.eKeys[0], combat.myStats, combat.targetStats) * 100 / (100 + combat.targetStats.armor)) >= combat.targetStats.currentHealth;
            case "Syndra":
                var damageSyndra = (combat.RSkill().UseSkill(combat.myStats.rLevel, combat.rKeys[0], combat.myStats, combat.targetStats) * multiplier) + stack + 3;
                damageSyndra = (int)(damageSyndra * 100 / (100 + combat.targetStats.spellBlock));
                return stack >= 100 ? damageSyndra >= combat.targetStats.currentHealth - (combat.targetStats.maxHealth * 0.15f) : damageSyndra >= combat.targetStats.currentHealth;
            default:
                return false;
        }
    }
}