using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1003 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfEnduranceCeiling += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfEnduranceCeiling -= influence.GetIntParam();
    }
}