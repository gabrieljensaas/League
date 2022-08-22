public abstract class Buff
{
    public float duration;
    public string source;
    protected BuffManager manager;
    public float value;
    protected Buff(BuffManager manager)
    {
        this.manager = manager;
    }

    public abstract void Update();
    public abstract void Kill();
}