using Simulator.Combat;

public class Aatrox : ChampionCombat
{
    public static float[] AatroxPassiveCooldownByLevelTable = { 24, 23.29f, 22.59f, 21.88f, 21.18f, 20.47f, 19.76f, 19.06f, 18.35f, 17.65f, 16.94f, 16.24f, 15.53f, 14.82f, 14.12f, 13.41f, 12.71f, 12f };

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
