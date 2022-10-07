using Simulator.Combat;
using System.Collections;

public class TwistedFate : ChampionCombat
{

	private bool wCast;
    private bool wBlueCard;
    private bool wGoldCard;
    private int eStack;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "E", "R", "A" };

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

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage"); //Blue Card
        wKeys.Add("Magic Damage"); //Red Card
        wKeys.Add("Magic Damage"); //Gold Card
        wKeys.Add("Stun Duration");
        eKeys.Add("Bonus Attack Speed");
        eKeys.Add("Bonus Magic Damage");

        MyBuffManager.Add("EPassiveBuff", new AttackSpeedBuff(float.MaxValue, MyBuffManager, ESkill().basic.name, ESkill().UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), "AttackSpeed"));
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, QSkill(), myStats.qLevel, qKeys[0]);
        myStats.qCD = QSkill().basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        if (!wCast)
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        }
        else
        {
            yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
            if (myStats.wSkill[0].UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats) >= targetStats.currentHealth)
            {
                wBlueCard = true;
            }
            else
            {
                wGoldCard = true;
                TargetBuffManager.Add("StunBuff", new StunBuff(WSkill().UseSkill(myStats.wLevel, wKeys[3], myStats, targetStats), TargetBuffManager, "StunBuff"));
            }
            myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
        }
    }
    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        if(eStack >= 3)
		{
            if (wBlueCard)
            {
                if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue)
                {
                    AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), SkillDamageType.Spell));
                    AutoAttack(new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.Spell));
                }
            }
            else if (wGoldCard)
            {
                if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue)
                {
                    AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), SkillDamageType.Spell));
                    AutoAttack(new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.Spell));
                }
            }
            else
			{
                if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue)
                {
                    AutoAttack(new Damage(ESkill().UseSkill(myStats.eLevel, eKeys[1], myStats, targetStats), SkillDamageType.Spell));
                }
            }
            eStack = 0;
        }
		else
		{
            if (wBlueCard)
            {
                if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue)
                {
                    AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[0], myStats, targetStats), SkillDamageType.Spell));
                }
            }
            else if (wGoldCard)
            {
                if (AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal)).damage != float.MinValue)
                {
                    AutoAttack(new Damage(WSkill().UseSkill(myStats.wLevel, wKeys[2], myStats, targetStats), SkillDamageType.Spell));
                }
            }
            else
                AutoAttack(new Damage(myStats.AD, SkillDamageType.Phyiscal));
            eStack++;
        }
    }
}