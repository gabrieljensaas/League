using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using static AttributeTypes;

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
        [HideInInspector] public SkillList qSkill;
        [HideInInspector] public SkillList wSkill;
        [HideInInspector] public SkillList eSkill;
        [HideInInspector] public SkillList rSkill;
        [HideInInspector] public float qCD, wCD, eCD, rCD, pCD;                                          //cooldown of skills


        [HideInInspector] public int level;                                            //stats of champion
        [HideInInspector] public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed, tenacity = 0f;
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
    }
    /// <summary>
    /// stats which states champion status and buffs/debuffs it has
    /// </summary>
    public class BuffManager
    {
        private ChampionStats stats;
        private ChampionCombat combat;
        private SimManager simulationManager;
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
        public float Flurry;
        public float FlurryDuration;
        public float DecisiveStrike;
        public float DecisiveStrikeDuration;
        public float Tenacity;
        public float TenacityDuration;
        public float Shield;
        public float ShieldDuration;
        public bool CantAA;
        public float CantAADuration;
        public float ArmorReduction;
        public float ArmorReductionDuration;

        public BuffManager(ChampionStats stats, ChampionCombat combat, SimManager simManager)
        {
            this.stats = stats;
            this.combat = combat;
            simulationManager = simManager;
        }

        public void Update()                                      //check if any expired
        {
            if (Stunned)
            {
                StunnedDuration -= Time.deltaTime;
                if (StunnedDuration <= 0)
                {
                    Stunned = false;
                    simulationManager.ShowText($"{stats.name} is No Longer Stunned!");
                }
            }

            if (Silenced)
            {
                SilencedDuration -= Time.deltaTime;
                if (SilencedDuration <= 0)
                {
                    Silenced = false;
                    simulationManager.ShowText($"{stats.name} is No Longer Silenced!");
                }
            }

            if (Disarmed)
            {
                DisarmedDuration -= Time.deltaTime;
                if(DisarmedDuration <= 0)
                {
                    Disarmed = false;
                    simulationManager.ShowText($"{stats.name} is No Longer Disarmed!");
                }
            }

            if (Invincible)
            {
                InvincibleDuration -= Time.deltaTime;
                if(InvincibleDuration <= 0)
                {
                    Invincible = false;
                    simulationManager.ShowText($"{stats.name} is No Longer Invincible!");
                }
            }

            if (DamageReductionFlat != 0)
            {
                DamageReductionFlatDuration -= Time.deltaTime;
                if(DamageReductionFlatDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} Has No Longer {DamageReductionFlat} Damage Reduction!");
                    DamageReductionFlat = 0;
                }
            }

            if (DamageReductionPercent != 0)
            {
                DamageReductionPercentDuration -= Time.deltaTime;
                if( DamageReductionPercentDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} Has No Longer {DamageReductionPercent}% Percent Damage Reduction!");
                    DamageReductionPercent = 0;
                }
            }

            if(AttackSpeed != 0)
            {
                AttackSpeedDuration -= Time.deltaTime;
                if (AttackSpeedDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} Has No Longer {AttackSpeed} Extra Attack Speed!");
                    combat.AttackCooldown *= stats.attackSpeed / (stats.attackSpeed - AttackSpeed);
                    stats.attackSpeed -= AttackSpeed;
                    AttackSpeed = 0;
                }
            }

            if(AsheQBuff > 0)
            {
                AsheQBuffDuration -= Time.deltaTime;
                if (AsheQBuffDuration <= 0)
                {
                    AsheQBuff--;
                    simulationManager.ShowText($"{stats.name} Has 1 Less Ranger's Focus!");
                    AsheQBuffDuration = 1;
                }
            }

            if (Frosted)
            {
                FrostedDuration -= Time.deltaTime;
                if (FrostedDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} is No Longer Frosted!");
                    Frosted = false;
                }
            }

            if(Flurry > 0)
            {
                FlurryDuration -= Time.deltaTime;
                if (FlurryDuration <= 0)
                {
                    Flurry = 0f;
                    simulationManager.ShowText($"{stats.name}'s Flurry Ended!");
                }
            }

            if (DecisiveStrike != 0)
            {
                DecisiveStrike -= Time.deltaTime;
                if (DecisiveStrikeDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name}'s Decisive Strike Ended!");
                    DecisiveStrike = 0;
                }
            }

            if (Tenacity != 0)
            {
                TenacityDuration -= Time.deltaTime;
                if (TenacityDuration <= 0)
                {
                    stats.tenacity -= Tenacity;
                    simulationManager.ShowText($"{stats.name} Has No Longer {Tenacity}% Percent Extra Tenacity!");
                    Tenacity = 0;
                }
            }

            if(Shield != 0)
            {
                ShieldDuration -= Time.deltaTime;
                if(ShieldDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} Has No Longer {Shield} Shield!");
                    Shield = 0f;
                }
            }

            if (CantAA)
            {
                CantAADuration -= Time.deltaTime;
                if(CantAADuration <= 0)
                {
                    CantAA = false;
                    simulationManager.ShowText($"{stats.name} Can Auto Attack Now!");
                }
            }

            if (ArmorReduction != 0)
            {
                ArmorReductionDuration -= Time.deltaTime;
                if (ArmorReductionDuration <= 0)
                {
                    simulationManager.ShowText($"{stats.name} Has No Longer {ArmorReduction}% Percent Reduced Armor!");
                    stats.armor *= 100 / (100 - ArmorReduction);
                    ArmorReduction = 0;
                }
            }
        }
        /// <summary>
        /// Add buff with string name ,duration and value of the said buff if there it is a bool value doesnt matter
        /// </summary>
        /// <param name="buffType"></param>
        /// <param name="duration"></param>
        public void AddBuff(string buffType, float duration, float buffvalue, string source)
        {
            switch (buffType)
            {
                case "Stunned":
                    Stunned = true;
                    StunnedDuration = duration * (100 - stats.tenacity) / 100;
                    simulationManager.ShowText($"{stats.name} Got Stunned By {source} For {StunnedDuration:F3} Seconds!");
                    break;
                case "Silenced":
                    Silenced = true;
                    SilencedDuration = duration * (100 - stats.tenacity) / 100;
                    simulationManager.ShowText($"{stats.name} Got Silenced By {source} For {SilencedDuration:F3} Seconds!");
                    break;
                case "Disarmed":
                    Disarmed = true;
                    DisarmedDuration = duration * (100 - stats.tenacity) / 100;
                    simulationManager.ShowText($"{stats.name} Got Disarmed By {source} For {DisarmedDuration:F3} Seconds!");
                    break;
                case "Invincible":
                    Invincible = true;
                    InvincibleDuration = duration;
                    simulationManager.ShowText($"{stats.name} Gained Invincibility By {source} For {InvincibleDuration} Seconds!");
                    break;
                case "DamageReductionFlat":
                    DamageReductionFlat = buffvalue;
                    DamageReductionFlatDuration = duration;
                    simulationManager.ShowText($"{stats.name} Gained {buffvalue} Damage Reduction from {source} for {duration} Seconds!");
                    break;
                case "DamageReductionPercent":
                    DamageReductionPercent = buffvalue;
                    DamageReductionPercentDuration = duration;
                    simulationManager.ShowText($"{stats.name} Gained {buffvalue}% Percent Damage Reduction from {source} for {duration} Seconds!");
                    break;
                case "AttackSpeed":
                    combat.AttackCooldown *= stats.attackSpeed / (stats.attackSpeed + buffvalue);
                    AttackSpeed = buffvalue;
                    AttackSpeedDuration = duration;
                    stats.attackSpeed += buffvalue;
                    simulationManager.ShowText($"{stats.name} Gained {buffvalue:F3} Attack Speed from {source} for {duration} Seconds!");
                    break;
                case "AsheQBuff":
                    if (AsheQBuff != 4) AsheQBuff++;
                    AsheQBuffDuration = duration;
                    simulationManager.ShowText($"{stats.name} Has {AsheQBuff} Ranger's Focus for {duration} Seconds!");
                    break;
                case "Frosted":
                    Frosted = true;
                    FrostedDuration = duration;
                    simulationManager.ShowText($"{stats.name} Got Frosted By {source} For {FrostedDuration} Seconds!");
                    break;
                case "Flurry":
                    Flurry = buffvalue;
                    FlurryDuration = duration;
                    simulationManager.ShowText($"{stats.name} Has Entered Flurry For {FlurryDuration} Seconds It Will Deal Extra {Flurry} Damage!");
                    break;
                case "DecisiveStrike":
                    DecisiveStrike = buffvalue;
                    DecisiveStrikeDuration = duration;
                    simulationManager.ShowText($"{stats.name} Has Decisive Strike For {DecisiveStrikeDuration} Seconds It Will Deal Extra {buffvalue} Damage!");
                    break;
                case "Tenacity":
                    Tenacity = buffvalue;
                    TenacityDuration = duration;
                    stats.tenacity += buffvalue;
                    simulationManager.ShowText($"{stats.name} Gained {buffvalue}% Percent Tenacity from {source} for {duration} Seconds!");
                    break;
                case "Shield":
                    Shield = buffvalue;
                    ShieldDuration = duration;
                    simulationManager.ShowText($"{stats.name} Gained {buffvalue} Shield from {source} for {duration} Seconds!");
                    break;
                case "CantAA":
                    CantAA = true;
                    CantAADuration = duration;
                    simulationManager.ShowText($"{stats.name} Cant Auto Attack for {duration} Seconds!");
                    break;
                case "ArmorReduction":
                    ArmorReduction = buffvalue;
                    ArmorReductionDuration = duration;
                    stats.armor *= (100-buffvalue) / 100;
                    simulationManager.ShowText($"{stats.name} Has {buffvalue}% Percent Reduced Armor for {duration} Seconds!");
                    break;
                default:
                    break;
            }
        }
    }
}