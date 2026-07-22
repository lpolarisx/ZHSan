using GameGlobal;
using GameObjects.TroopDetail;
using GameManager;
using GameDatas;

namespace GameObjects.MapDetail;

public class TerrainDetail : GameObject
{
    /// <summary>
    /// 图形层次
    /// </summary>
    public int GraphicLayer { get; set; }

    /// <summary>
    /// 视线可穿透
    /// </summary>
    public bool ViewThrough { get; set; }

    /// <summary>
    /// 粮道开通资金消耗
    /// </summary>
    public int RoutewayBuildFundCost { get; set; }

    /// <summary>
    /// 粮道维持资金消耗
    /// </summary>
    public int RoutewayActiveFundCost { get; set; }

    /// <summary>
    /// 粮道开通工作量
    /// </summary>
    public int RoutewayBuildWorkCost { get; set; }

    /// <summary>
    /// 粮草消耗率
    /// </summary>
    public float RoutewayConsumptionRate { get; set; }

    /// <summary>
    /// 粮草蕴藏量
    /// </summary>
    public int FoodDeposit { get; set; }

    /// <summary>
    /// 粮草恢复天数
    /// </summary>
    public int FoodRegainDays { get; set; }

    /// <summary>
    /// 春粮系数
    /// </summary>
    public float FoodSpringRate { get; set; }

    /// <summary>
    /// 夏粮系数
    /// </summary>
    public float FoodSummerRate { get; set; }

    /// <summary>
    /// 秋粮系数
    /// </summary>
    public float FoodAutumnRate { get; set; }

    /// <summary>
    /// 冬粮系数
    /// </summary>
    public float FoodWinterRate { get; set; }

    /// <summary>
    /// 火焰伤害率
    /// </summary>
    public float FireDamageRate { get; set; }

    public bool CanExtendInto { get; set; }

    public TerrainDetail(TerrainDetailConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        GraphicLayer = config.GraphicLayer;
        ViewThrough = config.ViewThrough;
        RoutewayBuildFundCost = config.RoutewayBuildFundCost;
        RoutewayActiveFundCost = config.RoutewayActiveFundCost;
        RoutewayBuildWorkCost = config.RoutewayBuildWorkCost;
        RoutewayConsumptionRate = config.RoutewayConsumptionRate;
        FoodDeposit = config.FoodDeposit;
        FoodRegainDays = config.FoodRegainDays;
        FoodSpringRate = config.FoodSpringRate;
        FoodSummerRate = config.FoodSummerRate;
        FoodAutumnRate = config.FoodAutumnRate;
        FoodWinterRate = config.FoodWinterRate;
        FireDamageRate = config.FireDamageRate;
        CanExtendInto = config.CanExtendInto;
    }

    public TerrainTextures Textures { get; set; } = new();

    public int GetFood(GameSeason season)
    {
        switch (season)
        {
            case GameSeason.春:
                return (int)(FoodDeposit * FoodSpringRate);

            case GameSeason.夏:
                return (int)(FoodDeposit * FoodSummerRate);

            case GameSeason.秋:
                return (int)(FoodDeposit * FoodAutumnRate);

            case GameSeason.冬:
                return (int)(FoodDeposit * FoodWinterRate);
        }
        return FoodDeposit;
    }

    public int GetRandomFood(GameSeason season)
    {
        var random = StaticMethods.Random(FoodDeposit / 2);
        int num = random + FoodDeposit * 3 / 4;

        switch (season)
        {
            case GameSeason.春:
                return (int)(num * FoodSpringRate);

            case GameSeason.夏:
                return (int)(num * FoodSummerRate);

            case GameSeason.秋:
                return (int)(num * FoodAutumnRate);

            case GameSeason.冬:
                return (int)(num * FoodWinterRate);
        }
        return num;
    }

    public string FireDamageRateString => StaticMethods.GetPercentString(FireDamageRate, 3);

    public string FoodAutumnRateString => StaticMethods.GetPercentString(FoodAutumnRate, 3);

    public string FoodSpringRateString => StaticMethods.GetPercentString(FoodSpringRate, 3);

    public string FoodSummerRateString => StaticMethods.GetPercentString(FoodSummerRate, 3);

    public string FoodWinterRateString => StaticMethods.GetPercentString(FoodWinterRate, 3);

    public int RandomRegainDays => StaticMethods.Random(FoodRegainDays / 2) + FoodRegainDays * 3 / 4;

    private bool? troopPassable = null;

    /// <summary>
    /// 允许部队通过
    /// </summary>
    public bool TroopPassable
    {
        get
        {
            if (troopPassable == null)
            {
                troopPassable = false;
                foreach (var militaryKind in Session.Current.Scenario.GameCommonData.AllMilitaryKinds.Values)
                {
                    bool passable = false;
                    switch (ID)
                    {
                        case 1:
                            passable = militaryKind.PlainAdaptability <= militaryKind.Movability;
                            break;
                        case 2:
                            passable = militaryKind.GrasslandAdaptability <= militaryKind.Movability;
                            break;
                        case 3:
                            passable = militaryKind.ForrestAdaptability <= militaryKind.Movability;
                            break;
                        case 4:
                            passable = militaryKind.MarshAdaptability <= militaryKind.Movability;
                            break;
                        case 5:
                            passable = militaryKind.MountainAdaptability <= militaryKind.Movability;
                            break;
                        case 6:
                            passable = militaryKind.WaterAdaptability <= militaryKind.Movability;
                            break;
                        case 7:
                            passable = militaryKind.RidgeAdaptability <= militaryKind.Movability;
                            break;
                        case 8:
                            passable = militaryKind.WastelandAdaptability <= militaryKind.Movability;
                            break;
                        case 9:
                            passable = militaryKind.DesertAdaptability <= militaryKind.Movability;
                            break;
                        case 10:
                            passable = militaryKind.CliffAdaptability <= militaryKind.Movability;
                            break;
                    }

                    if (passable)
                    {
                        troopPassable = true;
                        break;
                    }
                }
            }
            return troopPassable.Value;
        }
    }
}