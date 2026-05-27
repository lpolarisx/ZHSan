using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind388 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseNoCounterAttack = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseNoCounterAttack = false;
    }
}