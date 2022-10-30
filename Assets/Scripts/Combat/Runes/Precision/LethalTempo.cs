using UnityEngine;

public class LethalTempo : Rune
{
    private int stack;
    private float timer;
    private float expireTimer;
    private bool isMelee;

    public LethalTempo(RuneManager manager, bool isMelee) : base(manager)
    {
        manager.combat.OnAutoAttack += AddStack;

        this.isMelee = isMelee;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if(stack > 0 && timer <= 0)
        {
            expireTimer += Time.deltaTime;
            if (expireTimer >= 0.5f)
            {
                expireTimer = 0;
                stack--;
            }
        }
    }

    private void AddStack()
    {
        timer = 6;
        expireTimer = 0;
        if (stack < 6) stack++;
        //TODO: AS Buff and AS Cap
    }

    public override void Destroy()
    {
        manager.combat.OnAutoAttack -= AddStack;
    }

    private float AttackSpeedBonus(int level, bool isMelee)
    {
        if (isMelee)
        {
            return level switch
            {
                < 3 => 0.1f,
                < 6 => 0.11f,
                < 9 => 0.12f,
                < 12 => 0.13f,
                < 15 => 0.14f,
                _ => 0.15f
            };
        }
        else
        {
            return level switch
            {
                < 3 => 0.05f,
                < 6 => 0.06f,
                < 9 => 0.07f,
                < 12 => 0.08f,
                < 15 => 0.09f,
                _ => 0.09f
            };
        }
    }
}