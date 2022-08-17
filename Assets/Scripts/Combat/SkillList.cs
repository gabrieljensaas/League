using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Simulator.Combat;
using static AttributeTypes;

[CreateAssetMenu(fileName = "new Spell", menuName = "ScriptableObjects/SkillList")]
public class SkillList : ScriptableObject
{

    //string version;
    public SkillBasic basic;

    public SkillType skillType;
    public SkillDamageType skillDamageType;

    private SimManager simulationManager;

    public SkillMultiHit multihit;

    public SkillChargeType charge;
    public UnitList unit;
    public SkillBasicDamage damage;
    public SkillTrueDamage trueDamage;
    public SkillDamageByHP damageByHP;
    public SkillHeal heal;

    [Header("Buffs/Debuffs")]
    public SkillEnemyEffects enemyEffects;
    public SkillSelfEffects selfEffects;
    public enum SkillDamageType    
    { 
        Phyiscal,
        Spell,
        True,
        PhysAndSpell,
        PhysAndTrue,
        SpellAndTrue
    }

    public enum SkillType
    {
        ability,
        ult,
        passive
    }

    public float UseSkill(int level, Simulator.Combat.ChampionStats myStats, Simulator.Combat.ChampionStats target)
    {
        if(simulationManager == null) simulationManager = SimManager.Instance;
        float damage = 0;
        if(basic.name == "Ranger's Focus")
        {
            myStats.buffManager.AddBuff("Flurry", 4, 100 + (5 * (level+1)), basic.name);
            simulationManager.ShowText($"Ashe Used Ranger's Focus Her Auto Attacks Will Do Extra {100 + (5 * (level+1))} Damage For 4 Seconds!");
            myStats.buffManager.AsheQBuff = 0;
            SelfEffects(level, myStats);
            return 0;
        }

        if(basic.name == "Decisive Strike")
        {
            myStats.buffManager.AddBuff("Decisive Strike", 4.5f, ((level + 1) * 30) + myStats.AD * 0.5f, basic.name);
            simulationManager.ShowText($"Garen Used Decisive Strike His Next Auto Attack In 4.5 Seconds Will Do Extra {((level + 1) * 30) + myStats.AD * 0.5f} Damage!");
            return 0;
        }

        if(basic.name == "Judgment")
        {
            damage = (int)Mathf.Round((unit.flat[level] + Constants.GarenEDamageByLevelTable[myStats.level - 1] + (myStats.AD * (unit.percentAD[level] / 100))));
            damage = (int)Mathf.Round(damage * (100 / (100 + target.armor)));
            return damage;
        }

        switch (skillDamageType)
        {
            case SkillDamageType.Phyiscal:
                damage = (int)Mathf.Round((unit.flat[level] + (myStats.AD * (unit.percentAD[level] / 100))));
                damage = (int)Mathf.Round(damage * (100 / (100 + target.armor)));
                break;
            case SkillDamageType.Spell:
                damage = (int)Mathf.Round((unit.flat[level] + (myStats.AP * (unit.percentAP[level] / 100))));
                damage = (int)Mathf.Round(damage * (100 / (100 + target.spellBlock)));
                break;
            case SkillDamageType.True:
                damage = (int)Mathf.Round(unit.flat[level] + (target.currentHealth / target.maxHealth * (unit.percentTargetMissingHP[level] / 100)));
                break;
            default:
                break;
        }

        SelfEffects(level, myStats);
        EnemyEffects(level, target);
        return damage;
    }

    private void EnemyEffects(int level, Simulator.Combat.ChampionStats target)
    {
        if (basic.champion == "Ashe")
        {
            target.buffManager.AddBuff("Frost", 2, 0, basic.name);
        }

        if (enemyEffects.stun)
        {
            target.buffManager.AddBuff("Stunned", enemyEffects.stunDuration, 0, basic.name);
        }
    }

    private void SelfEffects(int level, Simulator.Combat.ChampionStats myStats)
    {
        if (selfEffects.Tenacity)
        {
            myStats.buffManager.AddBuff("Tenacity", selfEffects.TenacityDuration[level], selfEffects.TenacityPercent[level], basic.name);
        }
        
        if (selfEffects.ASIncrease)
        {
            myStats.buffManager.AddBuff("AttackSpeed", selfEffects.ASIncreaseDuration[level], myStats.baseAttackSpeed * 0.01f * selfEffects.ASIncreasePercent[level], basic.name);
        }        

        if (selfEffects.DamageRed)
        {
            myStats.buffManager.AddBuff("DamageReductionPercent", selfEffects.DamageRedDuration[level], selfEffects.DamageRedPercent[level], basic.name);
        }

        if (selfEffects.Shield)
        {
            myStats.buffManager.AddBuff("Shield", selfEffects.ShieldDuration[level], selfEffects.ShieldFlat[level], basic.name);       //bonus health scaling doesnt work because no items or runes implemented yet
        }
    }
}

[System.Serializable]
public class SkillsList
{
    public string name;
}

[System.Serializable]
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