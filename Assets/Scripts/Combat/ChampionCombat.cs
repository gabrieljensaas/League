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

        [HideInInspector] public float attackCooldown;
        [HideInInspector] public List<Check> checksQ = new List<Check>();
        [HideInInspector] public List<Check> checksW = new List<Check>();
        [HideInInspector] public List<Check> checksE = new List<Check>();
        [HideInInspector] public List<Check> checksR = new List<Check>();
        [HideInInspector] public List<Check> checksA = new List<Check>();
        [HideInInspector] public List<Check> checkTakeDamageAA = new List<Check>();
        [HideInInspector] public List<Check> checkTakeDamage = new List<Check>();
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

            attackCooldown -= Time.deltaTime;
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
                    if (!CheckForAbilityControl(checksQ)) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.qSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill, 4);
                    myStats.qCD = myStats.qSkill.basic.coolDown[4];
                    break;
                case "W":
                    if (!CheckForAbilityControl(checksW)) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.wSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill, 4);
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

                    break;
                case "E":
                    if (!CheckForAbilityControl(checksE)) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.eSkill.basic.castTime));
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
                    if (!CheckForAbilityControl(checksR)) yield break;

                    yield return StartCoroutine(StartCastingAbility(myStats.rSkill.basic.castTime));
                    UpdateAbilityTotalDamage(ref qSum, 3, myStats.rSkill, 2);
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
                    if (!CheckForAbilityControl(checksA)) yield break;

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

            aSum += targetCombat.TakeDamage(damage, $"{myStats.name}'s Auto Attack", true);
            aaSum.text = aSum.ToString();

            attackCooldown = 1f / myStats.attackSpeed;
        }

        public float TakeDamage(float damage, string source, bool isAutoAttack = false)
        {
            if (damage <= 0) return 0;

            if(!isAutoAttack)
                damage = CheckForDamageControl(checkTakeDamage, damage);
            else
                damage = CheckForDamageControl(checkTakeDamageAA, damage);

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
                    checksQ.Add(new CheckIfCasting(this));
                    checksQ.Add(new CheckQCD(this));
                    checksQ.Add(new CheckAsheQ(this));
                    targetCombat.checksQ.Add(new CheckIfStunned(targetCombat));
                    checksW.Add(new CheckIfCasting(this));
                    checksW.Add(new CheckWCD(this));
                    targetCombat.checksW.Add(new CheckIfStunned(targetCombat));
                    checksE.Add(new CheckIfCasting(this));
                    checksE.Add(new CheckECD(this));
                    targetCombat.checksE.Add(new CheckIfStunned(targetCombat));
                    checksR.Add(new CheckIfCasting(this));
                    checksR.Add(new CheckRCD(this));
                    targetCombat.checksR.Add(new CheckIfStunned(targetCombat));
                    checksA.Add(new CheckIfCasting(this));
                    checksA.Add(new CheckACD(this));
                    targetCombat.checksA.Add(new CheckIfStunned(targetCombat));
                    autoattackcheck = new AsheAACheck(this);
                    targetCombat.checkTakeDamageAA.Add(new CheckIfFrosted(targetCombat));
                    break;

                case "Garen":
                    combatPrio[0] = "R";
                    combatPrio[1] = "W";
                    combatPrio[2] = "Q";
                    combatPrio[3] = "A";
                    combatPrio[4] = "E";
                    checksQ.Add(new CheckIfCasting(this));
                    checksQ.Add(new CheckQCD(this));
                    targetCombat.checksQ.Add(new CheckIfSilenced(targetCombat));
                    checksW.Add(new CheckIfCasting(this));
                    checksW.Add(new CheckWCD(this));
                    targetCombat.checksW.Add(new CheckIfSilenced(targetCombat));
                    checksE.Add(new CheckIfCasting(this));
                    checksE.Add(new CheckECD(this));
                    targetCombat.checksE.Add(new CheckIfSilenced(targetCombat));
                    checksR.Add(new CheckIfCasting(this));
                    checksR.Add(new CheckRCD(this));
                    targetCombat.checksR.Add(new CheckIfSilenced(targetCombat));
                    checksA.Add(new CheckIfCasting(this));
                    checksA.Add(new CheckIfCantAA(this));
                    checksA.Add(new CheckACD(this));
                    autoattackcheck = new GarenAACheck(this);
                    checkTakeDamage.Add(new CheckDamageReductionPercent(this));
                    checkTakeDamageAA.Add(new CheckDamageReductionPercent(this));
                    checkTakeDamage.Add(new CheckShield(this));
                    checkTakeDamageAA.Add(new CheckShield(this));
                    break;

                case "Aatrox":
                    combatPrio[0] = "R";
                    combatPrio[1] = "Q";
                    combatPrio[2] = "W";
                    combatPrio[3] = "A";
                    checksQ.Add(new CheckIfCasting(this));
                    checksA.Add(new CheckIfCasting(this));
                    checksA.Add(new CheckACD(this));
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

        public bool CheckForAbilityControl(List<Check> checks)
        {
            foreach (Check item in checks)
                if (!item.Control()) return false;

            return true;
        }

        public float CheckForDamageControl(List<Check> checks, float damage)
        {
            foreach (Check item in checks)
                damage = item.Control(damage);

            return damage;
        }
    }
}