using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1400 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        arch.Kind = Session.Current.Scenario.GameCommonData.AllArchitectureKinds.Get(eventEffect.GetIntParam());
    }
}