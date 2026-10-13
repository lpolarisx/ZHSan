using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind585 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableRumourOnLowerIntelligence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableRumourOnLowerIntelligence = false;
    }
}