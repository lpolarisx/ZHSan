using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2420 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfRoutewayRadius += influence.GetIntParam();

        ReGenerateRoutePointArea(faction);
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfRoutewayRadius -= influence.GetIntParam();

        ReGenerateRoutePointArea(faction);
    }

    private void ReGenerateRoutePointArea(Faction faction)
    {
        if (Session.Current.Scenario.NewInfluence)
        {
            foreach (var routeway in faction.Routeways)
            {
                routeway.ReGenerateRoutePointArea();
            }
        }
    }
}