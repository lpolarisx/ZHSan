using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect450 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        person.ValuationOnGovernment = (PersonValuationOnGovernment)eventEffect.GetIntParam();
    }
}