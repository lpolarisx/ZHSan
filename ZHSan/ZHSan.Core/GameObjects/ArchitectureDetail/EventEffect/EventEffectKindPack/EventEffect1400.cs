using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1400 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var id = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllArchitectureKinds.TryGetValue(id, out var architectureKind))
        {
            arch.Kind = architectureKind;
        }
    }
}