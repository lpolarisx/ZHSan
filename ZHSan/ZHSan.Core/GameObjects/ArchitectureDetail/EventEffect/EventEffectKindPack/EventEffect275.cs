using GameGlobal;
using GameManager;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect275 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        if (person.BelongedFaction == null || person.LocationArchitecture == null || person.BelongedCaptive != null) return;

        var allPersonGeneratorTypes = Session.Current.Scenario.GameCommonData.AllPersonGeneratorTypes.Values.ToList();
        var type = StaticMethods.GetRandomItem(allPersonGeneratorTypes);
        person.LocationArchitecture.GenerateOfficer(type, true);
    }
}