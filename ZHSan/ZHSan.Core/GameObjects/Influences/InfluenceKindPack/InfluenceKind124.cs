using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind124 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidInternalDecrementOnBattle = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidInternalDecrementOnBattle = false;
    }
}