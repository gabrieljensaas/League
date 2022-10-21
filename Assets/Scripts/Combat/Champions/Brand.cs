using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Brand : ChampionCombat
{
    private int ablazeStack;
    private float timeSinceDetonation;
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

        targetCombat.checksQ.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksW.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksE.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksR.Add(new CheckIfEnemyTargetable(targetCombat));
        targetCombat.checksA.Add(new CheckIfEnemyTargetable(targetCombat));

        qKeys.Add("Magic Damage");

        wKeys.Add("Magic Damage");
        wKeys.Add("Increased Damage");

        eKeys.Add("Magic Damage");

        rKeys.Add("Total Single-Target Damage");

        //Apply PassiveDamage
        if ( timeSinceDetonation > 4)
		{
            if (ablazeStack > 0) StartCoroutine(Blaze());
            if (ablazeStack >= 3) StartCoroutine(BlazeBlast());
        }

        base.UpdatePriorityAndChecks();
    }
    public override void CombatUpdate()
    {
        base.CombatUpdate();
        timeSinceDetonation += Time.fixedDeltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ) || myStats.qLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        if(ablazeStack > 0)
		{
            TargetBuffManager.Add("Stun", new StunBuff(1.5f, TargetBuffManager, QSkill().basic.name));
		}
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable | SkillComponentTypes.Projectile | SkillComponentTypes.Blockable, buffNames: new string[] { "Stun"});
        myStats.qCD = QSkill().basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW) || myStats.wLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        if (ablazeStack > 0)
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[1], skillComponentTypes: SkillComponentTypes.Spellblockable);  
        else
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.wCD = WSkill().basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE) || myStats.eLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        UpdateAbilityTotalDamage(ref eSum, 2, ESkill(), myStats.eLevel, eKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.eCD = ESkill().basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR) || myStats.rLevel == 0) yield break;

        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable | SkillComponentTypes.Blockable);
        myStats.rCD = RSkill().basic.coolDown[myStats.rLevel];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
    }

    private IEnumerator Blaze()
	{
        yield return new WaitForSeconds(3.25f);
        UpdateAbilityTotalDamage(ref pSum, 4, new Damage(0.026f* targetStats.maxHealth, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage | SkillComponentTypes.Spellblockable), myStats.passiveSkill.skillName);
    } 

    private IEnumerator BlazeBlast()
	{
        yield return new WaitForSeconds(2f);
        UpdateAbilityTotalDamage(ref pSum, 4, new Damage((0.09f + (0.0025f *myStats.level)) * targetStats.maxHealth > 0.13f ? 0.13f :(0.09f + (0.0025f * myStats.level)) * targetStats.maxHealth, SkillDamageType.Spell,SkillComponentTypes.Spellblockable), myStats.passiveSkill.skillName);
        ablazeStack = 0;
        timeSinceDetonation = 0;
    }
}