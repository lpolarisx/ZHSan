using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2310 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateIncrementOfTerrainRate += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateIncrementOfTerrainRate -= influence.GetFloatParam();
    }
}