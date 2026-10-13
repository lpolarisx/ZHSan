using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind606 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NotAfraidOfFire = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NotAfraidOfFire = false;
    }
}