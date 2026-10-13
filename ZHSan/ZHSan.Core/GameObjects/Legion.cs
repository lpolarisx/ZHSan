using GameGlobal;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using GameManager;
using GameEnums;
using System.Linq;
using GameDatas;

namespace GameObjects
{
    [DataContract]
    public class Legion : GameObject
    {
        public Faction BelongedFaction;

        [DataMember]
        public int CoreTroopString { get; set; }

        public Troop CoreTroop;

        [DataMember]
        public Point? InformationDestination { get; set; } = null;

        [DataMember]
        public int PreferredRoutewayString { get; set; }

        public Routeway PreferredRouteway;

        [DataMember]
        public int StartArchitectureString { get; set; }

        public Architecture StartArchitecture;

        [DataMember]
        public List<SupplyingRoutewayPack> SupplyingRouteways { get; set; } = new();

        [DataMember]
        public List<Point> TakenPositions { get; set; } = new();

        [DataMember]
        public string TroopsString { get; set; }

        public List<Troop> Troops { get; set; } = new();

        [DataMember]
        public int WillArchitectureString { get; set; }

        public Architecture WillArchitecture;

        public Legion() {}

        public Legion(LegionConfig config)
        {
            ID = config.Id;
            Kind = config.Kind;
            CoreTroopString = config.CoreTroopString;
            InformationDestination = config.InformationDestination;
            PreferredRoutewayString = config.PreferredRoutewayString;
            StartArchitectureString = config.StartArchitectureString;
            TakenPositions = config.TakenPositions;
            TroopsString = config.TroopsString;
            WillArchitectureString = config.WillArchitectureString;
        }

        public LegionConfig ToConfig()
        {
            return new LegionConfig
            {
                Id = ID,
                Kind = Kind,
                CoreTroopString = CoreTroopString,
                InformationDestination = InformationDestination,
                PreferredRoutewayString = PreferredRoutewayString,
                StartArchitectureString = StartArchitectureString,
                TakenPositions = TakenPositions,
                TroopsString = TroopsString,
                WillArchitectureString = WillArchitectureString,
            };
        }

        public void AddRoutewayCredit(Routeway routeway, int credit)
        {
            foreach (SupplyingRoutewayPack pack in this.SupplyingRouteways)
            {
                if (pack.SupplyingRouteway == routeway)
                {
                    pack.Credit += credit;
                    return;
                }
            }
            SupplyingRoutewayPack item = new SupplyingRoutewayPack();
            item.SupplyingRouteway = routeway;
            item.Credit = credit;
            this.SupplyingRouteways.Add(item);
        }

        public void InitTroops(List<Troop> troops)
        {
            foreach (var troop in troops)
            {
                troop.BelongedLegion = this;
            }

            Troops = troops;
        }

        public void AddTroop(Troop troop)
        {
            troop.BelongedLegion = this;
            Troops.Add(troop);
        }

        public void AI()
        {
            this.CallRouteway();
            this.ResetCoreTroop();
            this.TroopAI();
        }

        public void AIWithAuto()
        {
            this.ResetCoreTroop();
            this.TakenPositions.Clear();

            foreach (var troop in Troops.ToList())
            {
                if (troop.Auto || troop.StartingArchitecture.BelongedSection.AIDetail.AutoRun)
                {
                    troop.AI();
                }
            }
        }

        public void CallInformation()
        {
            if (!this.InformationDestination.HasValue)
            {
                var list = new List<Person>();
                foreach (LinkNode node in this.WillArchitecture.AIAllLinkNodes.Values)
                {
                    if ((((node.A.BelongedFaction == this.BelongedFaction) && node.A.BelongedSection != null && 
                        node.A.BelongedSection.AIDetail.AllowInvestigateTactics) && node.A.InformationAvail()) &&
                        (node.A.RecentlyAttacked <= 0))
                    {
                        foreach (Person person in node.A.MovablePersons)
                        {
                            if (person.LocationArchitecture != null)
                            {
                                list.Add(person);
                            }
                        }
                        if (list.Count >= 10)
                        {
                            break;
                        }
                    }
                }

                if (list.Count > 0)
                {
                    var person = StaticMethods.GetRandomItem(list);

                    var availKinds = person.LocationArchitecture.GetAvailInformationKindList();
                    var count = availKinds.Count;

                    if (count == 0) return;

                    var index = StaticMethods.Random(count / 2); 

                    if (WillArchitecture.BelongedFaction == null)
                    {
                        availKinds.Sort((x, y) => x.CostFund.CompareTo(y.CostFund));
                    }
                    else
                    {
                        availKinds.Sort((x, y) => x.FightingWeighing.CompareTo(y.FightingWeighing));
                    }

                    SetInformationPosition();
                    if (InformationDestination.HasValue)
                    {
                        person.SetInformationKind(availKinds[index]);
                        person.GoForInformation(InformationDestination.Value);
                    }
                }
            }
        }

        private void CallRouteway()
        {
            if (!Session.GlobalVariables.LiangdaoXitong) return;
            if (this.WillArchitecture != null)
            {
                int foodCostPerDay;
                LinkNode node2;
                Routeway routeway;
                if (this.Kind == LegionKind.Offensive)
                {
                    if ((this.WillArchitecture.BelongedFaction != this.BelongedFaction) || (this.WillArchitecture.RecentlyAttacked > 0))
                    {
                        foodCostPerDay = this.FoodCostPerDay;
                        if (((this.PreferredRouteway == null) || (!this.PreferredRouteway.Building && (this.PreferredRouteway.LastActivePointIndex < 0))) || ((this.PreferredRouteway.LastPoint != null) && !this.PreferredRouteway.IsEnough(this.PreferredRouteway.LastPoint.ConsumptionRate, foodCostPerDay * 12)))
                        {
                            foreach (LinkNode node in this.WillArchitecture.AIAllLinkNodes.Values)
                            {
                                if (node.Level > 2)
                                {
                                    break;
                                }
                                if (((node.A.BelongedFaction == this.BelongedFaction) && (node.A.RecentlyAttacked <= 0)) && (node.A.Food >= (foodCostPerDay * 15)))
                                {
                                    node2 = null;
                                    if (node.A.AIAllLinkNodes.TryGetValue(this.WillArchitecture.ID, out node2))
                                    {
                                        routeway = node.A.GetRouteway(node2, true);
                                        if (((routeway != null) && (routeway.LastPoint != null) && (node.A.Fund >= (routeway.LastPoint.BuildFundCost * (2 + ((this.WillArchitecture.AreaCount >= 4) ? 1 : 0))))) && routeway.ByPassHostileArchitecture == null)
                                        {
                                            routeway.Building = true;
                                            this.PreferredRouteway = routeway;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if ((this.Kind == LegionKind.Defensive) && (this.WillArchitecture.BelongedFaction == this.BelongedFaction))
                {
                    foodCostPerDay = this.FoodCostPerDay;
                    if ((this.WillArchitecture.Food < (foodCostPerDay * 12)) && (((this.PreferredRouteway == null) || (!this.PreferredRouteway.Building && (this.PreferredRouteway.LastActivePointIndex < 0))) || ((this.PreferredRouteway.LastPoint != null) && !this.PreferredRouteway.IsEnough(this.PreferredRouteway.LastPoint.ConsumptionRate, foodCostPerDay * 12))))
                    {
                        foreach (LinkNode node in this.WillArchitecture.AIAllLinkNodes.Values)
                        {
                            if (node.Level > 2)
                            {
                                break;
                            }
                            if (((node.A.BelongedFaction == this.BelongedFaction) && (node.A.RecentlyAttacked <= 0)) && (node.A.Food >= (foodCostPerDay * 15)))
                            {
                                node2 = null;
                                if (node.A.AIAllLinkNodes.TryGetValue(this.WillArchitecture.ID, out node2))
                                {
                                    routeway = node.A.GetRouteway(node2, true);
                                    if ((routeway != null) && (routeway.LastPoint != null) && (node.A.Fund >= (routeway.LastPoint.BuildFundCost * 2)))
                                    {
                                        routeway.Building = true;
                                        this.PreferredRouteway = routeway;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DayEvent()
        {
            this.SupplyingRouteways.Clear();

            foreach (var troop in Troops.ToList())
            {
                troop.DayEvent();
            }
            Routeway maxCreditRouteway = this.GetMaxCreditRouteway();
            if (maxCreditRouteway != null)
            {
                this.PreferredRouteway = maxCreditRouteway;
            }
        }

        public void Disband()
        {
            this.PreferredRouteway = null;
            this.StartArchitecture = null;
            this.WillArchitecture = null;
            this.CoreTroop = null;

            foreach (var troop in Troops)
            {
                troop.BelongedLegion = null;
            }
            this.Troops.Clear();

            if (this.BelongedFaction != null)
            {
                foreach (Architecture architecture in this.BelongedFaction.Architectures)
                {
                    if (architecture.DefensiveLegion == this)
                    {
                        architecture.DefensiveLegion = null;
                    }
                }
                this.BelongedFaction.RemoveLegion(this);
            }
        }

        public int GetLegionHostileTroopFightingForceInView()
        {
            var list = new List<Troop>();

            int num = 0;
            foreach (var troop in Troops)
            {
                foreach (Troop troop2 in troop.GetHostileTroopsInView())
                {
                    if (!list.Contains(troop2))
                    {
                        list.Add(troop2);
                        num += troop2.FightingForce;
                    }
                }
            }
            return num;
        }

        public Architecture GetLegionTroopFactionStartArchitecture()
        {
            foreach (var troop in Troops)
            {
                var startingArchitecture = troop.StartingArchitecture;
                if (startingArchitecture != null && startingArchitecture.BelongedFaction == BelongedFaction)
                {
                    return startingArchitecture;
                }
            }
            return null;
        }

        public int GetLegionTroopFightingForce() => Troops.Sum(x => x.FightingForce);

        public Routeway GetMaxCreditRouteway()
        {
            int credit = 0;
            Routeway supplyingRouteway = null;
            foreach (SupplyingRoutewayPack pack in this.SupplyingRouteways)
            {
                if (pack.Credit > credit)
                {
                    credit = pack.Credit;
                    supplyingRouteway = pack.SupplyingRouteway;
                }
            }
            return supplyingRouteway;
        }

        public int GetMinTroopFoodCost()
        {
            if (Troops.Count <= 0) return 0;

            int foodCostPerDay = int.MaxValue;
            foreach (var troop in Troops)
            {
                foodCostPerDay = Math.Min(troop.FoodCostPerDay, foodCostPerDay);
            }
            return foodCostPerDay;
        }

        public Troop GetWillClosestTroop()
        {
            if (Troops.Count == 1) return Troops[0];

            double maxValue = double.MaxValue;
            Troop troop = null;
            foreach (var troop2 in Troops)
            {
                double distance = Session.Current.Scenario.GetDistance(troop2.Position, WillArchitecture.ArchitectureArea);
                if (distance < maxValue)
                {
                    maxValue = distance;
                    troop = troop2;
                }
            }
            return troop;
        }

        public bool HasMovingTroopStartFromArchitecture(Architecture start)
        {
            foreach (var troop in Troops)
            {
                if (troop.StartingArchitecture == start && !troop.IsBaseViewingArchitecture(troop.WillArchitecture))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasTroop(Troop troop)
        {
            return Troops.Contains(troop);
        }

        public void RemoveTroop(Troop troop)
        {
            troop.BelongedLegion = null;
            Troops.Remove(troop);
        }

        public void ResetCoreTroop()
        {
            if (Troops.Count == 0) return;

            var troops = Troops.ToList();
            troops.Sort((a, b) => b.Weighing.CompareTo(a.Weighing));

            CoreTroop = troops[0];
        }

        public void SetInformationPosition()
        {
            var orientations = new List<Point>();

            foreach (Troop troop in Troops)
            {
                orientations.Add(troop.Position);
            }

            InformationDestination = Session.Current.Scenario.GetClosestPosition(WillArchitecture.ArchitectureArea.Area, orientations);
        }

        public void TroopAI()
        {
            TakenPositions.Clear();

            foreach (var troop in Troops.ToList())
            {
                troop.AI();
            }
        }

        public int FoodCostPerDay => Troops.Sum(x => x.FoodCostPerDay);

        public bool HasCuttingRoutewayTroop => Troops.Any(x => x.CutRoutewayDays > 0);

        public bool HasTroopViewingWillArchitecture => Troops.Any(x => x.IsBaseViewingArchitecture(WillArchitecture));

        public bool HasTroopWillArchitectureIsWillArchitecture => Troops.Any(x => x.WillArchitecture == WillArchitecture);

        public int ArmyScale => Troops.Sum(x => x.Army.Scales);
        
        [DataMember]
        public LegionKind Kind { get; set; }
    }
}