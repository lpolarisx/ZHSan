using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind50 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayRateIncrementOfInternal += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayRateIncrementOfInternal -= influence.GetFloatParam();
    }
}