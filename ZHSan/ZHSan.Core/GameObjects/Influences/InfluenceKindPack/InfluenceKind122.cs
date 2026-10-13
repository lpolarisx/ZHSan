using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind122 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidInfluenceByBattle = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.DayAvoidInfluenceByBattle = false;
    }
}