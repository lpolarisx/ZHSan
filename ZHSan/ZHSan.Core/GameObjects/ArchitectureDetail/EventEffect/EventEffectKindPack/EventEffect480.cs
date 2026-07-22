using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect480 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var kindId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllIdealTendencyKinds.TryGetValue(kindId, out var idealTendencyKind))
        {
            person.IdealTendency = idealTendencyKind;
        }
    }
}