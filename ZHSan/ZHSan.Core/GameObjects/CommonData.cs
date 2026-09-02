using GameDatas;
using GameEnums;
using GameObjects.Animations;
using GameObjects.ArchitectureDetail;
using GameObjects.Conditions;
using GameObjects.FactionDetail;
using GameObjects.Influences;
using GameObjects.MapDetail;
using GameObjects.PersonDetail;
using GameObjects.SectionDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameObjects
{
    public class CommonData
    {
        private static ILogger logger;

        public static CommonData Current = null;

        public static bool CurrentReady = false;

        /// <summary>
        /// 建筑类型
        /// </summary>
        public Dictionary<int, ArchitectureKind> AllArchitectureKinds { get; set; } = new();
        
        /// <summary>
        /// 攻击默认类型
        /// </summary>
        public List<AttackDefaultKind> AllAttackDefaultKinds { get; set; } = new();

        /// <summary>
        /// 攻击目标类型
        /// </summary>
        public List<AttackTargetKind> AllAttackTargetKinds { get; set; } = new();

        /// <summary>
        /// 施法默认类型
        /// </summary>
        public List<CastDefaultKind> AllCastDefaultKinds { get; set; } = new();
        
        /// <summary>
        /// 施法目标类型
        /// </summary>
        public List<CastTargetKind> AllCastTargetKinds { get; set; } = new();

        /// <summary>
        /// 性格种类
        /// </summary>
        public Dictionary<int, CharacterKind> AllCharacterKinds { get; set; } = new();

        /// <summary>
        /// 颜色
        /// </summary>
        public List<Color> AllColors { get; set; } = new();
        
        /// <summary>
        /// 战法
        /// </summary>
        public Dictionary<int, CombatMethod> AllCombatMethods { get; set; } = new();

        /// <summary>
        /// 条件种类
        /// </summary>
        public Dictionary<int, ConditionKind> AllConditionKinds { get; set; } = new();

        /// <summary>
        /// 条件
        /// </summary>
        public Dictionary<int, Condition> AllConditions { get; set; } = new();

        /// <summary>
        /// 设施种类
        /// </summary>
        public Dictionary<int, FacilityKind> AllFacilityKinds { get; set; } = new();

        /// <summary>
        /// 设施种类等级
        /// </summary>
        public Dictionary<int, List<FacilityKindLevel>> GroupedFacilityKindLevels { get; set; } = new();
        
        public Dictionary<int, FacilityKindLevel> AllFacilityKindLevels { get; set; } = new();

        /// <summary>
        /// 灾难种类
        /// </summary>
        public Dictionary<int, DisasterKind> AllDisasterKinds { get; set; } = new();

        /// <summary>
        /// 官爵种类
        /// </summary>
        public Dictionary<int, OfficialTitleKind> AllOfficialTitleKinds { get; set; } = new();

        public Dictionary<int, IdealTendencyKind> AllIdealTendencyKinds { get; set; } = new();

        /// <summary>
        /// 影响种类
        /// </summary>
        public Dictionary<int, InfluenceKind> AllInfluenceKinds { get; set; } = new();

        /// <summary>
        /// 影响
        /// </summary>
        public Dictionary<int, Influence> AllInfluences { get; set; } = new();

        /// <summary>
        /// 情报种类
        /// </summary>
        public Dictionary<int, InformationKind> AllInformationKinds { get; set; } = new();

        /// <summary>
        /// 兵种种类
        /// </summary>
        public Dictionary<int, MilitaryKind> AllMilitaryKinds { get; set; } = new();

        /// <summary>
        /// 军区AI详情
        /// </summary>
        public Dictionary<int, SectionAIDetail> AllSectionAIDetails { get; set; } = new();

        /// <summary>
        /// 技能
        /// </summary>
        public Dictionary<int, Skill> AllSkills { get; set; } = new();

        /// <summary>
        /// 计略
        /// </summary>
        public Dictionary<int, Stratagem> AllStratagems { get; set; } = new();

        /// <summary>
        /// 特技
        /// </summary>
        public Dictionary<int, Stunt> AllStunts { get; set; } = new();

        /// <summary>
        /// 科技
        /// </summary>
        public Dictionary<int, Technique> AllTechniques { get; set; } = new();

        /// <summary>
        /// 地形
        /// </summary>
        public Dictionary<int, TerrainDetail> AllTerrainDetails { get; set; } = new();

        /// <summary>
        /// 人物个性语言
        /// </summary>
        public Dictionary<(int kindId, TextMessageKind kind), List<string>> AllTextMessages { get; set; } = new();

        /// <summary>
        /// 瓦片动画
        /// </summary>
        public Dictionary<int, Animation> AllTileAnimations { get; set; } = new();

        /// <summary>
        /// 称号种类
        /// </summary>
        public Dictionary<int, TitleKind> AllTitleKinds { get; set; } = new();

        /// <summary>
        /// 称号
        /// </summary>
        public Dictionary<int, Title> AllTitles { get; set; } = new();

        // public GuanzhiTable AllGuanzhis = new GuanzhiTable();
        //public GuanzhiKindTable AllGuanzhiKinds = new GuanzhiKindTable();

        /// <summary>
        /// 部队动画
        /// </summary>
        public Dictionary<int, Animation> AllTroopAnimations { get; set; } = new();

        /// <summary>
        /// 部队事件影响种类
        /// </summary>
        public Dictionary<int, TroopDetail.EventEffect.EventEffectKind> AllTroopEventEffectKinds { get; set; } = new();

        /// <summary>
        /// 部队事件影响
        /// </summary>
        public Dictionary<int, TroopDetail.EventEffect.EventEffect> AllTroopEventEffects { get; set; } = new();

        /// <summary>
        /// 事件影响种类
        /// </summary>
        public Dictionary<int, ArchitectureDetail.EventEffect.EventEffectKind> AllEventEffectKinds { get; set; } = new();

        /// <summary>
        /// 事件影响
        /// </summary>
        public Dictionary<int, ArchitectureDetail.EventEffect.EventEffect> AllEventEffects { get; set; } = new();

        public List<BiographyAdjectives> AllBiographyAdjectives { get; set; } = new();

        public PersonGeneratorSetting PersonGeneratorSetting  { get; set; } = new();

        public Dictionary<int, PersonGeneratorType> AllPersonGeneratorTypes { get; set; } = new();

        /// <summary>
        /// 培育方针
        /// </summary>
        public Dictionary<int, TrainPolicy> AllTrainPolicies { get; set; } = new();
        
        public Dictionary<int, TreasureCreationSetting> AllTreasureCreationSettings { get; set; } = new();

        public CombatNumberGenerator NumberGenerator = new CombatNumberGenerator();

        public TroopAnimation TroopAnimations = new TroopAnimation();

        /// <summary>
        /// 状态效果
        /// </summary>
        public Dictionary<int, StatusEffect> AllStatusEffects { get; set; } = new();

        public CommonData Clone()
        {
            var commonData = this.MemberwiseClone() as CommonData;
            return commonData;
        }

        /// <summary>
        /// CommonData初始化
        /// </summary>
        public static void Init()
        {
            logger = Log.ForContext<CommonData>();

            try
            {
                Current = new CommonData();

                using var archive = GameDataArchive.Open(@"Content\Data\Common\CommonData.dat");

                var terrainDetails = archive.Load<List<TerrainDetailConfig>>("TerrainDetails.json");
                var combatMethods = archive.Load<List<CombatMethodConfig>>("CombatMethods.json");
                var stunts = archive.Load<List<StuntConfig>>("Stunts.json");
                var techniques = archive.Load<List<TechniqueConfig>>("Techniques.json");
                var skills = archive.Load<List<SkillConfig>>("Skills.json");
                var stratagems = archive.Load<List<StratagemConfig>>("Stratagems.json");
                var titleKinds = archive.Load<List<TitleKindConfig>>("TitleKinds.json");
                var titles = archive.Load<List<TitleConfig>>("Titles.json");
                var influenceKinds = archive.Load<List<InfluenceKindConfig>>("InfluenceKinds.json");
                var influences = archive.Load<List<InfluenceConfig>>("Influences.json");
                var conditionKinds = archive.Load<List<ConditionKindConfig>>("ConditionKinds.json");
                var conditions = archive.Load<List<ConditionConfig>>("Conditions.json");
                var architectureEventEffectKinds = archive.Load<List<ArchitectureEventEffectKindConfig>>("ArchitectureEventEffectKinds.json");
                var architectureEventEffects = archive.Load<List<ArchitectureEventEffectConfig>>("ArchitectureEventEffects.json");
                var troopEventEffectKinds = archive.Load<List<TroopEventEffectKindConfig>>("TroopEventEffectKinds.json");
                var troopEventEffects = archive.Load<List<TroopEventEffectConfig>>("TroopEventEffects.json");
                var informationKinds = archive.Load<List<InformationKindConfig>>("InformationKinds.json");
                var characterKinds = archive.Load<List<CharacterKindConfig>>("CharacterKinds.json");
                var colors = archive.Load<List<Color>>("Colors.json");
                var facilityKinds = archive.Load<List<FacilityKindConfig>>("FacilityKinds.json");
                var facilityKindLevels = archive.Load<List<FacilityKindLevelConfig>>("FacilityKindLevels.json");
                var disasterKinds = archive.Load<List<DisasterKindConfig>>("DisasterKinds.json");
                var officialTitleKinds = archive.Load<List<OfficialTitleKindConfig>>("OfficialTitleKinds.json");
                var sectionAIDetails = archive.Load<List<SectionAIDetailConfig>>("SectionAIDetails.json");
                var idealTendencyKinds = archive.Load<List<IdealTendencyKindConfig>>("IdealTendencyKinds.json");
                var militaryKinds = archive.Load<List<MilitaryKindConfig>>("MilitaryKinds.json");
                var architectureKinds = archive.Load<List<ArchitectureKindConfig>>("ArchitectureKinds.json");
                var personMessages = archive.Load<List<PersonMessageConfig>>("PersonMessages.json");
                var tileAnimations = archive.Load<List<AnimationConfig>>("TileAnimations.json");
                var troopAnimations = archive.Load<List<AnimationConfig>>("TroopAnimations.json");
                var biographyAdjectives = archive.Load<List<BiographyAdjectiveConfig>>("BiographyAdjectives.json");
                var personGeneratorTypes = archive.Load<List<PersonGeneratorTypeConfig>>("PersonGeneratorTypes.json");
                var trainPolicies = archive.Load<List<TrainPolicyConfig>>("TrainPolicies.json");
                var personGeneratorSettings = archive.Load<List<PersonGeneratorSettingConfig>>("PersonGeneratorSettings.json");
                var treasureCreationSettings = archive.Load<List<TreasureCreationSettingConfig>>("TreasureCreationSettings.json");
                var attackDefaultKinds = archive.Load<List<AttackDefaultKindConfig>>("AttackDefaultKinds.json");
                var attackTargetKinds = archive.Load<List<AttackTargetKindConfig>>("AttackTargetKinds.json");
                var castDefaultKinds = archive.Load<List<CastDefaultKindConfig>>("CastDefaultKinds.json");
                var castTargetKinds = archive.Load<List<CastTargetKindConfig>>("CastTargetKinds.json");
                var statusEffects = archive.Load<List<StatusEffectConfig>>("StatusEffects.json");

                Current.AllTerrainDetails = terrainDetails.Select(x => new TerrainDetail(x)).ToDictionary(x => x.ID);
                Current.AllCombatMethods = combatMethods.Select(x => new CombatMethod(x)).ToDictionary(x => x.ID);
                Current.AllStunts = stunts.Select(x => new Stunt(x)).ToDictionary(x => x.ID);
                Current.AllTechniques = techniques.Select(x => new Technique(x)).ToDictionary(x => x.ID);
                Current.AllSkills = skills.Select(x => new Skill(x)).ToDictionary(x => x.ID);
                Current.AllStratagems = stratagems.Select(x => new Stratagem(x)).ToDictionary(x => x.ID);
                Current.AllTitleKinds = titleKinds.Select(x => new TitleKind(x)).ToDictionary(x => x.ID);
                Current.AllTitles = titles.Select(x => new Title(x)).ToDictionary(x => x.ID);
                Current.AllInfluenceKinds = influenceKinds.Select(x => CreateInfluenceKind(x)).Where(x => x != null).ToDictionary(x => x.ID);
                Current.AllInfluences = influences.Select(x => new Influence(x)).ToDictionary(x => x.ID);
                Current.AllConditionKinds = conditionKinds.Select(x => CreateConditionKind(x)).Where(x => x != null).ToDictionary(x => x.ID);
                Current.AllConditions = conditions.Select(x => new Condition(x)).ToDictionary(x => x.ID);
                Current.AllEventEffectKinds = architectureEventEffectKinds.Select(x => CreateArchitectureEventEffectKind(x)).Where(x => x != null).ToDictionary(x => x.ID);
                Current.AllEventEffects = architectureEventEffects.Select(x => new ArchitectureDetail.EventEffect.EventEffect(x)).ToDictionary(x => x.ID);
                Current.AllTroopEventEffectKinds = troopEventEffectKinds.Select(x => CreateTroopEventEffectKind(x)).Where(x => x != null).ToDictionary(x => x.ID);
                Current.AllTroopEventEffects = troopEventEffects.Select(x => new TroopDetail.EventEffect.EventEffect(x)).ToDictionary(x => x.ID);
                Current.AllInformationKinds = informationKinds.Select(x => new InformationKind(x)).ToDictionary(x => x.ID);
                Current.AllCharacterKinds = characterKinds.Select(x => new CharacterKind(x)).ToDictionary(x => x.ID);
                Current.AllColors = colors;
                Current.AllFacilityKinds = facilityKinds.Select(x => new FacilityKind(x)).ToDictionary(x => x.ID);
                Current.AllFacilityKindLevels = facilityKindLevels.Select(x => new FacilityKindLevel(x)).ToDictionary(x => x.Id);
                Current.AllDisasterKinds = disasterKinds.Select(x => new DisasterKind(x)).ToDictionary(x => x.ID);
                Current.AllOfficialTitleKinds = officialTitleKinds.Select(x => new OfficialTitleKind(x)).ToDictionary(x => x.ID);
                Current.AllSectionAIDetails = sectionAIDetails.Select(x => new SectionAIDetail(x)).ToDictionary(x => x.ID);
                Current.AllIdealTendencyKinds = idealTendencyKinds.Select(x => new IdealTendencyKind(x)).ToDictionary(x => x.ID);
                Current.AllMilitaryKinds = militaryKinds.Select(x => new MilitaryKind(x)).ToDictionary(x => x.ID);
                Current.AllArchitectureKinds = architectureKinds.Select(x => new ArchitectureKind(x)).ToDictionary(x => x.ID);
                Current.AllTextMessages = personMessages.ToDictionary(x => (x.PersonId, x.Kind), x => x.Messages);
                Current.AllTileAnimations = tileAnimations.Select(x => new Animation(x)).ToDictionary(x => x.ID);
                Current.AllTroopAnimations = troopAnimations.Select(x => new Animation(x)).ToDictionary(x => x.ID);
                Current.AllBiographyAdjectives = biographyAdjectives.Select(x => new BiographyAdjectives(x)).ToList();
                Current.AllPersonGeneratorTypes = personGeneratorTypes.Select(x => new PersonGeneratorType(x)).ToDictionary(x => x.ID);
                Current.AllTrainPolicies = trainPolicies.Select(x => new TrainPolicy(x)).ToDictionary(x => x.ID);
                Current.PersonGeneratorSetting = new PersonGeneratorSetting(personGeneratorSettings.FirstOrDefault());
                Current.AllTreasureCreationSettings = treasureCreationSettings.Select(x => new TreasureCreationSetting(x)).ToDictionary(x => x.ID);
                Current.AllAttackDefaultKinds = attackDefaultKinds.Select(x => new AttackDefaultKind(x)).ToList();
                Current.AllAttackTargetKinds = attackDefaultKinds.Select(x => new AttackTargetKind(x)).ToList();
                Current.AllCastDefaultKinds = castDefaultKinds.Select(x => new CastDefaultKind(x)).ToList();
                Current.AllCastTargetKinds = castTargetKinds.Select(x => new CastTargetKind(x)).ToList();
                Current.AllStatusEffects = statusEffects.Select(x => new StatusEffect(x)).ToDictionary(x => x.ID);

                GameScenario.ProcessCommonData(Current);

                CurrentReady = true;
            }
            catch (Exception ex)
            {
                throw new Exception("CommonData初始化失敗:" + ex);
            }

            // new PlatformTask(() =>
            // {
                
            // }).Start();
        }

        public static InfluenceKind CreateInfluenceKind(InfluenceKindConfig config)
        {
            var id = config.Id;
            var kind = InfluenceKindFactory.CreateInfluenceKindByID(id);
            if (kind == null)
            {
                logger.Error($"影响类型Id:[{id}]不存在");
                return null;
            }

            kind.ID = config.Id;
            kind.Name = config.Name;
            kind.Type = config.Type;
            kind.Combat = config.Combat;
            kind.AIPersonValue = config.AIPersonValue;
            kind.AIPersonValuePow = config.AIPersonValuePow;
            kind.TroopLeaderValid = config.TroopLeaderValid;

            return kind;
        }

        public static ConditionKind CreateConditionKind(ConditionKindConfig config)
        {
            var id = config.Id;
            var kind = ConditionKindFactory.CreateConditionKindByID(id);
            if (kind == null)
            {
                logger.Error($"条件类型Id:[{id}]不存在");
                return null;
            }

            kind.ID = config.Id;
            kind.Name = config.Name;

            return kind;
        }

        public static ArchitectureDetail.EventEffect.EventEffectKind CreateArchitectureEventEffectKind(ArchitectureEventEffectKindConfig config)
        {
            var id = config.Id;
            var kind = ArchitectureDetail.EventEffect.EventEffectKindFactory.CreateEventEffectKindByID(id);
            if (kind == null)
            {
                logger.Error($"建筑事件影响种类Id:[{id}]不存在");
                return null;
            }

            kind.ID = config.Id;
            kind.Name = config.Name;

            return kind;
        }

        public static TroopDetail.EventEffect.EventEffectKind CreateTroopEventEffectKind(TroopEventEffectKindConfig config)
        {
            var id = config.Id;
            var kind = TroopDetail.EventEffect.EventEffectKindFactory.CreateEventEffectKindByID(id);
            if (kind == null)
            {
                logger.Error($"部队事件影响种类Id:[{id}]不存在");
                return null;
            }

            kind.ID = config.Id;
            kind.Name = config.Name;

            return kind;
        }

        public static List<string> GetPersonMessage(int personId, TextMessageKind kind)
        {
            if (Current.AllTextMessages.TryGetValue((personId, kind), out var messages))
            {
                return messages;
            }

            return new List<string>();
        }

        public static List<SectionAIDetail> GetSectionAIDetailsByConditions(SectionOrientationKind orientationKind, bool autoRun, bool valueOffensiveCampaign, bool allowOffensiveCampaign, bool allowMilitaryTransfer, bool valueRecruitment)
        {
            var result = new List<SectionAIDetail>();

            foreach (var detail in Current.AllSectionAIDetails.Values)
            {
                if (detail.OrientationKind == orientationKind 
                    && detail.AutoRun == autoRun 
                    && detail.ValueOffensiveCampaign == valueOffensiveCampaign 
                    && detail.AllowOffensiveCampaign == allowOffensiveCampaign 
                    && detail.AllowMilitaryTransfer == allowMilitaryTransfer 
                    && detail.ValueRecruitment == valueRecruitment)
                {
                    result.Add(detail);
                }
            }
            
            return result;
        }

        public static List<SectionAIDetail> GetSectionNoOrientationAutoAIDetailsByConditions(bool allowOffensiveCampaign, bool valueRecruitment)
        {
            var result = new List<SectionAIDetail>();

            foreach (var detail in Current.AllSectionAIDetails.Values)
            {
                if (detail.OrientationKind == SectionOrientationKind.None 
                    && detail.AutoRun 
                    && detail.AllowOffensiveCampaign == allowOffensiveCampaign 
                    && detail.ValueRecruitment == valueRecruitment)
                {
                    result.Add(detail);
                }
            }

            return result;
        }
    }
}