using Simulator.Combat;
using UnityEngine;

public class Tibbers : Pet
{
    private float spellblock;
    private float armor;
    private float attackSpeed;
    private float aaDamage;
    private float health;
    private float flameAuraCooldown = 0.25f;
    private float flameAuraTimer = 0;
    private float aaTimer;
    private float enrageTimer;
    private int enrageAACounter = 0;
    public Tibbers(ChampionCombat owner, float health, float aaDamage, float attackSpeed, float spellblock, float armor) : base(owner)
    {
        this.health = health;
        this.aaDamage = aaDamage;
        this.spellblock = spellblock;
        this.armor = armor;
        this.attackSpeed = attackSpeed;
        Enrage(3f);
    }

    public override void Update()
    {
        flameAuraTimer -= Time.deltaTime;
        aaTimer -= Time.deltaTime;
        enrageTimer -= Time.deltaTime;
        if (aaTimer <= 0) AutoAttack();
        if (flameAuraTimer <= 0) ExecuteFlameAuraDamage();
        if (enrageTimer <= 0)
        {
            enrageAACounter = 0;
            attackSpeed = Constants.TibbersEnragedAttackSpeeds[0]; //default value
        }
    }

    public void ExecuteFlameAuraDamage()
    {
        float damage = (10 + (3 / 100 * owner.myStats.AP)) * (100 / (100 + owner.targetStats.spellBlock));
        owner.targetCombat.TakeDamage(damage, "Tibber's Flame Aura", SkillDamageType.Spell);
        flameAuraTimer = flameAuraCooldown;
    }

    public void Enrage(float duration)
    {
        enrageTimer = duration;
        aaTimer *= attackSpeed / 1.736f;
        attackSpeed = 1.736f;
        enrageAACounter = 5;
    }

    public void AutoAttack()
    {
        float damage = aaDamage * (100 / (100 + owner.targetStats.spellBlock));
        owner.targetCombat.TakeDamage(damage, "Tibber's Auto Attack", SkillDamageType.Phyiscal, true);
        enrageAACounter--;
        if (enrageAACounter < 0) enrageAACounter = 0;
        attackSpeed = Constants.TibbersEnragedAttackSpeeds[enrageAACounter];
        aaTimer = 1f / attackSpeed;
    }
}