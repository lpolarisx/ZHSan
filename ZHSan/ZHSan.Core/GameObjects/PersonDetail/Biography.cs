using GameObjects.TroopDetail;
using System.Runtime.Serialization;

namespace GameObjects.PersonDetail;

/// <summary>
/// 传记
/// </summary>
[DataContract]
public class Biography : GameObject
{
    /// <summary>
    /// 简要
    /// </summary>
    [DataMember]
    public string Brief { get; set; }

    /// <summary>
    /// 演义
    /// </summary>
    [DataMember]
    public string Romance { get; set; }

    /// <summary>
    /// 历史
    /// </summary>
    [DataMember]
    public string History { get; set; }

    /// <summary>
    /// 剧本
    /// </summary>
    [DataMember]
    public string InGame { get; set; }

    /// <summary>
    /// 势力颜色 此武将自立时使用的势力颜色
    /// </summary>
    [DataMember]
    public int FactionColor { get; set; }

    /// <summary>
    /// 兵种列表 此武将自立时使用的基本兵种
    /// </summary>
    [DataMember]
    public string MilitaryKindsString { get; set; }

    public MilitaryKindTable MilitaryKinds = new MilitaryKindTable();

    public void Init()
    {
        MilitaryKinds = new MilitaryKindTable();
    }
}