using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect230 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var princess = person.BelongedFactionWithPrincess;

        if (princess != null)
        {
            person.AdjustRelation(princess.Leader, 0, eventEffect.GetIntParam());
        }
    }
}