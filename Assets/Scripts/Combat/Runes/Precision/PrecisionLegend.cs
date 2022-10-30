public class PrecisionLegend : Rune
{
    protected int stack;
    public PrecisionLegend(RuneManager manager, int stack) : base(manager) 
    {
        this.stack = stack;
    }
    public override void Update() { }
    public override void Destroy() { }
}

public class LegendAlacrity : PrecisionLegend
{
    public LegendAlacrity(RuneManager manager, int stack) : base(manager, stack) { }
    public float BonusAttackSpeed() => 0.03f + (0.015f * (stack / 100));
}

public class LegendTenacity : PrecisionLegend
{
    public LegendTenacity(RuneManager manager, int stack) : base(manager, stack) { }
    public float BonusTenacity() => 0.05f + (0.025f * (stack / 100));
}

public class LegendBloodline : PrecisionLegend
{
    public LegendBloodline(RuneManager manager, int stack) : base(manager, stack) { }
    public float BonusAttackSpeed() => 0.04f + (stack / 100);
    public int BonusHP() => stack == 1500 ? 100 : 0;
}