using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind121 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.InfluenceIncrementOfLoyalty += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.InfluenceIncrementOfLoyalty -= influence.GetIntParam();
    }
}