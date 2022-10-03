using Simulator.Combat;

public class KaisaAACheck : Check
{
    private Kaisa kaisa;
    public KaisaAACheck(ChampionCombat ccombat, Kaisa kaisa) : base(ccombat)
    {
        this.kaisa = kaisa;
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.targetStats.buffManager.buffs.TryGetValue("Plasma", out Buff value))
        {
            damage += Kaisa.GetKaisaPassiveDamageByLevel(combat.myStats.level, (int)value.value, combat.myStats.AP);
            if (value.value == 5)
            {
                kaisa.DealPassiveDamage((combat.targetStats.maxHealth - combat.targetStats.currentHealth) / 100 * (15 + (5 * (combat.myStats.AP % 100))));
                value.Kill();
            }
            else
            {
                value.value++;
                value.duration = 4f;
            }
        }
        else
        {
            combat.targetStats.buffManager.buffs.Add("Plasma", new PlasmaBuff(4, combat.targetStats.buffManager, "Kaisa's Auto Attacks"));
        }
        combat.myStats.eCD -= 0.5f;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}