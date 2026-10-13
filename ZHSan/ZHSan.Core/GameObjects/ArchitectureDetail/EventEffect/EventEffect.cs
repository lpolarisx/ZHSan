using GameDatas;

namespace GameObjects.ArchitectureDetail.EventEffect;

/// <summary>
/// 事件影响
/// </summary>
public class EventEffect : GameObject
{
    #region DataMember

    /// <summary>
    /// 事件影响类型
    /// </summary>
    public EventEffectKind Kind;

    public int KindId { get; set; }

    private string parameter;
    private int? intParameter;

    /// <summary>
    /// 参数1
    /// </summary>
    public string Parameter
    {
        get => parameter;
        set
        {
            parameter = value;
            intParameter = null;
        }
    }

    private string parameter2;
    private int? intParameter2;
    
    /// <summary>
    /// 参数2
    /// </summary>
    public string Parameter2
    {
        get => parameter2;
        set
        {
            parameter2 = value;
            intParameter2 = null;
        }
    }

    #endregion

    public EventEffect(ArchitectureEventEffectConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        KindId = config.KindId;
        Parameter = config.Parameter;
        Parameter2 = config.Parameter2;
    }

    /// <summary>
    /// 获取参数1解析的int值
    /// </summary>
    /// <returns></returns>
    public int GetIntParam()
    {
        if (!intParameter.HasValue)
        {
            intParameter = int.TryParse(parameter, out int v) ? v : 0;
        }
        return intParameter.Value;
    }

    /// <summary>
    /// 获取参数2解析的int值
    /// </summary>
    /// <returns></returns>
    public int GetIntParam2()
    {
        if (!intParameter2.HasValue)
        {
            intParameter2 = int.TryParse(parameter2, out int v) ? v : 0;
        }
        return intParameter2.Value;
    }

    public void ApplyEffect(Person person, Event e)
    {
        Kind.ApplyEffectKind(this, person, e);
    }

    public void ApplyEffect(Architecture arch, Event e)
    {
        Kind.ApplyEffectKind(this, arch, e);
    }

    public void ApplyEffect(Faction faction, Event e)
    {
        Kind.ApplyEffectKind(this, faction, e);
    }
}