using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Shyvana : ChampionCombat
{
	private bool hasTwinBite;
	private bool hasDragonForm;
    private int durationTimesIncreased;
	private bool isMarked;
    private int fury =100;

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
        checksR.Add(new CheckIfImmobilize(this));

        qKeys.Add("");
        wKeys.Add("");
        eKeys.Add("");
        rKeys.Add("");

        MyBuffManager.Add("ArmorBuff", new ArmorBuff(float.MaxValue, MyBuffManager, myStats.passiveSkill.skillName, 5, "ArmorBuff"));
        MyBuffManager.Add("MRBuff", new MagicResistanceBuff(float.MaxValue, MyBuffManager, myStats.passiveSkill.skillName, 5, "MRBuff"));
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        hasTwinBite = true;
        myStats.qCD = QSkill().basic.coolDown[4];
        attackCooldown = 0;
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        MyBuffManager.Add("Burnout", new BurnoutBuff(3f, MyBuffManager, WSkill().basic.name));
        myStats.wCD = WSkill().basic.coolDown[4];
        StartCoroutine(BurnOut());
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(ESkill().basic.castTime));
        if(!hasDragonForm)
		{
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), ESkill().basic.name);
        }
		else
		{
            
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.Spell, SkillComponentTypes.Projectile), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name); 
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref eSum, 0, new Damage(30 + 2.5f * myStats.level, SkillDamageType.Spell, SkillComponentTypes.PersistentDamage), ESkill().basic.name);
        }
        isMarked = true;
        myStats.eCD = ESkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;
        if (fury < 100) yield break;
        yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref rSum, 3, RSkill(), myStats.rLevel, rKeys[1]);
        hasDragonForm = true;
        if(hasDragonForm)
		{

		}
        myStats.rCD = RSkill().basic.coolDown[2];
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(hasTwinBite)
		{
            AutoAttack(new Damage(QSkill().UseSkill(myStats.qLevel, qKeys[0], myStats, targetStats), SkillDamageType.Phyiscal, SkillComponentTypes.OnHit));
            hasTwinBite = false;
        }
        if(durationTimesIncreased <= 4)
		{
            if (MyBuffManager.buffs.TryGetValue("Burnout", out Buff value))
            {
                AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[1], myStats, targetStats), SkillDamageType.Spell));
                value.duration += 1;
            }
            durationTimesIncreased++;
        }
        if(isMarked)
		{
            AutoAttack(new Damage(0.35f * targetStats.maxHealth, SkillDamageType.Spell, SkillComponentTypes.OnHit));
        }
        AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
        fury += 2;
        myStats.qCD -= 0.5f;
    }

    public IEnumerator BurnOut()
    {
        while(MyBuffManager.buffs["Burnout"].duration >= 0.5f && durationTimesIncreased <= 4)
		{
            yield return new WaitForSeconds(0.5f);
            UpdateAbilityTotalDamage(ref wSum, 1, WSkill(), myStats.wLevel, wKeys[0]);
        }
    }
    public void FuryPassive()
	{
        fury += (int)(RSkill().UseSkill(myStats.rLevel, rKeys[0], myStats, targetStats) * Time.deltaTime);
	}
}