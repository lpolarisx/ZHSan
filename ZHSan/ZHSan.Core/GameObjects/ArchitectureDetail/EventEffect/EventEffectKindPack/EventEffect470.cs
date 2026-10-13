using System.Runtime.Serialization;
using GameEnums;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect470 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        person.StrategyTendency = (PersonStrategyTendency)eventEffect.GetIntParam();
    }
}