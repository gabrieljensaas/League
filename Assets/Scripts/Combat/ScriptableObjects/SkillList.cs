using Simulator.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillDamageType
{
    Phyiscal,
    Spell,
    True,
    PhysAndSpell,
    PhysAndTrue,
    SpellAndTrue
}

[CreateAssetMenu(fileName = "new Spell", menuName = "ScriptableObjects/SkillList")]
public class SkillList : ScriptableObject
{

    //string version;
    public SkillBasic basic;

    public SkillType skillType;
    public SkillDamageType skillDamageType;

    public AttributeList atrList;
    //public UnitList unit;
    public List<AbilityEffect> effects = new();

    [Header("Buffs/Debuffs")]
    public SkillEnemyEffects enemyEffects;
    public SkillSelfEffects selfEffects;

    public enum SkillType
    {
        ability,
        ult,
        passive
    }

    public float UseSkill(int level, string key, ChampionStats myStats, ChampionStats targetStats, float cap = 0, float expendedGrit = 0)
    {
        AbilityEffect effect = effects.Find(x => x.attribute == key);

        return (float)(effect.flat[level] +
           (effect.percentAP[level] * myStats.AP * 0.01f) +
           (effect.percentAD[level] * myStats.AD * 0.01f) +
           (effect.percentBonusAD[level] * myStats.bonusAD * 0.01f) + (effect.percentBonusAS[level] * (int)(myStats.bonusAS / myStats.baseAttackSpeed) * 0.01f) +
           (effect.percentBonusHP[level] * myStats.bonusHP * 0.01f) +
           (effect.percentTargetMissingHP[level] * (targetStats.maxHealth - targetStats.currentHealth) * 0.01f) +
           (effect.percent[level] * 0.01f) +
           (effect.percentTargetMaxHP[level] * 0.01f) + (effect.percentPer100AP[level] * (myStats.AP % 100) * 0.01f * targetStats.maxHealth) + (effect.percentPer100AD[level] * (myStats.AD % 100) * 0.01f * targetStats.maxHealth) +
           (effect.percentMissingHP[level] * 0.01f * ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) > cap ? cap : myStats.maxHealth - myStats.currentHealth) +
           (effect.percentDmgDealt[level] * 0.01f) +
           (((effect.expendedGrit[level] * 0.01f) + (myStats.bonusAD * 0.0025f)) * expendedGrit) +
           (effect.percentPrimaryTargetBonusHP[level] * 0.01f * targetStats.bonusHP));
    }

    public float SylasUseSkill(int level, string key, ChampionStats myStats, ChampionStats targetStats, float cap = 0)
    {
        AbilityEffect effect = effects.Find(x => x.attribute == key);

        return (float)(effect.flat[level] +
           (effect.percentAP[level] * myStats.AP * 0.01f) +
           (effect.percentAD[level] * myStats.AP * 0.006f) +
           ((effect.percentBonusAD[level] * myStats.AP * 0.004f) + (effect.percentBonusAS[level] * (int)(myStats.bonusAS / myStats.baseAttackSpeed) * 0.01f)) +
           (effect.percentBonusHP[level] * myStats.bonusHP * 0.01f) +
           (effect.percentTargetMissingHP[level] * (targetStats.maxHealth - targetStats.currentHealth) * 0.01f) +
           (effect.percent[level] * 0.01f) +
           (((effect.percentTargetMaxHP[level] * 0.01f) + (effect.percentPer100AP[level] * (myStats.AP % 100) * 0.01f)) * targetStats.maxHealth) +
           (effect.percentMissingHP[level] * 0.01f * ((myStats.maxHealth - myStats.currentHealth) / myStats.maxHealth) > cap ? cap : (myStats.maxHealth - myStats.currentHealth)) +
           (effect.percentDmgDealt[level] * 0.01f));
    }
}

[System.Serializable]
public class SkillsList
{
    public string name;
}

[Serializable]
public class SkillEnemyEffects
{
    [Header("Basic")]
    public bool stun;
    public float stunDuration;

    public bool silence;
    public float silenceDuration;

    public bool disarm;
    public float disarmDuration;

    public bool ASIncrease;
    public float ASIncreaseFlat;
    public float ASIncreasePercent;
    public float ASIncreaseDuration;

    public bool MSIncrease;
    public float MSIncreaseFlat;
    public float MSIncreasePercent;
    public float MSIncreaseDuration;

    public bool ASSlow;
    public float ASSlowDuration;

    public bool MSSlow;
    public float MSSlowDuration;

    public bool Invincible;
    public float InvincibleDuration;
}

[System.Serializable]
public class SkillSelfEffects
{
    [Header("Basic")]
    public bool stun;
    public float stunDuration;

    public bool silence;
    public float silenceDuration;

    public bool disarm;
    public float disarmDuration;

    public bool ASIncrease;
    public float[] ASIncreaseFlat = new float[5];
    public float[] ASIncreasePercent = new float[5];
    public float[] ASIncreaseDuration = new float[5];

    public bool MSIncrease;
    public float[] MSIncreaseFlat = new float[5];
    public float[] MSIncreasePercent = new float[5];
    public float[] MSIncreaseDuration = new float[5];

    public bool DamageRed;
    public float[] DamageRedFlat = new float[5];
    public float[] DamageRedPercent = new float[5];
    public float[] DamageRedDuration = new float[5];

    public bool Tenacity;
    public float[] TenacityPercent = new float[5];
    public float[] TenacityDuration = new float[5];

    public bool ASSlow;
    public float ASSlowDuration;

    public bool MSSlow;
    public float MSSlowDuration;

    public bool Invincible;
    public float InvincibleDuration;

    [Header("Armor")]
    public bool ArmorBonus;
    public float[] ArmorFlat = new float[5];
    public float[] ArmorPercent = new float[5];
    public float[] ArmorPercentByAD = new float[5];
    public float[] ArmorPercentByModAD = new float[5];
    public float[] ArmorPercentByAP = new float[5];
    public float[] ArmorPercentByModAP = new float[5];
    public float[] ArmorDuration = new float[5];


    [Header("Spell Block")]
    public bool SpellBlockBonus;
    public float[] SpellBlockFlat = new float[5];
    public float[] SpellBlockPercent = new float[5];
    public float[] SpellBlockPercentByAD = new float[5];
    public float[] SpellBlockPercentByModAD = new float[5];
    public float[] SpellBlockPercentByAP = new float[5];
    public float[] SpellBlockPercentByModAP = new float[5];
    public float[] SpellBlockDuration = new float[5];

    [Header("Shield")]
    public bool Shield;
    public float[] ShieldFlat = new float[5];
    public float[] ShieldPercent = new float[5];
    public float[] ShieldPercentByHP = new float[5];
    public float[] ShieldPercentByMissingHP = new float[5];
    public float[] ShieldPercentByBonusAD = new float[5];
    public float[] ShieldDuration = new float[5];
}