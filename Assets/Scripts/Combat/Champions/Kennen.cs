using Simulator.Combat;
using System.Collections;
using UnityEngine;

public class Kennen : ChampionCombat
{
    private CheckKennenP kennenP;
    private float passiveStunTimer;
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "W", "Q", "R", "A" };

        kennenP = new CheckKennenP(this);
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

        qKeys.Add("Magic Damage");
        wKeys.Add("Bonus Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Magic Damage");
        rKeys.Add("Magic Damage Per Bolt");

        base.UpdatePriorityAndChecks();
    }

    public override void CombatUpdate()
    {
        base.CombatUpdate();
        passiveStunTimer += Time.deltaTime;
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        CheckKennenPassiveStun(myStats.qSkill[0].basic.name);
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.wSkill[0].basic.castTime));
        CheckKennenPassiveStun(myStats.wSkill[0].basic.name);
        UpdateAbilityTotalDamage(ref wSum, 1, myStats.wSkill[0], 4, wKeys[1]);
        myStats.wCD = myStats.wSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("CantAA", new CantAABuff(2f, myStats.buffManager, myStats.eSkill[0].basic.name));
        CheckKennenPassiveStun(myStats.eSkill[0].basic.name);
        myStats.buffManager.buffs.Add("AttackSpeedBuff", new AttackSpeedBuff(4f, myStats.buffManager, myStats.eSkill[0].basic.name, myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats), "AttackSpeedBuff"));
        yield return new WaitForSeconds(4f);
        myStats.buffManager.buffs.Remove("AttackSpeedBuff");
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("BonusArmor", new ArmorBuff(3f, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusArmor"));
        myStats.buffManager.buffs.Add("BonusMR", new MagicResistanceBuff(3f, myStats.buffManager, myStats.rSkill[0].basic.name, (int)myStats.rSkill[0].UseSkill(2, rKeys[0], myStats, targetStats), "BonusMR"));
        CheckKennenPassiveStun(myStats.rSkill[0].basic.name);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        StartCoroutine(MaelStorm());
    }

    public IEnumerator MaelStorm()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        yield return new WaitForSeconds(0.5f);
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
    }

    private void CheckKennenPassiveStun(string skillName)
    {
        if (kennenP.Control())
        {
            if (passiveStunTimer<6)
            {
                targetStats.buffManager.buffs.Add("Stun", new StunBuff(0.5f, targetStats.buffManager, myStats.passiveSkill.skillName));
            }
			else
			{
                targetStats.buffManager.buffs.Add("Stun", new StunBuff(1.25f, targetStats.buffManager, myStats.passiveSkill.skillName));
                passiveStunTimer = 0;
            }
            myStats.buffManager.buffs.Remove("MarkOfTheStorm");
        }
        else if (myStats.buffManager.buffs.TryGetValue("MarkOfTheStorm", out Buff markOfTheStorm))
        {
            markOfTheStorm.value++;
            simulationManager.ShowText($"{myStats.name} Gained A Stack of Mark Of The Storm From {skillName}");
        }
        else
        {
            myStats.buffManager.buffs.Add("MarkOfTheStorm", new MarkOfTheStormBuff(6,myStats.buffManager, skillName));
        }
    }
}