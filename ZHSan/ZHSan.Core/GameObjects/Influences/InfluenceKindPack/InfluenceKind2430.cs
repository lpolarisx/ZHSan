using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2430 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        var rate = influence.GetFloatParam();

        faction.RateOfRoutewayConsumption += 1 - rate;

        ResetRoutePointConsumptionRate(faction, rate);
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        var rate = influence.GetFloatParam();

        faction.RateOfRoutewayConsumption -= 1 - rate;

        ResetRoutePointConsumptionRate(faction, rate);
    }

    private void ResetRoutePointConsumptionRate(Faction faction, float rate)
    {
        if (Session.Current.Scenario.NewInfluence)
        {
            faction.ClosedRouteways.Clear();
            foreach (var routeway in faction.Routeways)
            {
                routeway.ResetRoutePointConsumptionRate(rate);
            }
        }
    }
}