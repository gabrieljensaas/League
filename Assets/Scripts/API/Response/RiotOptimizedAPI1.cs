using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class OptimizedChampion
{
    public int id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string fullName { get; set; }
    public string icon { get; set; }
    public string resource { get; set; }
    public string attackType { get; set; }
    public string adaptiveType { get; set; }
    public List<string> roles { get; set; }
    public OptimizedAttributeRatings attributeRatings { get; set; }
    public Dictionary<string, OptimizedAbilities> abilities { get; set; }

    public Stats stats { get; set; }
}

public class OptimizedStats
{
    public Health health { get; set; }
    public HealthRegen healthRegen { get; set; }
    public Mana mana { get; set; }
    public ManaRegen manaRegen { get; set; }
    public Armor armor { get; set; }
    public MagicResistance magicResistance { get; set; }
    public AttackDamage attackDamage { get; set; }
    public Movespeed movespeed { get; set; }
    public AcquisitionRadius acquisitionRadius { get; set; }
    public SelectionRadius selectionRadius { get; set; }
    public PathingRadius pathingRadius { get; set; }
    public GameplayRadius gameplayRadius { get; set; }
    public CriticalStrikeDamage criticalStrikeDamage { get; set; }
    public CriticalStrikeDamageModifier criticalStrikeDamageModifier { get; set; }
    public AttackSpeed attackSpeed { get; set; }
    public AttackSpeedRatio attackSpeedRatio { get; set; }
    public AttackCastTime attackCastTime { get; set; }
    public AttackTotalTime attackTotalTime { get; set; }
    public AttackDelayOffset attackDelayOffset { get; set; }
    public AttackRange attackRange { get; set; }
    public AramDamageTaken aramDamageTaken { get; set; }
    public AramDamageDealt aramDamageDealt { get; set; }
    public AramHealing aramHealing { get; set; }
    public AramShielding aramShielding { get; set; }
    public UrfDamageTaken urfDamageTaken { get; set; }
    public UrfDamageDealt urfDamageDealt { get; set; }
    public UrfHealing urfHealing { get; set; }
    public UrfShielding urfShielding { get; set; }
}

public class OptimizedAttributeRatings
{
    public int damage { get; set; }
    public int toughness { get; set; }
    public int control { get; set; }
    public int mobility { get; set; }
    public int utility { get; set; }
    public int abilityReliance { get; set; }
    public int attack { get; set; }
    public int defense { get; set; }
    public int magic { get; set; }
    public int difficulty { get; set; }
}

public class OptimizedCooldown
{
    public List<Modifier> modifiers { get; set; }
    public bool affectedByCdr { get; set; }
}

public class OptimizedAbilities
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<OptimizedEffect> effects { get; set; }
    public object cost { get; set; }
    public OptimizedCooldown cooldown { get; set; }
    public string targeting { get; set; }
    public string affects { get; set; }
    public object spellshieldable { get; set; }
    public object resource { get; set; }
    public object damageType { get; set; }
    public object spellEffects { get; set; }
    public object projectile { get; set; }
    public object onHitEffects { get; set; }
    public object occurrence { get; set; }
    public string notes { get; set; }
    public string blurb { get; set; }
    public object missileSpeed { get; set; }
    public object rechargeRate { get; set; }
    public object collisionRadius { get; set; }
    public object tetherRadius { get; set; }
    public object onTargetCdStatic { get; set; }
    public object innerRadius { get; set; }
    public string speed { get; set; }
    public object width { get; set; }
    public object angle { get; set; }
    public string castTime { get; set; }
    public object effectRadius { get; set; }
    public string targetRange { get; set; }
}

public class OptimizedEffect
{
    public string description { get; set; }
    public List<object> leveling { get; set; }
}

public class OptimizedLeveling
{
    public string attribute { get; set; }
    public List<OptimizedModifier> modifiers { get; set; }
}

public class OptimizedModifier
{
    public List<double> values { get; set; }
    public List<string> units { get; set; }
}

public class RiotOptimized
{
    public Dictionary<string, OptimizedChampion> champion { get; set; }
}
*/