using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind695 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceRateDecrementInViewArea += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceRateDecrementInViewArea -= influence.GetFloatParam();
    }

    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var rate = influence.GetFloatParam();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && !architecture.IsFriendly(troopByPosition.BelongedFaction))
            {
                troopByPosition.RateOfDefence -= rate;
            }
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        var rate = influence.GetFloatParam();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && !architecture.IsFriendly(troopByPosition.BelongedFaction))
            {
                troopByPosition.RateOfDefence += rate;
            }
        }
    }
}