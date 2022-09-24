using Simulator.Combat;
using UnityEngine;

public class MistWalker : Pet
{
    public static float[] TibbersEnragedAttackSpeeds = { 0.625f, 0.739f, 1.043f, 1.307f, 1.536f, 1.736f };

    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;

    public MistWalker(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor) : base(owner)
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
    }

    public void AutoAttack()
    {
        float damage = aaDamage;
        if (owner.targetStats.buffManager.buffs.TryGetValue("MourningMistCurse", out Buff buff) && buff.value > 0)
        {
            buff.value--;
            damage += aaDamage * 0.4f;
        }

        owner.targetCombat.TakeDamage(damage, "Mist Walker Auto Attack", SkillDamageType.PhysAndSpell, true);
        aaTimer = 1f / attackSpeed;
    }
}