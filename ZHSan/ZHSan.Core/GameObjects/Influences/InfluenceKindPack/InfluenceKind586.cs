using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind586 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableAttractOnLowerIntelligence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InevitableAttractOnLowerIntelligence = false;
    }
}