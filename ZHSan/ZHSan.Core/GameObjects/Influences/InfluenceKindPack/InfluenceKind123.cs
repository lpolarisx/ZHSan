using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind123 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidPopulationEscape = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidPopulationEscape = false;
    }
}