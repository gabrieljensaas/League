using UnityEngine;

public class DreamDustBuff : Buff
{
    private float tickTimer;

    public DreamDustBuff(float duration, BuffManager manager, string source) : base(manager)
    {
        base.duration = duration;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} has Dream Dust!");
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) Kill();

        tickTimer += Time.deltaTime;
        if(tickTimer >= 1)
        {
            manager.combat.UpdateAbilityTotalDamage(ref manager.combat.pSum, 4, (manager.combat.targetStats.maxHealth * 0.06f + (0.012f * (int)(manager.combat.myStats.AP / 100))) / 3, manager.combat.myStats.passiveSkill.skillName, SkillDamageType.Spell);
            manager.combat.UpdateTotalHeal(ref manager.combat.hSum, Lillia.DreamDust(4) * manager.combat.myStats.AP * 0.18f / 3, manager.combat.myStats.passiveSkill.name);
            tickTimer = 0;
        }
    }
    public override void Kill()
    {
        manager.simulationManager.ShowText($"{manager.stats.name} no longer has Dream Dust");
        manager.buffs.Remove("DreamDust");
    }
}
