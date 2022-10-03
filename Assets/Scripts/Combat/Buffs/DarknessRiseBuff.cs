using UnityEngine;

public class DarknessRiseBuff : Buff
{
    private Mordekaiser mordekaiser;
    private float tickTimer;

    public DarknessRiseBuff(float duration, BuffManager manager, string source, Mordekaiser mordekaiser) : base(manager)
    {
        this.mordekaiser = mordekaiser;
        base.duration = duration;
        base.source = source;
        value = 1;
        manager.simulationManager.ShowText($"{manager.stats.name} has a stack of Darkness Rise by {source} for {duration} seconds!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();

        tickTimer += Time.deltaTime;
        if (value == 3 && tickTimer >= 1)
        {
            mordekaiser.Indestructible(manager.combat.UpdateAbilityTotalDamage(ref manager.combat.pSum, 4, new Damage(
                Mordekaiser.DarknessRiseDamage(manager.combat.myStats.level) + (0.3f * manager.combat.myStats.AP) + (manager.combat.targetStats.maxHealth * Mordekaiser.DarknessRiseTargetMaxHPPercentDamage(manager.combat.myStats.level)), SkillDamageType.Spell),
                manager.combat.myStats.passiveSkill.skillName));
            tickTimer = 0;
        }
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Darkness Rise");
        manager.buffs.Remove("DarknessRise");
    }
}
