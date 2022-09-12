using System.Collections;
using Simulator.Combat;

public class Katarina : ChampionCombat
{
    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "E", "Q", "W", "R", "A" };

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
        checksQ.Add(new CheckIfChanneling(this));
        checksW.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfChanneling(this));
        checksR.Add(new CheckIfChanneling(this));
        checksA.Add(new CheckIfChanneling(this));
        checksE.Add(new CheckIfImmobilize(this));
        checksA.Add(new CheckIfDisarmed(this));

        qKeys.Add("Magic Damage");
        wKeys.Add("Bonus Movement speed");
        eKeys.Add("Magic Damage");
        rKeys.Add("Physical Damage Per Dagger");
        rKeys.Add("Maximum Single-Target Magic Damage");
        rKeys.Add("Maximum Single-Target Physical Damage");
        rKeys.Add("Magic Damage Per Dagger");
        rKeys.Add("On-Attack/On-Hit Effectiveness");

        base.UpdatePriorityAndChecks();
    }

    protected void Passive()
    {
        UpdateAbilityTotalDamage(ref qSum, 0, myStats.qSkill[0], 4, qKeys[0]);
        myStats.eCD = myStats.eSkill[0].basic.coolDown[0];
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        Passive();
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        Passive();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        attackCooldown = 0.0f;
    }
    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));
        myStats.buffManager.buffs.Add("Channeling", new ChannelingBuff(10, myStats.buffManager, myStats.rSkill[0].basic.name, "Death Lotus"));
        UpdateAbilityTotalDamage(ref rSum, 3, myStats.rSkill[0], 2, rKeys[0]);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
    }

    }
