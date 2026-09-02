
using System.Collections.Generic;
using GameEnums;
using Microsoft.Xna.Framework;

namespace GameDatas;

public class PersonConfig : BaseConfig
{
    public bool huaiyun { get; set; }

    public bool shoudongsousuo { get; set; }

    public int huaiyuntianshu { get; set; } = -1;

    public bool ManualStudy { get; set; }

    public bool faxianhuaiyun { get; set; }

    public int suoshurenwu { get; set; } = -1;

    public int princessTakerID { get; set; } = -1;

    public int PCharacter { get; set; }

    public int ConvincingPersonID { get; set; }

    public ArchitectureWorkKind OldWorkKind { get; set; } = ArchitectureWorkKind.无;

    public ArchitectureWorkKind firstPreferred { get; set; } = ArchitectureWorkKind.无;

    public bool RewardFinished { get; set; }

    public string SkillsString { get; set; }

    public string StuntsString { get; set; }

    public int StudyingStuntString { get; set; }

    public string RealTitlesString { get; set; }

    public int PersonalTitleString { get; set; }

    public int CombatTitleString { get; set; }

    public int StudyingTitleString { get; set; }

    public string UniqueMilitaryKindsString { get; set; }

    public string UniqueTitlesString { get; set; }

    public int WaitForFeiZiPeriod { get; set; }

    public int waitForFeiziId { get; set; }

    public bool Immortal { get; set; }

    public int BattleSelfDamage { get; set; }

    public bool IsGeneratedChildren { get; set; }

    public int StrengthPotential { get; set; }

    public int CommandPotential { get; set; }

    public int IntelligencePotential { get; set; }

    public int PoliticsPotential { get; set; }

    public int GlamourPotential { get; set; }

    public int TrainPolicyIDString { get; set; }

    public string Tags { get; set; }

    public int TempLoyaltyChange { get; set; }

    public bool wasMayor { get; set; }

    public int DaySinceAvailable { get; set; }

    public bool NvGuan { get; set; }

    public int Karma { get; set; }

    public int Fund { get; set; }

    public float InjureRate { get; set; }

    public string preferredTroopPersonsString { get; set; }

    public int OfficerMerit { get; set; }

    public int Tiredness { get; set; }

    public int YearJoin { get; set; }

    public int TroopDamageDealt { get; set; }

    public int TroopBeDamageDealt { get; set; }

    public int ArchitectureDamageDealt { get; set; }

    public int RebelCount { get; set; }

    public int ExecuteCount { get; set; }

    public int OfficerKillCount { get; set; }

    public int FleeCount { get; set; }

    public int HeldCaptiveCount { get; set; }

    public int CaptiveCount { get; set; }

    public int StratagemSuccessCount { get; set; }

    public int StratagemFailCount { get; set; }

    public int StratagemBeSuccessCount { get; set; }

    public int StratagemBeFailCount { get; set; }

    public OutsideTaskKind LastOutsideTask { get; set; } = OutsideTaskKind.无;

    public int ReturnedDaySince { get; set; }

    public int NumberOfChildren { get; set; }

    public bool Alive { get; set; }

    public int Ambition { get; set; }

    public int ArrivingDays { get; set; }

    public bool Available { get; set; }

    public int AvailableLocation { get; set; }

    public int BaseCommand { get; set; }

    public int BaseGlamour { get; set; }

    public int BaseIntelligence { get; set; }

    public int BasePolitics { get; set; }

    public int BaseStrength { get; set; }

    public PersonBornRegion BornRegion { get; set; }

    public int BaseBraveness { get; set; }

    public int BubingExperience { get; set; }

    public string CalledName { get; set; }

    public int BaseCalmness { get; set; }

    public int CommandExperience { get; set; }

    public PersonDeadReason DeadReason { get; set; }

    public int Generation { get; set; }

    public string GivenName { get; set; }

    public int GlamourExperience { get; set; }

    public int Ideal { get; set; }

    public int IdealTendencyIDString { get; set; }

    public int InformationKindID { get; set; }

    public int IntelligenceExperience { get; set; }

    public int InternalExperience { get; set; }

    public bool LeaderPossibility { get; set; }

    public int NubingExperience { get; set; }

    public List<int> JoinFactionID { get; set; }

    public Dictionary<int, int> ProhibitedFactionID { get; set; }

    public Point? OutsideDestination { get; set; }

    public OutsideTaskKind OutsideTask { get; set; }

    public int PersonalLoyalty { get; set; }

    public int PictureIndex { get; set; }

    public int PoliticsExperience { get; set; }

    public int QibingExperience { get; set; }

    public int QixieExperience { get; set; }

    public PersonQualification Qualification { get; set; }

    public int Reputation { get; set; }

    public int RoutCount { get; set; }

    public int RoutedCount { get; set; }

    public bool Sex { get; set; }

    public int ShuijunExperience { get; set; }

    public int Strain { get; set; }

    public int StratagemExperience { get; set; }

    public PersonStrategyTendency StrategyTendency { get; set; }

    public int StrengthExperience { get; set; }

    public string SurName { get; set; }

    public int TacticsExperience { get; set; }

    public int TaskDays { get; set; }

    public PersonValuationOnGovernment ValuationOnGovernment { get; set; }

    public ArchitectureWorkKind WorkKind { get; set; }

    public int YearAvailable { get; set; }

    public int YearBorn { get; set; }

    public int YearDead { get; set; }

    public string BelongedPersonName { get; set; }

    public Dictionary<int, int> StatusEffects { get; set; }
}