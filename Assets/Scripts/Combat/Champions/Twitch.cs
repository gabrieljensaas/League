using Simulator.Combat;
using System.Collections;

public class Twitch : ChampionCombat
{
    public static float GetTwitchDeadlyVenomByLevel(int level, int stack)
    {
        return level switch
        {
            < 5 => 1f * stack,
            < 9 => 2f * stack,
            < 13 => 3f * stack,
            < 17 => 4f * stack,
            _ => 5f * stack
        };
    }

    public static float GetTwitchContaminateByLevel(int level, int stack) => 10 + (10 * level) + ((10 + (level * 5)) * stack);

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "Q", "W", "R", "A", "E" };

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

        qKeys.Add("Stealth Duration");
        qKeys.Add("Bonus Attack Speed");
        eKeys.Add("Base Physical Damage");
        eKeys.Add("Physical Damage Per Stack");
        rKeys.Add("Bonus Attack Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        //TODO: Ambush still allows twitch to get hit by anything since camo can still be targettable
        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Ambush", new AmbushBuff(myStats.qSkill[0].UseSkill(4, qKeys[0], myStats, targetStats), myStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }
    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));

        if (myStats.buffManager.buffs.TryGetValue("Ambush", out Buff ambush))
            ambush.Kill();

        targetStats.buffManager.buffs.Add("Venom Cask", new VenomCaskBuff(3, targetStats.buffManager, myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));

        //TODO: Pass 35% Bonus AD to contaminate
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats) + ((int)targetStats.buffManager.buffs["Deadly Venom"]?.value * myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats)), myStats.eSkill[0].basic.name, SkillDamageType.Spell);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("SprayAndPray", new AttackDamageBuff(6, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "SprayAndPray"));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("SprayAndPray", new AttackDamageBuff(6, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[0], targetStats, myStats), "SprayAndPray"));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));

        if (myStats.buffManager.buffs.TryGetValue("Ambush", out Buff ambush))
            ambush.Kill();

        AutoAttack();
        CheckDeadlyVenom("Auto Attack");
    }

    private void CheckDeadlyVenom(string skillName)
    {
        if (myStats.buffManager.buffs.TryGetValue("Deadly Venom", out Buff deadlyVenom))
        {
            if (deadlyVenom.value == 6)
            {
                deadlyVenom.duration = 6;
            }
            else
            {
                deadlyVenom.value++;
                deadlyVenom.duration = 6;
            }
        }
        else
        {
            targetStats.buffManager.buffs.Add("Deadly Venom", new DeadlyVenomBuff(6, targetStats.buffManager, skillName));
        }
    }
}