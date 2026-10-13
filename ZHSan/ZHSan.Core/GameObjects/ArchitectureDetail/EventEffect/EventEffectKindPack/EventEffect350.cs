using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect350 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var faction = person.BelongedFaction;

        if (person != faction?.Leader)
        {
            int idealOffset = Person.GetIdealOffset(person, faction.Leader);

            idealOffset += eventEffect.GetIntParam();
        }

    }
}