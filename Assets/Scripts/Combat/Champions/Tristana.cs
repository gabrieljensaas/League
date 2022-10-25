using Simulator.Combat;
using System.Collections;

public class Tristana : ChampionCombat
{
    public static float GetTristanaExplosiveChargeByLevel(int level, int stack)
    {
        return 30 + (25 * level) + ((18 + (level * 3)) * stack);
    }

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "W", "A" };

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
        checksW.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Bonus Attack Speed");
        wKeys.Add("Magic Damage");
        eKeys.Add("Minimum Physical Damage");
        eKeys.Add("Bonus Damage Per Stack");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if(myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("AmbushAS", new AttackSpeedBuff(7, myStats.buffManager, myStats.qSkill[0].basic.name, myStats.qSkill[0].UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), "AmbushAS"));
        UpdateTotalDamage(ref qSum, 0, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2048), QSkill().basic.name);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
        simulationManager.AddCastLog(myCastLog, 0);
    }
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0],skillComponentTypes: (SkillComponentTypes)18562);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
        CheckExplosiveCharge();
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(1f / myStats.attackSpeed));
        if(UpdateTotalDamage(ref eSum, 2, new Damage(0, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)2052), ESkill().basic.name) != float.MinValue)
            targetStats.buffManager.buffs.Add("Explosive Charge", new ExplosiveChargeBuff(4, targetStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        if(UpdateTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)34948) != float.MinValue)
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, targetStats.buffManager, myStats.rSkill[0].basic.name));
            CheckExplosiveCharge();
        }
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return StartCoroutine(base.HijackedR(skillLevel));
        myStats.buffManager.buffs.Add("Stun", new StunBuff(0.75f, myStats.buffManager, myStats.rSkill[0].basic.name));
        simulationManager.AddCastLog(targetCombat.myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5916), "Tristana's Auto Attack") != float.MinValue)
        {
            CheckExplosiveCharge();
        }
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }

    private void CheckExplosiveCharge()
    {
        if (targetStats.buffManager.buffs.TryGetValue("Explosive Charge", out Buff explosiveCharge))
        {
            explosiveCharge.value++;
            if (explosiveCharge.value == 4)
            {
                myStats.wCD = 0;
                explosiveCharge.Kill();
            }
        }
    }
}