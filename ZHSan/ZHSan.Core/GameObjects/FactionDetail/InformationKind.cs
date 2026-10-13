using Extensions;
using GameDatas;
using GameEnums;
using GameGlobal;

namespace GameObjects.FactionDetail;

/// <summary>
/// 情报种类
/// </summary>
public class InformationKind : GameObject
{
    /// <summary>
    /// 消耗资金
    /// </summary>
    public int CostFund { get; set; }
    
    /// <summary>
    /// 等级
    /// </summary>
    public InformationLevel Level { get; set; }

    /// <summary>
    /// 斜向
    /// </summary>
    public bool Oblique { get; set; }

    /// <summary>
    /// 半径范围
    /// </summary>
    public int Radius { get; set; }

    public InformationKind(InformationKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        CostFund = config.CostFund;
        Level = config.Level;
        Oblique = config.Oblique;
        Radius = config.Radius;
    }

    public string ObliqueString => StaticMethods.ToMark(Oblique);

    public int FightingWeighing => Radius * (int)Level * 100 / CostFund;

    public string LevelName => Level.GetDescription();
}