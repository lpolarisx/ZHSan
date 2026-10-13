using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3300 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfFactionReputationPerDay += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfFactionReputationPerDay -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var isReputationEnough = arch.BelongedFaction.Reputation >= Session.Current.Scenario.Parameters.MaxReputationForRecruit;

        return isReputationEnough ? 0.001 : influence.GetIntParam() / 5.0;
    }
}