public class PressTheAttack : Rune
{
    public PressTheAttack(RuneManager manager) : base(manager)
    {
        manager.combat.OnAutoAttack += AddStack;
    }

    public override void Update()
    {

    }

    private void AddStack()
    {

    }
}
