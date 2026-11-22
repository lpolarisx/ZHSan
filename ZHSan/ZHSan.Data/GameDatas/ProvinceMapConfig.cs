using System.Collections.Generic;

namespace GameDatas;

/// <summary>
/// 历史州郡地图配置（存放于 CommonData.dat 或剧本 .dat 内的 Provinces.json）
/// </summary>
public class ProvinceMapConfig
{
    public string Description { get; set; }

    public int MaxCost { get; set; } = 4500;

    public Dictionary<string, int> TerrainCosts { get; set; } = new();

    public List<ProvinceConfig> Provinces { get; set; } = new();

    public List<ProvinceOverrideConfig> Overrides { get; set; } = new();
}

/// <summary>
/// 州配置
/// </summary>
public class ProvinceConfig : BaseConfig
{
    public string Color { get; set; }

    public int[] LabelPos { get; set; }

    public List<CommanderyConfig> Commanderies { get; set; } = new();
}

/// <summary>
/// 郡配置
/// </summary>
public class CommanderyConfig : BaseConfig
{
    public int Weight { get; set; } = 100;

    public int MaxCost { get; set; }

    public int[] LabelPos { get; set; }

    public List<CommanderyAnchorConfig> Anchors { get; set; } = new();
}

/// <summary>
/// 郡扩散锚点
/// </summary>
public class CommanderyAnchorConfig
{
    public string Name { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int StartCost { get; set; }

    public int Weight { get; set; }
}

/// <summary>
/// 手工区域覆盖（CommanderyId 为 0 时清空该区域归属）
/// </summary>
public class ProvinceOverrideConfig
{
    public int CommanderyId { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; } = 1;

    public int Height { get; set; } = 1;
}
