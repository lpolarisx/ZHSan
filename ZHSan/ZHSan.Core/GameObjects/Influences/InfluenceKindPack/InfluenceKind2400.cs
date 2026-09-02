using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2400 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfRoutewayWorkforce += influence.GetIntParam();

        CloseRouteways(faction);
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.IncrementOfRoutewayWorkforce -= influence.GetIntParam();
        
        CloseRouteways(faction);
    }

    private void CloseRouteways(Faction faction)
    {
        if (Session.Current.Scenario.NewInfluence)
        {
            faction.ClosedRouteways.Clear();
            foreach (var routeway in faction.Routeways)
            {
                routeway.RemoveAfterClose = true;
            }
        }
    }
}