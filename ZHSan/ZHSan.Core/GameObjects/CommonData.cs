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
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
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
        [DataMember]
        public FacilityKindTable AllFacilityKinds = new FacilityKindTable();

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
            logger = Log.ForContext<GameScenario>();

            try
            {
                var dirPath = @"Content\Data\Common";

                Current = Tools.SimpleSerializer.DeserializeJsonFile<CommonData>(Path.Combine(dirPath, "CommonData.json"), false, false);

                var terrainDetailStore = new JsonStore<TerrainDetailConfig>(Path.Combine(dirPath, "TerrainDetails.json"));
                var terrainDetails = terrainDetailStore.Load();
                Current.AllTerrainDetails = terrainDetails.Select(x => new TerrainDetail(x)).ToDictionary(x => x.ID);

                var combatMethodStore = new JsonStore<CombatMethodConfig>(Path.Combine(dirPath, "CombatMethods.json"));
                var combatMethods = combatMethodStore.Load();
                Current.AllCombatMethods = combatMethods.Select(x => new CombatMethod(x)).ToDictionary(x => x.ID);

                var stuntStore = new JsonStore<StuntConfig>(Path.Combine(dirPath, "Stunts.json"));
                var stunts = stuntStore.Load();
                Current.AllStunts = stunts.Select(x => new Stunt(x)).ToDictionary(x => x.ID);

                var techniqueStore = new JsonStore<TechniqueConfig>(Path.Combine(dirPath, "Techniques.json"));
                var techniques = techniqueStore.Load();
                Current.AllTechniques = techniques.Select(x => new Technique(x)).ToDictionary(x => x.ID);

                var skillStore = new JsonStore<SkillConfig>(Path.Combine(dirPath, "Skills.json"));
                var skills = skillStore.Load();
                Current.AllSkills = skills.Select(x => new Skill(x)).ToDictionary(x => x.ID);

                var stratagemStore = new JsonStore<StratagemConfig>(Path.Combine(dirPath, "Stratagems.json"));
                var stratagems = stratagemStore.Load();
                Current.AllStratagems = stratagems.Select(x => new Stratagem(x)).ToDictionary(x => x.ID);

                var titleKindStore = new JsonStore<TitleKindConfig>(Path.Combine(dirPath, "TitleKinds.json"));
                var titleKinds = titleKindStore.Load();
                Current.AllTitleKinds = titleKinds.Select(x => new TitleKind(x)).ToDictionary(x => x.ID);

                var titleStore = new JsonStore<TitleConfig>(Path.Combine(dirPath, "Titles.json"));
                var titles = titleStore.Load();
                Current.AllTitles = titles.Select(x => new Title(x)).ToDictionary(x => x.ID);

                var influenceKindStore = new JsonStore<InfluenceKindConfig>(Path.Combine(dirPath, "InfluenceKinds.json"));
                var influenceKinds = influenceKindStore.Load();
                Current.AllInfluenceKinds = influenceKinds.Select(x => CreateInfluenceKind(x)).Where(x => x != null).ToDictionary(x => x.ID);

                var influenceStore = new JsonStore<InfluenceConfig>(Path.Combine(dirPath, "Influences.json"));
                var influences = influenceStore.Load();
                Current.AllInfluences = influences.Select(x => new Influence(x)).ToDictionary(x => x.ID);

                var conditionKindStore = new JsonStore<ConditionKindConfig>(Path.Combine(dirPath, "ConditionKinds.json"));
                var conditionKinds = conditionKindStore.Load();
                Current.AllConditionKinds = conditionKinds.Select(x => CreateConditionKind(x)).Where(x => x != null).ToDictionary(x => x.ID);

                var conditionStore = new JsonStore<ConditionConfig>(Path.Combine(dirPath, "Conditions.json"));
                var conditions = conditionStore.Load();
                Current.AllConditions = conditions.Select(x => new Condition(x)).ToDictionary(x => x.ID);

                var architectureEventEffectKindStore = new JsonStore<ArchitectureEventEffectKindConfig>(Path.Combine(dirPath, "ArchitectureEventEffectKinds.json"));
                var architectureEventEffectKinds = architectureEventEffectKindStore.Load();
                Current.AllEventEffectKinds = architectureEventEffectKinds.Select(x => CreateArchitectureEventEffectKind(x)).Where(x => x != null).ToDictionary(x => x.ID);

                var architectureEventEffectStore = new JsonStore<ArchitectureEventEffectConfig>(Path.Combine(dirPath, "ArchitectureEventEffects.json"));
                var architectureEventEffects = architectureEventEffectStore.Load();
                Current.AllEventEffects = architectureEventEffects.Select(x => new ArchitectureDetail.EventEffect.EventEffect(x)).ToDictionary(x => x.ID);

                var troopEventEffectKindStore = new JsonStore<TroopEventEffectKindConfig>(Path.Combine(dirPath, "TroopEventEffectKinds.json"));
                var troopEventEffectKinds = troopEventEffectKindStore.Load();
                Current.AllTroopEventEffectKinds = troopEventEffectKinds.Select(x => CreateTroopEventEffectKind(x)).Where(x => x != null).ToDictionary(x => x.ID);

                var troopEventEffectStore = new JsonStore<TroopEventEffectConfig>(Path.Combine(dirPath, "TroopEventEffects.json"));
                var troopEventEffects = troopEventEffectStore.Load();
                Current.AllTroopEventEffects = troopEventEffects.Select(x => new TroopDetail.EventEffect.EventEffect(x)).ToDictionary(x => x.ID);

                var informationKindStore = new JsonStore<InformationKindConfig>(Path.Combine(dirPath, "InformationKinds.json"));
                var informationKinds = informationKindStore.Load();
                Current.AllInformationKinds = informationKinds.Select(x => new InformationKind(x)).ToDictionary(x => x.ID);

                var characterKindStore = new JsonStore<CharacterKindConfig>(Path.Combine(dirPath, "CharacterKinds.json"));
                var characterKinds = characterKindStore.Load();
                Current.AllCharacterKinds = characterKinds.Select(x => new CharacterKind(x)).ToDictionary(x => x.ID);

                var colorStore = new JsonStore<Color>(Path.Combine(dirPath, "Colors.json"));
                Current.AllColors = colorStore.Load();

                // var facilityKindStore = new JsonStore<FacilityKindConfig>(Path.Combine(dirPath, "FacilityKinds.json"));
                // var facilityKinds = facilityKindStore.Load();
                // Current.AllFacilityKinds = facilityKinds.Select(x => new FacilityKind(x)).ToDictionary(x => x.ID);

                var disasterKindStore = new JsonStore<DisasterKindConfig>(Path.Combine(dirPath, "DisasterKinds.json"));
                var disasterKinds = disasterKindStore.Load();
                Current.AllDisasterKinds = disasterKinds.Select(x => new DisasterKind(x)).ToDictionary(x => x.ID);

                var officialTitleKindStore = new JsonStore<OfficialTitleKindConfig>(Path.Combine(dirPath, "OfficialTitleKinds.json"));
                var officialTitleKinds = officialTitleKindStore.Load();
                Current.AllOfficialTitleKinds = officialTitleKinds.Select(x => new OfficialTitleKind(x)).ToDictionary(x => x.ID);
                
                var sectionAIDetailStore = new JsonStore<SectionAIDetailConfig>(Path.Combine(dirPath, "SectionAIDetails.json"));
                var sectionAIDetails = sectionAIDetailStore.Load();
                Current.AllSectionAIDetails = sectionAIDetails.Select(x => new SectionAIDetail(x)).ToDictionary(x => x.ID);

                var idealTendencyKindStore = new JsonStore<IdealTendencyKindConfig>(Path.Combine(dirPath, "IdealTendencyKinds.json"));
                var idealTendencyKinds = idealTendencyKindStore.Load();
                Current.AllIdealTendencyKinds = idealTendencyKinds.Select(x => new IdealTendencyKind(x)).ToDictionary(x => x.ID);
                
                var militaryKindStore = new JsonStore<MilitaryKindConfig>(Path.Combine(dirPath, "MilitaryKinds.json"));
                var militaryKinds = militaryKindStore.Load();
                Current.AllMilitaryKinds = militaryKinds.Select(x => new MilitaryKind(x)).ToDictionary(x => x.ID);

                var architectureKindStore = new JsonStore<ArchitectureKindConfig>(Path.Combine(dirPath, "ArchitectureKinds.json"));
                var architectureKinds = architectureKindStore.Load();
                Current.AllArchitectureKinds = architectureKinds.Select(x => new ArchitectureKind(x)).ToDictionary(x => x.ID);

                var personMessageStore = new JsonStore<PersonMessageConfig>(Path.Combine(dirPath, "PersonMessages.json"));
                var personMessages = personMessageStore.Load();
                Current.AllTextMessages = personMessages.ToDictionary(x => (x.PersonId, x.Kind), x => x.Messages);

                var tileAnimationStore = new JsonStore<AnimationConfig>(Path.Combine(dirPath, "TileAnimations.json"));
                var tileAnimations = tileAnimationStore.Load();
                Current.AllTileAnimations = tileAnimations.Select(x => new Animation(x)).ToDictionary(x => x.ID);

                var troopAnimationStore = new JsonStore<AnimationConfig>(Path.Combine(dirPath, "TroopAnimations.json"));
                var troopAnimations = troopAnimationStore.Load();
                Current.AllTroopAnimations = troopAnimations.Select(x => new Animation(x)).ToDictionary(x => x.ID);

                var biographyAdjectiveStore = new JsonStore<BiographyAdjectiveConfig>(Path.Combine(dirPath, "BiographyAdjectives.json"));
                var biographyAdjectives = biographyAdjectiveStore.Load();
                Current.AllBiographyAdjectives = biographyAdjectives.Select(x => new BiographyAdjectives(x)).ToList();

                var personGeneratorTypeStore = new JsonStore<PersonGeneratorTypeConfig>(Path.Combine(dirPath, "PersonGeneratorTypes.json"));
                var personGeneratorTypes = personGeneratorTypeStore.Load();
                Current.AllPersonGeneratorTypes = personGeneratorTypes.Select(x => new PersonGeneratorType(x)).ToDictionary(x => x.ID);

                var trainPolicyStore = new JsonStore<TrainPolicyConfig>(Path.Combine(dirPath, "TrainPolicies.json"));
                var trainPolicies = trainPolicyStore.Load();
                Current.AllTrainPolicies = trainPolicies.Select(x => new TrainPolicy(x)).ToDictionary(x => x.ID);

                var personGeneratorSettingStore = new JsonStore<PersonGeneratorSettingConfig>(Path.Combine(dirPath, "PersonGeneratorSettings.json"));
                var personGeneratorSettings = personGeneratorSettingStore.Load();
                Current.PersonGeneratorSetting = new PersonGeneratorSetting(personGeneratorSettings.FirstOrDefault());

                var treasureCreationSettingStore = new JsonStore<TreasureCreationSettingConfig>(Path.Combine(dirPath, "TreasureCreationSettings.json"));
                var treasureCreationSettings = treasureCreationSettingStore.Load();
                Current.AllTreasureCreationSettings = treasureCreationSettings.Select(x => new TreasureCreationSetting(x)).ToDictionary(x => x.ID);

                var attackDefaultKindStore = new JsonStore<AttackDefaultKindConfig>(Path.Combine(dirPath, "AttackDefaultKinds.json"));
                var attackDefaultKinds = attackDefaultKindStore.Load();
                Current.AllAttackDefaultKinds = attackDefaultKinds.Select(x => new AttackDefaultKind(x)).ToList();

                var attackTargetKindStore = new JsonStore<AttackTargetKindConfig>(Path.Combine(dirPath, "AttackTargetKinds.json"));
                var attackTargetKinds = attackDefaultKindStore.Load();
                Current.AllAttackTargetKinds = attackDefaultKinds.Select(x => new AttackTargetKind(x)).ToList();

                var castDefaultKindStore = new JsonStore<CastDefaultKindConfig>(Path.Combine(dirPath, "CastDefaultKinds.json"));
                var castDefaultKinds = castDefaultKindStore.Load();
                Current.AllCastDefaultKinds = castDefaultKinds.Select(x => new CastDefaultKind(x)).ToList();

                var castTargetKindStore = new JsonStore<CastTargetKindConfig>(Path.Combine(dirPath, "CastTargetKinds.json"));
                var castTargetKinds = castTargetKindStore.Load();
                Current.AllCastTargetKinds = castTargetKinds.Select(x => new CastTargetKind(x)).ToList();

                var statusEffectStore = new JsonStore<StatusEffectConfig>(Path.Combine(dirPath, "StatusEffects.json"));
                var statusEffects = statusEffectStore.Load();
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