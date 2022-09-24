using Simulator.Combat;
using UnityEngine;

public class MaidenOfTheMist : Pet
{
    public static float[] TibbersEnragedAttackSpeeds = { 0.625f, 0.739f, 1.043f, 1.307f, 1.536f, 1.736f };

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
        owner.targetCombat.TakeDamage(aaDamage, "Maiden Of This Mist Auto Attack", SkillDamageType.Spell, true);
        aaTimer = 1f / attackSpeed;

        if (touchOfTheMaidenTimer >= 0.2f && !owner.targetStats.buffManager.buffs.ContainsKey("TouchOfTheMaiden"))
        {
            owner.targetStats.buffManager.buffs.Add("TouchOfTheMaiden", new TouchOfTheMaidenBuff(4, owner.targetStats.buffManager, "Maiden Of The Mist AA"));
            touchOfTheMaidenTimer = 0;
        }
    }
}