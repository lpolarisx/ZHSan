
using System.Collections.Generic;
using GameEnums;

namespace GameDatas;

public class PersonMessageConfig
{
    /// <summary>
    /// 人物Id
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// 语言类型
    /// </summary>
    public TextMessageKind Kind { get; set; }

    /// <summary>
    /// 个性语言
    /// </summary>
    public List<string> Messages { get; set; }
}