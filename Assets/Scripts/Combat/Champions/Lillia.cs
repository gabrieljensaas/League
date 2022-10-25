using Simulator.Combat;
using System.Collections;

public class Lillia : ChampionCombat
{
    public static float DreamDust(int level) => 10.5f + (114 / 17 * (level - 1));

    public override void UpdatePriorityAndChecks()
    {
        combatPrio = new string[] { "R", "E", "Q", "A", "W" };

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
        qKeys.Add("Total Mixed Damage");

        wKeys.Add("Magic Damage");
        wKeys.Add("Increased Damage");

        eKeys.Add("Magic Damage");

        rKeys.Add("Sleep Duration");
        rKeys.Add("Magic Damage");

        base.UpdatePriorityAndChecks();
    }

    public override IEnumerator ExecuteA()
    {
        yield return base.ExecuteA();
        LiltingLullabyProc();
    }

    public override IEnumerator ExecuteQ()
    {
        if (!CheckForAbilityControl(checksQ)) yield break;

        yield return StartCoroutine(StartCastingAbility(QSkill().basic.castTime));
        UpdateTotalDamage(ref qSum, 0, QSkill(), 4, qKeys[1]);
        myStats.qCD = QSkill().basic.coolDown[4];
        DreamDust();
        LiltingLullabyProc();
    }

    public override IEnumerator ExecuteW()
    {
        if (!CheckForAbilityControl(checksW)) yield break;

        yield return StartCoroutine(StartCastingAbility(WSkill().basic.castTime));
        UpdateTotalDamage(ref wSum, 1, WSkill(), 4, wKeys[1]);
        myStats.wCD = WSkill().basic.coolDown[4];
        DreamDust();
        LiltingLullabyProc();
    }

    public override IEnumerator ExecuteE()
    {
        yield return base.ExecuteE();
        DreamDust();
        LiltingLullabyProc();
    }

    public override IEnumerator ExecuteR()
    {
        if (TargetBuffManager.buffs.ContainsKey("DreamDust"))
        {
            if (!CheckForAbilityControl(checksR)) yield break;

            yield return StartCoroutine(StartCastingAbility(RSkill().basic.castTime));
            TargetBuffManager.Add("Drowsy", new DrowsyBuff(1.5f, TargetBuffManager, RSkill().name, RSkill().UseSkill(4, rKeys[0], myStats, targetStats)));
            myStats.rCD = RSkill().basic.coolDown[2];
        }
    }

    private void DreamDust()
    {
        if (TargetBuffManager.buffs.TryGetValue("DreamDust", out Buff buff))
            buff.duration = 3;
        else
            TargetBuffManager.Add("DreamDust", new DreamDustBuff(3, TargetBuffManager, myStats.passiveSkill.name));
    }

    private void LiltingLullabyProc()
    {
        if (TargetBuffManager.buffs.TryGetValue("Sleep", out Buff buff))
        {
            buff.Kill();
            UpdateTotalDamage(ref rSum, 3, RSkill(), 2, rKeys[1]);
        }
    }
}
