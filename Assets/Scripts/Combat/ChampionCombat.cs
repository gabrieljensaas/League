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

        [HideInInspector] public float AttackCooldown;
        
        private float aSum, hSum, qSum, wSum, eSum, rSum, pSum;
        private string[] combatPriority = { "", "", "", "", "" };
        private bool isCasting = false;


        public void CombatUpdate()
        {
            CheckPassive();

            if (!isCasting)
            {
                CheckAvailableSkills();
            }

            #region Mordekaiser
            #endregion

            AttackCooldown -= Time.deltaTime;
        }

        IEnumerator DynamicPassive(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);

        }

        private void CheckPassive()
        {
            if (myStats.passiveSkill.inactive) return;
            //myStats.passiveSkill.alwaysActive


            if (myStats.passiveSkill.alwaysActive)
                {
                    /*if (myStats.name == "Mordekaiser")
                    {
                        myStats.passiveSkill.UseSkill(19, myStats, targetStats);
                    }
                    else
                    {
                        myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    }
                    myStats.pCD = myStats.passiveSkill.coolDown * SimManager.Instance.GlobalGameSpeedMultiplier;
                    */
                }
            
        }

        void CheckAvailableSkills()
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(CheckIfReady(combatPriority[i]));
                
            }
        }



        IEnumerator CheckIfReady(string skill)
        {
            /*int skillLevel = 4;
            int ultLevel = 2;
            int damage = 0;
            int passiveDamage = 0;
            int prevDamage = 0;*/
            switch (skill)
            {
                case "Q":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.qSkill.basic.inactive)
                    {
                        if(myStats.buffManager.AsheQBuff != 4) yield break;
                    }

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.qSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.qSkill.basic.castTime);

                    /*if (myStats.qSkill.multihit.multiHit)
                    {
                        for (int i = 0; i < myStats.qSkill.multihit.hits[skillLevel]; i++)
                        {
                            damage = myStats.qSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[1], prevDamage);
                            {
                                damage *= (int)Mathf.Round(1.1f);
                            }
                            yield return new WaitForSeconds(0.01f * SimManager.Instance.GlobalGameSpeedMultiplier);
                        }
                    }*/
                    /*else
                    {*/
                    qSum += myStats.qSkill.UseSkill(4, myStats, targetStats);
                    abilitySum[0].text = qSum.ToString();
                    myStats.qCD = myStats.qSkill.basic.coolDown[4];
                    //}

                    /*if (myStats.passiveSkill.applyOnAbility)
                    {
                        passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    }*/
                    break;
                case "W":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.wSkill.basic.inactive) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.wSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.wSkill.basic.castTime);
                    wSum += myStats.wSkill.UseSkill(4, myStats, targetStats);
                    abilitySum[1].text = wSum.ToString();
                    myStats.wCD = myStats.wSkill.basic.coolDown[4];

                    break;

                /*#region Ability E
                case "E":
                    if (isCasting) yield break;
                    if (myStats.eSkill == null) yield break;
                    if (myStats.eSkill.basic.inactive) yield break;
                    //prevDamage = int.Parse(abilitySum[3].text);

                    isCasting = true;

                    #region Gangplank
                    if (myStats.name == "Gangplank")
                    {
                        if (myStats.eSkill.charge.chargeable)
                        {
                            if (myStats.eSkill.charge.charges > 0)
                            {
                                //myStats.eSkill.charges--;
                                damage = myStats.eSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[3], prevDamage);
                            }
                            else
                            {
                                yield break;
                            }
                        }
                    }
                    #endregion

                    #region Riven

                    #endregion

                    #region Lucian

                    #endregion

                    #region Akali

                    #endregion


                    yield return new WaitForSeconds(myStats.eSkill.basic.castTime * SimManager.Instance.GlobalGameSpeedMultiplier);


                   /* if (myStats.eSkill.multihit.multiHit)
                    {
                        for (int i = 0; i < myStats.eSkill.multihit.hits[skillLevel]; i++)
                        {
                            #region Garen
                            if (myStats.name == "Garen")
                            {
                                float timeToWait = 3f / (myStats.eSkill.multihit.hits[skillLevel] * SimManager.Instance.GlobalGameSpeedMultiplier);
                                yield return new WaitForSeconds(timeToWait);
                            }
                            #endregion
                        }
                        StartCoroutine(UpdateCasting(0f));
                    }*/
                    /*else
                    {
                        #region Jax
                        if (myStats.name == "Jax")
                        {
                            output.text += "[ABILITY] " + myStats.name + " used " + myStats.eSkill.basic.name + "\n\n";
                            SimManager.WriteTime();
                            StartCoroutine(UpdateCasting(0f));
                        }
                        #endregion

                        else
                        {
                            damage = myStats.eSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[3], prevDamage);
                            StartCoroutine(UpdateCasting(myStats.eSkill.basic.castTime * SimManager.Instance.GlobalGameSpeedMultiplier));
                        }
                    }
                    myStats.eCD = myStats.eSkill.basic.coolDown[skillLevel] * SimManager.Instance.GlobalGameSpeedMultiplier;*/

                    /*#region Fiora
                    if (myStats.name == "Fiora") yield break;
                    #endregion

                    #region Olaf
                    #endregion*/

                    /*prevDamage = int.Parse(abilitySum[3].text);
                    //abilitySum[3].text = (prevDamage + damage).ToString();

                    if (myStats.passiveSkill.applyOnAbility)
                    {
                        passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    }
                    break;*/
                case "R":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Silenced) yield break;
                    if (myStats.rSkill.basic.inactive) yield break;

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    yield return new WaitForSeconds(myStats.rSkill.basic.castTime);
                    rSum += myStats.rSkill.UseSkill(2, myStats, targetStats);
                    abilitySum[3].text = rSum.ToString();
                    myStats.rCD = myStats.rSkill.basic.coolDown[2];

                    break;
                /*if (myStats.rSkill.multihit.multiHit)
                {
                    for (int i = 0; i < myStats.rSkill.multihit.hits[skillLevel]; i++)
                    {
                        #region Gangplank
                        if (myStats.name == "Gangplank")
                        {
                            StartCoroutine(UpdateCasting(0f));
                            float timeToWait = myStats.rSkill.multihit.multiHitInterval;
                            if (targetStats.currentHealth <= 0) yield break;
                            damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                            if (targetStats.currentHealth - damage <= 0) yield break;
                            yield return new WaitForSeconds(timeToWait);
                        }
                        #endregion
                    }
                    if (myStats.name == "Gangplank")
                    {
                        int bonusDamage = (int)Mathf.Round(300 + (myStats.AP * 30 / 100));
                        if (targetStats.currentHealth - bonusDamage <= 0) yield break;
                        output.text += "[SPECIAL] " + myStats.name + " Death's Daughter is triggered dealing " + bonusDamage.ToString() + " damage.\n\n";
                        SimManager.WriteTime();
                    }
                }*/

                //#region Garen
                //if (myStats.name == "Garen")
                //{
                //    damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                //    if (myStats.name == "Garen" && damage < targetStats.currentHealth)
                //    {
                //        isCasting = false;
                //        yield break;
                //    }
                //}
                //#endregion

                /*#region Olaf
                if (myStats.name == "Olaf")
                {
                    int bonusAD = (int)Mathf.Round(30 + (myStats.AD * 0.25f));
                    myStats.AD += bonusAD;
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    output.text += "[BUFF] " + myStats.name + " gains " + bonusAD + " Bonus AD for 3 seconds.\n\n";
                    SimManager.WriteTime();
                }
                #endregion

                #region Riven
                #endregion

                #region Aatrox
                if (myStats.name == "Aatrox")
                {
                    myStats.AD = Mathf.Round(myStats.AD * 1.4f);
                    output.text += "[SPECIAL] " + myStats.name + " used " + myStats.rSkill.basic.name + " and gains " + Mathf.Round(myStats.AD * 0.4f) + " bonus AD.\n\n";
                    SimManager.WriteTime();
                    output.text += "[SPECIAL] " + myStats.name + " used " + myStats.rSkill.basic.name + " and fears the enemy for 3 seconds.\n\n";
                    SimManager.WriteTime();
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                }
                #endregion*/

                /*else
                {
                    if (myStats.name != "Gangplank")
                    {
                        StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    }
                }

                yield return new WaitForSeconds(myStats.rSkill.basic.castTime);

                if (targetStats.currentHealth <= 0) yield break;
                damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                if (myStats.name != "Akali")
                {
                    myStats.rCD = myStats.rSkill.basic.coolDown[ultLevel] * SimManager.Instance.GlobalGameSpeedMultiplier;
                }*/

                //prevDamage = int.Parse(abilitySum[4].text);
                //abilitySum[4].text = (prevDamage + damage).ToString();
                /*if (myStats.passiveSkill.applyOnAbility)
                {
                    passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                }
                break;*/
                case "A":
                    if (isCasting) yield break;
                    if (myStats.buffManager.Stunned) yield break;
                    if (myStats.buffManager.Disarmed) yield break;
                    if (AttackCooldown <= 0)
                    {
                        isCasting = true;
                        StartCoroutine(UpdateCasting(0.1f));
                        yield return new WaitForSeconds(0.1f);
                        AutoAttack();
                    }
                    break;
                default:
                    break;
            }
        }

        /*public void Attack(Simulator.Combat.ChampionStats target, float amount)
        {
            int prevDamage = int.Parse(aaSum.text);
            aaSum.text = (prevDamage + amount).ToString();

            //if (targetStats.currentHealth - damage <= 0)
            //{
            //    return;
            //}
            //if (myStats.currentHealth - damage <= 0)
            //{ 
            //    return;
            //}
            {
                if (myStats.currentHealth > myStats.maxHealth)
                {
                    myStats.currentHealth = myStats.maxHealth;
                }
            }
        }*/

        private void AutoAttack()
        {
            float damage = Mathf.Round(myStats.AD * (100 / (100 + targetStats.armor)));
            if (damage < 0)
            {
                damage = 0;
            }
            /*
            #region Fiora
            try
            {
                if (myStats.dynamicStatusStacks["Bladework"] >= 1)
                {
                    myStats.dynamicStatusStacks["Bladework"]--;
                    if (myStats.dynamicStatusStacks["Bladework"] <= 0)
                    {
                        myStats.dynamicStatus.Remove("Bladework");
                        myStats.dynamicStatusStacks.Remove("Bladework");
                        myStats.PercentAttackSpeedMod = 0;
                        myStats.attackSpeed = myStats.originalAS;
                        myStats.UpdateStats(false);
                        myStats.buffed = false;
                    }
                }
            }
            catch { }
            #endregion
            */
            //if (myStats.name == "Jax")
            //{
            //    if (myStats.level >= 6)
            //    {
            //        if (myStats.dynamicStatus.ContainsKey(myStats.rSkill.name))
            //        {
            //            myStats.dynamicStatusStacks[myStats.rSkill.name]++;
            //            if (myStats.dynamicStatusStacks[myStats.rSkill.name] >= 2)
            //            {
            //
            //                int prevDamage = 0;
            //                myStats.dynamicStatusStacks[myStats.rSkill.name] = 0; 
            //                prevDamage = int.Parse(abilitySum[4].text);
            //                int ultLevel = myStats.level / 6 - 1;
            //                myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
            //                generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.rSkill.name);
            //            }
            //        }
            //        else
            //        {
            //            myStats.dynamicStatus[myStats.rSkill.name] = true;
            //            myStats.dynamicStatusStacks[myStats.rSkill.name] = 1;
            //        }
            //    }
            //}
            //if (targetStats.currentHealth-damage <= 0) return;
            //Attack(targetStats, damage);
            if(myStats.buffManager.Flurry > 0)
            {
                damage *= myStats.buffManager.Flurry;
            }

            targetCombat.TakeDamage(damage);

            if (myStats.name == "Ashe")
            {
                targetStats.buffManager.AddBuff("Frosted", 2, 0);

                if(myStats.buffManager.Flurry == 0)
                {
                    myStats.buffManager.AddBuff("AsheQBuff", 1, 0);
                }
            }

                /*if (myStats.passiveSkill.applyOnAttack)
                {
                    if (!myStats.passiveSkill.inactive)
                    {

                        if (targetStats.currentHealth <= 0) return;
                    myStats.pCD = myStats.passiveSkill.coolDown;
                        int passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);

                    }
                }*/
            
            ///

            AttackCooldown = 1f / myStats.attackSpeed;
        }

        IEnumerator UpdateCasting(float amount)
        {
            yield return new WaitForSeconds(amount);
            isCasting = false;
        }

        public void TakeDamage(float damage)
        {
            if (myStats.buffManager.Invincible) return;
            if (myStats.buffManager.Frosted) damage *= 1.1f;

            myStats.currentHealth -= damage;

            if(myStats.currentHealth <= 0)
            {
                //DIE
            }
        }

        public void UpdatePriority(string name)
        {
            switch (name)
            {
                case "Ashe":
                    combatPriority[0] = "Q";
                    combatPriority[1] = "A";
                    combatPriority[2] = "W";
                    combatPriority[3] = "R";
                    break;

                case "Garen":
                    combatPriority[0] = "R";
                    combatPriority[1] = "W";
                    combatPriority[2] = "Q";
                    combatPriority[3] = "A";
                    combatPriority[4] = "E";
                    break;

                case "Gangplank":
                    combatPriority[0] = "A";
                    combatPriority[1] = "R";
                    combatPriority[2] = "E";
                    combatPriority[3] = "W";
                    combatPriority[4] = "Q";
                    break;

                case "Riven":
                    combatPriority[0] = "A";
                    combatPriority[1] = "Q";
                    combatPriority[2] = "W";
                    combatPriority[3] = "E";
                    combatPriority[4] = "R";
                    break;

                default:
                    combatPriority[0] = "Q";
                    combatPriority[1] = "W";
                    combatPriority[2] = "E";
                    combatPriority[3] = "R";
                    combatPriority[4] = "A";
                    break;
            }
        }
    }
}