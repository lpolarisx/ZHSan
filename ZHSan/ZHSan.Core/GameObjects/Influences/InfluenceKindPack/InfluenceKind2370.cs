using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2370 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfCombativityRecoveryAfterStratagemFail += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.RateOfCombativityRecoveryAfterStratagemFail -= influence.GetFloatParam();
    }
}