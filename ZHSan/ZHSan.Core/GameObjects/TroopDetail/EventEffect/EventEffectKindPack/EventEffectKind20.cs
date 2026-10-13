using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind20 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.IncreaseCombativity(eventEffect.GetIntParam());
        }
    }
}