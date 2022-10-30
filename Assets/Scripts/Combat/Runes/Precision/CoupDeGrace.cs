public class CoupDeGrace : Rune
{
    public CoupDeGrace(RuneManager manager) : base(manager)
    {
        manager.combat.OnPreDamage += OnDamage;
    }

    public override void Update() { }
   
    private void OnDamage(float damage)
    {
        if(manager.combat.targetStats.PercentCurrentHealth < 0.4f)
        {
            float damageIncrease = damage * 1.08f;
            //Return Damage (except true damage)
        }
    }

    public override void Destroy()
    {
        manager.combat.OnHeal -= OnDamage;
    }
}