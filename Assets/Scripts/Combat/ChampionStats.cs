using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using static AttributeTypes;
using System.Collections.Generic;
using System.Linq;
using static Unity.VisualScripting.Member;

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

        public void Heal(float heal)
        {
            if (currentHealth + heal > maxHealth) currentHealth = maxHealth;
            else currentHealth += heal;
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
        public List<Buff> buffs = new List<Buff>();                                 //buffs and durations

        public BuffManager(ChampionStats stats, ChampionCombat combat, SimManager simManager)
        {
            this.stats = stats;
            this.combat = combat;
            simulationManager = simManager;
        }

        public void Update()                                      //check if any expired
        {
            buffs.ForEach(x => x.Update());
            buffs.Where(x => x.duration <= 0).ToList().ForEach(x => x.Kill());
            buffs.RemoveAll(x => x.duration <= 0);
        }

        public bool IsStunned()
        {
            return buffs.OfType<StunBuff>().Any();
        }

        public bool IsDisarmed()
        {
            return buffs.OfType<DisarmBuff>().Any();
        }
        public bool IsSilenced()
        {
            return buffs.OfType<SilenceBuff>().Any();
        }

        public float DamageRed()
        {
            return buffs.OfType<DamageRedPercentBuff>().Any() ? buffs.OfType<DamageRedPercentBuff>().Max(x => x.reduction) : 0f;
        }

        public bool Has4AsheQ()
        {
            return buffs.OfType<AsheQBuff>().Any() && buffs.OfType<AsheQBuff>().First().qstack == 4 ? true : false;
        }

        public bool IsFrosted()
        {
            return buffs.OfType<FrostedBuff>().Any();
        }

        public float FlurryDamage()
        {
            return buffs.OfType<FlurryBuff>().Any() ? buffs.OfType<FlurryBuff>().Max(x => x.flurry) : 0f;
        }
        public float DecisiveStrikeDamage()
        {
            return buffs.OfType<DecisiveStrikeBuff>().Any() ? buffs.OfType<DecisiveStrikeBuff>().Max(x => x.strike) : 0f;
        }

        public float Shield()
        {
            return buffs.OfType<ShieldBuff>().Any() ? buffs.OfType<ShieldBuff>().Sum(x => x.shield) : 0f;
        }

        public bool CanAA()
        {
            return !buffs.OfType<CantAABuff>().Any();
        }

        public bool HasDeathbringerStance()
        {
            return buffs.OfType<DeathBringerStanceBuff>().Any();
        }

        public abstract class Buff
        {
            public float duration;
            public string source;
            protected BuffManager manager;
            protected Buff(BuffManager manager)
            {
                this.manager = manager;
            }

            public abstract void Update();
            public abstract void Kill();
        }

        public class StunBuff : Buff
        {
            public StunBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration * (100 - manager.stats.tenacity) / 100;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Got Stunned By {source} For {base.duration:F3} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }

            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Stunned By {source}!");
            }
        }

        public class SilenceBuff : Buff
        {
            public SilenceBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration * (100 - manager.stats.tenacity) / 100;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Got Silenced By {source} For {base.duration:F3} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }

            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Silenced By {source}!");
            }
        }

        public class DisarmBuff : Buff
        {
            public DisarmBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration * (100 - manager.stats.tenacity) / 100;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Got Disarmed By {source} For {base.duration:F3} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }

            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Disarmed By {source}!");
            }
        }

        public class DamageRedPercentBuff : Buff
        {
            public float reduction;
            public DamageRedPercentBuff(float duration, BuffManager manager, string source, float reduction) : base(manager)
            {
                this.reduction = reduction;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Gained {reduction}% Percent Damage Reduction from {source} for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {reduction}% Percent Damage Reduction From {source}!");
            }
        }

        public class AttackSpeedBuff : Buff
        {
            public float attackSpeed;
            public AttackSpeedBuff(float duration, BuffManager manager, string source, float attackSpeed) : base(manager)
            {
                manager.combat.AttackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed + attackSpeed);
                this.attackSpeed = attackSpeed;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Gained {attackSpeed:F3} Attack Speed from {source} for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {attackSpeed} Extra Attack Speed From {source}!");
                manager.combat.AttackCooldown *= manager.stats.attackSpeed / (manager.stats.attackSpeed - attackSpeed);
                manager.stats.attackSpeed -= attackSpeed;
            }
        }
        public class AsheQBuff : Buff
        {
            public int qstack = 0;
            public AsheQBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                if (qstack != 4) qstack++;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Has {qstack} Ranger's Focus for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                if (qstack > 1)
                {
                    qstack--;
                    duration += 1;
                }

                manager.simulationManager.ShowText($"{manager.stats.name} Has 1 Less Ranger's Focus!");
            }
        }
        public class FrostedBuff : Buff
        {
            public FrostedBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Got Frosted By {source} For {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} is No Longer Frosted By {source}");
            }
        }
        public class FlurryBuff : Buff
        {
            public float flurry;
            public FlurryBuff(float duration, BuffManager manager, string source,float flurry) : base(manager)
            {
                this.flurry = flurry;
                base.duration = duration;
                base.source = source;
                manager.buffs.Remove(manager.buffs.OfType<AsheQBuff>().FirstOrDefault());
                manager.simulationManager.ShowText($"{manager.stats.name} Has Entered Flurry For {duration} Seconds It Will Deal Extra {flurry} Damage!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name}'s Flurry Ended!");
            }
        }
        public class DecisiveStrikeBuff : Buff
        {
            public float strike;
            public DecisiveStrikeBuff(float duration, BuffManager manager, string source,float strike) : base(manager)
            {
                this.strike = strike;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Has Decisive Strike For {duration} Seconds It Will Deal Extra {strike} Damage!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name}'s Decisive Strike Ended!");
            }        
        }
        public class TenacityBuff : Buff
        {
            public float tenacity;
            public TenacityBuff(float duration, BuffManager manager, string source,float tenacity) : base(manager)
            {
                this.tenacity = tenacity;
                manager.stats.tenacity = tenacity;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Gained {tenacity}% Percent Tenacity from {source} for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.stats.tenacity -= tenacity;
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {tenacity}% Percent Extra Tenacity from {source}!");
            }
        }
        public class ShieldBuff : Buff
        {
            public float shield;
            public ShieldBuff(float duration, BuffManager manager, string source,float shield) : base(manager)
            {
                this.shield = shield;
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Gained {shield} Shield from {source} for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {shield} Shield!");
            }
        }
        public class CantAABuff : Buff
        {
            public CantAABuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Cant Auto Attack for {duration} Seconds Because of {source}!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Can Auto Attack Now!");
            }
        }
        public class ArmorReductionBuff : Buff
        {
            public float reduction;
            public ArmorReductionBuff(float duration, BuffManager manager, string source, float reduction) : base(manager)
            {
                this.reduction = reduction;
                base.duration = duration;
                manager.stats.armor *= (100 - reduction) / 100;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Has {reduction}% Percent Reduced Armor for {duration} Seconds!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer {reduction}% Percent Reduced Armor From {source}!");
                manager.stats.armor *= 100 / (100 - reduction);
            }
        }
        public class DeathBringerStanceBuff : Buff
        {
            public DeathBringerStanceBuff(float duration, BuffManager manager, string source) : base(manager)
            {
                base.duration = duration;
                base.source = source;
                manager.simulationManager.ShowText($"{manager.stats.name} Has DeathBringer Stance!");
            }

            public override void Update()
            {
                duration -= Time.deltaTime;
            }
            public override void Kill()
            {
                manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer DeathBringer Stance!");
            }
        }
    }
}