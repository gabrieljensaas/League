using Simulator.Combat;
using UnityEngine;

public class Tibbers : Pet
{
    public static float[] TibbersEnragedAttackSpeeds = { 0.625f, 0.739f, 1.043f, 1.307f, 1.536f, 1.736f };

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
            attackSpeed = TibbersEnragedAttackSpeeds[0]; //default value
        }
    }

    public void ExecuteFlameAuraDamage()
    {
        owner.UpdateAbilityTotalDamage(ref owner.rSum, 3, new Damage((10 + (3 / 100 * owner.myStats.AP)), SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), owner.RSkill().basic.name);
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
        owner.UpdateAbilityTotalDamage(ref owner.rSum, 3, new Damage(aaDamage, SkillDamageType.Spell, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable), owner.RSkill().basic.name);
        enrageAACounter--;
        if (enrageAACounter < 0) enrageAACounter = 0;
        attackSpeed = TibbersEnragedAttackSpeeds[enrageAACounter];
        aaTimer = 1f / attackSpeed;
    }
}