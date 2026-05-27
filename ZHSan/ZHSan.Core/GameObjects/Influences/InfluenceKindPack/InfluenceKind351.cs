using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind351 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfInjuryOnCriticalStrike = influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfInjuryOnCriticalStrike = 1;
    }
}