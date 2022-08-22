using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Simulator.Combat
{
    public class ChampionCombat : MonoBehaviour
    {
        [SerializeField] public ChampionStats myStats;
        [SerializeField] public ChampionStats targetStats;
        [SerializeField] private ChampionCombat targetCombat;
        [SerializeField] private TextMeshProUGUI output;
        [SerializeField] private TextMeshProUGUI aaSum;
        [SerializeField] public TextMeshProUGUI[] abilitySum;
        [SerializeField] private TextMeshProUGUI healSum;
        [SerializeField] private TextMeshProUGUI combatPriority;
        [SerializeField] public SimManager simulationManager;

        [HideInInspector] public float AttackCooldown;
        [HideInInspector] public List<Check> checksq = new List<Check>();
        [HideInInspector] public List<Check> checksw = new List<Check>();
        [HideInInspector] public List<Check> checkse = new List<Check>();
        [HideInInspector] public List<Check> checksr = new List<Check>();
        [HideInInspector] public List<Check> checksa = new List<Check>();
        [HideInInspector] public List<Check> checktakedamageaa = new List<Check>();
        [HideInInspector] public List<Check> checktakedamage = new List<Check>();
        [HideInInspector] public Check autoattackcheck;

        public float aSum, hSum, qSum, wSum, eSum, rSum, pSum;
        private string[] combatPrio = { "", "", "", "", "" };
        public bool isCasting = false;

        private void Start()
        {
            simulationManager = SimManager.Instance;
        }

        public void CombatUpdate()
        {
            CheckPassive();

            if (!isCasting)
            {
                CheckSkills();
            }

            AttackCooldown -= Time.deltaTime;
        }

        private void CheckPassive()
        {
            if (myStats.passiveSkill.inactive || myStats.pCD > 0) return;

            if (myStats.name == "Aatrox" && !myStats.buffManager.buffs.ContainsKey("DeathbringerStance"))
            {
                myStats.buffManager.buffs.Add("DeathbringerStance" ,new DeathbringerStanceBuff(float.MaxValue, myStats.buffManager, myStats.passiveSkill.name));
            }
        }

        private void CheckSkills()
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(ExecuteSkillIfReady(combatPrio[i]));                
            }
        }

        IEnumerator StartCastingAbility(float castTime)
        {
            isCasting = true;
            yield return new WaitForSeconds(castTime);
            isCasting = false;
        }

        private void UpdateAbilityTotalDamage(ref float totalDamage, int totalDamageTextIndex, SkillList skill, int level)
        {
            totalDamage += targetCombat.TakeDamage(skill.UseSkill(level, myStats, targetStats), skill.basic.name);
            abilitySum[totalDamageTextIndex].text = totalDamage.ToString();
        }

        IEnumerator ExecuteSkillIfReady(string skill)
        {
            switch (skill)
            {
                case "Q":
                    if (!CheckForQ()) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
                    myStats.qCD = myStats.qSkill.basic.coolDown[4];
                    break;
                case "W":
                    if (!CheckForW()) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

                    break;
                case "E":
                    if (!CheckForE()) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
                    if (myStats.name == "Garen")
                    {
                        simulationManager.ShowText($"Garen Used Judgment!");
                        myStats.eCD = myStats.eSkill.basic.coolDown[4];
                        myStats.buffManager.buffs.Add("CantAA" ,new CantAABuff(3f, myStats.buffManager, myStats.eSkill.basic.name));
                        StartCoroutine(GarenE(0, 0));
                        break;
                    }
                    UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill, 4);
                    myStats.eCD = myStats.eSkill.basic.coolDown[4];

                    break;
                case "R":
                    if (!CheckForR()) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref qSum, 3, myStats.qSkill, 2);
                    myStats.rCD = myStats.rSkill.basic.coolDown[2];

                    if (myStats.name == "Garen")
                    {
                        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
                        if (myStats.buffManager.buffs.ContainsKey("CantAA"))
                        {
                            myStats.buffManager.buffs.Remove("CantAA");
                        }
                    }

                    break;
                case "A":
                    if (!CheckForA()) yield break;

                    yield return StartCoroutine(StartCastingAbility(0.1f));
                    AutoAttack();
                    
                    break;
                default:
                    break;
            }
        }

        private IEnumerator GarenE(float seconds, int spinCount)
        {
            yield return new WaitForSeconds(seconds);
            eSum += targetCombat.TakeDamage(myStats.eSkill.UseSkill(4, myStats, targetStats), myStats.eSkill.basic.name);
            abilitySum[2].text = eSum.ToString();
            spinCount++;
            if (spinCount >= 6 && targetStats.buffManager.buffs.ContainsKey("Judgment"))
            {
                targetStats.buffManager.buffs["Judgment"].duration = 6;
            }
            else if(spinCount >= 6)
            {
                targetStats.buffManager.buffs.Add("Judgment", new ArmorReductionBuff(6, targetStats.buffManager, "Judgment", 25, "Judgment"));
            }
            if (spinCount > 6)
            {
                yield break;
            }
            StartCoroutine(GarenE(3f / 7f, spinCount));
        }

        private void AutoAttack()
        {
            float damage = Mathf.Round(myStats.AD * (100 / (100 + targetStats.armor)));
            if (damage < 0)
            {
                damage = 0;
            }

            if (autoattackcheck != null) damage = autoattackcheck.Control(damage);

            aSum += targetCombat.TakeDamageAA(damage, $"{myStats.name}'s Auto Attack");
            aaSum.text = aSum.ToString();

            AttackCooldown = 1f / myStats.attackSpeed;
        }

        IEnumerator UpdateCasting(float amount)
        {
            yield return new WaitForSeconds(amount);
            isCasting = false;
        }

        public float TakeDamageAA(float damage, string source)
        {
            damage = CheckForDamageAA(damage);

            myStats.currentHealth -= damage;
            simulationManager.ShowText($"{myStats.name} Took {damage} Damage From {source}!");

            if (myStats.currentHealth <= 0)
            {
                SimManager.battleStarted = false;
                simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
                StopAllCoroutines();
                targetCombat.StopAllCoroutines();
            }

            return damage;
        }
        public float TakeDamage(float damage, string source)
        {
            if (damage == 0) return 0;
            damage = CheckForDamage(damage);

            myStats.currentHealth -= damage;
            simulationManager.ShowText($"{myStats.name} Took {damage} Damage From {source}!");

            if (myStats.currentHealth <= 0)
            {
                SimManager.battleStarted = false;
                simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
                StopAllCoroutines();
                targetCombat.StopAllCoroutines();
            }

            return damage;
        }

        public void UpdatePriorityAndChecks()
        {
            switch (myStats.name)
            {
                case "Ashe":
                    combatPrio[0] = "Q";
                    combatPrio[1] = "A";
                    combatPrio[2] = "W";
                    combatPrio[3] = "R";
                    checksq.Add(new CheckIfCasting(this));
                    checksq.Add(new CheckQCD(this));
                    checksq.Add(new CheckAsheQ(this));
                    targetCombat.checksq.Add(new CheckIfStunned(targetCombat));
                    checksw.Add(new CheckIfCasting(this));
                    checksw.Add(new CheckWCD(this));
                    targetCombat.checksw.Add(new CheckIfStunned(targetCombat));
                    checkse.Add(new CheckIfCasting(this));
                    checkse.Add(new CheckECD(this));
                    targetCombat.checkse.Add(new CheckIfStunned(targetCombat));
                    checksr.Add(new CheckIfCasting(this));
                    checksr.Add(new CheckRCD(this));
                    targetCombat.checksr.Add(new CheckIfStunned(targetCombat));
                    checksa.Add(new CheckIfCasting(this));
                    checksa.Add(new CheckACD(this));
                    targetCombat.checksa.Add(new CheckIfStunned(targetCombat));
                    autoattackcheck = new AsheAACheck(this);
                    targetCombat.checktakedamageaa.Add(new CheckIfFrosted(targetCombat));
                    break;

                case "Garen":
                    combatPrio[0] = "R";
                    combatPrio[1] = "W";
                    combatPrio[2] = "Q";
                    combatPrio[3] = "A";
                    combatPrio[4] = "E";
                    checksq.Add(new CheckIfCasting(this));
                    checksq.Add(new CheckQCD(this));
                    targetCombat.checksq.Add(new CheckIfSilenced(targetCombat));
                    checksw.Add(new CheckIfCasting(this));
                    checksw.Add(new CheckWCD(this));
                    targetCombat.checksw.Add(new CheckIfSilenced(targetCombat));
                    checkse.Add(new CheckIfCasting(this));
                    checkse.Add(new CheckECD(this));
                    targetCombat.checkse.Add(new CheckIfSilenced(targetCombat));
                    checksr.Add(new CheckIfCasting(this));
                    checksr.Add(new CheckRCD(this));
                    targetCombat.checksr.Add(new CheckIfSilenced(targetCombat));
                    checksa.Add(new CheckIfCasting(this));
                    checksa.Add(new CheckIfCantAA(this));
                    checksa.Add(new CheckACD(this));
                    autoattackcheck = new GarenAACheck(this);
                    checktakedamage.Add(new CheckDamageReductionPercent(this));
                    checktakedamageaa.Add(new CheckDamageReductionPercent(this));
                    checktakedamage.Add(new CheckShield(this));
                    checktakedamageaa.Add(new CheckShield(this));
                    break;

                case "Aatrox":
                    combatPrio[0] = "R";
                    combatPrio[1] = "Q";
                    combatPrio[2] = "W";
                    combatPrio[3] = "A";
                    checksq.Add(new CheckIfCasting(this));
                    checksa.Add(new CheckIfCasting(this));
                    checksa.Add(new CheckACD(this));
                    autoattackcheck = new AatroxAACheck(this);
                    break;

                case "Gangplank":
                    combatPrio[0] = "A";
                    combatPrio[1] = "R";
                    combatPrio[2] = "E";
                    combatPrio[3] = "W";
                    combatPrio[4] = "Q";
                    break;

                case "Riven":
                    combatPrio[0] = "A";
                    combatPrio[1] = "Q";
                    combatPrio[2] = "W";
                    combatPrio[3] = "E";
                    combatPrio[4] = "R";
                    break;

                default:
                    combatPrio[0] = "Q";
                    combatPrio[1] = "W";
                    combatPrio[2] = "E";
                    combatPrio[3] = "R";
                    combatPrio[4] = "A";
                    break;
            }

            combatPriority.text = string.Join(", ", combatPrio);
        }

        public bool CheckForQ()
        {
            foreach (var item in checksq)
            {
                if (!item.Control()) return false;
            }
            return true;
        }
        public bool CheckForW()
        {
            foreach (var item in checksw)
            {
                if (!item.Control()) return false;
            }
            return true;
        }
        public bool CheckForE()
        {
            foreach (var item in checkse)
            {
                if (!item.Control()) return false;
            }
            return true;
        }
        public bool CheckForR()
        {
            foreach (var item in checksr)
            {
                if (!item.Control()) return false;
            }
            return true;
        }

        public bool CheckForA()
        {
            foreach (var item in checksa)
            {
                if (!item.Control()) return false;
            }
            return true;
        }

        public float CheckForDamageAA(float damage)
        {
            foreach (var item in checktakedamageaa)
            {
                damage = item.Control(damage);
            }

            return damage;
        }
        public float CheckForDamage(float damage)
        {
            foreach (var item in checktakedamage)
            {
                damage = item.Control(damage);
            }

            return damage;
        }
    }
}