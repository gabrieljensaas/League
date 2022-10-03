using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class VelKoz : ChampionCombat
{
    public float timeSinceAuto;
    private CheckVelKozP velKozP;
    private int voidRiftCharge = 2;
    private bool isResearched = false;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        velKozP = new CheckVelKozP(this);
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

        qKeys.Add("Magic Damage");
        wKeys.Add("Total Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Damage Per Tick");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        timeSinceAuto += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if (isResearched)
        {
            UpdateAbilityTotalDamage(ref qSum, 0, new Damage(QSkill().UseSkill(4, qKeys[0], myStats, targetStats), SkillDamageType.True), QSkill().basic.name);
        }
        else
        {
            UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[0]);
        }
        CheckVelKozPassiveDamage(QSkill().basic.name);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (voidRiftCharge > 0)
        {
            if (!CheckForAbilityControl(checksW)) yield break;

            yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
            if (isResearched)
            {
                UpdateAbilityTotalDamage(ref wSum, 1, new Damage(QSkill().UseSkill(4, wKeys[0], myStats, targetStats), SkillDamageType.True), WSkill().basic.name);
            }
            else
            {
                UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[0]);
            }
            voidRiftCharge--;
            myStats.wCD = WSkill().basic.coolDown[4];
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;
        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        myStats.eCD = ESkill().basic.coolDown[4];
        yield return new WaitForSeconds(0.75f);
        if (isResearched)
        {
            UpdateAbilityTotalDamage(ref eSum, 2, new Damage(ESkill().UseSkill(4, eKeys[0], myStats, targetStats), SkillDamageType.True), ESkill().basic.name);
        }
        else
        {
            UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), 4, eKeys[0]);
        }
        MyBuffManager.Add("KnockOff", new AirborneBuff(0.1f, TargetBuffManager, "KnockOff"));
        MyBuffManager.Add("StunBuff", new StunBuff(0.75f, TargetBuffManager, "StunBuff"));
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        myStats.rCD = RSkill().basic.coolDown[2];
        MyBuffManager.Add("Channeling", new ChannelingBuff(2.6f, MyBuffManager, RSkill().name, "DisintegrationRay"));
        StartCoroutine(DisintegrationRay());
    }

    private void CheckVelKozPassiveDamage(string skillName)
    {
        if (velKozP.Control())
        {
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            isResearched = true;
            myStats.buffManager.buffs.Remove("OrganicDestruction");
        }
        else if (myStats.buffManager.buffs.TryGetValue("OrganicDestruction", out Buff organicDestruction))
        {
            organicDestruction.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Destruction From {skillName}");
        }
        else
        {
            myStats.buffManager.buffs.Add("OrganicDestruction", new OrganicDestructionBuff(7, TargetBuffManager, skillName));
        }
    }

    public IEnumerator DisintegrationRay()
    {
        if (isResearched)
        {
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, new Damage(RSkill().UseSkill(2, rKeys[0], myStats, targetStats), SkillDamageType.True), RSkill().basic.name);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            yield return new WaitForSeconds(0.2f);
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[1]);
            UpdateAbilityTotalDamage(ref pSum, 4, new Damage((25 + 8 * myStats.level), SkillDamageType.True), myStats.passiveSkill.name);
        }
    }

    public override void StopChanneling(string uniqueKey)
    {
        StopCoroutine(uniqueKey);
    }
}