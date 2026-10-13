using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind570 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableStratagemOnLowerIntelligence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableStratagemOnLowerIntelligence = false;
    }
}