
using System.Collections.Generic;

namespace GameDatas;

public class ParametersConfig
{
    public float AIArchitectureDamageRate { get; set; }

    public float AIFoodRate { get; set; }

    public float AIFundRate { get; set; }

    public float AIRecruitmentSpeedRate { get; set; }

    public float AITrainingSpeedRate { get; set; }

    public float AITroopDefenceRate { get; set; }

    public float AITroopOffenceRate { get; set; }

    public float ArchitectureDamageRate { get; set; }

    public int AIAntiStratagem { get; set; }

    public int AIAntiSurround { get; set; }

    public int BuyFoodAgriculture { get; set; }

    public int ChangeCapitalCost { get; set; }

    public int ConvincePersonCost { get; set; }

    public float DefaultPopulationDevelopingRate { get; set; }

    public int DestroyArchitectureCost { get; set; }

    public int FindTreasureChance { get; set; }

    public float FireDamageScale { get; set; }

    public float FollowedLeaderDefenceRateIncrement { get; set; }

    public float FollowedLeaderOffenceRateIncrement { get; set; }

    public float FoodRate { get; set; }

    public int FoodToFundDivisor { get; set; }

    public float FundRate { get; set; }

    public int FundToFoodMultiple { get; set; }

    public int GossipArchitectureCost { get; set; }

    public int JailBreakArchitectureCost { get; set; }

    public int InstigateArchitectureCost { get; set; }

    public int InternalFundCost { get; set; }

    public float InternalRate { get; set; }

    public int LearnSkillDays { get; set; }

    public int LearnStuntDays { get; set; }

    public int LearnTitleDays { get; set; }

    public int SearchDays { get; set; }

    public int RecruitmentDomination { get; set; }

    public int RecruitmentFundCost { get; set; }

    public int RecruitmentMorale { get; set; }

    public float RecruitmentRate { get; set; }

    public int RewardPersonCost { get; set; }

    public int SellFoodCommerce { get; set; }

    public int SurroundArchitectureDominationUnit { get; set; }

    public float TrainingRate { get; set; }

    public float TroopDamageRate { get; set; }

    public float AIArchitectureDamageYearIncreaseRate { get; set; }

    public float AIFoodYearIncreaseRate { get; set; }

    public float AIFundYearIncreaseRate { get; set; }

    public float AIRecruitmentSpeedYearIncreaseRate { get; set; }

    public float AITrainingSpeedYearIncreaseRate { get; set; }

    public float AITroopDefenceYearIncreaseRate { get; set; }

    public float AITroopOffenceYearIncreaseRate { get; set; }

    public float AIArmyExperienceYearIncreaseRate { get; set; }

    public float AIOfficerExperienceYearIncreaseRate { get; set; }

    public float AIAntiStratagemIncreaseRate { get; set; }

    public float AIAntiSurroundIncreaseRate { get; set; }

    public float AIOfficerExperienceRate { get; set; }

    public float AIArmyExperienceRate { get; set; }

    public float BasicAIArchitectureDamageRate { get; set; }

    public float BasicAIFoodRate { get; set; }

    public float BasicAIFundRate { get; set; }

    public float BasicAIRecruitmentSpeedRate { get; set; }

    public float BasicAITrainingSpeedRate { get; set; }

    public float BasicAITroopDefenceRate { get; set; }

    public float BasicAITroopOffenceRate { get; set; }

    public float BasicAIArmyExperienceRate { get; set; }

    public float BasicAIOfficerExperienceRate { get; set; }

    public int BasicAIAntiStratagem { get; set; }

    public int BasicAIAntiSurround { get; set; }

    public float AIBackendArmyReserveCalmBraveDifferenceMultiply { get; set; }

    public float AIBackendArmyReserveAmbitionMultiply { get; set; }

    public float AIBackendArmyReserveAdd { get; set; }

    public float AIBackendArmyReserveMultiply { get; set; }

    public int AITradePeriod { get; set; }

    public int AITreasureChance { get; set; }

    public int AITreasureCountMax { get; set; }

    public float AITreasureCountCappedTitleLevelAdd { get; set; }

    public float AITreasureCountCappedTitleLevelMultiply { get; set; }

    public int AIGiveTreasureMaxWorth { get; set; }

    public float AIFacilityFundMonthWaitParam { get; set; }

    public float AIFacilityDestroyValueRate { get; set; }

    public float AIBuildHougongUnambitionProbWeight { get; set; }

    public float AIBuildHougongSpaceBuiltProbWeight { get; set; }

    public int AIBuildHougongMaxSizeAdd { get; set; }

    public int AIBuildHougongSkipSizeChance { get; set; }

    public int AINafeiUncreultyProbAdd { get; set; }

    public float AINafeiAbilityThresholdRate { get; set; }

    public float AINafeiStealSpouseThresholdRateAdd { get; set; }

    public float AINafeiStealSpouseThresholdRateMultiply { get; set; }

    public int AINafeiMaxAgeThresholdAdd { get; set; }

    public float AINafeiMaxAgeThresholdMultiply { get; set; }

    public float AINafeiSkipChanceAdd { get; set; }

    public float AINafeiSkipChanceMultiply { get; set; }

    public float AIRecruitPopulationCapMultiply { get; set; }

    public float AIRecruitPopulationCapBackendMultiply { get; set; }

    public float AIRecruitPopulationCapHostilelineMultiply { get; set; }

    public float AIRecruitPopulationCapStrategyTendencyMulitply { get; set; }

    public float AIRecruitPopulationCapStrategyTendencyAdd { get; set; }

    public int AINewMilitaryPopulationThresholdDivide { get; set; }

    public int AINewMilitaryPersonThresholdDivide { get; set; }

    public int AIExecuteMaxUncreulty { get; set; }

    public float AIExecutePersonIdealToleranceMultiply { get; set; }

    public int FireStayProb { get; set; }

    public float FireSpreadProbMultiply { get; set; }

    public int MinPregnantProb { get; set; }

    public float InternalExperienceRate { get; set; }

    public float AbilityExperienceRate { get; set; }

    public float ArmyExperienceRate { get; set; }

    public float AIAttackChanceIfUnfull { get; set; }

    public int AIObeyStrategyTendencyChance { get; set; }

    public int AIOffendMaxDiplomaticRelationMultiply { get; set; }

    public float AIOffendDefendTroopAdd { get; set; }

    public float AIOffendDefendTroopMultiply { get; set; }

    public int AIOffendIgnoreReserveProbAmbitionMultiply { get; set; }

    public int AIOffendIgnoreReserveProbAmbitionAdd { get; set; }

    public int AIOffendIgnoreReserveProbBCDiffMultiply { get; set; }

    public int AIOffendIgnoreReserveProbBCDiffAdd { get; set; }

    public float AIOffendIgnoreReserveChanceTroopRatioAdd { get; set; }

    public float AIOffendIgnoreReserveChanceTroopRatioMultiply { get; set; }

    public int PrincessMaintainenceCost { get; set; }

    public int AIUniqueTroopFightingForceThreshold { get; set; }

    public int LearnSkillSuccessRate { get; set; }

    public int LearnStuntSuccessRate { get; set; }

    public int LearnTitleSuccessRate { get; set; }

    public int AutoLearnSkillSuccessRate { get; set; }

    public int AutoLearnStuntSuccessRate { get; set; }

    public float MilitaryPopulationCap { get; set; }

    public float MilitaryPopulationReloadQuantity { get; set; }

    public int CloseThreshold { get; set; }

    public int HateThreshold { get; set; }

    public int VeryCloseThreshold { get; set; }

    public int MaxAITroopCountCandidates { get; set; }

    public float PopulationDevelopingRate { get; set; }

    public float CloseAbilityRate { get; set; }

    public float VeryCloseAbilityRate { get; set; }

    public int AIEncirclePlayerRate { get; set; }

    public float BasicAIExtraPerson { get; set; }

    public float AIExtraPerson { get; set; }

    public float AIExtraPersonIncreaseRate { get; set; }

    public int AITirednessDecrease { get; set; }

    public int InternalSurplusFactor { get; set; }

    public int MakeMarrigeIdealLimit { get; set; }

    public int MakeMarriageCost { get; set; }

    public int NafeiCost { get; set; }

    public int SelectPrinceCost { get; set; }

    public int TransferCostPerMilitary { get; set; }

    public int TransferFoodPerMilitary { get; set; }

    public int AIEncircleRank { get; set; }

    public int AIEncircleVar { get; set; }

    public float RansomRate { get; set; }

    public List<int> ExpandConditions { get; set; }

    public float SearchPersonArchitectureCountPower { get; set; }

    public int DayInTurn { get; set; }

    public int MaxRelation { get; set; }

    public float HougongRelationHateFactor { get; set; }

    public int AIMaxFeizi { get; set; }

    public int MaxReputationForRecruit { get; set; }

    public float TroopMoraleChange { get; set; }

    public float RecruitPopualationDecreaseRate { get; set; }

    public float AIOffensiveCampaignRequiredScaleFactor { get; set; }

    public int PersonCap { get; set; }

    public float PersonCapFalldown { get; set; }

    public int AlienTroopGain { get; set; }

    public int TrainAbilityCost { get; set; }

    public int TrainAbilityTiredness { get; set; }

    public int TrainAbilityAmount { get; set; }

    public int OfficerBaseSalary { get; set; }

    public float SalaryLoyaltyLoss { get; set; }
}