using GameObjects.Influences;
using GameObjects.Conditions;
using System.Collections.Generic;
using GameManager;
using Microsoft.Xna.Framework;
using GameGlobal;
using GameEnums;
using GameDatas;
using System.Linq;
using Extensions;

namespace GameObjects.TroopDetail;

/// <summary>
/// 军队类型
/// </summary>
public class MilitaryKind : GameObject
{
    /// <summary>
    /// 类别
    /// </summary>
    public MilitaryType Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 强度（AI）
    /// </summary>
    public int Merit { get; set; }

    /// <summary>
    /// 较强兵种ID：如果AI准备征召这个兵种的话，会考虑征召在这列表中的兵种，而这列表的兵种是绝对强于这个兵种
    /// </summary>
    public string SuccessorString { get; set; }

    /// <summary>
    /// 行动速率：行动速率高的部队将优先行动。行动速率＝兵种本身的行动速率×士气÷士气上限
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// 获得机率：每天有1除以此数的机率，拥有武将的势力可获得这个兵种
    /// </summary>
    public int ObtainProb { get; set; }

    /// <summary>
    /// 出兵称号影响
    /// </summary>
    public int TitleInfluence { get; set; } = -1;

    /// <summary>
    /// 新建资金
    /// </summary>
    public int CreateCost { get; set; }

    /// <summary>
    /// 新建所需技术
    /// </summary>
    public int CreateTechnology { get; set; }

    /// <summary>
    /// 水边新建
    /// </summary>
    public bool CreateBesideWater { get; set; }

    /// <summary>
    /// 攻击
    /// </summary>
    public int Offence { get; set; }

    /// <summary>
    /// 防御
    /// </summary>
    public int Defence { get; set; }

    /// <summary>
    /// 攻击半径
    /// </summary>
    public int OffenceRadius { get; set; }

    /// <summary>
    /// 能否反击
    /// </summary>
    public bool CounterOffence { get; set; }

    /// <summary>
    /// 能否被反击
    /// </summary>
    public bool BeCountered { get; set; }

    /// <summary>
    /// 斜向攻击
    /// </summary>
    public bool ObliqueOffence { get; set; }

    /// <summary>
    /// 箭矢攻击：弓箭攻击，投石车等部队不属于弓箭攻击
    /// </summary>
    public bool ArrowOffence { get; set; }

    /// <summary>
    /// 凌空攻击：是否可以攻击建筑内的部队
    /// </summary>
    public bool AirOffence { get; set; }

    /// <summary>
    /// 近身攻击
    /// </summary>
    public bool ContactOffence { get; set; }

    /// <summary>
    /// 建筑伤害系数
    /// </summary>
    public float ArchitectureDamageRate { get; set; }

    /// <summary>
    /// 建筑反击承受率
    /// </summary>
    public float ArchitectureCounterDamageRate { get; set; }

    /// <summary>
    /// 计略范围
    /// </summary>
    public int StratagemRadius { get; set; }

    /// <summary>
    /// 斜向计略
    /// </summary>
    public bool ObliqueStratagem { get; set; }

    /// <summary>
    /// 视野半径
    /// </summary>
    public int ViewRadius { get; set; }

    /// <summary>
    /// 斜向视野
    /// </summary>
    public bool ObliqueView { get; set; }

    /// <summary>
    /// 伤兵概率
    /// </summary>
    public int InjuryChance { get; set; }

    /// <summary>
    /// 行动力
    /// </summary>
    public int Movability { get; set; }

    /// <summary>
    /// 单一适性种类
    /// </summary>
    public int OneAdaptabilityKind { get; set; }

    /// <summary>
    /// 平原适性
    /// </summary>
    public int PlainAdaptability { get; set; }

    /// <summary>
    /// 草地适性
    /// </summary>
    public int GrasslandAdaptability { get; set; }

    /// <summary>
    /// 森林适性
    /// </summary>
    public int ForrestAdaptability { get; set; }

    /// <summary>
    /// 湿地适性
    /// </summary>
    public int MarshAdaptability { get; set; }

    /// <summary>
    /// 山地适性
    /// </summary>
    public int MountainAdaptability { get; set; }

    /// <summary>
    /// 水域适性
    /// </summary>
    public int WaterAdaptability { get; set; }

    /// <summary>
    /// 峻岭适性
    /// </summary>
    public int RidgeAdaptability { get; set; }

    /// <summary>
    /// 荒地适性
    /// </summary>
    public int WastelandAdaptability { get; set; }

    /// <summary>
    /// 沙漠适性
    /// </summary>
    public int DesertAdaptability { get; set; }

    /// <summary>
    /// 棧道适性
    /// </summary>
    public int CliffAdaptability { get; set; }

    /// <summary>
    /// 平原乘数
    /// </summary>
    public float PlainRate { get; set; }

    /// <summary>
    /// 草地乘数
    /// </summary>
    public float GrasslandRate { get; set; }

    /// <summary>
    /// 森林乘数
    /// </summary>
    public float ForrestRate { get; set; }

    /// <summary>
    /// 湿地乘数
    /// </summary>
    public float MarshRate { get; set; }

    /// <summary>
    /// 山地乘数
    /// </summary>
    public float MountainRate { get; set; }

    /// <summary>
    /// 水域乘数
    /// </summary>
    public float WaterRate { get; set; }

    /// <summary>
    /// 峻岭乘数
    /// </summary>
    public float RidgeRate { get; set; }

    /// <summary>
    /// 荒地乘数
    /// </summary>
    public float WastelandRate { get; set; }

    /// <summary>
    /// 沙漠乘数
    /// </summary>
    public float DesertRate { get; set; }

    /// <summary>
    /// 棧道乘数
    /// </summary>
    public float CliffRate { get; set; }

    /// <summary>
    /// 受火伤率
    /// </summary>
    public float FireDamageRate { get; set; }

    /// <summary>
    /// 势力编队上限
    /// </summary>
    public int RecruitLimit { get; set; }

    /// <summary>
    /// 每个士兵每天消耗的粮草数
    /// </summary>
    public int FoodPerSoldier { get; set; }

    /// <summary>
    /// 口粮天数
    /// </summary>
    public int RationDays { get; set; }

    /// <summary>
    /// 每补充1人所需的技巧点数
    /// </summary>
    public int PointsPerSoldier { get; set; }

    /// <summary>
    /// 成军最小规模
    /// </summary>
    public int MinScale { get; set; }

    /// <summary>
    /// 一个单位规模所增加的攻击力
    /// </summary>
    public int OffencePerScale { get; set; }

    /// <summary>
    /// 一个单位规模所增加的防御力
    /// </summary>
    public int DefencePerScale { get; set; }

    /// <summary>
    /// 最大规模
    /// </summary>
    public int MaxScale { get; set; }

    /// <summary>
    /// 能否升级
    /// </summary>
    public bool CanLevelUp { get; set; }

    /// <summary>
    /// 升级成的兵种ID
    /// </summary>
    public List<int> LevelUpKindID { get; set; } = new();

    /// <summary>
    /// 升级经验
    /// </summary>
    public int LevelUpExperience { get; set; }

    /// <summary>
    /// 每一百经验增加的攻击力
    /// </summary>
    public int OffencePer100Experience { get; set; }

    /// <summary>
    /// 每一百经验增加的防御力
    /// </summary>
    public int DefencePer100Experience { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 最低统率(AI)
    /// </summary>
    public int MinCommand { get; set; }

    /// <summary>
    /// 新编条件：编队所在建筑条件
    /// </summary>
    public string CreateConditionsString { get; set; }

    /// <summary>
    /// 资金上限
    /// </summary>
    public int zijinshangxian { get; set; }

    /// <summary>
    /// 攻击默认类型
    /// </summary>
    public TroopAttackDefaultKind AttackDefaultKind { get; set; }

    /// <summary>
    /// 攻击目标类型
    /// </summary>
    public TroopAttackTargetKind AttackTargetKind { get; set; }

    /// <summary>
    /// 施展默认类型
    /// </summary>
    public TroopCastDefaultKind CastDefaultKind { get; set; }

    /// <summary>
    /// 施展目标类型
    /// </summary>
    public TroopCastTargetKind CastTargetKind { get; set; }

    /// <summary>
    /// 是否外壳
    /// </summary>
    public bool IsShell { get; set; }

    /// <summary>
    /// 只能在移动前攻击
    /// </summary>
    public bool OffenceOnlyBeforeMove { get; set; }

    /// <summary>
    /// 变换至兵种
    /// </summary>
    public int MorphToKindId { get; set; }

    public string AICreateArchitectureConditionWeightString { get; set; }

    public string AIUpgradeArchitectureConditionWeightString { get; set; }

    public string AIUpgradeLeaderConditionWeightString { get; set; }

    public string AILeaderConditionWeightString { get; set; }

    public MilitaryKind(MilitaryKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Type = config.Type;
        Description = config.Description;
        Merit = config.Merit;
        SuccessorString = config.SuccessorString;
        Speed = config.Speed;
        ObtainProb = config.ObtainProb;
        TitleInfluence = config.TitleInfluence;
        CreateCost = config.CreateCost;
        CreateTechnology = config.CreateTechnology;
        CreateBesideWater = config.CreateBesideWater;
        Offence = config.Offence;
        Defence = config.Defence;
        OffenceRadius = config.OffenceRadius;
        CounterOffence = config.CounterOffence;
        BeCountered = config.BeCountered;
        ObliqueOffence = config.ObliqueOffence;
        ArrowOffence = config.ArrowOffence;
        AirOffence = config.AirOffence;
        ContactOffence = config.ContactOffence;
        ArchitectureDamageRate = config.ArchitectureDamageRate;
        ArchitectureCounterDamageRate = config.ArchitectureCounterDamageRate;
        StratagemRadius = config.StratagemRadius;
        ObliqueStratagem = config.ObliqueStratagem;
        ViewRadius = config.ViewRadius;
        ObliqueView = config.ObliqueView;
        InjuryChance = config.InjuryChance;
        Movability = config.Movability;
        OneAdaptabilityKind = config.OneAdaptabilityKind;
        PlainAdaptability = config.PlainAdaptability;
        GrasslandAdaptability = config.GrasslandAdaptability;
        ForrestAdaptability = config.ForrestAdaptability;
        MarshAdaptability = config.MarshAdaptability;
        MountainAdaptability = config.MountainAdaptability;
        WaterAdaptability = config.WaterAdaptability;
        RidgeAdaptability = config.RidgeAdaptability;
        WastelandAdaptability = config.WastelandAdaptability;
        DesertAdaptability = config.DesertAdaptability;
        CliffAdaptability = config.CliffAdaptability;
        PlainRate = config.PlainRate;
        GrasslandRate = config.GrasslandRate;
        ForrestRate = config.ForrestRate;
        MarshRate = config.MarshRate;
        MountainRate = config.MountainRate;
        WaterRate = config.WaterRate;
        RidgeRate = config.RidgeRate;
        WastelandRate = config.WastelandRate;
        DesertRate = config.DesertRate;
        CliffRate = config.CliffRate;
        FireDamageRate = config.FireDamageRate;
        RecruitLimit = config.RecruitLimit;
        FoodPerSoldier = config.FoodPerSoldier;
        RationDays = config.RationDays;
        PointsPerSoldier = config.PointsPerSoldier;
        MinScale = config.MinScale;
        OffencePerScale = config.OffencePerScale;
        DefencePerScale = config.DefencePerScale;
        MaxScale = config.MaxScale;
        CanLevelUp = config.CanLevelUp;
        LevelUpKindID = config.LevelUpKindID;
        LevelUpExperience = config.LevelUpExperience;
        OffencePer100Experience = config.OffencePer100Experience;
        DefencePer100Experience = config.DefencePer100Experience;
        InfluencesString = config.InfluencesString;
        MinCommand = config.MinCommand;
        CreateConditionsString = config.CreateConditionsString;
        zijinshangxian = config.zijinshangxian;
        AttackDefaultKind = config.AttackDefaultKind;
        AttackTargetKind = config.AttackTargetKind;
        CastDefaultKind = config.CastDefaultKind;
        CastTargetKind = config.CastTargetKind;
        IsShell = config.IsShell;
        OffenceOnlyBeforeMove = config.OffenceOnlyBeforeMove;
        MorphToKindId = config.MorphToKindId;
        AICreateArchitectureConditionWeightString = config.AICreateArchitectureConditionWeightString;
        AIUpgradeArchitectureConditionWeightString = config.AIUpgradeArchitectureConditionWeightString;
        AIUpgradeLeaderConditionWeightString = config.AIUpgradeLeaderConditionWeightString;
        AILeaderConditionWeightString = config.AILeaderConditionWeightString;
    }

    public string TypeName => Type.GetDescription();

    //[DataMember]
    public TroopSounds Sounds;

    public TroopTextures Textures;

    public List<Influence> Influences { get; set; } = new();

    public List<Condition> CreateConditions { get; set; } = new();

    public Dictionary<Condition, float> AICreateArchitectureConditionWeight = new();

    public Dictionary<Condition, float> AIUpgradeArchitectureConditionWeight = new();
    
    public Dictionary<Condition, float> AIUpgradeLeaderConditionWeight = new();

    public Dictionary<Condition, float> AILeaderConditionWeight = new();

    public List<Person> Persons { get; set; } = new();

    public List<MilitaryKind> Successor { get; set; } = new();

    private bool findSuccessor_visited;

    public bool LevelUpAvail(Architecture arch)
    {
        return CheckConditions(arch) && GetLevelUpKinds(arch).Count > 0;
    }

    public bool CreateAvail(Architecture arch)
    {
        if (IsShell) return false;
        
        if (arch.Fund < CreateCost * GetRateOfNewMilitary(arch) || arch.Technology < CreateTechnology)
        {
            return false;
        }

        if (arch.BelongedFaction.IsMilitaryKindOverLimit(ID))
        {
            return false;
        }

        if (CreateBesideWater && arch.IsBesideWater)
        {
            return false;
        }

        if (!CheckConditions(arch))
        {
            return false;
        }

        return true;
    }

    public bool IsTransport => ID == 29;

    public MilitaryKind findSuccessorCreatable(List<MilitaryKind> kinds, Dictionary<int, MilitaryKind> newKinds)
    {
        foreach (var militaryKind in kinds)
        {
            militaryKind.findSuccessor_visited = false;
        }

        var militaryKinds = kinds.ToDictionary(x => x.ID);

        return findSuccessorRecruitable_r(militaryKinds, newKinds, this);
    }

    private MilitaryKind findSuccessorRecruitable_r(Dictionary<int, MilitaryKind> kinds, Dictionary<int, MilitaryKind> newKinds, MilitaryKind prev)
    {
        if (prev.Successor.Count == 0) return prev;

        prev.findSuccessor_visited = true;

        var toVisit = new List<MilitaryKind>();
        foreach (var militaryKind in prev.Successor)
        {
            var kindId = militaryKind.ID;
            if (!militaryKind.findSuccessor_visited && newKinds.ContainsKey(kindId) && kinds.ContainsKey(kindId))
            {
                toVisit.Add(militaryKind);
            }
        }

        if (toVisit.Count == 0) return prev;

        return findSuccessorRecruitable_r(kinds, newKinds, StaticMethods.GetRandomItem(toVisit));
    }

    public float GetRateOfNewMilitary(Architecture arch)
    {
        switch (Type)
        {
            case MilitaryType.Infantry:
                return arch.RateOfNewBubingMilitaryFundCost;

            case MilitaryType.Crossbow:
                return arch.RateOfNewNubingMilitaryFundCost;

            case MilitaryType.Cavalry:
                return arch.RateOfNewQibingMilitaryFundCost;

            case MilitaryType.Navy:
                return arch.RateOfNewShuijunMilitaryFundCost;

            case MilitaryType.SiegeEquipment:
                return arch.RateOfNewQixieMilitaryFundCost;
        }
        
        return 1f;
    }

    public int[] Adaptabilities
    {
        get
        {
            return [PlainAdaptability, GrasslandAdaptability, ForrestAdaptability, WastelandAdaptability, MarshAdaptability,
                    MountainAdaptability, CliffAdaptability, RidgeAdaptability, WaterAdaptability];
        }
    }

    public bool Movable
    {
        get
        {
            foreach (int i in Adaptabilities)
            {
                if (Movability >= i)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public int GetTerrainAdaptability(TerrainKind terrain)
    {
        switch (terrain)
        {
            case TerrainKind.无:
                return 0xdac;

            case TerrainKind.平原:
                return PlainAdaptability;

            case TerrainKind.草原:
                return GrasslandAdaptability;

            case TerrainKind.森林:
                return ForrestAdaptability;

            case TerrainKind.湿地:
                return MarshAdaptability;

            case TerrainKind.山地:
                return MountainAdaptability;

            case TerrainKind.水域:
                return WaterAdaptability;

            case TerrainKind.峻岭:
                return RidgeAdaptability;

            case TerrainKind.荒地:
                return WastelandAdaptability;

            case TerrainKind.沙漠:
                return DesertAdaptability;

            case TerrainKind.栈道:
                return CliffAdaptability;
        }
        return 0xdac;
    }

    public bool IsMovableOnPosition(Point position)
    {
        return GetTerrainAdaptability(Session.Current.Scenario.GetTerrainKindByPosition(position)) <= Movability;
    }

    public override string ToString() => $"{Name} {Type}";
    
    public string ArrowOffenceString => StaticMethods.ToMark(ArrowOffence);
    
    public string BeCounteredString => StaticMethods.ToMark(BeCountered);
    
    public string CanLevelUpString => StaticMethods.ToMark(CanLevelUp);
    
    public string ContactOffenceString => StaticMethods.ToMark(ContactOffence);

    public string CounterOffenceString => StaticMethods.ToMark(CounterOffence);

    public string CreateBesideWaterString => StaticMethods.ToMark(CreateBesideWater);

    public string IsShellString => StaticMethods.ToMark(IsShell);

    public string ObliqueOffenceString => StaticMethods.ToMark(ObliqueOffence);
    
    public string ObliqueStratagemString => StaticMethods.ToMark(ObliqueStratagem);

    public string OffenceOnlyBeforeMoveString => StaticMethods.ToMark(OffenceOnlyBeforeMove);
    
    public int InfluenceCount => Influences.Count;
    
    public List<MilitaryKind> GetLevelUpKinds(Architecture arch)
    {
        var militaryKinds = Session.Current.Scenario.GameCommonData.AllMilitaryKinds;

        List<MilitaryKind> result = new List<MilitaryKind>();
        foreach (int id in LevelUpKindID)
        {
            if (!arch.BelongedFaction.IsMilitaryKindOverLimit(id) && militaryKinds.TryGetValue(id, out var militaryKind))
            {
                result.Add(militaryKind);
            }
        }

        return result;
    }
    
    public MilitaryKind MorphTo
    {
        get
        {
            if (Session.Current.Scenario.GameCommonData.AllMilitaryKinds.TryGetValue(MorphToKindId, out var militaryKind))
            {
                return militaryKind;
            }

            return null;
        }
    }

    public bool CheckConditions(Architecture arch)
    {
        return Condition.CheckConditionList(CreateConditions, arch);
    }

    /*
    public int EachMilitaryKindCount(Faction f)
    {
        int count = 0;
       // MilitaryKind mk = Session.Current.Scenario.GameCommonData.AllMilitaryKinds.GetMilitaryKind(id);
        if (f != null)
        {
            foreach (Military military in f.Militaries)
            {
                if (military.RealKindID == this.ID )
                {
                    count++;
                }
            }
        }

        return count;
    }
    */
}