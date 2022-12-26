using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Simulator.Combat
{
    public class ChampionStats : MonoBehaviour
    {
        [SerializeField] public ChampionCombat MyCombat;                           //to update attack speed buff

        [SerializeField] private TextMeshProUGUI[] staticStatsUI;                   //ui elements that stay same in combat
        [SerializeField] private TextMeshProUGUI currentHP;                         //dynamic hp ui element that gets updated every frame
        [HideInInspector] private SimManager simulationManager;                     //cached singleton instance of simulation manager used for outputing info

        [HideInInspector] public BuffManager buffManager;                    //class to manage buffs
        [HideInInspector] public PassiveList passiveSkill;
        [HideInInspector] public List<SkillList> wSkill = new List<SkillList>();
        [HideInInspector] public List<SkillList> eSkill = new List<SkillList>();
        [HideInInspector] public List<SkillList> rSkill = new List<SkillList>();
        [HideInInspector] public List<SkillList> qSkill = new List<SkillList>();
        [HideInInspector] public float qCD, wCD, eCD, rCD, pCD;                                          //cooldown of skills
        [HideInInspector] public int qLevel, wLevel, eLevel, rLevel;                                          //levels of skills


        [HideInInspector] public int level;                                            //stats of champion
        public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed, tenacity = 0, lifesteal = 0, grievouswounds = 0;
        [HideInInspector] public float baseHealth, baseAD, baseArmor, baseSpellBlock, baseAttackSpeed;
        [HideInInspector] public float bonusAD, bonusHP, bonusAS, bonusArmor, bonusSpellBlock;
        [HideInInspector] public float critStrikeChance = 0, critStrikeDamage = 1.75f;
        [HideInInspector] public float armorPenetrationFlat = 0, armorPenetrationPercent = 0;
        [HideInInspector] public float magicPenetrationFlat = 0, magicPenetrationPercent = 0;
        [HideInInspector] public float omniVamp = 0, physicalVamp = 0, spellVamp = 0;
        [HideInInspector] public float healShieldPower = 0;
        [HideInInspector] public float abilityHaste = 0, basicAbilityHaste = 0, ultimateHaste = 0, itemHaste = 0, sumSpellHaste = 0;
        [HideInInspector] public float hpRegen;

        public float PercentCurrentHealth => currentHealth > 0 ? currentHealth / maxHealth: 0;
        public float PercentMissingHealth => maxHealth - currentHealth > 0 ? (maxHealth - currentHealth) / maxHealth : 0;

        private void Start()
        {
            simulationManager = SimManager.Instance;
        }

        private void FixedUpdate()
        {
            qCD -= Time.fixedDeltaTime;            //update skills cooldown
            wCD -= Time.fixedDeltaTime;
            eCD -= Time.fixedDeltaTime;
            rCD -= Time.fixedDeltaTime;
            pCD -= Time.fixedDeltaTime;

            buffManager?.Update();                //update remaining duration of buffs

            currentHP.text = currentHealth.ToString();             //update health text
        }
        /// <summary>
        /// updates the unchanging elements of ui it is called while loading a new champion
        /// </summary>
        public void StaticUIUpdate()
        {
            gameObject.name = name;
            staticStatsUI[0].text = name;
            staticStatsUI[1].text = name;
            staticStatsUI[2].text = name;
            staticStatsUI[3].text = level.ToString();
            staticStatsUI[4].text = level.ToString();
            staticStatsUI[5].text = maxHealth.ToString();
            staticStatsUI[6].text = maxHealth.ToString();
            staticStatsUI[7].text = AD.ToString();
            staticStatsUI[8].text = AP.ToString();
            staticStatsUI[9].text = armor.ToString();
            staticStatsUI[10].text = spellBlock.ToString();
            staticStatsUI[11].text = attackSpeed.ToString();
            buffManager = new BuffManager(this, MyCombat, simulationManager);
        }

        public void Heal(float heal)
        {
            if (currentHealth + heal > maxHealth) currentHealth = maxHealth;
            else currentHealth += heal;
        }
    }
}