using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind370 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DominationDecrementOfCriticalStrike += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DominationDecrementOfCriticalStrike -= influence.GetIntParam();
    }
}