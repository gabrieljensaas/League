using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Simulator.Combat;
using static AttributeTypes;
using System;

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

    public float UseSkill(int level, ChampionStats myStats, ChampionStats target)
    {
        if(simulationManager == null) simulationManager = SimManager.Instance;
        float damage = 0;
        if(basic.name == "Ranger's Focus")
        {
            myStats.buffManager.buffs.Add("Flurry", new FlurryBuff(4, myStats.buffManager, basic.name, 100 + (5 * (level + 1))));
            SelfEffects(level, myStats);
            return 0;
        }

        if(basic.name == "Decisive Strike")
        {
            myStats.buffManager.buffs.Add("DecisiveStrike", new DecisiveStrikeBuff(4.5f, myStats.buffManager, basic.name, ((level + 1) * 30) + myStats.AD * 0.5f));
            return 0;
        }

        if(basic.name == "Judgment")
        {
            damage = (int)Mathf.Round((unit.flat[level] + Constants.GarenEDamageByLevelTable[myStats.level - 1] + (myStats.AD * (unit.percentAD[level] / 100))));
            damage = (int)Mathf.Round(damage * (100 / (100 + target.armor)));
            return damage;
        }

        if(basic.name == "Crippling Strike")
        {
            myStats.buffManager.buffs.Add("Crippling Strike", new CripplingStrikeBuff(4, myStats.buffManager, basic.name, (135 + ((level + 1) * 5)) / 100 * myStats.AD));
            return 0;
        }

        switch (skillDamageType)
        {
            case SkillDamageType.Phyiscal:
                var targetmissingHealth = target.maxHealth - target.currentHealth;
                if (myStats.name == "Riven" && targetmissingHealth > target.maxHealth * 0.75f) targetmissingHealth = target.maxHealth * 0.75f;
                damage = (int)Mathf.Round(unit.flat[level] + (myStats.AD * (unit.percentAD[level] / 100)) + unit.percentTargetMissingHP[level] * (targetmissingHealth / target.maxHealth) * 100);
                break;
            case SkillDamageType.Spell:
                damage = (int)Mathf.Round(unit.flat[level] + (myStats.AP * (unit.percentAP[level] / 100)));
                break;
            case SkillDamageType.True:
                damage = (int)Mathf.Round(unit.flat[level] + ((target.maxHealth - target.currentHealth) * (unit.percentTargetMissingHP[level] / 100)) + (unit.percentOwnMissingHP[level] * (unit.flat[level] * ((float)(100 - ((int)(100 * myStats.currentHealth/myStats.maxHealth))) / 100f))));
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
            if(!target.buffManager.buffs.TryAdd("Frosted",new FrostedBuff(2, target.buffManager, basic.name)))
            {
                target.buffManager.buffs["Frosted"].duration = 2;
                target.buffManager.buffs["Frosted"].source = basic.name;
            }
        }

        if (enemyEffects.stun)
        {
            target.buffManager.buffs.Add("Stun", new StunBuff(enemyEffects.stunDuration, target.buffManager, basic.name));
        }
    }

    private void SelfEffects(int level, Simulator.Combat.ChampionStats myStats)
    {
        if (selfEffects.Tenacity)
        {
            myStats.buffManager.buffs.Add("Tenacity", new TenacityBuff(selfEffects.TenacityDuration[level], myStats.buffManager, basic.name, selfEffects.TenacityPercent[level], basic.name));
        }
        
        if (selfEffects.ASIncrease)
        {
            if(!myStats.buffManager.buffs.TryAdd(basic.name, new AttackSpeedBuff(selfEffects.ASIncreaseDuration[level], myStats.buffManager, basic.name, myStats.baseAttackSpeed * 0.01f * selfEffects.ASIncreasePercent[level], basic.name)))
            {
                //myStats.buffManager.buffs["AttackSpeed"].duration = selfEffects.ASIncreaseDuration[level];
            }
        }        

        if (selfEffects.DamageRed)
        {
            myStats.buffManager.buffs.TryAdd("DamageReductionPercent" ,new DamageReductionPercentBuff(selfEffects.DamageRedDuration[level], myStats.buffManager, basic.name, selfEffects.DamageRedPercent[level]));
        }

        if (selfEffects.Shield)
        {
            var missingHealth = myStats.maxHealth - myStats.currentHealth;
            if(myStats.name == "Olaf" && missingHealth > myStats.maxHealth * 0.7f) missingHealth = myStats.maxHealth * 0.7f;

            myStats.buffManager.shields.Add(basic.name, new ShieldBuff(selfEffects.ShieldDuration[level], myStats.buffManager, basic.name, selfEffects.ShieldFlat[level] + (selfEffects.ShieldPercentByMissingHP[level] * 0.01f * missingHealth), basic.name));
        }
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