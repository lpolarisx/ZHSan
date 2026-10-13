using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect222 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedFactionWithPrincess != null)
        {
            var leader = person.BelongedFaction.Leader;

            person.Brothers.Add(leader);
            leader.Brothers.Add(person);
        }
    }
}