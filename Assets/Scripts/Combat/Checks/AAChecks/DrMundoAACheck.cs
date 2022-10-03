using Simulator.Combat;

public class DrMundoAACheck : Check
{
    DrMundo drMundo;
    public DrMundoAACheck(ChampionCombat ccombat, DrMundo drMundo) : base(ccombat)
    {
        this.drMundo = drMundo;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (drMundo.EActive)
        {
            float missingHP = (combat.myStats.maxHealth - combat.myStats.currentHealth) / combat.myStats.maxHealth;
            float multiplier = missingHP > 0.4 ? 0.6f: missingHP * 1.5f;
            damage += combat.myStats.eSkill[0].UseSkill(4, combat.eKeys[2], combat.myStats, combat.targetStats) * (multiplier + 1);
            drMundo.StopCoroutine(drMundo.BluntForceTrauma());
            drMundo.EActive = false;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}