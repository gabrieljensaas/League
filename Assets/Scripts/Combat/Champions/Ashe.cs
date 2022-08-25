using Simulator.Combat;

public class Ashe : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "A", "W", "R", "" };
        checksQ.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckAsheQ(this));
        targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
        checksW.Add(new CheckIfCasting(this));
        targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
        checksE.Add(new CheckIfCasting(this));
        targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
        checksR.Add(new CheckIfCasting(this));
        targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
        autoattackcheck = new AsheAACheck(this);
        targetCombat.checkTakeDamageAA.Add(new CheckIfFrosted(targetCombat));
        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }
}
