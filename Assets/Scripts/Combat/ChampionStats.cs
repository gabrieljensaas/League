using UnityEngine;
using TMPro;
using static AttributeTypes;

namespace Simulator.Combat
{
    public class ChampionStats : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] staticStatsUI;                   //ui elements that stay same in combat
        [SerializeField] private TextMeshProUGUI currentHP;                         //dynamic hp ui element that gets updated every frame

        [HideInInspector] public BuffManager buffManager;                    //class to manage buffs
        [HideInInspector] public PassiveList passiveSkill;
        [HideInInspector] public SkillList qSkill;
        [HideInInspector] public SkillList wSkill;
        [HideInInspector] public SkillList eSkill;
        [HideInInspector] public SkillList rSkill;
        [HideInInspector] public float qCD, wCD, eCD, rCD, pCD;                                          //cooldown of skills


        [HideInInspector] public int level;                                            //stats of champion
        [HideInInspector] public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed;
        [HideInInspector] public float baseHealth, baseAD, baseAP, baseArmor, baseSpellBlock, baseAttackSpeed;
        private void Start()
        {
            buffManager = new BuffManager(this);
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
    }
    /// <summary>
    /// stats which states champion status and buffs/debuffs it has
    /// </summary>
    public class BuffManager
    {
        private ChampionStats stats;
        public bool Stunned;                                  //buffs and durations
        public float StunnedDuration;
        public bool Silenced;
        public float SilencedDuration;
        public bool Disarmed;
        public float DisarmedDuration;
        public bool Invincible;
        public float InvincibleDuration;
        public float DamageReductionFlat;
        public float DamageReductionFlatDuration;
        public float DamageReductionPercent;
        public float DamageReductionPercentDuration;
        public float AttackSpeed;
        public float AttackSpeedDuration;
        public int AsheQBuff;
        public float AsheQBuffDuration;
        public bool Frosted;
        public float FrostedDuration;

        public BuffManager(ChampionStats stats)
        {
            this.stats = stats;
        }

        public void Update()                                      //check if any expired
        {
            if (Stunned)
            {
                StunnedDuration -= Time.deltaTime;
                if (StunnedDuration <= 0) Stunned = false;
            }

            if (Silenced)
            {
                SilencedDuration -= Time.deltaTime;
                if (SilencedDuration <= 0) Silenced = false;
            }

            if (Disarmed)
            {
                DisarmedDuration -= Time.deltaTime;
                if(DisarmedDuration <= 0) Disarmed = false;
            }

            if (Invincible)
            {
                InvincibleDuration -= Time.deltaTime;
                if(InvincibleDuration <= 0) Invincible = false;
            }

            if (DamageReductionFlat != 0)
            {
                DamageReductionFlatDuration -= Time.deltaTime;
                if(DamageReductionFlatDuration <= 0) DamageReductionFlat = 0;
            }

            if (DamageReductionPercent != 0)
            {
                DamageReductionPercentDuration -= Time.deltaTime;
                if( DamageReductionPercentDuration <= 0) DamageReductionPercent = 0;
            }

            if(AttackSpeed != 0)
            {
                AttackSpeedDuration -= Time.deltaTime;
                if (AttackSpeedDuration <= 0)
                {
                    stats.attackSpeed -= AttackSpeed;
                    AttackSpeed = 0;
                }
            }

            if(AsheQBuff > 0)
            {
                AsheQBuffDuration -= Time.deltaTime;
                if(AsheQBuffDuration <= 0) AsheQBuff--;
            }

            if (Frosted)
            {
                FrostedDuration -= Time.deltaTime;
                if (FrostedDuration <= 0) Frosted = false;
            }
        }
        /// <summary>
        /// Add buff with string name ,duration and value of the said buff if there it is a bool value doesnt matter
        /// </summary>
        /// <param name="buffType"></param>
        /// <param name="duration"></param>
        public void AddBuff(string buffType, float duration, float buffvalue)
        {
            switch (buffType)
            {
                case "Stunned":
                    Stunned = true;
                    StunnedDuration = duration;
                    break;
                case "Silenced":
                    Silenced = true;
                    SilencedDuration = duration;
                    break;
                case "Disarmed":
                    Disarmed = true;
                    DisarmedDuration = duration;
                    break;
                case "Invincible":
                    Invincible = true;
                    InvincibleDuration = duration;
                    break;
                case "DamageReductionFlat":
                    DamageReductionFlat = buffvalue;
                    DamageReductionFlatDuration = duration;
                    break;
                case "DamageReductionPercent":
                    DamageReductionPercent = buffvalue;
                    DamageReductionPercentDuration = duration;
                    break;
                case "AttackSpeed":
                    AttackSpeed = buffvalue;
                    AttackSpeedDuration = duration;
                    stats.attackSpeed += buffvalue;
                    break;
                case "AsheQBuff":
                    AsheQBuff++;
                    AsheQBuffDuration = duration;
                    break;
                case "Frosted":
                    Frosted = true;
                    FrostedDuration = duration;
                    break;
                default:
                    break;
            }
        }
    }
}