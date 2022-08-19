using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static AttributeTypes;
using static Simulator.Combat.BuffManager;
using System.Linq;

namespace Simulator.Combat
{
    public class ChampionCombat : MonoBehaviour
    {
        [SerializeField] private ChampionStats myStats;
        [SerializeField] private ChampionStats targetStats;
        [SerializeField] private ChampionCombat targetCombat;
        [SerializeField] private TextMeshProUGUI output;
        [SerializeField] private TextMeshProUGUI aaSum;
        [SerializeField] private TextMeshProUGUI[] abilitySum;
        [SerializeField] private TextMeshProUGUI healSum;
        [SerializeField] private TextMeshProUGUI combatPriority;
        [SerializeField] private SimManager simulationManager;

        [HideInInspector] public float AttackCooldown;
        [HideInInspector] public List<Check> checksq = new List<Check>();
        [HideInInspector] public List<Check> checksw = new List<Check>();
        [HideInInspector] public List<Check> checkse = new List<Check>();
        [HideInInspector] public List<Check> checksr = new List<Check>();
        [HideInInspector] public List<Check> checksa = new List<Check>();
        [HideInInspector] public List<Check> checktakedamageaa = new List<Check>();
        [HideInInspector] public List<Check> checktakedamage = new List<Check>();
        [HideInInspector] public Check autoattackcheck;

        private float aSum, hSum, qSum, wSum, eSum, rSum, pSum;
        private string[] combatPrio = { "", "", "", "", "" };
        private bool isCasting = false;

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
            if (myStats.passiveSkill.inactive) return;

            if (myStats.pCD > 0) return;

            /*if (myStats.passiveSkill.alwaysActive)
            {

            }*/    
        }

        private void CheckSkills()
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(ExecuteSkillIfReady(combatPrio[i]));                
            }
        }

        IEnumerator ExecuteSkillIfReady(string skill)
        {
            switch (skill)
            {
                case "Q":
                    if (!CheckForQ()) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.qSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.qSkill.basic.castTime);
                    
                    qSum += targetCombat.TakeDamage(myStats.qSkill.UseSkill(4, myStats, targetStats), myStats.qSkill.basic.name);
                    abilitySum[0].text = qSum.ToString();
                    myStats.qCD = myStats.qSkill.basic.coolDown[4];
                    break;
                case "W":
                    if (!CheckForW()) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.wSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.wSkill.basic.castTime);
                    wSum += targetCombat.TakeDamage(myStats.wSkill.UseSkill(4, myStats, targetStats), myStats.wSkill.basic.name);
                    abilitySum[1].text = wSum.ToString();
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

                    break;
                case "E":
                    if (!CheckForE()) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.eSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.eSkill.basic.castTime);
                    if (myStats.name == "Garen")
                    {
                        simulationManager.ShowText($"Garen Used Judgment!");
                        myStats.eCD = myStats.eSkill.basic.coolDown[4];
                        myStats.buffManager.buffs.Add(new CantAABuff(3f, myStats.buffManager, myStats.eSkill.basic.name));
                        StartCoroutine(GarenE(0, 0));
                        break;
                    }
                    eSum += targetCombat.TakeDamage(myStats.eSkill.UseSkill(4, myStats, targetStats), myStats.eSkill.basic.name);
                    abilitySum[2].text = eSum.ToString();
                    myStats.eCD = myStats.eSkill.basic.coolDown[4];

                    break;
                case "R":
                    if (!CheckForR()) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.rSkill.basic.castTime);
                    rSum += targetCombat.TakeDamage( myStats.rSkill.UseSkill(2, myStats, targetStats), myStats.rSkill.basic.name);
                    abilitySum[3].text = rSum.ToString();
                    myStats.rCD = myStats.rSkill.basic.coolDown[2];

                    if (myStats.name == "Garen")
                    {
                        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
                        if(myStats.buffManager.buffs.OfType<CantAABuff>().Any())
                        {
                            myStats.buffManager.buffs.OfType<CantAABuff>().FirstOrDefault().Kill();
                            myStats.buffManager.buffs.Remove(myStats.buffManager.buffs.OfType<CantAABuff>().First());
                        }
                    }

                    break;
                case "A":
                    if (!CheckForA()) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(0.1f));
                    yield return new WaitForSeconds(0.1f);
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
            if(spinCount >= 6)
            {
                targetStats.buffManager.buffs.Add(new ArmorReductionBuff(6, targetStats.buffManager, "Judgment", 25));
            }
            if(spinCount > 6)
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

        public abstract class Check
        {
            protected ChampionCombat combat;
            protected Check(ChampionCombat ccombat)
            {
                combat = ccombat;
            }

            public abstract bool Control();
            public abstract float Control(float damage);
        }

        public class CheckIfCasting : Check
        {
            public CheckIfCasting(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.isCasting) return false;
                return true;
            }

            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckQCD : Check
        {
            public CheckQCD(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.myStats.qCD > 0) return false;
                return true;
            }

            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckWCD : Check
        {
            public CheckWCD(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.myStats.wCD > 0) return false;
                return true;
            }

            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckECD : Check
        {
            public CheckECD(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.myStats.eCD > 0) return false;
                return true;
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }
        public class CheckRCD : Check
        {
            public CheckRCD(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.myStats.rCD > 0) return false;
                return true;
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }
        public class CheckACD : Check
        {
            public CheckACD(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                if (combat.AttackCooldown > 0) return false;
                return true;
            }

            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckAsheQ : Check
        {
            public CheckAsheQ(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                return combat.myStats.buffManager.Has4AsheQ();
            }

            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckIfStunned : Check
        {
            public CheckIfStunned(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                return !combat.myStats.buffManager.IsStunned();
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckIfSilenced : Check
        {
            public CheckIfSilenced(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                return !combat.myStats.buffManager.IsSilenced();
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckIfDisarmed : Check
        {
            public CheckIfDisarmed(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                return !combat.myStats.buffManager.IsDisarmed();
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }

        public class CheckIfCantAA : Check
        {
            public CheckIfCantAA(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                return combat.myStats.buffManager.CanAA();
            }
            public override float Control(float damage)
            {
                throw new System.NotImplementedException();
            }
        }
        public class CheckIfFrosted : Check
        {
            public CheckIfFrosted(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override bool Control()
            {
                throw new System.NotImplementedException();
            }
            public override float Control(float damage)
            {
                return damage * 1.1f;
            }
        }

        public class AsheAACheck : Check
        {
            public AsheAACheck(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override float Control(float damage)
            {
                if (combat.myStats.buffManager.FlurryDamage() > 0)
                {
                    damage *= combat.myStats.buffManager.FlurryDamage() / 100;
                    combat.qSum += combat.myStats.buffManager.FlurryDamage() * damage / 100;
                    combat.abilitySum[0].text = combat.qSum.ToString();
                }

                combat.targetStats.buffManager.buffs.Add(new FrostedBuff(2, combat.targetStats.buffManager, "Ashe's Auto Attack"));
                if (combat.myStats.buffManager.FlurryDamage() == 0)
                {
                    combat.myStats.buffManager.buffs.Add(new AsheQBuff(4, combat.myStats.buffManager, "Ashe's Auto Attack"));
                }
                return damage;
            }

            public override bool Control()
            {
                throw new System.NotImplementedException();
            }
        }
        public class GarenAACheck : Check
        {
            public GarenAACheck(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override float Control(float damage)
            {
                if (combat.myStats.buffManager.DecisiveStrikeDamage() > 0)
                {
                    damage += combat.myStats.buffManager.DecisiveStrikeDamage();
                    combat.qSum += combat.myStats.buffManager.DecisiveStrikeDamage();
                    combat.abilitySum[0].text = combat.qSum.ToString();
                    combat.myStats.buffManager.buffs.Remove(combat.myStats.buffManager.buffs.OfType<DecisiveStrikeBuff>().FirstOrDefault());
                    combat.targetStats.buffManager.buffs.Add(new SilenceBuff(1.5f, combat.targetStats.buffManager, "Decisive Strike"));
                }
                return damage;
            }

            public override bool Control()
            {
                throw new System.NotImplementedException();
            }
        }
        public class CheckDamageReductionPercent : Check
        {
            public CheckDamageReductionPercent(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override float Control(float damage)
            {
                if (combat.myStats.buffManager.DamageRed() > 0)
                {
                    damage *= (100 - combat.myStats.buffManager.DamageRed()) / 100;
                }
                return damage;
            }

            public override bool Control()
            {
                throw new System.NotImplementedException();
            }
        }
        public class CheckShield : Check
        {
            public CheckShield(ChampionCombat ccombat) : base(ccombat)
            {
            }

            public override float Control(float damage)
            {
                if (combat.myStats.buffManager.Shield() > 0)
                {
                    if (combat.myStats.buffManager.Shield() >= damage)
                    {
                        combat.myStats.buffManager.buffs.OfType<ShieldBuff>().FirstOrDefault().shield -= damage;
                        combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {damage} Damage!");
                    }
                    else
                    {
                        combat.simulationManager.ShowText($"{combat.myStats.name}'s Shield Absorbed {combat.myStats.buffManager.Shield()} Damage!");
                        damage -= combat.myStats.buffManager.Shield();
                        combat.myStats.buffManager.buffs.Remove(combat.myStats.buffManager.buffs.OfType<ShieldBuff>().FirstOrDefault());
                    }
                }
                return damage;
            }

            public override bool Control()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}