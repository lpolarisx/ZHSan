using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind353 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam() - 1;

        if (rate > 0)
        {
            troop.RateOfCriticalArchitectureDamage += rate;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam() - 1;

        if (rate > 0)
        {
            troop.RateOfCriticalArchitectureDamage -= rate;
        }
    }
}