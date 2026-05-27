using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind381 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseDefenceConsidered = influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseDefenceConsidered = 0;
    }
}