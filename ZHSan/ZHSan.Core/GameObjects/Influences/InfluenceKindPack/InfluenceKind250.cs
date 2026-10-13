using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind250 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam() - 1;

        if (rate > 0)
        {
            troop.RateOfMovability += rate;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam() - 1;

        if (rate > 0)
        {
            troop.RateOfMovability -= rate;
        }
    }
}