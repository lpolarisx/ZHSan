using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind582 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableChaosOnWaylay = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableChaosOnWaylay = false;
    }
}