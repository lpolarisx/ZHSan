using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1230 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        while (arch.Facilities.Count > 0)
        {
            arch.DemolishFacility(arch.Facilities[0]);
        }
    }
}