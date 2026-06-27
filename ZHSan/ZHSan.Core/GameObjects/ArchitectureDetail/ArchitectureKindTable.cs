using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail;
/// <summary>
/// 建筑类型表
/// </summary>
[DataContract]
public class ArchitectureKindTable
{
    [DataMember]
    public Dictionary<int, ArchitectureKind> ArchitectureKinds = new();

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="architectureKind"></param>
    /// <returns></returns>
    public bool Add(ArchitectureKind architectureKind)
    {
        return ArchitectureKinds.TryAdd(architectureKind.ID, architectureKind);
    }

    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ArchitectureKind Get(int id)
    {
        ArchitectureKinds.TryGetValue(id, out var architectureKind);

        return architectureKind;
    }

    public void Clear() => ArchitectureKinds.Clear();

    /// <summary>
    /// 获取建筑类型列表
    /// </summary>
    /// <returns></returns>
    public GameObjectList GetArchitectureKindList() => [.. ArchitectureKinds.Values];
}