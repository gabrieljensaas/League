using Simulator.Combat;

public class PracticeDummy : ChampionCombat
{
    public float baseHealth = 1000;
    public float baseAD = 0;
    public float baseArmor = 0;
    public float baseSpellBlock = 0;
    public float baseAttackSpeed = 0;

    protected override void Start()
    {
        base.Start();
        myStats.level = 1;
        myStats.name = "Practice Dummy";

        myStats.baseHealth = baseHealth;
        myStats.baseAD = baseAD;
        myStats.baseArmor = baseArmor;
        myStats.baseSpellBlock = baseSpellBlock;
        myStats.baseAttackSpeed = baseAttackSpeed;

        myStats.maxHealth = baseHealth;
        myStats.AD = baseAD;
        myStats.armor = baseArmor;
        myStats.spellBlock = baseSpellBlock;
        myStats.attackSpeed = baseAttackSpeed;

        myStats.currentHealth = myStats.maxHealth;

        myStats.StaticUIUpdate();
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "", "", "", "", "" };
    }

    protected override void CheckPassive() { }

    //protected override void CheckDeath()
    //{
    //    if (myStats.currentHealth <= 0)
    //        myStats.currentHealth = 1000;
    //}
}
