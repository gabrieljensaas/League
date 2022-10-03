using Simulator.Combat;

public class IreliaAACheck : Check
{
    public IreliaAACheck(ChampionCombat ccombat) : base(ccombat)
    {
    }

    public override float Control(float damage, SkillDamageType damageType, SkillComponentTypes componentTypes)
    {
        if (combat.myStats.buffManager.buffs.TryGetValue("IonianFervor", out Buff buff))
        {
            if(combat.myStats.buffManager.buffs.TryGetValue("IonianFervorAS", out Buff ASbuff))
                ASbuff.Kill();
                
            combat.myStats.buffManager.buffs.Add("IonianFervorAS", new AttackSpeedBuff(6, combat.myStats.buffManager, "IonianFervor", Irelia.IonianFervorAttackSpeed(combat.myStats.level, (int)buff.value), "IonianFervorAS"));

            if (buff.value == 4)
            {
                damage += Irelia.IonianFervorEmpoweredDamage(combat.myStats.level) + combat.myStats.bonusAD;
            }
        }
        return damage;
    }

    public override bool Control()
    {
        throw new System.NotImplementedException();
    }
}