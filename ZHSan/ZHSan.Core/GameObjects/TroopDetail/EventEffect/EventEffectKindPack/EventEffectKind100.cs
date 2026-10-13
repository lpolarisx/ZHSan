using GameManager;
using GameObjects.Influences;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind100 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;
        var influenceId = eventEffect.GetIntParam();

        if (troop != null && Session.Current.Scenario.GameCommonData.AllInfluences.TryGetValue(influenceId, out var influence))
        {
            troop.EventInfluences.Add(influence);
            influence.ApplyInfluence(troop, Applier.Event, 0);
        }
    }

    public override void ApplyEffectKind(EventEffect eventEffect, Troop troop)
    {
        var influenceId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllInfluences.TryGetValue(influenceId, out var influence))
        {
            troop.EventInfluences.Add(influence);
            influence.ApplyInfluence(troop, Applier.Event, 0);
        }
    }
}