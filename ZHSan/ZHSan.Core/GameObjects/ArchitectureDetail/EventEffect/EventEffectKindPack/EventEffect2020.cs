using GameManager;
using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect2020 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Faction faction, Event e)
    {
        var techniqueId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllTechniques.TryGetValue(techniqueId, out var technique))
        {
            faction.AddTechnique(technique);

            Session.Current.Scenario.NewInfluence = true;
            Influence.ApplyInfluenceList(technique.Influences, faction, Applier.Technique, techniqueId);
            Session.Current.Scenario.NewInfluence = false;
        }
    }
}