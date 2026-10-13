using GameManager;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind396 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.PutOutFire();
    }

    public override int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);
        int num = 0;
        if (source.BelongedLegion == null || Session.Current.Scenario.PositionIsOnFireNoCheck(source.Position) || Random(source.BelongedLegion.Troops.Count) <= 0)
        {
            int pureFightingForce = source.PureFightingForce;
            int num3 = source.TroopIntelligence + source.ChanceIncrementOfStratagem;
            
            num3 = Math.Min(num3, 100);
            num3 = Square(num3) / 60;

            if (num3 > 0)
            {
                foreach (Point point in source.GetStratagemArea(source.Position).Area)
                {
                    if (Session.Current.Scenario.PositionIsOnFire(point))
                    {
                        num += num3 / 40;
                        Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(point);
                        if (troopByPosition == source)
                        {
                            num += num3 * 3;
                        }
                        else if (troopByPosition != null)
                        {
                            num3 *= troopByPosition.FightingForce / pureFightingForce;

                            num += source.IsFriendly(troopByPosition.BelongedFaction) ? num3 : -num3;
                        }
                    }
                }

                if (num > 0)
                {
                    position = new Point?(source.Position);
                }
            }
        }
        return num;
    }
}