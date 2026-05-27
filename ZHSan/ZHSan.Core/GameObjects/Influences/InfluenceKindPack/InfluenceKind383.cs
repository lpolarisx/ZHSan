using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind383 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BasePierceAttack = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BasePierceAttack = false;
    }
}