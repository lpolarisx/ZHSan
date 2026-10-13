using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind595 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleRumour = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleRumour = false;
    }
}