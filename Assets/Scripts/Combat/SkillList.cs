using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "ScriptableObjects/SkillList")]
public class SkillList : ScriptableObject
{

    public SkillBasic basic;

    public SkillType skillType;
    public SkillDamageType skillDamageType;

    private TextMeshProUGUI output;

    public SkillMultiHit multihit;

    public SkillChargeType charge;
    public SkillBasicDamage damage;
    public SkillTrueDamage trueDamage;
    public SkillDamageByHP damageByHP;
    public SkillHeal heal;

    [Header("Buffs/Debuffs")]
    public SkillEnemyEffects enemyEffects;
    public SkillSelfEffects selfEffects;

    #region Enums
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
    #endregion

    public int UseSkill(int level, ChampStats myStats, ChampStats target, TextMeshProUGUI text, int prevDamage)
    {
        output = GameObject.FindGameObjectWithTag("Output Content").GetComponent<TextMeshProUGUI>();
        int totalDamage = 0;

        switch (skillDamageType)
            {
                case SkillDamageType.Phyiscal:
                    totalDamage = (int)Mathf.Round((damage.flatAD[level] + (myStats.AD * (damage.percentAD[level] / 100))));
                    totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor)));
                    break;

                case SkillDamageType.Spell:
                    totalDamage = (int)Mathf.Round((damage.flatAP[level] + (myStats.AP * (damage.percentAP[level] / 100))));
                    totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));
                    break;

                case SkillDamageType.True:
                    totalDamage = (int)trueDamage.flat[level];
                    if (basic.champion == "Olaf")
                    {
                    totalDamage += (int)Mathf.Round(myStats.AD * (50f / 100));
                    }
                    totalDamage += (int) Mathf.Round((target.maxHealth - target.currentHealth) * (damageByHP.targetMissingHPDamage[level] /100));
                    break;
            }
        if (totalDamage < 0)
        {
            totalDamage = 0;
        }

#region Special
        #region Garen
        if (myStats.name == "Garen")
        {
            if (basic.name == "Judgement")
            {
                totalDamage = (int)Mathf.Round((damage.flatAD[level] + (myStats.AD * (damage.percentAD[level] / 100))));
                if (target.dynamicStatus.ContainsKey(basic.name))
                {
                    if (target.dynamicStatusStacks[basic.name] < 8)
                    {
                        target.dynamicStatusStacks[basic.name]++;
                        for (int i = 0; i < target.dynamicStatusStacks[basic.name]; i++)
                        {
                            totalDamage += (int)Mathf.Round(35 + (myStats.AD % 0.5f));
                        }
                    }
                }
                else
                {
                    target.dynamicStatus[basic.name] = true;
                    target.dynamicStatusStacks[basic.name] = 1;
                }
                totalDamage *= (int)Mathf.Round(1.25f);
                totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor)));
            }
        }

        if (basic.champion == "Garen" && totalDamage < target.currentHealth && skillType == SkillType.ult) return totalDamage;
        #endregion

        #region Fiora
        if (myStats.name == "Fiora")
        {
            if (basic.name == "Bladework")
            {
                if (!myStats.buffed)
                {
                    myStats.dynamicStatus["Bladework"] = true;
                    myStats.dynamicStatusStacks["Bladework"] = 2;
                    myStats.dynamicStatusPercent["Bladework"] = 90;
                    myStats.PercentAttackSpeedMod += myStats.dynamicStatusPercent["Bladework"];
                    myStats.attackSpeed *= (1 + (myStats.PercentAttackSpeedMod / 100));
                    myStats.UpdateStats();
                    output.text += "[BUFF] " + myStats.name + " gains 90% AS increase for 2 attacks.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                    myStats.buffed = true;
                }
            }
        }
        #endregion

        #region Lillia
        if (basic.champion == "Lillia")
        {
            if (basic.name == "Watch Out! Eep!")
            {
                totalDamage *= 3;
            }
        }
        #endregion

        #region Jax        
        if (basic.name == "Counter Strike")
        {
            totalDamage = (int)Mathf.Round((damage.flatAD[level]) * (1 + (myStats.dynamicStatusStacks["Counter Strike"] * 0.2f)));
            totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor)));
        }

        #endregion

        #region Gangplank
        if (basic.champion == "Gangplank")
        {
            if (basic.name == "Remove Scurvy")            
            {
                int amount = (int)heal.flat[level];
                amount += (int)Mathf.Round(myStats.AP * (heal.flatByAP[level] / 100));
                amount += (int)Mathf.Round((myStats.maxHealth - myStats.currentHealth) * (13 / 100));
                myStats.currentHealth += amount;
                output.text += "[HEAL] " + myStats.name + " used " + basic.name + " and heals himself for " + heal + " health.\n\n";
                myStats.UpdateTimer(SimManager.timer);
                myStats.UpdateStats();
            }

            if (basic.name == "Powder Keg")
            {
                myStats.eSkill.charge.charges--;
                totalDamage = (int)Mathf.Round((damage.flatAD[level] + (myStats.AD * (damage.percentAD[level] / 100))));
                totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor * 0.6f)));
                myStats.eReady = true;
                myStats.passiveReady = true;
            }
        }
        #endregion

        #region Riven
        if (basic.champion == "Riven" && skillType == SkillType.ult)
        {
            int bonusAD = (int)Mathf.Round(myStats.AD * 0.2f);
            myStats.AD += bonusAD;
            myStats.UpdateStats();
            output.text += "[SPECIAL] " + myStats.name + " used " + basic.name + " and gains " + bonusAD + " bonus AD.\n\n";
            myStats.UpdateTimer(SimManager.timer);
            myStats.dynamicStatus[basic.name] = true;
            myStats.dynamicStatusDuration[basic.name] = 15;
            int missingHealth = (int)Mathf.Round(100-((myStats.currentHealth / myStats.maxHealth) * 100));
            if (missingHealth > 75)
            {
                missingHealth = 75;
            }
            int bonusDamage = (int)Mathf.Round(missingHealth * 2.667f);
            totalDamage = totalDamage * (1 + (bonusDamage / 100));
        }
        #endregion

        #region Mordekaiser
        if (basic.name == "Obliterate")
        {
            totalDamage = (int)Mathf.Round(totalDamage * 1.6f);
        }
        #endregion

        #region Veigar
        if (basic.champion == "Veigar")
        {
            if (skillType == SkillType.ult)
            {
                int missingHealth = (int)Mathf.Round(100 - ((myStats.currentHealth / myStats.maxHealth) * 100));
                int bonusDamage = (int)Mathf.Round(missingHealth * 1.5f);
                totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));
                totalDamage = totalDamage * (1 + (bonusDamage / 100));
            }
        }
        #endregion

        #region Akali
        if (basic.name == "Akali")
        {
            if (myStats.dynamicStatus.ContainsKey("Ult First Hit"))
            {
                int missingHealth = (int)Mathf.Round(100 - ((myStats.currentHealth / myStats.maxHealth) * 100));
                int bonusDamage = (int)Mathf.Round(missingHealth * 1.5f);
                totalDamage = totalDamage * (1 + (bonusDamage / 100));
                totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));
            }
        }
        #endregion

        #region Darius
        if (basic.champion == "Darius")
        {
            totalDamage += (int)Mathf.Round(myStats.FlatPhysicalDamageMod * 0.75f);
            if (skillType == SkillType.ult)
            {
                int bonusDamage = 0;
                try
                {
                    bonusDamage = myStats.dynamicStatusStacks[basic.name] * 75 + (int)Mathf.Round(myStats.FlatPhysicalDamageMod * 0.15f);
                }
                catch
                {
                    bonusDamage = 0;
                }
                finally
                {
                    totalDamage += bonusDamage;
                }
            }
        }
        #endregion

        #endregion

        if (target.status.damageReduction)
        {
            totalDamage = (int)Mathf.Round(totalDamage * (1 - (target.status.damageReducPercent / 100)));
        }

        if (totalDamage == 0)
        {
            output.text += "[ABILITY] " + myStats.name + " used " + basic.name + ".\n\n";
        }
        else
        {
            if (myStats.currentHealth-totalDamage <= 0) return 0;
            output.text += "[DAMAGE] " + myStats.name + " used " + basic.name + " and dealt " + totalDamage.ToString() + " damage.\n\n";
        }
        target.TakeDamage(totalDamage);
        myStats.UpdateTimer(SimManager.timer);

        SelfEffects(level, myStats);
        EnemyEffects(level,target);
        text.text = (prevDamage + totalDamage).ToString();
        myStats.totalDamage += totalDamage;
        return totalDamage;
    }

    void EnemyEffects(int level, ChampStats target)
    {
        if (enemyEffects.stun)
        {
            if (!target.ccImmune)
            {
                target.status.stun = true;
                target.status.stunDuration = enemyEffects.stunDuration;
                output.text += "[DEBUFF] " + target.name + " is stunned for " + enemyEffects.stunDuration + " seconds.\n\n";
                target.UpdateTimer(SimManager.timer);
            }
        }

        if (enemyEffects.silence)
        {
            target.status.silence = true;
            target.status.silenceDuration = enemyEffects.silenceDuration;
            output.text += "[DEBUFF] " + target.name + " is silenced for "+enemyEffects.silenceDuration+" seconds.\n\n";
            target.UpdateTimer(SimManager.timer);
        }

        if (enemyEffects.disarm)
        {
            target.status.disarm = true;
            target.status.disarmDuration = enemyEffects.disarmDuration;
        }
    }

    void SelfEffects(int level, ChampStats myStats)
    {
        #region Disarm
        if (selfEffects.disarm)
        {
            if (myStats.status.disarm) return;
            myStats.status.disarm = true;
            myStats.status.disarmDuration = selfEffects.disarmDuration;
        }
        #endregion

        #region AS Increase
        if (selfEffects.ASIncrease)
        {
            if (myStats.buffed) return;
            myStats.buffed = true;
            myStats.ModifyAS(selfEffects.ASIncreaseDuration[level], selfEffects.ASIncreasePercent[level]);
            output.text += "[BUFF] " + myStats.name + " gains " + selfEffects.ASIncreasePercent[level] + "% for "+selfEffects.ASIncreaseDuration[level] + " seconds.\n\n";
            myStats.UpdateTimer(SimManager.timer);
        }
        #endregion

        #region Invincible
        if (selfEffects.Invincible)
        { 
            myStats.status.invincible = true;
            myStats.status.invincibleDuration = selfEffects.InvincibleDuration;
        }
        #endregion

        #region Damage Reduction
        if (selfEffects.DamageRed)
        {
            myStats.status.damageReduction = true;
            myStats.status.damageReducPercent = selfEffects.DamageRedPercent[level];
            myStats.status.damageReducFlat = selfEffects.DamageRedFlat[level];
            myStats.status.damageReducDuration = selfEffects.DamageRedDuration[level];
            output.text += "[BUFF] " + myStats.name + " gains " + selfEffects.DamageRedPercent[level] + "% for " + selfEffects.DamageRedDuration[level] + " seconds.\n\n";
            myStats.UpdateTimer(SimManager.timer);
        }
        #endregion

        #region Shield
        if (selfEffects.Shield)
        {
            int shieldValue = (int)selfEffects.ShieldFlat[level];
            shieldValue += (int)Mathf.Round(myStats.FlatHPPoolMod * (selfEffects.ShieldPercentByHP[level] / 100));
            shieldValue += (int)Mathf.Round(((selfEffects.ShieldPercentByMissingHP[level]/100) * (myStats.maxHealth - myStats.currentHealth)));
            myStats.shieldValue = shieldValue;
            myStats.shieldDuration = selfEffects.ShieldDuration[level];
            output.text += "[BUFF] " + myStats.name + " gains " + shieldValue + " shield for " + selfEffects.ShieldDuration[level] + " seconds.\n\n";
            myStats.UpdateTimer(SimManager.timer);
        }
        #endregion
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
