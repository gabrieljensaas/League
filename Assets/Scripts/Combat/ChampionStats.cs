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
        [HideInInspector] public SkillList[] qSkill = new SkillList[5];
        [HideInInspector] public SkillList[] wSkill = new SkillList[5];
        [HideInInspector] public SkillList[] eSkill = new SkillList[5];
        [HideInInspector] public SkillList[] rSkill = new SkillList[5];
        [HideInInspector] public float qCD, wCD, eCD, rCD, pCD;                                          //cooldown of skills
        [HideInInspector] public int qLevel, wLevel, eLevel, rLevel;                                          //levels of skills


        [HideInInspector] public int level;                                            //stats of champion
        [HideInInspector] public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed, tenacity = 0, lifesteal = 0, grievouswounds = 0;
        [HideInInspector] public float baseHealth, baseAD, baseArmor, baseSpellBlock, baseAttackSpeed;
        [HideInInspector] public float bonusAD, bonusHP, bonusAS, bonusArmor, bonusSpellBlock;
        [HideInInspector] public float critStrikeChance = 0, critStrikeDamage = 1.75f;
        [HideInInspector] public float armorPenetrationFlat = 0, armorPenetrationPercent = 0;
        [HideInInspector] public float magicPenetrationFlat = 0, magicPenetrationPercent = 0;
        [HideInInspector] public float omniVamp = 0, physicalVamp = 0, spellVamp = 0;
        [HideInInspector] public float healShieldPower = 0;
        [HideInInspector] public float abilityHaste = 0, basicAbilityHaste = 0, ultimateHaste = 0, itemHaste = 0, sumSpellHaste = 0;
        [HideInInspector] public float hpRegen;

        public float PercentCurrentHealth => currentHealth / maxHealth;
        public float PercentMissingHealth => (maxHealth - currentHealth) / maxHealth;

        private void Start()
        {
            simulationManager = SimManager.Instance;
            buffManager = new BuffManager(this, MyCombat, simulationManager);
        }

        private void Update()
        {
            qCD -= Time.deltaTime;            //update skills cooldown
            wCD -= Time.deltaTime;
            eCD -= Time.deltaTime;
            rCD -= Time.deltaTime;
            pCD -= Time.deltaTime;

            buffManager.Update();                //update remaining duration of buffs

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
        }

        public void Heal(float heal)
        {
            if (currentHealth + heal > maxHealth) currentHealth = maxHealth;
            else currentHealth += heal;
        }
    }
}