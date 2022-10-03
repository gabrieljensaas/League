using Simulator.Combat;

public class CheckForAatroxEHeal : Check
{
    private Aatrox aatrox;
    public CheckForAatroxEHeal(ChampionCombat ccombat, Aatrox aatrox) : base(ccombat)
    {
        this.aatrox = aatrox;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.targetStats.buffManager.buffs.ContainsKey(aatrox.myStats.rSkill[0].basic.name))
        {
            aatrox.UpdateTotalHeal(ref aatrox.hSum, damage * aatrox.myStats.eSkill[0].UseSkill(4, aatrox.eKeys[1], aatrox.myStats, combat.myStats), aatrox.myStats.eSkill[0].basic.name);
        }
        else
        {
            aatrox.UpdateTotalHeal(ref aatrox.hSum, damage * aatrox.myStats.eSkill[0].UseSkill(4, aatrox.eKeys[0], aatrox.myStats, combat.myStats), aatrox.myStats.eSkill[0].basic.name);
        }
        return damage;
    }
}