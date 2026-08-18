using GameEnums;
using GameGlobal;
using GameManager;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class Information : GameObject
    {
        private GameArea area;
        public Faction BelongedFaction; // informations belonging to faction will naturally expire
        public Architecture BelongedArchitecture; // informations belonging to architecture will not expire

        public void Apply()
        {
            foreach (Point point in this.Area.Area)
            {
                if (this.BelongedArchitecture != null && this.BelongedArchitecture.BelongedFaction != null)
                {
                    this.BelongedArchitecture.BelongedFaction.AddPositionInformation(point, this.Level);
                }
                if (this.BelongedFaction != null)
                {
                    this.BelongedFaction.AddPositionInformation(point, this.Level);
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

        private void CheckAmbushTroop(Point p)
        {
            Troop troopByPosition = Session.Current.Scenario.GetTroopByPosition(p);
            if (troopByPosition != null 
                && troopByPosition.Status == TroopStatus.埋伏 
                && ((BelongedArchitecture != null && !BelongedArchitecture.IsFriendly(troopByPosition.BelongedFaction)) 
                    || (BelongedFaction != null && !BelongedFaction.IsFriendly(troopByPosition.BelongedFaction))))
            {
                DetectAmbush(troopByPosition);
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
            if (this.BelongedArchitecture != null && this.BelongedArchitecture.BelongedFaction != null)
            {
                foreach (Point point in this.Area.Area)
                {
                    this.BelongedArchitecture.BelongedFaction.RemovePositionInformation(point, this.Level);
                }
            }
            if (this.BelongedFaction != null)
            {
                foreach (Point point in this.Area.Area)
                {
                    this.BelongedFaction.RemovePositionInformation(point, this.Level);
                }
            }
        }

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

        [DataMember]
        public InformationLevel Level { get; set; }

        public string LevelString => Level.ToString();

        [DataMember]
        public bool Oblique { get; set; }

        [DataMember]
        public int DayCost { get; set; }

        [DataMember]
        public int DaysLeft { get; set; }

        [DataMember]
        public int DaysStarted { get; set; }

        public string ObliqueString => StaticMethods.ToMark(Oblique);

        [DataMember]
        public Point Position { get; set; }

        public string PositionString => $"{Position.X}, {Position.Y}";

        [DataMember]
        public int Radius { get; set; }

        public string BelongedArchitectureName => BelongedArchitecture?.Name ?? "----";
    }
}