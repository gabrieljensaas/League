using Simulator.Combat;

public class CheckIfImmuneToCC : Check
{
    private string skillName;
    public CheckIfImmuneToCC(ChampionCombat ccombat, string skillName) : base(ccombat)
    {
        this.skillName = skillName;
    }

    public override bool Control()
    {
        if (combat.myStats.buffManager.buffs.ContainsKey(skillName))
        {
            if (combat.myStats.buffManager.buffs.TryGetValue("Disarm", out Buff disarm)) disarm.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Silence", out Buff silence)) silence.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Stun", out Buff stun)) stun.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Airborne", out Buff airborne)) airborne.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Berserk", out Buff Berserk)) Berserk.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Blind", out Buff Blind)) Blind.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Charm", out Buff Charm)) Charm.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Cripple", out Buff Cripple)) Cripple.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Disrupt", out Buff Disrupt)) Disrupt.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Drowsy", out Buff Drowsy)) Drowsy.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Flee", out Buff Flee)) Flee.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Ground", out Buff Ground)) Ground.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Kinematics", out Buff Kinematics)) Kinematics.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Knockdown", out Buff Knockdown)) Knockdown.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Nearsight", out Buff Nearsight)) Nearsight.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Polymorph", out Buff Polymorph)) Polymorph.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Root", out Buff Root)) Root.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Sleep", out Buff Sleep)) Sleep.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Slow", out Buff Slow)) Slow.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Stasis", out Buff Stasis)) Stasis.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Supression", out Buff Supression)) Supression.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Suspension", out Buff Suspension)) Suspension.Kill();
            if (combat.myStats.buffManager.buffs.TryGetValue("Taunt", out Buff Taunt)) Taunt.Kill();

            return false;
        }
        return true;
    }
    public override float Control(float damage)
    {
        throw new System.NotImplementedException();
    }
}