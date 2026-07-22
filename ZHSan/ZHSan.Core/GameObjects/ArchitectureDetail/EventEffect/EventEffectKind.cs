namespace GameObjects.ArchitectureDetail.EventEffect;

/// <summary>
/// 事件影响种类
/// </summary>
public class EventEffectKind : GameObject
{
    protected static Person markedPerson = null;

    public virtual void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
    }
   
    public virtual void ApplyEffectKind(EventEffect eventEffect, Architecture architecture, Event e)
    {
        if (architecture.Mayor != null)
        {
            ApplyEffectKind(eventEffect, architecture.Mayor, e);
        }
    }

    public virtual void ApplyEffectKind(EventEffect eventEffect, Faction faction, Event e)
    {
        ApplyEffectKind(eventEffect, faction.Leader, e);
    }
}