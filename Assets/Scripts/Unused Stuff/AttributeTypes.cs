using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTypes : MonoBehaviour
{
	public static AttributeTypes singleton;
	
	void Start()
	{
		if (singleton == null)
		{
			singleton = this;
		}
		else
		{
			Destroy(this);
			return;
		}
	}
	public class FirstCastDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];

	}

	public class FirstSweetspotDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class SecondCastDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class SecondSweetspotDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class ThirdCastDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class ThirdSweetspotDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class PhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentMaxHP = new float[5];
		public float[] percentArmor = new float[5];
		public float[] percentOfDamageDealt = new float[5];
		public float[] percentBonusArmor = new float[5];
		public float[] percentBonusMagicRes = new float[5];
		public float[] percentOwnBonusHP = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentTargetCurrentHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
		public float[] percentTargetBonusHP = new float[5];
		public float[] percentCritChance = new float[5];
		//Viego unit problem 
	}

	public class MinionDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class TotalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentOwnMaxHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
	}

	public class Healing
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percent = new float[5];
		public float[] percentAP = new float[5];
		public float[] per1Lithality = new float[5];
	}

	public class WorldEnderIncreasedHealing
	{
		public float[] percent = new float[5];
	}

	public class BonusMovementSpeed
	{
		public float[] flat = new float[5];
		public float[] percent = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class BonusAD
	{
		public float[] flat = new float[5];
		public float[] AD = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentAP = new float[5];
	}

	public class IncreasedHealing
	{
		public float[] percent = new float[5];
	}

	public class DamagePerPass
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TotalMixedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMaxHP = new float[5];
	}

	public class MagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentOwnMaxHP = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentCurrentHP = new float[5];
		public float[] percentCurrentMaxHP = new float[5];
		public float[] percentPer100AP = new float[5];
		public float[] percentMaxMana = new float[5];
		public float[] percentBonusMana = new float[5];
		public float[] percentArmor = new float[5];
		public float[] percentBonusArmor = new float[5];
	}

	public class AdditionalMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TotalSingleTargetDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];

	}

	public class DisableDuration
	{
		public float[] flat = new float[5];
	}

	public class ShroudDuration
	{
		public float[] flat = new float[5];		
	}

	public class TotalMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMissingHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentOwnMaxHP = new float[5];
		public float[] percentPer100AP = new float[5];


	}

	public class MinimumMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentMaxHP = new float[5];

	}

	public class MaximumMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentMaxMana = new float[5];
		public float[] percentMaxHP = new float[5];
	}

	public class TotalPhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] targetMaxHP = new float[5];

	}

	public class NonChampionDamage
	{
		public float[] percent = new float[5];
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class PhysicalDamageperShot
	{
		// Akshan x unit
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] per100PercentBonusAS = new float[5];

	}

	public class MaximumBulletsStored
	{
		public float[] flat = new float[5];
	}

	public class MinimumPhysicalDamageperBullet
	{
		// Akshan x unit
		public float[] percentAD = new float[5];
		public float[] per100PercentCriticalStrikeChance = new float[5];
	}

	public class MaximumPhysicalDamageperBullet
	{
		// Akshan x unit
		public float[] percentAD = new float[5];
		public float[] per100PercentCriticalStrikeChance = new float[5];
	}

	public class MinimumChargedPhysicalDamage
	{
		// Akshan x unit
		public float[] percentAD = new float[5];
		public float[] per100PercentCriticalStrikeChance = new float[5];
	}

	public class Damagetotargetwithmissinghp
	{
		// Akshan x unit
		public float[] percentAD = new float[5];
		public float[] per100PercentCriticalStrikeChance = new float[5];
	}

	public class MagicDamagePerTick
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] per100AP = new float[5];
	}

	public class DamageReduction
	{
		public float[] percent = new float[5];
		public float[] per100AP = new float[5];
	}

	public class PhysicalDamageReduction
	{
		public float[] percent = new float[5];
		public float[] flat = new float[5];
		public float[] percentBonusArmor = new float[5];
		public float[] percentBonusMagicRs = new float[5];
		public float[] percentPer100AP = new float[5];
		public float[] percentPer100BonusMagicRs = new float[5];
	}

	public class Passthroughdamage
    {
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class StunDuration
	{
		public float[] flat = new float[5];
	}

	public class Width
	{
		public float[] flat = new float[5];
		public float[] units = new float[5];
		public float[] soldiers = new float[5];
	}

	public class Numberoficesegments
	{
		public float[] chunksOfIce = new float[5];
	}

	public class Distancebetweenoutermostsegments
	{
		public float[] units = new float[5];
	}

	public class Distancebetweenindividualsegments
	{
		public float[] units = new float[5];
	}

	public class EnhancedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class Slow
	{
		public float[] percent = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class EmpoweredDamageperTick
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class EmpoweredSlow
	{
		public float[] percent = new float[5];
	}

	public class Shield
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentMissingHP = new float[5];
		public float[] percentMaxHP = new float[5];
		public float[] perMistCollected = new float[5];

	}

	public class InitialMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class BonusAttackDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentAnchorsBonusAD = new float[5];
	}

	public class BonusAttackSpeed
	{
		public float[] percent = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class Lethality
	{
		public float[] flat = new float[5];
	}

	public class DamagePerArrow
	{
		public float[] percentAD = new float[5];
	}

	public class TotalDamagePerFlurry
	{
		public float[] percentAD = new float[5];
	}

	public class Arrows
	{
		public float[] flat = new float[5];
	}

	public class SecondaryMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class MinimumStunDuration
	{
		public float[] flat = new float[5];
	}

	public class MaximumStunDuration
	{
		public float[] flat = new float[5];
	}

	public class BonusDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class StaticMovementSpeed
	{
		public float[] flat = new float[5];
	}

	public class IncreasedAttackSpeed
	{
		public float[] percent = new float[5];
	}

	public class ShieldStrength
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentBonusHP = new float[5];
		public float[] percentMaxHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] perSoulCollected = new float[5];
	}

	public class MinimumHeal
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class MaximumHeal
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class MonsterDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetCurrentHP = new float[5];
	}

	public class ModifiedMinionDamage
	{
		public float[] percent = new float[5];
	}

	public class SlowDuration
	{
		public float[] flat = new float[5];
	}

	public class MinimumPhysicalDamageperhit
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class MaximumPhysicalDamageperhit
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class MinimumMonsterDamageperhit
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class MaximumMonsterDamageperhit
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class BonusTrueDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMaxHP = new float[5];
	}

	public class MaximumMonsterDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class TrueDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMissingHP = new float[5];

	}

	public class BonusHealth
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class IncreasedTotalAttackSpeed
	{
		public float[] percent = new float[5];
	}

	public class Heal
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentHPLost = new float[5];
		public float[] percentMissingHP = new float[5];
		public float[] percentOwnMissingHP = new float[5];
	}

	public class AttackSpeed
	{
		public float[] percent = new float[5];
	}

	public class MovementSpeed
	{
		public float[] percent = new float[5];
	}

	public class BoltMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class DetonationMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class IncreasedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentOwnBonusHP = new float[5];
	}

	public class AllyBonusArmor
	{
		public float[] flat = new float[5];
		public float[] percentBonusArmor = new float[5];
	}

	public class AllyBonusMagicResistance
	{
		public float[] flat = new float[5];
		public float[] percentBonusMagicRs = new float[5];
	}

	public class SelfBonusArmor
	{
		public float[] flat = new float[5];
		public float[] percentBonusArmor = new float[5];
	}

	public class SelfBonusMagicResistance
	{
		public float[] flat = new float[5];
		public float[] percentBonusMagicRs = new float[5];
	}

	public class Duration
	{
		public float[] flat = new float[5];
	}

	public class MaximumKnockupDuration
	{
		public float[] flat = new float[5];
	}

	public class ReducedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];

	}

	public class TrapDuration
	{
		public float[] flat = new float[5];
	}

	public class MaximumTraps
	{
		public float[] flat = new float[5];
	}

	public class HeadshotDamageIncrease
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class BonusPhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentArmor = new float[5];
		public float[] percentTargetCurrentHP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AD = new float[5];
		public float[] percentPerBonus100AD = new float[5];
		public float[] siphoningStrikeStacks = new float[5];
	}

	public class IncreasedMixedDamage
	{
		public float[] percentAD = new float[5];
	}

	public class OuterConeBonusDamage
	{
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AD = new float[5];
	}

	public class NonEpicMonsterDamage
	{
		public float[] flat = new float[5];		
		public float[] percentBonusAD = new float[5];
	}

	public class BonusNonEpicMonsterDamage
	{
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AD = new float[5];
	}

	public class ZoneDuration
	{
		public float[] flat = new float[5];
	}

	public class BonusMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentTargetCurrentHP = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class ReducedHealing
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class SilenceDuration
	{
		public float[] flat = new float[5];

	}

	public class ChampionTrueDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class NonChampionTrueDamage
	{
		public float[] bonusHPPerStack = new float[5];
	}

	public class BonusHealthPerStack
	{
		public float[] bonusHealth = new float[5];
	}

	public class BonusAttackRangePerStack
	{
		public float[] flat = new float[5];
	}

	public class BonusSizePerStack
	{
		public float[] percent = new float[5];
	}

	public class MixedDamagePerTick
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class ResistanceReductionPerTick
	{
		public float[] flat = new float[5];
	}

	public class TotalResistanceReduction
	{
		public float[] flat = new float[5];
	}

	public class BigOneMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
	}

	public class BladePhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class HandlePhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class ArmorPenetration
	{
		public float[] percent = new float[5];
	}

	public class BonusDamagePerStack
	{
		public float[] flat = new float[5];
		public float[] percentAp = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentMaxMana = new float[5];
	}

	public class MaximumTrueDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class TotalShieldStrength
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class MagicDamageperOrb
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class BonusDamagePerChampion
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TotalDamageVsChampions
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class MinimumDamage
	{
		public float[] flat = new float[5];		
	}

	public class CappedMonsterDamage
	{
		public float[] flat = new float[5];
	}

	public class DamageStored
	{
		public float[] percentDmgTaken = new float[5];
		public float[] percentDmgDealt = new float[5];
	}

	public class MaximumAdditionalBonusAD
	{
		public float[] flat = new float[5];
	}

	public class MaximumTotalBonusAD
	{
		public float[] flat = new float[5];
	}

	public class MinimumBonusPhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class MaximumBonusPhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class MinimumNonChampionBonusDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class MaximumNonChampionBonusDamage
	{
		public float[] flat = new float[5];
		public float[] percentBonusHP = new float[5];
	}

	public class IncreasedBaseHealth
	{		
		public float[] percentMissingHP = new float[5];
	}

	public class TotalHealthRegeneration
	{
		public float[] percentMaxHP = new float[5];
	}

	public class RegenerationperSecond
	{
		public float[] percentMaxHP = new float[5];
	}

	public class MinimumPhysicalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentPer100BonusAD = new float[5];
		public float[] percentTargetMissingHP = new float[5];
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPrimaryTargetBonusHP = new float[5];
	}

	public class MinimumTotalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class OutwardMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class ReturningMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class MaxMonsterTotalDamage
	{
		public float[] flat = new float[5];
	}

	public class DartDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TotalBonusDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentTargetMaxHP = new float[5];

	}

	public class SpikeDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class MaximumDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentAD = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class CharmDuration
	{
		public float[] flat = new float[5];
	}

	public class MonsterDuration
	{
		public float[] flat = new float[5];
	}

	public class MagicResistanceReduction
	{
		public float[] percent = new float[5];
	}

	public class EmpoweredDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetCurrentHP = new float[5];

	}

	public class FearDuration
	{
		public float[] flat = new float[5];
	}

	public class IncreasedMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
		public float[] percentPer100AP = new float[5];
		public float[] percentTargetCurrentHP = new float[5];
	}

	public class IncreasedMinimumDamage
	{
		public float[] flat = new float[5];
	}

	public class LastTickofDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
	}

	public class ChampionHealPercentage
	{
		public float[] percent = new float[5];
	}

	public class TotalHealperChampion
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
	}

	public class TotalHealperMonster
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
	}

	public class TotalHealperMinion
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentTargetMissingHP = new float[5];
	}

	public class CriticalDamage
	{
		public float[] flat = new float[5];
		public float[] percent = new float[5];
		public float[] percentAD = new float[5];
	}

	public class AdditionalBonusMovementSpeed
	{
		public float[] percent = new float[5];
	}

	public class HealperTick
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class ManaRefunded
	{
		public float[] flat = new float[5];
	}

	public class BonusOnHitDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class GuppyDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class ChomperDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class GigalodonDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class GustMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TornadoMagicDamagePerTick
	{
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class TotalTornadoMagicDamage
	{
		public float[] percentTargetMaxHP = new float[5];
		public float[] percentPer100AP = new float[5];
	}

	public class MagicDamageShield
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentMaxHP = new float[5];		
	}

	public class MagicDamageReduction
	{
		public float[] percentPer100AP = new float[5];
		public float[] percentPer100BonusMagicRS = new float[5];
	}

	public class ChampionMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class NonChampionMagicDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class bug
	{

	}

	public class GoldPlunder
	{
		public float[] flat = new float[5];
	}

	public class SilverSerpentPlunder
	{
		public float[] flat = new float[5];
	}

	public class Maximumcharges
	{
		public float[] flat = new float[5];
	}

	public class ChampionBonusDamage
	{

	}

	public class MagicDamagePerWave
	{

	}

	public class MagicDamagePerCluster
	{

	}

	public class TrueDamagewithDeathsDaughter
	{

	}

	public class TotalMixedDamagewithDeathsDaughter
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
	}

	public class TotalMagicDamagewithFireatWill
	{

	}

	public class MaximumMixedTotalDamagewithFireatWillandDeathsDaughter
	{

	}

	public class MovementSpeedDuration
	{

	}

	public class PhysicalDamagePerSpin
	{

	}

	public class IncreasedDamagePerSpin
	{

	}

	public class HyperMovementSpeed
	{

	}

	public class BonusMovespeed
	{

	}

	public class TotalMovespeed
	{

	}

	public class BonusRange
	{

	}

	public class MaximumMinionDamage
	{

	}

	public class MaximumSlow
	{

	}

	public class MaximumDamagetoMonsters
	{

	}

	public class InitialPhysicalDamage
	{

	}

	public class DetonationDamage
	{

	}

	public class BonusArmor
	{

	}

	public class MaximumArmor
	{

	}

	public class ExplosionDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class DamageperSnip
	{

	}

	public class CenterDamageperSnip
	{

	}

	public class FinalSnipDamage
	{

	}

	public class FinalSnipCenterDamage
	{

	}

	public class MinimumCenterDamage
	{

	}

	public class MaximumCenterDamage
	{

	}

	public class BonusResistances
	{

	}

	public class CooldownRefund
	{

	}

	public class MagicDamageperNeedle
	{

	}

	public class DamagewithThousandCuts
	{

	}

	public class ReducedSlow
	{

	}

	public class SecondCastTotalDamage
	{

	}

	public class ThirdCastTotalDamage
	{

	}

	public class MaximumTotalDamage
	{

	}

	public class Miniondamage
	{

	}

	public class CappedHealing
	{

	}

	public class MaximumPhysicalDamage
	{

	}

	public class InitialRocketMagicDamage
	{

	}

	public class AdditionalDamage
	{

	}

	public class AdditionalMinionDamage
	{

	}

	public class TotalMinionDamage
	{

	}

	public class DamageIncrease
	{

	}

	public class DamageTransmission
	{

	}

	public class CooldownReduction
	{

	}

	public class BarrageDamage
	{

	}

	public class PerimeterDamage
	{

	}

	public class RootDuration
	{

	}

	public class BonusMagicDamagePerSecond
	{

	}

	public class HealPerTick
	{

	}

	public class TotalHeal
	{

	}

	public class ArmorReduction
	{

	}

	public class BaseShield
	{

	}

	public class BonusMagicResistance
	{

	}

	public class ManaRestore
	{

	}

	public class MaximumAttackSpeed
	{

	}

	public class MaximumSecondaryDamage
	{

	}

	public class MinimumSecondaryDamage
	{

	}

	public class PhysicalDamagePerMissile
	{

	}

	public class ReducedDamagePerMissile
	{

	}

	public class TotalEvolvedSingleTargetDamage
	{

	}

	public class MinimumMovementSpeed
	{

	}

	public class MaximumMovementSpeed
	{

	}

	public class MaximumNonChampionDamage
	{

	}

	public class DamageperAdditionalSpear
	{

	}

	public class KnockupDuration
	{

	}

	public class EnhancedMonsterDamage
	{

	}

	public class WallLength
	{

	}

	public class ManaRestored
	{

	}

	public class DamagePerSecond
	{

	}

	public class MagicShield
	{

	}

	public class IncreasedBonusMagicDamage
	{

	}

	public class ManaRestoredAgainstChampions
	{

	}

	public class BonusMovementspeed
	{

	}

	public class PhysicalDamagePerDagger
	{

	}

	public class MaximumSingleTargetPhysicalDamage
	{

	}

	public class MagicDamagePerDagger
	{

	}

	public class MaximumSingleTargetMagicDamage
	{

	}

	public class OnAttackOROnHitEffectiveness
	{

	}

	public class PassiveDamage
	{

	}

	public class InvulnerabilityDuration
	{

	}

	public class TotalNonChampionDamage
	{

	}

	public class CappedMonsterDamageperHit
	{

	}

	public class TotalCappedMonsterDamage
	{

	}

	public class MagicDamagePerBolt
	{

	}

	public class IsolationPhysicalDamage
	{

	}

	public class StaticCooldown
	{

	}

	public class AdditionalPhysicalDamage
	{

	}

	public class Enhanceddamagebelowthreshold
	{

	}

	public class TetherDamage
	{

	}

	public class MinionandSmallMonsterDamage
	{

	}

	public class PullDamage
	{

	}

	public class MaximumDamageAgainstMonsters
	{

	}

	public class MinimumShield
	{

	}

	public class MaximumShield
	{

	}

	public class ResistancesReduction
	{

	}

	public class BonusAttackRange
	{

	}

	public class DelayedDamage
	{

	}

	public class MarkDamage
	{

	}

	public class RootDamage
	{

	}

	public class CollisionDamage
	{

	}

	public class FlatDamageReduction
	{

	}

	public class MaximumBonusMovementSpeed
	{

	}

	public class IncreasedMinionDamage
	{

	}

	public class SleepDuration
	{

	}

	public class MinimumSelfHeal
	{

	}

	public class MaximumSelfHeal
	{

	}

	public class PhysicalDamagePerShot
	{

	}

	public class MinionDamagePerShot
	{

	}

	public class IncreasedArmor
	{

	}

	public class Cripple
	{

	}

	public class VoidlingDuration
	{

	}

	public class DamagePerTick
	{

	}

	public class TotalIncreasedDamage
	{

	}

	public class IncreasedDamagePerTick
	{

	}

	public class ReducedDamageperhit
	{

	}

	public class MaximumSingleTargetDamage
	{

	}

	public class MonsterBonusDamage
	{

	}

	public class MonsterTotalDamage
	{

	}

	public class ReducedMonsterDamageperhit
	{

	}

	public class MaxMonsterSingleTargetDamage
	{

	}

	public class MinimumHealingPerHalfSecond
	{

	}

	public class MaximumHealingPerHalfSecond
	{

	}

	public class MinimumTotalHealing
	{

	}

	public class MaximumTotalHealing
	{

	}

	public class TurretDamageReduction
	{

	}

	public class IncreasedBonusMovementSpeed
	{

	}

	public class TotalWaves
	{

	}

	public class MaximumTotalPhysicalDamage
	{

	}

	public class WaveIntervalTime
	{

	}

	public class CloneDamage
	{

	}

	public class ShieldtoHealing
	{

	}

	public class MagicPenetration
	{

	}

	public class MinimumDamagePerTick
	{

	}

	public class MaximumDamagePerTick
	{

	}

	public class BonusMagicDamagePerHit
	{

	}

	public class TotalBonusMagicDamage
	{

	}

	public class AdditionalSlowPerSecond
	{

	}

	public class MaximumCripple
	{

	}

	public class AdditionalCripplePerSecond
	{

	}

	public class KnockUpDuration
	{

	}

	public class AdditionalBloomDamage
	{

	}

	public class TotalMaximumDamage
	{

	}

	public class PassiveMovementSpeed
	{

	}

	public class ActiveMovementSpeed
	{

	}

	public class EmpoweredRootDuration
	{

	}

	public class BonusShieldPerChampion
	{

	}

	public class TotalShieldvs5Champions
	{

	}

	public class WhirlTotalPhysicalDamage
	{

	}

	public class PhysicalDamageperTick
	{

	}

	public class BurstPhysicalDamage
	{

	}

	public class EnhancedBonusAttackSpeed
	{

	}

	public class BaseHealingfromNonChampions
	{

	}

	public class EmpoweredHealingfromNonChampions
	{

	}

	public class ChampionDamage
	{

	}

	public class BaseHealingfromChampions
	{

	}

	public class EmpoweredHealingfromChampions
	{

	}

	public class MinimumExplosionDamage
	{

	}

	public class MaximumExplosionDamage
	{

	}

	public class MinimumRolloverDamage
	{

	}

	public class MaximumRolloverDamage
	{

	}

	public class DamagePerSnowball
	{

	}

	public class DamagePerVolley
	{

	}

	public class MovementSpeedModifier
	{

	}

	public class TotalMinimumMinionDamage
	{

	}

	public class MinimumMinionDamagePerInstance
	{

	}

	public class TotalMonsterDamage
	{

	}

	public class MonsterDamagePerInstance
	{

	}

	public class SecondaryPhysicalDamage
	{

	}

	public class IncreasedSecondaryDamage
	{

	}

	public class SlamDamage
	{

	}

	public class MaximumInitialMonsterDamage
	{

	}

	public class UnchargedPhysicalDamage
	{

	}

	public class Cappedpercentagemonsterdamage
	{

	}

	public class TotalMovementSpeedIncrease
	{

	}

	public class TauntDuration
	{

	}

	public class BonusAttackSpeedDuration
	{

	}

	public class ImpactMagicDamage
	{

	}

	public class CenterMinimumDamage
	{

	}

	public class CenterMaximumDamage
	{

	}

	public class AftershockMagicDamage
	{

	}

	public class MaximumAftershockDamage
	{

	}

	public class IncreasedTotalDamage
	{

	}

	public class MaximumTurretDamage
	{

	}

	public class TotalTurretDamage
	{

	}

	public class ImpactTurretDamage
	{

	}

	public class AftershockTurretDamage
	{

	}

	public class TotalAftershockTurretDamage
	{

	}

	public class SecondaryDamage
	{

	}

	public class HealPerChampion
	{

	}

	public class TotalMagicDamagePerTarget
	{

	}

	public class MaximumBonusAttackSpeed
	{

	}

	public class BerserkDuration
	{

	}

	public class EnhancedPhysicalDamage
	{

	}

	public class HealingCap
	{

	}

	public class EnhancedHealingCap
	{

	}

	public class NonChampionHealing
	{

	}

	public class EnhancedNonChampionHealing
	{

	}

	public class ChampionHealing
	{

	}

	public class EnhancedChampionHealing
	{

	}

	public class PhysicalDamagePerHit
	{

	}

	public class EmpoweredBonusPhysicalDamage
	{

	}

	public class EmpoweredMagicDamage
	{

	}

	public class EmpoweredPhysicalDamage
	{

	}

	public class MinionDamagePercentage
	{

	}

	public class MinionDamagePerTick
	{

	}

	public class TotalEnhancedDamage
	{

	}

	public class EnhancedDamagePerTick
	{

	}

	public class TotalEnhancedMinionDamage
	{

	}

	public class EnhancedShieldStrength
	{

	}

	public class EnhancedBonusMovementSpeed
	{

	}

	public class TotalSlow
	{

	}

	public class EnhancedMagicDamage
	{

	}

	public class TotalEnhancedMagicDamage
	{

	}

	public class EnhancedSlow
	{

	}

	public class TotalEnhancedSlow
	{

	}

	public class MinimumMagicdamage
	{

	}

	public class BonusOverloadDamage
	{

	}

	public class SlashDamage
	{

	}

	public class TotalDamagePerTarget
	{

	}

	public class TotalDamagePerMinion
	{

	}

	public class SwingDamage
	{

	}

	public class LashDamage
	{

	}

	public class EffectDuration
	{

	}

	public class HealPerAlly
	{

	}

	public class Damage
	{

	}

	public class InvisibilityDuration
	{

	}

	public class ChampionDisableDuration
	{

	}

	public class MiniBoxMagicDamage
	{

	}

	public class MiniBoxIncreasedDamage
	{

	}

	public class IncreasedBonusDamage
	{

	}

	public class BonusMovementSpeedDecay
	{

	}

	public class FuryGenerationperSecond
	{

	}

	public class SizeIncrease
	{

	}

	public class SlowStrength
	{

	}

	public class BonusStats
	{

	}

	public class TotalRegeneration
	{

	}

	public class MaxBaseDamageIncrease
	{

	}

	public class MinimumMonsterDamage
	{

	}

	public class MinimumMinionDamage
	{

	}

	public class ChampionMaximumDamage
	{

	}

	public class BounceDamage
	{

	}

	public class MinionBounceDamage
	{

	}

	public class BuffDuration
	{

	}

	public class InitialBonusMovementSpeed
	{

	}

	public class MinimumDamageBlocked
	{

	}

	public class AuraBonusMovementSpeed
	{

	}

	public class HealthCostReduction
	{

	}

	public class ReducedHealthCost
	{

	}

	public class EnhancedHeal
	{

	}

	public class BonusDamagePerAdditionalBolt
	{

	}

	public class RevealDuration
	{

	}

	public class NonChampionDetonationDamage
	{

	}

	public class MaximumChampionDamage
	{

	}

	public class MagicDamageperSphere
	{

	}

	public class DamageStoredintoGreyHealth
	{

	}

	public class IncreasedDamageStoredintoGreyHealth
	{

	}

	public class PrimaryTargetDamage
	{

	}

	public class SecondaryTargetDamage
	{

	}

	public class MinimumDetonationDamage
	{

	}

	public class MaximumDetonationDamage
	{

	}

	public class ReturnPhysicalDamage
	{

	}

	public class MaximumCharges
	{

	}

	public class BlindDuration
	{

	}

	public class IncreasedBlindDuration
	{

	}

	public class PassiveBonusMovementSpeed
	{

	}

	public class ActiveBonusMovementSpeed
	{

	}

	public class MagicDamageOnHit
	{

	}

	public class DamageperTick
	{

	}

	public class TotalDoTDamage
	{

	}

	public class MonsterDamageOnHit
	{

	}

	public class MonsterDamageperTick
	{

	}

	public class TotalDoTMonsterDamage
	{

	}

	public class BounceRange
	{

	}

	public class MinimumBonusMagicDamage
	{

	}

	public class MaximumBonusMagicDamage
	{

	}

	public class KnockbackDistance
	{

	}

	public class AttackDamageReduction
	{

	}

	public class MagicDamagePerSecond
	{

	}

	public class BonusADPerMissingHealth
	{

	}

	public class MaximumBonusAD
	{

	}

	public class HealPerFury
	{

	}

	public class ADReduction
	{

	}

	public class FuryGained
	{

	}

	public class MinimumHealthThreshold
	{

	}

	public class StealthDuration
	{

	}

	public class BasePhysicalDamage
	{

	}

	public class PhysicalDamagePerStack
	{

	}

	public class MinimumMixedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class MaximumMixedDamage
	{
		public float[] flat = new float[5];
		public float[] percentAP = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class TotalBonusPhysicalDamage
	{

	}

	public class BonusPhysicalDamageperTick
	{

	}

	public class ConeDamage
	{

	}

	public class ModifiedPhysicalDamage
	{

	}

	public class MinimumFullyReducedDamage
	{

	}

	public class MaximumFullyReducedDamage
	{

	}

	public class BonusMagicDamageperStack
	{

	}

	public class MinimumTrueDamage
	{

	}

	public class TumbleCooldownReduction
	{

	}

	public class ManaRestoreperKill
	{

	}

	public class MaximumManaRestored
	{

	}

	public class MinimumBonusDamage
	{

	}

	public class PhysicalDamagetoMonsters
	{

	}

	public class DischargeDamage
	{

	}

	public class ReducedHeal
	{

	}

	public class IncreasedMovementSpeed
	{

	}

	public class MinionHeal
	{

	}

	public class TurretDisableDuration
	{

	}

	public class CapAgainstMonsters
	{

	}

	public class HealingPercentage
	{

	}

	public class DamagePerBlade
	{

	}

	public class MinimumDamagePerBlade
	{

	}

	public class PhysicalDamagePerFeather
	{

	}

	public class MinionDamagePerFeather
	{

	}

	public class NumberofRecasts
	{

	}

	public class Distance
	{

	}

	public class ThrustDamage
	{

	}

	public class WallWidth
	{

	}

	public class MaximumBonusDamage
	{

	}

	public class WallHealth
	{

	}

	public class MistWalkers
	{

	}

	public class BonusAbilityPower
	{

	}

	public class AdaptiveForce
	{

	}

	public class AdaptiveForceper100bonusAD
	{

	}

	public class AdaptiveForceper100AP
	{

	}

	public class ReducedDamagePerWave
	{

	}

	public class CappedDamage
	{

	}

	public class MaximumRangeChannelDuration
	{

	}

	public class ChunkHealing
	{

	}

	public class EnergyRestored
	{

	}

	public class PhysicalDamageperBullet
	{

	}

	public class PierceDamage
	{

	}

	public class DemolitionThreshold
	{

	}

	public class MagicDamageperMine
	{

	}

	public class ReducedDamageperMine
	{

	}

	public class BonusMovementSpeedDuration
	{

	}

	
}