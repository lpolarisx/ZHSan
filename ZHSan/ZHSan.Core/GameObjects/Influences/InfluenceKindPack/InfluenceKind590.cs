using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind590 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleGongxin = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleGongxin = false;
    }
}