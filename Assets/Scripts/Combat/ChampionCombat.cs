using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct AutoAttackReturn
{
    public float damage;
    public bool isCrit;
}

namespace Simulator.Combat
{
    public class ChampionCombat : MonoBehaviour, IExecuteQ, IExecuteW, IExecuteE, IExecuteR, IExecuteA
    {
        public static string[] indexSkillMap = { "Q", "W", "E", "R", "P", "A" };

        [SerializeField] public ChampionStats myStats;
        [SerializeField] public ChampionStats targetStats;
        [SerializeField] public ChampionCombat targetCombat;
        [SerializeField] public ChampionUI myUI;
        [SerializeField] public SimManager simulationManager;

        [HideInInspector] public float attackCooldown;
        [HideInInspector] public List<Check> checksQ = new();
        [HideInInspector] public List<Check> checksW = new();
        [HideInInspector] public List<Check> checksE = new();
        [HideInInspector] public List<Check> checksR = new();
        [HideInInspector] public List<Check> checksA = new();
        [HideInInspector] public List<Check> checkTakeDamage = new();
        [HideInInspector] public List<Check> checkTakeDamagePostMitigation = new();
        [HideInInspector] public List<Check> castingCheck = new();
        [HideInInspector] public Check autoattackcheck;
        [HideInInspector] public List<string> qKeys = new();
        [HideInInspector] public List<string> wKeys = new();
        [HideInInspector] public List<string> eKeys = new();
        [HideInInspector] public List<string> rKeys = new();
        public List<Pet> pets = new();
        public CastLog myCastLog;

        public float aSum, hSum, qSum, wSum, eSum, rSum, pSum;
        protected string[] combatPrio;
        public bool isCasting = false;

        public SkillList QSkill(int index = 0) => myStats.qSkill[index];
        public SkillList WSkill(int index = 0) => myStats.wSkill[index];
        public SkillList ESkill(int index = 0) => myStats.eSkill[index];
        public SkillList RSkill(int index = 0) => myStats.rSkill[index];
        public BuffManager MyBuffManager => myStats.buffManager;
        public BuffManager TargetBuffManager => targetStats.buffManager;

        public delegate void CombatEvent();
        public event CombatEvent OnAutoAttack;
        public event CombatEvent OnAbilityHit;

        public delegate void HealEvent(float heal);
        public event HealEvent OnHeal;

        public delegate void PreDamageEvent(float damage);
        public event PreDamageEvent OnPreDamage;

        protected virtual void Start()
        {
            simulationManager = SimManager.Instance;
            myStats = GetComponent<ChampionStats>();
            myUI = GetComponent<ChampionUI>();
        }

        public virtual void CombatUpdate()
        {
            CheckPassive();

            if (!isCasting)
            {
                CheckSkills();
            }

            attackCooldown -= Time.fixedDeltaTime;
        }

        protected virtual void CheckPassive()
        {
            if (myStats.passiveSkill.inactive || myStats.pCD > 0) return;
        }

        private void CheckSkills()
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(ExecuteSkillIfReady(combatPrio[i]));
            }
        }

        public virtual IEnumerator StartCastingAbility(float castTime)
        {
            CheckForAbilityControl(castingCheck);
            isCasting = true;
            yield return new WaitForSeconds(castTime);
            isCasting = false;
        }

        public float UpdateTotalDamage(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level, string skillKey, float damageModifier = 1, SkillComponentTypes skillComponentTypes = SkillComponentTypes.None, string[] buffNames = null)
        {
            float damageGiven = targetCombat.TakeDamage(new Damage(damageModifier * skill.UseSkill(level, skillKey, myStats, targetStats), skill.skillDamageType, skillComponentTypes, buffNames), skill.basic.name);
            if (damageGiven <= 0) return damageGiven;
            simulationManager.AddDamageLog(new DamageLog(myStats.name, skill.basic.name, damageGiven, simulationManager.timer, indexSkillMap[totalDamageTextIndex]));
            totalDamage += damageGiven;
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
            return damageGiven;
        }

        public float UpdateAbilityTotalDamageSylas(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level, string skillKey, float damageModifier = 1, SkillComponentTypes skillComponentTypes = SkillComponentTypes.None, string[] buffNames = null)
        {
            float damageGiven = TakeDamage(new Damage(damageModifier * skill.SylasUseSkill(level, skillKey, targetStats, myStats), skill.skillDamageType, skillComponentTypes, buffNames), skill.basic.name);
            if (damageGiven <= 0) return damageGiven;
            totalDamage += damageGiven;
            targetCombat.myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
            simulationManager.AddDamageLog(new DamageLog(targetStats.name, skill.basic.name, damageGiven, simulationManager.timer, indexSkillMap[totalDamageTextIndex]));
            return damageGiven;
        }

        public float UpdateTotalDamage(ref float totalDamage, int totalDamageTextIndex, Damage damage, string skillName)
        {
            float damageGiven = targetCombat.TakeDamage(damage, skillName);
            if (damageGiven <= 0) return 0;
            simulationManager.AddDamageLog(new DamageLog(myStats.name, skillName, damageGiven, simulationManager.timer, indexSkillMap[totalDamageTextIndex]));
            totalDamage += damageGiven;
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
            return damageGiven;
        }

        public void UpdateAbilityTotalDamageSylas(ref float totalDamage, int totalDamageTextIndex, Damage damage, string skillName)
        {
            float damageGiven = TakeDamage(damage, skillName);
            if (damageGiven <= 0) return;
            simulationManager.AddDamageLog(new DamageLog(targetStats.name, skillName, damageGiven, simulationManager.timer, indexSkillMap[totalDamageTextIndex]));
            totalDamage += damageGiven;
            targetCombat.myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        public void UpdateTotalHeal(ref float totalHeal, SkillList skill, int level, string skillKey, float healModifier = 1)
        {
            float healGiven = HealHealth(healModifier * skill.UseSkill(level, skillKey, myStats, targetStats) * (100 - myStats.grievouswounds) / 100, skill.basic.name);
            if (healGiven <= 0) return;
            simulationManager.AddHealLog(new HealLog(myStats.name, skill.basic.name, healGiven, simulationManager.timer));
            totalHeal += healGiven;
            myUI.healSum.text = totalHeal.ToString();
        }
        public void UpdateTotalHealSylas(ref float totalHeal, SkillList skill, int level, string skillKey)
        {
            float healGiven = targetCombat.HealHealth(skill.UseSkill(level, skillKey, targetStats, myStats) * (100 - targetStats.grievouswounds) / 100, skill.basic.name);
            if (healGiven <= 0) return;
            simulationManager.AddHealLog(new HealLog(targetStats.name, skill.basic.name, healGiven, simulationManager.timer));
            totalHeal += healGiven;
            targetCombat.myUI.healSum.text = totalHeal.ToString();
        }

        public void UpdateTotalHeal(ref float totalHeal, float heal, string skillName)
        {
            float healGiven = HealHealth(heal * (100 - targetStats.grievouswounds) / 100, skillName);
            if (healGiven <= 0) return;
            simulationManager.AddHealLog(new HealLog(myStats.name, skillName, healGiven, simulationManager.timer));
            totalHeal += healGiven;
            myUI.healSum.text = totalHeal.ToString();
        }

        public void UpdateTotalHealSylas(ref float totalHeal, float heal, string skillName)
        {
            float healGiven = targetCombat.HealHealth(heal * (100 - targetStats.grievouswounds) / 100, skillName);
            if (healGiven <= 0) return;
            simulationManager.AddHealLog(new HealLog(targetStats.name, skillName, healGiven, simulationManager.timer));
            totalHeal += healGiven;
            targetCombat.myUI.healSum.text = totalHeal.ToString();
        }

        public virtual IEnumerator ExecuteQ()
        {
            if (myStats.qLevel == -1) yield break;
            if (!CheckForAbilityControl(checksQ)) yield break;

            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            UpdateTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
            myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        }

        public virtual IEnumerator ExecuteW()
        {
            if (myStats.wLevel == -1) yield break;
            if (!CheckForAbilityControl(checksW)) yield break;

            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
            myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        }

        public virtual IEnumerator ExecuteE()
        {
            if (myStats.eLevel == -1) yield break;
            if (!CheckForAbilityControl(checksE)) yield break;

            yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
            UpdateTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
            myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
        }

        public virtual IEnumerator ExecuteR()
        {
            if (myStats.rLevel == -1) yield break;
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
            UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
            myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        }

        public virtual IEnumerator HijackedR(int skillLevel)
        {
            yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
            UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
            targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        }

        public virtual IEnumerator ExecuteA()
        {
            if (!CheckForAbilityControl(checksA)) yield break;

            yield return StartCoroutine(StartCastingAbility(0.1f));
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal, SkillComponentTypes.OnHit | SkillComponentTypes.Dodgeable | SkillComponentTypes.Blockable | SkillComponentTypes.Blindable));
        }

        protected virtual IEnumerator ExecuteSkillIfReady(string skill)
        {
            switch (skill)
            {
                case "Q":
                    yield return ExecuteQ();
                    break;
                case "W":
                    yield return ExecuteW();
                    break;
                case "E":
                    yield return ExecuteE();
                    break;
                case "R":
                    yield return ExecuteR();
                    break;
                case "A":
                    yield return ExecuteA();
                    break;
                default:
                    break;
            }
        }

        protected AutoAttackReturn AutoAttack(Damage damage)
        {
            OnAutoAttack?.Invoke();

            AutoAttackReturn autoAttackReturn = new()
            {
                isCrit = false
            };

            if (Random.Range(0, 1f) <= myStats.critStrikeChance)
            {
                damage.value *= myStats.critStrikeDamage;
                autoAttackReturn.isCrit = true;
            }

            if (damage.value < 0)
                damage.value = 0;

            if (autoattackcheck != null) damage = autoattackcheck.Control(damage);

            float damageGiven = targetCombat.TakeDamage(damage, $"{myStats.name}'s Auto Attack");
            aSum += damageGiven;
            hSum += HealHealth(damageGiven * myStats.lifesteal, "Lifesteal");
            autoAttackReturn.damage = damageGiven;

            simulationManager.AddDamageLog(new DamageLog(myStats.name, $"{myStats.name}'s Auto Attack", damageGiven, simulationManager.timer, "A"));
            myUI.aaSum.text = aSum.ToString();
            myUI.healSum.text = hSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;

            return autoAttackReturn;
        }

        public float TakeDamage(Damage damage, string source)
        {
            damage = CheckForDamageControl(checkTakeDamage, damage);

            if (damage.damageType == SkillDamageType.Phyiscal) damage.value = (int)(damage.value * 100 / (100 + myStats.armor));
            else if (damage.damageType == SkillDamageType.Spell) damage.value = (int)(damage.value * 100 / (100 + myStats.spellBlock));
            else if (damage.damageType == SkillDamageType.True) damage.value = (int)damage.value;

            damage.value = (int)CheckForDamageControl(checkTakeDamagePostMitigation, damage).value;

            if (damage.value <= 0) return damage.value;

            myStats.currentHealth -= damage.value;
            simulationManager.ShowText($"{myStats.name} Took {damage.value} Damage From {source}!");

            CheckDeath();

            return damage.value;
        }

        protected virtual void CheckDeath()
        {
            if (myStats.currentHealth <= 0)
                EndBattle(targetStats.name);
        }

        public float HealHealth(float heal, string source)
        {
            if (heal <= 0) return 0;

            //add checks here


            if (myStats.currentHealth + heal > myStats.maxHealth) myStats.currentHealth = myStats.maxHealth;
            else myStats.currentHealth += (int)heal;

            simulationManager.ShowText($"{myStats.name} Took {heal} Heal From {source}!");

            return heal;
        }

        protected void EndBattle(string winner)
        {
            Time.timeScale = 0;
            SimManager.Instance.isSimulating = false;
            simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
            StopAllCoroutines();
            targetCombat.StopAllCoroutines();
            simulationManager.StopCoroutine(simulationManager.TakeSnapShot());
            simulationManager.snaps.Add(new SnapShot("", new ChampionSnap(simulationManager.champStats[0].name, simulationManager.champStats[0].PercentCurrentHealth * 100), new ChampionSnap(simulationManager.champStats[1].name, simulationManager.champStats[1].PercentCurrentHealth * 100), simulationManager.timer));
            APIRequestManager.Instance.SendOutputToJS(new WebData(simulationManager.snaps.ToArray(), simulationManager.damagelogs.ToArray(), simulationManager.heallogs.ToArray(), simulationManager.bufflogs.ToArray(), winner, simulationManager.timer, new CastLog[] {simulationManager.castlog1, simulationManager.castlog2}, simulationManager.tooltips.ToArray(), simulationManager.championStats.ToArray()));
        }

        public void UpdateTarget(int index)
        {
            targetStats = SimManager.Instance.champStats[index];
            targetCombat = targetStats.MyCombat;
        }

        public virtual void UpdatePriorityAndChecks()
        {
            myUI.combatPriority.text = string.Join(", ", combatPrio);
        }

        protected bool CheckForAbilityControl(List<Check> checks)
        {
            foreach (Check item in checks)
                if (!item.Control()) return false;

            return true;
        }

        protected Damage CheckForDamageControl(List<Check> checks, Damage damage)
        {
            foreach (Check item in checks)
                damage = item.Control(damage);

            return damage;
        }

        public virtual void StopChanneling(string uniqueKey) { }

        public virtual IEnumerator StartHPRegeneration()
        {
            yield return new WaitForSeconds(0.5f);
            UpdateTotalHeal(ref hSum, myStats.hpRegen * 0.1f, "Health Regeneration");
            StartCoroutine(StartHPRegeneration());
        }

        public virtual Tooltip UpdateTooltip()
        {
            return null;
        }

        public virtual ChampionStatsExternal UpdateStatsExternal()
        {
            return null;
        }
    }
}

public class Damage
{
    public float value;
    public SkillDamageType damageType;
    public SkillComponentTypes skillComponentType;
    public string[] buffNames;

    public Damage(float value, SkillDamageType damageType, SkillComponentTypes skillComponentType = SkillComponentTypes.None, string[] buffNames = null)
    {
        this.value = value;
        this.damageType = damageType;
        this.skillComponentType = skillComponentType;
        this.buffNames = buffNames;
    }
}