using Simulator.Combat;
using System.Collections;
using System.Runtime.InteropServices;

public class Annie : ChampionCombat
{
    public static float GetAnnieStunDurationByLevel(int level)
    {
        if (level < 6) return 1.25f;
        if (level < 11) return 1.5f;
        return 1.75f;
    }

    private CheckAnnieP annieP;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

        annieP = new CheckAnnieP(this);
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
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        checkTakeDamageAbility.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckShield(this));
        checkTakeDamageAA.Add(new CheckMoltenShield(this));

        qKeys.Add("Magic damage");
        wKeys.Add("Magic damage");
        eKeys.Add("Shield Strength");
        eKeys.Add("Magic Damage");
        rKeys.Add("Initial Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();

        foreach (var item in pets)
        {
            item.Update();
        }
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        CheckAnniePassiveStun(myStats.qSkill[0].basic.name);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        CheckAnniePassiveStun(myStats.wSkill[0].basic.name);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[0]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff buff) && buff.value < 4)
        {
            buff.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {myStats.eSkill[0].basic.name}");
        }
        else
        {
            myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, myStats.eSkill[0].basic.name));
        }
        myStats.buffManager.buffs.Add(myStats.eSkill[0].basic.name, new ShieldBuff(3, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        CheckAnniePassiveStun(myStats.rSkill[0].basic.name);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        pets.Add(new Tibbers(this, 3100, 100 + (myStats.AP * 15 / 100), 0.625f, 90, 90)); //all stats are for max level change when level adjusting of skills done
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0]);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        targetCombat.pets.Add(new Tibbers(this, 3100, 100 + (myStats.AP * 15 / 100), 0.625f, 90, 90)); //all stats are for max level change when level adjusting of skills done
    }

    private void CheckAnniePassiveStun(string skillName)
    {
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
        }
        else if (myStats.buffManager.buffs.TryGetValue("Pyromania", out Buff pyromania))
        {
            pyromania.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Pyromania From {skillName}");
        }
        else
        {
            myStats.buffManager.buffs.Add("Pyromania", new PyromaniaBuff(myStats.buffManager, skillName));
        }
    }
}