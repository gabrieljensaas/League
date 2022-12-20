using Simulator.Combat;
using System.Collections;

public class Ezreal : ChampionCombat
{
    private int pStack = 0;
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
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Bonus Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: (SkillComponentTypes)34972);
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
        myStats.qCD -= 1.5f;
        myStats.wCD -= 1.5f;
        myStats.eCD -= 1.5f;
        myStats.rCD -= 1.5f;
        CheckEssenceFlux();
        RisingSpellForce();
        simulationManager.AddCastLog(myCastLog, 0);
    }
    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("EssenceFlux", new EssenceFluxBuff(4, targetStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
        RisingSpellForce();
        simulationManager.AddCastLog(myCastLog, 1);
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        UpdateTotalDamage(ref eSum, 2, myStats.eSkill[0], myStats.eLevel, eKeys[0],skillComponentTypes:(SkillComponentTypes)34949);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
        CheckEssenceFlux();
        RisingSpellForce();
        simulationManager.AddCastLog(myCastLog, 2);
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return targetCombat.StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0],skillComponentTypes:(SkillComponentTypes)18564);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        CheckEssenceFlux();
        RisingSpellForce();
        simulationManager.AddCastLog(myCastLog, 3);
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        UpdateTotalDamage(ref aSum, 5, new Damage(myStats.AD, SkillDamageType.Phyiscal, skillComponentType: (SkillComponentTypes)5912), "Ezreal's Auto Attack");
        attackCooldown = 1f / myStats.attackSpeed;
        simulationManager.AddCastLog(myCastLog, 5);
        CheckEssenceFlux();
    }

    private void CheckEssenceFlux()
    {
        if (targetStats.buffManager.buffs.TryGetValue("EssenceFlux", out Buff value))
        {
            value.Kill();
            UpdateTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0]);
        }
    }

    private void RisingSpellForce()
    {
        if (!myStats.buffManager.buffs.ContainsKey(myStats.passiveSkill.skillName)) pStack = 0;
        if (pStack != 5) pStack++;
        myStats.buffManager.buffs.Remove(myStats.passiveSkill.skillName);
        myStats.buffManager.buffs.Add(myStats.passiveSkill.skillName, new AttackSpeedBuff(2.5f, myStats.buffManager, myStats.passiveSkill.skillName, pStack * 0.1f * myStats.baseAttackSpeed, myStats.passiveSkill.skillName));
    }
}