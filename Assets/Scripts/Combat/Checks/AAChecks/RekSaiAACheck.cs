using Simulator.Combat;
using Unity.VisualScripting;

public class RekSaiAACheck : Check
{
    private RekSai reksai;
    public RekSaiAACheck(ChampionCombat ccombat, RekSai reksai) : base(ccombat)
    {
        this.reksai = reksai;
    }

    public override float Control(float damage)
    {
        if (reksai.autoattackQ > 1)
        {
            damage += combat.QSkill().UseSkill(4, combat.qKeys[0], combat.myStats, combat.targetStats);
            reksai.autoattackQ--;
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}