using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1130 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture a, Event e)
    {
        var quantity = eventEffect.GetIntParam();
        var type = eventEffect.GetIntParam2();

        foreach (Military military in a.Militaries)
        {
            if (military.KindID == type)
            {
                military.IncreaseQuantity(quantity);
            }
        }
    }
}