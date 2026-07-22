using GameDatas;
using GameGlobal;
using GameManager;

namespace GameObjects.ArchitectureDetail;

public class ArchitectureKind : GameObject
{
    #region DataMember

    /// <summary>
    /// 农业
    /// </summary>
    public bool HasAgriculture { get; set; }

    /// <summary>
    /// 农业基础 建筑农业值上限为：农业基础+农业单位*建筑规模
    /// </summary>
    public int AgricultureBase { get; set; }

    /// <summary>
    /// 农业单位
    /// </summary>
    public int AgricultureUnit { get; set; }

    /// <summary>
    /// 商业
    /// </summary>
    public bool HasCommerce { get; set; }

    /// <summary>
    /// 商业基础 建筑商业值上限为：商业基础+商业单位*建筑规模
    /// </summary>
    public int CommerceBase { get; set; }

    /// <summary>
    /// 商业单位
    /// </summary>
    public int CommerceUnit { get; set; }

    /// <summary>
    /// 技术
    /// </summary>
    public bool HasTechnology { get; set; }

    /// <summary>
    /// 技术基础 建筑技术值上限为：技术基础+技术单位*建筑规模
    /// </summary>
    public int TechnologyBase { get; set; }

    /// <summary>
    /// 技术单位
    /// </summary>
    public int TechnologyUnit { get; set; }

    /// <summary>
    /// 统治
    /// </summary>
    public bool HasDomination { get; set; }

    /// <summary>
    /// 统治基础 建筑统治值上限为：统治基础+ 统治单位*建筑规模
    /// </summary>
    public int DominationBase { get; set; }

    /// <summary>
    /// 统治单位
    /// </summary>
    public int DominationUnit { get; set; }

    /// <summary>
    /// 民心
    /// </summary>
    public bool HasMorale { get; set; }

    /// <summary>
    /// 民心基础 建筑民心值上限为：民心基础+民心单位*建筑规模
    /// </summary>
    public int MoraleBase { get; set; }

    /// <summary>
    /// 民心单位
    /// </summary>
    public int MoraleUnit { get; set; }

    /// <summary>
    /// 耐久
    /// </summary>
    public bool HasEndurance { get; set; }

    /// <summary>
    /// 耐久基础 建筑耐久值上限为：耐久基础+耐久单位*建筑规模
    /// </summary>
    public int EnduranceBase { get; set; }

    /// <summary>
    /// 耐久单位
    /// </summary>
    public int EnduranceUnit { get; set; }

    /// <summary>
    /// 人口
    /// </summary>
    public bool HasPopulation { get; set; }

    /// <summary>
    /// 人口基础 建筑人口值上限为：人口基础+人口单位*建筑规模
    /// </summary>
    public int PopulationBase { get; set; }

    /// <summary>
    /// 人口单位
    /// </summary>
    public int PopulationUnit { get; set; }

    /// <summary>
    /// 基本视野范围
    /// </summary>
    public int ViewDistance { get; set; }

    /// <summary>
    /// 视野范围增量除数 视野距离=基本视野范围+建筑规模除以此值
    /// </summary>
    public int ViewDistanceIncrementDivisor { get; set; }

    /// <summary>
    /// 斜向视野
    /// </summary>
    public bool HasObliqueView { get; set; }

    /// <summary>
    /// 长视距
    /// </summary>
    public bool HasLongView { get; set; }

    /// <summary>
    /// 可编组运兵船
    /// </summary>
    public bool HasHarbor { get; set; }

    /// <summary>
    /// 设施单位空间 此值*建筑规模为最终值
    /// </summary>
    public int FacilityPositionUnit { get; set; }

    /// <summary>
    /// 最大资金单位 此值*建筑规模为最终值
    /// </summary>
    public int FundMaxUnit { get; set; }

    /// <summary>
    /// 最大粮草单位 此值*建筑规模为最终值
    /// </summary>
    public int FoodMaxUnit { get; set; }

    /// <summary>
    /// 是否属于官职所需城池类型
    /// </summary>
    public bool CountToMerit { get; set; }

    /// <summary>
    /// 可扩建 4为可扩建一次，8为可扩建两次
    /// </summary>
    public int Expandable { get; set; }

    /// <summary>
    /// 征兵人口界限
    /// </summary>
    public int PopulationBoundary { get; set; }

    /// <summary>
    /// 船只可进入
    /// </summary>
    public bool ShipCanEnter { get; set; }

    #endregion

    public ArchitectureKind(ArchitectureKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        HasAgriculture = config.HasAgriculture;
        AgricultureBase = config.AgricultureBase;
        AgricultureUnit = config.AgricultureUnit;
        HasCommerce = config.HasCommerce;
        CommerceBase = config.CommerceBase;
        CommerceUnit = config.CommerceUnit;
        HasTechnology = config.HasTechnology;
        TechnologyBase = config.TechnologyBase;
        TechnologyUnit = config.TechnologyUnit;
        HasDomination = config.HasDomination;
        DominationBase = config.DominationBase;
        DominationUnit = config.DominationUnit;
        HasMorale = config.HasMorale;
        MoraleBase = config.MoraleBase;
        MoraleUnit = config.MoraleUnit;
        HasEndurance = config.HasEndurance;
        EnduranceBase = config.EnduranceBase;
        EnduranceUnit = config.EnduranceUnit;
        HasPopulation = config.HasPopulation;
        PopulationBase = config.PopulationBase;
        PopulationUnit = config.PopulationUnit;
        ViewDistance = config.ViewDistance;
        ViewDistanceIncrementDivisor = config.ViewDistanceIncrementDivisor;
        HasObliqueView = config.HasObliqueView;
        HasLongView = config.HasLongView;
        HasHarbor = config.HasHarbor;
        FacilityPositionUnit = config.FacilityPositionUnit;
        FundMaxUnit = config.FundMaxUnit;
        FoodMaxUnit = config.FoodMaxUnit;
        CountToMerit = config.CountToMerit;
        Expandable = config.Expandable;
        PopulationBoundary = config.PopulationBoundary;
        ShipCanEnter = config.ShipCanEnter;
    }

    public string HasAgricultureString => StaticMethods.ToMark(HasAgriculture);

    public string HasCommerceString => StaticMethods.ToMark(HasCommerce);

    public string HasDominationString => StaticMethods.ToMark(HasDomination);

    public string HasEnduranceString => StaticMethods.ToMark(HasEndurance);

    public string HasHarborString => StaticMethods.ToMark(HasHarbor);

    public string HasLongViewString => StaticMethods.ToMark(HasLongView);

    public string HasMoraleString => StaticMethods.ToMark(HasMorale);

    public string HasObliqueViewString => StaticMethods.ToMark(HasObliqueView);

    public string HasPopulationString => StaticMethods.ToMark(HasPopulation);

    public string HasTechnologyString => StaticMethods.ToMark(HasTechnology);

    private PlatformTexture texture;

    public PlatformTexture Texture
    {
        get
        {
            if (texture == null)
            {
                var fileName = $@"Content/Textures/Resources/Architecture/{ID}.png";

                texture = CacheManager.GetTempTexture(fileName);
            }

            return texture;
        }
    }
}