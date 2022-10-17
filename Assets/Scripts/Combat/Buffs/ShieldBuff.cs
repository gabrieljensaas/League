using UnityEngine;

public class ShieldBuff : Buff
{
    public string uniqueKey;
    public bool decaying = false;
    public ShieldType shieldType;

    public enum ShieldType
    {
        Normal,
        Magic,
        Physical
    }

    public ShieldBuff(float duration, BuffManager manager, string source, float shield, string uniqueKey, bool decaying = false, ShieldType shieldType = ShieldType.Normal) : base(manager)
    {
        this.decaying = decaying;
        value = shield;
        base.duration = float.MaxValue;
        base.source = source;
        manager.simulationManager.ShowText($"{manager.stats.name} Gained {shield} Shield from {source} for {duration} Seconds!");
        this.uniqueKey = uniqueKey;
        this.decaying = decaying;
        this.shieldType = shieldType;
    }

    public override void Update()
    {
        duration -= Time.fixedDeltaTime;
        if (decaying) value -= (Time.fixedDeltaTime / duration) * value;
        if (duration <= 0) Kill();
    }
    public override void Kill()
    {
        if (manager.buffs.TryGetValue("ThunderclapArmor", out Buff thunderclapTripledArmor)) //malphite
            thunderclapTripledArmor.Kill();

        manager.simulationManager.ShowText($"{manager.stats.name} Has No Longer Shield From {source}!");
        manager.buffs.Remove(uniqueKey);
    }
}