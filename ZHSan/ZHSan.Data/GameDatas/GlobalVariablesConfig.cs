
using GameEnums;

namespace GameDatas;

public class GlobalVariablesConfig
{
    public bool WujiangYoukenengDuli { get; set; }
    
    public bool LiangdaoXitong { get; set; }
    
    public bool ShowGrid { get; set; }
    
    public bool AdditionalPersonAvailable { get; set; }
    
    public float ArchitectureLayerDepth { get; set; }
    
    public bool CalculateAverageCostOfTiers { get; set; }
    
    public bool CommonPersonAvailable { get; set; }
    
    public MapLayerKind CurrentMapLayer { get; set; }
    
    public bool DrawMapVeil { get; set; }
    
    public bool DrawTroopAnimation { get; set; }
    
    public long FactionRunningTicksLimitInOneFrame { get; set; }
    
    public int FastBattleSpeed { get; set; }
    
    public string GameDifficulty { get; set; }
    
    public bool HintPopulation { get; set; }
    
    public bool HintPopulationUnder1000 { get; set; }
    
    public bool IdealTendencyValid { get; set; }
    
    public bool LoadBackGroundMapTexture { get; set; }
    
    public float MapScrollSpeed { get; set; }
    
    public int MaxCountOfKnownPaths { get; set; }
    
    public int MaxTimeOfAnimationFrame { get; set; }
    
    public bool MilitaryKindSpeedValid { get; set; }
    
    public bool MultipleResource { get; set; }
    
    public bool NoHintOnSmallFacility { get; set; }
    
    public bool? PersonNaturalDeath { get; set; }
    
    public bool PlayBattleSound { get; set; }
    
    public bool PlayerPersonAvailable { get; set; }
    
    public bool PlayMusic { get; set; }
    
    public bool PlayNormalSound { get; set; }
    
    public bool PopulationRecruitmentLimit { get; set; }
    
    public InformationLevel RoutewayInformationLevel { get; set; }
    public bool RunWhileNotFocused { get; set; }
    
    public InformationLevel ScoutRoutewayInformationLevel { get; set; }
    
    public bool SingleSelectionOneClick { get; set; }
    
    public bool SkyEye { get; set; }
    
    public int TroopMoveFrameCount { get; set; }
    
    public int TroopMoveLimitOnce { get; set; }
    
    public int TroopMoveSpeed { get; set; }
    
    public bool PinPointAtPlayer { get; set; }
    
    public bool IgnoreStrategyTendency { get; set; }
    
    public bool createChildren { get; set; }
    
    public int zainanfashengjilv { get; set; }
    
    public bool doAutoSave { get; set; }
    
    public bool createChildrenIgnoreLimit { get; set; }
    
    public bool internalSurplusRateForPlayer { get; set; }
    
    public bool internalSurplusRateForAI { get; set; }
    
    public int getChildrenRate { get; set; }
    
    public int hougongGetChildrenRate { get; set; }
    
    public bool hougongAlienOnly { get; set; }
    
    public int getRaisedSoliderRate { get; set; }
    
    public float AIExecutionRate { get; set; }
    
    public bool AIExecuteBetterOfficer { get; set; }
    
    public int maxExperience { get; set; }
    
    public bool lockChildrenLoyalty { get; set; }
    
    public bool AIAutoTakeNoFactionCaptives { get; set; }
    
    public bool AIAutoTakeNoFactionPerson { get; set; }
    
    public bool AIAutoTakePlayerCaptives { get; set; }
    
    public bool AIAutoTakePlayerCaptiveOnlyUnfull { get; set; }
    
    public float TechniquePointMultiple { get; set; }
    
    public bool PermitFactionMerge { get; set; }
    
    public float LeadershipOffenceRate { get; set; }
    
    public int DialogShowTime { get; set; }
    
    public bool LandArmyCanGoDownWater { get; set; }
    
    public bool EnableResposiveThreading { get; set; }
    
    public bool EnableCheat { get; set; }
    
    public bool HardcoreMode { get; set; }
    
    public int MaxAbility { get; set; }
    
    public int TirednessIncrease { get; set; }
    
    public int TirednessDecrease { get; set; }
    
    public bool EnableAgeAbilityFactor { get; set; }
    
    public int TabListDetailLevel { get; set; }
    
    public bool EnableExtensions { get; set; }
    
    public bool EncryptSave { get; set; }
    
    public int AutoSaveFrequency { get; set; }
    
    public bool ShowChallengeAnimation { get; set; }
    
    public bool PersonDieInChallenge { get; set; }
    
    public int OfficerDieInBattleRate { get; set; }
    
    public int OfficerChildrenLimit { get; set; }
    
    public bool StopToControlOnAttack { get; set; }
    
    public int MaxMilitaryExperience { get; set; }

    public int FactionMilitaryLimt { get; set; }
    
    public float ZhaoXianSuccessRate { get; set; }
    
    public int TroopTirednessDecrease { get; set; }
    
    public float CreateRandomOfficerChance { get; set; }
    
    public int ChildrenAvailableAge { get; set; }
    
    public float CreatedOfficerAbilityFactor { get; set; }
    
    public float ChildrenAbilityFactor { get; set; }
    
    public bool EnablePersonRelations { get; set; }
    
    public int FriendlyDiplomacyThreshold { get; set; }
    
    public int SurroundFactor { get; set; }
    
    public bool FullScreen { get; set; }
    
    public bool PermitQuanXiang { get; set; }
    
    public bool PermitManualAwardTitleAutoLearn { get; set; }
    
    public int zhaoxianOfficerMax { get; set; }
    
    public bool AIZhaoxianFixIdeal { get; set; }
    
    public bool PlayerZhaoxianFixIdeal { get; set; }
    
    public int FixedUnnaturalDeathAge { get; set; }
    
    public bool AIQuickBattle { get; set; }
    
    public bool PlayerAutoSectionHasAIResourceBonus { get; set; }
    
    public float ProhibitFactionAgainstDestroyer { get; set; }
    
    public float AIMergeAgainstPlayer { get; set; }
    
    public bool RemoveSpouseIfNotAvailable { get; set; }
    
    public bool SkyEyeSimpleNotification { get; set; }

    public bool AutoMultipleMarriage { get; set; }
    
    public bool BornHistoricalChildren { get; set; }
    
    public float StartCircleTime { get; set; }
    
    public float ScenarioMapPerTime { get; set; }
    
    public int KeepSpousePersonalLoyalty { get; set; }
    
    public bool TroopVoice { get; set; }
    
    public int MaxTupianwenzi { get; set; }

    public int ShowNumberAddTime { get; set; }
}