using TMPro;
using UnityEngine;

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

    public int UseSkill(int level, Simulator.Combat.ChampionStats myStats, Simulator.Combat.ChampionStats target)
    {
        if (output = null) output = SimManager.Instance.outputText;
        int damage = 0;

        switch (championName)
        {
            case "Akali":
                damage = (int)Mathf.Round(182 + myStats.AP * 0.55f);
                damage = (int)Mathf.Round((target.maxHealth * (percentMaxTargetMaxHealth + ((Mathf.Round((myStats.AP / 100))) * 12) / 10)) / 100);
                damage -= (int)target.spellBlock;
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + maxHeal + "\n\n";
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
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + maxHeal + "\n\n";
                break;

            case "Fiora":
                damage = (int)Mathf.Round((target.maxHealth * (percentMaxTargetMaxHealth + ((Mathf.Round(((myStats.AD - myStats.baseAD) / 100))) * 55) / 10)) / 100);
                myStats.currentHealth += maxHeal;
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + maxHeal + "\n\n";
                break;
            case "Olaf":
                float ASBase = 36.47f;
                float ASGrowth = 3.53f;
                float TotalAS = ASBase + (level * ASGrowth);
                float BaseADVamp = 7;
                float TotalADVamp = BaseADVamp + level;
                break;

            case "Gangplank":
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " dealing " + damage.ToString() + " true damage \n\n";
                break;

            case "Aatrox":
                int postdamage = (int)Mathf.Round(target.maxHealth * (12f / 100));
                damage = (int)Mathf.Round(postdamage * (100 / (100 + target.armor)));
                myStats.currentHealth += postdamage;
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " Damage: " + damage.ToString() + " Heal: " + postdamage + "\n\n";
                break;

            case "Riven":
                damage = (int)Mathf.Round(myStats.AD * 0.6f);
                output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                break;

            case "Mordekaiser":
                if (level == 19)
                {
                    damage = (int)15.2f;
                    damage += (int)Mathf.Round(myStats.AP * 0.3f);
                    damage += (int)Mathf.Round(target.maxHealth * 0.05f);
                    output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                }
                else
                {
                    damage = (int)Mathf.Round(myStats.AP * 0.4f);
                    output.text += "[PASSIVE] " + myStats.name + " " + myStats.passiveSkill.skillName + " is triggered dealing " + damage.ToString() + " damage. \n\n";
                }
                break;
        }
        return damage;
    }

    void EnemyEffects(Simulator.Combat.ChampionStats target)
    {
        if (enemyEffects.stun)
        {
        }

        if (enemyEffects.silence)
        {
        }

        if (enemyEffects.disarm)
        {
        }
    }

    void SelfEffects(Simulator.Combat.ChampionStats myStats)
    {
        if (selfEffects.disarm)
        {
        }

        if (selfEffects.Invincible)
        {
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
