using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChampStats : MonoBehaviour
{
    public ExternalJS exportJS;
    public string name;

    public PassiveList passiveSkill;
    public SkillList qSkill;
    public SkillList wSkill;
    public SkillList eSkill;
    public SkillList rSkill;

    public Dictionary<string, bool> dynamicStatus = new Dictionary<string, bool>();
    public Dictionary<string, int> dynamicStatusStacks = new Dictionary<string, int>();
    public Dictionary<string, int> dynamicStatusFlat = new Dictionary<string, int>();
    public Dictionary<string, float> dynamicStatusPercent = new Dictionary<string, float>();
    public Dictionary<string, float> dynamicStatusDuration = new Dictionary<string, float>();
    public int qStacks;

    ChampCombat champCombat;
    public bool buffed;
    public bool debuffed;
    public bool ccImmune;
    public int totalDamage;

    public float shieldValue;
    public float shieldDuration;
    public ChampStatus status = new ChampStatus();
    public List<SpellInfo> spellInfo = new List<SpellInfo>();
    public bool qReady = true, wReady = true, eReady = true, rReady = true, passiveReady = true;
    public float qCD, wCD, eCD, rCD, pCD;
    [HideInInspector] public TextMeshProUGUI[] outputStats;
    public TextMeshProUGUI[] outputStats1;
    [HideInInspector] public List<Item> item;
    [HideInInspector] public GameObject itemList;
    [HideInInspector] public GameObject itemPrefab;
    [HideInInspector] public int itemCount;
    [HideInInspector] public bool isLoaded;
    [HideInInspector] public bool isItemsLoaded;
    [HideInInspector] public bool isMatchLoaded;

    public static bool isReady = false;
    [HideInInspector] public int level;
    [HideInInspector] public float originalAS;
    [HideInInspector] public float maxHealth, currentHealth, AD, AP, armor, spellBlock, attackSpeed, damageMitigation;
    [HideInInspector] public float baseHealth, baseAD, baseAP, baseArmor, baseSpellBlock, baseAttackSpeed;

    #region API Stats
    private bool isDead;
    private int maxResult;
    private int champCount;
    [HideInInspector] public float FlatHPPoolMod;
    [HideInInspector] public float rFlatHPModPerLevel;
    [HideInInspector] public float FlatMPPoolMod;
    [HideInInspector] public float rFlatMPModPerLevel;
    [HideInInspector] public float PercentHPPoolMod;
    [HideInInspector] public float PercentMPPoolMod;
    [HideInInspector] public float FlatHPRegenMod;
    [HideInInspector] public float rFlatHPRegenModPerLevel;
    [HideInInspector] public float PercentHPRegenMod;
    [HideInInspector] public float FlatMPRegenMod;
    [HideInInspector] public float rFlatMPRegenModPerLevel;
    [HideInInspector] public float PercentMPRegenMod;
    [HideInInspector] public float FlatArmorMod;
    [HideInInspector] public float rFlatArmorModPerLevel;
    [HideInInspector] public float PercentArmorMod;
    [HideInInspector] public float rFlatArmorPenetrationMod;
    [HideInInspector] public float rFlatArmorPenetrationModPerLevel;
    [HideInInspector] public float rPercentArmorPenetrationMod;
    [HideInInspector] public float rPercentArmorPenetrationModPerLevel;
    [HideInInspector] public float FlatPhysicalDamageMod;
    [HideInInspector] public float rFlatPhysicalDamageModPerLevel;
    [HideInInspector] public float PercentPhysicalDamageMod;
    [HideInInspector] public float FlatMagicDamageMod;
    [HideInInspector] public float rFlatMagicDamageModPerLevel;
    [HideInInspector] public float PercentMagicDamageMod;
    [HideInInspector] public float FlatMovementSpeedMod;
    [HideInInspector] public float rFlatMovementSpeedModPerLevel;
    [HideInInspector] public float PercentMovementSpeedMod;
    [HideInInspector] public float rPercentMovementSpeedModPerLevel;
    [HideInInspector] public float FlatAttackSpeedMod;
    [HideInInspector] public float PercentAttackSpeedMod;
    [HideInInspector] public float rPercentAttackSpeedModPerLevel;
    [HideInInspector] public float rFlatDodgeMod;
    [HideInInspector] public float rFlatDodgeModPerLevel;
    [HideInInspector] public float PercentDodgeMod;
    [HideInInspector] public float FlatCritChanceMod;
    [HideInInspector] public float rFlatCritChanceModPerLevel;
    [HideInInspector] public float PercentCritChanceMod;
    [HideInInspector] public float FlatCritDamageMod;
    [HideInInspector] public float rFlatCritDamageModPerLevel;
    [HideInInspector] public float PercentCritDamageMod;
    [HideInInspector] public float FlatBlockMod;
    [HideInInspector] public float PercentBlockMod;
    [HideInInspector] public float FlatSpellBlockMod;
    [HideInInspector] public float rFlatSpellBlockModPerLevel;
    [HideInInspector] public float PercentSpellBlockMod;
    [HideInInspector] public float FlatEXPBonus;
    [HideInInspector] public float PercentEXPBonus;
    [HideInInspector] public float rPercentCooldownMod;
    [HideInInspector] public float rPercentCooldownModPerLevel;
    [HideInInspector] public float rFlatTimeDeadMod;
    [HideInInspector] public float rFlatTimeDeadModPerLevel;
    [HideInInspector] public float rPercentTimeDeadMod;
    [HideInInspector] public float rPercentTimeDeadModPerLevel;
    [HideInInspector] public float rFlatItemGoldPer10Mod;
    [HideInInspector] public float rFlatMagicPenetrationMod;
    [HideInInspector] public float rFlatMagicPenetrationModPerLevel;
    [HideInInspector] public float rPercentMagicPenetrationMod;
    [HideInInspector] public float rPercentMagicPenetrationModPerLevel;
    [HideInInspector] public float FlatEnergyRegenMod;
    [HideInInspector] public float rFlatEnergyRegenModPerLevel;
    [HideInInspector] public float FlatEnergyPoolMod;
    [HideInInspector] public float rFlatEnergyModPerLevel;
    [HideInInspector] public float PercentLifeStealMod;
    [HideInInspector] public float PercentSpellVampMod;
    #endregion

    public static int stacks;
    [HideInInspector] public GameObject[] targets;
    [HideInInspector] public GameObject currentTarget;
    TextMeshProUGUI output;
    TextMeshProUGUI timer;

    [HideInInspector] public GenerateJSON generateJSON;
    void Awake()
    {
        output = GameObject.Find("Output Content").GetComponent<TextMeshProUGUI>();
        timer = GameObject.Find("Timer Content").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        targets = GameObject.FindGameObjectsWithTag("Champion");
        champCount = targets.Length;
        champCombat = GetComponent<ChampCombat>();
    }

    void Update()
    {
        if (!SimManager.battleStarted) return;

        if (currentTarget == null || currentTarget == this)
        {
            GetTarget();
        }

        if (currentHealth <= 0)
        {
            if (isDead) return;
            Die();
            //generateJSON.DataSerialize();
        }

        if (currentTarget != null)
        {
            isReady = true;
        }

        #region Jax
        if (dynamicStatus.ContainsKey("Counter Strike"))
        {
            dynamicStatusDuration["Counter Strike"] -= Time.deltaTime;
            if (dynamicStatusDuration["Counter Strike"] <= 0)
            {
                int totalDamage = (int)Mathf.Round((eSkill.damage.flatAD[4] + (AD * (eSkill.damage.percentAD[4] / 100))));
                totalDamage = (int)Mathf.Round(totalDamage * (100 / (100 + currentTarget.gameObject.GetComponent<ChampStats>().armor)));
                int _prev = int.Parse(this.gameObject.GetComponent<ChampCombat>().abilitySum[3].text);
                TextMeshProUGUI s = this.gameObject.GetComponent<ChampCombat>().abilitySum[3];
                eSkill.UseSkill(4, this, currentTarget.gameObject.GetComponent<ChampStats>(),s,_prev);
                dynamicStatus.Remove("Counter Strike");
                dynamicStatusStacks.Remove("Counter Strike");
                dynamicStatusDuration.Remove("Counter Strike");
            }
        }
        #endregion

        #region Olaf
        if (name == "Olaf")
        {
            if (dynamicStatus.ContainsKey("Ragnarok"))
            {
                dynamicStatusDuration["Ragnarok"] -= Time.deltaTime;
                if (dynamicStatusDuration["Ragnarok"] <= 0)
                {
                    dynamicStatus.Remove("Ragnarok");
                    dynamicStatusDuration.Remove("Ragnarok");
                    AD -= (int)Mathf.Round(30 + (AD * 0.25f));
                    UpdateStats();
                }
            }
        }
        #endregion

        #region Riven
        if (name == "Riven")
        {
            if (dynamicStatusDuration.ContainsKey("Blade of the Exile"))
            {
                dynamicStatusDuration["Blade of the Exile"] -= Time.deltaTime;
                if (dynamicStatusDuration["Blade of the Exile"] <= 0)
                {
                    dynamicStatus.Remove("Blade of the Exile");
                    dynamicStatusDuration.Remove("Blade of the Exile");
                }
            }
        }
        #endregion

        #region Ability Cool Down
        try
        {
            if (qCD > 0)
            {
                qCD -= Time.deltaTime;
                if (qCD <= 0)
                {
                    qReady = true;
                }
            }

            if (wCD > 0)
            {
                wCD -= Time.deltaTime;
                if (wCD <= 0)
                {
                    wReady = true;
                }
            }

            if (eSkill.charge.chargeable)
            {
                if (eSkill.charge.charges > 0)
                {
                    eReady = true;
                }
            }

            if (eCD > 0)
            {
                eCD -= Time.deltaTime;
                if (eCD <= 0)
                {
                    eReady = true;
                    if (eSkill.charge.chargeable)
                    {
                        if (eSkill.charge.charges < eSkill.charge.maxCharges[4])
                        {
                            eSkill.charge.charges++;
                        }
                    }
                }
            }

            if (rCD > 0)
            {
                rCD -= Time.deltaTime;
                if (rCD <= 0)
                {
                    rReady = true;
                }
            }

            if (pCD > 0)
            {
                pCD -= Time.deltaTime;
                if (pCD <= 0)
                {
                    passiveReady = true;
                }
            }
        }
        catch { }

        #endregion

        #region Special Effects

        if (shieldDuration > 0)
        {
            shieldDuration -= Time.deltaTime;
            if (shieldDuration <= 0)
            {
                shieldValue = 0;
                output.text += name + " shield has expired \n\n";
                UpdateTimer(SimManager.timer);
            }
        }
        if (status.stun)
        {
            status.stunDuration -= Time.deltaTime;
            champCombat.isCasting = false;
            if (status.stunDuration <= 0)
            {
                status.stun = false;
            }
        }
        else
        {
            status.stunDuration = 0;
        }

        //Silence
        if (status.silence)
        {
            status.silenceDuration -= Time.deltaTime;
            if (status.silenceDuration <= 0)
            {
                status.silence = false;
            }
        }
        else
        {
            status.silenceDuration = 0;
        }

        //Disarm
        if (status.disarm)
        {
            status.disarmDuration -= Time.deltaTime;
            if (status.disarmDuration <= 0)
            {
                status.disarm = false;
            }
        }
        else
        {
            status.disarmDuration = 0;
        }

        //Invincible
        if (status.invincible)
        {
            status.invincibleDuration -= Time.deltaTime;
            if (status.invincibleDuration <= 0)
            {
                status.invincible = false;
            }
        }
        else
        {
            status.invincibleDuration = 0;
        }

        //Damage Reduction
        if (status.damageReduction)
        {
            status.damageReducDuration -= Time.deltaTime;
            if (status.damageReducDuration <= 0)
            {
                status.damageReduction = false;
            }
        }
        else
        {
            status.invincibleDuration = 0;
        }
        #endregion
    }

    //public void TakeDamage(float amount, string time)
    public void TakeDamage(float amount)
    {
        if (status.invincible) amount = 0;
        if (status.damageReduction)
        {
            amount = amount * (1 - (status.damageReducPercent / 100));
        }
        if (amount < 0)
        {
            amount = 0;
        }
        int absorbed = (int)Mathf.Round(amount);

        if (amount != 0)
        {
            if (shieldValue > 0)
            {
                int previousValue = (int)Mathf.Round(shieldValue);
                shieldValue -= amount;
                if (shieldValue <= 0)
                {
                    shieldValue = 0;
                    shieldDuration = 0.001f;
                    output.text += "[DAMAGE] " + name + " shield absorbed " + previousValue + " damage from " + currentTarget.GetComponent<ChampStats>().name + " \n\n";
                    UpdateTimer(SimManager.timer);
                    output.text += "[DAMAGE] " + name + " took " + (amount- previousValue).ToString() + " damage from " + currentTarget.GetComponent<ChampStats>().name + " \n\n";
                    UpdateTimer(SimManager.timer);
                    amount = previousValue;
                }
                else
                {
                    output.text += "[DAMAGE] " + name + " shield absorbed " + absorbed + " damage from " + currentTarget.GetComponent<ChampStats>().name + " \n\n";
                    UpdateTimer(SimManager.timer);
                    amount -= absorbed;
                }
            }
        }

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateStats();
    }

    public void Cleanse() 
    { 
        status.stun = false;
        status.stunDuration = 0;
        status.silence = false;
        status.silenceDuration = 0;
        status.disarm = false;
        status.disarmDuration = 0;
    }

    public void CCImmune(float duration)
    {
        ccImmune = true;
        StartCoroutine(_CCImmune(duration));
    }

    IEnumerator _CCImmune(float duration)
    {
        yield return new WaitForSeconds(duration);
        ccImmune = false;
    }

    #region stats
    public void UpdateStats()
    {
        this.gameObject.name = name;
        outputStats1[0].text = name;
        outputStats1[1].text = level.ToString("F0");
        outputStats1[2].text = maxHealth.ToString("F0");
        outputStats1[3].text = currentHealth.ToString("F0");
        string s = "";
        int i = 0;
        foreach (string item in champCombat.combatPriority)
        {
            i++;
            if (i == champCombat.combatPriority.Length)
            {
                s += item;
            }
            else
            {
                s += item + ",";
            }
        }
        outputStats1[4].text = s;
        //outputStats[0].text = name;
        //outputStats[1].text = level.ToString("F0");
        //outputStats[2].text = maxHealth.ToString("F0");
        //outputStats[3].text = AD.ToString("F0");
        //outputStats[4].text = AP.ToString("F0");
        //outputStats[5].text = armor.ToString("F0");
        //outputStats[6].text = spellBlock.ToString("F0");
        //outputStats[7].text = attackSpeed.ToString("F2");
    }

    void GetTarget()
    {
        if(currentTarget == null)
        {
            for(int i = 0; i<champCount; i++)
            {
                currentTarget = targets[i];
            }
        }

        if(currentTarget.gameObject.name == this.gameObject.name)
        {
            for(int i = champCount; i>0; i--)
            {
                currentTarget = targets[i-1];
            }
        }
    }

    void Die()
    {
        ExportJSData exportData = new ExportJSData();
        isDead = true;
        SimManager.battleStarted = false;
        SimManager.isNew = true;
        SimManager.isLoaded = false;
        currentHealth = 0;
        UpdateTimer(SimManager.timer);
        output.text += currentTarget.gameObject.name + " has won with " + currentTarget.gameObject.GetComponent<ChampStats>().currentHealth + " health remaining.\n\n";
        exportData.myDamage = totalDamage;
        exportData.enemyDamage = currentTarget.gameObject.GetComponent<ChampStats>().totalDamage;
        exportJS.SendData(JsonUtility.ToJson(exportData));
        LoadingScreenHandler.Hide();
    }

    public void UpdateTimer(float time)
    {
        timer.text += SimManager._timer.ToString("F2") + "\n \n";
    }

    public void FinalizeStats()
    {
        maxHealth += FlatHPPoolMod;
        maxHealth *= 1+PercentHPPoolMod;
        currentHealth = maxHealth;
        AD += FlatPhysicalDamageMod;
        AP += FlatMagicDamageMod;
        armor += FlatArmorMod;
        spellBlock += FlatSpellBlockMod;
        attackSpeed *= (1 + (PercentAttackSpeedMod / 100));
        originalAS = attackSpeed;
        UpdateStats();
    }

    public void Reset(int _type)
    {
        if(_type == 0)
        {
            maxHealth = 0;
            currentHealth = 0;
            AD = 0;
            AP = 0;
            armor = 0;
            spellBlock = 0;
            attackSpeed = 0;
        }
        else if(_type == 1)
        {
            maxHealth = baseHealth;
            currentHealth = maxHealth;
            AD = baseAD;
            AP = baseAP;
            armor = baseArmor;
            spellBlock = baseSpellBlock;
            attackSpeed = baseAttackSpeed;
        }
        damageMitigation = 0;
        FlatHPPoolMod = 0;
        rFlatHPModPerLevel = 0;
        FlatMPPoolMod = 0;
        rFlatMPModPerLevel = 0;
        PercentHPPoolMod = 0;
        PercentMPPoolMod = 0;
        FlatHPRegenMod = 0;
        rFlatHPRegenModPerLevel = 0;
        PercentHPRegenMod = 0;
        FlatMPRegenMod = 0;
        rFlatMPRegenModPerLevel = 0;
        PercentMPRegenMod = 0;
        FlatArmorMod = 0;
        rFlatArmorModPerLevel = 0;
        PercentArmorMod = 0;
        rFlatArmorPenetrationMod = 0;
        rFlatArmorPenetrationModPerLevel = 0;
        rPercentArmorPenetrationMod = 0;
        rPercentArmorPenetrationModPerLevel = 0;
        FlatPhysicalDamageMod = 0;
        rFlatPhysicalDamageModPerLevel = 0;
        PercentPhysicalDamageMod = 0;
        FlatMagicDamageMod = 0;
        rFlatMagicDamageModPerLevel = 0;
        PercentMagicDamageMod = 0;
        FlatMovementSpeedMod = 0;
        rFlatMovementSpeedModPerLevel = 0;
        PercentMovementSpeedMod = 0;
        rPercentMovementSpeedModPerLevel = 0;
        FlatAttackSpeedMod = 0;
        PercentAttackSpeedMod = 0;
        rPercentAttackSpeedModPerLevel = 0;
        rFlatDodgeMod = 0;
        rFlatDodgeModPerLevel = 0;
        PercentDodgeMod = 0;
        FlatCritChanceMod = 0;
        rFlatCritChanceModPerLevel = 0;
        PercentCritChanceMod = 0;
        FlatCritDamageMod = 0;
        rFlatCritDamageModPerLevel = 0;
        PercentCritDamageMod = 0;
        FlatBlockMod = 0;
        PercentBlockMod = 0;
        FlatSpellBlockMod = 0;
        rFlatSpellBlockModPerLevel = 0;
        PercentSpellBlockMod = 0;
        FlatEXPBonus = 0;
        PercentEXPBonus = 0;
        rPercentCooldownMod = 0;
        rPercentCooldownModPerLevel = 0;
        rFlatTimeDeadMod = 0;
        rFlatTimeDeadModPerLevel = 0;
        rPercentTimeDeadMod = 0;
        rPercentTimeDeadModPerLevel = 0;
        rFlatItemGoldPer10Mod = 0;
        rFlatMagicPenetrationMod = 0;
        rFlatMagicPenetrationModPerLevel = 0;
        rPercentMagicPenetrationMod = 0;
        rPercentMagicPenetrationModPerLevel = 0;
        FlatEnergyRegenMod = 0;
        rFlatEnergyRegenModPerLevel = 0;
        FlatEnergyPoolMod = 0;
        rFlatEnergyModPerLevel = 0;
        PercentLifeStealMod = 0;
        PercentSpellVampMod = 0;
        FinalizeStats();
    }
    #endregion

    public void ModifyAS(float duration, float amount)
    {
        float orig = attackSpeed;
        PercentAttackSpeedMod += amount;
        attackSpeed *= (1 + (PercentAttackSpeedMod / 100));
        StartCoroutine(ASIncrease(duration, amount, orig));
        champCombat.canAttack = false;
    }

    IEnumerator ASIncrease(float duration, float amount, float orig)
    {
        yield return new WaitForSeconds(duration);
        PercentAttackSpeedMod -= amount;
        attackSpeed = orig;
        champCombat.canAttack = true;
        qStacks = 0;
        buffed = false;
    }

    [System.Serializable]
    public class SpellInfo
    {
        public string name;
        public double[] cooldown;
        public double[] damage;
    }

    class ExportJSData
    { 
        public int myDamage = 0;
        public int enemyDamage = 0;
        public int time = 0;
    }

}
