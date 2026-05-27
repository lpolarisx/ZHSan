using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind261 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ImmunityOfCaptive = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ImmunityOfCaptive = false;
    }
}