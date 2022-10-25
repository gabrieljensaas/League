using Simulator.Combat;
using UnityEngine;

public class Hallucination : Pet
{
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;
    private float deathTimer;

    public Hallucination(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor, float deathTimer) : base(owner)
    {
        this.health = health;
        this.aaDamage = aaDamage;
        this.spellblock = spellblock;
        this.armor = armor;
        this.attackSpeed = attackSpeed;
        this.deathTimer = deathTimer;
    }

    public override void Update()
    {
        aaTimer -= Time.fixedDeltaTime;
        if (aaTimer <= 0) AutoAttack();

        deathTimer -= Time.fixedDeltaTime;
        if (deathTimer <= 0) Kill();
    }

    public void AutoAttack()
    {
        owner.targetCombat.TakeDamage(new Damage(aaDamage, SkillDamageType.Phyiscal), "Shaco Clone Auto Attack");
        aaTimer = 1f / attackSpeed;
    }

    private void Kill()
    {
        owner.pets.Add(new JackInTheBox(owner, Shaco.JackInTheBoxHallucinationHP(owner.myStats.level), owner.myStats.wSkill[0].UseSkill(4, owner.wKeys[2], owner.myStats, owner.targetStats), 2, 100, 50, 5, 0));
        owner.targetStats.buffManager.buffs.Add("Flee", new FleeBuff(1, owner.targetStats.buffManager, "Shaco Clone"));
        owner.pets.Remove(this);
    }
}