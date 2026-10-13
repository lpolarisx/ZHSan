using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect225 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedFactionWithPrincess != null)
        {
            person.BelongedFaction.Leader.AddHated(person);
        }
    }
}