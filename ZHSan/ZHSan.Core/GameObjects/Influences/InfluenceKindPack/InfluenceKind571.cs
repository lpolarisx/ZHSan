using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind571 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleStratagemFromLowerIntelligence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleStratagemFromLowerIntelligence = false;
    }
}