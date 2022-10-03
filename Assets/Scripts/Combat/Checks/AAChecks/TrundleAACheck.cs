using Simulator.Combat;

public class TrundleAACheck : Check
{
    public TrundleAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Chomp", out Buff value))
        {
            damage += value.value;
            combat.qSum += value.value;
            combat.myUI.abilitySum[0].text = combat.qSum.ToString();
            combat.myStats.buffManager.buffs.Remove("Chomp");
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}