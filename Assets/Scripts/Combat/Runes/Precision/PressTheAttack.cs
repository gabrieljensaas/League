using UnityEngine;

public class PressTheAttack : Rune
{
    private int stack;
    private float timer;
    private float cooldownTimer = 6;

    public PressTheAttack(RuneManager manager) : base(manager)
    {
        manager.combat.OnAutoAttack += AddStack;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
    }

    private void AddStack()
    {
        if (!OnCooldown()) return;

        if (stack < 3) stack++;
        else
        {
            timer = cooldownTimer;
            stack = 0;
            //TODO: Exposed Buff and extra Adaptive Damage on proc
        }
    }

    public override void Destroy()
    {
        manager.combat.OnAutoAttack -= AddStack;
    }
    private bool OnCooldown() => timer >= 0;
    private float ExposedDamageBonus(int level) => 0.08f + (0.04f / 17 * (level - 1));
}
