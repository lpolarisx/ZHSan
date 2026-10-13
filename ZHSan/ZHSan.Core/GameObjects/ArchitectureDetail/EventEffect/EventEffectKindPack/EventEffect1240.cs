using GameManager;
using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1240 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var influenceId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllInfluences.TryGetValue(influenceId, out var influence))
        {
            var characteristics = arch.Characteristics.Values;
            var applier = Applier.Characteristics;
            var id = 0;

            Influence.PurifyInfluenceList(characteristics, arch, applier, id);
            arch.Characteristics.TryAdd(influenceId, influence);
            Influence.ApplyInfluenceList(characteristics, arch, applier, id);
        }
    }
}