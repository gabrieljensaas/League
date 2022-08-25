using Simulator.Combat;

public class Gangplank : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "A", "R", "E", "W", "Q" };
        base.UpdatePriorityAndChecks();
    }
}
