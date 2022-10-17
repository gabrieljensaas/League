using UnityEngine;

public class UmbraBladeBuff : Buff
{
    public UmbraBladeBuff(float duration, BuffManager manager) : base(manager)
    {
        value = 1;
        base.source = source;

    }

    public override void Update()
    {
    }
    public override void Kill()
    {
        duration = -Time.fixedDeltaTime;
        manager.buffs.Remove("UmbraBladeBuff");
    }
}