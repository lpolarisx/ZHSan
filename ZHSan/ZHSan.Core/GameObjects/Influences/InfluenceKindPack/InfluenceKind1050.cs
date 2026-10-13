using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind1050 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.RateIncrementOfPopulationCeiling += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.RateIncrementOfPopulationCeiling -= influence.GetFloatParam();
    }
}