using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect270 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedFaction == null || person.LocationArchitecture == null || person.BelongedCaptive != null) return;

        var generatorTypeId = eventEffect.GetIntParam();
        if (Session.Current.Scenario.GameCommonData.AllPersonGeneratorTypes.TryGetValue(generatorTypeId, out var type))
        {
            person.LocationArchitecture.GenerateOfficer(type, true);
        }
    }
}