using System.Collections;
using UnityEngine;
using TMPro;
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
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.qCD > 0) yield break;
                    if (myStats.qSkill.basic.inactive)
                    {
                        if(myStats.buffManager.AsheQBuff != 4) yield break;
                    }

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.qSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.qSkill.basic.castTime);
                    
                    qSum += targetCombat.TakeDamage(myStats.qSkill.UseSkill(4, myStats, targetStats), myStats.qSkill.basic.name);
                    abilitySum[0].text = qSum.ToString();
                    myStats.qCD = myStats.qSkill.basic.coolDown[4];
                    break;
                case "W":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.wCD > 0) yield break;
                    if (myStats.wSkill.basic.inactive) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.wSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.wSkill.basic.castTime);
                    wSum += targetCombat.TakeDamage(myStats.wSkill.UseSkill(4, myStats, targetStats), myStats.wSkill.basic.name);
                    abilitySum[1].text = wSum.ToString();
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

                    if (myStats.name == "Ashe")
                    {
                        targetStats.buffManager.AddBuff("Frosted", 2, 0, myStats.wSkill.basic.name);
                    }

                    break;
                case "E":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.eCD > 0) yield break;
                    if (myStats.eSkill.basic.inactive) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.eSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.eSkill.basic.castTime);
                    if (myStats.name == "Garen")
                    {
                        simulationManager.ShowText($"Garen Used Judgment!");
                        myStats.eCD = myStats.eSkill.basic.coolDown[4];
                        myStats.buffManager.AddBuff("CantAA", 3f, 0, myStats.eSkill.basic.name);
                        StartCoroutine(GarenE(0, 0));
                        break;
                    }
                    eSum += targetCombat.TakeDamage(myStats.eSkill.UseSkill(4, myStats, targetStats), myStats.eSkill.basic.name);
                    abilitySum[2].text = eSum.ToString();
                    myStats.eCD = myStats.eSkill.basic.coolDown[4];

                    break;
                case "R":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.rCD > 0) yield break;
                    if (myStats.rSkill.basic.inactive) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.rSkill.basic.castTime);
                    rSum += targetCombat.TakeDamage( myStats.rSkill.UseSkill(2, myStats, targetStats), myStats.rSkill.basic.name);
                    abilitySum[3].text = rSum.ToString();
                    myStats.rCD = myStats.rSkill.basic.coolDown[2];

                    if (myStats.name == "Garen")
                    {
                        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
                        myStats.buffManager.CantAA = false;
                    }

                    if (myStats.name == "Ashe")
                    {
                        targetStats.buffManager.AddBuff("Frosted", 2, 0, myStats.rSkill.basic.name);
                    }

                    break;
                case "A":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Disarmed) yield break;
                    if (myStats.buffManager.CantAA) yield break;
                    if (AttackCooldown <= 0)
                    {
                        isCasting = true;
                        StartCoroutine(UpdateCasting(0.1f));
                        yield return new WaitForSeconds(0.1f);
                        AutoAttack();
                    }
                    else yield break;
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
                targetStats.buffManager.AddBuff("ArmorReduction", 6, 25, "Judgment");
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
            if(myStats.buffManager.Flurry > 0)
            {
                damage *= myStats.buffManager.Flurry / 100;
                qSum += myStats.buffManager.Flurry * damage / 100;
                abilitySum[0].text = qSum.ToString();
            }

            if(myStats.buffManager.DecisiveStrike > 0)
            {
                damage += myStats.buffManager.DecisiveStrike;
                qSum += myStats.buffManager.DecisiveStrike;
                abilitySum[0].text = qSum.ToString();
                myStats.buffManager.DecisiveStrike = 0f;
                targetStats.buffManager.AddBuff("Silenced", 1.5f, 0, "Decisive Strike");
            }

            aSum += targetCombat.TakeDamageAA(damage, $"{myStats.name}'s Auto Attack");
            aaSum.text = aSum.ToString();

            if (myStats.name == "Ashe")
            {
                targetStats.buffManager.AddBuff("Frosted", 2, 0, "Ashe's Auto Attack");

                if(myStats.buffManager.Flurry == 0)
                {
                    myStats.buffManager.AddBuff("AsheQBuff", 4, 0, "Ashe's Auto Attack");
                }
            }

            AttackCooldown = 1f / myStats.attackSpeed;
        }

        IEnumerator UpdateCasting(float amount)
        {
            yield return new WaitForSeconds(amount);
            isCasting = false;
        }

        public float TakeDamageAA(float damage, string source)
        {
            if (myStats.buffManager.Invincible) return 0;
            if (myStats.buffManager.Frosted)
            {
                damage *= 1.1f;
            }
            if(myStats.buffManager.DamageReductionPercent > 0)
            {
                damage *= (100 - myStats.buffManager.DamageReductionPercent) / 100;
            }
            if(myStats.buffManager.Shield > 0)
            {
                if (myStats.buffManager.Shield >= damage)
                {
                    myStats.buffManager.Shield -= damage;
                    simulationManager.ShowText($"{myStats.name}'s Shield Absorbed {damage} Damage!");
                }
                else
                {
                    simulationManager.ShowText($"{myStats.name}'s Shield Absorbed {myStats.buffManager.Shield} Damage!");
                    damage -= myStats.buffManager.Shield;
                    myStats.buffManager.Shield = 0;
                }
            }

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
            if (myStats.buffManager.Invincible) return 0;
            if (damage == 0) return 0;
            if (myStats.buffManager.DamageReductionPercent > 0)
            {
                damage *= (100 - myStats.buffManager.DamageReductionPercent) / 100;
            }
            if (myStats.buffManager.Shield > 0)
            {
                if (myStats.buffManager.Shield >= damage)
                {
                    myStats.buffManager.Shield -= damage;
                    simulationManager.ShowText($"{myStats.name}'s Shield Absorbed {damage} Damage!");
                }
                else
                {
                    simulationManager.ShowText($"{myStats.name}'s Shield Absorbed {myStats.buffManager.Shield} Damage!");
                    damage -= myStats.buffManager.Shield;
                    myStats.buffManager.Shield = 0;
                }
            }

            myStats.currentHealth -= damage;
            simulationManager.ShowText($"{myStats.name} Took {damage} Damage From {source}!");

            if (myStats.currentHealth <= 0)
            {
                SimManager.battleStarted = false;
                simulationManager.ShowText($"{myStats.name} Has Died! {targetStats.name} Won With {targetStats.currentHealth} Health Remaining!");
            }

            return damage;
        }

        public void UpdatePriority()
        {
            switch (myStats.name)
            {
                case "Ashe":
                    combatPrio[0] = "Q";
                    combatPrio[1] = "A";
                    combatPrio[2] = "W";
                    combatPrio[3] = "R";
                    break;

                case "Garen":
                    combatPrio[0] = "R";
                    combatPrio[1] = "W";
                    combatPrio[2] = "Q";
                    combatPrio[3] = "A";
                    combatPrio[4] = "E";
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

        public bool ChecksForAsheQSelf()
        {
            if (isCasting) return false;
            if (myStats.qCD > 0) return false;
            if (myStats.buffManager.AsheQBuff != 4) return false;
            return true;
        }
    }
}