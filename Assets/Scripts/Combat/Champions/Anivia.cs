using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Anivia : ChampionCombat
{
    private static float CheckBonusResistanceByLevel(int level)
	{
        return level switch
        {
            < 5 => -40,
            < 8 =>  -25,
            < 12 => -10,
            < 15 => 5,
            _ => 20, 
        };
	}
    public bool isChilled = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));

        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));

        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamageAAPostMitigation.Add(new CheckAniviaP(this, this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckAniviaP(this, this));


        qKeys.Add("Total Magic Damage");
        qKeys.Add("Stun Duration");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage per Tick");
        rKeys.Add("Empowered Damage per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        TargetBuffManager.Add("StunBuff", new StunBuff(QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), TargetBuffManager, "StunBuff"));
        if (TargetBuffManager.buffs.TryGetValue("ChilledBuff", out Buff buff))
            buff.duration = 3;
        else
            TargetBuffManager.Add("ChilledBuff", new ChilledBuff(3, TargetBuffManager, "ChilledBuff"));
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        TargetBuffManager.Add(WSkill().basic.name, new AirborneBuff(0.1f, TargetBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if (isChilled)
        {
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        }
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0]);
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        if (TargetBuffManager.buffs.TryGetValue("ChilledBuff", out Buff buff))
            buff.duration = 3;
        else
            TargetBuffManager.Add("ChilledBuff", new ChilledBuff(3, TargetBuffManager, "ChilledBuff"));
        myStats.rCD = RSkill().basic.coolDown[2];
        StartCoroutine(GlacialStorm());
    }

    public IEnumerator GlacialStorm()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[1]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[1]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 4, RSkill(), myStats.rLevel, rKeys[1]);
    }

    public void PassiveEgg(float currenthp)
	{
        if(currenthp < 0)
		{
            myStats.currentHealth = myStats.maxHealth;
			MyBuffManager.Add("EggPassive", new UnableToActBuff(6, MyBuffManager, "EggPassive"));
            MyBuffManager.Add("BonusArmor", new ArmorBuff(6, MyBuffManager, myStats.passiveSkill.skillName, CheckBonusResistanceByLevel(myStats.level), "BonusArmor"));
            MyBuffManager.Add("BonusMagicResistance", new MagicResistanceBuff(6, MyBuffManager, myStats.passiveSkill.skillName, (int) CheckBonusResistanceByLevel(myStats.level), "BonusMagicResistance"));
        }
	}
}