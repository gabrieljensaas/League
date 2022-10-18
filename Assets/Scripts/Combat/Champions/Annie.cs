using Simulator.Combat;
using System.Collections;

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
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckMoltenShield(this));

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
        if (myStats.qLevel == -1) yield break;
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        if (CheckAnniePassiveStun(myStats.qSkill[0].basic.name))
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], myStats.qLevel, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable, buffNames: new string[] { "Stun"});
        else 
            UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0], skillComponentTypes: SkillComponentTypes.Projectile | SkillComponentTypes.Spellblockable);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[myStats.qLevel];
    }

    public override IEnumerator ExecuteW()
    {
        if (myStats.wLevel == -1) yield break;
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        if (CheckAnniePassiveStun(myStats.wSkill[0].basic.name))
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] {"Stun"});
        else
            UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], myStats.wLevel, wKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[myStats.wLevel];
    }

    public override IEnumerator ExecuteE()
    {
        if (myStats.eLevel == -1) yield break;
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
        myStats.buffManager.buffs.Add(myStats.eSkill[0].basic.name, new ShieldBuff(3, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(myStats.eLevel, eKeys[0], myStats, targetStats), myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[myStats.eLevel];
    }

    public override IEnumerator ExecuteR()
    {
        if (myStats.rLevel == -1) yield break;
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        if(CheckAnniePassiveStun(myStats.rSkill[0].basic.name))
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable, buffNames: new string[] {"Stun"});
        else
            UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], myStats.rLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[myStats.rLevel];
        pets.Add(new Tibbers(this, 3100, ((myStats.rLevel + 1) / 2 * 50) + (myStats.AP * 15 / 100), 0.625f, 90, 90)); //health, armor and mresist are for max level change when level adjusting of skills done
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, myStats.rSkill[0], skillLevel, rKeys[0], skillComponentTypes: SkillComponentTypes.Spellblockable);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
        targetCombat.pets.Add(new Tibbers(this, 3100, ((skillLevel + 1) / 2 * 50) + (myStats.AP * 15 / 100), 0.625f, 90, 90)); //health, armor and mresist are for max level change when level adjusting of skills done
    }

    private bool CheckAnniePassiveStun(string skillName)
    {
        if (annieP.Control())
        {
            targetStats.buffManager.buffs.Add("Stun", new StunBuff(GetAnnieStunDurationByLevel(myStats.level), targetStats.buffManager, myStats.passiveSkill.skillName));
            myStats.buffManager.buffs.Remove("Pyromania");
            return true;
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
        return false;
    }
}