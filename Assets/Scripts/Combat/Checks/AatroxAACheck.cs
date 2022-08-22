using Simulator.Combat;

public class AatroxAACheck : Check
{
    public AatroxAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage)
    {
        if (combat.myStats.buffManager.buffs.ContainsKey("DeathbringerStance"))
        {
            damage += combat.targetStats.maxHealth * (5 + (7 / 17 * (combat.myStats.level - 1))) / 100 * (100 / (100 + combat.targetStats.armor));
            combat.pSum += combat.targetStats.maxHealth * (5 + (7 / 17 * (combat.myStats.level - 1))) / 100 * (100 / (100 + combat.targetStats.armor));
            combat.myUI.abilitySum[4].text = combat.pSum.ToString();
            combat.myStats.buffManager.buffs.Remove("DeathbringerStance");
            combat.myStats.Heal(damage);
            combat.simulationManager.ShowText($"{combat.name} Healed From Death Bringer Stance For {damage}!");
            combat.myStats.pCD = combat.myStats.passiveSkill.coolDown;
        }
        else combat.myStats.pCD -= 2;
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}