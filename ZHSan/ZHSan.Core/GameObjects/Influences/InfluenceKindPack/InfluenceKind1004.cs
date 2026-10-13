using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1004 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfDominationCeiling += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfDominationCeiling -= influence.GetIntParam();
    }
}