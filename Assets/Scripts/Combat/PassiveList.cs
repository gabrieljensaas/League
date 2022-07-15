using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "ScriptableObjects/Passives")]
public class PassiveList : ScriptableObject
{
    public string championName;
    public string skillName;
    public float castTime;

    public SkillDamageType skillDamageType;

    private TextMeshProUGUI output;
    public float coolDown;

    //If scales by champion level
    public bool inactive;
    public bool levelScale;
    public bool applyOnAttack;
    public bool applyOnAbility;
    public bool alwaysActive;

    //Attack Damage
    public float minAD;
    public float maxAD;
    public float percentMinAD;
    public float percentMaxAD;

    //Spell Damage
    public float minAP;
    public float maxAP;
    public float percentMinAP;
    public float percentMaxAP;

    //True Damage
    public float minTD;
    public float maxTD;
    public float percentMinTD;
    public float percentMaxTD;

    //Heal
    public float minHeal;
    public float maxHeal;
    public float percentMinHeal;
    public float percentMaxHeal;

    //Missing Health
    public float percentMinTargetMissingHealth;
    public float percentMaxTargetMissingHealth;
    public float percentMinTargetMaxHealth;
    public float percentMaxTargetMaxHealth;


    //public TrueDamageType TDType;
    SkillEnemyEffects enemyEffects;
    SkillSelfEffects selfEffects;

    public enum SkillDamageType
    {
        Phyiscal,
        Spell,
        True,
        PhysAndSpell,
        MissingHealth,
        PhysAndTrue,
        SpellAndTrue,
        PhysAndMissingHealth,
        SpellAndMissingHealth
    }

    public enum TrueDamageType
    {
        AttackDamage,
        AbilityPower,
        Mixed
    }

    public int UseSkill(int level, ChampStats myStats, ChampStats target)
    {
        output = GameObject.FindGameObjectWithTag("Output Content").GetComponent<TextMeshProUGUI>();
        int damage = 0;

        switch (championName)
        {
            case "Akali":
                damage = (int)Mathf.Round(182 + myStats.AP * 0.55f);
                damage = (int)Mathf.Round((target.maxHealth * (percentMaxTargetMaxHealth + ((Mathf.Round((myStats.AP / 100))) * 12) / 10)) / 100);
                damage -= (int)target.spellBlock;
                target.TakeDamage(damage);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + maxHeal + "\n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Darius":
                if (!myStats.dynamicStatus.ContainsKey(skillName))
                {
                    myStats.dynamicStatus[skillName] = true;
                    myStats.dynamicStatusStacks[skillName] = 1;
                }
                else
                {
                    if (myStats.dynamicStatusStacks[skillName] < 5)
                    {
                        myStats.dynamicStatusStacks[skillName]++;
                    }
                    else if(myStats.dynamicStatusStacks[skillName] >= 5)
                    {
                        myStats.FlatPhysicalDamageMod += 230;
                    }
                    damage = 150 + (int)Mathf.Round(myStats.FlatPhysicalDamageMod * 0.3f);
                    output.text += "[PASSIVE] " +myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing "+damage+" damage\n\n";
                    target.UpdateTimer(SimManager.timer);
                }
                break;

            case "Ashe":
                if (!target.dynamicStatus.ContainsKey(skillName))
                {
                    target.dynamicStatus[skillName] = true;
                }
                else
                {
                    output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing 10% more damage\n\n";
                    target.UpdateTimer(SimManager.timer);
                }
                break;

            case "Lillia":
                damage = (int)Mathf.Round((target.maxHealth * (percentMaxTargetMaxHealth + ((Mathf.Round((myStats.AP / 100))) * 12) / 10)) / 100);
                damage -= (int)target.spellBlock;
                if (myStats.currentHealth <= 0) return 0;
                myStats.currentHealth += maxHeal;
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
                myStats.UpdateStats();
                target.TakeDamage(damage);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: "+maxHeal+"\n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Fiora":
                damage = (int)Mathf.Round((target.maxHealth * (percentMaxTargetMaxHealth + ((Mathf.Round(((myStats.AD-myStats.baseAD) / 100))) * 55) / 10)) / 100);
                myStats.currentHealth += maxHeal;
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
                myStats.UpdateStats();
                target.TakeDamage(damage);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + maxHeal + "\n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Jax":
                float[] value = { 3.5f, 3.5f, 3.5f, 5f, 5f, 5f, 6.5f, 6.5f, 6.5f, 8f, 8f, 8f, 9.5f, 9.5f, 9.5f, 11f, 11f, 11f };
                if (myStats.dynamicStatus.ContainsKey(skillName))
                {
                    myStats.dynamicStatusStacks[skillName]++;
                    if (myStats.dynamicStatusStacks[skillName] > 8)
                    {
                        myStats.dynamicStatusStacks[skillName] = 8;
                    }
                    else
                    {
                        myStats.attackSpeed = myStats.originalAS;
                        myStats.PercentAttackSpeedMod = (value[level-1]) * myStats.dynamicStatusStacks[skillName];
                        //Debug.Log(myStats.PercentAttackSpeedMod);
                        myStats.attackSpeed *= (1 + (myStats.PercentAttackSpeedMod / 100));
                        myStats.UpdateStats();
                    }
                }
                else
                {
                    myStats.dynamicStatus[skillName] = true;
                    myStats.dynamicStatusStacks[skillName] = 1;
                    myStats.PercentAttackSpeedMod += value[level-1];
                    myStats.attackSpeed *= (1 + (myStats.PercentAttackSpeedMod / 100));
                    myStats.UpdateStats();
                }
                break;

            case "Olaf":
                float ASBase = 36.47f;
                float ASGrowth = 3.53f;
                float TotalAS = ASBase + (level * ASGrowth);
                float BaseADVamp = 7;
                float TotalADVamp = BaseADVamp + level;
                myStats.PercentLifeStealMod = TotalADVamp;
                myStats.attackSpeed = myStats.originalAS;
                myStats.PercentAttackSpeedMod = TotalAS;
                myStats.attackSpeed *= (1 + (myStats.PercentAttackSpeedMod / 100));
                myStats.UpdateStats();
                break;

            case "Gangplank":
                damage = (int)Mathf.Round(310+myStats.FlatPhysicalDamageMod);
                target.TakeDamage(damage);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " dealing " + damage.ToString() + " true damage \n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Aatrox":
                int postdamage = (int)Mathf.Round(target.maxHealth * (12f / 100));
                damage = (int)Mathf.Round(postdamage * (100 / (100 + target.armor)));
                myStats.currentHealth += postdamage;
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: "+postdamage+"\n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Riven":
                damage = (int)Mathf.Round(myStats.AD * 0.6f);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                target.UpdateTimer(SimManager.timer);
                break;

            case "Mordekaiser":
                if (level == 19)
                {
                    damage = (int)15.2f;
                    damage += (int)Mathf.Round(myStats.AP * 0.3f);
                    damage += (int)Mathf.Round(target.maxHealth * 0.05f);
                    output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                    target.UpdateTimer(SimManager.timer);
                }
                else
                {
                    damage = (int)Mathf.Round(myStats.AP * 0.4f);
                    output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                    target.UpdateTimer(SimManager.timer);
                }
                break;
        }
        return damage;
    }

    void EnemyEffects(ChampStats target)
    {
        if (enemyEffects.stun)
        {
            target.status.stun = true;
            target.status.stunDuration = enemyEffects.stunDuration;
        }

        if (enemyEffects.silence)
        {
            target.status.silence = true;
            target.status.silenceDuration = enemyEffects.silenceDuration;
        }

        if (enemyEffects.disarm)
        {
            target.status.disarm = true;
            target.status.disarmDuration = enemyEffects.disarmDuration;
        }
    }

    void SelfEffects(ChampStats myStats)
    {
        if (selfEffects.disarm)
        {
            myStats.status.disarm = true;
            myStats.status.disarmDuration = selfEffects.disarmDuration;
        }

        if (selfEffects.Invincible)
        {
            myStats.status.invincible = true;
            myStats.status.invincibleDuration = selfEffects.InvincibleDuration;
        }
    }
}

//[System.Serializable]
//public class SkillsList
//{
//    public string name;
//}
//
//[System.Serializable]
//public class SkillEnemyEffects
//{
//    public bool stun;
//    public float stunDuration;
//
//    public bool silence;
//    public float silenceDuration;
//
//    public bool disarm;
//    public float disarmDuration;
//
//    public bool ASIncrease;
//    public float ASIncreaseDuration;
//
//    public bool MSIncrease;
//    public float MSIncreaseDuration;
//
//    public bool ASSlow;
//    public float ASSlowDuration;
//
//    public bool MSSlow;
//    public float MSSlowDuration;
//
//    public bool Invincible;
//    public float InvincibleDuration;
//}
//
//[System.Serializable]
//public class SkillSelfEffects
//{
//    public bool stun;
//    public float stunDuration;
//
//    public bool silence;
//    public float silenceDuration;
//
//    public bool disarm;
//    public float disarmDuration;
//
//    public bool ASIncrease;
//    public float ASIncreaseDuration;
//
//    public bool MSIncrease;
//    public float MSIncreaseDuration;
//
//    public bool ASSlow;
//    public float ASSlowDuration;
//
//    public bool MSSlow;
//    public float MSSlowDuration;
//
//    public bool Invincible;
//    public float InvincibleDuration;
//}
