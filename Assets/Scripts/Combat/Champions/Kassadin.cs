using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Kassadin : ChampionCombat
{
    public int eStack = 0;
    public int rStack = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

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
        checksR.Add(new CheckIfImmobilize(this));
        checkTakeDamageAA.Add(new CheckForKassadinPassive(this));
        checkTakeDamageAbility.Add(new CheckForKassadinPassive(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        autoattackcheck = new KassadinAACheck(this);
        targetCombat.castingCheck.Add(new CheckKassadinEPassive(targetCombat, this));

        qKeys.Add("Magic Damage");
        qKeys.Add("Magic Shield Strength");
        wKeys.Add("Increased Bonus Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");
        rKeys.Add("Bonus Damage Per Stack");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        TargetBuffManager.Add("Disrupt", new DisruptBuff(0.1f, TargetBuffManager, QSkill().basic.name));
        MyBuffManager.Add(QSkill().basic.name, new ShieldBuff(1.5f, MyBuffManager, QSkill().basic.name, QSkill().UseSkill(myStats.qLevel, qKeys[1], myStats, targetStats), QSkill().basic.name, shieldType: ShieldBuff.ShieldType.Magic));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        MyBuffManager.Add("NetherBlade", new NetherBladeBuff(5, MyBuffManager, WSkill().basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        if (eStack < 6) yield break;
        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        eStack = 0;
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) + (rStack * RSkill().UseSkill(myStats.rLevel, rKeys[1], myStats, targetStats)), SkillDamageType.Spell, SkillComponentTypes.Blink), RSkill().basic.name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        StopCoroutine(RStacks());
        StartCoroutine(RStacks());
    }

    public IEnumerator RStacks()
    {
        rStack = rStack == 4 ? 4 : rStack++;
        yield return new WaitForSeconds(5f);
        rStack = 0;
    }
}