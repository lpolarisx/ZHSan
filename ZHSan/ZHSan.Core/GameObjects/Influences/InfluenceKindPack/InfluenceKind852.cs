using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind852 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ProhibitAllAction = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ProhibitAllAction = false;
    }
}