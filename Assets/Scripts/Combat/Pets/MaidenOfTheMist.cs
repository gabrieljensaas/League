using Simulator.Combat;
using UnityEngine;

public class MaidenOfTheMist : Pet
{
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;
    private float touchOfTheMaidenTimer;

    public MaidenOfTheMist(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor) : base(owner)
    {
        this.health = health;
        this.aaDamage = aaDamage;
        this.spellblock = spellblock;
        this.armor = armor;
        this.attackSpeed = attackSpeed;
    }

    public override void Update()
    {
        aaTimer -= Time.deltaTime;
        if (aaTimer <= 0) AutoAttack();

        touchOfTheMaidenTimer += Time.deltaTime;
    }

    public void AutoAttack()
    {
        owner.targetCombat.TakeDamage(new Damage(aaDamage, SkillDamageType.Spell), "Maiden Of This Mist Auto Attack", true);
        aaTimer = 1f / attackSpeed;

        if (touchOfTheMaidenTimer >= 0.2f && !owner.targetStats.buffManager.buffs.ContainsKey("TouchOfTheMaiden"))
        {
            owner.targetStats.buffManager.buffs.Add("TouchOfTheMaiden", new TouchOfTheMaidenBuff(4, owner.targetStats.buffManager, "Maiden Of The Mist AA"));
            touchOfTheMaidenTimer = 0;
        }
    }
}