using Simulator.Combat;
using System.Collections;

public class Olaf : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "W", "Q", "A" };

        checksQ.Add(new CheckIfRagnorok(this));
        checksW.Add(new CheckIfRagnorok(this));
        checksE.Add(new CheckIfRagnorok(this));
        checksR.Add(new CheckIfRagnorok(this));
        checksA.Add(new CheckIfRagnorok(this));
        checksQ.Add(new CheckIfCasting(this));
        checksW.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksR.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksQ.Add(new CheckCD(this, "Q"));
        checksW.Add(new CheckCD(this, "W"));
        checksE.Add(new CheckCD(this, "E"));
        checksR.Add(new CheckCD(this, "R"));
        checksA.Add(new CheckCD(this, "A"));
        autoattackcheck = new OlafAACheck(this);
        checkTakeDamageAbility.Add(new CheckOlafPassive(this));
        checkTakeDamageAA.Add(new CheckOlafPassive(this));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));
        checkTakeDamageAbilityPostMitigation.Add(new CheckShield(this));
        checkTakeDamageAAPostMitigation.Add(new CheckShield(this));

        qKeys.Add("Physical Damage");
        wKeys.Add("Bonus Attack Speed");
        wKeys.Add("Shield Strength");
        eKeys.Add("True Damage");
        rKeys.Add("Bonus Attack Damage");

        myUI.combatPriority.text = string.Join(", ", combatPrio);
        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        targetStats.buffManager.buffs.Add(myStats.qSkill[0].basic.name, new ArmorReductionBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name, 20f, myStats.qSkill[0].basic.name));
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add(myStats.wSkill[0].basic.name, new AttackSpeedBuff(4, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[0], myStats, targetStats), myStats.wSkill[0].basic.name));
        myStats.buffManager.shields.Add(myStats.wSkill[0].basic.name, new ShieldBuff(2.5f, myStats.buffManager, myStats.wSkill[0].basic.name, myStats.wSkill[0].UseSkill(4, wKeys[1], myStats, targetStats, 0.7f), myStats.wSkill[0].basic.name));
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        TakeDamage(new Damage(75 + (myStats.AD * 0.15f), SkillDamageType.True), myStats.eSkill[0].basic.name); //health cost
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0], 4, eKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        if (myStats.buffManager.buffs.TryGetValue(myStats.rSkill[0].basic.name, out Buff value))
        {
            value.duration += 2.5f;
        }
        if (myStats.buffManager.buffs.TryGetValue(myStats.rSkill[0].basic.name + " ", out Buff val))
        {
            val.duration += 2.5f;
        }
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(3, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), myStats.rSkill[0].basic.name));
        myStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name + " ", new ImmuneToCCBuff(3, myStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].basic.name + " "));
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return targetCombat.StartCoroutine(targetCombat.StartCastingAbility(myStats.rSkill[0].basic.castTime));
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[2] * 2;
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name, new AttackDamageBuff(3, targetStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].SylasUseSkill(skillLevel, rKeys[0], targetStats, myStats), myStats.rSkill[0].basic.name));
        targetStats.buffManager.buffs.Add(myStats.rSkill[0].basic.name + " ", new ImmuneToCCBuff(3, targetStats.buffManager, myStats.rSkill[0].basic.name, myStats.rSkill[0].basic.name + " "));
    }
}