using Simulator.Combat;
using UnityEngine;

public class WarriorTrickster : Pet
{
    private Wukong wukong;
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;
    private float deathTimer;
    private float damageMultiplier;
    public WarriorTrickster(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor, float deathTimer, float damageMultiplier) : base(owner)
    {
        this.health = health;
        this.aaDamage = aaDamage;
        this.spellblock = spellblock;
        this.armor = armor;
        this.attackSpeed = attackSpeed;
        this.deathTimer = deathTimer;
        this.damageMultiplier = damageMultiplier;
    }

    public override void Update()
    {
        aaTimer -= Time.fixedDeltaTime;
        if (aaTimer <= 0) AutoAttack();

        deathTimer -= Time.fixedDeltaTime;
        if (deathTimer <= 0) owner.pets.Remove(this);
    }

    public void AutoAttack()
    {
        owner.targetCombat.TakeDamage(new Damage(aaDamage * damageMultiplier, SkillDamageType.Phyiscal), "Clone Auto Attack");
        wukong.pStack++;
        aaTimer = 1f / attackSpeed;
    }
}