using Simulator.Combat;

public abstract class Pet
{
    protected ChampionCombat owner;

    protected Pet(ChampionCombat owner)
    {
        this.owner = owner;
    }

    public abstract void Update();
}