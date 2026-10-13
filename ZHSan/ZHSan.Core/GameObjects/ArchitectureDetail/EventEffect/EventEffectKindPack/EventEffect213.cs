using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect213 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedCaptive != null)
        {
            person.SetBelongedCaptive(null, PersonStatus.Princess);
        }
        
        if (person.LocationArchitecture != null)
        {
            person.Status = PersonStatus.Princess;
        }
    }
}