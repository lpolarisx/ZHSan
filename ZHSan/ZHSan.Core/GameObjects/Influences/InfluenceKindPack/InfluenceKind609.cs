using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind609 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceNoChangeOnChaos = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceNoChangeOnChaos = false;
    }
}