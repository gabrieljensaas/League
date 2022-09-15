using Simulator.Combat;
using System.Collections;

public class Veigar : ChampionCombat
{
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

        qKeys.Add("Magic Damage");
        wKeys.Add("Magic Damage");
        eKeys.Add("Stun Duration");
        rKeys.Add("Minimum Damage");
        rKeys.Add("Maximum Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteQ()
    {
        yield return base.ExecuteQ();
        PhenomenalEvilPowerStack();
    }

    public override IEnumerator ExecuteW()
    {
        yield return base.ExecuteW();
        PhenomenalEvilPowerStack();
    }

    public override IEnumerator ExecuteE()
    {
        if (!CheckForAbilityControl(checksE)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.eSkill[0].basic.castTime));
        targetStats.buffManager.buffs.Add("Stun", new StunBuff(myStats.eSkill[0].UseSkill(4, eKeys[1], myStats, targetStats), targetStats.buffManager, myStats.eSkill[0].basic.name));
        myStats.eCD = myStats.eSkill[0].basic.coolDown[4];
        PhenomenalEvilPowerStack();
    }

    public override IEnumerator ExecuteR()
    {
        if (!CheckForAbilityControl(checksR)) yield break;

        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));

        float missingHealthModifier = ((1 - targetStats.currentHealth / targetStats.maxHealth) * 1.5f) + 1;
        if (missingHealthModifier > 2) missingHealthModifier = 2;
        float damage = 325 * missingHealthModifier;
        UpdateAbilityTotalDamage(ref rSum, 3, damage, myStats.rSkill[0].name, SkillDamageType.Spell);
        myStats.rCD = myStats.rSkill[0].basic.coolDown[2];
        PhenomenalEvilPowerStack();
    }

    public override IEnumerator HijackedR(int skillLevel)
    {
        yield return StartCoroutine(StartCastingAbility(myStats.rSkill[0].basic.castTime));

        float missingHealthModifier = ((1 - myStats.currentHealth / myStats.maxHealth) * 1.5f) + 1;
        if (missingHealthModifier > 2) missingHealthModifier = 2;
        float damage = 325 * missingHealthModifier;
        UpdateAbilityTotalDamageSylas(ref targetCombat.rSum, 3, damage, myStats.rSkill[0].name, SkillDamageType.Spell);
        targetStats.rCD = myStats.rSkill[0].basic.coolDown[skillLevel] * 2;
    }

    private void PhenomenalEvilPowerStack() => myStats.AP++;
}