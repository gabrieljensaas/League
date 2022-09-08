using Simulator.Combat;

public class Aatrox : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "Q", "W", "A", "" };
        checksQ.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        autoattackcheck = new AatroxAACheck(this);
        base.UpdatePriorityAndChecks();
    }

    protected override void CheckPassive()
    {
        base.CheckPassive();

        if (!myStats.buffManager.buffs.ContainsKey("DeathbringerStance"))
        {
            myStats.buffManager.buffs.Add("DeathbringerStance", new DeathbringerStanceBuff(float.MaxValue, myStats.buffManager, myStats.passiveSkill.name));
        }
    }
}
