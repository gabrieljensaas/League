using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Simulator.API;
using static SkillList;

namespace Simulator.Combat
{
    public class ChampionCombat : MonoBehaviour, IExecuteQ, IExecuteW, IExecuteE, IExecuteR, IExecuteA
    {
        [SerializeField] public ChampionStats myStats;
        [SerializeField] public ChampionStats targetStats;
        [SerializeField] public ChampionCombat targetCombat;
        [SerializeField] public ChampionUI myUI;
        [SerializeField] public SimManager simulationManager;

        [HideInInspector] public float attackCooldown;
        [HideInInspector] public List<Check> checksQ = new List<Check>();
        [HideInInspector] public List<Check> checksW = new List<Check>();
        [HideInInspector] public List<Check> checksE = new List<Check>();
        [HideInInspector] public List<Check> checksR = new List<Check>();
        [HideInInspector] public List<Check> checksA = new List<Check>();
        [HideInInspector] public List<Check> checkTakeDamageAA = new List<Check>();
        [HideInInspector] public List<Check> checkTakeDamage = new List<Check>();
        [HideInInspector] public Check autoattackcheck;
        protected List<Pet> pets = new List<Pet>();

        public float aSum, hSum, qSum, wSum, eSum, rSum, pSum;
        protected string[] combatPrio;
        public bool isCasting = false;

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

            attackCooldown -= Time.deltaTime;
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

        protected IEnumerator StartCastingAbility(float castTime)
        {
            isCasting = true;
            yield return new WaitForSeconds(castTime);
            isCasting = false;
        }

        protected void UpdateAbilityTotalDamage(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level, float damageModifier = 1)
        {
            totalDamage += targetCombat.TakeDamage(damageModifier * skill.UseSkill(level, myStats, targetStats), skill.basic.name, skill.skillDamageType);
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        public void UpdateAbilityTotalDamage(ref float totalDamage, int totalDamageTextIndex, float damage, string skillName, SkillDamageType skillDamageType)
        {
            totalDamage += targetCombat.TakeDamage(damage, skillName, skillDamageType);
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        protected void UpdateTotalHeal(ref float totalHeal, SkillList skill, int level)
        {
            totalHeal += HealHealth(skill.UseSkill(level, myStats, targetStats), skill.basic.name);
            myUI.healSum.text = totalHeal.ToString();
        }

        protected void UpdateTotalHeal(ref float totalHeal, float heal, string skillName)
        {
            totalHeal += HealHealth(heal, skillName);
            myUI.healSum.text = totalHeal.ToString();
        }

        public virtual IEnumerator ExecuteQ()
        {
            if (!CheckForAbilityControl(checksQ)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4);
            myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteW()
        {
            if (!CheckForAbilityControl(checksW)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4);
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteE()
        {
            if (!CheckForAbilityControl(checksE)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4);
            myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteR()
        {
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2);
            myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        }

        public virtual IEnumerator ExecuteA()
        {
            if (!CheckForAbilityControl(checksA)) yield break;

            yield return StartCoroutine(StartCastingAbility(0.1f));
            AutoAttack();
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

        protected void AutoAttack()
        {
            float damage = myStats.AD;
            if (damage < 0)
                damage = 0;

            if (autoattackcheck != null) damage = autoattackcheck.Control(damage);

            aSum += targetCombat.TakeDamage(damage, $"{myStats.name}'s Auto Attack", SkillDamageType.Phyiscal, true);
            hSum += HealHealth(damage * myStats.lifesteal, "Lifesteal");
            myUI.aaSum.text = aSum.ToString();
            myUI.healSum.text = hSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;
        }

        public float TakeDamage(float rawDamage, string source, SkillDamageType damageType, bool isAutoAttack = false)
        {
            if(!isAutoAttack)
                rawDamage = CheckForDamageControl(checkTakeDamage, rawDamage);
            else
                rawDamage = CheckForDamageControl(checkTakeDamageAA, rawDamage);

            int postMitigationDamage = 0;
            if (damageType == SkillDamageType.Phyiscal) postMitigationDamage = (int)(rawDamage * 100 / (100 + myStats.armor));
            else if (damageType == SkillDamageType.Spell) postMitigationDamage = (int)(rawDamage * 100 / (100 + myStats.spellBlock));
            else if (damageType == SkillDamageType.True) postMitigationDamage = (int)rawDamage;

            if (postMitigationDamage <= 0) return 0;

            myStats.currentHealth -= postMitigationDamage;
            simulationManager.ShowText($"{myStats.name} Took {postMitigationDamage} Damage From {source}!");

            CheckDeath();

            return postMitigationDamage;
        }

        protected virtual void CheckDeath()
        {
            if (myStats.currentHealth <= 0)
                EndBattle();
        }

        public float HealHealth(float health, string source)
        {
            if (health <= 0) return 0;

            //add checks here

            myStats.currentHealth += (int)health;
            if (myStats.currentHealth > myStats.maxHealth) myStats.currentHealth = myStats.maxHealth;

            simulationManager.ShowText($"{myStats.name} Took {health} Heal From {source}!");

            return health;
        }

        protected void EndBattle()
        {
            Time.timeScale = 1;
            SimManager.battleStarted = false;
            simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
            StopAllCoroutines();
            targetCombat.StopAllCoroutines();
            APIRequestManager.Instance.SendOutputToJS(simulationManager.output[0].text.Split("\n"));
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

        protected float CheckForDamageControl(List<Check> checks, float damage)
        {
            foreach (Check item in checks)
                damage = item.Control(damage);

            return damage;
        }

        public virtual void StopChanneling(string uniqueKey) { }
    }
}