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
		public float[] percentAD = new float[5];
	}

	public class TotalDamage
	{
		public float[] flat = new float[5];
		public float[] percentAD = new float[5];
	}

	public class Healing
	{
		public float[] percent = new float[5];
	}

	public class WorldEnderIncreasedHealing
	{
		public float[] percent = new float[5];
	}

	public class BonusMovementSpeed
	{
		public float[] percent = new float[5];
	}

	public class BonusAD
	{
		public float[] percentAD = new float[5];
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
	}

	public class DisableDuration
	{
		public float[] flat = new float[5];
	}

	public class ShroudDuration
	{

	}

	public class TotalMagicDamage
	{

	}

	public class MinimumMagicDamage
	{

	}

	public class MaximumMagicDamage
	{

	}

	public class TotalPhysicalDamage
	{

	}

	public class NonChampionDamage
	{

	}

	public class PhysicalDamageperShot
	{

	}

	public class MaximumBulletsStored
	{

	}

	public class MinimumPhysicalDamageperBullet
	{

	}

	public class MaximumPhysicalDamageperBullet
	{

	}

	public class MinimumChargedPhysicalDamage
	{

	}

	public class Damagetotargetwithmissinghp
	{

	}

	public class MagicDamagePerTick
	{

	}

	public class DamageReduction
	{

	}

	public class PhysicalDamageReduction
	{

	}

	public class StunDuration
	{

	}

	public class Width
	{

	}

	public class Numberoficesegments
	{

	}

	public class Distancebetweenoutermostsegments
	{

	}

	public class Distancebetweenindividualsegments
	{

	}

	public class EnhancedDamage
	{

	}

	public class MagicDamageperTick
	{

	}

	public class Slow
	{

	}

	public class EmpoweredDamageperTick
	{

	}

	public class EmpoweredSlow
	{

	}

	public class Shield
	{

	}

	public class InitialMagicDamage
	{

	}

	public class BonusAttackDamage
	{

	}

	public class BonusAttackSpeed
	{

	}

	public class Lethality
	{

	}

	public class DamagePerArrow
	{

	}

	public class TotalDamagePerFlurry
	{

	}

	public class Arrows
	{

	}

	public class SecondaryMagicDamage
	{

	}

	public class MinimumStunDuration
	{

	}

	public class MaximumStunDuration
	{

	}

	public class BonusDamage
	{

	}

	public class StaticMovementSpeed
	{

	}

	public class IncreasedAttackSpeed
	{

	}

	public class ShieldStrength
	{

	}

	public class MinimumHeal
	{

	}

	public class MaximumHeal
	{

	}

	public class MonsterDamage
	{

	}

	public class ModifiedMinionDamage
	{

	}

	public class SlowDuration
	{

	}

	public class MinimumPhysicalDamageperhit
	{

	}

	public class MaximumPhysicalDamageperhit
	{

	}

	public class MinimumMonsterDamageperhit
	{

	}

	public class MaximumMonsterDamageperhit
	{

	}

	public class BonusTrueDamage
	{

	}

	public class MaximumMonsterDamage
	{

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

	}

	public class IncreasedTotalAttackSpeed
	{

	}

	public class Heal
	{

	}

	public class AttackSpeed
	{

	}

	public class MovementSpeed
	{

	}

	public class BoltMagicDamage
	{

	}

	public class DetonationMagicDamage
	{

	}

	public class IncreasedDamage
	{

	}

	public class AllyBonusArmor
	{

	}

	public class AllyBonusMagicResistance
	{

	}

	public class SelfBonusArmor
	{

	}

	public class SelfBonusMagicResistance
	{

	}

	public class Damagereduction
	{

	}

	public class Duration
	{

	}

	public class MaximumKnockupDuration
	{

	}

	public class ReducedDamage
	{

	}

	public class TrapDuration
	{

	}

	public class MaximumTraps
	{

	}

	public class HeadshotDamageIncrease
	{

	}

	public class Physicaldamage
	{

	}

	public class BonusPhysicalDamage
	{

	}

	public class IncreasedMixedDamage
	{
		public float[] percentAD = new float[5];
	}

	public class OuterConeBonusDamage
	{

	}

	public class NonEpicMonsterDamage
	{

	}

	public class BonusNonEpicMonsterDamage
	{

	}

	public class ZoneDuration
	{

	}

	public class BonusMagicDamage
	{

	}

	public class ReducedHealing
	{

	}

	public class SilenceDuration
	{

	}

	public class ChampionTrueDamage
	{

	}

	public class NonChampionTrueDamage
	{

	}

	public class BonusHealthPerStack
	{

	}

	public class BonusAttackRangePerStack
	{

	}

	public class BonusSizePerStack
	{

	}

	public class MixedDamagePerTick
	{
		public float[] flat = new float[5];
		public float[] percentBonusAD = new float[5];
	}

	public class ResistanceReductionPerTick
	{

	}

	public class TotalResistanceReduction
	{

	}

	public class BigOneMagicDamage
	{

	}

	public class BladePhysicalDamage
	{

	}

	public class HandlePhysicalDamage
	{

	}

	public class ArmorPenetration
	{

	}

	public class BonusDamagePerStack
	{

	}

	public class MaximumTrueDamage
	{

	}

	public class TotalShieldStrength
	{

	}

	public class MagicDamageperOrb
	{

	}

	public class BonusDamagePerChampion
	{

	}

	public class TotalDamageVsChampions
	{

	}

	public class MinimumDamage
	{

	}

	public class CappedMonsterDamage
	{

	}

	public class DamageStored
	{

	}

	public class MaximumAdditionalBonusAD
	{

	}

	public class MaximumTotalBonusAD
	{

	}

	public class MinimumBonusPhysicalDamage
	{

	}

	public class MaximumBonusPhysicalDamage
	{

	}

	public class MinimumNonChampionBonusDamage
	{

	}

	public class MaximumNonChampionBonusDamage
	{

	}

	public class IncreasedBaseHealth
	{

	}

	public class TotalHealthRegeneration
	{

	}

	public class RegenerationperSecond
	{

	}

	public class MinimumPhysicalDamage
	{

	}

	public class MinimumTotalDamage
	{

	}

	public class OutwardMagicDamage
	{

	}

	public class ReturningMagicDamage
	{

	}

	public class MaxMonsterTotalDamage
	{

	}

	public class DartDamage
	{

	}

	public class TotalBonusDamage
	{

	}

	public class SpikeDamage
	{

	}

	public class MaximumDamage
	{

	}

	public class CharmDuration
	{

	}

	public class MonsterDuration
	{

	}

	public class MagicResistanceReduction
	{

	}

	public class EmpoweredDamage
	{

	}

	public class FearDuration
	{

	}

	public class IncreasedMagicDamage
	{

	}

	public class IncreasedMinimumDamage
	{

	}

	public class LastTickofDamage
	{

	}

	public class ChampionHealPercentage
	{

	}

	public class TotalHealperChampion
	{

	}

	public class TotalHealperMonster
	{

	}

	public class TotalHealperMinion
	{

	}

	public class Criticaldamage
	{

	}

	public class AdditionalBonusMovementSpeed
	{

	}

	public class HealperTick
	{

	}

	public class ManaRefunded
	{

	}

	public class BonusOnHitDamage
	{

	}

	public class GuppyDamage
	{

	}

	public class ChomperDamage
	{

	}

	public class GigalodonDamage
	{

	}

	public class GustMagicDamage
	{

	}

	public class TornadoMagicDamagePerTick
	{

	}

	public class TotalTornadoMagicDamage
	{

	}

	public class MagicDamageShield
	{

	}

	public class MagicDamageReduction
	{

	}

	public class ChampionMagicDamage
	{

	}

	public class NonChampionMagicDamage
	{

	}

	public class CriticalDamage
	{

	}

	public class bug
	{

	}

	public class GoldPlunder
	{

	}

	public class SilverSerpentPlunder
	{

	}

	public class Maximumcharges
	{

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