using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1110 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var quantity = eventEffect.GetIntParam();

        foreach (Military military in arch.Militaries)
        {
            military.IncreaseMorale(quantity);
        }
    }
}