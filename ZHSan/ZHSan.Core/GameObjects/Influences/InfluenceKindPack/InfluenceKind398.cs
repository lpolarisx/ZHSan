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
        
        var troops = new List<Troop>();
        var hostileTroopsInView = source.GetHostileTroopsInView();

        foreach (var troop in hostileTroopsInView)
        {
            if (troop.IsInArchitecture || !troop.DaysToReachPosition(source.Position, 1) || troop.Army.Kind.Type == MilitaryType.Navy)
            {
                troops.Add(troop);
            }
        }
        foreach (var troop in troops)
        {
            hostileTroopsInView.Remove(troop);
        }
        
        if (hostileTroopsInView.Count == 0) return 0;

        var orientations = new List<Point>();
        int num = 0;
        foreach (Troop troop in hostileTroopsInView)
        {
            orientations.Add(troop.Position);
            num += troop.FightingForce;
        }
        int num4 = source.TroopIntelligence + source.ChanceIncrementOfStratagem;
        num4 = Math.Min(num4, 100);
       
        int num2 = Square(num4) / 60 * num / source.FightingForce / 100;

        if (num2 <= 0) return num2;

        var points = new List<Point>();

        foreach (var point in source.GetStratagemArea(source.Position).Area)
        {
            if (!Session.Current.Scenario.PositionIsOnFire(point) && Session.Current.Scenario.IsPositionEmpty(point) && Session.Current.Scenario.IsFireVaild(point, false, MilitaryType.Infantry))
            {
                points.Add(point);
            }
        }

        if (points.Count == 0) return 0;

        position = Session.Current.Scenario.GetClosestPosition(points, orientations);

        return num2;
    }
}