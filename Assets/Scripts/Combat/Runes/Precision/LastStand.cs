using UnityEngine;

public class LastStand : Rune
{
    public LastStand(RuneManager manager) : base(manager)
    {
        manager.combat.OnPreDamage += OnDamage;
    }

    public override void Update() { }

    private void OnDamage(float damage)
    {
        //Return Damage (except true damage)
    }

    public override void Destroy()
    {
        manager.combat.OnHeal -= OnDamage;
    }

    private float LastStandDamageBonus()
    {
        return manager.stats.PercentMissingHealth switch
        {
            >= 0.7f => 0.11f,
            >= 0.6f => 0.9f,
            >= 0.5f => 0.7f,
            >= 0.4f => 0.5f,
            _ => 0f
        };
    }
}