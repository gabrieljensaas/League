using UnityEngine;

public class Conqueror : Rune
{
    private int stack;
    private float timer;
    private bool isMelee;

    public Conqueror(RuneManager manager, bool isMelee) : base(manager)
    {
        this.isMelee = isMelee;

        manager.combat.OnAutoAttack += AddStackAA;
        manager.combat.OnAbilityHit += AddStackAbility;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (stack > 0 && timer <= 0)
            stack = 0;
    }

    private void AddStackAA()
    {
        timer = 5;
        if(isMelee)
            Mathf.Clamp(stack += 2, 0, 12);
        else
            Mathf.Clamp(stack++, 0, 12);

        ApplyBuffs();
    }

    private void AddStackAbility()
    {
        timer = 5;
        Mathf.Clamp(stack += 2, 0, 12);

        ApplyBuffs();
    }

    private void ApplyBuffs()
    {
        //TODO: Adaptive AD/AP
        //TODO: Heal Post Mitigation Damage only at max stacks
    }

    public override void Destroy()
    {
        manager.combat.OnAutoAttack -= AddStackAA;
        manager.combat.OnAbilityHit -= AddStackAbility;
    }
}
