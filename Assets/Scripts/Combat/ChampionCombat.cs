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
                    
                    targetCombat.TakeDamage(qSum += myStats.qSkill.UseSkill(4, myStats, targetStats));
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
                    targetCombat.TakeDamage(wSum += myStats.wSkill.UseSkill(4, myStats, targetStats));
                    abilitySum[1].text = wSum.ToString();
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

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
                        myStats.eCD = myStats.eSkill.basic.coolDown[4];
                        myStats.buffManager.AddBuff("CantAA", 3f, 0, myStats.eSkill.basic.name);
                        StartCoroutine(GarenE(3f / 7f, 0));
                        break;
                    }
                    targetCombat.TakeDamage(eSum += myStats.eSkill.UseSkill(4, myStats, targetStats));
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
                    targetCombat.TakeDamage(rSum += myStats.rSkill.UseSkill(2, myStats, targetStats));
                    abilitySum[3].text = rSum.ToString();
                    myStats.rCD = myStats.rSkill.basic.coolDown[2];

                    if (myStats.name == "Garen")
                    {
                        StopCoroutine("GarenE");          //if 2 GarenE coroutine exists this could leat to some bugs
                        myStats.buffManager.CantAA = false;
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
            targetCombat.TakeDamage(eSum += myStats.eSkill.UseSkill(4, myStats, targetStats));
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
                damage *= myStats.buffManager.Flurry;
            }

            if(myStats.buffManager.DecisiveStrike > 0)
            {
                damage += myStats.buffManager.DecisiveStrike;
                myStats.buffManager.DecisiveStrike = 0f;
                targetStats.buffManager.AddBuff("Silenced", 1.5f, 0, "Decisive Strike");
            }
         
            myStats.buffManager.DecisiveStrike = 0f;

            targetCombat.TakeDamageAA(damage);

            if (myStats.name == "Ashe")
            {
                targetStats.buffManager.AddBuff("Frosted", 2, 0, "Ashe's Auto Attack");
                //simulationManager.ShowText(targetStats.name + " Frosted", "Ashe's Auto Attack");

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

        public void TakeDamageAA(float damage)
        {
            if (myStats.buffManager.Invincible) return;
            if (myStats.buffManager.Frosted)
            {
                //simulationManager.ShowText("  " + myStats.name + " Took Extra " + (damage * 0.1f).ToString() + "Becuase of Frost");
                pSum += damage * 0.1f;
                damage *= 1.1f;
            }
            if(myStats.buffManager.DamageReductionPercent > 0)
            {
                simulationManager.ShowText($"{myStats.name} Reduced {damage * myStats.buffManager.DamageReductionPercent / 100} Damage!");
                damage *= (100 - myStats.buffManager.DamageReductionPercent) / 100;
            }
            if(myStats.buffManager.Shield > 0)
            {
                if (myStats.buffManager.Shield >= damage)
                {
                    myStats.buffManager.Shield -= damage;
                }
                else
                {
                    damage -= myStats.buffManager.Shield;
                    myStats.buffManager.Shield = 0;
                }
            }

            myStats.currentHealth -= damage;

            if(myStats.currentHealth <= 0)
            {
                //DIE
            }
        }
        public void TakeDamage(float damage)
        {
            if (myStats.buffManager.Invincible) return;
            if (myStats.buffManager.DamageReductionPercent > 0)
            {
                simulationManager.ShowText($"{myStats.name} Reduced {damage * myStats.buffManager.DamageReductionPercent / 100} Damage!");
                damage *= (100 - myStats.buffManager.DamageReductionPercent) / 100;
            }
            if (myStats.buffManager.Shield > 0)
            {
                if (myStats.buffManager.Shield >= damage)
                {
                    myStats.buffManager.Shield -= damage;
                }
                else
                {
                    damage -= myStats.buffManager.Shield;
                    myStats.buffManager.Shield = 0;
                }
            }

            myStats.currentHealth -= damage;

            if (myStats.currentHealth <= 0)
            {
                //DIE
            }
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
    }
}