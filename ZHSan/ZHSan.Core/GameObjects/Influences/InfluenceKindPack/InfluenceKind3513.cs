using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3513 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.NavalTrainingFacilityRate += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.NavalTrainingFacilityRate -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return 1;
    }
}