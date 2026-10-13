
namespace GameDatas;

public class InfluenceConfig : BaseConfig
{
    /// <summary>
    /// 影响种类Id
    /// </summary>
    public int KindId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 参数1
    /// </summary>
    public string Parameter { get; set; }

    /// <summary>
    /// 参数2
    /// </summary>
    public string Parameter2 { get; set; }
}