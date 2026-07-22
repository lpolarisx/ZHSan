using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect320 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var stuntId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllStunts.TryGetValue(stuntId, out var stunt))
        {
            person.AddStunt(stunt);
        }
    }
}