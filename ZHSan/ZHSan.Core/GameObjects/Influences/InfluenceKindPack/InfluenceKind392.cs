using GameManager;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind392 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var days = influence.GetIntParam();

        var isPlayer = Session.Current.Scenario.IsPlayer(troop.BelongedFaction);
        bool isNotAutoRun = troop.StartingArchitecture?.BelongedFaction == troop.BelongedFaction
                            && troop.StartingArchitecture?.BelongedSection?.AIDetail?.AutoRun == true;

        if (isPlayer && !troop.Auto && !isNotAutoRun)
        {
            troop.Investigate(days);
        }
        else
        {
            troop.BelongedLegion.SetInformationPosition();
            if (troop.BelongedLegion.InformationDestination.HasValue)
            {
                troop.SelfCastPosition = troop.BelongedLegion.InformationDestination.Value;
                troop.BelongedLegion.InformationDestination = null;
                troop.Investigate(days);
            }
        }
    }

    public override int GetCreditWithPosition(Troop source, out Point? position)
    {
        position = new Point(0, 0);

        var belongedLegion = source.BelongedLegion;

        if (belongedLegion == null || belongedLegion.Troops == null || belongedLegion.WillArchitecture == null || source.BaseViewArea == null) return 0;

        if (belongedLegion != null && Random(belongedLegion.Troops.Count) == 0 && belongedLegion.WillArchitecture.BelongedFaction != null && !source.BelongedFaction.IsArchitectureKnown(belongedLegion.WillArchitecture))
        {
            bool flag = false;
            foreach (Point point in source.BaseViewArea.Area)
            {
                Architecture architectureByPosition = Session.Current.Scenario.GetArchitectureByPosition(point);
                if (source.BelongedLegion.WillArchitecture == architectureByPosition)
                {
                    flag = true;
                    break;
                }
            }
            if (flag && !source.BelongedLegion.InformationDestination.HasValue)
            {
                position = new Point?(source.Position);
                return 150;
            }
        }
        return 0;
    }
}