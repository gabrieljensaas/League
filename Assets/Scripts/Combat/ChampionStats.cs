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


        [HideInInspector] public int level;                                            //stats of champion
        [HideInInspector] public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed, tenacity = 0f, lifesteal = 0f, grievouswounds = 0f;
        [HideInInspector] public float baseHealth, baseAD, baseAP, baseArmor, baseSpellBlock, baseAttackSpeed;
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