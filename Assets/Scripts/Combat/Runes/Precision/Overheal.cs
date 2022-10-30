using UnityEngine;

public class Overheal : Rune
{
    private float timer;
    private float expireTimer;
    private int level;

    public Overheal(RuneManager manager) : base(manager)
    {
        manager.combat.OnHeal += OnHeal;

        level = manager.stats.level;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            expireTimer += Time.deltaTime;
            if (expireTimer >= 1)
            {
                expireTimer = 0;
            }
        }
    }

    private void OnHeal(float heal)
    {
        timer = 6;
    }

    public override void Destroy()
    {
        manager.combat.OnHeal -= OnHeal;
    }

    private float HealingConversion() => 0.2f + (0.8f / 17 * (level - 1));
}
