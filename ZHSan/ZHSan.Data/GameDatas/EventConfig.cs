
namespace GameDatas;

public class EventConfig : BaseConfig
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
    /// 不重要 不重要的事件不会出现对话，除非涉及君主
    /// </summary>
    public bool Minor { get; set; }

    /// <summary>
    /// 某事件发生之后 需要在某事件发生过之后才能触发
    /// </summary>
    public int AfterEventHappened { get; set; }

    /// <summary>
    /// 发动几率 实际机率为1除以此数
    /// </summary>
    public int HappenChance { get; set; }

    /// <summary>
    /// 全势力可见
    /// </summary>
    public bool GloballyDisplayed { get; set; }

    /// <summary>
    /// 开始年
    /// </summary>
    public int StartYear { get; set; }

    /// <summary>
    /// 开始月
    /// </summary>
    public int StartMonth { get; set; }

    /// <summary>
    /// 结束年
    /// </summary>
    public int EndYear { get; set; }

    /// <summary>
    /// 结束月
    /// </summary>
    public int EndMonth { get; set; }

    /// <summary>
    /// 武将编号 指定可能触发的武将ID，以空格分隔，先指定第k个武将，后跟随一个武将ID，如0 100 0 234 1 346 2 -1 可在同一个k指定多个武将，则代表列表中任何一个 -1代表任何武将
    /// </summary>
    public string PersonString { get; set; }

    /// <summary>
    /// 武将条件 触发的武将需符合的条件，以空格分隔，先指定第k个武将，后跟随一个武将ID
    /// </summary>
    public string PersonCondString { get; set; }

    /// <summary>
    /// 建筑编号 触发时，指定所有武将所在建筑的ID，以空格分隔 留空代表任何建筑
    /// </summary>
    public string ArchitectureString { get; set; }

    /// <summary>
    /// 建筑条件 触发时，所有武将所在的建筑需符合的条件 如使用武将条件，将检查该建筑的县令
    /// </summary>
    public string ArchitectureCondString { get; set; }

    /// <summary>
    /// 势力编号 触发时，指定所有武将所在势力的ID，以空格分隔 留空代表任何势力
    /// </summary>
    public string FactionString { get; set; }

    /// <summary>
    /// 势力条件 触发时，所有武将所在的势力需符合的条件 如使用武将条件，将检查该势力的君主
    /// </summary>
    public string FactionCondString { get; set; }

    /// <summary>
    /// 对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
    /// </summary>
    public string DialogString { get; set; }

    /// <summary>
    /// 效果 先指定第k个武将 后跟随一个效果种类 以空格分隔
    /// </summary>
    public string EffectString { get; set; }

    /// <summary>
    /// 建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
    /// </summary>
    public string ArchitectureEffectString { get; set; }

    /// <summary>
    /// 势力效果 武将所在势力效果，以空格分隔 如使用武将效果，将应用于该势力的君主
    /// </summary>
    public string FactionEffectIDString { get; set; }

    /// <summary>
    /// 图片 图片档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录tupian里
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// 音效 音效档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录yinxiao里
    /// </summary>
    public string Sound { get; set; }

    /// <summary>
    /// 选是的对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
    /// </summary>
    public string YesDialogString { get; set; }

    /// <summary>
    /// 选否的对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
    /// </summary>
    public string NoDialogString { get; set; }

    /// <summary>
    /// 选是的效果 如果填上，这事件会有选项 选是后执行这些效果 先指定第k个武将，后跟随一个效果种类 以空格分隔
    /// </summary>
    public string YesEffectString { get; set; }

    /// <summary>
    /// 选否的效果 如果填上，这事件会有选项 选否后执行这些效果 先指定第k个武将，后跟随一个效果种类 以空格分隔
    /// </summary>
    public string NoEffectString { get; set; }

    /// <summary>
    /// 选是的建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
    /// </summary>
    public string YesArchitectureEffectString { get; set; }

    /// <summary>
    /// 选否的建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
    /// </summary>
    public string NoArchitectureEffectString { get; set; }

    /// <summary>
    /// 武将列传 先指定第k个武将，后跟随一段武将列传 以空格分隔 可使用%k表示第k个武将的姓名
    /// </summary>
    public string ScenBiographyString { get; set; }

    /// <summary>
    /// 下一剧本，暂时无用
    /// </summary>
    public string NextScenario { get; set; }

    public string TryToShowString { get; set; }
}