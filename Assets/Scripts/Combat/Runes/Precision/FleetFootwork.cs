using UnityEngine;

public class FleetFootwork : Rune
{
    private int stack;

    public FleetFootwork(RuneManager manager) : base(manager)
    {
        manager.combat.OnAutoAttack += AddStack;
    }

    public override void Update() { }

    private void AddStack()
    {
        if (stack == 100)
        {
            stack = 0;
            //TODO: Heal + Movement Speed
        }

        Mathf.Clamp(stack += 6, 0, 100);
    }

    public override void Destroy()
    {
        manager.combat.OnAutoAttack -= AddStack;
    }
}
