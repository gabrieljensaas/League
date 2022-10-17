using Simulator.Combat;
using UnityEngine;

public class JackInTheBox : Pet
{
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;
    private float deathTimer;
    private float activationTime;

    public JackInTheBox(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor, float deathTimer, float activationTime) : base(owner)
    {
        this.health = health;
        this.aaDamage = aaDamage;
        this.spellblock = spellblock;
        this.armor = armor;
        this.attackSpeed = attackSpeed;
        this.deathTimer = deathTimer;
        this.activationTime = activationTime;
    }

    public override void Update()
    {
        activationTime -= Time.fixedDeltaTime;

        if (activationTime <= 0)
        {
            aaTimer -= Time.fixedDeltaTime;
            if (aaTimer <= 0) AutoAttack();

            deathTimer -= Time.fixedDeltaTime;
            if (deathTimer <= 0) Kill();
        }
    }

    public void AutoAttack()
    {
        owner.targetCombat.TakeDamage(new Damage(aaDamage, SkillDamageType.Spell), "Jack In The Box Attack", true);
        aaTimer = 1f / attackSpeed;
    }

    private void Kill()
    {
        owner.targetStats.buffManager.buffs.Add("Flee", new FleeBuff(2, owner.targetStats.buffManager, "Jack In The Box"));
        owner.pets.Remove(this);
    }
}