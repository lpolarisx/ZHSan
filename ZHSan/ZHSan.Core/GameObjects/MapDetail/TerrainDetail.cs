using GameGlobal;
using GameObjects.TroopDetail;
using System.Runtime.Serialization;
using GameManager;

namespace GameObjects.MapDetail;

[DataContract]
public class TerrainDetail : GameObject
{
    #region DataMember

    /// <summary>
    /// 图形层次
    /// </summary>
    [DataMember]
    public int GraphicLayer { get; set; }

    /// <summary>
    /// 视线可穿透
    /// </summary>
    [DataMember]
    public bool ViewThrough { get; set; }

    /// <summary>
    /// 粮道开通资金消耗
    /// </summary>
    [DataMember]
    public int RoutewayBuildFundCost { get; set; }

    /// <summary>
    /// 粮道维持资金消耗
    /// </summary>
    [DataMember]
    public int RoutewayActiveFundCost { get; set; }

    /// <summary>
    /// 粮道开通工作量
    /// </summary>
    [DataMember]
    public int RoutewayBuildWorkCost { get; set; }

    /// <summary>
    /// 粮草消耗率
    /// </summary>
    [DataMember]
    public float RoutewayConsumptionRate { get; set; }

    /// <summary>
    /// 粮草蕴藏量
    /// </summary>
    [DataMember]
    public int FoodDeposit { get; set; }

    /// <summary>
    /// 粮草恢复天数
    /// </summary>
    [DataMember]
    public int FoodRegainDays { get; set; }

    /// <summary>
    /// 春粮系数
    /// </summary>
    [DataMember]
    public float FoodSpringRate { get; set; }

    /// <summary>
    /// 夏粮系数
    /// </summary>
    [DataMember]
    public float FoodSummerRate { get; set; }

    /// <summary>
    /// 秋粮系数
    /// </summary>
    [DataMember]
    public float FoodAutumnRate { get; set; }

    /// <summary>
    /// 冬粮系数
    /// </summary>
    [DataMember]
    public float FoodWinterRate { get; set; }

    /// <summary>
    /// 火焰伤害率
    /// </summary>
    [DataMember]
    public float FireDamageRate { get; set; }

    [DataMember]
    public bool CanExtendInto { get; set; }

    #endregion

    public TerrainTextures Textures = new TerrainTextures();

    public void Init()
    {
        Textures = new TerrainTextures();
    }

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
                foreach (MilitaryKind militaryKind in Session.Current.Scenario.GameCommonData.AllMilitaryKinds.GetMilitaryKindList())
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