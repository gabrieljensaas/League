using Simulator.Combat;
using UnityEngine;

public class Daisy : Pet
{
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float aaTimer;
    private int daisySmashStack;
    private float timeSinceDaisySmash;
    public Daisy(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor) : base(owner)
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
        timeSinceDaisySmash += Time.deltaTime;
        
    }

    public void AutoAttack()
    {
        if(timeSinceDaisySmash > 3 && daisySmashStack == 2)
		{
            // need to apply emowered charge before stunning and dealing damage
            owner.targetCombat.TakeDamage(new Damage(aaDamage, SkillDamageType.Phyiscal), "Daisy's Auto Attack", true);
            owner.TargetBuffManager.Add("StunBuff", new StunBuff(1f, owner.TargetBuffManager, "Daisy's Stun"));
            owner.TargetBuffManager.Add("KnockOff", new AirborneBuff(1f, owner.TargetBuffManager, "Daisy's Airborne"));
		}
        else
            owner.targetCombat.TakeDamage(new Damage(aaDamage, SkillDamageType.Phyiscal), "Daisy's Auto Attack", true);

        aaTimer = 1f / attackSpeed;
    }
}