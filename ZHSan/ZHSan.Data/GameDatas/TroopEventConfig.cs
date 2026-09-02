
using GameEnums;

namespace GameDatas;

public class TroopEventConfig : BaseConfig
{
    /// <summary>
    /// 已发生过
    /// </summary>
    public bool Happened { get; set; }

    /// <summary>
    /// 可以重复
    /// </summary>
    public bool Repeatable { get; set; }

    /// <summary>
    /// 某事件发生之后：需要在某事件发生过之后才能触发
    /// </summary>
    public int AfterEventHappened { get; set; }

    /// <summary>
    /// 发动人物：发动事件的人物，所有效果将以本人物所在部队为中心。如果为-1，则每个部队都可以触发此事件。
    /// </summary>
    public int LaunchPersonString { get; set; }

    /// <summary>
    /// 人物对话：每个人物ID后跟随其要说的话。以空格分隔。留空则无对话。
    /// </summary>
    public string DialogString { get; set; }

    /// <summary>
    /// 发动条件：发动事件的部队所需要满足的条件，留空则为满足。
    /// </summary>
    public string ConditionsString { get; set; }

    /// <summary>
    /// 发动几率：0--100
    /// </summary>
    public int HappenChance { get; set; }

    /// <summary>
    /// 目标人物列表：每个关系之后跟随一个人物ID。0：非友好；1：友好。留空则不在搜索范围检查此条件。
    /// </summary>
    public string TargetPersonsString { get; set; }

    /// <summary>
    /// 自身效果：发动部队的效果列表
    /// </summary>
    public string SelfEffectsString { get; set; }

    /// <summary>
    /// 特定人物效果：每个人物ID后跟随一个效果种类。以空格分隔。
    /// </summary>
    public string EffectPersonsString { get; set; }

    /// <summary>
    /// 特定范围效果：范围类别：
    /// 0：视野内所有敌军；
    /// 1：视野内所有友军；
    /// 2：攻击范围内所有敌军；
    /// 3：攻击范围内所有友军；
    /// 4：周围八格内所有敌军；
    /// 5：周围八格内所有友军；
    /// 每个范围类别后跟随一个效果种类。以空格分隔。
    /// </summary>
    public string EffectAreasString { get; set; }

    /// <summary>
    /// 图片。图片档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录tupian里
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// 音效。音效档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录yinxiao里
    /// </summary>
    public string Sound { get; set; }

    /// <summary>
    /// 0--视野内 1--周边八格 2--攻击范围 用来搜索目标人物列表
    /// </summary>
    public EventCheckAreaKind CheckArea { get; set; }

    public string TryToShowString { get; set; }
}