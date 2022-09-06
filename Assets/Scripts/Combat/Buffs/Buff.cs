public abstract class Buff
{
    public float duration;
    public string source;
    protected BuffManager manager;
    public float value;
    public bool paused = false;
    protected Buff(BuffManager manager)
    {
        this.manager = manager;
    }

    public abstract void Update();
    public abstract void Kill();

    public static bool IsTotalCC(Buff buff)
    {
        if (buff is AirborneBuff
                    or BerserkBuff
                    or CharmBuff
                    or FleeBuff
                    or TauntBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
            return true;
        return false;
    }

    public static bool IsDisrupt(Buff buff)
    {
        if (buff is AirborneBuff
                    or BerserkBuff
                    or CharmBuff
                    or FleeBuff
                    or TauntBuff
                    or PolymorphBuff
                    or SilenceBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
            return true;
        return false;
    }

    public static bool IsImmobilize(Buff buff)
    {
        if (buff is AirborneBuff
                    or BerserkBuff
                    or CharmBuff
                    or FleeBuff
                    or TauntBuff
                    or RootBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
            return true;
        return false;
    }

    public static bool IsDisarm(Buff buff)
    {
        if (buff is AirborneBuff
                    or CharmBuff
                    or DisarmBuff
                    or FleeBuff
                    or SleepBuff
                    or StasisBuff
                    or StunBuff
                    or SuppressionBuff
                    or SuspensionBuff)
            return true;
        return false;
    }
}