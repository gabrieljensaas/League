using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ChampCombat : MonoBehaviour
{
    public float attackCooldown;
    float _attackCooldown = 1f / (float) _attackSpeed;
    ChampStats myStats;
    ChampStats targetStats;
    GameObject[] champions;
    TextMeshProUGUI output;
    public string[] combatPriority = {"","","","",""};
    int cpLength = 5; //Combat Priority
    float damage;
    static float _attackSpeed = 0f;
    float skillDamage;
    public bool canAttack = true;
    public bool isCasting = true;
    bool firstAttack = true;
    public TextMeshProUGUI damageSum;
    public TextMeshProUGUI[] abilitySum;

    public GenerateJSON generateJSON;

    void Awake()
    {
        output = GameObject.Find("Output Content").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        myStats = GetComponent<ChampStats>();
        _attackSpeed = myStats.attackSpeed;
        //StartCoroutine(Test());
        
    }
    IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        if (myStats.eSkill.charge.chargeable)
        {
            myStats.eSkill.charge.charges = (int)myStats.eSkill.charge.maxCharges[4];
        }
    }

    void Update()
    {        
        if(!targetStats)
        {
            if(!myStats.currentTarget) return;
            targetStats = myStats.currentTarget.GetComponent<ChampStats>();
        }
        if (!SimManager.battleStarted)
        {
            SimManager.timer = 0;
            SimManager._timer = 0;
            return;
        }
        

        CheckPassive();
        
        if(!isCasting)
        {
            CheckAvailableSkills();
        }

        #region Mordekaiser
        if (myStats.dynamicStatus.ContainsKey("Darkness Rise"))
        {
            StartCoroutine(DynamicPassive(1f));
        }
        #endregion

        attackCooldown -= Time.deltaTime;
    }

    IEnumerator DynamicPassive(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        
    }

    void CheckPassive()
    {
        if (myStats.passiveSkill != null)
        {
            if (myStats.passiveSkill.alwaysActive)
            {
                if (myStats.passiveReady)
                {
                    if (myStats.passiveSkill.inactive) return;
                    if (myStats.name == "Mordekaiser")
                    {
                        myStats.passiveSkill.UseSkill(19, myStats, targetStats);
                    }
                    else
                    {
                        myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    }
                    myStats.passiveReady = false;
                    myStats.pCD = myStats.passiveSkill.coolDown;
                }
            }
        }
    }

    void CheckAvailableSkills()
    {
        for(int i = 0; i<cpLength; i++)
        {
            StartCoroutine(CheckIfReady(combatPriority[i]));
        }
    }

    IEnumerator CheckIfReady(string skill)
    {
        int skillLevel = 4;
        int ultLevel = 2;
        int damage = 0;
        int passiveDamage = 0;
        int prevDamage = 0;
        switch (skill)
        {
            #region Ability Q
            case "Q":
                    if (myStats.status.silence || myStats.status.stun) yield break;
                    if (isCasting) yield break;
                    if (!myStats.qReady) yield break;
                    if (myStats.qSkill == null) yield break;
                    if (myStats.qSkill.basic.inactive) yield break;

                    //Ashe
                    if (myStats.name == "Ashe" && myStats.qStacks < 4) yield break;
                    //

                    isCasting = true;
                    StartCoroutine(UpdateCasting(myStats.qSkill.basic.castTime));
                    if (targetStats.currentHealth <= 0) yield break;
                    yield return new WaitForSeconds(myStats.qSkill.basic.castTime);
                    myStats.qReady = false;
                    prevDamage = int.Parse(abilitySum[1].text);

                    if (myStats.qSkill.multihit.multiHit)
                    {
                        for (int i = 0; i < myStats.qSkill.multihit.hits[skillLevel]; i++)
                        {
                            damage = myStats.qSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[1], prevDamage);
                            if (targetStats.dynamicStatus["Frost Shot"])
                            {
                                damage *= (int)Mathf.Round(1.1f);
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                    else
                    {
                        if (targetStats.currentHealth <= 0) yield break;
                        damage = myStats.qSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[1], prevDamage);
                    }

                    #region Ashe
                    if (myStats.name == "Ashe")
                    {
                        isCasting = true;
                        StartCoroutine(UpdateCasting(attackCooldown = 1f / (float)myStats.attackSpeed));
                        yield return new WaitForSeconds(attackCooldown = 1f / (float)myStats.attackSpeed);
                        myStats.qReady = true;
                        myStats.qCD = (float)(attackCooldown = 1f / myStats.attackSpeed);
                    }
                    #endregion

                    #region Riven
                    if (myStats.name == "Riven")
                    {
                        myStats.passiveReady = true;
                    }
                    #endregion

                    #region Lucian
                    if (myStats.name == "Lucian")
                    {
                        myStats.passiveReady = true;
                    }
                    #endregion

                    #region Akali
                    if (myStats.name == "Akali")
                    {
                        myStats.passiveReady = true;
                    }
                    #endregion

                    else
                    {
                        myStats.qCD = myStats.qSkill.basic.coolDown[skillLevel];
                    }
                    generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.qSkill.name);
                    if (myStats.passiveSkill.applyOnAbility)
                    {
                        passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                        generateJSON.SendData(false, this.gameObject.name, passiveDamage, SimManager.timer, 3, myStats.passiveSkill.name);
                    }
            
                break;
#endregion

            #region Ability W
            case "W":
                if (myStats.status.silence || myStats.status.stun) yield break;
                if (isCasting) yield break;
                if (!myStats.wReady) yield break;
                if (myStats.wSkill == null) yield break;
                if (myStats.wSkill.basic.inactive) yield break;
            
                isCasting = true;
                StartCoroutine(UpdateCasting(myStats.wSkill.basic.castTime));
                yield return new WaitForSeconds(myStats.wSkill.basic.castTime);
                if (myStats.status.silence || myStats.status.stun) yield break;
                if (targetStats.currentHealth <= 0) yield break;
                prevDamage = int.Parse(abilitySum[2].text);
                damage = myStats.wSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[2], prevDamage);
                myStats.wReady = false;
                myStats.wCD = myStats.wSkill.basic.coolDown[skillLevel];
                generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.wSkill.name);

                #region Gangplank
                if (myStats.name == "Gangplank")
                {
                    myStats.Cleanse();
                }
                #endregion

                #region Riven
                if (myStats.name == "Riven")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Lucian
                if (myStats.name == "Lucian")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Akali
                if (myStats.name == "Akali")
                {
                    myStats.passiveReady = true;
                }
                #endregion


                if (myStats.passiveSkill.applyOnAbility)
                {
                    passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    generateJSON.SendData(false, this.gameObject.name, passiveDamage, SimManager.timer, 3, myStats.passiveSkill.name);
                }
                break;
#endregion

            #region Ability E
            case "E":
                if (myStats.status.silence || myStats.status.stun) yield break;
                if (isCasting) yield break;
                if (!myStats.eReady) yield break;
                if (myStats.eSkill == null) yield break;
                if (myStats.eSkill.basic.inactive) yield break;
                prevDamage = int.Parse(abilitySum[3].text);

                isCasting = true;
                myStats.eReady = false;

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
                if (myStats.name == "Riven")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Lucian
                if (myStats.name == "Lucian")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Akali
                if (myStats.name == "Akali")
                {
                    myStats.passiveReady = true;
                }
                #endregion


                yield return new WaitForSeconds(myStats.eSkill.basic.castTime);


                if (myStats.eSkill.multihit.multiHit)
                {
                    for (int i = 0; i < myStats.eSkill.multihit.hits[skillLevel]; i++)
                    {
                        #region Garen
                        if (myStats.name == "Garen")
                        {
                            float timeToWait = 3f / (myStats.eSkill.multihit.hits[skillLevel]);
                            if (!myStats.status.stun)
                            {
                                damage = myStats.eSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[3], prevDamage);
                            }
                            yield return new WaitForSeconds(timeToWait);
                        }
                        #endregion
                    }
                    StartCoroutine(UpdateCasting(0f));
                }
                else
                {                    
                    #region Jax
                    if (myStats.name == "Jax")
                    {
                        myStats.dynamicStatusDuration[myStats.eSkill.basic.name] = 2f;
                        myStats.dynamicStatus[myStats.eSkill.basic.name] = true;
                        myStats.dynamicStatusStacks[myStats.eSkill.basic.name] = 0;
                        output.text += "[ABILITY] " + myStats.name + " used " + myStats.eSkill.basic.name + "\n\n";
                        myStats.UpdateTimer(SimManager.timer);
                        StartCoroutine(UpdateCasting(0f));
                    }
                    #endregion

                    else
                    {
                        damage = myStats.eSkill.UseSkill(skillLevel, myStats, targetStats, abilitySum[3], prevDamage);
                        StartCoroutine(UpdateCasting(myStats.eSkill.basic.castTime));
                    }
                }
                myStats.eCD = myStats.eSkill.basic.coolDown[skillLevel];
                generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.eSkill.name);

                #region Fiora
                if (myStats.name == "Fiora") yield break;
                #endregion

                #region Olaf
                try
                {
                    if (myStats.dynamicStatus["Ragnarok"])
                    {
                        myStats.dynamicStatusDuration["Ragnarok"] += 2.5f;
                    }
                }
                catch { }
                #endregion

                prevDamage = int.Parse(abilitySum[3].text);
                //abilitySum[3].text = (prevDamage + damage).ToString();

                if (myStats.passiveSkill.applyOnAbility)
                {
                    passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    generateJSON.SendData(false, this.gameObject.name, passiveDamage, SimManager.timer, 3, myStats.passiveSkill.name);
                }
                break;
#endregion

            #region Ability R
            case "R":
                if (myStats.status.silence || myStats.status.stun) yield break;
                if (isCasting) yield break;
                if (!myStats.rReady) yield break;
                if (myStats.rSkill == null) yield break;
                if (myStats.rSkill.basic.inactive) yield break;
                prevDamage = int.Parse(abilitySum[4].text);

                isCasting = true;

                if (myStats.rSkill.multihit.multiHit)
                {
                    myStats.rReady = false;
                    for (int i = 0; i < myStats.rSkill.multihit.hits[skillLevel]; i++)
                    {
                        #region Gangplank
                        if (myStats.name == "Gangplank")
                        {
                            StartCoroutine(UpdateCasting(0f));
                            float timeToWait = myStats.rSkill.multihit.multiHitInterval;
                            if (targetStats.currentHealth <= 0) yield break;
                            damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats,abilitySum[4], prevDamage);
                            if (targetStats.currentHealth-damage <= 0) yield break;
                            yield return new WaitForSeconds(timeToWait);
                        }
                        #endregion
                    }
                    if (myStats.name == "Gangplank")
                    {
                        int bonusDamage = (int)Mathf.Round(300 + (myStats.AP * 30 / 100));
                        if (targetStats.currentHealth - bonusDamage <= 0) yield break;
                        output.text += "[SPECIAL] " + myStats.name + " Death's Daughter is triggered dealing " + bonusDamage.ToString() + " damage.\n\n";
                        myStats.UpdateTimer(SimManager.timer);
                    }
                }

                #region Garen
                if (myStats.name == "Garen")
                {
                    damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                    if (myStats.name == "Garen" && damage < targetStats.currentHealth)
                    {
                        isCasting = false;
                        yield break;
                    }
                }
                #endregion

                #region Olaf
                if (myStats.name == "Olaf")
                {
                    myStats.Cleanse();
                    myStats.CCImmune(3f);
                    myStats.dynamicStatus[myStats.rSkill.name] = true;
                    myStats.dynamicStatusDuration[myStats.rSkill.name] = 3f;
                    int bonusAD = (int)Mathf.Round(30 + (myStats.AD * 0.25f));
                    myStats.AD += bonusAD;
                    myStats.UpdateStats();
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    output.text += "[BUFF] " + myStats.name + " gains " + bonusAD + " Bonus AD for 3 seconds.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                }
                #endregion

                #region Riven
                if (myStats.name == "Riven")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Aatrox
                if (myStats.name == "Aatrox")
                {
                    myStats.AD = Mathf.Round(myStats.AD * 1.4f);
                    myStats.PercentLifeStealMod *= (int)Mathf.Round(1.55f);
                    myStats.UpdateStats();
                    output.text += "[SPECIAL] " + myStats.name + " used "+myStats.rSkill.basic.name+" and gains " + Mathf.Round(myStats.AD * 0.4f) + " bonus AD.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                    output.text += "[SPECIAL] " + myStats.name + " used " + myStats.rSkill.basic.name + " and gains " + Mathf.Round(myStats.PercentLifeStealMod * 0.55f) + "% bonus healing.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                    output.text += "[SPECIAL] " + myStats.name + " used " + myStats.rSkill.basic.name + " and fears the enemy for 3 seconds.\n\n";
                    myStats.UpdateTimer(SimManager.timer);
                    targetStats.status.silence = true;
                    targetStats.status.disarm = true;
                    targetStats.status.silenceDuration = 3f;
                    targetStats.status.disarmDuration = 3f;
                    StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                }
                #endregion

                #region Lucian
                if (myStats.name == "Lucian")
                {
                    myStats.passiveReady = true;
                }
                #endregion

                #region Akali
                if (myStats.name == "Akali")
                {
                    if (myStats.dynamicStatus.ContainsKey("Ult First Cast"))
                    {
                        myStats.dynamicStatus.Remove("Ult First Cast");
                    }
                    else
                    {
                        myStats.passiveReady = true;
                        myStats.dynamicStatus["Ult First Cast"] = true;
                        myStats.rCD = 2.5f;
                    }
                }
                #endregion


                else
                {
                    if (myStats.name != "Gangplank")
                    {
                        StartCoroutine(UpdateCasting(myStats.rSkill.basic.castTime));
                    }
                }

                yield return new WaitForSeconds(myStats.rSkill.basic.castTime);

                if (targetStats.currentHealth <= 0) yield break;
                damage = myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                myStats.rReady = false;
                if(myStats.name != "Akali")
                {
                    myStats.rCD = myStats.rSkill.basic.coolDown[ultLevel];
                }
                generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.rSkill.name);

                //prevDamage = int.Parse(abilitySum[4].text);
                //abilitySum[4].text = (prevDamage + damage).ToString();
                if (myStats.passiveSkill.applyOnAbility)
                {
                    passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                    generateJSON.SendData(false, this.gameObject.name, passiveDamage, SimManager.timer, 3, myStats.passiveSkill.name);
                }
                break;
            #endregion

            #region Auto Attack
            case "A":
                if (attackCooldown <= 0)
                {
                    if (isCasting) yield break;
                    if (!canAttack) yield break;
                    if (myStats.status.disarm || myStats.status.stun) yield break;
                    isCasting = true;
                    canAttack = false;
                    StartCoroutine(UpdateCasting(0.1f));
                    yield return new WaitForSeconds(0.1f);
                    AutoAttack();
                }
                break;
                #endregion
        }

        firstAttack = false;
    }

    #region Attack Function
    //int type: 0 = auto attack, 1 = q, 2 = w, 3 = e, 4 = r, 5 = passive
    public void Attack(ChampStats target, float amount)
    {
        myStats.qStacks++;
        int prevDamage = int.Parse(damageSum.text);
        damageSum.text = (prevDamage +amount).ToString();
        myStats.totalDamage += + (int)amount;

        //if (targetStats.currentHealth - damage <= 0)
        //{
        //    return;
        //}
        //if (myStats.currentHealth - damage <= 0)
        //{ 
        //    return;
        //}
        target.TakeDamage(amount);
        myStats.UpdateTimer(SimManager.timer);
        if (myStats.PercentLifeStealMod > 0)
        {
            float LSValue = amount * (myStats.PercentLifeStealMod / 100);
            myStats.currentHealth += LSValue;
            if (myStats.currentHealth > myStats.maxHealth)
            {
                myStats.currentHealth = myStats.maxHealth;
            }
        }
    }

    void AutoAttack()
    {
        int damage = 0;
        int _damage = 0;
        damage = (int)Mathf.Round(myStats.AD * (100 / (100 + targetStats.armor)));
        if (damage < 0)
        {
            damage = 0;
        }

        #region Ashe
        try
        {
            if (targetStats.dynamicStatus["Frost Shot"])
            {
                damage = (int)Mathf.Round(damage * 1.1f);
            }
        }
        catch { }
        #endregion

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
                    myStats.UpdateStats();
                    myStats.buffed = false;
                }
            }
        }
        catch { }
        #endregion

        #region Olaf
        try
        {
            if (myStats.dynamicStatus["Ragnarok"])
            {
                myStats.dynamicStatusDuration["Ragnarok"] += 2.5f;
            }
        }
        catch { }
        #endregion

        if (targetStats.status.damageReduction)
        {
            _damage = (int)Mathf.Round(damage * (1 - (targetStats.status.damageReducPercent / 100)));
        }
        else 
        {
            _damage = damage;
        }

        #region Jax
        try
        {
            if (targetStats.dynamicStatusDuration["Counter Strike"] > 0)
            {
                damage = 0;
                output.text += "[SPECIAL] " + targetStats.name + " evaded " + myStats.name + " attack.\n\n";
                myStats.UpdateTimer(SimManager.timer);
                try
                {
                    targetStats.dynamicStatusStacks["Counter Strike"]++;
                }
                catch 
                { 
                    targetStats.dynamicStatusStacks["Counter Strike"] = 1; 
                }

                if (targetStats.dynamicStatusStacks["Counter Strike"] > 5)
                {
                    targetStats.dynamicStatusStacks["Counter Strike"] = 5;
                }
            }
        }
        catch
        {
        }

        if (myStats.name == "Jax")
        {
            if (myStats.level >= 6)
            {
                if (myStats.dynamicStatus.ContainsKey(myStats.rSkill.name))
                {
                    myStats.dynamicStatusStacks[myStats.rSkill.name]++;
                    if (myStats.dynamicStatusStacks[myStats.rSkill.name] >= 2)
                    {

                        int prevDamage = 0;
                        myStats.dynamicStatusStacks[myStats.rSkill.name] = 0; 
                        prevDamage = int.Parse(abilitySum[4].text);
                        int ultLevel = myStats.level / 6 - 1;
                        myStats.rSkill.UseSkill(ultLevel, myStats, targetStats, abilitySum[4], prevDamage);
                        generateJSON.SendData(false, this.gameObject.name, damage, SimManager.timer, 2, myStats.rSkill.name);
                    }
                }
                else
                {
                    myStats.dynamicStatus[myStats.rSkill.name] = true;
                    myStats.dynamicStatusStacks[myStats.rSkill.name] = 1;
                }
            }
        }
        #endregion

        #region Mordekaiser
        if (myStats.name == "Mordekaiser")
        {
            try
            {
                if (targetStats.dynamicStatusStacks["Darkness Rise"] < 3)
                {
                    targetStats.dynamicStatusStacks["Darkness Rise"]++;
                }

                if (targetStats.dynamicStatusStacks["Darkness Rise"] >= 3)
                {
                    //myStats.dynamicStatus["Darkness Rise"] = true;
                    myStats.passiveSkill.coolDown = 1f;
                    myStats.passiveSkill.alwaysActive = true;
                }
            }
            catch 
            {
                targetStats.dynamicStatus["Darkness Rise"] = true;
                targetStats.dynamicStatusStacks["Darkness Rise"] = 1;
            }
        }
        #endregion

        //if (targetStats.currentHealth-damage <= 0) return;
        if (myStats.currentHealth - damage <= 0) return;
        output.text += "[DAMAGE] " + this.gameObject.name + " used Auto Attack and dealt " + _damage.ToString() + " damage.\n\n";
        Attack(targetStats, damage);


        //Apply Passive
        if (myStats.passiveSkill != null)
        {
            if (myStats.passiveSkill.applyOnAttack)
            {
                if (!myStats.passiveSkill.inactive)
                {
                    if (myStats.passiveReady)
                    {
                        if (targetStats.currentHealth <= 0) return;
                        myStats.passiveReady = false;
                        myStats.pCD = myStats.passiveSkill.coolDown;
                        int passiveDamage = myStats.passiveSkill.UseSkill(myStats.level, myStats, targetStats);
                        generateJSON.SendData(false, this.gameObject.name, passiveDamage, SimManager.timer, 3, myStats.passiveSkill.name);
                    }
                }
            }
        }
        ///

        attackCooldown = 1f / (float) myStats.attackSpeed;

        #region Generate JSON File
        if (SimManager.isNew == true)
        {            
            generateJSON.SendData(true, this.gameObject.name, (int)(myStats.AD-targetStats.armor), SimManager.timer, 1, null);
        }
        else
        {
            generateJSON.SendData(false, this.gameObject.name, (int)(myStats.AD-targetStats.armor), SimManager.timer, 1, null);
        }
        #endregion
    }
    #endregion

    IEnumerator UpdateCasting(float amount)
    {
        yield return new WaitForSeconds(amount);
        isCasting = false;
        canAttack = true;
    }

    public void PriorityList(string name)
    {
        switch (name)
        {
            case "Ashe":
                combatPriority[0] = "Q";
                combatPriority[1] = "A";
                combatPriority[2] = "W";
                combatPriority[3] = "R";
                combatPriority[4] = "E";
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
