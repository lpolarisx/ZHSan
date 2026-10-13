using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind601 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NeverBeIntoChaos = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.NeverBeIntoChaos = false;
    }
}