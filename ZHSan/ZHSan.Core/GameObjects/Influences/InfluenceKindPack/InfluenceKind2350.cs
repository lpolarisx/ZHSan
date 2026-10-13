using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2350 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfCombativityRecoveryAfterAttacked += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfCombativityRecoveryAfterAttacked -= influence.GetFloatParam();
    }
}