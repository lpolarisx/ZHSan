using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind350 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfCriticalDamageReceived = influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateOfCriticalDamageReceived = 1;
    }
}    