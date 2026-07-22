using GameDatas;

namespace GameObjects.TroopDetail.EventEffect;

/// <summary>
/// 部队事件影响
/// </summary>
public class EventEffect : GameObject
{
    #region DataMember

    /// <summary>
    /// 部队事件影响类型
    /// </summary>
    public EventEffectKind Kind { get; set; }

    public int KindId { get; set; }

    private string parameter;
    private int? intParameter;
    private float? floatParameter;

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
            floatParameter = null;
        }
    }

    #endregion

    public EventEffect(TroopEventEffectConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        KindId = config.KindId;
        Parameter = config.Parameter;
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
        /// 获取参数1解析的float值
        /// </summary>
        /// <returns></returns>
        public float GetFloatParam()
        {
            if (!floatParameter.HasValue)
            {
                floatParameter = float.TryParse(parameter, out float v) ? v : 0;
            }
            return floatParameter.Value;
        }

    public void ApplyEffect(Person person)
    {
        Kind.ApplyEffectKind(this, person);
    }
}