using GameDatas;

namespace GameObjects.ArchitectureDetail;

/// <summary>
/// 资源种类
/// </summary>
public class ResourceKind : GameObject
{
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    public ResourceKind() {}

    public ResourceKind(ResourceKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Description = config.Description;
    }
}
