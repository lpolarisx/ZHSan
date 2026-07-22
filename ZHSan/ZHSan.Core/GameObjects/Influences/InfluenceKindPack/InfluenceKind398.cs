using GameEnums;
using GameManager;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind398 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        if (troop.GetCurrentStratagemSuccess(troop, false, false, false))
        {
            Session.Current.Scenario.SetPositionOnFire(troop.SelfCastPosition);
        }
    }

    public override int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);
        TroopList hostileTroopsInView = source.GetHostileTroopsInView();
        TroopList list2 = new TroopList();
        foreach (Troop troop in hostileTroopsInView)
        {
            if (troop.IsInArchitecture || !troop.DaysToReachPosition(source.Position, 1) || (troop.Army.Kind.Type == MilitaryType.Navy))
            {
                list2.Add(troop);
            }
        }
        foreach (Troop troop in list2)
        {
            hostileTroopsInView.Remove(troop);
        }
        if (hostileTroopsInView.Count == 0)
        {
            return 0;
        }
        List<Point> orientations = new List<Point>();
        int num = 0;
        foreach (Troop troop in hostileTroopsInView)
        {
            orientations.Add(troop.Position);
            num += troop.FightingForce;
        }
        int num4 = source.TroopIntelligence + source.ChanceIncrementOfStratagem;
        num4 = Math.Min(num4, 100);
       
        int num2 = Square(num4) / 60 * num / source.FightingForce / 100;
        if (num2 > 0)
        {
            GameArea area = new GameArea();
            foreach (Point point in source.GetStratagemArea(source.Position).Area)
            {
                if (!Session.Current.Scenario.PositionIsOnFire(point) && Session.Current.Scenario.IsPositionEmpty(point) && Session.Current.Scenario.IsFireVaild(point, false, MilitaryType.Infantry))
                {
                    area.Area.Add(point);
                }
            }
            if (area.Count > 0)
            {
                position = Session.Current.Scenario.GetClosestPosition(area, orientations);
            }
            else
            {
                num2 = 0;
            }
        }
        return num2;
    }
}