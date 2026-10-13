using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect200 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedFaction?.Leader != person)
        {
            person.TempLoyaltyChange += eventEffect.GetIntParam();
        }
    }
}