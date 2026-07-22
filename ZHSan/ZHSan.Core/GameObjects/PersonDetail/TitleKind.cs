using System.Collections.Generic;
using GameDatas;

namespace GameObjects.PersonDetail;

/// <summary>
/// 称号类别
/// </summary>
public class TitleKind : GameObject
{
    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 习得天数
    /// </summary>
    public int StudyDay { get; set; }

    /// <summary>
    /// 习得成功率
    /// </summary>
    public int SuccessRate { get; set; }

    /// <summary>
    /// 可免除
    /// </summary>
    public bool Recallable { get; set; }

    /// <summary>
    /// 可额外传授
    /// </summary>
    public bool RandomTeachable { get; set; }

    private bool? inheritable;

    public bool IsInheritable(List<Title> titles)
    {
        if (inheritable.HasValue) return inheritable.Value;

        inheritable = false;
        foreach (var title in titles)
        {
            if (title.KindId == ID && title.CanBeBorn())
            {
                inheritable = true;
                break;
            }
        }

        return inheritable.Value;
    }

    public TitleKind(TitleKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Combat = config.Combat;
        StudyDay = config.StudyDay;
        SuccessRate = config.SuccessRate;
        Recallable = config.Recallable;
        RandomTeachable = config.RandomTeachable;
    }
}