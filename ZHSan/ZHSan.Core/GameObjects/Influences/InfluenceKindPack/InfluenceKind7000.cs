using GameManager;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind7000 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceRateIncrementInViewArea += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.DefenceRateIncrementInViewArea -= influence.GetFloatParam();
    }

    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        var rate = influence.GetFloatParam();
        var militaryKindId = influence.GetIntParam2();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && troopByPosition.Army != null && troopByPosition.Army.KindID == militaryKindId)
            {
                troopByPosition.RateOfDefence += rate;
            }
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        var rate = influence.GetFloatParam();
        var militaryKindId = influence.GetIntParam2();

        foreach (Point point in architecture.ViewArea.Area)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
            if (troopByPosition != null && troopByPosition.Army != null && troopByPosition.Army.KindID == militaryKindId)
            {
                troopByPosition.RateOfDefence -= rate;
            }
        }
    }
}