using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind580 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableGongxinOnLowerIntelligence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableGongxinOnLowerIntelligence = false;
    }
}