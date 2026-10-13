using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind592 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NeverBeIntoChaosWhileWaylay = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NeverBeIntoChaosWhileWaylay = false;
    }
}