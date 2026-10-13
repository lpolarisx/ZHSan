using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect240 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var princess = person.BelongedFactionWithPrincess;

        if (princess != null)
        {
            princess.Leader.AdjustRelation(person, 0, eventEffect.GetIntParam());
        }
    }
}