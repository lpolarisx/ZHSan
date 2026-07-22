using GameManager;
using GameObjects.TroopDetail;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.PersonDetail;

[DataContract]
public class Biography : GameObject
{
    public List<MilitaryKind> MilitaryKinds { get; set; } = new();

    /// <summary>
    /// 简要
    /// </summary>
    [DataMember]
    public string Brief { get; set; }

    /// <summary>
    /// 势力颜色 此武将自立时使用的势力颜色
    /// </summary>
    [DataMember]
    public int FactionColor { get; set; }

    /// <summary>
    /// 历史
    /// </summary>
    [DataMember]
    public string History { get; set; }

    /// <summary>
    /// 演义
    /// </summary>
    [DataMember]
    public string Romance { get; set; }

    /// <summary>
    /// 剧本
    /// </summary>
    [DataMember]
    public string InGame { get; set; }

    /// <summary>
    /// 兵种列表 此武将自立时使用的基本兵种
    /// </summary>
    [DataMember]
    public string MilitaryKindsString { get; set; }

    public void AddBasicMilitaryKinds()
    {
        var allMilitaryKinds = Session.Current.Scenario.GameCommonData.AllMilitaryKinds;
        var basicKindIds = new int[] { 0, 1, 2, 30 };

        foreach (var kindId in basicKindIds)
        {
            if (allMilitaryKinds.TryGetValue(kindId, out var militaryKind))
            {
                MilitaryKinds.Add(militaryKind);
            }
        }
    }
}