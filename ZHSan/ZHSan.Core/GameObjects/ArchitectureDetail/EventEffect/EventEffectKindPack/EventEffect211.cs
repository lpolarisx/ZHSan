using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect211 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var location = person.LocationArchitecture;

        if (location != null)
        {
            if (person.BelongedFaction != null)
            {
                person.LeaveToNoFaction();
            }
            else if (person.BelongedCaptive != null)
            {
                person.BelongedCaptive.CaptiveDirectEscape();
            }
        }
    }
}