using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Stats")]
public class StatsList : ScriptableObject
{
    public Health health;
    public HealthRegen healthRegen;
    public Mana mana;
    public ManaRegen manaRegen;
    public Armor armor;
    public MagicResistance magicResistance;
    public AttackDamage attackDamage;
    public Movespeed movespeed;
    public AcquisitionRadius acquisitionRadius;
    public SelectionRadius selectionRadius;
    public PathingRadius pathingRadius;
    public GameplayRadius gameplayRadius;
    public CriticalStrikeDamage criticalStrikeDamage;
    public CriticalStrikeDamageModifier criticalStrikeDamageModifier;
    public AttackSpeed attackSpeed;
    public AttackSpeedRatio attackSpeedRatio;
    public AttackCastTime attackCastTime;
    public AttackTotalTime attackTotalTime;
    public AttackDelayOffset attackDelayOffset;
    public AttackRange attackRange;
    public AramDamageTaken aramDamageTaken;
    public AramDamageDealt aramDamageDealt;
    public AramHealing aramHealing;
    public AramShielding aramShielding;
    public UrfDamageTaken urfDamageTaken;
    public UrfDamageDealt urfDamageDealt;
    public UrfHealing urfHealing;
    public UrfShielding urfShielding;
}

[System.Serializable]
public class Health
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class HealthRegen
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class Mana
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class ManaRegen
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class Armor
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class MagicResistance
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackDamage
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class MoveSpeed
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AcquisitionRadius
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class SelectionRadius
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class PathingRadius
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class GameplayRadius
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class CriticalStrikeDamage
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class CriticalStrikeDamageModifier
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackSpeed
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackSpeedRatio
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackCastTime
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackTotalTime
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackDelayOffset
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AttackRange
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AramDamageTaken
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AramDamageDealt
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AramHealing
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class AramShielding
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class UrfDamageTaken
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class UrfDamageDealt
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class UrfHealing
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}

[System.Serializable]
public class UrfShielding
{
    public double flat;
    public double percent;
    public double perLevel;
    public double percentPerLevel;
}