using Simulator.Combat;
using System.Collections;

public class Kalista : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "A", "", "" };

        checksQ.Add(new CheckCD(this, "Q"));
        checksE.Add(new CheckCD(this, "E"));
        checksA.Add(new CheckCD(this, "A"));
        checksQ.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfCasting(this));
        checksA.Add(new CheckIfCasting(this));
        checksE.Add(new CheckIfEnemyHasRend(this));
        checksE.Add(new CheckIfExecutes(this, "Kalista"));
        checksQ.Add(new CheckIfDisrupt(this));
        checksW.Add(new CheckIfDisrupt(this));
        checksE.Add(new CheckIfDisrupt(this));
        checksR.Add(new CheckIfDisrupt(this));
        checksA.Add(new CheckIfTotalCC(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Physical Damage");
        eKeys.Add("Physical Damage");
        eKeys.Add("Damage per Additional Spear");

        base.UpdatePriorityAndChecks();
    }
    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.qSkill[0].basic.castTime));
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.qCD = myStats.qSkill[0].basic.coolDown[4];
        if (targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value))
        {
            value.value++;
            value.duration = 4;
        }
        else
        {
            targetStats.buffManager.buffs.Add("Rend", new RendBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value);
        UpdateAbilityTotalDamage(ref eSum, 2, myStats.eSkill[0].UseSkill(4, eKeys[0], myStats, targetStats) + ((value.value - 1) * myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats)), myStats.eSkill[0].basic.name, SkillDamageType.Phyiscal);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        value.Kill();
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield break;
    }

    public override IEnumerator ExecuteA()
    {
        if (!CheckForAbilityControl(checksA)) yield break;

        yield return StartCoroutine(StartCastingAbility(0.1f));
        AutoAttack(0.9f);
        if (targetStats.buffManager.buffs.TryGetValue("Rend", out Buff value))
        {
            value.value++;
            value.duration = 4;
        }
        else
        {
            targetStats.buffManager.buffs.Add("Rend", new RendBuff(4, targetStats.buffManager, myStats.qSkill[0].basic.name));
        }
    }
}