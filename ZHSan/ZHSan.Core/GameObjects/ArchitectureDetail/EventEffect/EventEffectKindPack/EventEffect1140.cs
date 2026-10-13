using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1140 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var quantity = eventEffect.GetIntParam();
        var type = eventEffect.GetIntParam2();

        foreach (Military military in arch.Militaries)
        {
            if (military.KindID == type)
            {
                military.IncreaseMorale(quantity);
            }
        }
    }
}