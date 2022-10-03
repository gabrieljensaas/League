public abstract class Rune
{
    protected RuneManager manager;

    protected Rune(RuneManager manager)
    {
        this.manager = manager;
    }

    public abstract void Update();
}