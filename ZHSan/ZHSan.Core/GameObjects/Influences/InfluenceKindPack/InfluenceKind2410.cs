using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2410 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfFoodTransportBetweenArchitectures += 1 - influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfFoodTransportBetweenArchitectures -= 1 - influence.GetFloatParam();
    }
}