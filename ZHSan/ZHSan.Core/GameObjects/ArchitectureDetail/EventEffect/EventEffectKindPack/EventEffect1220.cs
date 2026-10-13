using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1220 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var kindId = eventEffect.GetIntParam();

        foreach (var facility in arch.Facilities)
        {
            if (facility.KindID == kindId)
            {
                arch.DemolishFacility(facility);
                return;
            }
        }
    }
}