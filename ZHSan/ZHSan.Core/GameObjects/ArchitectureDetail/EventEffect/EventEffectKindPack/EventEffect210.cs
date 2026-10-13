using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect210 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var location = person.LocationArchitecture;

        if (location != null)
        {
            if (person.BelongedFaction == null)
            {
                person.Status = PersonStatus.Normal;
                person.ChangeFaction(location.BelongedFaction);
            }
            else if (person.BelongedCaptive != null)
            {
                Faction faction = person.BelongedCaptive.LocationArchitecture.BelongedFaction;
                person.SetBelongedCaptive(null, PersonStatus.Normal);
                person.ChangeFaction(faction);
            }
        }
    }
}