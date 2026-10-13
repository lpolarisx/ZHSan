using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect217 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.LocationArchitecture != null && person.Status == PersonStatus.Princess)
        {
            person.Status = PersonStatus.Normal;
        }
    }
}