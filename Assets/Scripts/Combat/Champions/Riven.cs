using Simulator.Combat;

public class Riven : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "A", "Q", "W", "E", "R" };
        base.UpdatePriorityAndChecks();
    }
}
