using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind630 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.IncrementOffenceRate += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.IncrementOffenceRate -= influence.GetFloatParam();
    }
}
