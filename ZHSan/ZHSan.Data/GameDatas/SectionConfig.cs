
namespace GameDatas;

public class SectionConfig : BaseConfig
{
    /// <summary>
    /// 委任类型
    /// </summary>
    public int AIDetailIDString { get; set; }

    /// <summary>
    /// 目标势力
    /// </summary>
    public int OrientationFactionID { get; set; }

    /// <summary>
    /// 目标军区
    /// </summary>
    public int OrientationSectionID { get; set; }

    /// <summary>
    /// 目标州域
    /// </summary>
    public int OrientationStateID { get; set; }

    /// <summary>
    /// 目标建筑
    /// </summary>
    public int OrientationArchitectureID { get; set; }

    /// <summary>
    /// 建筑列表
    /// </summary>
    public string ArchitecturesString { get; set; }
}