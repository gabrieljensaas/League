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
    public override Damage Control(Damage damage)
    {
        if (combat.targetStats.buffManager.buffs.ContainsKey(aatrox.myStats.rSkill[0].basic.name) && combat.myStats.eLevel > -1)
        {
            aatrox.UpdateTotalHeal(ref aatrox.hSum, damage.value * aatrox.myStats.eSkill[0].UseSkill(aatrox.myStats.eLevel, aatrox.eKeys[1], aatrox.myStats, combat.myStats), aatrox.ESkill().basic.name);
        }
        else if(combat.myStats.eLevel > -1)
        {
            aatrox.UpdateTotalHeal(ref aatrox.hSum, damage.value * aatrox.myStats.eSkill[0].UseSkill(aatrox.myStats.eLevel, aatrox.eKeys[0], aatrox.myStats, combat.myStats), aatrox.ESkill().basic.name);
        }
        return damage;
    }
}