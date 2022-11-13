using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Sion : ChampionCombat
{
    public int wPassive = 0;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "W", "E", "Q", "A" };

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
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("Maximum Physical Damage");
        wKeys.Add("Shield Strength");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Minimum Physical Damage");

        myStats.maxHealth += wPassive;
        myStats.currentHealth += wPassive;

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(2, myStats.buffManager, myStats.rSkill[0].basic.name, "DecimatingStrike"));
        StartCoroutine(DecimatingStrike());
        simulationManager.AddCastLog(myCastLog, 0);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (MyBuffManager.shields.ContainsKey(myStats.wSkill[0].basic.name))
        {
            myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(6, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
            myStats.wCD = 3;
        }
        else
        {
            MyBuffManager.shields[myStats.wSkill[0].basic.name].Kill();
            UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[1], skillComponentTypes: (SkillComponentTypes)51328);
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        }
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0], skillComponentTypes: (SkillComponentTypes)51332);
        TargetBuffManager.Add(myStats.eSkill[0].basic.name, new ArmorReductionBuff(4, targetStats.buffManager, myStats.eSkill[0].basic.name, 20, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(0.5f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: (SkillComponentTypes)18562);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        simulationManager.AddCastLog(myCastLog, 4);
    }

    public IEnumerator DecimatingStrike()
    {
        yield return new WaitForSeconds(2f);
		UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0],skillComponentTypes:(SkillComponentTypes)18560);
		targetStats.buffManager.buffs.Add("Airborne", new AirborneBuff(1, targetStats.buffManager, myStats.qSkill[0].basic.name));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(2.25f, targetStats.buffManager, myStats.qSkill[0].basic.name));
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Sion's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
    }
    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}