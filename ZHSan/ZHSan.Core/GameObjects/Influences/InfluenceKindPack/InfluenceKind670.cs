using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind670 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ChanceIncrementOfStratagemInViewArea += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.ChanceIncrementOfStratagemInViewArea -= influence.GetIntParam();
    }

    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var increment = influence.GetIntParam();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && architecture.IsFriendly(troopByPosition.BelongedFaction))
            {
                troopByPosition.ChanceIncrementOfStratagem += increment;
            }
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        var increment = influence.GetIntParam();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && architecture.IsFriendly(troopByPosition.BelongedFaction))
            {
                troopByPosition.ChanceIncrementOfStratagem -= increment;
            }
        }
    }
}