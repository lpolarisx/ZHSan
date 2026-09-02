using GameGlobal;
using GameObjects.Animations;
using GameObjects.ArchitectureDetail;
using GameObjects.ArchitectureDetail.EventEffect;
using GameObjects.Conditions;
using GameObjects.FactionDetail;
using GameObjects.Influences;
using GameObjects.MapDetail;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using Tools;
using GameManager;
using Platforms;
using WorldOfTheThreeKingdoms.GameScreens;
using Serilog.Core;
using Serilog;
using GameEnums;
using System.Text;
using GameDatas;

namespace GameObjects
{
    [DataContract]
    public class GameScenario
    {
        //public GameFreeText.FreeText GameProgressCaution;

        private static ILogger logger;

        [DataMember]
        public string MOD { get; set; }

        public static string SCENARIO_ERROR_TEXT_FILE
        {
            get
            {
                return Platform.Current.DirectoryName(Platform.Current.Location) + "/GameData/ScenarioErrors.txt";
            }
        }

        public Dictionary<int, Person> AllPersons { get; set; } = new();

        /// <summary>
        /// 已出场武将
        /// </summary>
        public Dictionary<int, Person> AvailablePersons { get; set; } = new();

        public FactionList PlayerFactions = new FactionList();
        public PersonList PreparedAvailablePersons = new PersonList();
        public bool Preparing = false;

        public Dictionary<TroopEvent, TroopList> TroopEventsToApply = new Dictionary<TroopEvent, TroopList>();

        public Dictionary<Event, Architecture> EventsToApply = new Dictionary<Event, Architecture>();
        public Dictionary<Event, Architecture> YesEventsToApply = new Dictionary<Event, Architecture>();
        public Dictionary<Event, Architecture> NoEventsToApply = new Dictionary<Event, Architecture>();

        // 缓存地图上有几支部队在埋伏
        private int numberOfAmbushTroop = -1;
        public static int savemaxcounts=49;
        // public Dictionary<Event, Architecture> YesArchiEventsToApply = new Dictionary<Event, Architecture>();
        //public Dictionary<Event, Architecture> NoArchiEventsToApply = new Dictionary<Event, Architecture>();

        public bool EnableLoadAndSave = true;

        // public OngoingBattleList AllOngoingBattles = new OngoingBattleList();

        private PersonList emptyPersonList = new PersonList();
        private CaptiveList emptyCaptiveList = new CaptiveList();

        public Dictionary<PathCacheKey, List<Point>> pathCache = new Dictionary<PathCacheKey, List<Point>>();

        public bool JustSaved = false;

        public GameScenario Clone()
        {
            return this.MemberwiseClone() as GameScenario;
        }

        [DataMember]
        public Dictionary<int, int[]> AiBattlingArchitectureStrings = new Dictionary<int, int[]>();

        /// <summary>
        /// 建筑列表
        /// </summary>
        [DataMember]
        public Dictionary<int, Architecture> Architectures { get; set; } = new();
        
        public Faction CurrentFaction;
        public Faction CurrentPlayer;

        [DataMember]
        public GameDate Date = new GameDate();

        [DataMember]
        public DiplomaticRelationTable DiplomaticRelations = new DiplomaticRelationTable();
        
        /// <summary>
        /// 设施
        /// </summary>
        public Dictionary<int, Facility> Facilities { get; set; } = new();

        [DataMember]
        public FactionListWithQueue Factions = new FactionListWithQueue();

        [DataMember]
        public PositionTable FireTable = new PositionTable();

        [DataMember]
        public CommonData GameCommonData = new CommonData();
        
        public TileAnimationGenerator GeneratorOfTileAnimation;

        /// <summary>
        /// 情报
        /// </summary>
        public Dictionary<int, Information> Informations { get; set; } = new();

        [DataMember]
        public LegionList Legions = new LegionList();

        public TileData[,] MapTileData;

        /// <summary>
        /// 军队
        /// </summary>
        public Dictionary<int, Military> Militaries { get; set; } = new();

        public bool NewInfluence;

        [DataMember]
        public NoFoodTable NoFoodDictionary = new NoFoodTable();

        public int[,] PenalizedMapData;

        [DataMember]
        public Dictionary<int, int> FatherIds = new Dictionary<int, int>();

        [DataMember]
        public Dictionary<int, int> MotherIds = new Dictionary<int, int>();
        [DataMember]
        public Dictionary<int, int> SpouseIds = new Dictionary<int, int>();
        [DataMember]
        public Dictionary<int, int[]> BrotherIds = new Dictionary<int, int[]>();
        [DataMember]
        public Dictionary<int, int[]> SuoshuIds = new Dictionary<int, int[]>();
        [DataMember]
        public Dictionary<int, int[]> CloseIds = new Dictionary<int, int[]>();
        [DataMember]
        public Dictionary<int, int[]> HatedIds = new Dictionary<int, int[]>();
        [DataMember]
        public Dictionary<int, int> MarriageGranterId = new Dictionary<int, int>();

        [DataMember]
        public List<PersonIDRelation> PersonRelationIds = new List<PersonIDRelation>();

        // [DataMember]
        // public PersonList Persons = new PersonList();

        [DataMember]
        public List<int> PlayerList { get; set; }  

        [DataMember]
        public string CurrentPlayerID { get; set; }

        [DataMember]
        public string PlayerInfo { get; set; }        

        /// <summary>
        /// 地区
        /// </summary>
        public Dictionary<int, Region> Regions { get; set; } = new();

        [DataMember]
        public RoutewayList Routeways = new RoutewayList();

        [DataMember]
        public string ScenarioDescription;

        [DataMember]
        public Map ScenarioMap = new Map();

        [DataMember]
        public string ScenarioTitle;

        /// <summary>
        /// 军区
        /// </summary>
        [DataMember]
        public Dictionary<int, Section> Sections { get; set; } = new();

        //public GameMessageList SpyMessages = new GameMessageList();

        /// <summary>
        /// 州域
        /// </summary>
        public Dictionary<int, State> States { get; set; } = new();

        public int[] TerrainAdaptability;
        public bool Threading;

        [DataMember]
        public TreasureList Treasures = new TreasureList();

        [DataMember]
        public TroopEventList TroopEvents = new TroopEventList();

        [DataMember]
        public TroopListWithQueue Troops = new TroopListWithQueue();

        [DataMember]
        public YearTable YearTable = new YearTable();

        [DataMember]
        public EventList AllEvents = new EventList();

        public String LoadedFileName;

        [DataMember]
        public bool UsingOwnCommonData;

        [DataMember]
        public BiographyTable AllBiographies = new BiographyTable();

        [DataMember]
        public int GameTime;

        private DateTime sessionStartTime;

        public bool needAutoSave = false;

        public int NumberOfAmbushTroop
        {
            get
            {
                if (numberOfAmbushTroop >= 0)
                    return numberOfAmbushTroop;
                else
                {
                    int number = 0;
                    foreach (Troop t in Troops)
                    {
                        if (t.Status == TroopStatus.埋伏)
                            number++;
                    }
                    numberOfAmbushTroop = number;
                    return numberOfAmbushTroop;
                }
            }
        }

        public event AfterLoadScenario OnAfterLoadScenario;

        public event AfterSaveScenario OnAfterSaveScenario;

        public event NewFactionAppear OnNewFactionAppear;

        public bool scenarioJustLoaded;

        [DataMember]
        public int DaySince { get; set; }

        [DataMember]
        public Parameters Parameters { get; set; }

        [DataMember]
        public GlobalVariables GlobalVariables { get; set; }

        public GameScenario()
        {

        }
          
        public void Init()
        {
            logger = Log.ForContext<GameScenario>();

            this.GeneratorOfTileAnimation = new TileAnimationGenerator();

            //public static readonly string SCENARIO_ERROR_TEXT_FILE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GameData/ScenarioErrors.txt";

            AvailablePersons = new();

            PlayerFactions = new FactionList();
            PreparedAvailablePersons = new PersonList();
            Preparing = false;

            TroopEventsToApply = new Dictionary<TroopEvent, TroopList>();

            EventsToApply = new Dictionary<Event, Architecture>();
            YesEventsToApply = new Dictionary<Event, Architecture>();
            NoEventsToApply = new Dictionary<Event, Architecture>();

            // 缓存地图上有几支部队在埋伏
            numberOfAmbushTroop = -1;

            EnableLoadAndSave = true;

            emptyPersonList = new PersonList();
            emptyCaptiveList = new CaptiveList();

            pathCache = new Dictionary<PathCacheKey, List<Point>>();

            if (this.UsingOwnCommonData)
            {
                this.GameCommonData = CommonData.Current;
            }
        }

        private Dictionary<Architecture, PersonList>
             NormalPLCache, MovingPLCache, NoFactionPLCache, NoFactionMovingPLCache, PrincessPLCache,
             ZhenzaiPLCache, AgriculturePLCache, CommercePLCache, TechnologyPLCache,
             DominationPLCache, MoralePLCache, EndurancePLCache, TrainingPLCache;
        private Dictionary<Architecture, CaptiveList> CaptivePLCache;

        public PersonList GetPersonList(Architecture a)
        {
            if (NormalPLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.NormalPLCache.ContainsKey(a)) return emptyPersonList;
            return NormalPLCache[a];
        }

        public PersonList GetMovingPersonList(Architecture a)
        {
            if (MovingPLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.MovingPLCache.ContainsKey(a)) return emptyPersonList;
            return MovingPLCache[a];
        }

        public PersonList GetNoFactionPersonList(Architecture a)
        {
            if (NoFactionPLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.NoFactionPLCache.ContainsKey(a)) return emptyPersonList;
            return NoFactionPLCache[a];
        }

        public PersonList GetNoFactionMovingPersonList(Architecture a)
        {
            if (NoFactionMovingPLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.NoFactionMovingPLCache.ContainsKey(a)) return emptyPersonList;
            return NoFactionMovingPLCache[a];
        }

        public PersonList GetPrincessPersonList(Architecture a)
        {
            if (PrincessPLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.PrincessPLCache.ContainsKey(a)) return emptyPersonList;
            return PrincessPLCache[a];
        }

        public CaptiveList GetCaptiveList(Architecture a)
        {
            if (CaptivePLCache == null)
            {
                CreatePersonStatusCache();
            }
            if (!this.CaptivePLCache.ContainsKey(a)) return emptyCaptiveList;
            return CaptivePLCache[a];
        }

        public PersonList GetZhenzaiPersonList(Architecture a)
        {
            if (ZhenzaiPLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.ZhenzaiPLCache.ContainsKey(a)) return emptyPersonList;
            return ZhenzaiPLCache[a];
        }

        public PersonList GetAgriculturePersonList(Architecture a)
        {
            if (AgriculturePLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.AgriculturePLCache.ContainsKey(a)) return emptyPersonList;
            return AgriculturePLCache[a];
        }

        public PersonList GetCommercePersonList(Architecture a)
        {
            if (CommercePLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.CommercePLCache.ContainsKey(a)) return emptyPersonList;
            return CommercePLCache[a];
        }

        public PersonList GetTechnologyPersonList(Architecture a)
        {
            if (TechnologyPLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.TechnologyPLCache.ContainsKey(a)) return emptyPersonList;
            return TechnologyPLCache[a];
        }

        public PersonList GetDomintaionPersonList(Architecture a)
        {
            if (DominationPLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.DominationPLCache.ContainsKey(a)) return emptyPersonList;
            return DominationPLCache[a];
        }

        public PersonList GetMoralePersonList(Architecture a)
        {
            if (MoralePLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.MoralePLCache.ContainsKey(a)) return emptyPersonList;
            return MoralePLCache[a];
        }

        public PersonList GetEndurancePersonList(Architecture a)
        {
            if (EndurancePLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.EndurancePLCache.ContainsKey(a)) return emptyPersonList;
            return EndurancePLCache[a];
        }

        public PersonList GetTrainingPersonList(Architecture a)
        {
            if (TrainingPLCache == null)
            {
                CreatePersonWorkCache();
            }
            if (!this.TrainingPLCache.ContainsKey(a)) return emptyPersonList;
            return TrainingPLCache[a];
        }

        private void CreatePersonWorkCache()
        {
            ZhenzaiPLCache = new Dictionary<Architecture, PersonList>();
            AgriculturePLCache = new Dictionary<Architecture, PersonList>();
            CommercePLCache = new Dictionary<Architecture, PersonList>();
            TechnologyPLCache = new Dictionary<Architecture, PersonList>();
            DominationPLCache = new Dictionary<Architecture, PersonList>();
            MoralePLCache = new Dictionary<Architecture, PersonList>();
            EndurancePLCache = new Dictionary<Architecture, PersonList>();
            TrainingPLCache = new Dictionary<Architecture, PersonList>();

            foreach (var i in this.AvailablePersons.Values.ToList())
            {
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.赈灾 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.ZhenzaiPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.ZhenzaiPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    ZhenzaiPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.农业 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.AgriculturePLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.AgriculturePLCache[i.LocationArchitecture] = new PersonList();
                    }
                    AgriculturePLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.商业 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.CommercePLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.CommercePLCache[i.LocationArchitecture] = new PersonList();
                    }
                    CommercePLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.技术 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.TechnologyPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.TechnologyPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    TechnologyPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.统治 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.DominationPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.DominationPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    DominationPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.民心 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.MoralePLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.MoralePLCache[i.LocationArchitecture] = new PersonList();
                    }
                    MoralePLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.耐久 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.EndurancePLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.EndurancePLCache[i.LocationArchitecture] = new PersonList();
                    }
                    EndurancePLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Normal && i.WorkKind == ArchitectureWorkKind.训练 && (i.LocationTroop == null || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.TrainingPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.TrainingPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    TrainingPLCache[i.LocationArchitecture].Add(i);
                }
            }
        }

        private void CreatePersonStatusCache()
        {
            NormalPLCache = new Dictionary<Architecture, PersonList>();
            MovingPLCache = new Dictionary<Architecture, PersonList>();
            NoFactionPLCache = new Dictionary<Architecture, PersonList>();
            NoFactionMovingPLCache = new Dictionary<Architecture, PersonList>();
            PrincessPLCache = new Dictionary<Architecture, PersonList>();
            CaptivePLCache = new Dictionary<Architecture, CaptiveList>();

            foreach (var i in AvailablePersons.Values.ToList())
            {
                if (i.Status == PersonStatus.Normal && i.LocationArchitecture != null && (i.LocationTroop == null || i.LocationTroop.Destroyed || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.NormalPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.NormalPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    NormalPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Moving && i.LocationArchitecture != null && (i.LocationTroop == null || i.LocationTroop.Destroyed || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.MovingPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.MovingPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    MovingPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.NoFaction && i.LocationArchitecture != null && (i.LocationTroop == null || i.LocationTroop.Destroyed || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.NoFactionPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.NoFactionPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    NoFactionPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.NoFactionMoving && i.LocationArchitecture != null && (i.LocationTroop == null || i.LocationTroop.Destroyed || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.NoFactionMovingPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.NoFactionMovingPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    NoFactionMovingPLCache[i.LocationArchitecture].Add(i);
                }
                if (i.Status == PersonStatus.Princess && i.LocationArchitecture != null && (i.LocationTroop == null || i.LocationTroop.Destroyed || !this.Troops.GameObjects.Contains(i.LocationTroop)))
                {
                    if (!this.PrincessPLCache.ContainsKey(i.LocationArchitecture))
                    {
                        this.PrincessPLCache[i.LocationArchitecture] = new PersonList();
                    }
                    PrincessPLCache[i.LocationArchitecture].Add(i);
                }
            }

            var captives = GetCaptives();
            foreach (var captive in captives)
            {
                var architecture = captive.LocationArchitecture;

                if (architecture != null && captive.CaptivePerson != null)
                {
                    if (!CaptivePLCache.ContainsKey(architecture))
                    {
                        CaptivePLCache[architecture] = new CaptiveList();
                    }
                    CaptivePLCache[architecture].Add(captive);
                }
            }
        }

        public void ClearPersonStatusCache()
        {
            NormalPLCache = MovingPLCache = NoFactionPLCache = NoFactionMovingPLCache = PrincessPLCache = null;
            CaptivePLCache = null;
        }

        public void ClearPersonWorkCache()
        {
            ZhenzaiPLCache = AgriculturePLCache = CommercePLCache = TechnologyPLCache =
            DominationPLCache = MoralePLCache = EndurancePLCache = TrainingPLCache = null;
        }

        [DataMember]
        public CaptiveList captiveData = new CaptiveList();

        /// <summary>
        /// 获取所有俘虏
        /// </summary>
        /// <returns></returns>
        public List<Captive> GetCaptives()
        {
            var result = new List<Captive>();
            foreach (var person in AllPersons.Values)
            {
                if (person.Status == PersonStatus.Captive && person.BelongedCaptive != null)
                {
                    result.Add(person.BelongedCaptive);
                }
            }
            return result;
        }

        // public PersonList AvailablePersons
        // {
        //     get
        //     {
        //         PersonList result = new PersonList();
        //         foreach (Person i in this.Persons)
        //         {
        //             if (i.Status != PersonStatus.None && i.Alive && i.Available)
        //             {
        //                 result.Add(i);
        //             }
        //         }
        //         return result;
        //     }
        // }

        public List<Person> GetDeadPersons()
        {
            var result = new List<Person>();
            foreach (var person in AllPersons.Values)
            {
                if (person.Status != PersonStatus.None && !person.Alive && person.Available)
                {
                    result.Add(person);
                }
            }
            return result;
        }

        public void AddPositionAreaInfluence(Troop troop, Point position, AreaInfluenceKind kind, int offset, float rate)
        {
            if (!this.PositionOutOfRange(position))
            {
                Troop troopByPositionNoCheck = this.GetTroopByPositionNoCheck(position);
                this.MapTileData[position.X, position.Y].AddAreaInfluence(troop, kind, offset, rate, troopByPositionNoCheck);
            }
        }

        public void AddPositionContactingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].AddContactingTroop(troop);
            }
        }

        public void AddPositionOffencingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].AddOffencingTroop(troop);
            }
        }

        public void AddPositionStratagemingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].AddStratagemingTroop(troop);
            }
        }

        public void AddPositionViewingTroopNoCheck(Troop troop, Point position)
        {
            this.MapTileData[position.X, position.Y].AddViewingTroop(troop);
        }
        /*
        private void AddPreparedAvailablePersons()
        {
            foreach (Person person in this.PreparedAvailablePersons)
            {
                Architecture gameObject = this.Architectures.GetGameObject(person.AvailableLocation) as Architecture;
                person.Available = true;
                foreach (Treasure treasure in person.Treasures)
                {
                    treasure.Available = true;
                }
                if (person.Father > 0)
                {
                    foreach (Person p in this.Persons)
                    {
                        if (p.ID == person.Father)
                        {
                            if (p.Available && p.Alive && p.LocationArchitecture != null && p.BelongedFaction != null&&p.BelongedCaptive==null )
                            {
                                p.LocationArchitecture.AddPerson(person);
                                p.BelongedFaction.AddPerson(person);
                                Session.MainGame.mainGameScreen.xianshishijiantupian(p.BelongedFaction.Leader,(this.Persons.GetGameObject(person.Father) as Person).Name,"ChildJoin","","",person.Name ,false);
                                Session.MainGame.mainGameScreen.xianshishijiantupian(person, p.LocationArchitecture.Name, "ChildJoinSelfTalk", "", "",  false);

                            }
                        }
                    }
                }
                else if (person.Mother > 0)
                {
                    foreach (Person p in this.Persons)
                    {
                        if (p.ID == person.Mother)
                        {
                            if (p.Available && p.Alive && p.LocationArchitecture != null && p.BelongedFaction != null && p.BelongedCaptive == null)
                            {
                                p.LocationArchitecture.AddPerson(person);
                                p.BelongedFaction.AddPerson(person);
                                Session.MainGame.mainGameScreen.xianshishijiantupian(p.BelongedFaction.Leader, (this.Persons.GetGameObject(person.Mother) as Person).Name, "ChildJoin", "", "", person.Name, false);
                                Session.MainGame.mainGameScreen.xianshishijiantupian(person, p.LocationArchitecture.Name, "ChildJoinSelfTalk", "", "", false);

                            }
                        }
                    }
                }
                else if (person.Spouse > 0 )
                {
                    foreach (Person p in this.Persons)
                    {
                        if (p.ID == person.Spouse)
                        {
                            if (p.Alive && p.LocationArchitecture != null && p.BelongedFaction != null && p.BelongedCaptive == null)
                            {
                                p.LocationArchitecture.AddPerson(person);
                                p.BelongedFaction.AddPerson(person);
                                if (person.Sex) //女的
                                {
                                    Session.MainGame.mainGameScreen.xianshishijiantupian(person, (this.Persons.GetGameObject(person.Spouse) as Person).Name, "FemaleSpouseJoin", "", "", false);
                                }
                                else
                                {
                                    Session.MainGame.mainGameScreen.xianshishijiantupian(person, (this.Persons.GetGameObject(person.Spouse) as Person).Name, "MaleSpouseJoin", "", "", false);

                                }

                            }
                        }
                    }
                }
                else
                {
                    gameObject.AddNoFactionPerson(person);
                }
                this.AvailablePersons.Add(person);
            }
            this.PreparedAvailablePersons.Clear();
        }
        */

        private void AddPreparedAvailablePersons()
        {
            foreach (Person person in this.PreparedAvailablePersons)
            {
                person.Available = true;
                foreach (Treasure treasure in person.Treasures)
                {
                    treasure.Available = true;
                }

                if (person.Sex)
                {
                    person.NvGuan = true;
                }

                List<GameObject> candidates = new List<GameObject>();
                candidates.Add(person.Spouse);
                candidates.AddRange(person.Brothers.GameObjects);
                candidates.Add(person.Father);
                candidates.Add(person.Mother);
                candidates.AddRange(person.GetSiblings());
                candidates.Add(person.Spouse?.Father);
                candidates.Add(person.Spouse?.Mother);

                Person joinToPerson = null;
                foreach (Person q in candidates)
                {
                    if (q != null && q.Available && q.Alive && q.BelongedCaptive == null)
                    {
                        joinToPerson = q;
                        break;
                    }
                }
                
                if (joinToPerson != null)
                {
                    person.LocationArchitecture = joinToPerson.BelongedArchitecture;
                    person.Status = joinToPerson.Status;
                    if (person.Status == PersonStatus.Moving || person.Status == PersonStatus.NoFactionMoving)
                    {
                        person.Status = PersonStatus.Normal;
                    }
                    else if (person.Status == PersonStatus.Princess)
                    {
                        person.Status = PersonStatus.Normal;
                    }
                    person.YearJoin = this.Date.Year;

                    if (joinToPerson.BelongedFactionWithPrincess != null)
                    {
                        if (person.Father == joinToPerson || person.Mother == joinToPerson)
                        {
                            Session.MainGame.mainGameScreen.xianshishijiantupian(joinToPerson.BelongedFactionWithPrincess.Leader, joinToPerson.Name, TextMessageKind.ChildJoin, "ChildJoin", "", "", person.Name, false);
                            if (person.LocationArchitecture != null)
                            {
                                Session.MainGame.mainGameScreen.xianshishijiantupian(person, person.LocationArchitecture.Name, TextMessageKind.ChildJoinSelfTalk, "ChildJoinSelfTalk", "", "", false);
                            }
                        }
                        else
                        {
                            Faction f = joinToPerson.BelongedFactionWithPrincess;
                            Session.MainGame.mainGameScreen.xianshishijiantupian(person, person.LocationArchitecture.Name, TextMessageKind.PersonJoin, "PersonJoin", "", "", f.Name, false);
                        }
                    }

                    if (person.BelongedFaction != null && !Session.Current.Scenario.IsPlayer(person.BelongedFaction))
                    {
                        person.BelongedFaction.ConsiderPromoteNvGuan(person);
                    }

                    AvailablePersons.Add(person.ID, person);
                    if (joinToPerson.BelongedFactionWithPrincess != null) { 
                        Session.MainGame.mainGameScreen.haizizhangdachengren(joinToPerson, person, false);
                    }
                    this.YearTable.addGrownBecomeAvailableEntry(this.Date, person);

                    continue;
                }

                bool joined = false;
                foreach (int id in person.JoinFactionID)
                {
                    Faction f = (Faction)this.Factions.GetGameObject(id);
                    if (f != null)
                    {
                        AvailablePersons.Add(person.ID, person);
                        person.LocationArchitecture = f.Capital;
                        person.Status = PersonStatus.Normal;
                        person.YearJoin = this.Date.Year;

                        if (person.BelongedFaction != null && !Session.Current.Scenario.IsPlayer(person.BelongedFaction))
                        {
                            person.BelongedFaction.ConsiderPromoteNvGuan(person);
                        }

                        Session.MainGame.mainGameScreen.xianshishijiantupian(person, f.Capital.Name, TextMessageKind.PersonJoin, "PersonJoin", "", "", f.Name, false);
                        this.YearTable.addGrownBecomeAvailableEntry(this.Date, person);
                        Session.MainGame.mainGameScreen.haizizhangdachengren(joinToPerson, person, false);
                        joined = true;

                        break;
                    }
                }

                if (joined) continue;
                person.LocationArchitecture = Setting.Current.Chuchangsuiji ? StaticMethods.GetRandomItem(Architectures.Values.ToList()) : Architectures.GetValueOrDefault(person.AvailableLocation);
                person.Status = PersonStatus.NoFaction;
            }
            this.PreparedAvailablePersons.Clear();
        }

        public void haizichusheng(Person person, Person father, Person muqin, bool doAffect)
        {
            person.Available = true;
            foreach (Treasure treasure in person.Treasures)
            {
                treasure.Available = true;
            }

            person.LocationArchitecture = muqin.BelongedArchitecture;
            person.ChangeFaction(muqin.BelongedFaction);

            if (muqin.IsCaptive)
            {
                Captive.Create(person, muqin.BelongedArchitecture == null ? null : muqin.BelongedArchitecture.BelongedFaction);
            }

            ExtensionInterface.call("ChildrenJoinFaction", new Object[] { this, person });

            Session.MainGame.mainGameScreen.haizizhangdachengren(person, person, true);
        }

        public void ApplyFireTable()
        {
            foreach (Point point in this.FireTable.Positions)
            {
                this.GeneratorOfTileAnimation.AddTileAnimation(TileAnimationKind.火焰, point, true);
            }
        }

        public void ApplyTroopEvents()
        {
            if (this.TroopEventsToApply.Count != 0)
            {
                foreach (TroopEvent event2 in this.TroopEvents)
                {
                    TroopList list = null;
                    if (this.TroopEventsToApply.TryGetValue(event2, out list))
                    {
                        foreach (Troop troop in list.GetList())
                        {
                            event2.ApplyEventEffects(troop);
                        }
                    }
                }
                this.TroopEventsToApply.Clear();
            }
        }

        public void ApplyYesEvents()
        {
            foreach (KeyValuePair<Event, Architecture> i in this.YesEventsToApply)
            {
                i.Key.DoYesApplyEvent(i.Value);
                i.Key.happened = true;
            }
            this.YesEventsToApply.Clear();
            this.NoEventsToApply.Clear();
        }

        public void ApplyNoEvents()
        {
            foreach (KeyValuePair<Event, Architecture> i in this.NoEventsToApply)
            {
                i.Key.DoNoApplyEvent(i.Value);
                i.Key.happened = true;
            }
            this.YesEventsToApply.Clear();
            this.NoEventsToApply.Clear();
            /*
            foreach (KeyValuePair<Event, Architecture> i in this.NoArchiEventsToApply)
            {
                i.Key.DoNoApplyEvent(i.Value);
                i.Key.happened = true;
            }
            this.NoArchiEventsToApply.Clear();
             */
        }
        /*
        public void ApplyYesArchiEvents()
        {
            foreach (KeyValuePair<Event, Architecture> i in this.YesArchiEventsToApply)
            {
                i.Key.DoYesArchiApplyEvent(i.Value);
                i.Key.happened = true;
            }
            this.YesArchiEventsToApply.Clear();
        }

        public void ApplyNoArchiEvents()
        {
            foreach (KeyValuePair<Event, Architecture> i in this.NoArchiEventsToApply)
            {
                i.Key.DoNoArchiApplyEvent(i.Value);
                i.Key.happened = true;
            }
            this.NoArchiEventsToApply.Clear();
        }*/

        public void ApplyEvents()
        {
            Dictionary<Event, Architecture> events = this.EventsToApply;
            foreach (KeyValuePair<Event, Architecture> i in events)
            {
                i.Key.DoApplyEvent(i.Value);
                i.Key.happened = true;
            }

            this.EventsToApply.Clear();
        }

        public void ChangeDiplomaticRelation(int faction1, int faction2, int offset)
        {
            if (faction1 != faction2)
            {
                DiplomaticRelation diplomaticRelation = this.DiplomaticRelations.GetDiplomaticRelation(faction1, faction2);
                if (diplomaticRelation != null)
                {
                    diplomaticRelation.Relation += offset;
                }
            }
        }

        public void SetDiplomaticRelationIfHigher(int faction1, int faction2, int value)
        {
            if (faction1 != faction2)
            {
                DiplomaticRelation diplomaticRelation = this.DiplomaticRelations.GetDiplomaticRelation(faction1, faction2);
                if (diplomaticRelation != null)
                {
                    if (diplomaticRelation.Relation > value)
                    {
                        diplomaticRelation.Relation = value;
                    }
                }
            }
        }

        public void SetDiplomaticRelationTruce(int faction1, int faction2, int value)
        {
            if (faction1 != faction2)
            {
                DiplomaticRelation diplomaticRelation = this.DiplomaticRelations.GetDiplomaticRelation(faction1, faction2);
                if (diplomaticRelation != null)
                {
                    diplomaticRelation.Truce = value;
                }
            }
        }

        private void CheckGameEnd()
        {
            FactionList noArchFaction = new FactionList();
            foreach (Faction f in this.Factions)
            {
                if (f.ArchitectureCount == 0)
                {
                    noArchFaction.Add(f);
                }
            }

            foreach (Faction f in noArchFaction)
            {
                this.Factions.Remove(f);
            }

            if (this.Factions.Count == 1)
            {
                ExtensionInterface.call("GameEnd", new Object[] { this });
                if (this.CurrentPlayer != null && !this.runScenarioEnd(this.CurrentPlayer.Capital, Session.MainGame.mainGameScreen))
                {
                    Session.MainGame.mainGameScreen.GameEndWithUnite(this.Factions[0] as Faction);
                }
            }
        }

        public void Clear()
        {
            this.AllEvents.Clear();
            this.TroopEvents.Clear();
            this.PreparedAvailablePersons.Clear();
            this.Treasures.Clear();
            //this.SpyMessages.Clear();
            this.Routeways.Clear();
            GameObjectList t1 = this.Troops.GetList();
            foreach (Troop t in t1)
            {
                t.Destroy(true, false);
            }
            this.Troops.Clear();
            this.Legions.Clear();
            this.Factions.Clear();
            this.ScenarioMap.Clear();
            this.PlayerFactions.Clear();
            this.FireTable.Clear();
            this.NoFoodDictionary.Clear();
            this.DiplomaticRelations.Clear();
            this.GeneratorOfTileAnimation.Clear();
            this.YearTable.Clear();

            //this.GameCommonData.Clear();

            this.CurrentFaction = null;
            this.CurrentPlayer = null;
        }

        public void ClearPenalizedMapDataByArea(GameArea gameArea)
        {
            foreach (Point point in gameArea.Area)
            {
                if (!this.PositionOutOfRange(point))
                {
                    this.PenalizedMapData[point.X, point.Y] = 0;
                }
            }
        }

        public void ClearPenalizedMapDataByPosition(Point position)
        {
            this.PenalizedMapData[position.X, position.Y] = 0;
        }

        public void ClearPositionFire(Point position)
        {
            this.FireTable.RemovePosition(position);
            this.GeneratorOfTileAnimation.RemoveTileAnimation(TileAnimationKind.火焰, position, true);
        }

        public void CreateNewFaction(Person leader)
        {
            if (leader.Status != PersonStatus.Normal && leader.Status != PersonStatus.NoFaction) return;

            Faction newFaction = new Faction();
            newFaction.Init();
            // newFaction.ID = this.Factions.GetFreeGameObjectID(); 
            newFaction.Leader = leader;
            newFaction.ID = leader.ID;
            if (this.Factions.HasGameObject(newFaction.ID)) { newFaction.ID = this.Factions.GetFreeGameObjectID(); }
            this.Factions.AddFactionWithEvent(newFaction);
            foreach (Faction faction2 in this.Factions)
            {
                if (faction2 != newFaction)
                {
                    this.DiplomaticRelations.AddDiplomaticRelation(newFaction.ID, faction2.ID, 0);
                }
            }
            newFaction.Leader = leader;
            newFaction.Reputation = leader.Reputation;
            newFaction.Name = leader.Name;
            if (leader.PersonBiography != null)
            {
                foreach (var kind in leader.PersonBiography.MilitaryKinds)
                {
                    newFaction.AddMilitaryKind(kind);
                }
                newFaction.ColorIndex = leader.PersonBiography.FactionColor;
            }
            else
            {
                newFaction.AddBasicMilitaryKinds();
                newFaction.ColorIndex = -1;
            }

            List<int> allUnusedColors = new List<int>();
            for (int i = 0; i < this.GameCommonData.AllColors.Count; ++i)
            {
                allUnusedColors.Add(i);
            }
            foreach (Faction f in this.Factions)
            {
                allUnusedColors.Remove(f.ColorIndex);
            }
            if (allUnusedColors.Count == 0)
            {
                newFaction.ColorIndex = GameObject.Random(this.GameCommonData.AllColors.Count);
            }
            else
            {
                if (!allUnusedColors.Contains(newFaction.ColorIndex))
                {
                    newFaction.ColorIndex = allUnusedColors[GameObject.Random(allUnusedColors.Count)];
                }
            }

            newFaction.FactionColor = this.GameCommonData.AllColors[newFaction.ColorIndex];

            Architecture newFactionCapital = leader.LocationArchitecture;
            Faction oldFaction = newFactionCapital.BelongedFaction;

            if (oldFaction != null)
            {
                foreach (var technique in oldFaction.AvailableTechniques.Values)
                {
                    newFaction.AddTechnique(technique);
                }

                if (oldFaction.IsAlien && leader.PersonalLoyalty < 2)
                {
                    newFaction.IsAlien = true;
                }
            }

            newFaction.Capital = newFactionCapital;

            if (leader.BelongedFaction == null)
            {
                leader.Status = PersonStatus.Normal;
            }
            else
            {
                this.ChangeDiplomaticRelation(newFaction.ID, newFactionCapital.BelongedFaction.ID, -500);
            }
            newFaction.PrepareData();

            newFactionCapital.ResetFaction(newFaction);

            newFaction.AddArchitectureKnownData(newFactionCapital);
            newFaction.FirstSection.AddArchitecture(newFactionCapital);

            leader.MoveToArchitecture(newFactionCapital, null, true, false, oldFaction);

            foreach (Point p in newFactionCapital.ArchitectureArea.Area)
            {
                Troop t = GetTroopByPositionNoCheck(p);
                if (t != null)
                {
                    t.Morale = -100;
                    Troop.CheckTroopRout(t);
                }
            }

            if (oldFaction != null && !GameObject.GetChance((int)oldFaction.Leader.PersonalLoyalty * 10))
            {
                oldFaction.Leader.AddHated(leader, -2000);
                leader.AdjustRelation(oldFaction.Leader, -60f, -10);
            }

            if (oldFaction != null)
            {
                int oldFactionLoyalty = oldFaction.Leader.PersonalLoyalty;
                leader.DecreaseKarma(Math.Max(12, 12 + 5 * oldFactionLoyalty + oldFaction.Leader.Karma / 2));
            }

            foreach (var p in AvailablePersons.Values)
            {
                if ((p.BelongedFaction != null && p.BelongedFaction != oldFaction)
                    || p.IsCaptive || p.Status == PersonStatus.Princess || p == leader)
                {
                    continue;
                }
                
                int offset = Person.GetIdealOffset(leader, p);
                if (p.HasCloseStrainTo(leader) || p.IsVeryCloseTo(leader) || (GameObject.GetChance(100 - offset * 20) && p.BelongedFaction == oldFaction))
                {
                    if (p.BelongedFaction == null || p.IsVeryCloseTo(leader) || (GameObject.GetChance(100 - ((int)p.PersonalLoyalty) * 25 + (5 - offset) * 10)
                        && GameObject.GetChance(220 - p.Loyalty * 2 + (5 - offset) * 20)))
                    {
                        if (p.BelongedFaction != null)
                        {
                            p.BelongedFaction.Leader.AdjustRelation(p, -45f - p.PersonalLoyalty * 4.5f, -8);
                            p.BelongedFaction.Leader.AdjustRelation(newFaction.Leader, -45f, -2.5f);
                            p.AdjustRelation(p.BelongedFaction.Leader, -7.5f, -2);
                            p.ChangeFaction(newFaction);
                            p.DecreaseKarma(5 - p.BelongedFaction.Leader.PersonalLoyalty - Math.Min(0, p.BelongedFaction.Leader.Karma / 2));
                        }
                        newFaction.Leader.AdjustRelation(p, 15f, 3);
                        p.AdjustRelation(newFaction.Leader, 4.5f, 1);
                        if (p.LocationTroop == null)
                        {
                            p.MoveToArchitecture(newFactionCapital, null, true, false, oldFaction);
                        }
                        else
                        {
                            p.LocationTroop.ChangeFaction(newFaction);
                        }
                    }
                }
            }
            ExtensionInterface.call("CreateNewFaction", new Object[] { this, oldFaction, newFaction, newFactionCapital });

            this.YearTable.addNewFactionEntry(this.Date, oldFaction, newFaction, newFactionCapital);
            if (this.OnNewFactionAppear != null)
            {
                this.OnNewFactionAppear(newFaction);
            }
        }

        public int PlayerArchitectureCount
        {
            get
            {
                int r = 0;
                foreach (Faction f in this.Factions)
                {
                    if (this.IsPlayer(f))
                    {
                        r += f.ArchitectureCount;
                    }
                }
                return r;
            }
        }
        /*
        private void OngoingBattleDayEvent()
        {
            List<OngoingBattle> toRemove = new List<OngoingBattle>();
            foreach (OngoingBattle ob in this.AllOngoingBattles)
            {
                ob.CalmDay++;
                if (ob.CalmDay >= 5)
                {
                    Dictionary<Faction, int> factionDamages = new Dictionary<Faction, int>();
                    List<Person> persons = new List<Person>();
                    foreach (Person p in this.Persons)
                    {
                        if (p.Battle == ob && p.BelongedFaction != null) 
                        {
                            persons.Add(p);
                            if (!factionDamages.ContainsKey(p.BelongedFaction))
                            {
                               factionDamages.Add(p.BelongedFaction, 0); 
                            }
                            factionDamages[p.BelongedFaction] += p.BattleSelfDamage;
                        }
                    }

                    ArchitectureList battleArch = ob.Architectures;

                    bool first = true;
                    foreach (Person p in persons)
                    {
                        this.YearTable.addBattleEntry(first, this.Date, ob, p, battleArch, factionDamages);
                        p.Battle = null;
                        first = false;
                    }

                    foreach (Architecture a in battleArch)
                    {
                        a.OldFactionName = a.BelongedFaction == null ? "贼军" : a.BelongedFaction.Name;
                        a.Battle = null;
                    }


                    toRemove.Add(ob);
                }
            }

            foreach (OngoingBattle i in toRemove)
            {
                this.AllOngoingBattles.Remove(i);
            }
        }
        */
        public void DayPassedEvent()
        {
            ExtensionInterface.call("DayEvent", new Object[] { this });

            JustSaved = false;

            //this.GameProgressCaution.Text = "开始";
            Session.Parameters.DayEvent(this.PlayerArchitectureCount);

            /*this.ClearPersonStatusCache();
            this.ClearPersonWorkCache();*/

            //clearupRepeatedOfficers();

            this.Troops.FinalizeQueue();
            this.Factions.BuildQueue(false);

            this.TrainChildren();
            NoFactionDevelop();
            this.FireDayEvent();
            this.NoFoodPositionDayEvent();

            this.NewFaction();

            //this.GameProgressCaution.Text = "运行外交";
            foreach (DiplomaticRelationDisplay display in this.DiplomaticRelations.GetAllDiplomaticRelationDisplayList())
            {
                if (display.Truce > 0)
                {
                    display.Truce--;
                }
            }
            //this.GameProgressCaution.Text = "运行势力";
            //this.OngoingBattleDayEvent();

            foreach (Faction faction in this.Factions.GetRandomList())
            {
                faction.DayEvent();
            }
            foreach (var architecture in StaticMethods.GetRandomList(Architectures.Values.ToList()))
            {
                architecture.DayEvent();
            }
            foreach (Routeway routeway in this.Routeways.GetRandomList())
            {
                routeway.DayEvent();
            }
            foreach (Legion legion in this.Legions.GetRandomList())
            {
                legion.DayEvent();
                if (legion.Troops.Count == 0)
                {
                    legion.Disband();
                    this.Legions.Remove(legion);
                }
            }
            //this.GameProgressCaution.Text = "运行军队";
            foreach (Troop troop in this.Troops.GetRandomList())
            {
                if (troop.BelongedFaction == null)
                {
                    troop.DayEvent();
                }
            }

            this.detectCurrentPlayerBattleState(this.CurrentPlayer);

            this.militaryKindEvent();
            this.titleDayEvent();
            this.guanzhiDayEvent();


            //this.GameProgressCaution.Text = "运行人物";

            var persons = AvailablePersons.Values.ToList();
            var randomPersons = StaticMethods.GetRandomList(persons);

            foreach (var person in persons)
            {
                person.PreDayEvent();
            }
            foreach (var person in randomPersons)
            {
                person.DayEvent();
            }
            this.AdjustGlobalPersonRelation();
            this.AddPreparedAvailablePersons();
            /*
            foreach (SpyMessage message in this.SpyMessages.GetRandomList())
            {
                message.DayEvent();
            }
             */
            
            var randomCaptives = StaticMethods.GetRandomList(GetCaptives());
            foreach (var captive in randomCaptives)
            {
                captive.DayEvent();
            }

            foreach (Treasure treasure in this.Treasures.GetList())
            {
                if (treasure.Durability > 0)
                {
                    treasure.Durability -= Session.Parameters.DayInTurn;
                    if (treasure.Durability <= 0)
                    {
                        if (treasure.BelongedPerson != null)
                        {
                            treasure.BelongedPerson.LoseTreasure(treasure);
                        }

                        Session.Current.Scenario.Treasures.Remove(treasure);
                    }
                }
            }
            
            this.CheckGameEnd();

            //this.DaySince++;
            this.DaySince += Session.Parameters.DayInTurn;

            ExtensionInterface.call("PostDayEvent", new Object[] { this });

            scenarioJustLoaded = false;
            Session.MainGame.mainGameScreen.LoadScenarioInInitialization = false;
            numberOfAmbushTroop = -1; // 缓存有几支部队在埋伏，绝大多数时候地图上根本没有埋伏部队，这时候不需要叫浪费时间的函数detectAmbushTroop

            Session.MainGame.mainGameScreen.DisposeMapTileMemory(false, false);
        }

        private void NoFactionDevelop()
        {
            foreach (var architecture in Architectures.Values)
            {
                if (architecture.BelongedFaction == null)
                {
                    architecture.DevelopDayNoFaction();
                }
            }
        }

        private void militaryKindEvent()
        {
            foreach (var militaryKind in GameCommonData.AllMilitaryKinds.Values)
            {
                var obtainProb = militaryKind.ObtainProb;

                if (obtainProb > 0)
                {
                    foreach (var person in militaryKind.Persons)
                    {
                        var faction = person.BelongedFaction;
                        if (StaticMethods.Random(obtainProb) == 0 && faction != null && faction.AddMilitaryKind(militaryKind))
                        {
                            Session.MainGame.mainGameScreen.xianshishijiantupian(person, militaryKind.Name, TextMessageKind.ObtainMilitaryKind, "ObtainMilitaryKind", "", "", false);
                        }
                    }
                }
            }
        }

        private void guanzhiDayEvent()
        {

            List<Title> ManualAwardTitles = new List<Title>();
            foreach (var t in GameCommonData.AllTitles.Values)
            {
                if (t.ManualAward)
                {
                    ManualAwardTitles.Add(t);
                }
            }
            foreach (Title t in ManualAwardTitles)
            {
                if (t.AutoLearn > 0 && GameObject.Random(t.AutoLearn) == 0)
                {
                    var candidates = new List<Person>();
                    if (t.Persons.Count > 0)
                    {
                        foreach (Person p in t.Persons)
                        {
                            if (p.Available && p.Alive)
                            {
                                candidates.Add(p);
                            }
                        }
                    }
                    else
                    {
                        candidates = AvailablePersons.Values.ToList();
                    }

                    foreach (var person in candidates)
                    {
                        if ((!IsPlayer(person.BelongedFaction) || Session.GlobalVariables.PermitManualAwardTitleAutoLearn) && !person.HasHigherLevelTitle(t) && !t.ManualAward && t.CanLearn(person, true))
                        {
                            person.AwardTitle(t);
                        }
                    }
                }
            }
        }

        private void titleDayEvent()
        {
            var courier = AllPersons.GetValueOrDefault(7200);

            foreach (var title in GameCommonData.AllTitles.Values)
            {
                var autoLearn = title.AutoLearn;
                if (autoLearn > 0 && StaticMethods.Random(autoLearn) == 0)
                {
                    var persons = title.Persons;
                    var candidates = new List<Person>();
                    
                    if (persons.Count > 0)
                    {
                        foreach (Person person in persons)
                        {
                            if (person.Available && person.Alive)
                            {
                                candidates.Add(person);
                            }
                        }
                    }
                    else
                    {
                        candidates = AvailablePersons.Values.ToList();
                    }

                    foreach (var person in candidates)
                    {
                        if (!person.HasHigherLevelTitle(title) && title.CanLearn(person, true) && !title.ManualAward)
                        {
                            person.LearnTitle(title);
                            Session.MainGame.mainGameScreen.AutoLearnTitle(person, courier, title);
                        }
                        else if (person.HasTitle() && title.WillLose(person))
                        {
                            person.LoseTitle();
                        }
                    }
                }
            }
        }

        private void detectCurrentPlayerBattleState(Faction faction, bool init = false)
        {

            if (faction == null) return;
            //defend
            ZhandouZhuangtai originalBattleState = faction.BattleState;
            bool fangshou = false;
            int fightingArchitectureCount = 0;
            foreach (Architecture architecture in faction.Architectures)
            {
                if (architecture.BelongedFaction == null) continue;

                if (architecture.BelongedSection == null || architecture.BelongedSection.AIDetail.AutoRun) continue;

                if (architecture.FindHostileTroopInView())
                {
                    fightingArchitectureCount++;

                    if (!architecture.hostileTroopInViewLastDay)  //如果已经提醒过就不再提醒
                    {
                        //architecture.JustAttacked = true;
                        architecture.BelongedFaction.StopToControl = Setting.Current.GlobalVariables.StopToControlOnAttack;
                        architecture.RecentlyAttacked = 5;
                        Session.MainGame.mainGameScreen.ArchitectureBeginRecentlyAttacked(architecture);  //提示玩家建筑视野范围内出现敌军。

                    }
                    architecture.hostileTroopInViewLastDay = true;

                }
                else
                {
                    architecture.hostileTroopInViewLastDay = false;
                }

            }
            if (fightingArchitectureCount == 0)
            {
                fangshou = false;
            }
            else
            {
                fangshou = true;
            }
            //attack
            bool jingong = false;

            foreach (Troop t in faction.Troops)
            {
                if (t.HasHostileArchitectureInView())         //||t.HasHostileTroopInView())
                {
                    jingong = true;
                    break;
                }
            }

            if (!jingong && !fangshou)
            {
                faction.BattleState = ZhandouZhuangtai.和平;
            }
            else if (jingong && !fangshou)
            {
                faction.BattleState = ZhandouZhuangtai.进攻;

            }
            else if (!jingong && fangshou)
            {
                faction.BattleState = ZhandouZhuangtai.防守;

            }
            else
            {
                faction.BattleState = ZhandouZhuangtai.攻守兼备;
            }

            if (originalBattleState != faction.BattleState || init)
            {
                Session.MainGame.mainGameScreen.SwichMusic(this.Date.Season);
            }

        }

        public void DayStartingEvent()
        {
            this.Factions.SetControlling(false);
            
            foreach (Troop troop in this.Troops.GetList())
            {
                if (troop.BelongedFaction == null || troop.BelongedLegion == null || !troop.BelongedLegion.Troops.HasGameObject(troop))
                {
                    troop.AI();
                }
            }
            this.Troops.BuildQueue();

            foreach (var architecture in Architectures.Values)
            {
                architecture.HireFinished = false;
                architecture.HasManualHire = false;
                architecture.TodayPersonArriveNote = false;
            }
        }

        public void FireDayEvent()
        {
            List<Point> list = new List<Point>();
            foreach (Point point in this.FireTable.Positions)
            {
                if (GameObject.GetChance(Session.Parameters.FireStayProb))
                {
                    list.Add(point);
                }
            }
            foreach (Point point in list)
            {
                this.ClearPositionFire(point);
            }
            list.Clear();
            foreach (Point point in this.FireTable.Positions)
            {
                list.Add(point);
            }
            foreach (Point point in list)
            {
                this.FireSpread(point);
            }
        }

        public void FireSpread(Point position)
        {
            GameArea area = GameArea.GetArea(position, 1, false);
            foreach (Point point in area.Area)
            {
                if ((point != position) && this.IsFireVaild(point, false, MilitaryType.Infantry))
                {
                    if (this.PositionIsOnFire(point))
                    {
                        continue;
                    }
                    int chance = 0;
                    switch (this.GetTerrainKindByPosition(position))
                    {
                        case TerrainKind.平原:
                            chance = 3;
                            break;

                        case TerrainKind.草原:
                            chance = 4;
                            break;

                        case TerrainKind.森林:
                            chance = 10;
                            break;

                        case TerrainKind.山地:
                            chance = 6;
                            break;
                    }
                    if (GameObject.GetChance((int)(chance * Session.Parameters.FireSpreadProbMultiply)))
                    {
                        this.SetPositionOnFire(point);
                        Troop troopByPosition = this.GetTroopByPosition(point);
                        if (troopByPosition != null)
                        {
                            troopByPosition.BurntBySpreadFire();
                        }
                    }
                }
            }
        }

        public RoutewayList GetActiveRoutewayListByPosition(Point position)
        {
            RoutewayList list = new RoutewayList();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].TileRouteways == null)
                {
                    return list;
                }
                foreach (Routeway routeway in this.MapTileData[position.X, position.Y].TileRouteways)
                {
                    if (routeway.IsActive || routeway.IsPointActive(position))
                    {
                        list.Add(routeway);
                    }
                }
            }
            return list;
        }

        public Architecture GetArchitectureByPosition(Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return null;
            }
            return this.MapTileData[position.X, position.Y].TileArchitecture;
        }

        public Architecture GetArchitectureByPositionNoCheck(Point position)
        {
            return this.MapTileData[position.X, position.Y].TileArchitecture;
        }

        public GameArea GetAreaWithinDistance(Point centre, int distance, bool includingCentre)
        {
            GameArea area = new GameArea();
            for (int i = -distance; i <= distance; i++)
            {
                for (int j = -distance; j <= distance; j++)
                {
                    Point fromPosition = new Point(centre.X + i, centre.Y + j);
                    if ((includingCentre || !(fromPosition == centre)) && (this.GetDistance(fromPosition, centre) <= distance))
                    {
                        area.AddPoint(fromPosition);
                    }
                }
            }
            return area;
        }

        public Point GetClosestPoint(GameArea area, Point fromPosition)
        {
            int simpleDistance = 0, minSimpleDistance = int.MaxValue;
            double distance = 0, minDistance = double.MaxValue;
            Point point = new Point();
            foreach (Point point2 in area.Area)
            {
                simpleDistance = this.GetSimpleDistance(fromPosition, point2);
                if (simpleDistance <= minSimpleDistance)
                {
                    distance = this.GetDistance(fromPosition, point2);
                    if (distance < minDistance)
                    {
                        minSimpleDistance = simpleDistance;
                        minDistance = distance;
                        point = point2;
                    }
                }
            }
            return point;
        }

        public void GetClosestPointsBetweenTwoAreas(GameArea area1, GameArea area2, out Point? out1, out Point? out2)
        {
            out1 = null;
            out2 = null;
            int simpleDistance = 0, minSimpleDistance = int.MaxValue;
            double distance = 0, minDistance = double.MaxValue;
            foreach (Point point in area1.Area)
            {
                foreach (Point point2 in area2.Area)
                {
                    simpleDistance = this.GetSimpleDistance(point, point2);
                    if (simpleDistance <= minSimpleDistance)
                    {
                        distance = this.GetDistance(point, point2);
                        if (distance < minDistance)
                        {
                            minSimpleDistance = simpleDistance;
                            minDistance = distance;
                            out1 = new Point?(point);
                            out2 = new Point?(point2);
                        }
                    }
                }
            }
        }

        public Point? GetClosestPosition(GameArea area, List<Point> orientations)
        {
            Point? nullable = null;
            int num = 0x7fffffff;
            foreach (Point point in area.Area)
            {
                int num2 = 0;
                foreach (Point point2 in orientations)
                {
                    num2 += this.GetSimpleDistance(point, point2);
                }
                if (num2 < num)
                {
                    num = num2;
                    nullable = new Point?(point);
                }
            }
            return nullable;
        }

        public string GetCoordinateString(Point position)
        {
            return (position.X + "," + position.Y);
        }

        public int GetDiplomaticRelation(int faction1, int faction2)
        {
            if (faction1 != faction2)
            {
                DiplomaticRelation diplomaticRelation = this.DiplomaticRelations.GetDiplomaticRelation(faction1, faction2);
                if (diplomaticRelation != null)
                {
                    return diplomaticRelation.Relation;
                }
            }
            return 0;
        }

        public int GetDiplomaticRelationTruce(int faction1, int faction2)
        {
            if (faction1 != faction2)
            {
                DiplomaticRelation diplomaticRelation = this.DiplomaticRelations.GetDiplomaticRelation(faction1, faction2);
                if (diplomaticRelation != null)
                {
                    return diplomaticRelation.Truce;
                }
            }
            return 0;
        }

        public double GetResourceConsumptionRate(Architecture a, Troop b)
        {
            return this.GetDistance(b.Position, a.ArchitectureArea) / 50.0 + 1;
        }

        public double GetResourceConsumptionRate(Architecture a, Architecture b)
        {
            return this.GetDistance(a.ArchitectureArea, b.ArchitectureArea) / 150.0 + 1;
        }

        public double GetDistance(GameArea fromArea, GameArea toArea)
        {
            // 上面这段浪费太多时间O(n^2)，下面仅需要O(1)，一个非常近似的值已经足够
            double distance = GetDistance(fromArea.Centre, toArea.Centre);

            if (distance < 0) return 0;

            distance -= (1 + Math.Sqrt(2 * fromArea.Count + 1)) / 2;
            distance -= (1 + Math.Sqrt(2 * toArea.Count + 1)) / 2;

            return distance;
        }

        public double GetDistance(Point fromPosition, GameArea toArea)
        {
            // O(1) instead of O(n)
            double distance = GetDistance(fromPosition, toArea.Centre);

            distance -= (1 + Math.Sqrt(2 * toArea.Count + 1)) / 2;

            return distance;
        }

        public double GetDistance(Point fromPosition, Point toPosition)
        {
            return Math.Sqrt(Math.Pow(toPosition.X - fromPosition.X, 2) + Math.Pow(toPosition.Y - fromPosition.Y, 2));
        }

        public Point? GetFarthestPosition(GameArea area, List<Point> orientations)
        {
            Point? nullable = null;
            int num = -2147483648;
            foreach (Point point in area.Area)
            {
                int num2 = 0;
                foreach (Point point2 in orientations)
                {
                    num2 += this.GetSimpleDistance(point, point2);
                }
                if (num2 > num)
                {
                    num = num2;
                    nullable = new Point?(point);
                }
            }
            return nullable;
        } 

        public ArchitectureList GetHighViewingArchitecturesByPosition(Point position)
        {
            ArchitectureList list = new ArchitectureList();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].HighViewingArchitectures == null)
                {
                    return list;
                }
                foreach (Architecture architecture in this.MapTileData[position.X, position.Y].HighViewingArchitectures)
                {
                    list.Add(architecture);
                }
            }
            return list;
        }

        public string GetPlayerInfo()
        {
            if (this.CurrentPlayer != null)
            {
                if (this.PlayerFactions.Count > 1)
                {
                    return (this.CurrentPlayer.Name + " 等");
                }
                if (this.PlayerFactions.Count == 1)
                {
                    return this.CurrentPlayer.Name;
                }
                return "电脑";
            }
            return "电脑";
        }

        //public Texture2D GetPortrait(float id)
        //{
        //    return Session.MainGame.mainGameScreen.GetPortrait(id);
        //}

        public int GetPositionHostileOffencingDiscredit(Troop troop, Point position)
        {
            return this.MapTileData[position.X, position.Y].GetPositionHostileOffencingDiscredit(troop);
        }

        public int GetPositionMapCost(Faction faction, Point position)
        {
            Architecture architectureByPositionNoCheck = this.GetArchitectureByPositionNoCheck(position);
            if (architectureByPositionNoCheck != null)
            {
                if ((architectureByPositionNoCheck.Endurance > 0) && (architectureByPositionNoCheck.BelongedFaction != faction))
                {
                    return 0xdac;
                }
                return 5;
            }
            Troop troopByPositionNoCheck = this.GetTroopByPositionNoCheck(position);
            if (troopByPositionNoCheck != null)
            {
                if (!((faction != null) && faction.IsFriendly(troopByPositionNoCheck.BelongedFaction)))
                {
                    return 0xdac;
                }
                return 0;
            }
            if (this.PositionIsOnFire(position))
            {
                return 10;
            }
            return 0;
        }

        public Point GetProperDestination(Point from, Point to)
        {
            double distance = this.GetDistance(from, to);
            if (distance > 15.0)
            {
                return new Point(from.X + ((int)(((double)((to.X - from.X) * 15)) / distance)), from.Y + ((int)(((double)((to.Y - from.Y) * 15)) / distance)));
            }
            return to;
        }

        public int GetReturnDays(Point destination, GameArea fromArea)
        {
            int num = (int)Math.Ceiling((double)(this.GetDistance(destination, this.GetClosestPoint(fromArea, destination)) / 10.0));
            num *= 2;
            if (num == 0)
            {
                num = 1;
            }
            return num;
        }

        public ArchitectureList GetRoutewayArchitecturesByPosition(Routeway routeway, Point position)
        {
            ArchitectureList list = new ArchitectureList();
            if (!this.PositionOutOfRange(position))
            {
                foreach (Architecture architecture in routeway.BelongedFaction.Architectures)
                {
                    if ((architecture != routeway.StartArchitecture) && architecture.GetRoutewayStartArea().HasPoint(position))
                    {
                        list.Add(architecture);
                    }
                }
            }
            return list;
        }

        public Routeway GetRoutewayByPosition(Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return null;
            }
            if (this.MapTileData[position.X, position.Y].TileRouteways == null)
            {
                return null;
            }
            if (this.MapTileData[position.X, position.Y].TileRouteways.Count == 0)
            {
                return null;
            }
            return this.MapTileData[position.X, position.Y].TileRouteways[0];
        }

        public Routeway GetRoutewayByPositionAndFaction(Point position, Faction faction)
        {
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].TileRouteways == null)
                {
                    return null;
                }
                foreach (Routeway routeway in this.MapTileData[position.X, position.Y].TileRouteways)
                {
                    if (((routeway.BelongedFaction == faction) && (routeway.StartArchitecture != null)) && ((((routeway.DestinationArchitecture == null) || !routeway.StartArchitecture.BelongedSection.AIDetail.AutoRun) || routeway.Building) || (routeway.LastActivePointIndex >= 0)))
                    {
                        return routeway;
                    }
                }
            }
            return null;
        }

        public List<Routeway> GetRoutewaysByPositionAndFaction(Point position, Faction faction)
        {
            List<Routeway> list = new List<Routeway>();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].TileRouteways == null)
                {
                    return list;
                }
                foreach (Routeway routeway in this.MapTileData[position.X, position.Y].TileRouteways)
                {
                    if (routeway.BelongedFaction == faction)
                    {
                        list.Add(routeway);
                    }
                }
            }
            return list;
        }

        public int GetSimpleDistance(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public int GetSingleWayDays(Point destination, GameArea fromArea)
        {
            int num = (int)Math.Ceiling((double)(this.GetDistance(destination, this.GetClosestPoint(fromArea, destination)) / 10.0));
            if (num == 0)
            {
                num = 1;
            }
            return num;
        }

        //public Texture2D GetSmallPortrait(float id)
        //{
        //    return Session.MainGame.mainGameScreen.GetSmallPortrait(id);
        //}

        //public Texture2D GetTroopPortrait(float id)
        //{
        //    return Session.MainGame.mainGameScreen.GetTroopPortrait(id);
        //}
        //public Texture2D GetFullPortrait(float id)
        //{
        //    return Session.MainGame.mainGameScreen.GetFullPortrait(id);
        //}

        public ArchitectureList GetSupplyArchitecturesByPositionAndFaction(Point position, Faction faction)
        {
            ArchitectureList list = new ArchitectureList();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].SupplyingArchitectures == null)
                {
                    return list;
                }
                foreach (Architecture architecture in this.MapTileData[position.X, position.Y].SupplyingArchitectures)
                {
                    //if (faction.IsFriendly(architecture.BelongedFaction))
                    if (faction == architecture.BelongedFaction)
                    {
                        list.Add(architecture);
                    }
                }
            }
            return list;
        }

        public List<RoutePoint> GetSupplyRoutePointsByPositionAndFaction(Point position, Faction faction)
        {
            List<RoutePoint> list = new List<RoutePoint>();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].SupplyingRoutePoints == null)
                {
                    return list;
                }
                foreach (RoutePoint point in this.MapTileData[position.X, position.Y].SupplyingRoutePoints)
                {
                    if (point.BelongedRouteway.IsSupporting(faction))
                    {
                        list.Add(point);
                    }
                }
            }
            return list;
        }

        public TerrainDetail GetTerrainDetailByPosition(Point position)
        {
            TerrainDetail terrainDetail = null;

            if (!PositionOutOfRange(position))
            {
                var terrainId = ScenarioMap.MapData[position.X, position.Y];

                GameCommonData.AllTerrainDetails.TryGetValue(terrainId, out terrainDetail);
            }

            return terrainDetail;
        }

        public TerrainDetail GetTerrainDetailByPositionNoCheck(Point position)
        {
            var terrainId = ScenarioMap.MapData[position.X, position.Y];

            GameCommonData.AllTerrainDetails.TryGetValue(terrainId, out var terrainDetail);

            return terrainDetail;
        }

        public TerrainKind GetTerrainKindByPosition(Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return TerrainKind.无;
            }
            return (TerrainKind)ScenarioMap.MapData[position.X, position.Y];
        }

        public TerrainKind GetTerrainKindByPositionNoCheck(Point position)
        {
            return (TerrainKind)ScenarioMap.MapData[position.X, position.Y];
        }

        public string GetTerrainNameByPosition(Point position)
        {
            TerrainDetail terrainDetail = null;

            if (!PositionOutOfRange(position))
            {
                var terrainId = ScenarioMap.MapData[position.X, position.Y];

                GameCommonData.AllTerrainDetails.TryGetValue(terrainId, out terrainDetail);
            }

            return terrainDetail?.Name ?? "----";
        }

        public int GetTransferFundDays(Architecture from, Architecture to)
        {
            //return (int)Math.Ceiling(this.GetDistance(from.ArchitectureArea, to.ArchitectureArea) / 2.5);
            return (int)Math.Ceiling(this.GetDistance(from.ArchitectureArea, to.ArchitectureArea) / 2.5);
        }


        public Troop GetTroopByPosition(Point position)
        {
            if (PositionOutOfRange(position)) return null;
            
            return MapTileData[position.X, position.Y].TileTroop;
        }

        public Troop GetTroopByPositionNoCheck(Point position)
        {
            return this.MapTileData[position.X, position.Y].TileTroop;
        }

        public ArchitectureList GetViewingArchitecturesByPosition(Point position)
        {
            ArchitectureList list = new ArchitectureList();
            if (!this.PositionOutOfRange(position))
            {
                if (this.MapTileData[position.X, position.Y].ViewingArchitectures == null)
                {
                    return list;
                }
                foreach (Architecture architecture in this.MapTileData[position.X, position.Y].ViewingArchitectures)
                {
                    list.Add(architecture);
                }
            }
            return list;
        }

        public int GetWaterPositionMapCost(MilitaryKind kind, Point position)
        {
            if (ScenarioMap.MapData[position.X, position.Y] == 6)
            {
                if (Session.GlobalVariables.LandArmyCanGoDownWater)
                {
                    return 0;
                }

                if (this.GetArchitectureByPositionNoCheck(position) != null)
                {
                    return 0;
                }
                if (kind.Type == MilitaryType.Navy)
                {
                    return 0;
                }
                int num = 0;
                Point point = new Point(position.X - 1, position.Y);
                if (!(this.PositionOutOfRange(point) || (ScenarioMap.MapData[point.X, point.Y] != 6)))
                {
                    num++;
                }
                Point point2 = new Point(position.X, position.Y - 1);
                if (!(this.PositionOutOfRange(point2) || (ScenarioMap.MapData[point2.X, point2.Y] != 6)))
                {
                    num++;
                }
                Point point3 = new Point(position.X + 1, position.Y);
                if (!(this.PositionOutOfRange(point3) || (ScenarioMap.MapData[point3.X, point3.Y] != 6)))
                {
                    num++;
                }
                if (num > 2)
                {
                    return 0xdac;
                }
                Point point4 = new Point(position.X, position.Y + 1);
                if (!(this.PositionOutOfRange(point4) || (ScenarioMap.MapData[point4.X, point4.Y] != 6)))
                {
                    num++;
                }
                if (num > 2)
                {
                    return 0xdac;
                }
            }
            else
            {
                if (kind.Type != MilitaryType.Navy || kind.IsShell || kind.IsTransport)
                {
                    return 0;
                }

                Architecture a = this.GetArchitectureByPositionNoCheck(position);
                if (a != null && !a.Kind.ShipCanEnter)
                {
                    return 0xdac;
                }
            }
            return 0;
        }

        private bool HasSameIdealFaction(Person person)
        {
            if ((person.BelongedFaction != null) && (person.BelongedFaction.Leader == person))
            {
                return true;
            }
            foreach (Faction faction in this.Factions)
            {
                if ((faction.Leader != null) && (faction.Leader.Ideal == person.Ideal))
                {
                    return true;
                }
            }
            return false;
        }

        public int HostileContactingTroopsCount(Faction faction, Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return 0;
            }
            return this.MapTileData[position.X, position.Y].HostileContactingTroopsCount(faction);
        }

        public int HostileOffencingTroopsCount(Faction faction, Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return 0;
            }
            return this.MapTileData[position.X, position.Y].HostileOffencingTroopsCount(faction);
        }

        public int HostileViewingTroopsCount(Faction faction, Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return 0;
            }
            return this.MapTileData[position.X, position.Y].HostileViewingTroopsCount(faction);
        }

        public void InitialGameData()
        {
            this.InitializeSectionData();
            this.InitializeRoutewayData();
            this.InitializeArchitectureData();
            this.InitializeMilitariesData();
            this.InitializeTroopData();
            this.InitializeCaptiveData();
            this.InitializePersonData();
            //this.InitializeSpyMessageData();

            var persons = AllPersons.Values;
            foreach (var person in AllPersons.Values)
            {
                foreach (var title in person.UniqueTitles)
                {
                    title.Persons.Add(person);
                }

                foreach (var militaryKind in person.UniqueMilitaryKinds)
                {
                    militaryKind.Persons.Add(person);
                }
            }

            if (Session.GlobalVariables.RemoveSpouseIfNotAvailable)
            {
                foreach (var person in persons)
                {
                    if (!person.Available && person.Spouse != null && !person.Spouse.Available)
                    {
                        person.suoshurenwuList.Remove(person.Spouse);
                        person.Spouse = null;
                    }
                }
            }

            Session.Parameters.MigrateData();

            /*
            this.GameProgressCaution = new GameFreeText.FreeText(new System.Drawing.Font("宋体", 16f), new Color(1f, 1f, 1f));
            this.GameProgressCaution.Text = "——";
            this.GameProgressCaution.Align=TextAlign.Middle;
            */
        }

        public void InitializeArchitectureMapTile()
        {
            var architectures = Architectures.Values;

            foreach (var architecture in Architectures.Values)
            {
                foreach (var point in architecture.ArchitectureArea.Area)
                {
                    MapTileData[point.X, point.Y].TileArchitecture = architecture;
                }
            }

            foreach (var architecture in architectures)
            {
                SetMapTileArchitecture(architecture);
            }
        }

        private void InitializeArchitectureData()
        {
            var architectures = Architectures.Values;

            foreach (var architecture in architectures)
            {
                architecture.PlanArchitecture = Architectures.GetValueOrDefault(architecture.PlanArchitectureID);
                architecture.TransferFundArchitecture = Architectures.GetValueOrDefault(architecture.TransferFundArchitectureID);
                architecture.TransferFoodArchitecture = Architectures.GetValueOrDefault(architecture.TransferFoodArchitectureID);
                
                if (architecture.DefensiveLegionID >= 0)
                {
                    architecture.DefensiveLegion = this.Legions.GetGameObject(architecture.DefensiveLegionID) as Legion;
                }
                if (architecture.RobberTroopID >= 0)
                {
                    architecture.RobberTroop = this.Troops.GetGameObject(architecture.RobberTroopID) as Troop;
                }
            }

            bool redoLinks = false;
            foreach (var architecture in architectures)
            {
                architecture.AILandLinks = StaticMethods.LoadFromString(Architectures, architecture.AILandLinksString).Values.ToList();
                architecture.AIWaterLinks = StaticMethods.LoadFromString(Architectures, architecture.AIWaterLinksString).Values.ToList();
            }

            foreach (var architecture2 in architectures)
            {
                if (architecture2.AILandLinks.Count == 0 && architecture2.AIWaterLinks.Count == 0)
                {
                    redoLinks = true;
                    break;
                }
            }

            if (redoLinks)
            {
                foreach (var architecture2 in architectures)
                {
                    architecture2.AILandLinks.Clear();
                    architecture2.AIWaterLinks.Clear();
                }
                foreach (var architecture2 in architectures)
                {
                    architecture2.FindLinks(architectures.ToList());
                }
            }

            foreach (var architecture in architectures)
            {
                if (architecture.BelongedFaction != null)
                {
                    architecture.CheckIsFrontLine();
                }
                architecture.GenerateAllAILinkNodes(2);
            }

            /*foreach (Architecture a in this.Architectures)
            {
                foreach (LinkNode i in a.AILandLinks)
                {
                    Point? p1;
                    Point? p2;
                    this.GetClosestPointsBetweenTwoAreas(a.ArchitectureArea, i.A.ArchitectureArea, out p1, out p2);

                    if (p1 != null && p2 != null){
                        Military m = new Military();
                        Troop t = new Troop();

                        t.pathFinder.GetFirstTierPath(p1.Value, p2.Value);
                        this.pathCache[new PathCacheKey(a, i.A)] = new List<Point>(t.FirstTierPath);
                    }
                }
            }*/
        }

        private void InitializeCaptiveData()
        {
            var captives = GetCaptives();
            foreach (var captive in captives)
            {
                if (captive.CaptiveFactionID >= 0)
                {
                    captive.CaptiveFaction = Factions.GetGameObject(captive.CaptiveFactionID) as Faction;
                }
                
                captive.RansomArchitecture = Architectures.GetValueOrDefault(captive.RansomArchitectureID);
            }
        }

        private void InitializeFactionData()
        {
            foreach (Faction faction in this.Factions)
            {
                faction.PrepareData();
            }
        }

        public void InitializeMapData()
        {
            this.MapTileData = new TileData[ScenarioMap.MapDimensions.X, ScenarioMap.MapDimensions.Y];
            this.PenalizedMapData = new int[ScenarioMap.MapDimensions.X, ScenarioMap.MapDimensions.Y];
        }

        private void InitializeMilitaryData()
        {
            foreach (var military in Militaries.Values)
            {
                if (military.ShelledMilitaryID >= 0)
                {
                    military.SetShelledMilitary(Militaries.GetValueOrDefault(military.ShelledMilitaryID));
                }
            }
        }

        private void InitializePersonData()
        {
            foreach (var person in AllPersons.Values)
            {
                person.ConvincingPerson = AllPersons.GetValueOrDefault(person.ConvincingPersonID);
            }
        }

        private void InitializeRoutewayData()
        {
            foreach (Routeway routeway in this.Routeways)
            {
                routeway.RefreshRoutewayPointsData();
            }
        }

        public void InitializeScenarioPlayerFactions(List<int> factionIDs)
        {
            this.PlayerFactions.LoadFromString(this.Factions, StaticMethods.SaveToString(factionIDs));
        }

        private void InitializeSectionData()
        {
            foreach (var section in Sections.Values)
            {
                if (section.OrientationFactionID >= 0)
                {
                    section.OrientationFaction = Factions.GetGameObject(section.OrientationFactionID) as Faction;
                }
                section.OrientationSection = Sections.GetValueOrDefault(section.OrientationSectionID);
                section.OrientationState = States.GetValueOrDefault(section.OrientationStateID);
                section.OrientationArchitecture = Architectures.GetValueOrDefault(section.OrientationArchitectureID);
            }
        }

        /*
        private void InitializeSpyMessageData()
        {
            foreach (SpyMessage message in this.SpyMessages)
            {
                if (message.MessageFactionID >= 0)
                {
                    message.MessageFaction = this.Factions.GetGameObject(message.MessageFactionID) as Faction;
                }
                if (message.MessageArchitectureID >= 0)
                {
                    message.MessageArchitecture = this.Architectures.GetGameObject(message.MessageArchitectureID) as Architecture;
                }
            }
        }
        */

        private void InitializeTroopData()
        {
            TroopList toRemove = new TroopList();
            foreach (Troop troop in this.Troops)
            {
                if (troop.Leader == null || troop.Army == null || troop.Army.Kind == null)
                {
                    toRemove.Add(troop);
                }
                else if (troop.Persons.Count == 0)
                {
                    troop.Leader.LocationTroop = troop;
                }
            }
            foreach (Troop troop in toRemove)
            {
                if (troop.BelongedFaction != null)
                {
                    troop.BelongedFaction.RemoveTroop(troop);
                }
                this.Troops.Remove(troop);
            }

            foreach (Troop troop in this.Troops)
            {
                troop.Initialize();
            }
            foreach (TroopEvent event2 in this.TroopEvents)
            {
                if (event2.AfterEventHappened >= 0)
                {
                    event2.AfterHappenedEvent = this.TroopEvents.GetGameObject(event2.AfterEventHappened) as TroopEvent;
                }
            }
        }

        private void InitializeMilitariesData()
        {
            var toRemove = new List<Military>();

            foreach (var military in Militaries.Values)
            {
                if (military.Kind == null)
                {
                    toRemove.Add(military);
                }
            }

            foreach (var military in toRemove)
            {
                if (military.BelongedArchitecture != null)
                {
                    military.BelongedArchitecture.RemoveMilitary(military);
                }
                Militaries.Remove(military.ID);
            }
        }

        public bool IsCurrentPlayer(Faction faction)
        {
            return (this.CurrentPlayer == faction);
        }

        public bool IsFireVaild(Point position, bool typevalid, MilitaryType type)
        {
            if (this.GetArchitectureByPosition(position) != null)
            {
                return false;
            }
            TerrainKind terrainKindByPosition = this.GetTerrainKindByPosition(position);
            return (((typevalid && (type == MilitaryType.Navy)) && (terrainKindByPosition == TerrainKind.水域)) || ((((terrainKindByPosition == TerrainKind.平原) || (terrainKindByPosition == TerrainKind.草原)) || (terrainKindByPosition == TerrainKind.森林)) || (terrainKindByPosition == TerrainKind.山地)));
        }

        public bool IsLastPlayer(Faction faction)
        {
            if (faction == null)
            {
                return false;
            }
            foreach (Faction faction2 in this.PlayerFactions)
            {
                if ((faction2 != faction) && !faction2.Passed)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPlayer(Faction faction)
        {
            return ((faction != null) && (this.PlayerFactions.GetGameObject(faction.ID) != null));
        }

        public bool HasAIResourceBonus(Section section)
        {
            if (Session.GlobalVariables.PlayerAutoSectionHasAIResourceBonus)
            {
                return section != null && (!IsPlayer(section.BelongedFaction) || !section.AIDetail.AutoRun);
            }
            else
            {
                return section != null && !IsPlayer(section.BelongedFaction);
            }
        }

        public bool IsPlayerControlling()
        {
            return (((this.CurrentPlayer != null) && (this.CurrentFaction == this.CurrentPlayer)) && this.CurrentPlayer.Controlling);
        }

        public bool IsPositionDisplayable(Point position)
        {
            return (Session.MainGame.mainGameScreen.TileInScreen(position) && ((Session.GlobalVariables.SkyEye || (this.CurrentPlayer == null)) || this.CurrentPlayer.IsPositionKnown(position)));
        }

        public bool IsPositionEmpty(Point position)
        {
            if (this.PositionIsArchitecture(position))
            {
                return false;
            }
            if (this.PositionIsTroop(position))
            {
                return false;
            }
            return true;
        }

        public bool IsPositionMovable(Point position, Faction faction)
        {
            if (this.PositionIsTroop(position))
            {
                return false;
            }
            Architecture architectureByPosition = this.GetArchitectureByPosition(position);
            return ((architectureByPosition == null) || (architectureByPosition.BelongedFaction == faction));
        }

        public bool IsTheBottomTroop(Troop troop)
        {
            return (this.MapTileData[troop.Position.X, troop.Position.Y].TileTroop == troop);
        }

        public bool IsTroopViewingPosition(Troop troop, Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return false;
            }
            return this.MapTileData[position.X, position.Y].IsTroopViewing(troop);
        }

        public bool IsWaterPositionRoutewayable(Point position)
        {
            if (ScenarioMap.MapData[position.X, position.Y] == 6)
            {
                int num = 0;
                Point point = new Point(position.X - 1, position.Y);
                if (!(this.PositionOutOfRange(point) || (ScenarioMap.MapData[point.X, point.Y] != 6)))
                {
                    num++;
                }
                Point point2 = new Point(position.X, position.Y - 1);
                if (!(this.PositionOutOfRange(point2) || (ScenarioMap.MapData[point2.X, point2.Y] != 6)))
                {
                    num++;
                }
                Point point3 = new Point(position.X + 1, position.Y);
                if (!(this.PositionOutOfRange(point3) || (ScenarioMap.MapData[point3.X, point3.Y] != 6)))
                {
                    num++;
                }
                if (num > 2)
                {
                    return false;
                }
                Point point4 = new Point(position.X, position.Y + 1);
                if (!(this.PositionOutOfRange(point4) || (ScenarioMap.MapData[point4.X, point4.Y] != 6)))
                {
                    num++;
                }
                if (num > 2)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveAvail() => IsPlayerControlling() && EnableLoadAndSave && !Session.GlobalVariables.HardcoreMode;
       
        public bool LoadAvail() => IsPlayerControlling() && EnableLoadAndSave && !Session.GlobalVariables.HardcoreMode;

        public bool IsInCaptiveList(int personId)
        {
            var captives = GetCaptives();
            foreach (var captive in captives)
            {
                if (captive.CaptivePerson.ID == personId)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static CommonData ProcessCommonData(CommonData commonData)
        {
            commonData.NumberGenerator = new CombatNumberGenerator();

            commonData.TroopAnimations = new TroopAnimation();

            // TODO: CommonData需移除InfluenceTable
            var allInfluences = commonData.AllInfluences;
            var allConditions = commonData.AllConditions;

            LoadGameCommonData();

            var allTitleKinds = commonData.AllTitleKinds;
            foreach (var title in commonData.AllTitles.Values)
            {
                if (allTitleKinds.TryGetValue(title.KindId, out var titleKind))
                {
                    title.Kind = titleKind;
                }
                else
                {
                    logger.Error($"称号Id:[{title.ID}]没有对应称号类别");
                }
            }

            foreach (var influence in allInfluences.Values ?? Enumerable.Empty<Influence>())
            {
                influence.Init();
            }

            foreach (var facilityKind in commonData.AllFacilityKinds.Values)
            {
                facilityKind.AIBuildConditionWeight = Condition.LoadConditionWeightFromString(allConditions, facilityKind.AIBuildConditionWeightString);
            }

            foreach (var facilityLevel in commonData.AllFacilityKindLevels.Values)
            {
                facilityLevel.Influences = StaticMethods.LoadFromString(allInfluences, facilityLevel.InfluencesString).Values.ToList();
                facilityLevel.Conditions = StaticMethods.LoadFromString(allConditions, facilityLevel.ConditionTableString).Values.ToList();
            }
            commonData.GroupedFacilityKindLevels = commonData.AllFacilityKindLevels.Values.GroupBy(x => x.KindId).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var technique in commonData.AllTechniques.Values)
            {
                technique.Influences = StaticMethods.LoadFromString(allInfluences, technique.InfluencesString).Values.ToList();
                technique.Conditions = StaticMethods.LoadFromString(allConditions, technique.ConditionTableString).Values.ToList();
                technique.AIConditionWeight = Condition.LoadConditionWeightFromString(allConditions, technique.AIConditionWeightString);
            }

            foreach (var skill in commonData.AllSkills.Values)
            {
                skill.Influences = StaticMethods.LoadFromString(allInfluences, skill.InfluencesString);
                skill.Conditions = StaticMethods.LoadFromString(allConditions, skill.ConditionTableString).Values.ToList();
            }

            foreach (var title in commonData.AllTitles.Values)
            {
                title.Init();
                title.Influences = StaticMethods.LoadFromString(allInfluences, title.InfluencesString);
                title.Conditions = StaticMethods.LoadFromString(allConditions, title.ConditionTableString).Values.ToList();
                title.ArchitectureConditions = StaticMethods.LoadFromString(allConditions, title.ArchitectureConditionsString).Values.ToList();
                title.FactionConditions = StaticMethods.LoadFromString(allConditions, title.FactionConditionsString).Values.ToList();
                title.LoseConditions = StaticMethods.LoadFromString(allConditions, title.LoseConditionsString).Values.ToList();
                title.GenerateConditions = StaticMethods.LoadFromString(allConditions, title.GenerateConditionsString).Values.ToList();
            }

            foreach (var militaryKind in commonData.AllMilitaryKinds.Values)
            {
                militaryKind.Influences = StaticMethods.LoadFromString(allInfluences, militaryKind.InfluencesString).Values.ToList();
                militaryKind.CreateConditions = StaticMethods.LoadFromString(allConditions, militaryKind.CreateConditionsString).Values.ToList();
                militaryKind.AICreateArchitectureConditionWeight = Condition.LoadConditionWeightFromString(allConditions, militaryKind.AICreateArchitectureConditionWeightString);
                militaryKind.AIUpgradeArchitectureConditionWeight = Condition.LoadConditionWeightFromString(allConditions, militaryKind.AIUpgradeArchitectureConditionWeightString);
                militaryKind.AIUpgradeLeaderConditionWeight = Condition.LoadConditionWeightFromString(allConditions, militaryKind.AIUpgradeLeaderConditionWeightString);
                militaryKind.AILeaderConditionWeight = Condition.LoadConditionWeightFromString(allConditions, militaryKind.AILeaderConditionWeightString);
                militaryKind.Successor = StaticMethods.LoadFromString(commonData.AllMilitaryKinds, militaryKind.SuccessorString).Values.ToList();
            }

            foreach (var combatMethod in commonData.AllCombatMethods.Values)
            {
                combatMethod.Influences = StaticMethods.LoadFromString(allInfluences, combatMethod.InfluencesString).Values.ToList();
                combatMethod.CastConditions = StaticMethods.LoadFromString(allConditions, combatMethod.CastConditionsString).Values.ToList();
                combatMethod.AIConditionWeightSelf = Condition.LoadConditionWeightFromString(allConditions, combatMethod.AIConditionWeightSelfString);
                combatMethod.AIConditionWeightEnemy = Condition.LoadConditionWeightFromString(allConditions, combatMethod.AIConditionWeightEnemyString);
            }

            foreach (var stunt in commonData.AllStunts.Values)
            {
                stunt.Influences = StaticMethods.LoadFromString(allInfluences, stunt.InfluencesString);
                stunt.CastConditions = StaticMethods.LoadFromString(allConditions, stunt.CastConditionsString).Values.ToList();
                stunt.LearnConditions = StaticMethods.LoadFromString(allConditions, stunt.LearnConditionsString).Values.ToList();
                stunt.AIConditions = StaticMethods.LoadFromString(allConditions, stunt.AIConditionsString).Values.ToList();
            }

            foreach (var stratagem in commonData.AllStratagems.Values)
            {
                stratagem.Influences = StaticMethods.LoadFromString(allInfluences, stratagem.InfluencesString).Values.ToList();
                stratagem.CastConditions = StaticMethods.LoadFromString(allConditions, stratagem.CastConditionsString).Values.ToList();
                stratagem.AIConditionWeightSelf = Condition.LoadConditionWeightFromString(allConditions, stratagem.AIConditionWeightSelfString);
                stratagem.AIConditionWeightEnemy = Condition.LoadConditionWeightFromString(allConditions, stratagem.AIConditionWeightEnemyString);
            }

            foreach (var statusEffect in commonData.AllStatusEffects.Values)
            {
                statusEffect.Influences = StaticMethods.LoadFromString(allInfluences, statusEffect.InfluenceString).Values.ToList();
            }

            return commonData;
        }

        public List<string> ProcessScenarioData(bool fromScenario, bool editing = false)  //读剧本和读存档都调用了此函数
        {
            var errorMsg = new List<string>();

            Init();
            
            scenarioJustLoaded = true;
                        
            ScenarioMap.LoadMapData(ScenarioMap.MapDataString, ScenarioMap.MapDimensions.X, ScenarioMap.MapDimensions.Y);
            ScenarioMap.Init();
                       
            //if (Platform.PlatFormType == PlatFormType.Android || Platform.PlatFormType == PlatFormType.iOS || Platform.PlatFormType == PlatFormType.Win)
            //{
//                ScenarioMap.TileWidth = 50;
                //ScenarioMap.TileHeight = 50;
            //}

            var dirPath = @"Content\Save";
            var facilityStore = new JsonStore<FacilityConfig>(Path.Combine(dirPath, "Facilities.json"));
            var facilities = facilityStore.Load();
            Facilities = facilities.Select(x => new Facility(x)).ToDictionary(x => x.ID);

            var informationStore = new JsonStore<InformationConfig>(Path.Combine(dirPath, "Informations.json"));
            var informations = informationStore.Load();
            Informations = informations.Select(x => new Information(x)).ToDictionary(x => x.ID);

            var architectureStore = new JsonStore<ArchitectureConfig>(Path.Combine(dirPath, "Architectures.json"));
            var architectures = architectureStore.Load();
            Architectures = architectures.Select(x => new Architecture(x)).ToDictionary(x => x.ID);

            var personStore = new JsonStore<PersonConfig>(Path.Combine(dirPath, "Persons.json"));
            var persons = personStore.Load();
            AllPersons = persons.Select(x => new Person(x)).ToDictionary(x => x.ID);

            var stateStore = new JsonStore<StateConfig>(Path.Combine(dirPath, "States.json"));
            var states = stateStore.Load();
            States = states.Select(x => new State(x)).ToDictionary(x => x.ID);

            var regionStore = new JsonStore<RegionConfig>(Path.Combine(dirPath, "Regions.json"));
            var regions = regionStore.Load();
            Regions = regions.Select(x => new Region(x)).ToDictionary(x => x.ID);

            var sectionStore = new JsonStore<SectionConfig>(Path.Combine(dirPath, "Sections.json"));
            var sections = sectionStore.Load();
            Sections = sections.Select(x => new Section(x)).ToDictionary(x => x.ID);

            var militaryStore = new JsonStore<MilitaryConfig>(Path.Combine(dirPath, "Militaries.json"));
            var militaries = militaryStore.Load();
            Militaries = militaries.Select(x => new Military(x)).ToDictionary(x => x.ID);


            // var legionStore = new JsonStore<LegionConfig>(Path.Combine(dirPath, "Legions.json"));

            foreach (var state in States.Values)
            {
                state.ContactStates = StaticMethods.LoadFromString(States, state.ContactStatesString).Values.ToList();
            }

            foreach (var region in Regions.Values)
            {
                var regionStates = StaticMethods.LoadFromString(States, region.StatesListString).Values.ToList();
                foreach (var item in regionStates)
                {
                    item.LinkedRegion = region;
                }
                region.States = regionStates;
            }

            foreach (var person in AllPersons.Values)
            {
                List<string> errors = new List<string>();

                person.Init();

                if (GameCommonData.AllIdealTendencyKinds.TryGetValue(person.IdealTendencyIDString, out var idealTendencyKind))
                {
                    person.IdealTendency = idealTendencyKind;
                }

                if (GameCommonData.AllCharacterKinds.TryGetValue(person.PCharacter, out var characterKind))
                {
                    person.Character = characterKind;
                }

                person.UniqueMilitaryKinds = StaticMethods.LoadFromString(GameCommonData.AllMilitaryKinds, person.UniqueMilitaryKindsString).Values.ToList();
                person.UniqueTitles = StaticMethods.LoadFromString(GameCommonData.AllTitles, person.UniqueTitlesString).Values.ToList();

                //errors.AddRange(person.Guanzhis.LoadFromString(this.GameCommonData.AllTitles, reader["Guanzhis"].ToString()));

                person.Skills = StaticMethods.LoadFromString(GameCommonData.AllSkills, person.SkillsString);

                if (GameCommonData.AllTitles.TryGetValue(person.StudyingTitleString, out var title))
                {
                    person.StudyingTitle = title;
                }

                person.RealTitles = StaticMethods.LoadFromString(GameCommonData.AllTitles, person.RealTitlesString).Values.ToList();

                // TODO: catch里无法命中
                // try
                // {
                //     person.RealTitles = StaticMethods.LoadFromString<Title>(GameCommonData.AllTitles, person.RealTitlesString).Values.ToList();
                // }
                // catch
                // {
                //     if (GameCommonData.AllTitles.TryGetValue(person.PersonalTitleString, out var title1))
                //     {
                //         person.RealTitles.Add(title1);
                //     }

                //     if (GameCommonData.AllTitles.TryGetValue(person.CombatTitleString, out var title2))
                //     {
                //         person.RealTitles.Add(title2);
                //     }
                // }

                person.Stunts = StaticMethods.LoadFromString(GameCommonData.AllStunts, person.StuntsString);

                if (GameCommonData.AllStunts.TryGetValue(person.StudyingStuntString, out var stunt))
                {
                    person.StudyingStunt = stunt;
                }

                if (GameCommonData.AllTrainPolicies.TryGetValue(person.TrainPolicyIDString, out var trainPolicy))
                {
                    person.TrainPolicy = trainPolicy;
                }

                person.WaitForFeiZi = AllPersons.GetValueOrDefault(person.waitForFeiziId);
                person.PreferredTroopPersons = StaticMethods.LoadFromString(AllPersons, person.preferredTroopPersonsString).Values.ToList();

                // Persons.AddPersonWithEvent(person, false);  //所有武将，并加载武将事件

                // this.AllChildren.Add(person, person.NumberOfChildren);

                if (person.Available && person.Alive)
                {
                    AvailablePersons.Add(person.ID, person);
                }
            }

            foreach (var (childrenId, fatherId) in FatherIds)
            {
                if (AllPersons.ContainsKey(childrenId) && AllPersons.ContainsKey(fatherId))
                {
                    AllPersons[childrenId].Father = AllPersons[fatherId];
                }
            }

            foreach (var (childrenId, motherId) in MotherIds)
            {
                if (AllPersons.ContainsKey(childrenId) && AllPersons.ContainsKey(motherId))
                {
                    AllPersons[childrenId].Father = AllPersons[motherId];
                }
            }

            foreach (var (key, value) in SpouseIds)
            {
                var person1 = AllPersons.GetValueOrDefault(key);
                var person2 = AllPersons.GetValueOrDefault(value);

                if (person1 != null && person2 != null)
                {
                    person1.Spouse = person2;

                    if (fromScenario)
                    {
                        person1.EnsureRelationAtLeast(person2, Session.Parameters.VeryCloseThreshold);
                    }
                }
            }

            foreach (var (key, ids) in BrotherIds)
            {
                var person = AllPersons.GetValueOrDefault(key);

                if (person == null)
                {
                    logger.Error($"兄弟关系的人物Id: [{key}]不存在");
                    continue;
                }

                foreach (var id in ids)
                {
                    var brother = AllPersons.GetValueOrDefault(id);

                    if (brother != null)
                    {
                        person.Brothers.Add(brother);

                        if (fromScenario)
                        {
                            person.EnsureRelationAtLeast(brother, Session.Parameters.VeryCloseThreshold);
                        }
                    }
                    else
                    {
                        logger.Error($"兄弟关系的兄弟人物Id: [{id}]不存在");
                    }
                }

                // if (ids.Length == 1 && ids[0] != -1)
                // {
                //     foreach (KeyValuePair<int, int[]> j in BrotherIds)
                //     {
                //         if (j.Value.Length > 0 && ids[0] == j.Value[0])
                //         {
                //             Person p = this.Persons.GetGameObject(i.Key) as Person;
                //             Person q = this.Persons.GetGameObject(j.Key) as Person;
                //             if (p != null)
                //             {
                //                 p.Brothers.Add(q);
                //                 if (q != null && fromScenario)
                //                 {
                //                     p.EnsureRelationAtLeast(q, Session.Parameters.VeryCloseThreshold);
                //                 }
                //             }
                //         }
                //     }
                // }
                // else
                // {
                //     var person = AllPersons.GetValueOrDefault(key);

                //     if (person == null)
                //     {
                //         logger.Error($"兄弟关系的人物Id: [{key}]不存在");
                //         continue;
                //     }

                //     foreach (var id in ids)
                //     {
                //         var brother = AllPersons.GetValueOrDefault(id);

                //         if (brother != null)
                //         {
                //             person.Brothers.Add(brother);
                            
                //             if (fromScenario)
                //             {
                //                 person.EnsureRelationAtLeast(brother, Session.Parameters.VeryCloseThreshold);
                //             }
                //         }
                //         else
                //         {
                //             logger.Error($"兄弟关系的兄弟人物Id: [{id}]不存在");
                //         }
                //     }
                // }
            }

            foreach (var (key, ids) in CloseIds)
            {
                var person = AllPersons.GetValueOrDefault(key);

                if (person == null)
                {
                    logger.Error($"亲密关系的人物Id: [{key}]不存在");
                    continue;
                }

                foreach (var id in ids)
                {
                    var closePerson = AllPersons.GetValueOrDefault(id);

                    if (closePerson != null)
                    {
                        person.AddClose(closePerson);
                    }
                    else
                    {
                        logger.Error($"亲密关系的亲爱人物Id: [{id}]不存在");
                    }
                }
            }

            foreach (var (key, ids) in HatedIds)
            {
                var person = AllPersons.GetValueOrDefault(key);

                if (person == null)
                {
                    logger.Error($"厌恶关系的人物Id: [{key}]不存在");
                    continue;
                }

                foreach (var id in ids)
                {
                    var hatedPerson = AllPersons.GetValueOrDefault(id);

                    if (hatedPerson != null)
                    {
                        person.AddHated(hatedPerson);
                    }
                    else
                    {
                        logger.Error($"厌恶关系的厌恶人物Id: [{id}]不存在");
                    }
                }
            }

            foreach (var (key, ids) in SuoshuIds)
            {
                var person = AllPersons.GetValueOrDefault(key);

                if (person == null)
                {
                    logger.Error($"所属关系的人物Id: [{key}]不存在");
                    continue;
                }

                foreach (var id in ids)
                {
                    var belongedPerson = AllPersons.GetValueOrDefault(id);

                    if (belongedPerson != null)
                    {
                        person.suoshurenwuList.Add(belongedPerson);
                    }
                    else
                    {
                        logger.Error($"所属关系的所属人物Id: [{id}]不存在");
                    }
                }
            }

            foreach (var (id, otherId) in MarriageGranterId)
            {
                var person = AllPersons.GetValueOrDefault(id);
                var otherPerson = AllPersons.GetValueOrDefault(otherId);

                if (person != null && otherPerson != null)
                {
                    person.marriageGranter = otherPerson;
                }
            }

            foreach (var person in AllPersons.Values)
            {
                var spouse = person.Spouse;

                if (spouse != null && !person.suoshurenwuList.HasGameObject(spouse))
                {
                    person.suoshurenwuList.Add(spouse);
                    person.Spouse.suoshurenwuList.Add(person);
                }
            }

            foreach (var (id, biography) in AllBiographies.Biographys)
            {
                var person = AllPersons.GetValueOrDefault(id);

                if (person == null) continue;

                biography.MilitaryKinds = StaticMethods.LoadFromString(GameCommonData.AllMilitaryKinds, biography.MilitaryKindsString).Values.ToList();

                if (biography.MilitaryKinds.Count == 0)
                {
                    logger.Error($"列传人物Id: [{id}]没有基本兵种。");
                }

                person.PersonBiography = biography;
            }

            foreach (var person in AllPersons.Values)
            {
                if (person.PersonBiography == null)
                {
                    var biography = new Biography
                    {
                        ID = person.ID,
                        FactionColor = 52,
                        Brief = "",
                        History = "",
                        Romance = "",
                        InGame = "",
                    };
                    biography.AddBasicMilitaryKinds();

                    person.PersonBiography = biography;
                    AllBiographies.AddBiography(biography);
                }
            }

            foreach (var relation in PersonRelationIds)
            {
                var id = relation.PersonID1;
                var otherId = relation.PersonID2;

                var person = AllPersons.GetValueOrDefault(id);
                var otherPerson = AllPersons.GetValueOrDefault(otherId);
                
                if (person != null && otherPerson != null)
                {
                    person.SetRelation(otherPerson, relation.Relation);
                }

                if (person == null)
                {
                    logger.Error($"人物关系, 人物Id: [{id}]不存在");
                }
                if (otherPerson == null)
                {
                    logger.Error($"人物关系, 其他人物Id: [{otherId}]不存在");
                }
            }

            if (captiveData != null && !editing)
            {
                foreach (Captive captive in captiveData)
                {
                    var personId = captive.CaptivePersonID;
                    var person = AllPersons.GetValueOrDefault(personId);
                    if (person == null)
                    {
                        logger.Error($"俘虏Id: [{captive.ID}], 人物Id: [{personId}]不存在");
                        continue;
                    }
                    else
                    {
                        person.SetBelongedCaptive(captive, PersonStatus.Captive);
                        person.Status = PersonStatus.Captive;
                        captive.CaptivePerson = person;
                    }
                }
            }

            // TODO: 俘虏绑定事件

            // Captives.BindEvents();

            foreach (var military in Militaries.Values)
            {
                military.Init();

                var kindId = military.KindID;
                if (!GameCommonData.AllMilitaryKinds.ContainsKey(kindId))
                {
                    logger.Error($"编队Id: [{military.ID}], 军队Id: [{kindId}]不存在");
                    continue;
                }

                var person = AllPersons.GetValueOrDefault(military.RecruitmentPersonID);
                if (person != null)
                {
                    person.RecruitMilitary(military);
                }

                //foreach (Person p in this.Persons)
                //{
                //    if (p.ID == military.RecruitmentPersonID)
                //    {
                //        //p.RecruitmentMilitary = military;
                //        p.RecruitMilitary(military);
                //    }
                //}
            }

            this.InitializeMilitaryData();

            var captiveDict = GetCaptives().ToDictionary(x => x.ID);

            // 处理建筑数据
            foreach (var architecture in Architectures.Values)
            {
                List<string> e = new List<string>();

                architecture.Init();
                
                // 建筑类型
                if (GameCommonData.AllArchitectureKinds.TryGetValue(architecture.KindId, out var kind))
                {
                    architecture.Kind = kind;
                }
                else
                {
                    logger.Error($"建筑种类Id：{architecture.KindId}, 不存在");
                }

                var architectureId = architecture.ID;
                var stateId = architecture.StateID;

                var state = States.GetValueOrDefault(stateId);
                if (state != null)
                {
                    state.Architectures.Add(architecture);
                    state.LinkedRegion.Architectures.Add(architecture);
                    
                    if (state.StateAdminID == architectureId)
                    {
                        state.StateAdmin = architecture;
                    }
                    if (state.LinkedRegion.RegionCoreID == architectureId)
                    {
                        state.LinkedRegion.RegionCore = architecture;
                    }

                    architecture.LocationState = state;
                }
                else
                {
                    logger.Error($"州域Id: [{stateId}]不存在");
                }

                architecture.Characteristics = StaticMethods.LoadFromString(GameCommonData.AllInfluences, architecture.CharacteristicsString);

                architecture.LoadFromString(architecture.ArchitectureArea, architecture.ArchitectureAreaString);

                //if (architecture.ArchitectureArea == null)
                //{
                //    architecture.ArchitectureArea = new GameArea();
                //}

                //if (architecture.ArchitectureArea.Area == null)
                //{
                //    architecture.ArchitectureArea.Area = new List<Point>();
                //}

                //architecture.PersonsString = reader["Persons"].ToString();
                //architecture.MovingPersonsString = reader["MovingPersons"].ToString();
                //architecture.NoFactionPersonsString = reader["NoFactionPersons"].ToString();
                //architecture.NoFactionMovingPersonsString = reader["NoFactionMovingPersons"].ToString();
                //architecture.feiziliebiaoString = reader["feiziliebiao"].ToString();

                e.AddRange(architecture.LoadPersonsFromString(AllPersons, architecture.PersonsString, PersonStatus.Normal));
                e.AddRange(architecture.LoadPersonsFromString(AllPersons, architecture.MovingPersonsString, PersonStatus.Moving));
                e.AddRange(architecture.LoadPersonsFromString(AllPersons, architecture.NoFactionPersonsString, PersonStatus.NoFaction));
                e.AddRange(architecture.LoadPersonsFromString(AllPersons, architecture.NoFactionMovingPersonsString, PersonStatus.NoFactionMoving));
                e.AddRange(architecture.LoadPersonsFromString(AllPersons, architecture.feiziliebiaoString, PersonStatus.Princess));

                var architectureMilitaries = StaticMethods.LoadFromString(Militaries, architecture.MilitariesString).Values.ToList();
                architecture.InitMilitaries(architectureMilitaries);

                architecture.Facilities = StaticMethods.LoadFromString(Facilities, architecture.FacilitiesString).Values.ToList();

                architecture.InitFundPacks();
                architecture.InitFoodPacks();
                architecture.InitPoplationPacks();

                e.AddRange(architecture.LoadMilitaryPopulationPacksFromString(architecture.MilitaryPopulationPacksString));

                var captives = StaticMethods.LoadFromString(captiveDict, architecture.CaptivesString).Values;
                foreach (var captive in captives)
                {
                    var captivePerson = captive.CaptivePerson;

                    if (captivePerson == null) continue;

                    captivePerson.LocationArchitecture = architecture;
                    captivePerson.LocationTroop = null;
                    captivePerson.Status = PersonStatus.Captive;
                }

                //architecture.AILandLinksString = reader["AILandLinks"].ToString();
                //architecture.AIWaterLinksString = reader["AIWaterLinks"].ToString();

                if (GameCommonData.AllDisasterKinds.TryGetValue(architecture.zainan.zainanleixing, out var disasterKind))
                {
                    architecture.zainan.DisasterKind = disasterKind;
                }
                else
                {
                    architecture.youzainan = false;
                }

                // 初始化情报
                var formations = StaticMethods.LoadFromString(Informations, architecture.InformationsString).Values.ToList();

                foreach (var item in formations)
                {
                    item.BelongedArchitecture = architecture;
                }
                architecture.Informations = formations;

                architecture.AIBattlingArchitectures = new ArchitectureList();

                if (e.Count > 0)
                {
                    errorMsg.Add("建筑ID" + architecture.ID + "：");
                    errorMsg.AddRange(e);
                }
                //else
                //{
                    // this.Architectures.AddArchitectureWithEvent(architecture, false);
                //}

            }

            foreach (var (key, arrays) in AiBattlingArchitectureStrings)
            {
                var architecture = Architectures.GetValueOrDefault(key);

                if (architecture == null) continue;

                foreach (int i in arrays)
                {
                    architecture.AIBattlingArchitectures.Add(Architectures.GetValueOrDefault(i));
                }
            }

            foreach(Routeway routeway in Routeways)
            {
                List<string> e = new List<string>();

                routeway.Init();

                routeway.StartArchitecture = Architectures.GetValueOrDefault(routeway.StartArchitectureString);

                if (routeway.StartArchitecture != null)
                {
                    routeway.StartArchitecture.Routeways.Add(routeway);
                }
                else
                {
                    e.Add("建筑ID" + routeway.StartArchitectureString + "不存在");
                }

                routeway.EndArchitecture = Architectures.GetValueOrDefault(routeway.EndArchitectureString);

                routeway.DestinationArchitecture = Architectures.GetValueOrDefault(routeway.DestinationArchitectureString);

                routeway.BelongedFaction = this.Factions.GetGameObject(routeway.BelongedFactionString) as Faction;

                //routeway.LoadRoutePointsFromString(reader["Points"].ToString());

                if (e.Count > 0)
                {
                    errorMsg.Add("粮道ID" + routeway.ID + "：");
                    errorMsg.AddRange(e);
                }
                //this.Routeways.AddRoutewayWithEvent(routeway);
            }

            Troops.Init();
            
            foreach (Troop troop in Troops)
            {
                List<string> errors = new List<string>();

                troop.Init();

                troop.StartingArchitecture = Architectures.GetValueOrDefault(troop.StartingArchitectureString);

                if (troop.StartingArchitecture == null)
                {
                    errors.Add("起始建筑ID" + troop.StartingArchitectureString + "不存在");
                }

                //troop.PersonsString = reader["Persons"].ToString();
                //troop.LeaderIDString = (short)reader["LeaderID"];

                errors.AddRange(troop.LoadPersonsFromString(this.AllPersons, troop.PersonsString, troop.LeaderIDString));

                //troop.MilitaryID = (short)reader["MilitaryID"];
                //if (this.Militaries.GetGameObject(troop.MilitaryID) == null)
                //{
                //    errors.Add("编队ID" + troop.MilitaryID + "不存在");
                //}

                var captives = StaticMethods.LoadFromString(captiveDict, troop.CaptivesString).Values;
                foreach (var captive in captives)
                {
                    troop.AddCaptive(captive);
                }

                troop.EventInfluences = StaticMethods.LoadFromString(GameCommonData.AllInfluences, troop.EventInfluencesString).Values.ToList();

                troop.CombatMethods = StaticMethods.LoadFromString(GameCommonData.AllCombatMethods, troop.CombatMethodsString);

                if (GameCommonData.AllStunts.TryGetValue(troop.CurrentStuntIDString, out var stunt))
                {
                    troop.CurrentStunt = stunt;
                }

                if (GameCommonData.AllStratagems.TryGetValue(troop.CurrentStratagemID, out var stratagem))
                {
                    troop.CurrentStratagem = stratagem;
                }

                if (errors.Count > 0)
                {
                    errors.Add("部队ID" + troop.ID + "：");
                    errorMsg.AddRange(errors);
                }

                if (troop.Army != null && !editing)//取消编辑器人物气泡事件，以便于可以存档
                {
                    this.Troops.AddTroopWithEvent(troop, false);
                }
            }

            foreach(Legion legion in this.Legions)
            {
                legion.Init();

                legion.StartArchitecture = Architectures.GetValueOrDefault(legion.StartArchitectureString);

                legion.WillArchitecture = Architectures.GetValueOrDefault(legion.WillArchitectureString);

                //legion.PreferredRoutewayString = (int)reader["PreferredRouteway"];
                legion.PreferredRouteway = this.Routeways.GetGameObject(legion.PreferredRoutewayString) as Routeway;

                //legion.InformationDestination = StaticMethods.LoadFromString(reader["InformationDestination"].ToString());

                //legion.CoreTroopString = (int)reader["CoreTroop"];
                legion.CoreTroop = this.Troops.GetGameObject(legion.CoreTroopString) as Troop;

                //legion.TroopsString = reader["Troops"].ToString();
                legion.LoadTroopsFromString(this.Troops, legion.TroopsString);

                //this.Legions.AddLegionWithEvent(legion);
            }

            foreach (var section in Sections.Values)
            {
                if (GameCommonData.AllSectionAIDetails.TryGetValue(section.AIDetailIDString, out var sectionAIDetail))
                {
                    section.AIDetail = sectionAIDetail;
                }
                else
                {
                    logger.Error($"军区委任类型Id: [{section.AIDetailIDString}]不存在");
                }

                var sectionArchitectures = StaticMethods.LoadFromString(Architectures, section.ArchitecturesString).Values.ToList();
                foreach (var architecture in sectionArchitectures)
                {
                    architecture.BelongedSection = section;
                }
                section.Architectures = sectionArchitectures;
            }

            foreach (Faction faction in Factions)
            {
                List<string> e = new List<string>();

                faction.Init();

                var factionArchitectures = StaticMethods.LoadFromString(Architectures, faction.ArchitecturesString).Values.ToList();
                faction.InitArchitectures(factionArchitectures);

                var factionSections = StaticMethods.LoadFromString(Sections, faction.SectionsString).Values.ToList();
                faction.InitSections(factionSections);

                //faction.TroopListString = reader["Troops"].ToString();
                e.AddRange(faction.LoadTroopsFromString(this.Troops, faction.TroopListString));

                // 初始化情报
                var formations = StaticMethods.LoadFromString(Informations, faction.InformationsString).Values.ToList();

                foreach (var item in formations)
                {
                    item.BelongedFaction = faction;
                }
                faction.Informations = formations;

                //faction.RoutewaysString = reader["Routeways"].ToString();
                e.AddRange(faction.LoadRoutewaysFromString(this.Routeways, faction.RoutewaysString));

                //faction.LegionsString = reader["Legions"].ToString();
                e.AddRange(faction.LoadLegionsFromString(this.Legions, faction.LegionsString));

                var baseMilitaryKinds = StaticMethods.LoadFromString(GameCommonData.AllMilitaryKinds, faction.BaseMilitaryKindsString);
                if (baseMilitaryKinds.Count == 0)
                {
                    faction.AddBasicMilitaryKinds();
                }
                else
                {
                    faction.BaseMilitaryKinds = baseMilitaryKinds;
                }
                
                faction.AvailableTechniques = StaticMethods.LoadFromString(GameCommonData.AllTechniques, faction.AvailableTechniquesString);

                if (GameCommonData.AllTechniques.TryGetValue(faction.PlanTechniqueString, out var technique))
                {
                    faction.PlanTechnique = technique;
                }
                
                faction.TransferingMilitaries = StaticMethods.LoadFromString(Militaries, faction.TransferingMilitariesString).Values.ToList();

                // e.AddRange(faction.LoadMilitariesFromString(this.Militaries, faction.MilitariesString.NullToString()));


                //faction.GetGeneratorPersonCountString = reader["GetGeneratorPersonCount"].ToString();
                e.AddRange(faction.LoadGeneratorPersonCountFromString(faction.GetGeneratorPersonCountString.NullToString()));

                //取消储君序列化，原有的方法会导致二次存档后储君为空
                var prince = AllPersons.GetValueOrDefault(faction.PrinceID);
               
                if (e.Count > 0)
                {
                    errorMsg.Add("势力ID" + faction.ID + "：");
                    errorMsg.AddRange(e);
                }

                this.Factions.AddFactionWithEvent(faction, false);
            }

            this.DiplomaticRelations.Init(this.Factions);

            foreach (Treasure treasure in Treasures)
            {
                treasure.HidePlace = Architectures.GetValueOrDefault(treasure.HidePlaceIDString);

                //treasure.BelongedPersonIDString = (short)reader["BelongedPerson"];
                treasure.BelongedPerson = AllPersons.ContainsKey(treasure.BelongedPersonIDString) ? AllPersons[treasure.BelongedPersonIDString] : null;
                
                if (treasure.BelongedPerson != null)
                {
                    treasure.BelongedPerson.Treasures.Add(treasure);
                }

                treasure.Influences = StaticMethods.LoadFromString(GameCommonData.AllInfluences, treasure.InfluencesString);

                //this.Treasures.AddTreasure(treasure);
            }

            //foreach (var dr in this.DiplomaticRelations.DiplomaticRelations)
            //{

            //}

            foreach (TroopEvent te in TroopEvents)
            {
                te.Init();

                te.LaunchPerson = AllPersons.GetValueOrDefault(te.LaunchPersonString);

                te.Conditions = StaticMethods.LoadFromString(GameCommonData.AllConditions, te.ConditionsString).Values.ToList();

                //te.TargetPersonsString = reader["TargetPersons"].ToString();
                te.LoadTargetPersonFromString(this.AllPersons, te.TargetPersonsString);

                te.SelfEffects = StaticMethods.LoadFromString(GameCommonData.AllTroopEventEffects, te.SelfEffectsString).Values.ToList();

                te.LoadEffectPersonFromString(AllPersons, GameCommonData.AllTroopEventEffects, te.EffectPersonsString);
                te.LoadEffectAreaFromString(this.GameCommonData.AllTroopEventEffects, te.EffectAreasString);

                te.LoadDialogFromString(this.AllPersons, te.dialogString);
                if (te.TryToShowString == null) te.TryToShowString = "";
                this.TroopEvents.AddTroopEventWithEvent(te, false);
            }

            foreach (Event e in this.AllEvents)
            {
                e.Init();

                e.person = e.LoadPersonIdFromString(AllPersons, e.personString);

                e.personCond = StaticMethods.LoadListFromString(GameCommonData.AllConditions, e.PersonCondString);
                
                e.Architectures = StaticMethods.LoadFromString(Architectures, e.architectureString).Values.ToList();

                e.architectureCond = StaticMethods.LoadFromString(GameCommonData.AllConditions, e.architectureCondString).Values.ToList();

                //e.factionString = reader["FactionID"].ToString();
                e.LoadFactionFromString(this.Factions, e.factionString);

                e.factionCond = StaticMethods.LoadFromString(GameCommonData.AllConditions, e.factionCondString).Values.ToList();

                e.effect = StaticMethods.LoadListFromString(GameCommonData.AllEventEffects, e.effectString);

                e.architectureEffect = StaticMethods.LoadFromString(GameCommonData.AllEventEffects, e.architectureEffectString).Values.ToList();
                e.factionEffect = StaticMethods.LoadFromString(GameCommonData.AllEventEffects, e.factionEffectIDString).Values.ToList();

                if (e.dialogString != null)
                {
                    e.LoadDialogFromString(e.dialogString);
                }

                e.yesEffect = StaticMethods.LoadListFromString(GameCommonData.AllEventEffects, e.yesEffectString);
                e.noEffect = StaticMethods.LoadListFromString(GameCommonData.AllEventEffects, e.noEffectString);

                if (e.yesdialogString != null)
                {
                    e.LoadyesDialogFromString(e.yesdialogString);
                }
                if (e.nodialogString != null)
                {
                    e.LoadnoDialogFromString(e.nodialogString);
                }

                e.yesArchitectureEffect = StaticMethods.LoadFromString(GameCommonData.AllEventEffects, e.yesArchitectureEffectString).Values.ToList();
                e.noArchitectureEffect = StaticMethods.LoadFromString(GameCommonData.AllEventEffects, e.noArchitectureEffectString).Values.ToList();

                if (e.scenBiographyString != null)
                {
                    e.LoadScenBiographyFromString(e.scenBiographyString);
                }

                if (e.TryToShowString == null) e.TryToShowString = "";
                //e.LoadScenBiographyFromString(reader["ScenBiography"].ToString());
                this.AllEvents.AddEventWithEvent(e, false);
            }
            if(!editing)//这里不加条件的话，用剧本编辑器读取有错剧本时，可能出现游戏主程序能读剧本而编辑器打不开剧本的情况
            {
                foreach (var person in AllPersons.Values)
                {
                    if (person.Status == PersonStatus.Normal || person.Status == PersonStatus.Moving)
                    {
                        if (person.LocationArchitecture != null && person.LocationArchitecture.BelongedFaction == null)
                        {
                            logger.Error($"人物Id: [{person.ID}]在一座没有势力的城池仕官");
                            if (person.Status == PersonStatus.Normal)
                            {
                                person.Status = PersonStatus.NoFaction;
                            }
                            else
                            {
                                person.Status = PersonStatus.NoFactionMoving;
                            }
                        }
                    }
                    if (person.Status == PersonStatus.Moving || person.Status == PersonStatus.NoFactionMoving)
                    {
                        if (person.ArrivingDays <= 0)
                        {
                            logger.Error($"人物Id: [{person.ID}]正移动，但没有移动天数");
                            person.ArrivingDays = 1;
                        }
                    }
                    if (person.Available && person.Alive && person.LocationArchitecture == null && person.LocationTroop == null && (person.ID < 7000 || person.ID >= 8000))
                    {
                        if (person.Status != PersonStatus.Princess)
                        {
                            logger.Error($"人物Id: [{person.ID}]已登场，但没有所属建筑");
                            person.Available = false;
                            person.Alive = false;
                            person.Status = PersonStatus.None;
                        }
                    }
                }
                ClearTempDic();
            }

            this.YearTable.Init();
            //this.YearTable = new YearTable();

            this.alterTransportShipAdaptibility();

            //using (TextWriter tw = new StreamWriter(SCENARIO_ERROR_TEXT_FILE))
            //{
            //    foreach (string s in errorMsg)
            //    {
            //        tw.WriteLine(s);
            //    }
            //}

            ExtensionInterface.call("Load", new Object[] { this });

            return errorMsg;
        }

        void ClearTempDic()
        {
            FatherIds.Clear();
            MotherIds.Clear();
            SpouseIds.Clear();
            BrotherIds.Clear();
            SuoshuIds.Clear();
            CloseIds.Clear();
            HatedIds.Clear();
            MarriageGranterId.Clear();
            PersonRelationIds.Clear();
        }
        
        private void alterTransportShipAdaptibility()
        {
            if (GameCommonData.AllMilitaryKinds.TryGetValue(28, out var militaryKind))
            {
                if (Session.GlobalVariables.LandArmyCanGoDownWater)
                {
                    militaryKind.OneAdaptabilityKind = 0;
                    /*militaryKind.PlainAdaptability = 5;
                    militaryKind.GrasslandAdaptability = 5;
                    militaryKind.ForrestAdaptability = 6;
                    militaryKind.MarshAdaptability = 100;
                    militaryKind.MountainAdaptability = 10;
                    militaryKind.WaterAdaptability = 5;
                    militaryKind.RidgeAdaptability = 100;
                    militaryKind.WastelandAdaptability = 6;
                    militaryKind.DesertAdaptability = 10;
                    militaryKind.CliffAdaptability = 7;*/
                }
                else
                {
                    militaryKind.OneAdaptabilityKind = 6;
                    militaryKind.PlainAdaptability = 100;
                    militaryKind.GrasslandAdaptability = 100;
                    militaryKind.ForrestAdaptability = 100;
                    militaryKind.MarshAdaptability = 100;
                    militaryKind.MountainAdaptability = 100;
                    //militaryKind.WaterAdaptability = 5;
                    militaryKind.RidgeAdaptability = 100;
                    militaryKind.WastelandAdaptability = 100;
                    militaryKind.DesertAdaptability = 100;
                    militaryKind.CliffAdaptability = 100;
                }
            }
        }

        private void ApplyInformations()
        {
            foreach (var information in Informations.Values)
            {
                information.Apply();
            }
        }

        public void ForceOptionsOnAutoplay()
        {
            if (this.PlayerFactions.Count == 0)
            {
                Session.GlobalVariables.SkyEye = true;
                Session.GlobalVariables.EnableCheat = true;
                Session.GlobalVariables.HardcoreMode = false;
            }
        }

        public void InitPluginsWithScenario(MainGameScreen screen)
        {
            foreach (GameObject plugin in screen.PluginList)
            {
                if (plugin is IScenarioAwarePlugin)
                {
                    ((IScenarioAwarePlugin)plugin).SetScenario();
                }
            }
        }

        private void MigrateScenario()
        {
            foreach (var architecture in Architectures.Values)
            {
                if (architecture.MilitaryPopulation == 0)
                {
                    architecture.MilitaryPopulation = (int)(architecture.Population * (0.25 + (500000 - architecture.Population) / 500000 * 0.25));
                }
            }
        }

        private void DeleteInvalidRelations()
        {
            foreach (var person in AllPersons.Values)
            {
                if (person.Spouse != null && !person.Spouse.Alive)
                {
                    person.Spouse = null;
                }

                if (person.Brothers != null)
                {
                    foreach (Person brother in person.Brothers.GetList())
                    {
                        if (!brother.Alive)
                        {
                            person.Brothers.Remove(brother);
                        }
                    }
                }
            }
        }

        public void AfterLoadGameScenario(MainGameScreen screen)
        {
            MigrateScenario();

            DeleteInvalidRelations();

            this.InitPluginsWithScenario(screen);
            this.InitializeMapData();
            this.TroopAnimations.UpdateDirectionAnimations(ScenarioMap.TileWidth);
            this.ApplyFireTable();
            this.InitializeArchitectureMapTile();
            this.InitializeFactionData();
            this.ApplyInformations();
            this.Preparing = true;
            this.Factions.BuildQueue(true);
            this.Factions.ApplyInfluences();
            ApplyArchitectureInfluences();
            ApplyPersonInfluences();
            this.Preparing = false;
            this.InitialGameData();
            Session.Parameters.InitBaseRates();

            if (this.OnAfterLoadScenario != null)
            {
                this.OnAfterLoadScenario();
            }

            this.LoadedFileName = "";

            this.sessionStartTime = DateTime.Now;
        }

        public void AfterLoadSaveFile(MainGameScreen screen)
        {
            this.InitPluginsWithScenario(screen);
            this.InitializeMapData();
            this.TroopAnimations.UpdateDirectionAnimations(ScenarioMap.TileWidth);
            this.ApplyFireTable();
            this.InitializeArchitectureMapTile();
            this.InitializeFactionData();
            this.ApplyInformations();
            this.Preparing = true;

            this.Factions.BuildQueue(true);  //待考慮效果
            
            this.Factions.ApplyInfluences();            
            ApplyArchitectureInfluences();

            ApplyPersonInfluences();

            this.Preparing = false;

            this.InitialGameData();

            if (this.OnAfterLoadScenario != null)
            {
                this.OnAfterLoadScenario();
            }
            
            if (this.PlayerFactions.Count == 0)
            {
                oldDialogShowTime = Setting.Current.GlobalVariables.DialogShowTime;
                Setting.Current.GlobalVariables.DialogShowTime = 0;
            }
            else
            {
                //if (oldDialogShowTime >= 0)
                if (oldDialogShowTime > 0)
                {
                    Setting.Current.GlobalVariables.DialogShowTime = oldDialogShowTime;
                }
                else
                {
                    //Setting.Current.GlobalVariables.DialogShowTime = Session.globalVariablesBasic.DialogShowTime;
                }
            } 
            this.ForceOptionsOnAutoplay();

            this.sessionStartTime = DateTime.Now;
        }

        private void ApplyArchitectureInfluences()
        {
            foreach (var architecture in Architectures.Values)
            {
                architecture.ApplyInfluences();
            }
        }

        public void AfterInit()
        {
            if (this.CurrentPlayer != null)
            {
                detectCurrentPlayerBattleState(this.CurrentPlayer, true);
                this.CurrentPlayer.RefreshImportantPerson();
            }
        }

        public void ApplyPersonInfluences()
        {
            foreach (var person in AllPersons.Values)
            {
                person.ApplyTitles();
                person.ApplySkills();
                person.ApplyStunts();
                person.ApplyAllTreasures();
            }
        }

        private int oldDialogShowTime = -1;

        private void AIMergeAgainstPlayer()
        {
            if (this.PlayerFactions.Count == 0) return;
            if (this.Factions.Count < 3) return;
            if (!Session.GlobalVariables.PermitFactionMerge) return;
            if (Session.GlobalVariables.AIMergeAgainstPlayer < 0) return;

            Faction strongestAI = null;
            Faction strongestPlayer = null;
            int strongestAIPower = int.MinValue;
            int strongestPlayerPower = int.MinValue;

            foreach (Faction f in this.Factions)
            {
                if (this.IsPlayer(f))
                {
                    if (f.Power > strongestPlayerPower)
                    {
                        strongestPlayerPower = f.Power;
                        strongestPlayer = f;
                    }
                }
                else
                {
                    FactionList adjacent = f.GetAdjecentFactions();
                    bool nextToPlayer = false;
                    foreach (Faction g in adjacent)
                    {
                        if (this.IsPlayer(g) && this.GetDiplomaticRelation(f.ID, g.ID) < -100)
                        {
                            nextToPlayer = true;
                            break;
                        }
                    }

                    if (!nextToPlayer) continue;

                    if (f.Power > strongestAIPower)
                    {
                        strongestAIPower = f.Power;
                        strongestAI = f;
                    }
                }
            }

            if (strongestAI == null || strongestPlayer == null) return;


            if (GameObject.GetChance((int)(((float)strongestPlayerPower / strongestAIPower - Session.GlobalVariables.AIMergeAgainstPlayer) * 100)))
            {
                GameObjectList fl = this.Factions.GetList();
                fl.IsNumber = true;
                fl.PropertyName = "Power";
                fl.SmallToBig = false;
                fl.ReSort();

                Faction toMerge = null;
                foreach (Faction f in fl)
                {
                    if (this.IsPlayer(f) || f == strongestAI) continue;

                    if (!f.Leader.Hates(strongestAI.Leader))
                    {
                        if (GameObject.GetChance((int)(Person.GetIdealAttraction(strongestAI.Leader, f.Leader) + strongestPlayerPower / strongestAIPower * 100)))
                        {
                            if (strongestAI.adjacentTo(f) && this.GetDiplomaticRelation(strongestAI.ID, f.ID) > 0)
                            {
                                toMerge = f;
                                break;
                            }
                        }
                    }
                }

                if (toMerge != null)
                {
                    if (toMerge.Power > strongestAI.Power)
                    {
                        Faction temp = toMerge;
                        toMerge = strongestAI;
                        strongestAI = temp;
                    }
                    Session.MainGame.mainGameScreen.OnAIMergeAgainstPlayer(strongestPlayer, strongestAI, toMerge);
                    this.YearTable.addChangeFactionEntry(this.Date, toMerge, strongestAI);
                    GameObjectList rebelCandidates = toMerge.Persons.GetList();
                    toMerge.ChangeFaction(strongestAI);
                    toMerge.AfterChangeLeader(strongestAI, rebelCandidates, toMerge.Leader, strongestAI.Leader);
                }
            }

        }

        public void MonthPassedEvent()
        {
            ExtensionInterface.call("MonthEvent", new Object[] { this });

            this.AIMergeAgainstPlayer();

            foreach (Faction faction in this.Factions.GetRandomList())
            {
                faction.MonthEvent();
            }
            foreach (var person in AllPersons.Values)
            {
                person.TryToBeAvailable();
            }
            this.AddPreparedAvailablePersons();

            var randomPersons = StaticMethods.GetRandomList(AvailablePersons.Values.ToList());
            foreach (var person in randomPersons)
            {
                person.MonthEvent();
            }

            foreach (var architecture in StaticMethods.GetRandomList(Architectures.Values.ToList()))
            {
                architecture.MonthEvent();
            }

            foreach (var militaryKind in GameCommonData.AllMilitaryKinds.Values)
            {
                var flag = true;
                foreach (Troop troop in Troops)
                {
                    if ((troop.Army.Kind == militaryKind) && Session.MainGame.mainGameScreen.TileInScreen(troop.Position))
                    {
                        flag = false;
                        break;
                    }
                }
                //if (flag)
                //{
                //    kind.Textures.Dispose();
                //}
            }
        }

        private void AdjustGlobalPersonRelation()
        {
            var dayInTurn = Session.Parameters.DayInTurn;
            foreach (var person in AllPersons.Values)
            {
                if (person.Available && person.Alive && GameObject.Random(120 / dayInTurn) == 0)
                {
                    foreach (var otherPerson in AllPersons.Values)
                    {
                        if (person == otherPerson) continue;

                        if (!otherPerson.Alive)
                        {
                            person.SetRelation(otherPerson, 0);
                            otherPerson.SetRelation(person, 0);
                            continue;
                        }

                        if (otherPerson.Available 
                            && otherPerson.Alive 
                            && person.BelongedFactionWithPrincess != null 
                            && GameObject.Random(30 / dayInTurn) == 0)
                        {
                            float likeability = Person.GetIdealAttraction(person, otherPerson) * 8 + otherPerson.Glamour * 0.75f + person.Glamour * 0.25f + otherPerson.PersonalLoyalty * 7.5f + person.PersonalLoyalty * 2.5f - (otherPerson.Ambition + person.Ambition) * 5 - 100;
                            bool sameLocation = person.SameLocationAs(otherPerson);

                            bool sameWork = sameLocation
                                            && ((person.Status == PersonStatus.Normal 
                                                 && otherPerson.Status == PersonStatus.Normal 
                                                 && (person.WorkKind == otherPerson.WorkKind || person.OutsideTask == otherPerson.OutsideTask)) 
                                            || (person.Status == PersonStatus.Princess && otherPerson.Status == PersonStatus.Princess));
                            float factor = 0.0f;
                            
                            if (person.LocationTroop == otherPerson.LocationTroop && person.LocationTroop != null && otherPerson.LocationTroop != null)
                            {
                                factor = 3.0f;
                            }
                            else if (sameLocation && person.Hates(otherPerson) && person.Spouse == otherPerson && GameObject.GetChance(50))
                            {
                                factor = 3.0f;
                            }
                            else if (sameWork)
                            {
                                factor = 1.0f;
                            } 
                            else if (sameLocation && GameObject.GetChance(50))
                            {
                                factor = 1.0f;
                            }
                            else if (person.BelongedFactionWithPrincess == otherPerson.BelongedFactionWithPrincess && GameObject.GetChance(20))
                            {
                                factor = 1.0f;
                            }

                            if (factor > 0)
                            {
                                int chance = (int)(likeability / 4);
                                float relationFactor = 6 * factor;
                                float adjust = 2 * factor;

                                if (GameObject.GetChance(chance))
                                {
                                    person.AdjustRelation(otherPerson, relationFactor, adjust);
                                    otherPerson.AdjustRelation(person, relationFactor, adjust);
                                }
                                else if (GameObject.GetChance(-chance))
                                {
                                    person.AdjustRelation(otherPerson, -relationFactor, -adjust);
                                    otherPerson.AdjustRelation(person, -relationFactor, -adjust);
                                }
                            }
                        }

                        var relation = person.GetRelation(otherPerson);
                        if (relation > 0)
                        {
                            var chance = (5 - person.PersonalLoyalty) * 20 - 10;
                            if (!person.Closes(otherPerson) && GameObject.GetChance(chance))
                            {
                                float d = (float)Session.Parameters.CloseThreshold / Math.Max(10, relation);
                                if (person.LocationArchitecture == otherPerson.LocationArchitecture || person.LocationTroop == otherPerson.LocationTroop)
                                {
                                    person.AdjustRelation(otherPerson, -d / 5f, 0);
                                }
                                else
                                {
                                    person.AdjustRelation(otherPerson, -d / 12.5f, 0);
                                }

                                if (person.GetRelation(otherPerson) < 0)
                                {
                                    person.SetRelation(otherPerson, 0);
                                }
                            }
                        }
                        else if (relation < 0)
                        {
                            if (person.Hates(otherPerson)) continue;

                            float d = Session.Parameters.HateThreshold / -relation / 5f;
                            if (person.Status == PersonStatus.Princess && otherPerson.Status == PersonStatus.Princess)
                            {
                                d *= 4;
                            }
                            if (person.LocationArchitecture == otherPerson.LocationArchitecture || person.LocationTroop == otherPerson.LocationTroop)
                            {
                                person.AdjustRelation(otherPerson, -d / 5f, 0);
                            }
                            else
                            {
                                person.AdjustRelation(otherPerson, -d / 12.5f, 0);
                            }

                            if (person.GetRelation(otherPerson) > 0)
                            {
                                person.SetRelation(otherPerson, 0);
                            }
                        }
                    }
                }
            }
        }

        public void MonthStartingEvent()
        {
        }

        public void SeasonChangeEvent()
        {
            if (!scenarioJustLoaded)
            {
                ExtensionInterface.call("SeasonEvent", new Object[] { this });
                if ((this.Date.Month == 3 || this.Date.Month == 6 || this.Date.Month == 9 || this.Date.Month == 12) && this.Date.Day <= Session.Current.Scenario.Parameters.DayInTurn)
                {
                    foreach (Faction faction in this.Factions.GetRandomList())
                    {
                        faction.SeasonEvent();
                    }
                    foreach (var architecture in StaticMethods.GetRandomList(Architectures.Values.ToList()))
                    {
                        architecture.DevelopSeason();
                    }
                }
            }
        }

        public bool MoreThanOneTroopOnPosition(Point position)
        {
            return (this.MapTileData[position.X, position.Y].TroopCount > 1);
        }

        public void NewFaction()
        {
            if (GameObject.Random(15) == 0)
            {
                NewFaction(AvailablePersons.Values.ToList());
            }
        }

        public void NewFaction(List<Person> candidates, bool leaderChange = false, bool nonInherited = false)
        {
            if (!Session.GlobalVariables.WujiangYoukenengDuli) return;

            var list = new List<Person>();
            foreach (var person in candidates)
            {
                if (person.YoukenengChuangjianXinShili())   //里面包含武将有可能独立的参数
                {
                    var ambition = 5 - person.Ambition;
                    var ambitionChance = ambition * ambition * ambition;
                    var faction = person.BelongedFaction;

                    if ((person.Ambition > 1 && GameObject.Random(ambitionChance) == 0) 
                        || (faction != null && person.Hates(faction.Leader)))
                    {
                        list.Add(person);
                    }
                }
            }

            if (list.Count == 0) return;

            var p = StaticMethods.GetRandomItem(list);
            int cnt = 0;
            foreach (var person8 in list)
            {
                cnt++;
                if (!leaderChange && cnt > 1)
                {
                    break;
                }

                if (leaderChange)
                {
                    p = person8;
                }

                var location = p.BelongedArchitecture;
                var faction = p.BelongedFaction;
                if (location == null) continue;
                if (faction != null && !p.Hates(faction.Leader))
                {
                    if (p.Loyalty >= 100) continue;
                    if (p.Loyalty >= 90 && !p.LeaderPossibility) continue;
                }
                if (faction != null && Person.GetIdealOffset(faction.Leader, p) <= 10 && !p.Hates(faction.Leader)) continue;
                if (faction != null && location == faction.Capital) continue;
                //if (GameObject.Random(15) != 0) return;

                if (GameObject.Random(location.Population + location.ArmyScale * 5000 +
                        location.Domination * 200 + location.Morale * 10) >
                    GameObject.Random(p.Reputation *
                    (p.LeaderPossibility ? 3 : 1) *
                    (leaderChange && nonInherited ? p.Ambition * p.Ambition * (p.Glamour / 20) : 1) *
                    (faction != null && leaderChange && nonInherited ? Person.GetIdealOffset(p, faction.Leader) / 10 + 1 : 1) *
                    (faction != null && (p.Hates(faction.Leader) || faction.Leader.Hates(p)) ? (leaderChange ? 10000 : 3) : 1) *
                    (faction == null ? 3 : 1))) continue;
                this.CreateNewFaction(p);
            }
        }

        private void NoFoodPositionDayEvent()
        {
            List<NoFoodPosition> list = new List<NoFoodPosition>();
            foreach (NoFoodPosition position in this.NoFoodDictionary.Positions.Values)
            {
                position.Days--;
                if (position.Days <= 0)
                {
                    list.Add(position);
                }
            }
            foreach (NoFoodPosition position in list)
            {
                this.NoFoodDictionary.RemovePosition(position);
            }
        }

        public bool PositionIsArchitecture(Point position)
        {
            return (this.GetArchitectureByPosition(position) != null);
        }

        public bool PositionIsOnFire(Point position)
        {
            if (this.PositionOutOfRange(position))
            {
                return false;
            }
            return this.FireTable.HasPosition(position);
        }

        public bool PositionIsOnFireNoCheck(Point position)
        {
            return this.FireTable.HasPosition(position);
        }

        public bool PositionIsTroop(Point position)
        {
            return (this.GetTroopByPosition(position) != null);
        }

        public bool PositionOutOfRange(Point position)
        {
            return ScenarioMap.PositionOutOfRange(position);
        }

        public string PositionString(Point position)
        {

            if (this.PositionIsArchitecture(position))
            {
                return this.GetArchitectureByPositionNoCheck(position).Name;
            }
            /*
            if (this.PositionIsTroop(position))
            {
                return this.GetTroopByPositionNoCheck(position).DisplayName;
            }
            */
            return (this.GetTerrainNameByPosition(position) + " " + this.GetCoordinateString(position));
        }

        public void ReflectDiplomaticRelations(int src, int des, int offset)
        {
            foreach (DiplomaticRelation relation in this.DiplomaticRelations.GetDiplomaticRelationListByFactionID(des))
            {
                int theOtherFactionID = relation.GetTheOtherFactionID(des);
                if ((theOtherFactionID != src) && (Math.Abs(relation.Relation) >= 100))
                {
                    int num2 = this.DiplomaticRelations.GetDiplomaticRelation(src, theOtherFactionID).Relation;
                    if ((num2 > -GlobalVariables.FriendlyDiplomacyThreshold) && (num2 < Session.GlobalVariables.FriendlyDiplomacyThreshold))
                    {
                        int num3 = relation.Relation;
                        if (num3 > 0x3e8)
                        {
                            num3 = 0x3e8;
                        }
                        else if (num3 < -0x3e8)
                        {
                            num3 = -0x3e8;
                        }
                        this.ChangeDiplomaticRelation(src, theOtherFactionID, (offset * num3) / 0x3e8);
                    }
                }
            }
        }

        public void RemovePositionAreaInfluence(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                Troop troopByPositionNoCheck = this.GetTroopByPositionNoCheck(position);
                this.MapTileData[position.X, position.Y].RemoveAreaInfluence(troop, troopByPositionNoCheck);
                if (troopByPositionNoCheck != null)
                {
                    troopByPositionNoCheck.RefreshDataOfAreaInfluence();
                }
            }
        }

        public void RemovePositionContactingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].RemoveContactingTroop(troop);
            }
        }

        public void RemovePositionOffencingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].RemoveOffencingTroop(troop);
            }
        }

        public void RemovePositionStratagemingTroop(Troop troop, Point position)
        {
            if (!this.PositionOutOfRange(position))
            {
                this.MapTileData[position.X, position.Y].RemoveStratagemingTroop(troop);
            }
        }

        public void RemovePositionViewingTroopNoCheck(Troop troop, Point position)
        {
            this.MapTileData[position.X, position.Y].RemoveViewingTroop(troop);
        }

        public void RemoveRouteway(Routeway routeway)
        {
            if (routeway.FirstPoint != null)
            {
                routeway.CutAt(routeway.FirstPoint.Position);
            }
            if (routeway.StartArchitecture != null)
            {
                routeway.StartArchitecture.Routeways.Remove(routeway);
            }
            if (routeway.BelongedFaction != null)
            {
                routeway.BelongedFaction.RemoveRouteway(routeway);
            }
            this.Routeways.Remove(routeway);
        }

        public void ResetMapTileTroop(Point position)
        {
            if (this.MapTileData[position.X, position.Y].TileTroop != null && this.MapTileData[position.X, position.Y].TileTroop.Destroyed)
            {
                TileData data1 = this.MapTileData[position.X, position.Y];
                data1.TroopCount--;
                this.MapTileData[position.X, position.Y].TileTroop = null;
            }
        }

        public void ReallyResetMapTileTroop()
        {
            for (int i = 0; i < this.MapTileData.GetLength(0); ++i)
            {
                for (int j = 0; j < this.MapTileData.GetLength(1); ++j)
                {
                    TileData t = this.MapTileData[i, j];
                    if (t.ContactingTroops != null)
                    {
                        t.ContactingTroops.RemoveAll(u => u == null || u.Destroyed || u.Simulating);
                        if (t.ContactingTroops.Count == 0)
                        {
                            // Yes I mean it. Too many empty lists kill the memory.......
                            this.MapTileData[i, j].ContactingTroops = null;
                        }
                        else
                        {
                            t.ContactingTroops.Capacity = t.ContactingTroops.Count;
                        }
                    }
                    if (t.OffencingTroops != null)
                    {
                        t.OffencingTroops.RemoveAll(u => u == null || u.Destroyed || u.Simulating);
                        if (t.OffencingTroops.Count == 0)
                        {
                            this.MapTileData[i, j].OffencingTroops = null;
                        }
                        else
                        {
                            t.OffencingTroops.Capacity = t.OffencingTroops.Count;
                        }
                    }
                    if (t.StratagemingTroops != null)
                    {
                        t.StratagemingTroops.RemoveAll(u => u == null || u.Destroyed || u.Simulating);
                        if (t.StratagemingTroops.Count == 0)
                        {
                            this.MapTileData[i, j].StratagemingTroops = null;
                        }
                        else
                        {
                            t.StratagemingTroops.Capacity = t.StratagemingTroops.Count;
                        }
                    }
                    if (t.ViewingTroops != null)
                    {
                        t.ViewingTroops.RemoveAll(u => u == null || u.Destroyed || u.Simulating);
                        if (t.ViewingTroops.Count == 0)
                        {
                            this.MapTileData[i, j].ViewingTroops = null;
                        }
                        else
                        {
                            t.ViewingTroops.Capacity = t.ViewingTroops.Count;
                        }
                    }

                    if (t.AreaInfluenceList != null)
                    {
                        t.AreaInfluenceList.RemoveAll(u => u == null || u.Owner.Destroyed || u.Owner.Simulating);
                        if (t.AreaInfluenceList.Count == 0)
                        {
                            this.MapTileData[i, j].AreaInfluenceList = null;
                        }
                        else
                        {
                            t.AreaInfluenceList.Capacity = t.AreaInfluenceList.Count;
                        }
                    }

                    if (t.TileRouteways != null)
                    {
                        if (t.TileRouteways.Count == 0)
                        {
                            this.MapTileData[i, j].TileRouteways = null;
                        }
                        else
                        {
                            t.TileRouteways.Capacity = t.TileRouteways.Count;
                        }
                    }

                    if (t.SupplyingRoutePoints != null)
                    {
                        if (t.SupplyingRoutePoints.Count == 0)
                        {
                            this.MapTileData[i, j].SupplyingRoutePoints = null;
                        }
                        else
                        {
                            t.SupplyingRoutePoints.Capacity = t.SupplyingRoutePoints.Count;
                        }
                    }

                    if (t.SupplyingRoutePoints != null)
                    {
                        if (t.SupplyingRoutePoints.Count == 0)
                        {
                            this.MapTileData[i, j].SupplyingRoutePoints = null;
                        }
                        else
                        {
                            t.SupplyingRoutePoints.Capacity = t.SupplyingRoutePoints.Count;
                        }
                    }

                    if (t.TileTroop != null && (t.TileTroop.Destroyed || t.TileTroop.Simulating))
                    {
                        this.MapTileData[i, j].TileTroop = null;
                    }
                }
            }
        }

        public bool SaveGameScenario(string LoadedFileName, bool saveMap, bool saveCommonData, bool saveSettings, bool disposeMemory = true, bool fullPathProvided = false, bool editing = false)
        {
            if (this.GameTime < 0)
            {
                this.GameTime = 0;
            }
            if(!editing)
            {
                this.GameTime += (int)DateTime.Now.Subtract(sessionStartTime).TotalSeconds;
            }
            sessionStartTime = DateTime.Now;

            List<string> errors = new List<string>();

            ClearPersonStatusCache();
            ClearPersonWorkCache();

            var dirPath = @"Content\Save";
            var facilityStore = new JsonStore<FacilityConfig>(Path.Combine(dirPath, "Facilities.json"));
            var facilities = Facilities.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            facilityStore.Save(facilities);

            var informationStore = new JsonStore<InformationConfig>(Path.Combine(dirPath, "Informations.json"));
            var informations = Informations.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            informationStore.Save(informations);

            var architectureStore = new JsonStore<ArchitectureConfig>(Path.Combine(dirPath, "Architectures.json"));
            var architectures = Architectures.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            architectureStore.Save(architectures);

            var personStore = new JsonStore<PersonConfig>(Path.Combine(dirPath, "Persons.json"));
            var persons = AllPersons.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            personStore.Save(persons);

            var stateStore = new JsonStore<StateConfig>(Path.Combine(dirPath, "States.json"));
            var states = States.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            stateStore.Save(states);

            var regionStore = new JsonStore<RegionConfig>(Path.Combine(dirPath, "Regions.json"));
            var regions = Regions.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            regionStore.Save(regions);

            var sectionStore = new JsonStore<SectionConfig>(Path.Combine(dirPath, "Sections.json"));
            var sections = Sections.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            sectionStore.Save(sections);

            var militaryStore = new JsonStore<MilitaryConfig>(Path.Combine(dirPath, "Militaries.json"));
            var militaries = Militaries.Values.Select(x => x.ToConfig()).OrderBy(x => x.Id).ToList();
            militaryStore.Save(militaries);

            this.AllBiographies.Biographys = this.AllBiographies.Biographys.OrderBy(x => x.Value.ID).ToDictionary(x => x.Key, y => y.Value);
            this.AllEvents.GameObjects = this.AllEvents.GameObjects.OrderBy(x => x.ID).ToList();
            this.Factions.GameObjects = this.Factions.GameObjects.OrderBy(x => x.ID).ToList();
            this.Legions.GameObjects = this.Legions.GameObjects.OrderBy(x => x.ID).ToList();
            this.Routeways.GameObjects = this.Routeways.GameObjects.OrderBy(x => x.ID).ToList();
            this.Treasures.GameObjects = this.Treasures.GameObjects.OrderBy(x => x.ID).ToList();
            this.Troops.GameObjects = this.Troops.GameObjects.OrderBy(x => x.ID).ToList();
            this.TroopEvents.GameObjects = this.TroopEvents.GameObjects.OrderBy(x => x.ID).ToList();
            this.DiplomaticRelations.DiplomaticRelations = this.DiplomaticRelations.DiplomaticRelations.OrderBy(x => x.Value.RelationFaction1ID).ToDictionary(x => x.Key, y => y.Value);
            if(editing)
            {
                this.FatherIds = this.FatherIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.MotherIds = this.MotherIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.SpouseIds = this.SpouseIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.BrotherIds = this.BrotherIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.SuoshuIds = this.SuoshuIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.CloseIds = this.CloseIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.HatedIds = this.HatedIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                this.PersonRelationIds = this.PersonRelationIds.OrderBy(x => x.PersonID1).ToList();
            }

            if (!disposeMemory)
            {
                this.DisposeLotsOfMemory();
            }

            if (!editing)
            {
                foreach (Faction faction in this.Factions)
                {
                    faction.SectionsString = StaticMethods.SaveIdToString(faction.Sections);
                    faction.ArchitecturesString = StaticMethods.SaveIdToString(faction.Architectures);
                    faction.TroopListString = faction.Troops.SaveToString();
                    faction.InformationsString = StaticMethods.SaveIdToString(faction.Informations);
                    faction.RoutewaysString = faction.Routeways.SaveToString();
                    faction.LegionsString = faction.Legions.SaveToString();
                    faction.BaseMilitaryKindsString = StaticMethods.SaveIdToString(faction.GetMilitaryKinds());
                    faction.AvailableTechniquesString = StaticMethods.SaveIdToString(faction.AvailableTechniques.Values);
                    faction.PlanTechniqueString = faction.PlanTechnique?.ID ?? -1;
                    faction.GetGeneratorPersonCountString = faction.SaveGeneratorPersonCountToString();
                    faction.TransferingMilitariesString = StaticMethods.SaveIdToString(faction.TransferingMilitaries);
                    faction.MilitariesString = StaticMethods.SaveIdToString(faction.Militaries);
                    faction.PrinceID = faction.Prince != null ? faction.Prince.ID : -1;
                }
            }

            foreach (var section in Sections.Values)
            {
                section.EnsureSectionArchitecture();
                if (!editing)
                {
                    section.AIDetailIDString = section.AIDetail.ID;
                    section.OrientationFactionID = section.OrientationFaction?.ID ?? -1;
                    section.OrientationSectionID = section.OrientationSection?.ID ?? -1;
                    section.OrientationStateID = section.OrientationState?.ID ?? -1;
                    section.OrientationArchitectureID = section.OrientationArchitecture?.ID ?? -1;
                    section.ArchitecturesString = StaticMethods.SaveIdToString(section.Architectures);
                }
            }

            if (!editing)
            {
                foreach (var architecture in Architectures.Values)
                {
                    architecture.KindId = architecture.Kind.ID;
                    architecture.StateID = architecture.LocationState.ID;
                    architecture.CharacteristicsString = StaticMethods.SaveIdToString(architecture.Characteristics.Values);

                    architecture.ArchitectureAreaString = StaticMethods.SaveToString(architecture.ArchitectureArea.Area);

                    architecture.PersonsString = architecture.Persons.SaveToString();
                    architecture.MovingPersonsString = architecture.MovingPersons.SaveToString();
                    architecture.NoFactionPersonsString = architecture.NoFactionPersons.SaveToString();
                    architecture.NoFactionMovingPersonsString = architecture.NoFactionMovingPersons.SaveToString();

                    //row["AgricultureWorkingPersons"] = architecture.AgricultureWorkingPersons.SaveToString();
                    //row["CommerceWorkingPersons"] = architecture.CommerceWorkingPersons.SaveToString();
                    //row["TechnologyWorkingPersons"] = architecture.TechnologyWorkingPersons.SaveToString();
                    //row["DominationWorkingPersons"] = architecture.DominationWorkingPersons.SaveToString();
                    //row["MoraleWorkingPersons"] = architecture.MoraleWorkingPersons.SaveToString();
                    //row["EnduranceWorkingPersons"] = architecture.EnduranceWorkingPersons.SaveToString();
                    //row["zhenzaiWorkingPersons"] = architecture.ZhenzaiWorkingPersons.SaveToString();
                    //row["TrainingWorkingPersons"] = architecture.TrainingWorkingPersons.SaveToString();

                    architecture.feiziliebiaoString = architecture.Feiziliebiao.SaveToString();
                    architecture.MilitariesString = StaticMethods.SaveIdToString(architecture.Militaries);
                    architecture.FacilitiesString = StaticMethods.SaveIdToString(architecture.Facilities);

                    architecture.PlanFacilityKindID = architecture.PlanFacilityKind?.ID ?? -1;

                    architecture.FundPacksString = architecture.SaveFundPacksToString();
                    architecture.FoodPacksString = architecture.SaveFoodPacksToString();
                    architecture.PopulationPacksString = architecture.SavePopulationPacksToString();

                    architecture.PlanArchitectureID = (architecture.PlanArchitecture != null) ? architecture.PlanArchitecture.ID : -1;

                    architecture.TransferFundArchitectureID = (architecture.TransferFundArchitecture != null) ? architecture.TransferFundArchitecture.ID : -1;

                    architecture.TransferFoodArchitectureID = (architecture.TransferFoodArchitecture != null) ? architecture.TransferFoodArchitecture.ID : -1;

                    architecture.DefensiveLegionID = (architecture.DefensiveLegion != null) ? architecture.DefensiveLegion.ID : -1;

                    architecture.CaptivesString = architecture.Captives.SaveToString();

                    architecture.RobberTroopID = (architecture.RobberTroop != null) ? architecture.RobberTroop.ID : -1;

                    architecture.AILandLinksString = StaticMethods.SaveIdToString(architecture.AILandLinks);

                    architecture.AIWaterLinksString = StaticMethods.SaveIdToString(architecture.AIWaterLinks);

                    //row["zainanleixing"] = architecture.zainan.zainanzhonglei.ID;
                    //row["zainanshengyutianshu"] = architecture.zainan.shengyutianshu;

                    architecture.InformationsString = StaticMethods.SaveIdToString(architecture.Informations);

                    //string s = "";
                    //foreach (Architecture i in architecture.AIBattlingArchitectures)
                    //{
                    //    s += i.ID + " ";
                    //}
                    //row["AIBattlingArchitectures"] = s;
                }
            }

            foreach (Legion legion in this.Legions)
            {
                legion.StartArchitectureString = (legion.StartArchitecture != null) ? legion.StartArchitecture.ID : -1;
                legion.WillArchitectureString = (legion.WillArchitecture != null) ? legion.WillArchitecture.ID : -1;

                legion.PreferredRoutewayString = (legion.PreferredRouteway != null) ? legion.PreferredRouteway.ID : -1;

                legion.CoreTroopString = (legion.CoreTroop != null) ? legion.CoreTroop.ID : -1;

                legion.TroopsString = legion.Troops.SaveToString();
            }

            foreach (Troop troop in this.Troops)
            {
                troop.LeaderIDString = troop.Leader.ID;

                troop.MilitaryID = troop.Army.ID;

                troop.StartingArchitectureString = (troop.StartingArchitecture != null) ? troop.StartingArchitecture.ID : -1;
                troop.PersonsString = troop.SavePersonsToString();

                //row["PositionX"] = troop.Position.X;
                //row["PositionY"] = troop.Position.Y;
                //row["RealDestinationX"] = troop.RealDestination.X;
                //row["RealDestinationY"] = troop.RealDestination.Y;

                troop.WillTroopID = troop.RealWillTroop == null ? -1 : troop.RealWillTroop.ID;
                troop.WillArchitectureID = troop.RealWillArchitecture == null ? -1 : troop.RealWillArchitecture.ID;

                if (!editing) troop.CaptivesString = StaticMethods.SaveIdToString(troop.Captives);  //0413剧本编辑器部队可以存储俘虏  

                troop.EventInfluencesString = StaticMethods.SaveIdToString(troop.EventInfluences);

                troop.CombatMethodsString = StaticMethods.SaveIdToString(troop.CombatMethods.Values);

                troop.CurrentStuntIDString = (troop.CurrentStunt != null) ? troop.CurrentStunt.ID : -1;
                
            }

            if (saveMap)
            {
                foreach (TroopEvent event2 in this.TroopEvents)
                {
                    event2.AfterEventHappened = (event2.AfterHappenedEvent != null) ? event2.AfterHappenedEvent.ID : -1;
                    event2.LaunchPersonString = (event2.LaunchPerson != null) ? event2.LaunchPerson.ID : -1;
                    event2.ConditionsString = StaticMethods.SaveIdToString(event2.Conditions);
                    event2.TargetPersonsString = event2.SaveTargetPersonToString();
                    event2.SelfEffectsString = event2.SaveSelfEffectToString();
                    event2.EffectPersonsString = event2.SaveEffectPersonToString();
                    event2.EffectAreasString = event2.SaveEffectAreaToString();
                    event2.dialogString = event2.SaveDialogToString();
                }
            }

            foreach (Routeway routeway in this.Routeways)
            {
                if ((routeway.StartArchitecture != null) && ((routeway.Building || (routeway.LastActivePointIndex >= 0)) || (routeway.StartArchitecture.BelongedSection == null || (!routeway.StartArchitecture.BelongedSection.AIDetail.AutoRun && this.IsPlayer(routeway.StartArchitecture.BelongedFaction)))))
                {
                    routeway.StartArchitectureString = (routeway.StartArchitecture != null) ? routeway.StartArchitecture.ID : -1;
                    routeway.EndArchitectureString = (routeway.EndArchitecture != null) ? routeway.EndArchitecture.ID : -1;
                    routeway.DestinationArchitectureString = (routeway.DestinationArchitecture != null) ? routeway.DestinationArchitecture.ID : -1;
                }
            }

            foreach (var military in Militaries.Values)
            {
                military.FollowedLeaderID = military.FollowedLeader?.ID ?? -1;
                military.LeaderID = military.Leader?.ID ?? -1;

                //row["LeaderExperience"] = military.LeaderExperience;

                //row["TrainingPersonID"] = -1;

                military.RecruitmentPersonID = military.RecruitmentPerson?.ID ?? -1;
                military.ShelledMilitaryID = military.ShelledMilitary?.ID ?? -1;
            }

            var captives = GetCaptives();
            foreach (var captive in captives)
            {
                captive.CaptivePersonID = captive.CaptivePerson?.ID ?? -1;
                captive.CaptiveFactionID = captive.CaptiveFaction?.ID ?? -1;
                captive.RansomArchitectureID = captive.RansomArchitecture?.ID ?? -1; 
            }

            if (!editing)
            {
                ClearTempDic();
            }

            if (!editing)
            {
                foreach (var person in AllPersons.Values)
                {
                    person.UniqueTitlesString = StaticMethods.SaveIdToString(person.UniqueTitles);
                    // person.UniqueMilitaryKindsString = person.UniqueMilitaryKinds.SaveToString();
                    person.IdealTendencyIDString = (person.IdealTendency != null) ? person.IdealTendency.ID : -1;
                    if (person.Character != null)
                    {
                        person.PCharacter = person.Character.ID;
                    }
                    person.UniqueTitlesString = StaticMethods.SaveIdToString(person.UniqueTitles);
                    person.UniqueMilitaryKindsString = StaticMethods.SaveIdToString(person.UniqueMilitaryKinds);

                    //row["Braveness"] = person.BaseBraveness;                    
                    //row["Calmness"] = person.BaseCalmness;
                    //row["Loyalty"] = person.Loyalty;

                    FatherIds[person.ID] = person.Father == null ? -1 : person.Father.ID;
                    MotherIds[person.ID] = person.Mother == null ? -1 : person.Mother.ID;
                    SpouseIds[person.ID] = person.Spouse == null ? -1 : person.Spouse.ID;

                    String brotherStr = "";
                    foreach (Person p in person.Brothers)
                    {
                        brotherStr += p.ID + " ";
                    }

                    String str;
                    char[] separator = separator = new char[] { ' ', '\n', '\r', '\t' };
                    String[] strArray;
                    int[] intArray;
                    try
                    {
                        str = brotherStr;
                        strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        intArray = new int[strArray.Length];
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            intArray[i] = int.Parse(strArray[i]);
                        }
                        BrotherIds.Add(person.ID, intArray);
                    }
                    catch
                    {
                        errors.Add("义兄弟一栏应为半型空格分隔的人物ID");
                    }

                    String suoshuStr = "";
                    foreach (Person p in person.suoshurenwuList)
                    {
                        suoshuStr += p.ID + " ";
                    }

                    if (suoshuStr != null)
                    {
                        try
                        {
                            strArray = suoshuStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            intArray = new int[strArray.Length];
                            for (int i = 0; i < strArray.Length; i++)
                            {
                                intArray[i] = int.Parse(strArray[i]);
                            }
                            SuoshuIds.Add(person.ID, intArray);
                        }
                        catch
                        {
                            errors.Add("所属人物表一栏应为半型空格分隔的人物ID");
                        }
                    }

                    String closeStr = "";
                    String hatedStr = "";
                    foreach (Person p in person.GetClosePersons())
                    {
                        closeStr += p.ID + " ";
                    }
                    foreach (Person p in person.GetHatedPersons())
                    {
                        hatedStr += p.ID + " ";
                    }

                    try
                    {
                        str = closeStr;
                        strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        intArray = new int[strArray.Length];
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            intArray[i] = int.Parse(strArray[i]);
                        }
                        CloseIds.Add(person.ID, intArray);
                    }
                    catch
                    {
                        errors.Add("亲爱武将一栏应为半型空格分隔的人物ID");
                    }

                    try
                    {
                        str = hatedStr;
                        strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        intArray = new int[strArray.Length];
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            intArray[i] = int.Parse(strArray[i]);
                        }
                        HatedIds.Add(person.ID, intArray);
                    }
                    catch
                    {
                        errors.Add("厌恶武将一栏应为半型空格分隔的人物ID");
                    }

                    MarriageGranterId.Add(person.ID, person.marriageGranter != null ? person.marriageGranter.ID : -1);

                    //row["TrainingMilitaryID"] = -1;
                    //row["RecruitmentMilitaryID"] = person.RecruitmentMilitary == null ? -1 : person.RecruitmentMilitary.ID;

                    person.ConvincingPersonID = (person.ConvincingPerson != null) ? person.ConvincingPerson.ID : -1;

                    person.SkillsString = StaticMethods.SaveIdToString(person.Skills.Values);
                    person.RealTitlesString = StaticMethods.SaveIdToString(person.RealTitles);
                    person.StudyingTitleString = (person.StudyingTitle != null) ? person.StudyingTitle.ID : -1;

                    person.StuntsString = StaticMethods.SaveIdToString(person.Stunts.Values);
                    person.StudyingStuntString = (person.StudyingStunt != null) ? person.StudyingStunt.ID : -1;

                    person.waitForFeiziId = (person.WaitForFeiZi != null) ? person.WaitForFeiZi.ID : -1;
                    person.preferredTroopPersonsString = StaticMethods.SaveIdToString(person.PreferredTroopPersons);

                    person.TrainPolicyIDString = person.TrainPolicy == null ? -1 : person.TrainPolicy.ID;

                    foreach (KeyValuePair<Person, int> pi in person.GetRelations())
                    {
                        var personIDRelation = new PersonIDRelation()
                        {
                            PersonID1 = person.ID,
                            PersonID2 = pi.Key.ID,
                            Relation = pi.Value
                        };
                        PersonRelationIds.Add(personIDRelation);
                    }
                }
            }
            if(!editing)
            {
                var captiveList = new CaptiveList();
                foreach (var captive in GetCaptives())
                {
                    captiveList.Add(captive);
                }
                captiveData = captiveList;
            }

            if (saveMap)
            {
                this.ScenarioMap.MapDataString = ScenarioMap.SaveToString();//修复游戏中编辑地形后无法保存
                foreach (var region in Regions.Values)
                {
                    region.StatesListString = StaticMethods.SaveIdToString(region.States);
                    region.RegionCoreID = region.RegionCore?.ID ?? -1;
                }

                foreach (var state in States.Values)
                {
                    state.ContactStatesString = StaticMethods.SaveIdToString(state.ContactStates);
                    state.StateAdminID = state.StateAdmin?.ID ?? -1;
                }
            }

            foreach (Treasure treasure in this.Treasures)
            {
                treasure.BelongedPersonIDString = (treasure.BelongedPerson != null) ? treasure.BelongedPerson.ID : -1;
                treasure.HidePlaceIDString = (treasure.HidePlace != null) ? treasure.HidePlace.ID : -1;
                treasure.InfluencesString = StaticMethods.SaveIdToString(treasure.Influences.Values);
                treasure.Available = (treasure.BelongedPerson != null) ? true : false;
                if (treasure.Available)
                {
                    if (!treasure.BelongedPerson.Alive || (treasure.BelongedPerson.ID >= 7000 && treasure.BelongedPerson.ID < 8000))
                    { treasure.Available = false; }
                }
            
            }

            foreach (YearTableEntry yt in this.YearTable)
            {
                string factionStr = "";
                foreach (Faction f in yt.Factions)
                {
                    if (f != null)
                    {
                        factionStr += f.ID + " ";
                    }
                }
                yt.FactionsString = factionStr;
            }

            if (saveMap && !editing)
            {
                foreach (Event e in this.AllEvents)
                {
                    e.personString = StaticMethods.SaveKeyToString(e.person);
                    e.PersonCondString = StaticMethods.SaveKeyToString(e.personCond);
                    e.architectureString = StaticMethods.SaveIdToString(e.Architectures);
                    e.architectureCondString = StaticMethods.SaveIdToString(e.architectureCond);
                    e.factionString = e.faction.SaveToString();
                    e.factionCondString = StaticMethods.SaveIdToString(e.factionCond);
                    e.dialogString = e.SaveDialogToString();
                    e.effectString = StaticMethods.SaveKeyToString(e.effect);
                    e.architectureEffectString = StaticMethods.SaveIdToString(e.architectureEffect);
                    e.factionEffectIDString = StaticMethods.SaveIdToString(e.factionEffect);
                    e.yesdialogString = e.SaveyesDialogToString();
                    e.nodialogString = e.SavenoDialogToString();
                    e.yesEffectString = StaticMethods.SaveKeyToString(e.yesEffect);
                    e.noEffectString = StaticMethods.SaveKeyToString(e.noEffect);
                    e.yesArchitectureEffectString = StaticMethods.SaveIdToString(e.yesArchitectureEffect);
                    e.noArchitectureEffectString = StaticMethods.SaveIdToString(e.noArchitectureEffect);
                    e.scenBiographyString = e.SaveScenBiographyToString();
                }
            }

            this.CurrentPlayerID = ((this.CurrentPlayer != null) ? this.CurrentPlayer.ID : -1).ToString();
            if(!editing)
            {
                this.PlayerList = this.PlayerFactions.GameObjects.Select(ob => ob.ID).NullToEmptyList();
                this.PlayerInfo = this.GetPlayerInfo();
            }
            this.Factions.FactionQueue = this.Factions.SaveQueueToString();


            //row["JumpPosition"] = StaticMethods.SaveToString(new Point?(ScenarioMap.JumpPosition));

            if (this.OnAfterSaveScenario != null)
            {
                this.OnAfterSaveScenario();
            }

            foreach (Biography biography in AllBiographies.Biographys.Values)
            {
                biography.MilitaryKindsString = StaticMethods.SaveIdToString(biography.MilitaryKinds);
            }

            var scenarioClone = this.Clone();            

            if (!saveCommonData && !UsingOwnCommonData)
            {
                scenarioClone.GameCommonData = null;
            }
            


            if (saveSettings)
            {

            }
            else
            {
                //scenarioClone.Parameters = null;
                //scenarioClone.GlobalVariables = null;
            }

            var saves = LoadScenarioSaves();
            string file = LoadedFileName;
            if (!fullPathProvided)
            {
                file = @"Save\" + LoadedFileName;
            }

            //bool zip = true;

            //if (Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop)
            //{
            //    zip = false;
            //}

            bool result = SimpleSerializer.SerializeJsonFile(scenarioClone, file, false, false, fullPathProvided);

            if (result)
            {
                int id;

                string name = LoadedFileName.Replace(".json", "");

                if (int.TryParse(name.Replace("Save", ""), out id))
                {
                    string time = scenarioClone.Date.Year + "-" + scenarioClone.Date.Month + "-" + scenarioClone.Date.Day;

                    saves[id] = new Scenario()
                    {
                        Create = DateTime.Now.ToSeasonDateTime(),
                        Desc = scenarioClone.ScenarioDescription,
                        IDs = "",
                        Info = scenarioClone.PlayerInfo,
                        Name = name,
                        Names = "",
                        Path = "",
                        PlayTime = GameTime.ToString(),
                        Player = "",
                        Players = String.Join(",", scenarioClone.PlayerList.NullToEmptyList()),
                        Time = time.ToSeasonDate(),
                        Title = scenarioClone.ScenarioTitle,
                        Mod = scenarioClone.MOD
                    };
                    if(!editing)
                    {
                        SaveScenarioSaves(saves);
                    }
                    else 
                    {
                        string saveDir = @"Save\";
                        string saveFile = saveDir + "Saves.json";
                        SimpleSerializer.SerializeJsonFile(saves, saveFile);
                    }
                }
            }

            scenarioClone = null;

            JustSaved = true;

            //ExtensionInterface.call("Save", new Object[] { this });

            return true;
        }

        public static void LoadGameCommonData()
        {
            // TODO: 配置项全局唯一的，为什么需要重新匹配？编辑器修改没有强关联或校验？

            var conditionKinds = CommonData.Current.AllConditionKinds;
            var eventEffectKinds = CommonData.Current.AllEventEffectKinds;
            var troopEventEffectKinds = CommonData.Current.AllTroopEventEffectKinds;

            foreach (var condition in CommonData.Current.AllConditions.Values)
            {
                var kindId = condition.KindId;
                if (conditionKinds.TryGetValue(kindId, out var matchedKind))
                {
                    condition.Kind = matchedKind;
                }
                else
                {
                    logger.Error($"条件类型Id:[{kindId}]不存在");
                }
            }

            foreach (var influence in CommonData.Current.AllInfluences.Values)
            {
                var kindId = influence.KindId;
                if (CommonData.Current.AllInfluenceKinds.TryGetValue(kindId, out var matchedKind))
                {
                    influence.Kind = matchedKind;
                }
                else
                {
                    logger.Error($"影响类型Id:[{kindId}]不存在");
                }
            }

            foreach (var eventEffect in CommonData.Current.AllEventEffects.Values)
            {
                var kindId = eventEffect.KindId;
                if (eventEffectKinds.TryGetValue(kindId, out var matchedKind))
                {
                    eventEffect.Kind = matchedKind;
                }
                else
                {
                    logger.Error($"事件影响类型Id:[{kindId}]不存在");
                }
            }

            foreach (var eventEffect in CommonData.Current.AllTroopEventEffects.Values)
            {
                var kindId = eventEffect.KindId;
                if (troopEventEffectKinds.TryGetValue(kindId, out var matchedKind))
                {
                    eventEffect.Kind = matchedKind;
                }
                else
                {
                    logger.Error($"部队事件影响类型Id:[{kindId}]不存在");
                }
            }
        }

        public static List<Scenario> LoadScenarioSaves()
        {
            string saveDir = @"Save\";

            if (!Platform.Current.UserDirectoryExist(saveDir))
            {
                Platform.Current.UserDirectoryCreate(saveDir);
            }

            string saveFile = saveDir + "Saves.json";

            List<Scenario> scesList = null;

            if (Platform.Current.UserFileExist(new String[] {saveFile})[0])
            {
                scesList = SimpleSerializer.DeserializeJsonFile<List<Scenario>>(saveFile, true).NullToEmptyList();
            }
            
            if (scesList == null)
            {
                scesList = new List<Scenario>();

                for (int i = 0; i <= savemaxcounts+1; i++)
                {
                    var sce = new Scenario()
                    {
                        ID = i < 10 ? "0" + i.ToString() : i.ToString()
                    };
                    scesList.Add(sce);
                }
            }
            else if(scesList.Count<=GameScenario.savemaxcounts)
            {
                for (int i = scesList.Count; i <= savemaxcounts ; i++)
                {
                    var sce = new Scenario()
                    {
                        ID = i < 10 ? "0" + i.ToString() : i.ToString()
                    };
                    scesList.Add(sce);
                }
            }

            return scesList;
        }

        public static void SaveScenarioSaves(List<Scenario> saves)
        {
            string saveDir = @"Save\";
            string saveFile = saveDir + "Saves.json";

            SimpleSerializer.SerializeJsonFile(saves, saveFile);

            if (Session.MainGame.mainMenuScreen.MenuType == WorldOfTheThreeKingdoms.GameScreens.MenuType.Save)
            {
                Session.MainGame.mainMenuScreen.InitScenarioSaveList();
            }
        }

        public void DisposeLotsOfMemory()
        {
            //foreach (MilitaryKind kind in this.GameCommonData.AllMilitaryKinds.MilitaryKinds.Values)
            //{
            //    kind.Textures.Dispose();
            //}
            //foreach (Animation a in this.GameCommonData.AllTroopAnimations.Animations.Values)
            //{
            //    a.disposeTexture();
            //}
            //foreach (Architecture a in this.Architectures)
            //{
            //    if (a.CaptionTexture != null)
            //    {
            //        a.CaptionTexture.Dispose();
            //        a.CaptionTexture = null;
            //    }
            //}
            //foreach (ArchitectureKind k in this.GameCommonData.AllArchitectureKinds.ArchitectureKinds.Values)
            //{
            //    if (k.Texture != null)
            //    {
            //        k.ClearTexture();
            //    }
            //}
            //foreach (Treasure t in this.Treasures)
            //{
            //    t.disposeTexture();
            //}
            //foreach (TerrainDetail t in this.GameCommonData.AllTerrainDetails.TerrainDetails.Values)
            //{
            //    if (t.Textures != null)
            //    {
            //        //foreach (var u in t.Textures.BasicTextures)
            //        //{
            //        //    u.Dispose();
            //        //}
            //        foreach (Texture u in t.Textures.BottomEdgeTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.BottomLeftCornerTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.BottomLeftTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.BottomRightCornerTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.BottomRightTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.BottomTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.CentreTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.LeftEdgeTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.LeftTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.RightEdgeTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.RightTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.LeftEdgeTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopEdgeTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopLeftCornerTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopLeftTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopRightCornerTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopRightTextures)
            //        {
            //            u.Dispose();
            //        }
            //        foreach (Texture u in t.Textures.TopTextures)
            //        {
            //            u.Dispose();
            //        }
            //    }
            //    t.Textures = null;
            //}

            if (Session.MainGame != null && Session.MainGame.mainGameScreen != null)
            {
                Session.MainGame.mainGameScreen.DisposeMapTileMemory(true, false);
            }
        }

        public void SetMapTileArchitecture(Architecture architecture)
        {
            if (!architecture.AutoRefillFoodInLongViewArea)
            {
                architecture.AddBaseSupplyingArchitecture();
            }
            foreach (Point point in architecture.ViewArea.Area)
            {
                if (!this.PositionOutOfRange(point))
                {
                    this.MapTileData[point.X, point.Y].AddHighViewingArchitecture(architecture);
                }
            }
            foreach (Point point in architecture.LongViewArea.Area)
            {
                if (!this.PositionOutOfRange(point))
                {
                    this.MapTileData[point.X, point.Y].AddViewingArchitecture(architecture);
                }
            }
        }

        public void SetMapTileTroop(Troop troop)
        {
            if (this.MapTileData[troop.PreviousPosition.X, troop.PreviousPosition.Y].TroopCount > 0)
            {
                TileData data1 = this.MapTileData[troop.PreviousPosition.X, troop.PreviousPosition.Y];
                data1.TroopCount--;
            }
            if (this.MapTileData[troop.PreviousPosition.X, troop.PreviousPosition.Y].TileTroop == troop)
            {
                this.MapTileData[troop.PreviousPosition.X, troop.PreviousPosition.Y].TileTroop = null;
                /*foreach (Troop t in this.Troops)
                {
                    if (!t.Destroyed && t.Position == troop.PreviousPosition)
                    {
                        this.MapTileData[troop.PreviousPosition.X, troop.PreviousPosition.Y].TileTroop = t;
                        break;
                    }
                }*/
            }
            TileData data2 = this.MapTileData[troop.Position.X, troop.Position.Y];
            data2.TroopCount++;
            if (this.MapTileData[troop.Position.X, troop.Position.Y].TileTroop == null)
            {
                this.MapTileData[troop.Position.X, troop.Position.Y].TileTroop = troop;
            }
        }

        public void SetPenalizedMapDataByArea(GameArea gameArea, int cost)
        {
            foreach (Point point in gameArea.Area)
            {
                if (!this.PositionOutOfRange(point))
                {
                    this.PenalizedMapData[point.X, point.Y] = cost;
                }
            }
            this.SetPenalizedMapDataByPosition(gameArea.Centre, 0xdac);
        }

        public void SetPenalizedMapDataByPosition(Point position, int cost)
        {
            this.PenalizedMapData[position.X, position.Y] = cost;
        }

        public void SetPlayerFactionList(GameObjectList factions)
        {
            this.PlayerFactions.Clear();
            if (factions != null)
            {
                foreach (Faction faction in factions)
                {
                    this.PlayerFactions.Add(faction);
                }
            }
        }

        public void SetPositionOnFire(Point position)
        {
            this.FireTable.AddPosition(position);
            this.GeneratorOfTileAnimation.AddTileAnimation(TileAnimationKind.火焰, position, true);
        }

        public void YearPassedEvent()
        {
            ExtensionInterface.call("YearEvent", new Object[] { this });
            foreach (var architecture in StaticMethods.GetRandomList(Architectures.Values.ToList()))
            {
                architecture.YearEvent();
            }

            foreach (Faction faction in this.Factions)
            {
                faction.YearOfficialLimit = 0;
            }

            var minChildrenAge = Session.GlobalVariables.ChildrenAvailableAge;
            foreach (var person in AllPersons.Values)
            {
                if (person.Available && person.IsGeneratedChildren && person.Age >= minChildrenAge)
                {
                    person.IsGeneratedChildren = false;
                }
            }
        }

        public void YearStartingEvent()
        {
        }

        public bool Animating
        {
            get
            {
                return this.Troops.HasAnimatingTroop;
            }
        }

        public Person NeutralPerson => AllPersons.GetValueOrDefault(7007);

        public bool NoCurrentPlayer => CurrentPlayer == null;

        public TroopAnimation TroopAnimations => GameCommonData.TroopAnimations;

        private Architecture huangdisuozai = null;

        public Architecture huangdisuozaijianzhu()
        {
            if (huangdisuozai == null)
            {
                foreach (var architecture in Architectures.Values)
                {
                    if (architecture.huangdisuozai)
                    {
                        huangdisuozai = architecture;
                    }
                }
            }

            return huangdisuozai;
        }

        public bool youhuangdi()
        {
            foreach (var architecture in Architectures.Values)
            {
                if (architecture.huangdisuozai) return true;
            }

            return false;
        }

        public delegate void AfterLoadScenario();

        public delegate void AfterSaveScenario();

        public delegate void NewFactionAppear(Faction faction);

        public void BecomeNoEmperor()
        {
            foreach (var architecture in Architectures.Values)
            {
                if (architecture.huangdisuozai)
                {
                    architecture.huangdisuozai = false;
                    huangdisuozai = null;
                }
            }

            var neutralPerson = NeutralPerson;
            if (neutralPerson == null)
            {
                if (CurrentPlayer != null)
                {
                    neutralPerson = CurrentPlayer.Leader;
                }
                else
                {
                    if (Factions.Count > 0)
                    {
                        neutralPerson = (Factions[0] as Faction).Leader;
                    }
                }
            }

            Session.MainGame.mainGameScreen.xianshishijiantupian(neutralPerson, "汉朝", "FactionDestroy", "shilimiewang.jpg", "shilimiewang", true);

        }

        public YearTable getFactionYearTable(Faction f)
        {
            YearTable result = new YearTable();
            foreach (YearTableEntry i in this.YearTable)
            {
                if (i.IsGloballyKnown || i.Factions.GameObjects.Contains(f) || Session.GlobalVariables.SkyEye)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public YearTable getFactionYearTableRecentYears(Faction f, int y)
        {
            YearTable result = new YearTable();
            foreach (YearTableEntry i in this.YearTable)
            {
                if ((i.IsGloballyKnown || i.Factions.GameObjects.Contains(f) || Session.GlobalVariables.SkyEye) &&
                    i.Date.Year > this.Date.Year - y)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public YearTable getOnlyFactionYearTable(Faction f)
        {
            YearTable result = new YearTable();
            foreach (YearTableEntry i in this.YearTable)
            {
                if (i.Factions.GameObjects.Contains(f))
                {
                    result.Add(i);
                }
            }
            return result;
        }
        public bool runScenarioStart(Architecture triggerArch, Screen screen)
        {
            bool ran = false;
            foreach (Event e in this.AllEvents)
            {
                if ((e.IsStart() && e.matchEventPersons(triggerArch)) || e.checkConditions(triggerArch))
                {
                    if (!this.EventsToApply.ContainsKey(e))
                    {
                        this.EventsToApply.Add(e, triggerArch);
                        e.ApplyEventDialogs(triggerArch, screen);
                        ran = true;
                    }
                    if (!this.YesEventsToApply.ContainsKey(e) && e.yesEffect.Count > 0)
                    {
                        this.YesEventsToApply.Add(e, triggerArch);
                        ran = true;
                    }
                    if (!this.NoEventsToApply.ContainsKey(e) && e.noEffect.Count > 0)
                    {
                        this.NoEventsToApply.Add(e, triggerArch);
                        ran = true;
                    }
                    /*
                    if (!this.YesArchiEventsToApply.ContainsKey(e))
                    {
                        this.YesArchiEventsToApply.Add(e, triggerArch);

                        e.ApplyEventDialogs(triggerArch);
                        ran = true;
                    }
                    if (!this.NoArchiEventsToApply.ContainsKey(e))
                    {
                        this.NoArchiEventsToApply.Add(e, triggerArch);
                        e.ApplyEventDialogs(triggerArch);
                        ran = true;
                    }
                    */
                }
            }
            return ran;
        }

        public bool runScenarioEnd(Architecture triggerArch, Screen screen)
        {
            bool ran = false;
            foreach (Event e in this.AllEvents)
            {
                if ((e.IsEnd() && e.matchEventPersons(triggerArch)) || e.checkConditions(triggerArch))
                {
                    if (!this.EventsToApply.ContainsKey(e))
                    {
                        this.EventsToApply.Add(e, triggerArch);
                        e.ApplyEventDialogs(triggerArch, screen);
                        ran = true;
                    }

                    if (!this.YesEventsToApply.ContainsKey(e) && e.yesEffect.Count > 0)
                    {
                        this.YesEventsToApply.Add(e, triggerArch);
                        ran = true;
                    }
                    if (!this.NoEventsToApply.ContainsKey(e) && e.noEffect.Count > 0)
                    {
                        this.NoEventsToApply.Add(e, triggerArch);
                        ran = true;
                    }
                    /*
                    if (!this.YesArchiEventsToApply.ContainsKey(e))
                    {
                        this.YesArchiEventsToApply.Add(e, triggerArch);

                        e.ApplyEventDialogs(triggerArch);
                        ran = true;
                    }
                    if (!this.NoArchiEventsToApply.ContainsKey(e))
                    {
                        this.NoArchiEventsToApply.Add(e, triggerArch);
                        e.ApplyEventDialogs(triggerArch);
                        ran = true;
                    }*/
                }
            }
            return ran;
        }

        /// <summary>
        /// 野武将列表
        /// </summary>
        /// <returns></returns>
        public List<Person> Officers()
        {
            var result = new List<Person>();

            foreach (var person in AllPersons.Values)
            {
                if (person.Available && person.Alive && person.ID >= 25000)
                {
                    result.Add(person);
                }
            }

            return result;
        }

        /// <summary>
        /// 野武将总数
        /// </summary>
        public int OfficerCount => Officers().Count;

        public int OfficerLimit => Session.GlobalVariables.zhaoxianOfficerMax;

        public int GetAITroopCount()
        {
            int cnt = 0;
            foreach (Troop t in this.Troops)
            {
                if (!this.IsPlayer(t.BelongedFaction))
                {
                    cnt++;
                }
            }
            return cnt;
        }

        public bool IsKnownToAnyPlayer(Architecture a)
        {
            if (Session.GlobalVariables.SkyEye) return true;
            foreach (Faction f in this.PlayerFactions)
            {
                if (f.IsArchitectureKnown(a)) return true;
            }
            return false;
        }

        public bool IsKnownToAnyPlayer(Troop a)
        {
            if (Session.GlobalVariables.SkyEye) return true;
            foreach (Faction f in this.PlayerFactions)
            {
                if (f.IsTroopKnown(a)) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取训练的老师
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private List<Person> GetTeachers(Person person)
        {
            var teachers = new List<Person>();

            // 按年龄倒排人物
            var allPersons = AllPersons.Values.ToList();
            allPersons.Sort((a, b) => b.Age.CompareTo(a.Age));

            foreach (var candidate in allPersons)
            {
                if (candidate.HasCloseStrainTo(person) && candidate.IsValidTeacher(person))
                {
                    teachers.Add(candidate);

                    if (teachers.Count > 3) break;
                }
            }
            
            if (teachers.Count <= 3)
            {
                foreach (var candidate in allPersons)
                {
                    if (!teachers.Contains(candidate) && candidate.HasMotherStrainTo(person) && candidate.IsValidTeacher(person))
                    {
                        teachers.Add(candidate);

                        if (teachers.Count > 3) break;
                    }
                }
            }

            if (teachers.Count == 0)
            {
                // 按功绩倒排人物
                allPersons.Sort((a, b) => b.Merit.CompareTo(a.Merit));

                foreach (var candidate in allPersons)
                {
                    if (GameObject.GetChance(10) && candidate.IsValidTeacher(person))
                    {
                        teachers.Add(candidate);
                        break;
                    }
                }
            }

            return teachers;
        }

        /// <summary>
        /// 培育子女
        /// </summary>
        public void TrainChildren()
        {
            var dayInTurn = Session.Parameters.DayInTurn;
            var aiRate = Session.Current.Scenario.Parameters.AIExtraPerson;
            var defaultTrainPolicy = GameCommonData.AllTrainPolicies.Values.ToList().FirstOrDefault();

            foreach (var person in AllPersons.Values)
            {
                if (!ShouldTrainThisTurn(person, dayInTurn, aiRate)) continue;

                var policy = person.TrainPolicy ?? defaultTrainPolicy;
                var weighting = GetSafePolicyWeighting(policy, person);

                int r = GameObject.WeightedRandom(weighting);
                var teachers = GetTeachers(person);

                switch (r)
                {
                    case 1:
                        TrainAttribute(person, teachers,
                            p => p.Strength, p => p.StrengthPotential,
                            t => t.Strength, t => t.childrenAbilityIncrease,
                            (p, inc) => p.BaseStrength += inc,
                            ratioNumerator: 6, ratioDenominator: 5);
                        break;
                    case 2:
                        TrainAttribute(person, teachers,
                            p => p.Command, p => p.CommandPotential,
                            t => t.Command, t => t.childrenAbilityIncrease,
                            (p, inc) => p.BaseCommand += inc,
                            ratioNumerator: 6, ratioDenominator: 5);
                        break;
                    case 3:
                        TrainAttribute(person, teachers,
                            p => p.Intelligence, p => p.IntelligencePotential,
                            t => t.Intelligence, t => t.childrenAbilityIncrease,
                            (p, inc) => p.BaseIntelligence += inc,
                            ratioNumerator: 6, ratioDenominator: 5);
                        break;
                    case 4:
                        TrainAttribute(person, teachers,
                            p => p.Politics, p => p.PoliticsPotential,
                            t => t.Politics, t => t.childrenAbilityIncrease,
                            (p, inc) => p.BasePolitics += inc,
                            ratioNumerator: 6, ratioDenominator: 5);
                        break;
                    case 5:
                        TrainAttribute(person, teachers,
                            p => p.Glamour, p => p.GlamourPotential,
                            t => t.Glamour, t => t.childrenAbilityIncrease,
                            (p, inc) => p.BaseGlamour += inc,
                            ratioNumerator: 6, ratioDenominator: 5);
                        break;
                    case 6:
                        TrainSkill(person, teachers);
                        break;
                    case 7:
                        TrainStunt(person, teachers);
                        break;
                    case 8:
                        TrainTitle(person, teachers);
                        break;
                }
            }
        }

        /// <summary>
        /// 本回合是否触发培育
        /// </summary>
        /// <param name="person"></param>
        /// <param name="dayInTurn"></param>
        /// <param name="aiRate"></param>
        /// <returns></returns>
        private bool ShouldTrainThisTurn(Person person, int dayInTurn, float aiRate)
        {
            if (!person.Trainable) return false;
        
            float playerRate = IsPlayer(person.Father.BelongedFaction) ? 1 : aiRate;
        
            int chance = (int)(30 / playerRate / dayInTurn);
        
            return StaticMethods.Random(chance) == 0;
        }

        /// <summary>
        /// 安全获取培育策略权重
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        private Dictionary<int, float> GetSafePolicyWeighting(TrainPolicy policy, Person person)
        {
            var weighting = new Dictionary<int, float>(policy.Weighting);
            if (person.Age < 8)
            {
                weighting.Remove(8);
            }

            return weighting;
        }
        
        /// <summary>
        /// 基本属性培育
        /// </summary>
        /// <param name="person"></param>
        /// <param name="teachers"></param>
        /// <param name="getPersonValue"></param>
        /// <param name="getPersonPotential"></param>
        /// <param name="getTeacherValue"></param>
        /// <param name="getTeacherIncrease"></param>
        /// <param name="addToBase"></param>
        /// <param name="ratioNumerator"></param>
        /// <param name="ratioDenominator"></param>
        private void TrainAttribute(
            Person person,
            IEnumerable<Person> teachers,
            Func<Person, int> getPersonValue,
            Func<Person, int> getPersonPotential,
            Func<Person, int> getTeacherValue,
            Func<Person, int> getTeacherIncrease,
            Action<Person, int> addToBase,
            int ratioNumerator,
            int ratioDenominator)
        {
            foreach (var teacher in teachers)
            {
                int personValue = getPersonValue(person);
                if (personValue <= 0) continue; // 防止除零

                int strengthChance = (int)(
                    (getTeacherValue(teacher) - personValue + 50 + getTeacherIncrease(teacher))
                    * ((float)getPersonPotential(person) / personValue));

                if (!GameObject.GetChance(strengthChance)) continue;

                var baseIncrement = Math.Max(
                    (getPersonPotential(person) * ratioNumerator / ratioDenominator - personValue) / 10,
                    1) + 1;

                addToBase(person, GameObject.Random(baseIncrement));
                AdjustTeacherRelations(person, teacher);
            }
        }

        /// <summary>
        /// 技能培育
        /// </summary>
        /// <param name="person"></param>
        /// <param name="teachers"></param>
        private void TrainSkill(Person person, IEnumerable<Person> teachers)
        {
            var father = person.Father;
            var mother = person.Mother;

            foreach (var teacher in teachers)
            {
                var skills = new List<Skill>();

                foreach (var skill in teacher.Skills.Values)
                {
                    if (skill.CanBeBorn(person))
                    {
                        skills.Add(skill);
                    }
                }

                foreach (var skill in GameCommonData.AllSkills.Values)
                {
                    if (!skill.CanBeBorn(person)) continue;

                    var skillChance = (skill.GetRelatedAbility(teacher) - 70) / 5;
                    var levelChance = 100 / skill.Level;

                    if (GameObject.GetChance(skillChance) && GameObject.GetChance(levelChance))
                    {
                        skills.Add(skill);
                    }
                }

                var candidateCount = Math.Min(skills.Count, 3);
                var candidateSkills = StaticMethods.GetRandomList(skills).GetRange(0, candidateCount);

                foreach (var skill in candidateSkills)
                {
                    int skillId = skill.ID;
                    int skillChance = 100 / skill.Level + teacher.childrenSkillChanceIncrease;

                    // 如果父母有该技能，则提升机率
                    if ((father != null && father.Skills.ContainsKey(skillId)) || (mother != null && mother.Skills.ContainsKey(skillId)))
                    {
                        skillChance += 5;
                    }

                    if (GameObject.GetChance(skillChance))
                    {
                        person.AddSkill(skill);
                        AdjustTeacherRelations(person, teacher);
                    }
                }
            }
        }

        /// <summary>
        /// 特技培育
        /// </summary>
        /// <param name="person"></param>
        /// <param name="teachers"></param>
        private void TrainStunt(Person person, IEnumerable<Person> teachers)
        {
            var father = person.Father;
            var mother = person.Mother;

            foreach (var teacher in teachers)
            {
                var stunts = new List<Stunt>();

                foreach (var stunt in teacher.Stunts.Values)
                {
                    if (stunt.CanBeBorn(person))
                    {
                        stunts.Add(stunt);
                    }
                }

                var candidates = new List<Stunt>();
                foreach (var stunt in GameCommonData.AllStunts.Values)
                {
                    if (stunt.CanBeBorn(person))
                    {
                        candidates.Add(stunt);
                    }
                }

                int teacherChance = (teacher.Strength + teacher.Command + teacher.Intelligence - 210) / 15;
                if (candidates.Count > 0 && GameObject.GetChance(teacherChance))
                {
                    stunts.Add(StaticMethods.GetRandomItem(candidates));
                }

                if (stunts.Count == 0) continue;

                var stuntToTeach = StaticMethods.GetRandomItem(stunts);

                int extraChance = 0;
                if ((father != null && father.Stunts.ContainsKey(stuntToTeach.ID)) || (mother != null && mother.Stunts.ContainsKey(stuntToTeach.ID)))
                {
                    extraChance += 10;
                }

                int stuntChance = (10 + teacher.childrenStuntChanceIncrease + extraChance) / 3;
                if (GameObject.GetChance(stuntChance))
                {
                    person.AddStunt(stuntToTeach);
                    AdjustTeacherRelations(person, teacher, 10);
                }
            }
        }

        /// <summary>
        /// 称号培育
        /// </summary>
        /// <param name="person"></param>
        /// <param name="teachers"></param>
        private void TrainTitle(Person person, IEnumerable<Person> teachers)
        {
            var father = person.Father;
            var mother = person.Mother;

            foreach (var teacher in teachers)
            {
                var titles = teacher.Titles;

                int maxLevel = 1;
                foreach (var title in titles)
                {
                    if (title.Level > maxLevel && title.Kind.RandomTeachable)
                    {
                        maxLevel = title.Level;
                    }
                }

                maxLevel += teacher.childrenTitleChanceIncrease + 1;

                foreach (var title in GameCommonData.AllTitles.Values)
                {
                    if (title.Kind.RandomTeachable
                        && title.Level <= maxLevel
                        && GameObject.GetChance(title.InheritChance)
                        && title.CanBeBorn(person))
                    {
                        titles.Add(title);
                    }
                }

                foreach (var title in titles)
                {
                    var titleChance = (title.InheritChance + teacher.childrenTitleChanceIncrease) * 3;

                    if ((father != null && father.RealTitles.Contains(title)) || (mother != null && mother.RealTitles.Contains(title)))
                    {
                        titleChance += 5;
                    }

                    if (!GameObject.GetChance(titleChance) || !title.CanBeBorn(person)) continue;

                    var existedTitle = person.GetTitleByKind(title.KindId);

                    // TODO let player choose
                    bool shouldReplace = existedTitle == null
                        || existedTitle.Level < title.Level
                        || (existedTitle.Level == title.Level && existedTitle.Merit < title.Merit);

                    if (shouldReplace)
                    {
                        person.RealTitles.Remove(existedTitle);
                        person.RealTitles.Add(title);
                        AdjustTeacherRelations(person, teacher, 5 * title.Level);
                    }
                }
            }
        }

        /// <summary>
        /// 调整和老师及其相关人物的关系
        /// </summary>
        /// <param name="person"></param>
        /// <param name="teacher"></param>
        private void AdjustTeacherRelations(Person person, Person teacher, int adjust = 5)
        {
            person.AdjustRelation(teacher, 5, adjust);
            teacher.AdjustRelation(person, 2, adjust);

            if (GameObject.GetChance(30))
            {
                var relations = teacher.GetAllRelations();
                var relationsCount = relations.Count;

                foreach (var item in relations)
                {
                    if (GameObject.GetChance(100 / relationsCount))
                    {
                        var minAdjust = Math.Min(adjust, item.Value / 10);
                        person.AdjustRelation(item.Key, 2, minAdjust);
                    }
                }
            }
        }

        public bool SkyEyeSimpleNotification(GameObject gameobject)
        {
            if (Session.GlobalVariables.SkyEyeSimpleNotification && gameobject != null)
            {
                if (gameobject is Person && (this.CurrentPlayer == null || !this.CurrentPlayer.IsPositionKnown((gameobject as Person).Position)))
                {
                    return true;
                }
                if (gameobject is Troop && (this.CurrentPlayer == null || !this.CurrentPlayer.IsPositionKnown((gameobject as Troop).Position)))
                {
                    return true;
                }
                if (gameobject is Architecture && (this.CurrentPlayer == null || !this.CurrentPlayer.IsArchitectureKnown((gameobject as Architecture))))
                {
                    return true;
                }
            }
            return false;
        }

        public void captivestocaptiveData(CaptiveList captives)
        {
            this.captiveData = captives;
        }
    }
}
