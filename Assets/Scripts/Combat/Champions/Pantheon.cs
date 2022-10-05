using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Pantheon : ChampionCombat
{
    public static float[] PantheonRDamageByQLevel = { 40, 70, 100, 130, 160, 190 };
    private bool qCast;
    private float timeSinceQ;
    public int passiveStack = 5;
    public bool hasMortalWill = false;
	private bool eCast;
    private float timeSinceE;

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

        qKeys.Add("Physical Damage");
        qKeys.Add("Increased Damage");
        wKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        rKeys.Add("Armor Penetration");
        rKeys.Add("Magic Damage");

        //Add Armor Pen Buff

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        if (!qCast)
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            StartCoroutine(CometSpear());
            MyBuffManager.Add("Channeling", new ChannelingBuff(0.35f, MyBuffManager, QSkill().basic.name, "Channelling"));
            timeSinceQ = 0;

        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
            CheckMortalWill();
            if (targetStats.PercentMissingHealth < 0.2f)
            {
                if (hasMortalWill)
                {
                    UpdateAbilityTotalDamage(ref qSum, 0, new Damage(20 + 220 / 17 * (myStats.level - 1), SkillDamageType.Phyiscal), QSkill().basic.name);
                }
                UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[1]);
            }
            else
            {
                if (hasMortalWill)
                {
                    UpdateAbilityTotalDamage(ref qSum, 0, new Damage(20 + 220 / 17 * (myStats.level - 1), SkillDamageType.Phyiscal), QSkill().basic.name);
                }
                UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
            }
            StopCoroutine(CometSpear());
            myStats.qCD = QSkill().basic.coolDown[4] - timeSinceQ;
        }
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        CheckMortalWill();
        if (hasMortalWill)
        {
            //Apply Crit through autoattackcheck
        }
        UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        TargetBuffManager.Add("Stun", new StunBuff(1, TargetBuffManager, "Stun"));
        myStats.wCD = WSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        if (!eCast)
        {
            yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
            StartCoroutine(AegisAssault());
            MyBuffManager.Add(ESkill().basic.name, new ChannelingBuff(1.5f, MyBuffManager, ESkill().basic.name, ESkill().basic.name));
            MyBuffManager.Add(ESkill().basic.name, new UntargetableBuff(1.5f, MyBuffManager, ESkill().basic.name));
            myStats.eCD = 0.3f;
            timeSinceE = 0;
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), 4, eKeys[0]);
            StopCoroutine(AegisAssault());
            myStats.eCD = ESkill().basic.coolDown[4] - timeSinceE;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        CheckMortalWill();
        MyBuffManager.Add("Channeling", new ChannelingBuff(4.2f, MyBuffManager, RSkill().basic.name, "Channelling"));
        yield return new WaitForSeconds(4f);
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage( PantheonRDamageByQLevel[myStats.qLevel], SkillDamageType.Phyiscal), RSkill().basic.name);
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0]);
        passiveStack = 5;
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    private void CheckMortalWill()
    {
        if(passiveStack == 5)
		{
            hasMortalWill = true;
            passiveStack = 0;
		}
        else
		{
            passiveStack++;
		}
    }

    public IEnumerator CometSpear()
    {
        qCast = true;
        yield return new WaitForSeconds(3f);
        qCast = false;
        myStats.qCD = QSkill().basic.coolDown[4] - timeSinceQ;
    }

    public IEnumerator AegisAssault()
	{
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
        yield return new WaitForSeconds(0.125f);
        UpdateAbilityTotalDamage(ref eSum, 2, new Damage((float)0.83 * myStats.AD, SkillDamageType.Phyiscal), ESkill().basic.name);
    }
}