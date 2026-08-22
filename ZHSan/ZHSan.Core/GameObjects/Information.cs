using Extensions;
using GameDatas;
using GameEnums;
using GameGlobal;
using GameManager;
using Microsoft.Xna.Framework;

namespace GameObjects;

public class Information : GameObject
{
    public Faction BelongedFaction { get; set; }
    public Architecture BelongedArchitecture { get; set; }

    public InformationLevel Level { get; set; }

    public bool Oblique { get; set; }

    public int DayCost { get; set; }

    public int DaysLeft { get; set; }

    public int DaysStarted { get; set; }

    public Point Position { get; set; }

    public int Radius { get; set; }

    public string LevelString => Level.GetDescription() ?? "----";

    public string ObliqueString => StaticMethods.ToMark(Oblique);

    public string PositionString => $"{Position.X}, {Position.Y}";

    public string BelongedArchitectureName => BelongedArchitecture?.Name ?? "----";

    public Information()
    {

    }

    public Information(InformationConfig config)
    {
        ID = config.Id;
        Level = config.Level;
        Oblique = config.Oblique;
        DayCost = config.DayCost;
        DaysLeft = config.DaysLeft;
        DaysStarted = config.DaysStarted;
        Position = config.Position;
        Radius = config.Radius;
    }

    public InformationConfig ToConfig()
    {
        return new InformationConfig
        {
            Id = ID,
            Level = Level,
            Oblique = Oblique,
            DayCost = DayCost,
            DaysLeft = DaysLeft,
            DaysStarted = DaysStarted,
            Position = Position,
            Radius = Radius,
        };
    }

    public void Apply()
    {
        foreach (var point in Area.Area)
        {
            if (BelongedArchitecture != null && BelongedArchitecture.BelongedFaction != null)
            {
                BelongedArchitecture.BelongedFaction.AddPositionInformation(point, Level);
            }

            if (BelongedFaction != null)
            {
                BelongedFaction.AddPositionInformation(point, Level);
            }
            //this.CheckAmbushTroop(point);
        }
    }

    public void CheckAmbushTroop()
    {
        foreach (var point in Area.Area)
        {
            CheckAmbushTroop(point);
        }
    }

    private void CheckAmbushTroop(Point point)
    {
        var troop = Session.Current.Scenario.GetTroopByPosition(point);

        // 不是友军
        var notAnAlly = !BelongedArchitecture.IsFriendly(troop.BelongedFaction);

        if (troop != null && troop.Status == TroopStatus.埋伏
            && ((BelongedArchitecture != null && notAnAlly) || (BelongedFaction != null && notAnAlly)))
        {
            DetectAmbush(troop);
        }
    }

    private void DetectAmbush(Troop troop)
    {
        var chance = 40 - troop.Leader.Calmness;

        if (Level <= InformationLevel.Medium)
        {
            if (troop.OnlyBeDetectedByHighLevelInformation)
            {
                return;
            }
        }
        else
        {
            chance *= 3;
        }
        if (GameObject.GetChance(chance))
        {
            troop.AmbushDetected(troop);
        }
    }

    public void Initialize()
    {
        foreach (var point in Area.Area)
        {
            if (BelongedArchitecture != null && BelongedArchitecture.BelongedFaction != null)
            {
                BelongedArchitecture.BelongedFaction.AddPositionInformation(point, Level);
            }
            else if (BelongedFaction != null)
            {
                BelongedFaction.AddPositionInformation(point, Level);
            }
        }
    }

    public void Purify()
    {
        foreach (var point in Area.Area)
        {
            if (BelongedArchitecture != null && BelongedArchitecture.BelongedFaction != null)
            {
                BelongedArchitecture.BelongedFaction.RemovePositionInformation(point, Level);
            }

            if (BelongedFaction != null)
            {
                BelongedFaction.RemovePositionInformation(point, Level);
            }
        }
    }

    private GameArea area;
    public GameArea Area
    {
        get
        {
            if (area == null)
            {
                area = GameArea.GetViewArea(Position, Radius, Oblique, null);
            }

            return area;
        }
        set
        {
            area = value;
        }
    }
}