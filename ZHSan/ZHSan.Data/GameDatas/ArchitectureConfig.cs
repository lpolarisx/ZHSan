
using System.Collections.Generic;

namespace GameDatas;

/// <summary>
/// 建筑
/// </summary>
public class ArchitectureConfig : BaseConfig
{
    public bool TodayPersonArriveNote { get; set; }

    public int CaptionId { get; set; }

    public bool HasManualHire { get; set; }

    public string AILandLinksString { get; set; }

    public string AIWaterLinksString { get; set; }

    public string ArchitectureAreaString { get; set; }

    public string CharacteristicsString { get; set; }

    public int DefensiveLegionId { get; set; }

    public string FacilitiesString { get; set; }

    public string FundPacksString { get; set; }

    public string FoodPacksString { get; set; }

    public string InformationsString { get; set; }

    public int StateId { get; set; }

    public string MilitariesString { get; set; }

    public int PlanArchitectureId { get; set; }

    public int PlanFacilityKindId { get; set; }

    public string PopulationPacksString { get; set; }

    public string MilitaryPopulationPacksString { get; set; }

    public int RecentlyAttacked { get; set; }

    public int RecentlyHit { get; set; }

    public int RecentlyBreaked { get; set; }

    public int RobberTroopId { get; set; }

    public int TransferFoodArchitectureId { get; set; }

    public int TransferFundArchitectureID { get; set; }

    public bool TroopershipAvailable { get; set; }

    // public zainanlei zainan { get; set; }

    public Dictionary<int, int> CaptiveLoyaltyFall { get; set; }

    public bool NoFundToSustainFacility { get; set; }

    public int SuspendTroopTransfer { get; set; }

    public int MayorOnDutyDays { get; set; }

    public string OldFactionName { get; set; }

    public int MilitaryPopulation { get; set; }

    public string CaptivesString { get; set; }

    public string PersonsString { get; set; }

    public string MovingPersonsString { get; set; }

    public string NoFactionPersonsString { get; set; }

    public string NoFactionMovingPersonsString { get; set; }

    public string ConcubineString { get; set; }

    public int Agriculture { get; set; }

    public bool AutoHiring { get; set; }

    public bool AutoRewarding { get; set; }

    public bool AutoSearching { get; set; }

    public bool AutoZhaoXian { get; set; }

    public bool AutoWorking { get; set; }

    public bool AutoRecruiting { get; set; }

    public int BuildingDaysLeft { get; set; }

    public int BuildingFacility { get; set; }

    public int Commerce { get; set; }

    public int Domination { get; set; }

    public int Endurance { get; set; }

    public bool FacilityEnabled { get; set; }

    public int Food { get; set; }

    public int Fund { get; set; }

    public bool HireFinished { get; set; }

    public bool IsStrategicCenter { get; set; }

    public int KindId { get; set; }

    public int Morale { get; set; }

    public int Population { get; set; }

    public int Technology { get; set; }

    /// <summary>
    /// 有灾难
    /// </summary>
    public bool HasDisaster { get; set; }

    /// <summary>
    /// 皇帝所在
    /// </summary>
    public bool HasEmperor { get; set; }

    /// <summary>
    /// 县令Id
    /// </summary>
    public int MayorId { get; set; }
}