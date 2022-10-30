using UnityEngine;

public class CutDown : Rune
{
    public CutDown(RuneManager manager) : base(manager)
    {
        manager.combat.OnPreDamage += OnDamage;
    }

    public override void Update() { }

    private void OnDamage(float damage)
    {
        float targetAdditionalMaxHP = Mathf.Clamp((manager.combat.targetStats.maxHealth - manager.stats.maxHealth) / manager.stats.maxHealth, 0, 1);
        if(targetAdditionalMaxHP >= 0.1f)
        {
            float damageIncrease = ((targetAdditionalMaxHP - 0.1f) * (1f/9f)) + 1.05f;
            //Return Damage (except true damage)
        }
    }

    public override void Destroy()
    {
        manager.combat.OnHeal -= OnDamage;
    }
}