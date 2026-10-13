namespace GameObjects.TroopDetail.EventEffect;

/// <summary>
/// 部队事件影响类型
/// </summary>
public class EventEffectKind : GameObject
{
    public virtual void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
    }

    public virtual void ApplyEffectKind(EventEffect eventEffect, Troop troop)
    {
    }
}