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

    private TextMeshProUGUI output;

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

    public float UseSkill(int level, Simulator.Combat.ChampionStats myStats, Simulator.Combat.ChampionStats target)
    {
        float damage = 0;
        if(basic.name == "Ranger's Focus")
        {
            myStats.buffManager.AddBuff("Flurry", 4, 100 + (5 * (level+1)));
            SelfEffects(level, myStats);
            return 0;
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
            default:
                break;
        }


        //if (output == null) output = SimManager.outputText;
        //int totalDamage = 0;
        //int tempDamage = 0;

        /*switch (skillDamageType)
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
            }*/
        //totalDamage = (int)Mathf.Round((damage.flatAD[level] + (myStats.AD * (damage.percentAD[level] / 100))));
        //tempDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor)));

        //totalDamage = (int)Mathf.Round((damage.flatAP[level] + (myStats.AP * (damage.percentAP[level] / 100))));
        //tempDamage += (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));

        //tempDamage = (int)trueDamage.flat[level];

        //if (totalDamage < 0)
        {
            //totalDamage = 0;
        }


#region Special
        #region Garen

        //if (basic.champion == "Garen" && totalDamage < target.currentHealth && skillType == SkillType.ult) return totalDamage;
        #endregion
        /*
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
                    myStats.UpdateStats(false);
                    output.text += "[BUFF] " + myStats.name + " gains 90% AS increase for 2 attacks.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                    myStats.buffed = true;
                }
            }
        }
        #endregion
        */
        #region Lillia
        if (basic.champion == "Lillia")
        {
            if (basic.name == "Watch Out! Eep!")
            {
                //totalDamage *= 3;
            }
        }
        #endregion

        #region Jax        

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
            }

            if (basic.name == "Powder Keg")
            {
                myStats.eSkill.charge.charges--;
                //totalDamage = (int)Mathf.Round((damage.flatAD[level] + (myStats.AD * (damage.percentAD[level] / 100))));
                //totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.armor * 0.6f)));
            }
        }
        #endregion

        #region Riven
        if (basic.champion == "Riven" && skillType == SkillType.ult)
        {
            int bonusAD = (int)Mathf.Round(myStats.AD * 0.2f);
            myStats.AD += bonusAD;
            output.text += "[SPECIAL] " + myStats.name + " used " + basic.name + " and gains " + bonusAD + " bonus AD.\n\n";

            int missingHealth = (int)Mathf.Round(100-((myStats.currentHealth / myStats.maxHealth) * 100));
            if (missingHealth > 75)
            {
                missingHealth = 75;
            }
            int bonusDamage = (int)Mathf.Round(missingHealth * 2.667f);
            //totalDamage = totalDamage * (1 + (bonusDamage / 100));
        }
        #endregion

        #region Mordekaiser
        if (basic.name == "Obliterate")
        {
            //totalDamage = (int)Mathf.Round(totalDamage * 1.6f);
        }
        #endregion

        #region Veigar
        if (basic.champion == "Veigar")
        {
            if (skillType == SkillType.ult)
            {
                int missingHealth = (int)Mathf.Round(100 - ((myStats.currentHealth / myStats.maxHealth) * 100));
                int bonusDamage = (int)Mathf.Round(missingHealth * 1.5f);
                //totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));
                //totalDamage = totalDamage * (1 + (bonusDamage / 100));
            }
        }
        #endregion

        #region Akali
        if (basic.name == "Akali")
        {
            {
                int missingHealth = (int)Mathf.Round(100 - ((myStats.currentHealth / myStats.maxHealth) * 100));
                int bonusDamage = (int)Mathf.Round(missingHealth * 1.5f);
                //totalDamage = totalDamage * (1 + (bonusDamage / 100));
                //totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + target.spellBlock)));
            }
        }
        #endregion

        #region Darius
        #endregion

        #endregion

        //if (totalDamage == 0)
        
            //output.text += "[ABILITY] " + myStats.name + " used " + basic.name + ".\n\n";
        
        //else
        //{
            //if (myStats.currentHealth-totalDamage <= 0) return 0;
          //  output.text += "[DAMAGE] " + myStats.name + " used " + basic.name + " and dealt " + totalDamage.ToString() + " damage.\n\n";
        //}
        SelfEffects(level, myStats);
        EnemyEffects(level, target);
        //text.text = (prevDamage + totalDamage).ToString();
        //return totalDamage;
        return damage;
    }

    void EnemyEffects(int level, Simulator.Combat.ChampionStats target)
    {
        if (basic.champion == "Ashe")
        {
            target.buffManager.AddBuff("Frost", 2, 0);
        }

        if (enemyEffects.stun)
        {
            target.buffManager.AddBuff("Stunned", enemyEffects.stunDuration, 0);     
        }

        if (enemyEffects.silence)
        {
            output.text += "[DEBUFF] " + target.name + " is silenced for "+enemyEffects.silenceDuration+" seconds.\n\n";
        }

        if (enemyEffects.disarm)
        {
        }
    }

    void SelfEffects(int level, Simulator.Combat.ChampionStats myStats)
    {
        #region Disarm
        if (selfEffects.disarm)
        {
        }
        #endregion
        
        #region AS Increase
        if (selfEffects.ASIncrease)
        {
            myStats.buffManager.AddBuff("AttackSpeed", selfEffects.ASIncreaseDuration[level], myStats.baseAttackSpeed * 0.01f * selfEffects.ASIncreasePercent[level]);
        }
        #endregion
        
        #region Invincible
        if (selfEffects.Invincible)
        { 
        }
        #endregion

        #region Damage Reduction
        if (selfEffects.DamageRed)
        {
            output.text += "[BUFF] " + myStats.name + " gains " + selfEffects.DamageRedPercent[level] + "% for " + selfEffects.DamageRedDuration[level] + " seconds.\n\n";
        }
        #endregion

        #region Shield
        if (selfEffects.Shield)
        {
            int shieldValue = (int)selfEffects.ShieldFlat[level];
            shieldValue += (int)Mathf.Round(((selfEffects.ShieldPercentByMissingHP[level]/100) * (myStats.maxHealth - myStats.currentHealth)));
            output.text += "[BUFF] " + myStats.name + " gains " + shieldValue + " shield for " + selfEffects.ShieldDuration[level] + " seconds.\n\n";
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