using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect1250 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Architecture arch, Event e)
    {
        var characteristics = arch.Characteristics.Values;
        var applier = Applier.Characteristics;
        var id = 0;

        Influence.PurifyInfluenceList(characteristics, arch, applier, id);
        arch.Characteristics.Remove(eventEffect.GetIntParam());
        Influence.ApplyInfluenceList(characteristics, arch, applier, id);
    }
}