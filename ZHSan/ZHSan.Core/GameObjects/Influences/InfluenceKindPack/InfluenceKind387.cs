using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind387 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseAttackEveryAround = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseAttackEveryAround = false;
    }
}