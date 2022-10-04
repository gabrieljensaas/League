using Simulator.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AutoAttackReturn
{
    public float damage;
    public bool isCrit;
}

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
        [HideInInspector] public List<Check> checksQ = new();
        [HideInInspector] public List<Check> checksW = new();
        [HideInInspector] public List<Check> checksE = new();
        [HideInInspector] public List<Check> checksR = new();
        [HideInInspector] public List<Check> checksA = new();
        [HideInInspector] public List<Check> checkTakeDamageAA = new();
        [HideInInspector] public List<Check> checkTakeDamageAbility = new();
        [HideInInspector] public List<Check> checkTakeDamageAAPostMitigation = new();
        [HideInInspector] public List<Check> checkTakeDamageAbilityPostMitigation = new();
        [HideInInspector] public Check autoattackcheck;
        [HideInInspector] public List<string> qKeys = new();
        [HideInInspector] public List<string> wKeys = new();
        [HideInInspector] public List<string> eKeys = new();
        [HideInInspector] public List<string> rKeys = new();
        public List<Pet> pets = new();

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

        public IEnumerator StartCastingAbility(float castTime)
        {
            isCasting = true;
            yield return new WaitForSeconds(castTime);
            isCasting = false;
        }

        public float UpdateAbilityTotalDamage(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level, string skillKey, float damageModifier = 1, SkillComponentTypes skillComponentTypes = SkillComponentTypes.None)
        {
            float damageGiven = targetCombat.TakeDamage(new Damage(damageModifier * skill.UseSkill(level, skillKey, myStats, targetStats), skill.skillDamageType, skillComponentTypes), skill.basic.name);
            totalDamage += damageGiven;
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
            return damageGiven;
        }

        public void UpdateAbilityTotalDamageSylas(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level, string skillKey, float damageModifier = 1, SkillComponentTypes skillComponentTypes = SkillComponentTypes.None)
        {
            totalDamage += TakeDamage(new Damage(damageModifier * skill.SylasUseSkill(level, skillKey, targetStats, myStats), skill.skillDamageType, skillComponentTypes), skill.basic.name);
            targetCombat.myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        public float UpdateAbilityTotalDamage(ref float totalDamage, int totalDamageTextIndex, Damage damage, string skillName)
        {
            float damageGiven = targetCombat.TakeDamage(damage, skillName);
            totalDamage += damageGiven;
            myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
            return damageGiven;
        }

        public void UpdateAbilityTotalDamageSylas(ref float totalDamage, int totalDamageTextIndex, Damage damage, string skillName)
        {
            totalDamage += TakeDamage(damage, skillName);
            targetCombat.myUI.abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        public void UpdateTotalHeal(ref float totalHeal, SkillList skill, int level, string skillKey)
        {
            totalHeal += HealHealth(skill.UseSkill(level, skillKey, myStats, targetStats) * (100 - myStats.grievouswounds) / 100, skill.basic.name);
            myUI.healSum.text = totalHeal.ToString();
        }
        public void UpdateTotalHealSylas(ref float totalHeal, SkillList skill, int level, string skillKey)
        {
            totalHeal += targetCombat.HealHealth(skill.UseSkill(level, skillKey, targetStats, myStats) * (100 - targetStats.grievouswounds) / 100, skill.basic.name);
            targetCombat.myUI.healSum.text = totalHeal.ToString();
        }

        public void UpdateTotalHeal(ref float totalHeal, float heal, string skillName)
        {
            totalHeal += HealHealth(heal * (100 - targetStats.grievouswounds) / 100, skillName);
            myUI.healSum.text = totalHeal.ToString();
        }

        public void UpdateTotalHealSylas(ref float totalHeal, float heal, string skillName)
        {
            totalHeal += targetCombat.HealHealth(heal * (100 - targetStats.grievouswounds) / 100, skillName);
            targetCombat.myUI.healSum.text = totalHeal.ToString();
        }

        public virtual IEnumerator ExecuteQ()
        {
            if (!CheckForAbilityControl(checksQ)) yield break;

            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
            myStats.qCD = QSkill().basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteW()
        {
            if (!CheckForAbilityControl(checksW)) yield break;

            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
            myStats.wCD = WSkill().basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteE()
        {
            if (!CheckForAbilityControl(checksE)) yield break;

            yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
            myStats.eCD = ESkill().basic.coolDown[4];
        }

        public virtual IEnumerator ExecuteR()
        {
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
            UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
            myStats.rCD = RSkill().basic.coolDown[2];
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
            AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
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

            float damageGiven = targetCombat.TakeDamage(damage, $"{myStats.name}'s Auto Attack", true);
            aSum += damageGiven;
            hSum += HealHealth(damageGiven * myStats.lifesteal, "Lifesteal");
            autoAttackReturn.damage = damageGiven;

            myUI.aaSum.text = aSum.ToString();
            myUI.healSum.text = hSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;

            return autoAttackReturn;
        }

        public float TakeDamage(Damage damage, string source, bool isAutoAttack = false)
        {
            if (!isAutoAttack)
                damage = CheckForDamageControl(checkTakeDamageAbility, damage);
            else
                damage = CheckForDamageControl(checkTakeDamageAA, damage);

            int postMitigationDamage = 0;
            if (damage.damageType == SkillDamageType.Phyiscal) postMitigationDamage = (int)(damage.value * 100 / (100 + myStats.armor));
            else if (damage.damageType == SkillDamageType.Spell) postMitigationDamage = (int)(damage.value * 100 / (100 + myStats.spellBlock));
            else if (damage.damageType == SkillDamageType.True) postMitigationDamage = (int)damage.value;

            if (!isAutoAttack)
                postMitigationDamage = (int)CheckForDamageControl(checkTakeDamageAbilityPostMitigation, damage).value;
            else
                postMitigationDamage = (int)CheckForDamageControl(checkTakeDamageAAPostMitigation, damage).value;

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

        public float HealHealth(float heal, string source)
        {
            if (heal <= 0) return 0;

            //add checks here


            myStats.currentHealth += (int)heal;
            if (myStats.currentHealth > myStats.maxHealth) myStats.currentHealth = myStats.maxHealth;

            simulationManager.ShowText($"{myStats.name} Took {heal} Heal From {source}!");

            return heal;
        }

        protected void EndBattle()
        {
            Time.timeScale = 1;
            SimManager.isSimulating = false;
            simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
            StopAllCoroutines();
            targetCombat.StopAllCoroutines();
            simulationManager.StopCoroutine(simulationManager.TakeSnapShot());
            APIRequestManager.Instance.SendOutputToJS(new WebData(simulationManager.outputText.text.Split("\n"), simulationManager.snaps.ToArray()));
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