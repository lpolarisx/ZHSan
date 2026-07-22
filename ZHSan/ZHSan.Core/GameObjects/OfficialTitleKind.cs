using GameDatas;

namespace GameObjects;

public class OfficialTitleKind : GameObject
{
    /// <summary>
    /// 声望上限
    /// </summary>
    public int ReputationCap { get; set; }

    /// <summary>
    /// 封官所需朝廷贡献度
    /// </summary>
    public int RequiredContribution { get; set; }

    /// <summary>
    /// 是否全地图显示封官对话
    /// </summary>
    public bool ShowDialog { get; set; }
    
    /// <summary>
    /// 封官所需城池列表
    /// </summary>
    public int RequiredArchitecture { get; set; }

    /// <summary>
    /// 手下武将忠诚度变化值
    /// </summary>
    public int Loyalty { get; set; }

    public OfficialTitleKind() {}

    public OfficialTitleKind(OfficialTitleKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        ReputationCap = config.ReputationCap;
        RequiredContribution = config.RequiredContribution;
        ShowDialog = config.ShowDialog;
        RequiredArchitecture = config.RequiredArchitecture;
        Loyalty = config.Loyalty;
    }
}