using Simulator.Combat;

public class DariusAACheck : Check
{
    public DariusAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("Crippling Strike", out Buff value))
        {
            damage += value.value;
            combat.wSum += value.value;
            combat.myUI.abilitySum[0].text = combat.wSum.ToString();
            combat.myStats.buffManager.buffs.Remove("Crippling Strike");
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}