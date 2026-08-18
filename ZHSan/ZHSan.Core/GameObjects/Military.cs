using GameGlobal;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using GameManager;
using GameEnums;
using GameDatas;

namespace GameObjects
{
    [DataContract]
    public class Military : GameObject
    {
        private Architecture belongedArchitecture;
        public Faction BelongedFaction;
        public Troop BelongedTroop;
        private int combativity;
        private float experience;
        private Person followedLeader;
        private int injuryQuantity;
        private MilitaryKind kind;
        [DataMember]
        private int kindID;
        private Person leader;
        [DataMember]
        private int leaderExperience;
        private int morale;
        private int quantity;
        public Military ShelledMilitary;

        [DataMember]
        public int ShelledMilitaryID;

        public Military ShellingMilitary;

        private Architecture startingArchitecture;
        private Architecture targetArchitecture;
        public void Init()
        {
        }

        [DataMember]
        public int RoutCount { get; set; }
        [DataMember]
        public int YearCreated { get; set; }
        [DataMember]
        public int TroopDamageDealt { get; set; }
        [DataMember]
        public int TroopBeDamageDealt { get; set; }
        [DataMember]
        public int ArchitectureDamageDealt { get; set; }
        [DataMember]
        public int OfficerKillCount { get; set; }
        [DataMember]
        public int CaptiveCount { get; set; }
        [DataMember]
        public int StratagemSuccessCount { get; set; }
        [DataMember]
        public int StratagemFailCount { get; set; }
        [DataMember]
        public int StratagemBeSuccessCount { get; set; }
        [DataMember]
        public int StratagemBeFailCount { get; set; }

        [DataMember]
        public int belongedArchitectureID;

        public int KindMerit
        {
            get
            {
                return this.Kind.Merit;
            }
        }

        public Architecture BelongedArchitecture
        {
            get
            {
                if (belongedArchitectureID == -1) return null;

                if (belongedArchitecture == null)
                {
                    belongedArchitecture = (Architecture) Session.Current.Scenario.Architectures.GetGameObject(belongedArchitectureID);
                }
                return belongedArchitecture;
            }
            set
            {
                belongedArchitecture = value;
                if (value != null)
                {
                    belongedArchitectureID = value.ID;
                } else
                {
                    belongedArchitectureID = -1;
                }
            }
        }

        [DataMember]
        public int Tiredness { get; set; }

        public int Merit
        {
            get
            {
                return this.Kind.Merit * 2000 + this.Experience * 3 + (this.FollowedLeaderID >= 0 ? 1000 : this.LeaderExperience) * 3;
            }
        }

        public bool IsTransport => Kind.IsTransport;

        public void ApplyFollowedLeader(Troop troop)
        {
            if (this.FollowedLeader == troop.Leader)
            {
                troop.RateOfOffence += Session.Parameters.FollowedLeaderOffenceRateIncrement;
                troop.RateOfDefence += Session.Parameters.FollowedLeaderDefenceRateIncrement;
            }
        }

        public static Military Create(Architecture architecture, MilitaryKind kind)
        {
            Military military = new Military();
            
            military.KindID = kind.ID;
            military.ID = Session.Current.Scenario.Militaries.GetFreeGameObjectID();
            if (kind.RecruitLimit == 1)
            {
                military.Name = kind.Name;
            }
            else
            {
                military.Name = kind.Name + "队";
            }
            architecture.AddMilitary(military);
            architecture.BelongedFaction.AddMilitary(military);
            Session.Current.Scenario.Militaries.AddMilitary(military);
            architecture.DecreaseFund((int) (kind.CreateCost * kind.GetRateOfNewMilitary(architecture)));
            if (kind.IsTransport)
            {
                military.Quantity = kind.MaxScale;
                military.Morale = military.MoraleCeiling;
                military.Combativity = military.CombativityCeiling;
            }
            military.YearCreated = Session.Current.Scenario.Date.Year;
            return military;
        }

        public int DecreaseCombativity(int value)
        {
            var decrement = Math.Min(Combativity, value);

            Combativity -= decrement;

            return decrement;
        }

        public void DecreaseInjuryQuantity(int decrement)
        {
            InjuryQuantity -= decrement;

            InjuryQuantity = Math.Max(InjuryQuantity, 0);
        }

        public int DecreaseMorale(int value)
        {
            var decrement = Math.Min(Morale, value);
          
            Morale -= decrement;

            return decrement;
        }

        public bool DecreaseQuantity(int decrement)
        {
            this.Quantity -= decrement;
            if (this.Quantity <= 0)
            {
                this.Quantity = 0;
                return true;
            }
            return false;
        }

        public int GetTerrainAdaptability(TerrainKind terrain)
        {
            switch (terrain)
            {
                case TerrainKind.无:
                    return 0xdac;

                case TerrainKind.平原:
                    return this.Kind.PlainAdaptability;

                case TerrainKind.草原:
                    return this.Kind.GrasslandAdaptability;

                case TerrainKind.森林:
                    return this.Kind.ForrestAdaptability;

                case TerrainKind.湿地:
                    return this.Kind.MarshAdaptability;

                case TerrainKind.山地:
                    return this.Kind.MountainAdaptability;

                case TerrainKind.水域:
                    return this.Kind.WaterAdaptability;

                case TerrainKind.峻岭:
                    return this.Kind.RidgeAdaptability;

                case TerrainKind.荒地:
                    return this.Kind.WastelandAdaptability;

                case TerrainKind.沙漠:
                    return this.Kind.DesertAdaptability;

                case TerrainKind.栈道:
                    return this.Kind.CliffAdaptability;
            }
            return 0xdac;
        }

        public int IncreaseCombativity(int value)
        {
            var increment = Math.Min(CombativityCeiling - Combativity, value);

            Combativity += increment;

            return increment;
        }

        public void IncreaseExperience(int increment)
        {
            if (this.ShelledMilitary == null)
            {
                this.experience += increment * (Session.Current.Scenario.IsPlayer(this.BelongedFaction) ? 1 : Session.Parameters.AIArmyExperienceRate);
                if (this.experience > Session.GlobalVariables.MaxMilitaryExperience)
                {
                    this.experience = Session.GlobalVariables.MaxMilitaryExperience;
                }
            }
            else
            {
                this.ShelledMilitary.experience += increment * (Session.Current.Scenario.IsPlayer(this.BelongedFaction) ? 1 : Session.Parameters.AIArmyExperienceRate);
                if (this.ShelledMilitary.experience > Session.GlobalVariables.MaxMilitaryExperience)
                {
                    this.ShelledMilitary.experience = Session.GlobalVariables.MaxMilitaryExperience;
                }
            }
        }

        public void IncreaseInjuryQuantity(int increment)
        {
            if (increment > 0)
            {
                InjuryQuantity += increment;
            }
        }

        public bool IncreaseLeaderExperience(int increment)
        {
            if (this.LeaderID != this.FollowedLeaderID)
            {
                this.LeaderExperience += increment;
                if (this.LeaderExperience >= 0x3e8)
                {
                    this.LeaderExperience = 0;
                    this.FollowedLeader = this.Leader;
                    return true;
                }
            }
            return false;
        }

        public int IncreaseMorale(int value)
        {
            var increment = Math.Min(MoraleCeiling - Morale, value);

            Morale += increment;

            return increment;
        }

        public bool IncreaseQuantity(int increment)
        {
            this.Quantity += increment;
            if (this.Quantity >= this.Kind.MaxScale)
            {
                this.Quantity = this.Kind.MaxScale;
                return true;
            }
            return false;
        }

        public bool IncreaseQuantity(int increment, int morale, int combativity, int experience, int leaderExperience)
        {
            if (increment > 0)
            {
                this.Morale = ((this.Quantity * this.Morale) + (increment * morale)) / (increment + this.Quantity);
                this.Combativity = ((this.Quantity * this.Combativity) + (increment * combativity)) / (increment + this.Quantity);
                this.Experience = ((this.Quantity * this.Experience) + (increment * experience)) / (increment + this.Quantity);
                this.LeaderExperience = ((this.Quantity * this.LeaderExperience) + (increment * leaderExperience)) / (increment + this.Quantity);
            }
            return this.IncreaseQuantity(increment);
        }

        public bool IsFollowedLeader(Person person)
        {
            return person.ID == FollowedLeaderID;
        }

        public void ModifyAreaByTerrainAdaptablity(GameArea area)
        {
            for (int i = area.Count - 1; i >= 0; i--)
            {
                Architecture architectureByPosition = Session.Current.Scenario.GetArchitectureByPosition(area[i]);
                if (((architectureByPosition == null) || (this.BelongedFaction != architectureByPosition.BelongedFaction))
                    && this.GetTerrainAdaptability(Session.Current.Scenario.GetTerrainKindByPosition(area[i])) > this.Kind.Movability)
                {
                        area.Area.RemoveAt(i);
                } 
                else if (Session.Current.Scenario.GetWaterPositionMapCost(this.Kind, area[i]) >= 0xdac)
                {
                    area.Area.RemoveAt(i);
                }
            }
        }

        public void Recovery(int multiple)
        {
            if (this.InjuryQuantity > 0)
            {
                int decrement = (this.Kind.MinScale * multiple) / 2;
                if (decrement > this.InjuryQuantity)
                {
                    decrement = this.InjuryQuantity;
                }
                this.DecreaseInjuryQuantity(decrement);
                this.IncreaseQuantity(decrement);
            }
        }

        public int LoseInjuredTroop(float rate)
        {
            if (this.InjuryQuantity > 0)
            {
                int decrement = (int)(this.Kind.MinScale * rate);
                if (decrement > this.InjuryQuantity)
                {
                    decrement = this.InjuryQuantity;
                }
                this.DecreaseInjuryQuantity(decrement);
                //this.IncreaseQuantity(decrement);
                return decrement;
            }
            return 0;
        }

        public int Recovery(float rate)
        {
            if (this.InjuryQuantity > 0)
            {
                int decrement = (int) (this.Kind.MinScale * rate);
                if (decrement > this.InjuryQuantity)
                {
                    decrement = this.InjuryQuantity;
                }
                this.DecreaseInjuryQuantity(decrement);
                this.IncreaseQuantity(decrement);
                return decrement;
            }
            return 0;
        }

        public void SetShelledMilitary(Military military)
        {
            if (this.ShelledMilitary != null)
            {
                this.ShelledMilitary.ShellingMilitary = null;
            }
            this.ShelledMilitary = military;
            if (military != null)
            {
                military.ShellingMilitary = this;
            }
        }

        public static Military SimCreate(Architecture architecture, MilitaryKind kind)
        {
            Military military = new Military();
            military.KindID = kind.ID;
            military.ID = Session.Current.Scenario.Militaries.GetFreeGameObjectID();
            if (kind.RecruitLimit == 1)
            {
                military.Name = kind.Name;
                return military;
            }
            military.Name = kind.Name + "队";
            return military;
        }

        public void SimulateSetLeader(Person person)
        {
            if (person != null)
            {
                if (this.ShelledMilitary == null)
                {
                    this.leader = person;
                    LeaderID = person.ID;
                }
                else
                {
                    this.ShelledMilitary.leader = person;
                    this.ShelledMilitary.LeaderID = person.ID;
                }
            }
        }

        public void StopRecruitment()
        {
            if (this.RecruitmentPerson != null)
            {
                this.RecruitmentPerson.WorkKind = ArchitectureWorkKind.无;
            }
            if (this.RecruitmentPerson != null) // 需要重复检查一遍，因为上面可能将this.RecruitmentPerson变null了
            {
                this.RecruitmentPerson.RecruitmentMilitary = null;
                this.RecruitmentPerson = null;
            }
        }

        public override string ToString() => $"{Name} {Kind.Name} {Quantity}";
       

        [DataMember]
        public int Combativity
        {
            get => ShelledMilitary?.Combativity ?? combativity;
            set
            {
                if (ShelledMilitary == null)
                {
                    combativity = value;
                }
                else
                {
                    ShelledMilitary.Combativity = value;
                }
            }
        }

        public int CombativityCeiling
        {
            get
            {
                if (this.ShelledMilitary == null)
                {
                    if (this.BelongedFaction != null)
                    {
                        return (100 + this.BelongedFaction.IncrementOfCombativityCeiling);
                    }
                }
                else if (this.ShelledMilitary.BelongedFaction != null)
                {
                    return (100 + this.ShelledMilitary.BelongedFaction.IncrementOfCombativityCeiling);
                }
                return 100;
            }
        }

        public int Defence
        {
            get
            {
                return (int) ((this.Kind.Defence + (this.Kind.DefencePerScale * this.Scales)) + (this.Kind.DefencePer100Experience * (Math.Sqrt(this.Experience) / 10)));
            }
        }

        [DataMember]
        public int Experience
        {
            get => ShelledMilitary?.Experience ?? (int)experience;
            set
            {
                if (ShelledMilitary == null)
                {
                    experience = value;
                }
                else
                {
                    ShelledMilitary.Experience = value;
                }
            }
        }

        public string ExperienceWithLimit => CanLevelUp ? $"{Experience}/{Kind.LevelUpExperience}" : Experience.ToString();

        public Person FollowedLeader
        {
            get
            {
                if (this.ShelledMilitary == null)
                {
                    if (this.followedLeader == null)
                    {
                        this.followedLeader = Session.Current.Scenario.Persons.GetGameObject(FollowedLeaderID) as Person;
                    }
                    return this.followedLeader;
                }
                return this.ShelledMilitary.FollowedLeader;
            }
            set
            {
                if (this.ShelledMilitary == null)
                {
                    this.followedLeader = value;
                    FollowedLeaderID = followedLeader?.ID ?? -1;
                }
                else
                {
                    this.ShelledMilitary.FollowedLeader = value;
                }
            }
        }

        /// <summary>
        /// 追随将领ID
        /// </summary>
        [DataMember]
        public int FollowedLeaderID { get; set; } = -1;

        public string FollowedLeaderName => FollowedLeader?.Name ?? "----";

        public int FoodCostPerDay => Kind.FoodPerSoldier * TotalQuantity;

        public int FoodMax => FoodCostPerDay * RationDays;

        public int InjuryChance => ShelledMilitary?.InjuryChance ?? Kind.InjuryChance;

        [DataMember]
        public int InjuryQuantity
        {
            get
            {
                if (this.ShelledMilitary == null)
                {
                    return this.injuryQuantity;
                }
                return this.ShelledMilitary.InjuryQuantity;
            }
            set
            {
                if (ShelledMilitary == null)
                {
                    injuryQuantity = value;
                }
                else
                {
                    ShelledMilitary.InjuryQuantity = value;
                }
               
                injuryQuantity = Math.Max(injuryQuantity, 0);
            }
        }

        public MilitaryKind Kind  //小写为真实值，大写转换为运兵船时改变为假值
        {
            get
            {
                var allMilitaryKinds = Session.Current.Scenario.GameCommonData.AllMilitaryKinds;

                if (kind == null && allMilitaryKinds.TryGetValue(kindID, out var militaryKind))
                {
                    kind = militaryKind;
                }

                if (BelongedArchitecture == null && bushiShuijunBingqieChuyuShuiyu() && allMilitaryKinds.TryGetValue(28, out militaryKind))
                {
                    return militaryKind;
                }

                return kind;
            }
            set
            {
                kind = value;
                kindID = kind == null ? -1 : kind.ID;
            }
        }

        public int KindID   //小写为真实值，大写转换为运兵船时改变为假值
        {
            get
            {
                if (BelongedArchitecture == null && bushiShuijunBingqieChuyuShuiyu()) return 28;
                
                return kindID;
            }
            set
            {
                kindID = value;

                if (Session.Current.Scenario.GameCommonData.AllMilitaryKinds.TryGetValue(kindID, out var militaryKind))
                {
                    kind = militaryKind;
                }
            }
        }

        public int RealKindID => kindID;

        public MilitaryKind RealMilitaryKind => kind;

        public string RealKind
        {
            get
            {
                if (this.ShelledMilitary == null)  //没包裹军队
                {
                    return this.kind.Name;
                }
                else   //包裹军队，部队改为进入水中自动切换运兵船之后已经没有这种情况，保留代码是为了和以前的存档兼容。
                {
                    return this.ShelledMilitary.kind.Name;
                }
            }
        }

        public bool bushiShuijunBingqieChuyuShuiyu(Point position)
        {
            if (Session.GlobalVariables.LandArmyCanGoDownWater 
                && kind != null 
                && kind.Type != MilitaryType.Navy 
                && Session.Current.Scenario.GetTerrainKindByPosition(position) == TerrainKind.水域)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool bushiShuijunBingqieChuyuShuiyu()
        {
            return bushiShuijunBingqieChuyuShuiyu(this.Position);
        }

        public string KindString => Kind.Name;

        public int LeaderFightingForce => leader?.FightingForce ?? 0;

        public Person Leader
        {
            get
            {
                if (this.ShelledMilitary == null)
                {
                    if (this.leader == null)
                    {
                        this.leader = Session.Current.Scenario.Persons.GetGameObject(LeaderID) as Person;
                    }
                    return this.leader;
                }
                return this.ShelledMilitary.Leader;
            }
            set
            {
                if (this.ShelledMilitary == null)
                {
                    this.leader = value;
                    if (this.leader != null)
                    {
                        if (LeaderID != this.leader.ID)
                        {
                            this.LeaderExperience = 0;
                            LeaderID = this.leader.ID;
                        }
                    }
                    else
                    {
                        LeaderID = -1;
                    }
                }
                else
                {
                    this.ShelledMilitary.Leader = value;
                }
            }
        }

        public int LeaderExperience
        {
            get => ShelledMilitary?.LeaderExperience ?? leaderExperience;
            set
            {
                if (ShelledMilitary == null)
                {
                    leaderExperience = value;
                }
                else
                {
                    ShelledMilitary.LeaderExperience = value;
                }
            }
        }

        [DataMember]
        public int RecruitmentPersonID { get; set; }

        /// <summary>
        /// 队长ID
        /// </summary>
        [DataMember]
        public int LeaderID { get; set; } = -1;

        public string LeaderName
        {
            get
            {
                if (this.ShelledMilitary == null)
                {
                    if (this.Leader == null)
                    {
                        return "----";
                    }
                    return this.Leader.Name;
                }
                if (this.ShelledMilitary.Leader == null)
                {
                    return "----";
                }
                return this.ShelledMilitary.Leader.Name;
            }
        }

        public string LocationString
        {
            get
            {
                if (this.BelongedArchitecture != null)
                {
                    return this.BelongedArchitecture.Name;
                }
                if (this.BelongedTroop != null)
                {
                    return this.BelongedTroop.DisplayName;
                }
                if (this.ShellingMilitary != null)
                {
                    return this.ShellingMilitary.LocationString;
                }
                return "----";
            }
        }

        public int MaxRecruitmentWeighing => Kind.MaxScale * (Kind.PointsPerSoldier + 1);

        public int MaxTrainingWeighing => Kind.MaxScale * MoraleCeiling * CombativityCeiling;

        /// <summary>
        /// 士气
        /// </summary>
        [DataMember]
        public int Morale
        {
            get
            {
                morale = Math.Min(morale, MoraleCeiling);

                return ShelledMilitary?.Morale ?? morale;
            }
            set
            {
                if (ShelledMilitary == null)
                {
                    morale = value;
                }
                else
                {
                    ShelledMilitary.Morale = value;
                }
            }
        }

        public int EncourageMoraleCeiling => 100;

        public int MoraleCeiling => BelongedTroop != null && BelongedArchitecture == null ? 120 : 100;

        public int Offence
        {
            get
            {
                return (int) ((this.Kind.Offence + (this.Kind.OffencePerScale * this.Scales)) + (this.Kind.OffencePer100Experience * (Math.Sqrt(this.Experience) / 10)));
            }
        }

        public Point Position
        {
            get
            {
                if (this.BelongedTroop != null)
                {
                    return this.BelongedTroop.Position;
                }
                if (this.BelongedArchitecture != null)
                {
                    return this.BelongedArchitecture.Position;
                }
                if (this.ShellingMilitary != null)
                {
                    return this.ShellingMilitary.Position;
                }
                return Point.Zero;
            }
        }

        /// <summary>
        /// 人数
        /// </summary>
        [DataMember]
        public int Quantity
        {
            get => ShelledMilitary?.Quantity ?? quantity;
            set
            {
                if (ShelledMilitary == null)
                {
                    quantity = value;
                }
                else
                {
                    ShelledMilitary.Quantity = value;
                }
            }
        }

        public int RationDays => Kind.RationDays;

        public int zijinzuidazhi => Kind.zijinshangxian;

        public Person RecruitmentPerson { get; set; }

        public string RecruitmentString
        {
            get
            {
                if ((this.BelongedArchitecture != null) && (this.RecruitmentPerson != null))
                {
                    return this.RecruitmentPerson.Name;
                }
                return "----";
            }
        }

        public int RecruitmentWeighing => Quantity;

        public int Scales => Quantity / Kind.MinScale;

        public int TotalQuantity => Quantity + InjuryQuantity;

        public string TrainingString
        {
            get
            {
                if (this.Morale >= this.MoraleCeiling && this.Combativity >= this.CombativityCeiling)
                {
                    return "√";
                }
                else if (this.BelongedArchitecture != null && this.BelongedArchitecture.TrainingWorkingPersons.Count > 0)
                {
                    return "↑";
                }
                else
                {
                    return "----";
                }
            }
        }

        public int TrainingWeighing => (Kind.MaxScale - Scales) * Morale * Combativity;

        public int Weighing
        {
            get
            {
                return ((this.Offence + this.Defence) * (((this.Kind.ViewRadius + (this.Kind.FireDamageRate >= 1.5 ? -1 : 0)) + (this.Kind.ObliqueView ? 1 : 0)) + (this.Kind.RecruitLimit <= 10 ? 1 : 0)));
            }
        }

        public string BuchongZhuangtai
        {
            get
            {
                if (TotalQuantity >= Kind.MaxScale)
                {
                    return "√";
                }
                else if (BelongedArchitecture != null && RecruitmentPerson != null)
                {
                    return "↑";
                }
                else
                {
                    return "";
                }
            }
        }

        public string YijingXunlianHao
        {
            get
            {
                if (Morale >= MoraleCeiling && Combativity >= CombativityCeiling)
                {
                    return "√";
                }
                else if (BelongedArchitecture != null && BelongedArchitecture.TrainingWorkingPersons.Count > 0)
                {
                    return "↑";
                }
                else
                {
                    return "";
                }
            }
        }

        public int RecoverCost
        {
            get
            {
                if (this.BelongedFaction == null) return 0;
                
                int result = (int)(this.RealMilitaryKind.CreateCost + this.Experience * 5 + (this.FollowedLeader != null ? 1000 : this.LeaderExperience) / 1000.0 * 5000);
                if (!BelongedFaction.AvailableMilitaryKinds.ContainsKey(RealMilitaryKind.ID) || this.RealMilitaryKind.RecruitLimit == 1)
                {
                    result *= 2;
                }
                return result;
            }
        }

        public double RetreatScale
        {
            get
            {
                double retreatScaleRatio = Math.Min(0.5, this.RecoverCost / 50000.0);
                return this.RealMilitaryKind.MaxScale / this.RealMilitaryKind.MinScale * retreatScaleRatio;
            }
        }

        public bool IsFewScaleNeedRetreat
        {
            get
            {
                if (BelongedFaction == null || IsTransport) return false;

                if (IsShell)
                {
                    return Quantity / RealMilitaryKind.MinScale < RetreatScale;
                }
                else
                {
                    return Scales < RetreatScale;
                }
            }
        }

        public bool FollowedLeaderAvailable
        {
            get
            {
                return ((this.BelongedArchitecture != null && this.BelongedArchitecture.PersonsExcludeNvGuan.GameObjects.Contains(this.FollowedLeader)) ||
                    (this.BelongedTroop != null && !this.BelongedTroop.Destroyed && this.BelongedTroop.Leader == this.FollowedLeader)) && 
                    this.FollowedLeader != null && this.FollowedLeader.Status == GameObjects.PersonDetail.PersonStatus.Normal;
            }
        }

        public bool LeaderAvailable
        {
            get
            {
                return ((this.BelongedArchitecture != null && this.BelongedArchitecture.PersonsExcludeNvGuan.GameObjects.Contains(this.Leader)) ||
                    (this.BelongedTroop != null && !this.BelongedTroop.Destroyed && this.BelongedTroop.Leader == this.leader)) &&
                    this.Leader != null && this.Leader.Status == GameObjects.PersonDetail.PersonStatus.Normal;
            }
        }

        public bool AllPersonsAvailable
        {
            get
            {
                if (this.BelongedTroop != null) return true;
                if (this.BelongedArchitecture == null) return false;
                if (this.Leader == null) return false;
                foreach (Person p in this.Leader.preferredTroopPersons)
                {
                    if (!this.BelongedArchitecture.Persons.GameObjects.Contains(p))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int FightingForce
        {
            get
            {
                double influenceValue = 0;
                foreach (var influence in Kind.Influences)
                {
                    influenceValue += influence.AIPersonValue;
                }
                return (int) (this.Offence + this.Defence + influenceValue * 2);
            }
        }

        public float FireDamageRate => Kind.FireDamageRate;

        public bool AirOffence => Kind.AirOffence;

        public float ArchitectureCounterDamageRate => Kind.ArchitectureCounterDamageRate;

        public float ArchitectureDamageRate => Kind.ArchitectureDamageRate;

        public bool ArrowOffence => Kind.ArrowOffence;

        public string ArrowOffenceString => StaticMethods.ToMark(Kind.ArrowOffence);

        public TroopAttackDefaultKind AttackDefaultKind => Kind.AttackDefaultKind;

        public TroopAttackTargetKind AttackTargetKind => Kind.AttackTargetKind;

        public bool BeCountered => Kind.BeCountered;

        public string BeCounteredString => StaticMethods.ToMark(Kind.BeCountered);

        public bool CanLevelUp => Kind.CanLevelUp;

        public string CanLevelUpString => StaticMethods.ToMark(Kind.CanLevelUp);

        public TroopCastDefaultKind CastDefaultKind => Kind.CastDefaultKind;

        public TroopCastTargetKind CastTargetKind => Kind.CastTargetKind;

        public int CliffAdaptability => Kind.CliffAdaptability;

        public float CliffRate => Kind.CliffRate;

        public bool ContactOffence => Kind.ContactOffence;

        public string ContactOffenceString => StaticMethods.ToMark(Kind.ContactOffence);

        public bool CounterOffence => Kind.CounterOffence;

        public string CounterOffenceString => StaticMethods.ToMark(Kind.CounterOffence);

        public bool CreateBesideWater => Kind.CreateBesideWater;

        public string CreateBesideWaterString => StaticMethods.ToMark(Kind.CreateBesideWater);

        public int CreateCost => Kind.CreateCost;

        public int CreateTechnology => Kind.CreateTechnology;

        public string Description => Kind.Description;

        public int DesertAdaptability => Kind.DesertAdaptability;

        public float DesertRate => Kind.DesertRate;

        public int FoodPerSoldier => Kind.FoodPerSoldier;

        public int ForrestAdaptability => Kind.ForrestAdaptability;

        public float ForrestRate => Kind.ForrestRate;

        public int GrasslandAdaptability => Kind.GrasslandAdaptability;

        public float GrasslandRate => Kind.GrasslandRate;

        public int InfluenceCount => Kind.Influences.Count;

        public bool IsShell => Kind.IsShell;

        public string IsShellString => StaticMethods.ToMark(Kind.IsShell);

        public int LevelUpExperience => Kind.LevelUpExperience;

        public int MarshAdaptability => Kind.MarshAdaptability;

        public float MarshRate => Kind.MarshRate;

        public int MaxScale => Kind.MaxScale;

        public int MinScale => Kind.MinScale;

        public int MountainAdaptability => Kind.MountainAdaptability;

        public float MountainRate => Kind.MountainRate;

        public int Movability => Kind.Movability;

        public bool ObliqueOffence => Kind.ObliqueOffence;

        public string ObliqueOffenceString => StaticMethods.ToMark(Kind.ObliqueOffence);

        public bool ObliqueStratagem => Kind.ObliqueStratagem;

        public string ObliqueStratagemString => StaticMethods.ToMark(Kind.ObliqueStratagem);

        public bool ObliqueView => Kind.ObliqueView;

        public string ObliqueViewString => StaticMethods.ToMark(Kind.ObliqueView);

        public bool OffenceOnlyBeforeMove => Kind.OffenceOnlyBeforeMove;

        public string OffenceOnlyBeforeMoveString => StaticMethods.ToMark(Kind.OffenceOnlyBeforeMove);

        public int OffenceRadius => Kind.OffenceRadius;

        public int OneAdaptabilityKind => Kind.OneAdaptabilityKind;

        public int PlainAdaptability => Kind.PlainAdaptability;

        public float PlainRate => Kind.PlainRate;

        public int PointsPerSoldier => Kind.PointsPerSoldier;

        public int RidgeAdaptability => Kind.RidgeAdaptability;

        public float RidgeRate => Kind.RidgeRate;

        public int Speed => Kind.Speed;

        public int StratagemRadius => Kind.StratagemRadius;

        public int TitleInfluence => Kind.TitleInfluence;

        public MilitaryType Type => Kind.Type;

        public int RecruitLimit => Kind.RecruitLimit;

        public int ViewRadius => Kind.ViewRadius;

        public int WastelandAdaptability => Kind.WastelandAdaptability;

        public float WastelandRate => Kind.WastelandRate;

        public int WaterAdaptability => Kind.WaterAdaptability;

        public float WaterRate => Kind.WaterRate;

        public int MorphToKindId => Kind.MorphToKindId;

        public Architecture TargetArchitecture
        {
            get
            {
                if (TargetArchitectureID == -1) return null;
                
                if (this.targetArchitecture == null)
                {
                    this.targetArchitecture = Session.Current.Scenario.Architectures.GetGameObject(TargetArchitectureID) as Architecture;
                }
                return this.targetArchitecture;
            }
            set
            {
                if (this.targetArchitecture == null && value == null) return;
                this.targetArchitecture = value;
                
                if (value != null)
                {
                    TargetArchitectureID = value.ID;
                }
                else
                {
                    TargetArchitectureID = -1;
                }
            }
        }

        /// <summary>
        /// 目标建筑
        /// </summary>
        [DataMember]
        public int TargetArchitectureID { get; set; } = -1;

        public string TargetArchitectureString => TargetArchitectureID > 0 ? TargetArchitecture.Name : "----";

        public Architecture StartingArchitecture
        {
            get
            {
                if (StartingArchitectureID == -1) return null;

                if (this.startingArchitecture == null)
                {
                    this.startingArchitecture = Session.Current.Scenario.Architectures.GetGameObject(StartingArchitectureID) as Architecture;
                }
                return this.startingArchitecture;
            }
            set
            {
                if (this.startingArchitecture == null && value == null) return;
                this.startingArchitecture = value;
                StartingArchitectureID = value?.ID ?? -1;
            }
        }

        /// <summary>
        /// 出发建筑
        /// </summary>
        [DataMember]
        public int StartingArchitectureID { get; set; }

        public string StartingArchitectureString => StartingArchitectureID > 0 ? StartingArchitecture.Name : "----";
        
        /// <summary>
        /// 到达时间
        /// </summary>
        [DataMember]
        public int ArrivingDays { get; set; }

        public string Travel
        {
            get
            {
                if (this.ArrivingDays > 0)
                {
                    return (this.ArrivingDays * Session.Parameters.DayInTurn + "天");
                }
                return "----";
            }
        }

        public int ServedYears
        {
            get
            {
                int year = Session.Current.Scenario.Date.Year - this.YearCreated;
                int sinceBeginning = Session.Current.Scenario.DaySince / 360;
                return Math.Min(year, sinceBeginning);
            }
        }

        public int TransferDays(double distance)
        {
            return (int) Math.Ceiling(distance / this.Movability * 6);
        }

        public int TransferFundCost(double distance)
        {
            return (int) (distance * 10);
        }

        public int TransferFoodCost(double distance)
        {
            return (int)(TransferDays(distance) * this.Kind.FoodPerSoldier * this.Quantity);
        }
    }
}