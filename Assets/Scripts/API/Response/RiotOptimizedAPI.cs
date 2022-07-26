using System.Collections.Generic;
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

/*
public class Aatrox
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
    public Stats stats { get; set; }
    public List<string> roles { get; set; }
    public AttributeRatings attributeRatings { get; set; }
    public Abilities abilities { get; set; }
    public string releaseDate { get; set; }
    public string releasePatch { get; set; }
    public string patchLastChanged { get; set; }
    public Price price { get; set; }
    public string lore { get; set; }
    public List<Skin> skins { get; set; }
}

public class Abilities
{
    public List<P> P { get; set; }
    public List<Q> Q { get; set; }
    public List<W> W { get; set; }
    public List<E> E { get; set; }
    public List<R> R { get; set; }
}

public class AcquisitionRadius
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AramDamageDealt
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AramDamageTaken
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AramHealing
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AramShielding
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Armor
{
    public string flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackCastTime
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackDamage
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackDelayOffset
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackRange
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackSpeed
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackSpeedRatio
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttackTotalTime
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class AttributeRatings
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

public class Chroma
{
    public string name { get; set; }
    public int id { get; set; }
    public string chromaPath { get; set; }
    public List<string> colors { get; set; }
    public List<Description> descriptions { get; set; }
    public List<Rarity> rarities { get; set; }
}

public class Cooldown
{
    public List<Modifier> modifiers { get; set; }
    public bool affectedByCdr { get; set; }
}

public class CriticalStrikeDamage
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class CriticalStrikeDamageModifier
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Description
{
    public string description { get; set; }
    public string region { get; set; }
}

public class E
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<Effect> effects { get; set; }
    public object cost { get; set; }
    public Cooldown cooldown { get; set; }
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

public class Effect
{
    public string description { get; set; }
    public List<object> leveling { get; set; }
}

public class GameplayRadius
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Health
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class HealthRegen
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Leveling
{
    public string attribute { get; set; }
    public List<Modifier> modifiers { get; set; }
}

public class MagicResistance
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Mana
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class ManaRegen
{
    public int flat { get; set; }
    public double percent { get; set; }
    public int perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Modifier
{
    public List<double> values { get; set; }
    public List<string> units { get; set; }
}

public class Movespeed
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class P
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<Effect> effects { get; set; }
    public object cost { get; set; }
    public Cooldown cooldown { get; set; }
    public string targeting { get; set; }
    public string affects { get; set; }
    public string spellshieldable { get; set; }
    public object resource { get; set; }
    public string damageType { get; set; }
    public string spellEffects { get; set; }
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
    public object speed { get; set; }
    public object width { get; set; }
    public object angle { get; set; }
    public object castTime { get; set; }
    public object effectRadius { get; set; }
    public object targetRange { get; set; }
}

public class PathingRadius
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class Price
{
    public int blueEssence { get; set; }
    public int rp { get; set; }
    public int saleRp { get; set; }
}

public class Q
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<Effect> effects { get; set; }
    public object cost { get; set; }
    public Cooldown cooldown { get; set; }
    public string targeting { get; set; }
    public string affects { get; set; }
    public string spellshieldable { get; set; }
    public object resource { get; set; }
    public string damageType { get; set; }
    public string spellEffects { get; set; }
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
    public object speed { get; set; }
    public object width { get; set; }
    public object angle { get; set; }
    public string castTime { get; set; }
    public object effectRadius { get; set; }
    public object targetRange { get; set; }
}

public class R
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<Effect> effects { get; set; }
    public object cost { get; set; }
    public Cooldown cooldown { get; set; }
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
    public object speed { get; set; }
    public object width { get; set; }
    public object angle { get; set; }
    public string castTime { get; set; }
    public string effectRadius { get; set; }
    public object targetRange { get; set; }
}

public class Rarity
{
    public int? rarity { get; set; }
    public string region { get; set; }
}

public class RiotOptimized
{
    //public Dictionary<string, OptimizedChampion> data { get; set; }
    //public Aatrox aatrox { get; set; }
    public Dictionary<string,Aatrox> aatrox { get; set; }
}

public class SelectionRadius
{
    public int flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class OptimizedSkin
{
    public string name { get; set; }
    public int id { get; set; }
    public bool isBase { get; set; }
    public string availability { get; set; }
    public string formatName { get; set; }
    public bool lootEligible { get; set; }
    public object cost { get; set; }
    public int sale { get; set; }
    public string distribution { get; set; }
    public string rarity { get; set; }
    public List<Chroma> chromas { get; set; }
    public string lore { get; set; }
    public string release { get; set; }
    public List<string> set { get; set; }
    public string splashPath { get; set; }
    public string uncenteredSplashPath { get; set; }
    public string tilePath { get; set; }
    public string loadScreenPath { get; set; }
    public string loadScreenVintagePath { get; set; }
    public bool newEffects { get; set; }
    public bool newAnimations { get; set; }
    public bool newRecall { get; set; }
    public bool newVoice { get; set; }
    public bool newQuotes { get; set; }
    public List<string> voiceActor { get; set; }
    public List<string> splashArtist { get; set; }
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

public class UrfDamageDealt
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class UrfDamageTaken
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class UrfHealing
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class UrfShielding
{
    public double flat { get; set; }
    public double percent { get; set; }
    public double perLevel { get; set; }
    public double percentPerLevel { get; set; }
}

public class W
{
    public string name { get; set; }
    public string icon { get; set; }
    public List<Effect> effects { get; set; }
    public object cost { get; set; }
    public Cooldown cooldown { get; set; }
    public string targeting { get; set; }
    public string affects { get; set; }
    public string spellshieldable { get; set; }
    public object resource { get; set; }
    public string damageType { get; set; }
    public string spellEffects { get; set; }
    public string projectile { get; set; }
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
    public string width { get; set; }
    public object angle { get; set; }
    public string castTime { get; set; }
    public object effectRadius { get; set; }
    public object targetRange { get; set; }
    
}
*/
