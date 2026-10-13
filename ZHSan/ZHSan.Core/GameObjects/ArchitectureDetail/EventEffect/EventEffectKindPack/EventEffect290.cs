using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect290 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var faction = person.BelongedFaction;

        if (faction == null && person.LocationArchitecture != null && person.BelongedCaptive == null)
        {
            Session.Current.Scenario.CreateNewFaction(person);
        }
        else if (faction != null && person != faction.Leader && person.LocationArchitecture != faction.Capital)
        {
            Session.Current.Scenario.CreateNewFaction(person);
        }
    }
}