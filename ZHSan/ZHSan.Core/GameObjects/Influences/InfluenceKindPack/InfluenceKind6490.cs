using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6490 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.TroopTransportFundRate += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.TroopTransportFundRate -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetFloatParam() * 160;
    }
}