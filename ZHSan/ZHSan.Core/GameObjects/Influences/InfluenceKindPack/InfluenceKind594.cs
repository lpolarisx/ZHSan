using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind594 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleHuogong = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.InvincibleHuogong = false;
    }
}