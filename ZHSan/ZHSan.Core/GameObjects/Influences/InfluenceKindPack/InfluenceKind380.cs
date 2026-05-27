using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind380 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfQibingDamage = influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfQibingDamage = troop.BaseRateOfQibingDamage;
    }
}