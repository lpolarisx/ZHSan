using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6165 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.PoliticsExperienceIncrease += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.PoliticsExperienceIncrease -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetIntParam() * arch.PersonCount / 200;
    }
}